using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using SQL_Library;
using Utilities;

namespace WasteDisposalPermits
{
    public abstract class CImport
    {
        private const string databasename = "DataBase=HistoricJamaicaCopy;";
        private string m_sDataDirectory = "";
        private TextReader m_tr;
        protected DataTable m_Person_tbl;
        protected DataTable m_Marriage_tbl;
        protected DataTable m_Cemetery_tbl;
        protected string m_sFileName;
        protected CSql SQLCopy = null;
        //****************************************************************************************************************************
        public CImport()
        {
            m_sDataDirectory = SQL.DataDirectory();
        }
        //****************************************************************************************************************************
        protected bool SetupCopySQLDatabase()
        {
            string sDataDirectory = "";
            string sOperatingSystem = "";
            string sUserid = "";
            string sPassword = "";
            bool xxxx = false;
            string sServer = UU.GetServerFromIniFile(".\\HistoricJamaica.ini", ref sOperatingSystem, ref sDataDirectory, ref sUserid, ref sPassword, ref xxxx);
            SQLCopy = new CSql(databasename, sServer, sDataDirectory, false);
            string SQLError = SQLCopy.GetSQLErrorMessage();
            if (SQLError != U.NoSQLError)
            {
                MessageBox.Show(SQLError);
                return false;
            }
            SQLCopy.DeleteFromTable(U.Marriage_Table);
            SQLCopy.DeleteFromTable(U.Person_Table);
            SQLCopy.DeleteFromTable(U.CemeteryValue_Table);
            m_Person_tbl = SQL.DefinePersonTable();
//            m_Person_tbl = SQLCopy.DefinePersonTable();
            m_Person_tbl.PrimaryKey = new DataColumn[] { m_Person_tbl.Columns[U.ImportPersonID_col] };
             m_Marriage_tbl = SQL.DefineMarriageTable();
            //            m_Marriage_tbl = SQLCopy.DefineMarriageTable();
            //            m_Marriage_tbl = SQLCopy.DefineMarriageTable();
            m_Cemetery_tbl = SQLCopy.DefineCemeteryValueTable();
            return true;
        }
        //****************************************************************************************************************************
        protected bool OpenInputFile(string sFilter)
        {
            string sFileNameWithPath = UU.SelectFile(sFilter, m_sDataDirectory);
            if (sFileNameWithPath.Length == 0)
            {
                return false;
            }
            m_sFileName = GetFileNameFromPath(sFileNameWithPath);
            try
            {
                m_tr = new StreamReader(sFileNameWithPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            return true;
        }
        //****************************************************************************************************************************
        protected void CloseInputFile()
        {
            m_tr.Close();
        }
        //****************************************************************************************************************************
        protected string GetFileNameFromPath(string sFileNameWithPath)
        {
            char[] c = new char[1];
            c[0] = '\\';
            int iIndexOfLastBackslash = sFileNameWithPath.LastIndexOfAny(c);
            return sFileNameWithPath.Substring(iIndexOfLastBackslash+1);
        }
        //****************************************************************************************************************************
        protected string ReadRecord()
        {
            return m_tr.ReadLine();
        }
        //****************************************************************************************************************************
        protected string RemoveChar(string sString,
                                    char cCharToRemove)
        {
            bool dDone = false;
            do
            {
                int ifoundChar = sString.IndexOf(cCharToRemove);
                if (ifoundChar < 0)
                    dDone = true;
                else
                {
                    sString = sString.Remove(ifoundChar, 1);
                }
            } while (!dDone);
            return sString;
        }
        //****************************************************************************************************************************
        private string RemoveAllCharacterAndReturnLowerCase(string sString,
                                                            char cCharToRemove)
        {
            sString = RemoveChar(sString, cCharToRemove);
            return sString.ToLower().TrimString();
        }
        //****************************************************************************************************************************
        protected string ValidPrefix(string sName)
        {
            string sNameWithoutDots = RemoveAllCharacterAndReturnLowerCase(sName, '.');
            string sNameWithoutCommasDots = RemoveAllCharacterAndReturnLowerCase(sNameWithoutDots, ',');
            if (sNameWithoutCommasDots == "dr")
                return "Dr";
            else if (sNameWithoutCommasDots == "rev")
                return "Rev";
            else if (sNameWithoutCommasDots == "col" || sNameWithoutCommasDots == "colonel")
                return "Col";
            else if (sNameWithoutCommasDots == "gen")
                return "Gen";
            else if (sNameWithoutCommasDots == "cpt" || sNameWithoutCommasDots == "capt")
                return "Cpt";
            else if (sNameWithoutCommasDots == "ltg")
                return "Ltg";
            else
                return "";
        }
        //****************************************************************************************************************************
        protected string ValidSuffix(string sName)
        {
            string sNameWithoutDots = RemoveAllCharacterAndReturnLowerCase(sName, '.');
            string sNameWithoutCommasDots = RemoveAllCharacterAndReturnLowerCase(sNameWithoutDots, ',');
            if (sNameWithoutCommasDots == "jr")
                return "Jr";
            else if (sNameWithoutCommasDots == "ii")
                return "II";
            else if (sNameWithoutCommasDots == "2nd")
                return "II";
            else if (sNameWithoutCommasDots == "iii")
                return "III";
            else if (sNameWithoutCommasDots == "iv")
                return "IV";
            else if (sNameWithoutCommasDots == "phd")
                return "PhD";
            else if (sNameWithoutCommasDots == "md")
                return "MD";
            else if (sNameWithoutCommasDots == "esq")
                return "Esq";
            else if (sNameWithoutCommasDots == "cpa")
                return "CPA";
            else if (sNameWithoutCommasDots == "usn")
                return "USN";
            else if (sNameWithoutCommasDots == "usmc")
                return "USMC";
            else if (sNameWithoutCommasDots == "pe")
                return "PE";
            else if (sNameWithoutCommasDots == "ra")
                return "RA";
            else
                return "";
        }
    }
}
