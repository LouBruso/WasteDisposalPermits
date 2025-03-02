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
    public struct SDifferentProperties
    {
        public string propertyID;
        public string propertySubID;
        public string name1;
        public string name2;
        public string name3;
        public string Address;
        public string Street;
    }

    public class CImportGrandList : CImport
    {
        private string m_sInputRecord;
        private ArrayList InactiveProperties = new ArrayList();
        //****************************************************************************************************************************
        private string Field(string[] sInputFields,
                             int iFieldNum)
        {
            if (sInputFields.Length > iFieldNum)
            {
                string sInputField = sInputFields[iFieldNum];
                const string sQuotesWithSlashBefore = @"""";
                sInputField = sInputField.Replace(sQuotesWithSlashBefore, "");
                return sInputField;
            }
            else
                return "";
        }
        //****************************************************************************************************************************
        private int GetNumCells(string[] sInputFields,
                               int iNum)
        {
            int iNumValues = 0;
            bool bDone = false;
            do
            {
                iNum++;
                string sCellValue = Field(sInputFields, iNum);
                if (sCellValue.Length == 0)
                    bDone = true;
                else
                    iNumValues++;
            }
            while (!bDone);
            return iNumValues;
        }
        //****************************************************************************************************************************
        public CImportGrandList()
        {
            string sFilter = "Tab Delimited Files (txt)|*.txt";
            if (!OpenInputFile(sFilter))
                return;
            m_sInputRecord = ReadRecord();
            if (m_sInputRecord != null)
                m_sInputRecord = ReadRecord();
            while (m_sInputRecord != null)
            {
                string[] sInputFields = m_sInputRecord.Split(U.Tab);
                if (Field(sInputFields, 0) == "")
                    break;
                string sPropertyID = Field(sInputFields, 0).Trim();
                string sPropertySubID = Field(sInputFields, 1).Trim();
                string sPropertyCombined = sPropertyID + "." + sPropertySubID;
                string sName1 = Field(sInputFields, 2).Trim();
                string sName2 = Field(sInputFields, 3).Trim();
                string sAddressA = Field(sInputFields, 4).Trim();
                string sAddressB = Field(sInputFields, 5).Trim();
                string sCity = Field(sInputFields, 6).Trim();
                string sState = Field(sInputFields, 7).Trim();
                string sZip = Field(sInputFields, 8).Trim();
                string sLoactionA = Field(sInputFields, 9).Trim();
                string sLoactionB = Field(sInputFields, 10).Trim();
                string sLoactionC = Field(sInputFields, 11).Trim();
                int i911Number = Field(sInputFields, 12).ToInt();
                string sAddress = Field(sInputFields, 13).Trim();
                string sDescription = Field(sInputFields, 14).Trim();
                string sWhereOwnerLiveID = Field(sInputFields, 15).Trim();
                DataTable tbl = SQL.GetGrandListPropertyByGrandListID(sPropertyID, sPropertySubID);
                if (tbl.Rows.Count > 1)
                {
                }
//                PatchGrandListID(sPropertyCombined, sPropertyID, sPropertyIDSub,
//                                 sAddress, i911Number, sName1, sName2);
                bool bSuccess = true;
                if (tbl.Rows.Count == 0)
                {
                    bSuccess = SQL.InsertGrandList(sPropertyID, sPropertySubID, sPropertyCombined, i911Number, sAddress, sName1, sName2,
                                    sAddressA, sAddressB, sCity, sState, sZip,
                                    sDescription, sLoactionA, sLoactionB, sLoactionC, sWhereOwnerLiveID);
                }
                else
                {
                    CheckForInactivePermit(tbl.Rows[0], sPropertyID, sPropertySubID, sPropertyCombined, sName1, i911Number, sAddress);
                    RowIsDifferent(tbl, sPropertyID, sPropertySubID, sPropertyCombined, sName1, sName2,
                                   sAddressA, sAddressB, sCity, sState, sZip, sLoactionA, sLoactionB, sLoactionC, i911Number, sAddress, sDescription);
                }
                if (!bSuccess)
                {
//                    MessageBox.Show(SQL.GetSQLErrorMessage());
                    break;
                }
                m_sInputRecord = ReadRecord();
            }
            CloseInputFile();
            CPrintReport PrintReport = new CPrintReport();
            PrintReport.PrintStickerReport(InactiveProperties);
        }

        private void CheckForInactivePermit(DataRow row,
                                            string sPropertyID,
                                            string sPropertySubID,
                                            string sPropertyCombined,
                                            string sName1,
                                            int i911Number,
                                            string sAddress)
        {
            if (sName1.Trim() != row[U.Name1_col].ToString().Trim())
            {
                DataTable PermitTable = SQL.GetPermitByGrandListID(row[U.GrandListIDChar_col].ToString());
                string newLastName = GetLastName(sName1);
                string rowLstName = GetLastName(row[U.Name1_col].ToString());
                SetPermitToInactive(PermitTable, sPropertyID, sPropertySubID, newLastName, rowLstName, i911Number, sAddress);
            }
        }

        private void SetPermitToInactive(DataTable PermitTable,
                                         string sPropertyID,
                                         string sPropertySubID,
                                         string newLastName,
                                         string rowLstName,
                                         int i911Number,
                                         string sAddress)
        {
            foreach (DataRow row in PermitTable.Rows)
            {
                if (DifferentLastName(newLastName, rowLstName, GetLastName(row[U.LastName_col].ToString())))
                {
                    if (row[U.Status_col].ToChar() != 'I')
                    {
                        ArrayList CaretakerFieldsModified = null;
                        DataTable CaretakerTable = SQL.DefineCareTakerTable();
                        ArrayList FieldsModified = new ArrayList();
                        FieldsModified.Add(U.Status_col);
                        row[U.Status_col] = 'I';
                        SQL.UpdatePermit(PermitTable, CaretakerTable, FieldsModified, CaretakerFieldsModified, false, 0);
                        SDifferentProperties diffProps;
                        diffProps.propertyID = sPropertyID;
                        diffProps.propertySubID = sPropertySubID;
                        diffProps.name1 = rowLstName;
                        diffProps.name2 = newLastName;
                        diffProps.name3 = PermitTable.Rows[0][U.LastName_col].ToString();
                        diffProps.Address = i911Number.ToString();
                        diffProps.Street = sAddress;
                        InactiveProperties.Add(diffProps);
                    }
                }
            }
        }

        private void RowIsDifferent(DataTable tbl,
            string sPropertyID,
            string sPropertySubID,
            string sPropertyCombined,
            string sName1,
            string sName2,
            string sAddressA,
            string sAddressB,
            string sCity,
            string sState,
            string sZip,
            string sLoactionA,
            string sLoactionB,
            string sLoactionC,
            int i911Number,
            string sAddress,
            string sDescription)
        {
            DataRow row = tbl.Rows[0];
            ArrayList FieldsModified = new ArrayList();
            FieldIsDifferent(FieldsModified, row, sName1, U.Name1_col);
            FieldIsDifferent(FieldsModified, row, sName2, U.Name2_col);
            FieldIsDifferent(FieldsModified, row, sAddressA, U.AddressA_col);
            FieldIsDifferent(FieldsModified, row, sAddressB, U.AddressB_col);
            FieldIsDifferent(FieldsModified, row, sCity, U.City_col);
            FieldIsDifferent(FieldsModified, row, sState, U.State_col);
            FieldIsDifferent(FieldsModified, row, sZip, U.Zip_col);
            FieldIsDifferent(FieldsModified, row, sLoactionA, U.LoactionA_col);
            FieldIsDifferent(FieldsModified, row, sLoactionB, U.LoactionB_col);
            FieldIsDifferent(FieldsModified, row, sLoactionC, U.LoactionC_col);
            FieldIsDifferent(FieldsModified, row, i911Number.ToString(), U.StreetNum_col);
            FieldIsDifferent(FieldsModified, row, sAddress, U.StreetName_col);
            FieldIsDifferent(FieldsModified, row, sDescription, U.Description_col);
            if (FieldsModified.Count != 0)
            {
                SQL.UpdateGrandlist(tbl, FieldsModified);
            }
        }
        //****************************************************************************************************************************
        private bool DifferentLastName(string newLastName,
                                       string rowLastName,
                                       string permitLastName)
        {
            if (newLastName.ToLower() == rowLastName.ToLower())
                return false;
            return rowLastName.Trim().ToLower() == permitLastName.ToLower();
        }
        //****************************************************************************************************************************
        private string GetLastName(string name)
        {
            int indexOfSpace = name.IndexOf(' ');
            if (indexOfSpace == 1)
            {
                string s = "'";
                char[] arr = name.ToCharArray();
                arr[1] = s[0];
                name = new string(arr);
                indexOfSpace = name.IndexOf(' ');
            }
            int indexOfComma = name.IndexOf(',');
            int indexOfSemicolin = name.IndexOf(';');
            if (indexOfSpace < 0)
            {
                if (indexOfComma > 0)
                    indexOfSpace = indexOfComma;
                else
                    indexOfSpace = indexOfSemicolin;
            }
            if (indexOfComma > 0 && indexOfComma < indexOfSpace)
                indexOfSpace = indexOfComma;
            if (indexOfSemicolin > 0 && indexOfSemicolin < indexOfSpace)
                indexOfSpace = indexOfSemicolin;
            if (indexOfSpace < 0)
                return name.Trim();
            else
                return name.Substring(0, indexOfSpace).Trim();
        }
        //****************************************************************************************************************************
        private void FieldIsDifferent(ArrayList FieldsModified, 
                                      DataRow row,
                                      string NewValue,
                                      string OldValue_col)
        {
            if (NewValue.Trim() != row[OldValue_col].ToString().Trim())
            {
                row[OldValue_col] = NewValue;
                FieldsModified.Add(OldValue_col);
            }
        }
        //****************************************************************************************************************************
        private void PatchGrandListID(string sGrandListID,
                                      string sPropertyID,
                                      string sPropertyIDSub,
                                      string sAddress,
                                      int i911Number,
                                      string sName1,
                                      string sName2)
        {
            string sNewGrandListID = sPropertyID + '.' + sPropertyIDSub;
            if (sGrandListID != sNewGrandListID)
            {
                DataTable tbl = SQL.GetGrandListProperties(i911Number, sAddress);
                foreach (DataRow row in tbl.Rows)
                {
                    string sOldName1 = row[U.Name1_col].ToString();
                    string sOldName2 = row[U.Name2_col].ToString();
                    if (sName1 == sOldName1 && sName2 == sOldName2)
                    {
                        row[U.GrandListIDChar_col] = sNewGrandListID;
                    }
                }
            }
        }
        //****************************************************************************************************************************
    }
}
