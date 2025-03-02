using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Data;
using System.Windows.Forms;
using SQL_Library;

namespace WasteDisposalPermits
{
    public class CImportGrandList
    {
        private EPPlus epPlus;
        private ArrayList spanList = new ArrayList();
        private ArrayList spanDeletes = new ArrayList();
        private ArrayList SpanInNewFile = new ArrayList();
        private DataTable grandListTbl;
        private DataTable grandListHistoryTbl;
        private DataTable wasteDisposalPermitsTbl = new DataTable();
        private int historyYear = 9999;
        //****************************************************************************************************************************
        public CImportGrandList()
        {
            try
            {
                using (epPlus = new EPPlus())
                {
                    grandListHistoryTbl = SQL.DefineGrandListHistoryTable();
                    grandListTbl = SQL.GetAllGrandList();
                    SQL.GetAllPermits(wasteDisposalPermitsTbl);
                    string filename = GetInputFile("Actives");
                    if (historyYear != 0)
                    {
                        grandListHistoryTbl = SQL.GetAllGrandListHistory();
                    }
                    if (!string.IsNullOrEmpty(filename))
                    {
                        GetGrandListRecords(filename, 'A'); // actives
                    }
                    filename = GetInputFile("Inactives");
                    if (!string.IsNullOrEmpty(filename))
                    {
                        GetGrandListRecords(filename, 'I'); // inactives
                    }
                    if (historyYear == 0)
                    {
                        CheckAllInactiveGrandlistRecords(grandListTbl);
                        SQL.UpdateInsertDeleteGrandList(grandListTbl);
                    }
                    else
                    {
                        SQL.InsertGrandListHistory(grandListHistoryTbl);
                    }
                    MessageBox.Show("Grand List Import Complete. Check for Affixes");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //****************************************************************************************************************************
        private string GetInputFile(string fileType)
        {
            string filename;
            //epPlus.GetExcelInputFile(@"c:\Reports\", out filename);
            epPlus.GetExcelInputFile(@"X:\ListersInformation\Reports\", out filename);
            string filenameWithoutExtension = Path.GetFileNameWithoutExtension(filename);
            int lengthOfFilename = filenameWithoutExtension.Length;
            int indexOf = filenameWithoutExtension.IndexOf(fileType);
            int lengthOfFilenamePlusFiletype = indexOf + fileType.Length;
            if (filenameWithoutExtension.Length <= lengthOfFilenamePlusFiletype)
            {
                CheckWithPreviousHistoryYear(0);
                return filename;
            }
            int dateLength = filenameWithoutExtension.Length - lengthOfFilenamePlusFiletype;
            if (dateLength != 4)
            {
                throw new Exception("Filename contains invalid date: " + filenameWithoutExtension);
            }
            int newHistoryYear = Convert.ToInt32(filenameWithoutExtension.Substring(lengthOfFilenamePlusFiletype));
            CheckWithPreviousHistoryYear(newHistoryYear);
            return filename;
        }
        //****************************************************************************************************************************
        private void CheckWithPreviousHistoryYear(int newHistoryYear)
        {
            if (historyYear == 9999)
            {
                historyYear = newHistoryYear;
                return;
            }
            if (historyYear != newHistoryYear)
            {
                throw new Exception("New History Year does not match previous History Year: " + newHistoryYear + "-" + historyYear);
            }
        }
        //****************************************************************************************************************************
        private void GetGrandListRecords(string filename, char activeStatus)
        {
            epPlus.OpenWithEPPlus(filename);
            if (string.IsNullOrEmpty(filename))
            {
                return;
            }
            int rowIndex = 2;
            while (rowIndex <= epPlus.numRows)
            {
                try
                {
                    AddRecordToDatabase(rowIndex, activeStatus);
                    rowIndex++;
                }
                catch (Exception ex)
                {
                    string message = "Row: " + rowIndex + " - " + ex.Message;
                    throw new Exception(message);
                }
            }
        }
        private int counter = 0;
        //****************************************************************************************************************************
        private void AddRecordToDatabase(int rowIndex, char activeStatus)
        {
            if (rowIndex == 1359)
            {
            }
            CNemrcExtract nemrcExtract = new CNemrcExtract(epPlus, rowIndex);
            string selectStatement = U.Span_col + " = '" + nemrcExtract.Span + "'";
            DataRow[] foundRows = grandListTbl.Select(selectStatement);
            if (ExcludedProperty(nemrcExtract.Name1, nemrcExtract.Name2, nemrcExtract.TaxMapID, nemrcExtract.Owner))
            {
                if (foundRows.Length == 1 && historyYear == 0)
                {
                    DeleteGrandListRecordIfNotUsed(foundRows[0]);
                }
                return;
            }
            if (foundRows.Length == 1)
            {
                if (historyYear != 0)
                {
                    if (nemrcExtract.AddGrandListRecordToHistoryIfDifferent(grandListHistoryTbl, foundRows[0], historyYear))
                    {
                        counter++;
                    }
                }
                else
                {
                    nemrcExtract.UpdateExistingGrandListRecord(foundRows[0], activeStatus);
                }
            }
            else if (historyYear == 0)
            {
                DataRow grandListNewRow = grandListTbl.NewRow();
                nemrcExtract.CreateNewGrandListRecord(grandListNewRow, activeStatus);
                grandListTbl.Rows.Add(grandListNewRow);
            }
            spanList.Add(nemrcExtract.Span);
        }
        //****************************************************************************************************************************
        private void DeleteGrandListRecordIfNotUsed(DataRow grandListRow)
        {
            string selectStatement = U.GrandListID_col + "=" + grandListRow[U.GrandListID_col].ToInt();
            DataRow[] foundRows = wasteDisposalPermitsTbl.Select(selectStatement);
            if (foundRows.Length != 0)
            {
                string span = grandListRow[U.Span_col].ToString();
                SpanInNewFile.Add(span);
                spanDeletes.Add(span);
                //MessageBox.Show("Grand List Id to be deleted used in permit: ", span);
            }
            else
            {
                grandListRow.Delete();
            }
        }
        //****************************************************************************************************************************
        private void CheckAllInactiveGrandlistRecords(DataTable grandListTbl)
        {
            foreach (DataRow grandListRow in grandListTbl.Rows)
            {
                if (grandListRow.RowState != DataRowState.Deleted)
                {
                    string grandListId = grandListRow[U.GrandListID_col].ToString();
                    string span = grandListRow[U.Span_col].ToString();
                    if (!String.IsNullOrEmpty(span) && !SpanInList(span))
                    {
                        DeleteGrandListRecordIfNotUsed(grandListRow);
                    }
                }
            }
        }
        //****************************************************************************************************************************
        private bool SpanInList(string span)
        {
            foreach (string spanInList in spanList)
            {
                if (span == spanInList)
                {
                    return true;
                }
                else
                {

                }
            }
            return false;
        }
        //****************************************************************************************************************************
        private bool ExcludedProperty(string name1, string name2, string taxMapId, string Owner)
        {
            if (name1.ToUpper().Contains("AGENCY OF TRANSPORT") || name2.ToUpper().Contains("AGENCY OF TRANSPORT"))
            {
                return true;
            }
            if (Owner.Length == 0) // Utility
            {
                return true;
            }
            return false;
        }
        //****************************************************************************************************************************
    }
}
