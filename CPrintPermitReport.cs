using System;
using System.IO;
using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Windows.Forms;
using SQL_Library;

namespace WasteDisposalPermits
{
    class CPrintReport : ExcelClass
    {
        //****************************************************************************************************************************
        public CPrintReport()
        {
        }
        //****************************************************************************************************************************
        public void PrintReport(DataTable tbl)
        {
            SubmitButton Submit = new SubmitButton("Permit Type ('H', 'F', 'L', 'R' or 'C')", "");
            Submit.ShowDialog();
            char cPermitType = Submit.GetPermitType();
            if (cPermitType == '0')
            {
                return;
            }
            if (cPermitType != 'R' && cPermitType != 'F' && cPermitType != 'C' && cPermitType != 'L' && cPermitType != 'H')
            {
                if (cPermitType != ' ' && cPermitType != '0')
                    MessageBox.Show("Invalid Permit Type");
                return;
            }
            CreateNewExcelWorkbook("Permit Report" + " " + cPermitType);
            WriteLabelHeader(cPermitType);
            int rowIndex = 1;
            foreach (DataRow row in tbl.Rows)
            {
                char PermitType = row[U.PermitType_col].ToString()[0];
                char Status = row[U.Status_col].ToString()[0];
                if (Status != 'I' && PermitType == cPermitType)
                {
                    rowIndex++;
                    PrintLabel(row, rowIndex, cPermitType);
                }
            }
            CloseOutput();
            MessageBox.Show("Report Complete");
        }
        //****************************************************************************************************************************
        protected string GetFileNameFromPath(string sFileNameWithPath)
        {
            char[] c = new char[1];
            c[0] = '\\';
            int iIndexOfLastBackslash = sFileNameWithPath.LastIndexOfAny(c);
            return sFileNameWithPath.Substring(iIndexOfLastBackslash + 1);
        }
        //****************************************************************************************************************************
        public void WriteHeaderFile()
        {
            outputWorksheet.Cells[1, 1].Value = "Lastname";
            outputWorksheet.Cells[1, 2].Value = "Firstname";
            outputWorksheet.Cells[1, 3].Value = "Num Cards";
            outputWorksheet.Cells[1, 4].Value = "Permit Number";
        }
        //****************************************************************************************************************************
        public void WriteOldHeaderFile()
        {
            outputWorksheet.Cells[1, 1].Value = "Permit Number";
            outputWorksheet.Cells[1, 2].Value = "Name";
            outputWorksheet.Cells[1, 3].Value = "911 Address";
            outputWorksheet.Cells[1, 4].Value = "Permit Type";
            outputWorksheet.Cells[1, 5].Value = "Number Cards";
        }
        //****************************************************************************************************************************
        private string RemoveAmpersand(string inString)
        {
            inString = inString.Replace("&", " and ");
            inString = inString.Replace(" &", " and");
            inString = inString.Replace("&", "and");
            return inString;
        }
        //****************************************************************************************************************************
        private void GetCaretakerInfo(int iCareTakerID, int rowIndex)
        {
            string sAddress = "";
            string sName = "";
            string sTown = "Jamaica";
            string sZipCode = "05343";
            DataTable Caretaker_tbl = new DataTable();
            if (SQL.GetCareTaker(Caretaker_tbl, iCareTakerID))
            {
                DataRow CatetakerRow = Caretaker_tbl.Rows[0];
                int iStreetNum = CatetakerRow[U.StreetNum_col].ToInt();
                if (iStreetNum != 0)
                    sAddress = iStreetNum.ToString() + " ";
                sAddress += CatetakerRow[U.StreetName_col].ToString();
                sName = CatetakerRow[U.CaretakerName_col].ToString();
                sTown = CatetakerRow[U.Town_col].ToString();
                if (sTown[0] == 'L')
                    sZipCode = "05148";
                else
                    if (sTown[0] == 'S')
                    sZipCode = "05155";
                else
                        if (sTown[0] == 'W')
                    sZipCode = "05355";
            }
            outputWorksheet.Cells[rowIndex, 1].Value = sName;
            outputWorksheet.Cells[rowIndex, 2].Value = sAddress;
            outputWorksheet.Cells[rowIndex, 3].Value = sTown + ", Vermont " + sZipCode;
        }
        //****************************************************************************************************************************
        private void PrintInfo(DataRow row,
                               int rowIndex,
                               char cPermitType,
                               int iNumPermits)
        {
            string sAddressNum = "";
            string sAddress = "";
            string sName = "";
            string sFirstName = "";
            string sLastName = "";
            DataTable GrandList_tbl = SQL.GetGrandListPropertyByGrandListID(row[U.GrandListID_col].ToInt());
            if (GrandList_tbl.Rows.Count == 0)
            {
                //MessageBox.Show("Unable to locate grandlist record");
                return;
            }
            DataRow GrandList_row = GrandList_tbl.Rows[0];
            sAddressNum = GrandList_row[U.StreetNum_col].ToString() + " ";
            sAddress = GrandList_row[U.StreetName_col].ToString();
            outputWorksheet.Cells[rowIndex, 1].Value = row[U.PermitNumber_col].ToString();
            if (cPermitType == 'L')
            {
                sName = row[U.Apartment_col].ToString();
                if (sName.Length == 0)
                {
                    sFirstName = RemoveAmpersand(row[U.FirstName_col].ToString());
                    sLastName = RemoveAmpersand(row[U.LastName_col].ToString());
                    sName = sFirstName + " " + sLastName;
                }
                outputWorksheet.Cells[rowIndex, 2].Value = sName;
            }
            else
            if (sName.Length == 0)
            {
                sFirstName = RemoveAmpersand(row[U.FirstName_col].ToString());
                sName = sFirstName;
                if (sName.Length > 0)
                    sName += " ";
                sLastName = RemoveAmpersand(row[U.LastName_col].ToString());
                sName += sLastName;
                outputWorksheet.Cells[rowIndex, 2].Value = sLastName;
                outputWorksheet.Cells[rowIndex, 3].Value = sFirstName;
            }
            else
            {
                outputWorksheet.Cells[rowIndex, 2].Value = sName;
            }
            string address = String.IsNullOrEmpty(sAddressNum) ? "" : sAddressNum + " ";
            address += sAddress + " " + row[U.Apartment_col].ToString();
            if (cPermitType == 'R')
            {
                outputWorksheet.Cells[rowIndex, 4].Value = address.Trim();
            }
            else
            {
                outputWorksheet.Cells[rowIndex, 4].Value = address;
                if (cPermitType == 'H')
                {
                    outputWorksheet.Cells[rowIndex, 5].Value = iNumPermits.ToString();
                    if (GrandList_row[U.City_col].ToString().ToLower() != "jamaica")
                    {
                        outputWorksheet.Cells[rowIndex, 6].Value = "*";
                    }
                }
                else
                {
                    outputWorksheet.Cells[rowIndex, 5].Value = iNumPermits.ToString();
                }
            }
            //outputWorksheet.Cells[rowIndex, 6].Value = sName;
            //outputWorksheet.Cells[rowIndex, 6].Value = sAddress;
            //outputWorksheet.Cells[rowIndex, 6].Value = sTown + ", Vermont " + sZipCode;
        }
        //****************************************************************************************************************************
        private void PrintLabel(DataRow row,
                                int rowIndex,
                               char cPermitType)
        {
            string sPermitNumber = row[U.PermitNumber_col].ToString();
            int iNumPermits = row[U.NumberCards_col].ToInt();

            int iCareTakerID = row[U.CareTakerID_col].ToInt();
            if (iCareTakerID != 0)
                GetCaretakerInfo(iCareTakerID, rowIndex);
            else
                PrintInfo(row, rowIndex, cPermitType, iNumPermits);
        }
        //****************************************************************************************************************************
        public void WriteOldPrintFile(DataRow row,
                               string sAddress)
        {
            string sPermitNumber = row[U.PermitNumber_col].ToString();
            string sName = row[U.FirstName_col].ToString() + " " + row[U.LastName_col].ToString();
            char PermitType = row[U.PermitType_col].ToString()[0];
            int iNumPermits = row[U.NumberCards_col].ToInt();
            string sPermitType;
            if (PermitType == 'R'/*Renter*/)
            {
                sPermitType = "Qualified Household";
                string sApartment = row[U.Apartment_col].ToString();
                if (sApartment.Length != 0)
                    sAddress += ("-" + sApartment);
            }
            else if (PermitType == 'L'/*Low Impact Business*/)
            {
                sPermitType = "Low Impact Business";
                string sApartment = row[U.Apartment_col].ToString();
                if (sApartment.Length != 0)
                    sPermitType += ("-" + row[U.Apartment_col].ToString());
            }
            else if (PermitType == 'C'/*Caretaker*/)
            {
                sPermitType = "Care-Taker";
            }
            else /*HomeOwner*/
            {
                sPermitType = "Qualified Household";
            }
            outputWorksheet.Cells[1, 1].Value = sPermitNumber;
            outputWorksheet.Cells[1, 2].Value = sName;
            outputWorksheet.Cells[1, 3].Value = sAddress;
            outputWorksheet.Cells[1, 4].Value = sPermitType;
            outputWorksheet.Cells[1, 5].Value = iNumPermits.ToString();
        }
        //****************************************************************************************************************************
        private void FormatErrorMessage(string exe,
                                          string systemMessage)
        {
            string sErrorMessage = "Error trying to call Card Five: " + exe + "\n" + systemMessage;
            MessageBox.Show(sErrorMessage);
        }
        //****************************************************************************************************************************
        public void Execute(string exe, string args)
        {
            ProcessStartInfo sqlCmdStartInfo = null;
            Process sqlCmdProcess = null;
            try
            {
                sqlCmdStartInfo = new ProcessStartInfo();
                sqlCmdStartInfo.CreateNoWindow = true;
                sqlCmdStartInfo.ErrorDialog = false;
                sqlCmdStartInfo.RedirectStandardError = true;
                sqlCmdStartInfo.RedirectStandardOutput = false;
                sqlCmdStartInfo.UseShellExecute = false;

                sqlCmdStartInfo.FileName = exe;
                sqlCmdStartInfo.Arguments = args;

                sqlCmdProcess = new Process();
                sqlCmdProcess.StartInfo = sqlCmdStartInfo;

                sqlCmdProcess.Start();
                sqlCmdProcess.WaitForExit();

                string errorMessage = sqlCmdProcess.StandardError.ReadToEnd();

                if (errorMessage != string.Empty)
                {
                    FormatErrorMessage(exe, errorMessage);
                }
            }
            catch (Exception ex)
            {
                FormatErrorMessage(exe, ex.Message);
            }
        }
        //****************************************************************************************************************************
        private void WriteLabelHeader(char cPermitType)
        {
            switch (cPermitType)
            {
                case 'H':
                    outputWorksheet.Cells[1, 2].Value = "Home Owner";
                    break;
                case 'L':
                    outputWorksheet.Cells[1, 2].Value = "Low Impact Business";
                    break;
                case 'C':
                    outputWorksheet.Cells[1, 2].Value = "Caretaker";
                    break;
                case 'R':
                    outputWorksheet.Cells[1, 2].Value = "Renter";
                    break;
                default:
                    return;
            }
            outputWorksheet.Cells[1, 4].Value = "Jamaica Address";
            outputWorksheet.Cells[1, 5].Value = "Num Perits";
        }
        /*
        //****************************************************************************************************************************
        public void PrintStickerReport(ArrayList permitsToBecomeInactive)
        {
            CreateNewExcelWorkbook("Stirker Report");
            WriteStrickerHeader();
            foreach (SDifferentProperties row in permitsToBecomeInactive)
            {
                WriteStickerRow(row);
            }
            CloseOutput();
            string sExecutable = @"C:\Program Files\Microsoft Office15\root\Office15\EXCEL.exe";
            Execute(sExecutable, m_sFileNameWithPath_xls);
        }
        //****************************************************************************************************************************
        private void WriteStickerRow(SDifferentProperties row)
        {
            outputWorksheet.Cells[1, 1].Value = row.propertyID + "." + row.propertySubID;
            outputWorksheet.Cells[1, 2].Value = row.name1;
            outputWorksheet.Cells[1, 3].Value = row.name2;
            outputWorksheet.Cells[1, 4].Value = row.name3;
            outputWorksheet.Cells[1, 5].Value = row.Address;
            outputWorksheet.Cells[1, 6].Value = row.Address;
        }
        //****************************************************************************************************************************
        private void WriteStrickerHeader()
        {
            outputWorksheet.Cells[1, 1].Value = "Property ID";
            outputWorksheet.Cells[1, 2].Value = "Previous Owner";
            outputWorksheet.Cells[1, 3].Value = "New Owner";
            outputWorksheet.Cells[1, 4].Value = "Permit Holder";
            outputWorksheet.Cells[1, 5].Value = "911 address";
            outputWorksheet.Cells[1, 6].Value = "Street";
        }
        //****************************************************************************************************************************
        public void PrintOldReport(CSql sql,
                               DataTable tbl)
        {
            CreateNewExcelWorkbook("Permit Report");
            WriteOldHeaderFile();
            foreach (DataRow row in tbl.Rows)
            {
                int iCareTakerID = row[U.CareTakerID_col].ToInt();
                string sAddress = "";
                if (iCareTakerID != 0)
                    sAddress = row[U.Apartment_col].ToString();
                else
                {
                    DataTable GrandList_tbl = SQL.GetGrandListPropertyByGrandListID(
                                                      row[U.GrandListIDChar_col].ToString());
                    if (GrandList_tbl.Rows.Count != 0)
                    {
                        DataRow GrandList_row = GrandList_tbl.Rows[0];
                        sAddress = GrandList_row[U.StreetNum_col].ToString() + " " +
                                          GrandList_row[U.StreetName_col].ToString();
                    }
                }
                WriteOldPrintFile(row, sAddress);
            }
            CloseOutput();
            //                string sExecutable = @"C:\Program Files\Microsoft Office\Office12\EXCEL.exe";
            //                Execute(sExecutable, m_sFileNameWithPath);
        }
        */
        //****************************************************************************************************************************
        public void PrintPermitErrorsReport(ArrayList printReport)
        {
            CreateNewExcelWorkbook("PermitErrors");
            int rowIndex = 1;
            PrintPermitErrorsHeaders();
            foreach (SQL.GrandListNames permitName in printReport)
            {
                rowIndex++;
                outputWorksheet.Cells[rowIndex, 1].Value = permitName.span;
                outputWorksheet.Cells[rowIndex, 2].Value = permitName.grandlistId;
                outputWorksheet.Cells[rowIndex, 3].Value = permitName.permitNumber;
                outputWorksheet.Cells[rowIndex, 4].Value = permitName.permitLastName;
                outputWorksheet.Cells[rowIndex, 5].Value = permitName.permitFirstName;
                outputWorksheet.Cells[rowIndex, 6].Value = permitName.name1;
                outputWorksheet.Cells[rowIndex, 7].Value = permitName.name2;
                outputWorksheet.Cells[rowIndex, 8].Value = permitName.status.ToString();
                outputWorksheet.Cells[rowIndex, 9].Value = permitName.permitType.ToString();
                outputWorksheet.Cells[rowIndex, 10].Value = permitName.streetNum + " " + permitName.streetName;
            }
            CloseOutput();
        }
        //****************************************************************************************************************************
        public void PrintPermitErrorsHeaders()
        {
            outputWorksheet.Cells[1, 1].Value = "Span";
            outputWorksheet.Cells[1, 2].Value = "Grand List Id";
            outputWorksheet.Cells[1, 3].Value = "Permit Id";
            outputWorksheet.Cells[1, 4].Value = "Permit Last Name";
            outputWorksheet.Cells[1, 5].Value = "Permit First Name";
            outputWorksheet.Cells[1, 6].Value = "Grand List Name1";
            outputWorksheet.Cells[1, 7].Value = "Grand List Name2";
            outputWorksheet.Cells[1, 8].Value = "Status";
            outputWorksheet.Cells[1, 9].Value = "Permit Type";
            outputWorksheet.Cells[1, 10].Value = "Address";
        }
        //****************************************************************************************************************************
        public void PrintGrandlistHistory(DataTable grandListTbl, DataTable grandListHistoryTbl)
        {
            CreateNewExcelWorkbook("GrandListHistory");
            try
            {
                int rowIndex = 1;
                PrintGrandlistHistoryHeaders();
                foreach (DataRow grandListHistoryRow in grandListHistoryTbl.Rows)
                {
                    rowIndex++;
                    string selectStatement = U.GrandListID_col + " = " + grandListHistoryRow[U.GrandListID_col].ToString();
                    DataRow[] foundRows = grandListTbl.Select(selectStatement);
                    if (foundRows.Length == 0 || foundRows.Length > 1)
                    {
                        throw new Exception("Unable to find grandlist Record: " + grandListHistoryRow[U.GrandListID_col].ToString());
                    }
                    outputWorksheet.Cells[rowIndex, 1].Value = grandListHistoryRow[U.GrandListID_col].ToString();
                    outputWorksheet.Cells[rowIndex, 2].Value = foundRows[0][U.Span_col].ToString();
                    outputWorksheet.Cells[rowIndex, 3].Value = foundRows[0][U.TaxMapID_col].ToString();
                    outputWorksheet.Cells[rowIndex, 4].Value = grandListHistoryRow[U.Year_col].ToString();
                    outputWorksheet.Cells[rowIndex, 5].Value = grandListHistoryRow[U.Name1_col].ToString();
                    outputWorksheet.Cells[rowIndex, 6].Value = grandListHistoryRow[U.Name2_col].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            CloseOutput();
        }
        //****************************************************************************************************************************
        public void PrintGrandlistHistoryHeaders()
        {
            outputWorksheet.Cells[1, 1].Value = "Grand List Id";
            outputWorksheet.Cells[1, 2].Value = "Span";
            outputWorksheet.Cells[1, 3].Value = "TaxMapId";
            outputWorksheet.Cells[1, 4].Value = "Year";
            outputWorksheet.Cells[1, 5].Value = "Name1";
            outputWorksheet.Cells[1, 6].Value = "Name2";
        }
    }
}