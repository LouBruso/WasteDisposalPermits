using System;
using System.IO;
using System.Diagnostics;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SQL_Library;
using Utilities;

namespace WasteDisposalPermits
{
    class CPrintCards
    {
        private StreamWriter m_StreamWriter;
        private const string TabChar = "\t";
        public CPrintCards()
        {
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
        public bool OpenOutputFile()
        {
            //string sFilter = "Tab Delimited Files (txt)|*.txt";
            string sFileNameWithPath = @"C:\WasteDisposalPermits\PermitsPrintFile.txt";// UU.SelectFile(sFilter, "c:\\WasteDisposalPermits");
            if (sFileNameWithPath.Length == 0)
            {
                return false;
            }
            string sFileName = GetFileNameFromPath(sFileNameWithPath);
            try
            {
                m_StreamWriter = new StreamWriter(sFileNameWithPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            return true;
        }
        //****************************************************************************************************************************
        public void CloseOutputFile()
        {
            m_StreamWriter.Close();
        }
        //****************************************************************************************************************************
        public void WriteHeaderFile()
        {
            m_StreamWriter.Write("Permit Number");
            m_StreamWriter.Write(TabChar);
            m_StreamWriter.Write("Name");
            m_StreamWriter.Write(TabChar);
            m_StreamWriter.Write("911 Address");
            m_StreamWriter.Write(TabChar);
            m_StreamWriter.WriteLine("Permit Type");
        }
        //****************************************************************************************************************************
        public void WritePrintFile(DataRow row,
                               int iCardNumber,
                               string sAddress)
        {
            int iFirstCard = iCardNumber;
            int iLastCard = iCardNumber;
            if (iCardNumber == 99)
            {
                iFirstCard = 1;
                iLastCard = row[U.NumberCards_col].ToInt();
            }
            string sPermitNumber = row[U.PermitNumber_col].ToString() + "-";
            char PermitType = row[U.PermitType_col].ToString()[0];
            string sName = ParseName(row, PermitType).Trim();
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
            for (int iCardNum = iFirstCard; iCardNum <= iLastCard; ++iCardNum)
            {
                m_StreamWriter.Write(sPermitNumber + iCardNum.ToString("D2"));
                m_StreamWriter.Write(TabChar);
                m_StreamWriter.Write(sName);
                m_StreamWriter.Write(TabChar);
                m_StreamWriter.Write(sAddress);
                m_StreamWriter.Write(TabChar);
                m_StreamWriter.WriteLine(sPermitType);
            }
        }
        //****************************************************************************************************************************
        private string ParseName(DataRow row, char PermitType)
        {
            if (PermitType == 'L'/*Low Impact Business*/)
            {
                string name = row[U.FirstName_col].ToString().Trim();
                if (name.ToLower().Contains("jamaica"))
                {
                    name = "";
                }
                return row[U.LastName_col].ToString().Trim() + " " + name;
            }
            string firstName = row[U.FirstName_col].ToString().Trim();
            int indexof = firstName.IndexOf('&');
            if (indexof <= 0)
            {
                return row[U.FirstName_col].ToString().Trim() + " " + row[U.LastName_col].ToString();
            }
            string otherName = firstName.Substring(indexof + 1).Trim();
            firstName = firstName.Substring(0, indexof - 1).Trim();
            indexof = otherName.IndexOf(' ');
            if (indexof < 0)
            {
                return row[U.FirstName_col].ToString() + " " + row[U.LastName_col].ToString();
            }
            return firstName + " " + row[U.LastName_col].ToString() + " & " + otherName;
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
                    FormatErrorMessage(exe,errorMessage);
                }
            }
            catch (Exception ex)
            {
                FormatErrorMessage(exe, ex.Message);
            }
        }
        //****************************************************************************************************************************
        public void PrintCards(DataTable tbl,
                               int iCardNumber)
        {
            if (OpenOutputFile())
            {
                WriteHeaderFile();
                foreach (DataRow row in tbl.Rows)
                {
                    int iCareTakerID = row[U.CareTakerID_col].ToInt();
                    string sAddress = "";
                    if (iCareTakerID != 0)
                        sAddress = row[U.Apartment_col].ToString();
                    else
                    {
                        DataTable GrandList_tbl = SQL.GetGrandListPropertyByGrandListID(
                                                          row[U.GrandListID_col].ToInt());
                        if (GrandList_tbl.Rows.Count != 0)
                        {
                            DataRow GrandList_row = GrandList_tbl.Rows[0];
                            sAddress = GrandList_row[U.StreetNum_col].ToString() + " " +
                                              GrandList_row[U.StreetName_col].ToString();
                        }
                    }
                    WritePrintFile(row, iCardNumber, sAddress);
                }
                CloseOutputFile();
                string sCardFiveFile = @"c:\WasteDisposalPermits\Permits.car";
                string sExecutable = @"C:\Program Files (x86)\Number Five\CardFive Vision\Cardfive.exe";
                Execute(sExecutable, sCardFiveFile);
            }
        }
    }
}
