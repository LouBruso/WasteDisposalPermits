using System;
using System.IO;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using SQL_Library;

namespace System.Runtime.CompilerServices
{
    public class ExtensionAttribute : Attribute { }
}

namespace Utilities
{
    //****************************************************************************************************************************
    public class DataGridViewWithDoubleClick : DataGridView
    {
        public DataGridViewWithDoubleClick()
            : base()
        { // Set the style so a double click event occurs.
            SetStyle(ControlStyles.StandardDoubleClick, true);
        }
    }
    //****************************************************************************************************************************
    public class ListBoxWithDoubleClick : ListBox
    {
        public ListBoxWithDoubleClick()
            : base()
        { // Set the style so a double click event occurs.
            SetStyle(ControlStyles.StandardDoubleClick, true);
        }
    }
    //****************************************************************************************************************************
    public class TextBoxWithDoubleClick : TextBox
    {
        public TextBoxWithDoubleClick()
            : base()
        { // Set the style so a double click event occurs.
            SetStyle(ControlStyles.StandardDoubleClick, true);
        }
    }
    //****************************************************************************************************************************
    public class CheckedListBoxWithDoubleClick : CheckedListBox
    {
        public CheckedListBoxWithDoubleClick()
            : base()
        { // Set the style so a double click event occurs.
            SetStyle(ControlStyles.StandardDoubleClick, true);
        }
    }
    //****************************************************************************************************************************
    public class LabelWithDoubleClick : Label
    {
        public LabelWithDoubleClick()
            : base()
        { // Set the style so a double click event occurs.
            SetStyle(ControlStyles.StandardDoubleClick, true);
        }
    }
    public static class UU       // Utilities
    {   // Global Constants
        //****************************************************************************************************************************
        private static void PaintParentLines(Graphics G,
                                      int Person_LocationX,
                                      int Person_LocationY,
                                      int Person_Width,
                                      int Person_Height,
                                      int Father_LocationX,
                                      int Father_LocationY,
                                      int Father_Width,
                                      int Father_Height,
                                      int Mother_LocationX,
                                      int Mother_LocationY,
                                      int Mother_Width,
                                      int Mother_Height)
        {
            int iPerson_x = Person_LocationX + Person_Width;
            int iPerson_y = Person_LocationY + (Person_Height / 2);
            int iFather_y = Father_LocationY + (Father_Height / 2);
            int iMother_y = Mother_LocationY + (Mother_Height / 2);
            int iMidpointx = (iPerson_x + (Father_LocationX + Mother_LocationX) / 2) / 2;
            int iMidpointy = (iFather_y + iMother_y) / 2;
            G.DrawLine(Pens.Black, iPerson_x, iMidpointy, iMidpointx, iMidpointy);
            G.DrawLine(Pens.Black, iMidpointx, iFather_y, iMidpointx, iMother_y);
            G.DrawLine(Pens.Black, iMidpointx, iFather_y, Father_LocationX, iFather_y);
            G.DrawLine(Pens.Black, iMidpointx, iMother_y, Mother_LocationX, iMother_y);
        }
        //****************************************************************************************************************************
        public static void PaintParentLines(Graphics G,
                                     ListBoxWithDoubleClick Person_ListView,
                                     ListBoxWithDoubleClick Father_ListView,
                                     ListBoxWithDoubleClick Mother_ListView)
        {
            PaintParentLines(G, Person_ListView.Location.X, Person_ListView.Location.Y, Person_ListView.Width, Person_ListView.Height,
                                Father_ListView.Location.X, Father_ListView.Location.Y, Father_ListView.Width, Father_ListView.Height,
                                Mother_ListView.Location.X, Mother_ListView.Location.Y, Mother_ListView.Width, Mother_ListView.Height);
        }
        //****************************************************************************************************************************
        public static void PaintParentLines(Graphics G,
                                     TextBox Person_TextBox,
                                     TextBox Father_TextBox,
                                     TextBox Mother_TextBox)
        {
            PaintParentLines(G, Person_TextBox.Location.X, Person_TextBox.Location.Y, Person_TextBox.Width, Person_TextBox.Height,
                                Father_TextBox.Location.X, Father_TextBox.Location.Y, Father_TextBox.Width, Father_TextBox.Height,
                                Mother_TextBox.Location.X, Mother_TextBox.Location.Y, Mother_TextBox.Width, Mother_TextBox.Height);
        }
        //****************************************************************************************************************************
        public static void LoadSuffixComboBox(ComboBox Suffix_comboBox)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("Suffix", typeof(string)));
            list.Rows.Add(list.NewRow());
            list.Rows.Add(list.NewRow());
            list.Rows.Add(list.NewRow());
            list.Rows.Add(list.NewRow());
            list.Rows.Add(list.NewRow());
            list.Rows[0][0] = "";
            list.Rows[1][0] = "Sr";
            list.Rows[2][0] = "Jr";
            list.Rows[3][0] = "III";
            list.Rows[4][0] = "IV";
            Suffix_comboBox.DataSource = list;
            Suffix_comboBox.DisplayMember = "Suffix";
        }
        //****************************************************************************************************************************
        public static void LoadMariatalStatusComboBox(ComboBox MariatalStatusComboBox)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("Mariatal Status", typeof(string)));
            for (int i = 0; i < 5; i++)
                list.Rows.Add(list.NewRow());
            list.Rows[0][0] = "Married";
            list.Rows[1][0] = "Civil Union";
            list.Rows[2][0] = "Divorced";
            list.Rows[3][0] = "Living Together";
            list.Rows[4][0] = "Parents Of";
            MariatalStatusComboBox.DataSource = list;
            MariatalStatusComboBox.DisplayMember = "Mariatal Status";
        }
        //****************************************************************************************************************************
        public static void LoadPrefixComboBox(ComboBox Prefix_comboBox)
        {
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn("Prefix", typeof(string)));
            for (int i=0;i < 5;i++)
                list.Rows.Add(list.NewRow());
            list.Rows[0][0] = "";
            list.Rows[1][0] = "Rev";
            list.Rows[2][0] = "Sgt";
            list.Rows[3][0] = "Dr";
            list.Rows[4][0] = "Cpt";
            Prefix_comboBox.DataSource = list;
            Prefix_comboBox.DisplayMember = "Prefix";
        }
        //****************************************************************************************************************************
        public static string GetFileNameFromPath(string sFileNameWithPath)
        {
            char[] c = new char[1];
            c[0] = '\\';
            int iIndexOfLastBackslash = sFileNameWithPath.LastIndexOfAny(c);
            return sFileNameWithPath.Substring(iIndexOfLastBackslash + 1);
        }
        //****************************************************************************************************************************
        public static TextReader OpenInputFile(string sFileName)
        {
            TextReader tr = null;
            try
            {
                tr = new StreamReader(sFileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return tr;
        }
        //****************************************************************************************************************************
        public static string GetServerFromIniFile(string sFileName,
                                                  ref string sOperatingSystem,
                                                  ref string sDataDirectory,
                                                  ref string sUserid,
                                                  ref string sPassword,
                                                  ref bool permitMode)
        {
            TextReader tr = OpenInputFile(sFileName);
            permitMode = true;
            if (tr == null)
            {
                Exception e = new Exception("Unable to find ini file");
                throw e;
            }
            else
            {
                string sServer = "";
                string sStr = tr.ReadLine();
                while (sStr != null)
                {
                    if (sStr.Length > 8 && sStr.Substring(0, 8).ToLower() == "[server]")
                    {
                        sServer = "Server=" + sStr.Substring(8) + ";";
                    }
                    else if (sStr.Length > 6 && sStr.Substring(0, 6).ToLower() == "[data]")
                    {
                        sDataDirectory = sStr.Substring(6);
                    }
                    else if (sStr.Length > 2 && sStr.Substring(0, 4).ToLower() == "[os]")
                    {
                        sOperatingSystem = sStr.Substring(4);
                    }
                    else if (sStr.Length > 2 && sStr.Substring(0, 8).ToLower() == "[userid]")
                    {
                        sUserid = "User Id=" + sStr.Substring(8) + ";";
                    }
                    else if (sStr.Length > 2 && sStr.Substring(0, 10).ToLower() == "[password]")
                    {
                        sPassword = "Password = " + sStr.Substring(10) + ";";
                    }
                    else if (sStr.Length > 2 && sStr.Substring(0, 9).ToLower() == "[contact]")
                    {
                        permitMode = false; ;
                    }
                    sStr = tr.ReadLine();
                }
                tr.Close();
                if (sServer.Length == 0)
                {
                    Exception e = new Exception("ini file does not contain 'Server' string");
                    throw e;
                }
                if (String.IsNullOrEmpty(sPassword) && sUserid.ToLower().Contains("apro"))
                {
                    sPassword = "Password = " + "support" + ";";
                }
                return sServer;
            }
            //            string sServer = @"Server=JHFServer.JHF.local\JHF;";
            //string sServer = @"Server=JHFNotebook\SQLExpress;";
            //            string sServer = @"Server=JHFNotebook\SQLExpress;";
        }
        //****************************************************************************************************************************
        public static string SelectFile(string sFilter,
                                        string sFolder)
        {
            OpenFileDialog myDialog = new OpenFileDialog();
            myDialog.Filter = sFilter;
            myDialog.InitialDirectory = sFolder;
            if (myDialog.ShowDialog() == DialogResult.OK)
                return myDialog.FileName;
            else
                return "";
        }
        //****************************************************************************************************************************
        public static void LoadCategory_listBox(DataTable CategoryTBL,
                                         ListBoxWithDoubleClick Categories_listBox,
                                         CSql Sql,
                                         string sTableName,
                                         string sTableId,
                                         int iTableID)
        {
            Categories_listBox.Items.Clear();
            string sPreviousCategoryName = "";
            foreach (DataRow row in CategoryTBL.Rows)
            {
                int iCategoryID = row[U.CategoryID_col].ToInt();
                string sCategoryName = SQL.GetCategoryName(iCategoryID);
                if (sCategoryName != sPreviousCategoryName)
                {
                    Categories_listBox.Items.Add(sCategoryName);
                    sPreviousCategoryName = sCategoryName;
                }
                string sCategoryIDValue = row[U.CategoryValueValue_col].ToString();
                Categories_listBox.Items.Add(UU.ShowGroupValue(sCategoryIDValue));
            }
        }
        //****************************************************************************************************************************
        public static string ShowGroupValue(string sGroupValue)
        {
            return @"    " + sGroupValue;
        }
        //****************************************************************************************************************************
        public static string GetStreetName(int streetNum, string streetName)
        {
            string returnStreet = "";
            if (streetNum != 0)
            {
                returnStreet += streetNum + " ";
            }
            returnStreet += streetName;
            return returnStreet;
        }
        //****************************************************************************************************************************
        public static string ShowGrandListType(char type)
        {
            switch (type)
            {
                case 'T': return "In Town";
                case 'S': return "In State";
                case 'N': return "Out State";
                case 'H': return "HomeOwner ";
                case 'C': return "Caretaker";
                case 'F': return "Family/Friend";
                case 'R': return "Renter";
                case 'L': return "Business";
                default: return "";
            }
        }
        //****************************************************************************************************************************
        public static void LoadSourceComboBox(CSql sql,
                     ComboBox PhotoSource_comboBox)
        {
            DataTable tbl = new DataTable();
            SQL.GetAllCollections(tbl);
            DataTable list = new DataTable();
            list.Columns.Add(new DataColumn(U.PhotoSource_col, typeof(string)));
            foreach (DataRow row in tbl.Rows)
            {
                DataRow List_row = list.NewRow();
                string s = row[U.PhotoSource_col].ToString();
                List_row[U.PhotoSource_col] = row[U.PhotoSource_col].ToString();
                list.Rows.Add(List_row);
            }
            PhotoSource_comboBox.DataSource = list;
            PhotoSource_comboBox.DisplayMember = U.PhotoSource_col;
        }
        //****************************************************************************************************************************
        private static bool NotCircularDecendant(CSql sql,
                                                 int iPersonID,
                                                 int iCompareID)
        {
            if (iPersonID == 0 || iCompareID == 0)
                return true;
            DataTable tblChildren = SQL.GetAllChildrenForPerson(iPersonID);
            foreach (DataRow row in tblChildren.Rows)
            {
                int iChildID = row[U.PersonID_col].ToInt();
                if (iCompareID == iChildID)
                {
                    MessageBox.Show("This person is a Decendant");
                    return false;
                }
                if (!NotCircularDecendant(sql, iChildID, iCompareID))
                    return false;
            }
            return true;
        }
        //****************************************************************************************************************************
        private static bool NotCircularAncestor(CSql sql,
                                                int iPersonID,
                                                int iCompareID)
        {
            // Recursive function to ensure that no person can be added as a spouse or child of an ancestor
            if (iPersonID == 0 || iCompareID == 0)
                return true;
            int iFatherID;
            int iMotherID;
            SQL.GetFatherMother(iPersonID, out iFatherID, out iMotherID);
            if (iCompareID == iFatherID || iCompareID == iMotherID)
            {
                MessageBox.Show("This person is an Ancestor");
                return false;
            }
            if (NotCircularAncestor(sql, iFatherID, iCompareID) && NotCircularAncestor(sql, iMotherID, iCompareID))
                return true;
            else
                return false;
        }
        //****************************************************************************************************************************
        public static bool NotCircularReference(CSql sql,
                                                int iPersonID,
                                                int iCompareID)
        {
            if (NotCircularAncestor(sql, iPersonID, iCompareID))
                return NotCircularDecendant(sql, iPersonID, iCompareID);
            else
                return false;
        }
        //****************************************************************************************************************************
        public static void ShowErrorMessage(HistoricJamaicaException ex)
        {
            switch (ex.errorCode)
            {
                case ErrorCodes.eSuccess:
                    MessageBox.Show("Historic Jamaica Exception: Success");
                    break;
                case ErrorCodes.eDeleteUnsuccessful:
                    MessageBox.Show("Delete Unsuccessful");
                    break;
                case ErrorCodes.eException:
                    MessageBox.Show("Historic Jamaica Exception: " + ex.errorString);
                    break;
                case ErrorCodes.eSaveUnsuccessful:
                    MessageBox.Show("Save Unsuccessful");
                    break;
                case ErrorCodes.eUpdateUnsuccessful:
                    MessageBox.Show("Update Unsuccessful");
                    break;
                default:
                    MessageBox.Show("Historic Jamaica Unknown Exception: " + ex.errorString);
                    break;
            }
        }
        //****************************************************************************************************************************
    }
}
