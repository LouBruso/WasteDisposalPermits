using System;
using System.IO;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Drawing;
using SQL_Library;
using Utilities;

namespace WasteDisposalPermits
{
    public partial class PermitForm : Form
    {
        private string m_sDataDirectory = "";
        private string sOperatingSystem = "";
        private string sUserid = "";
        private string sPassword = "";
        private int m_iPermitID = 0;
        private int m_iCareTakerID = 0;
        private bool m_bChangePropertyMode = false;
        private bool m_bWasNewPermitClicked = false;
        private bool m_bEditCaretaker = false;
        private bool m_bRadioButtonChanged = false;
        private const string databasename = "DataBase=WasteDisposalPermits;";
        //private const string sServer = @"Server=TJ1\JVTSQL;";
        private const string sServer = @"Server=JHFNotebook\SQLEXPRESS;";
        private int row1Location;
        private int row2Location;
        private int row3Location;
        private int col2Location;
        private int col3Location;
        private int col4Location;
        private int col5Location;
        private DataTable m_PermitTable;
        private DataTable m_CaretakerTable;
        private DataTable m_ContactsTable;
        private DataTable m_grandListTbl;
        private int m_GrandListId;
        private string m_Span;
        private bool permitMode;
        private bool doCheckRadioButtons = false;
        private bool LastNameLeaveAlreadyFired = false;
        private bool initializeMode = false;
        DataTable alternateFirstNamesTbl;

        public PermitForm(string[] args)
        {
            string sServer = UU.GetServerFromIniFile("c:\\WasteDisposalPermits\\WasteDisposalPermits.ini", ref sOperatingSystem, ref m_sDataDirectory, ref sUserid, ref sPassword, ref permitMode);
            //string sServer = UU.GetServerFromIniFile(".\\WasteDisposalPermits.ini", ref sOperatingSystem, ref m_sDataDirectory);
            try
            {
                SQL.OpenConnection(databasename, sServer, m_sDataDirectory, true, sUserid, sPassword);
            }
            catch (SqlException)
            {
                MessageBox.Show("unable to open connection");
                //                Dispose(true);
                this.Close();
                return;
            }
            InitializeComponent();
            this.Size = new Size(1167, Permit_groupBox.Size.Height + 100);
            Permit_groupBox.Location = new Point(Contacts_groupBox.Location.X, Contacts_groupBox.Location.Y);
            ContactsToolStrip_MenuItem.Text = (permitMode) ? "Contacts" : "Permits";
            SaveRowColLocation();
            SetGroupBoxesVisible();
            NumClients_label.Visible = false;
            NumClients_textBox.Visible = false;
            SetEditMode(false);
            m_PermitTable = SQL.DefineWasteDisposalPermitsTable();
            m_CaretakerTable = SQL.DefineCareTakerTable();
            alternateFirstNamesTbl = SQL.GetAllAlternativeSpellings(U.AlternativeSpellingsFirstName_Table);
        }
        //****************************************************************************************************************************
        protected override void Dispose(bool disposing)
        {
            SQL.CloseConnection();
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        //****************************************************************************************************************************
        private void ShowPermitType(char PermitType)
        {
            switch (PermitType)
            {
                case 'L':
                    LowImpactBusiness_radioButton.Checked = true;
                    Homeowner_radioButton.Checked = false;
                    Family_radioButton.Checked = false;
                    Caretaker_radioButton.Checked = false;
                    Renter_radioButton.Checked = false;
                    break;
                case 'C':
                    Caretaker_radioButton.Checked = true;
                    Homeowner_radioButton.Checked = false;
                    Family_radioButton.Checked = false;
                    LowImpactBusiness_radioButton.Checked = false;
                    Renter_radioButton.Checked = false;
                    PermitType_groupBox.Enabled = false;
                    break;
                case 'R':
                    Caretaker_radioButton.Checked = false;
                    Homeowner_radioButton.Checked = false;
                    Family_radioButton.Checked = false;
                    LowImpactBusiness_radioButton.Checked = false;
                    Renter_radioButton.Checked = true;
                    break;
                case 'F':
                    Caretaker_radioButton.Checked = false;
                    Homeowner_radioButton.Checked = false;
                    Family_radioButton.Checked = true;
                    LowImpactBusiness_radioButton.Checked = false;
                    Renter_radioButton.Checked = false;
                    break;
                case 'H':
                    Homeowner_radioButton.Checked = true;
                    Family_radioButton.Checked = false;
                    LowImpactBusiness_radioButton.Checked = false;
                    Caretaker_radioButton.Checked = false;
                    Renter_radioButton.Checked = false;
                    break;
                default:
                    Homeowner_radioButton.Checked = false;
                    Family_radioButton.Checked = false;
                    LowImpactBusiness_radioButton.Checked = false;
                    Caretaker_radioButton.Checked = false;
                    Renter_radioButton.Checked = false;
                    break;
            }
        }
        //****************************************************************************************************************************
        private void ShowStatus(char Status)
        {
            switch (Status)
            {
                case 'I':
                    Active_radioButton.Checked = false;
                    Inactive_radioButton.Checked = true;
                    break;
                default:
                    Active_radioButton.Checked = true;
                    Inactive_radioButton.Checked = false;
                    break;
            }
        }
        //****************************************************************************************************************************
        private char PermitType()
        {
            if (Homeowner_radioButton.Checked)
                return 'H';
            else
            if (Renter_radioButton.Checked)
                return 'R';
            else
            if (Family_radioButton.Checked)
                return 'F';
            else
            if (LowImpactBusiness_radioButton.Checked)
                return 'L';
            else
            if (Caretaker_radioButton.Checked)
                return 'C';
            else
                return 'H';
        }
        //****************************************************************************************************************************
        private bool PermitChanged()
        {
            if (LastName_textBox.Modified ||
                FirstName_textBox.Modified ||
                Apartment_textBox.Modified ||
                StreetName_textBox.Modified ||
                StreetNum_textBox.Modified ||
                PropertyID_textBox.Modified ||
                Phone_textBox.Modified ||
                CellPhone_textBox.Modified ||
                EMail_textBox.Modified ||
                NumCards_textBox.Modified ||
                NumClients_textBox.Modified ||
                PropertyID_textBox.Modified ||
                m_bRadioButtonChanged)
                return true;
            return false;
        }
        //****************************************************************************************************************************
        protected void SetToUnmodified()
        {
            LastName_textBox.Modified = false;
            FirstName_textBox.Modified = false;
            Apartment_textBox.Modified = false;
            StreetName_textBox.Modified = false;
            StreetNum_textBox.Modified = false;
            PropertyID_textBox.Modified = false;
            Phone_textBox.Modified = false;
            CellPhone_textBox.Modified = false;
            EMail_textBox.Modified = false;
            NumCards_textBox.Modified = false;
            NumClients_textBox.Modified = false;
            PropertyID_textBox.Modified = false;
            m_bRadioButtonChanged = false;
        }
        //****************************************************************************************************************************
        protected bool CheckIfPermitChanged()
        {
            if (!PermitChanged())
                return false;
            switch (MessageBox.Show("Save Changes?", "", MessageBoxButtons.YesNoCancel))
            {
                case DialogResult.Yes:
                    if (!SavePermit())
                        return true;
                    else
                        return false;
                case DialogResult.No:
                    return false;
                default:
                case DialogResult.Cancel:
                    return true;
            }
        }
        //****************************************************************************************************************************
        protected bool CheckIfContactChanged()
        {
            if (!ContactModified())
                return false;
            switch (MessageBox.Show("Save Changes?", "", MessageBoxButtons.YesNoCancel))
            {
                case DialogResult.Yes:
                    if (!SaveContact())
                        return true;
                    else
                        return false;
                case DialogResult.No:
                    return false;
                default:
                case DialogResult.Cancel:
                    return true;
            }
        }
        //****************************************************************************************************************************
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            e.Cancel = CheckIfSaveCanceled();
        }
        //****************************************************************************************************************************
        private bool CheckIfSaveCanceled()
        {
            if (permitMode)
            {
                return CheckIfPermitChanged();
            }
            else
            {
                return CheckIfContactChanged();
            }
        }
        //****************************************************************************************************************************
        private char GetStatus()
        {
            if (Inactive_radioButton.Checked)
                return 'I';
            else
                return 'A';
        }
        //****************************************************************************************************************************
        private void SaveContact_Click(object sender, EventArgs e)
        {
            if (ContactModified())
            {
                SaveContact();
            }
            SetRowColLocation(true);
            SetContactsBlank(false);
            StreetName_textBox.Enabled = true;
            SearchAddress_button.Enabled = true;
        }
        //****************************************************************************************************************************
        private bool SaveContact()
        {
            try
            {
                if (m_ContactsTable != null && m_ContactsTable.Rows.Count == 0)
                {
                    SaveNewContact();
                }
                if (m_GrandListId > 90000)
                {
                    UpdateContact();
                }
                else if (m_Span.IndexOf('-') > 0)
                {
                    UpdateGrandList();
                }
                else
                {
                    UpdatePermit(m_Span.ToInt());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Save Unsuccessful: " + ex.Message);
            }
            return false;
        }
        //****************************************************************************************************************************
        private void SaveNewContact()
        {
            DataRow ContactsRow = m_ContactsTable.NewRow();
            m_ContactsTable.Rows.Add(ContactsRow);
            ContactsRow[U.ContactID_col] = 0;
            ContactsRow[U.Name1_col] = ContactLastName1_textBox.Text + " " + ContactFirstName1_textBox.Text;
            ContactsRow[U.Name2_col] = "";
            ContactsRow[U.AddressA_col] = ContactAddress1_textBox.Text;
            ContactsRow[U.AddressB_col] = ContactAddress2_textBox.Text;
            ContactsRow[U.City_col] = ContactCity_textBox.Text;
            ContactsRow[U.State_col] = ContactState_textBox.Text;
            ContactsRow[U.Zip_col] = ContactPhone_textBox.Text;
            ContactsRow[U.Phone_col] = ContactPhone_textBox.Text;
            ContactsRow[U.CellPhone_col] = ContactCellPhone_textBox.Text;
            ContactsRow[U.EMail_col] = ContactEMail_textBox.Text;
            SqlCommand insertCommand = SQL.InsertCommand(null, m_ContactsTable, U.Contacts_Table, true);
            SQL.InsertWithDA(m_ContactsTable, insertCommand);
        }
        //****************************************************************************************************************************
        private void UpdatePermit(int permitId)
        {
            m_PermitTable.Clear();
            SQL.GetWasteDisposalPermit(m_PermitTable, permitId);
            DataRow permitRow = m_PermitTable.Rows[0];
            permitRow[U.Phone_col] = ContactPhone_textBox.Text;
            permitRow[U.CellPhone_col] = ContactCellPhone_textBox.Text;
            permitRow[U.EMail_col] = ContactEMail_textBox.Text;
            ArrayList fieldValues = SQL.ColumnList(U.Phone_col, U.CellPhone_col, U.EMail_col);
            SQL.UpdateWithDA(m_PermitTable, U.WasteDisposalPermits_Table, U.PermitID_col, fieldValues);
        }
        //****************************************************************************************************************************
        private void UpdateGrandList()
        {
            DataTable ContactsTbl = SQL.GetGrandList(m_GrandListId);
            if (ContactsTbl.Rows.Count == 0)
            {
                MessageBox.Show("Unable to Get Grand List Record");
            }
            DataRow ContactsRow = ContactsTbl.Rows[0];
            ContactsRow[U.Phone_col] = ContactPhone_textBox.Text;
            ContactsRow[U.CellPhone_col] = ContactCellPhone_textBox.Text;
            ContactsRow[U.EMail_col] = ContactEMail_textBox.Text;
            ArrayList fieldValues = SQL.ColumnList(U.Phone_col, U.CellPhone_col, U.EMail_col);
            SQL.UpdateWithDA(m_ContactsTable, U.Contacts_Table, U.GrandListID_col, fieldValues);
        }
        //****************************************************************************************************************************
        private void UpdateContact()
        {
            DataRow ContactsRow = m_ContactsTable.Rows[0];
            ContactsRow[U.Phone_col] = ContactPhone_textBox.Text;
            ContactsRow[U.CellPhone_col] = ContactCellPhone_textBox.Text;
            ContactsRow[U.EMail_col] = ContactEMail_textBox.Text;
            ArrayList fieldValues = SQL.ColumnList(U.Phone_col, U.CellPhone_col, U.EMail_col);
            SQL.UpdateWithDA(m_ContactsTable, U.Contacts_Table, U.GrandListID_col, fieldValues);
        }
        //****************************************************************************************************************************
        private bool CheckForSpaceInLastName()
        {
            if (!LowImpactBusiness_radioButton.Checked && !Caretaker_radioButton.Checked && LastName_textBox.Text.Trim().Contains(" "))
            {
                MessageBox.Show("Only Low Impact Business or Caretaker can have a space in its Last Name");
                return true;
            }
            return false;
        }
        //****************************************************************************************************************************
        private bool CheckRequiredFields()
        {
            if (PropertyID_textBox.Text.ToString().Length == 0)
            {
                MessageBox.Show("Property ID is required");
                return false;
            }
            if (String.IsNullOrEmpty(LastName_textBox.Text))
            {
                MessageBox.Show("Last Name is Required");
                return false;
            }
            if (CheckForSpaceInLastName())
            {
                return false;
            }
            if (LastName_textBox.Text.Trim().Contains(","))
            {
                MessageBox.Show("Cannot have Comma in Last Name");
                return false;
            }
            if (NumCards_textBox.Text.ToString().Length == 0)
            {
                NumCards_textBox.Text = "1";
                MessageBox.Show("Number Cards Set to 1");
            }
            return true;
        }
        //****************************************************************************************************************************
        private bool SavePermit()
        {
            if (!PermitChanged() || !CheckRequiredFields())
            {
                return false;
            }
            try
            {
                bool bSuccess = false;
                if (m_iPermitID == 0)
                {
                    AddValuesToPermitsTable(m_PermitTable);
                    AddValuesToCaretakerTable(m_CaretakerTable);
                    m_iPermitID = SQL.CreateNewPermit(m_PermitTable, m_CaretakerTable, m_bEditCaretaker);
                    bSuccess = m_iPermitID != 0;
                }
                else
                {
                    ArrayList CaretakerFieldsModified = null;
                    ArrayList FieldsModified = UpdatePermitInDataTable(m_PermitTable);
                    if (m_bEditCaretaker)
                    {
                        CaretakerFieldsModified = UpdateCaretakerInDataTable(m_CaretakerTable);
                    }
                    bSuccess = SQL.UpdatePermit(m_PermitTable, m_CaretakerTable, FieldsModified, CaretakerFieldsModified,
                                                m_bEditCaretaker, m_iCareTakerID);
                }
                if (bSuccess)
                {
                    SetToUnmodified();
                }
                return bSuccess;
            }
            catch (HistoricJamaicaException e)
            {
                HistoricJamaicaException ex = new HistoricJamaicaException(e.Message);
                UU.ShowErrorMessage(ex);
                return false;
            }
            catch (Exception e)
            {
                HistoricJamaicaException ex = new HistoricJamaicaException(e.Message);
                UU.ShowErrorMessage(ex);
                return false;
            }
        }
        //****************************************************************************************************************************
        private ArrayList UpdateCaretakerInDataTable(DataTable caretaker_tbl)
        {
            ArrayList FieldsModified = new ArrayList();
            DataRow caretaker_row = caretaker_tbl.Rows[0];
            DataColumnCollection columns = caretaker_tbl.Columns;
            U.SetToNewValueIfDifferent(FieldsModified, columns, caretaker_row, U.StreetNum_col, StreetNum_textBox.Text.ToString());
            U.SetToNewValueIfDifferent(FieldsModified, columns, caretaker_row, U.StreetName_col, StreetName_textBox.Text.ToString());
            U.SetToNewValueIfDifferent(FieldsModified, columns, caretaker_row, U.CaretakerName_col, Apartment_textBox.Text.ToString());
            U.SetToNewValueIfDifferent(FieldsModified, columns, caretaker_row, U.Town_col, PropertyID_textBox.Text.ToString());
            return FieldsModified;
        }
        //****************************************************************************************************************************
        private ArrayList UpdatePermitInDataTable(DataTable permit_tbl)
        {
            ArrayList FieldsModified = new ArrayList();
            DataRow permit_row = permit_tbl.Rows[0];
            DataColumnCollection columns = permit_tbl.Columns;
            U.SetToNewValueIfDifferent(FieldsModified, permit_row, U.PermitNumber_col, Permit_textBox.Text.ToInt());
            if (!m_bEditCaretaker)
            {
                U.SetToNewValueIfDifferent(FieldsModified, permit_row, U.GrandListID_col, PropertyID_textBox.Text.ToInt());
            }
            U.SetToNewValueIfDifferent(FieldsModified, columns, permit_row, U.LastName_col, LastName_textBox.Text.ToString());
            U.SetToNewValueIfDifferent(FieldsModified, columns, permit_row, U.FirstName_col, FirstName_textBox.Text.ToString());
            U.SetToNewValueIfDifferent(FieldsModified, columns, permit_row, U.Apartment_col, Apartment_textBox.Text.ToString());
            U.SetToNewValueIfDifferent(FieldsModified, columns, permit_row, U.Phone_col, Phone_textBox.Text.ToString());
            U.SetToNewValueIfDifferent(FieldsModified, columns, permit_row, U.CellPhone_col, CellPhone_textBox.Text.ToString());
            U.SetToNewValueIfDifferent(FieldsModified, columns, permit_row, U.EMail_col, EMail_textBox.Text.ToString());
            U.SetToNewValueIfDifferent(FieldsModified, permit_row, U.PermitType_col, PermitType());
            U.SetToNewValueIfDifferent(FieldsModified, permit_row, U.NumberCards_col, NumCards_textBox.Text.ToInt());
            U.SetToNewValueIfDifferent(FieldsModified, permit_row, U.CareTakerID_col, m_iCareTakerID);
            U.SetToNewValueIfDifferent(FieldsModified, permit_row, U.Status_col, GetStatus());
            U.SetToNewValueIfDifferent(FieldsModified, permit_row, U.NumClients_col, NumClients_textBox.Text.ToInt());
            return FieldsModified;
        }
        //****************************************************************************************************************************
        private void AddValuesToPermitsTable(DataTable tbl)
        {
            DataRow WasteDisposalPermits_row = tbl.NewRow();
            WasteDisposalPermits_row[U.PermitNumber_col] = Permit_textBox.Text.ToInt();
            WasteDisposalPermits_row[U.GrandListID_col] = PropertyID_textBox.Text.ToInt();
            WasteDisposalPermits_row[U.LastName_col] = LastName_textBox.Text.ToString().Trim();
            WasteDisposalPermits_row[U.FirstName_col] = FirstName_textBox.Text.ToString().Trim();
            WasteDisposalPermits_row[U.Apartment_col] = Apartment_textBox.Text.ToString().Trim();
            WasteDisposalPermits_row[U.Phone_col] = Phone_textBox.Text.ToString().Trim();
            WasteDisposalPermits_row[U.CellPhone_col] = CellPhone_textBox.Text.ToString().Trim();
            WasteDisposalPermits_row[U.EMail_col] = EMail_textBox.Text.ToString().Trim();
            WasteDisposalPermits_row[U.PermitType_col] = PermitType();
            WasteDisposalPermits_row[U.NumberCards_col] = NumCards_textBox.Text.ToInt();
            WasteDisposalPermits_row[U.NumClients_col] = NumClients_textBox.Text.ToInt();
            WasteDisposalPermits_row[U.CareTakerID_col] = m_iCareTakerID;
            WasteDisposalPermits_row[U.Status_col] = GetStatus();
            tbl.Rows.Add(WasteDisposalPermits_row);
        }
        //****************************************************************************************************************************
        private void AddValuesToCaretakerTable(DataTable tbl)
        {
            DataRow CareTaker_row = tbl.NewRow();
            CareTaker_row[U.CaretakerName_col] = Apartment_textBox.Text.ToString();
            CareTaker_row[U.StreetName_col] = StreetName_textBox.Text.ToString();
            CareTaker_row[U.StreetNum_col] = StreetNum_textBox.Text.ToInt();
            CareTaker_row[U.Town_col] = PropertyID_textBox.Text.ToString();
            tbl.Rows.Add(CareTaker_row);
        }
        //****************************************************************************************************************************
        private void CancelContact_Click(object sender, EventArgs e)
        {
            SetRowColLocation(true);
            SetContactsBlank(false);
            StreetName_textBox.Enabled = true;
            SearchAddress_button.Enabled = true;
        }
        //****************************************************************************************************************************
        private void SaveRowColLocation()
        {
            row1Location = ContactCity_textBox.Location.Y;
            row2Location = ContactState_textBox.Location.Y;
            row3Location = ContactZip_textBox.Location.Y;
            col2Location = ContactLastName2_textBox.Location.X;
            col3Location = ContactAddress1_textBox.Location.X;
            col4Location = ContactCity_textBox.Location.X;
            col5Location = ContactPhone_textBox.Location.X;
        }
        //****************************************************************************************************************************
        private void SetRowColLocation(bool withName2)
        {
            ContactLastName1_label.Text = (withName2) ? "Last Name 1" : "Last Name";
            ContactFirstName1_label.Text = (withName2) ? "First Name 1" : "First Name";
            ContactCellPhone_label.Text = "Cell Phone";
            ContactLastName2_textBox.Visible = withName2;
            ContactFirstName2_textBox.Visible = withName2;
            ContactLastName2_label.Visible = withName2;
            ContactFirstName2_label.Visible = withName2;
            int col = (withName2) ? col3Location : col2Location;
            if (withName2)
            {
                Type_textBox.Location = new Point(col4Location, row1Location - 40);
                Type_label.Location = new Point(col4Location, row1Location - 40 - 16);
            }
            else
            {
                Type_textBox.Location = new Point(col4Location, row1Location);
                Type_label.Location = new Point(col4Location, row1Location - 16);
            }
            ContactAddress1_textBox.Location = new Point(col, row1Location);
            ContactAddress2_textBox.Location = new Point(col, row2Location);
            ContactAddress1_label.Location = new Point(col - 3, row1Location - 16);
            ContactAddress2_label.Location = new Point(col - 3, row2Location - 16);
            col = (withName2) ? col4Location : col3Location;
            ContactCity_textBox.Location = new Point(col, row1Location);
            ContactState_textBox.Location = new Point(col, row2Location);
            ContactCity_label.Location = new Point(col - 3, row1Location - 16);
            ContactState_label.Location = new Point(col - 3, row2Location - 16);
            int row = (withName2) ? row3Location : row1Location;
            ContactZip_textBox.Location = new Point(col + 48, row2Location);
            ContactZip_label.Location = new Point(col +48, row2Location - 16);
            row = (withName2) ? row3Location : row2Location;
            //ContactPhone_textBox.Location = new Point(col5Location, row);
            //ContactPhone_label.Location = new Point(col5Location - 3, row - 16);
        }
        //****************************************************************************************************************************
        private bool ContactModified()
        {
            if (ContactLastName1_textBox.Modified)
            {
                return true;
            }
            if (ContactFirstName1_textBox.Modified)
            {
                return true;
            }
            if (ContactLastName2_textBox.Modified)
            {
                return true;
            }
            if (ContactFirstName2_textBox.Modified)
            {
                return true;
            }
            if (ContactAddress1_textBox.Modified)
            {
                return true;
            }
            if (ContactAddress2_textBox.Modified)
            {
                return true;
            }
            if (ContactCity_textBox.Modified)
            {
                return true;
            }
            if (ContactState_textBox.Modified)
            {
                return true;
            }
            if (ContactZip_textBox.Modified)
            {
                return true;
            }
            if (ContactPhone_textBox.Modified)
            {
                return true;
            }
            if (ContactCellPhone_textBox.Modified)
            {
                return true;
            }
            if (ContactEMail_textBox.Modified)
            {
                return true;
            }
            return false;
        }
        //****************************************************************************************************************************
        private void SetContactsBlank(bool newContact)
        {
            ContactLastName1_textBox.Enabled = true;
            ContactFirstName1_textBox.Enabled = newContact;
            ContactLastName2_textBox.Enabled = false;
            ContactFirstName2_textBox.Enabled = false;
            ContactAddress1_textBox.Enabled = newContact;
            ContactAddress2_textBox.Enabled = newContact;
            ContactCity_textBox.Enabled = newContact;
            ContactState_textBox.Enabled = newContact;
            ContactZip_textBox.Enabled = newContact;
            ContactPhone_textBox.Enabled = newContact;
            ContactCellPhone_textBox.Enabled = newContact;
            ContactEMail_textBox.Enabled = newContact;
            //ContactLastName1_textBox.Text = "";
            ContactFirstName1_textBox.Text = "";
            ContactLastName2_textBox.Text = "";
            ContactFirstName2_textBox.Text = "";
            ContactAddress1_textBox.Text = "";
            ContactAddress2_textBox.Text = "";
            ContactCity_textBox.Text = "";
            ContactState_textBox.Text = "";
            ContactZip_textBox.Text = "";
            ContactPhone_textBox.Text = "";
            ContactCellPhone_textBox.Text = "";
            ContactEMail_textBox.Text = "";
            ContactLastName1_textBox.Modified = false;
            ContactFirstName1_textBox.Modified = false;
            ContactLastName2_textBox.Modified = false;
            ContactFirstName2_textBox.Modified = false;
            ContactAddress1_textBox.Modified = false;
            ContactAddress2_textBox.Modified = false;
            ContactCity_textBox.Modified = false;
            ContactState_textBox.Modified = false;
            ContactZip_textBox.Modified = false;
            ContactPhone_textBox.Modified = false;
            ContactCellPhone_textBox.Modified = false;
            ContactEMail_textBox.Modified = false;
            ContactSave_button.Enabled = newContact;
            ContactCancel_button.Enabled = newContact;
            ContactName_button.Enabled = !newContact;
            NewContact_button.Enabled = !newContact;
        }
        //****************************************************************************************************************************
        private void SavePermit_Click(object sender, EventArgs e)
        {
            SavePermit();
        }
        //****************************************************************************************************************************
        private void GrandListEdit(bool bEdit)
        {
            OutOfTownCareTakerToolStrip_MenuItem.Enabled = bEdit;
            PermitByName_button.Enabled = bEdit;
            PermitByID_button.Enabled = bEdit;
            Permit_textBox.Enabled = bEdit;
        }
        //****************************************************************************************************************************
        private void SetEditMode(bool bEdit)
        {
            if (!permitMode)
            {
                StreetName_textBox.Enabled = !bEdit;
                SearchAddress_button.Enabled = !bEdit;
                return;
            }
            LastName_textBox.Enabled = true;
            PermitByName_button.Enabled = !bEdit;
            PermitByID_button.Enabled = !bEdit;
            Permit_textBox.Enabled = !bEdit;
            if (m_bEditCaretaker)
            {
                StreetName_textBox.Enabled = true;
            }
            else if (permitMode)
            {
                StreetName_textBox.Enabled = !bEdit;
                SearchAddress_button.Enabled = !bEdit;
            }
            if (permitMode)
            {
                SearchAddress_button.Enabled = !bEdit;
                StreetName_button.Enabled = !bEdit;
            }
            LastName_textBox.Enabled = bEdit;
            FirstName_textBox.Enabled = bEdit;
            Apartment_textBox.Enabled = bEdit;
            Phone_textBox.Enabled = bEdit;
            EMail_textBox.Enabled = bEdit;
            CellPhone_textBox.Enabled = bEdit;
            Active_radioButton.Enabled = bEdit;
            Inactive_radioButton.Enabled = bEdit;
            NumCards_textBox.Enabled = bEdit;
            Homeowner_radioButton.Enabled = bEdit;
            Family_radioButton.Enabled = bEdit;
            Renter_radioButton.Enabled = bEdit;
            LowImpactBusiness_radioButton.Enabled = bEdit;
            Caretaker_radioButton.Enabled = bEdit;
            DeletePermit_button.Enabled = bEdit;
            ChangeProperty_button.Enabled = bEdit;
            Save_button.Enabled = bEdit;
            NewPermit_button.Enabled = bEdit;
        }
        //****************************************************************************************************************************
        private void DeletePermit_Click(object sender, EventArgs e)
        {
            if (m_iPermitID != 0)
            {
                switch (MessageBox.Show("Do you wish to delete this permit?", "", MessageBoxButtons.YesNo))
                {
                    case DialogResult.Yes:
                        SQL.DeleteWithParms(U.WasteDisposalPermits_Table, new NameValuePair(U.PermitID_col, m_iPermitID));
                        InitializePermit();
                        NumClients_label.Visible = false;
                        NumClients_textBox.Visible = false;
                        m_bEditCaretaker = false;
                        break;
                    default:
                        break;
                }

            }
        }
        //****************************************************************************************************************************
        private void InitializePermit()
        {
            initializeMode = true;
            m_iPermitID = 0;
            m_PermitTable.Clear();
            m_CaretakerTable.Clear();
            InitializeGrandListWindow();
            Permit_textBox.Text = "";
            LastName_textBox.Text = "";
            FirstName_textBox.Text = "";
            Apartment_textBox.Text = "";
            Phone_textBox.Text = "";
            CellPhone_textBox.Text = "";
            EMail_textBox.Text = "";
            //ShowPermitType('H');
            ShowStatus('A');
            NumCards_textBox.Text = "";
            NumClients_textBox.Text = "";
            m_bWasNewPermitClicked = true; // must be before checked items below
            Homeowner_radioButton.Checked = false;
            Family_radioButton.Checked = false;
            Renter_radioButton.Checked = false;
            Caretaker_radioButton.Checked = false;
            LowImpactBusiness_radioButton.Checked = false;
            PermitType_groupBox.Enabled = true;
            m_bWasNewPermitClicked = false; // must be after checked items above
            this.LastName_textBox.Leave -= new System.EventHandler(this.LastNameLeave_Click);
            SetEditMode(false);
            SetToUnmodified();
            initializeMode = false;
        }
        //****************************************************************************************************************************
        private void NewPermit_Click(object sender, EventArgs e)
        {
            if (!CheckIfPermitChanged())
            {
                InitializePermit();
                StreetNum_textBox.Enabled = false;
                PropertyID_textBox.Enabled = false;
                PropertyID_label.Text = "PropertyID";
                OwnerName1_textBox.Enabled = false;
                StreetName_button.Visible = true;
                SearchAddress_button.Visible = true;
                PermitByName_button.Visible = true;
                PermitByID_button.Visible = true;
                StreetName_textBox.Focus();
                NumClients_label.Visible = false;
                NumClients_textBox.Visible = false;
                m_bEditCaretaker = false;
            }
        }
        //****************************************************************************************************************************
        private void ShowContactValues(int grandListId, string span)
        {
            m_GrandListId = grandListId;
            m_Span = span;

            DataRow grandListRow, permitRow;
            GetInfoFromDatabase(grandListId, span, out grandListRow, out permitRow);
            bool Name2Found = false;
            char permitType;
            if (permitRow == null)
            {
                ParceGrandlistName(grandListRow[U.Name1_col].ToString().Trim(), ContactLastName1_textBox, ContactFirstName1_textBox);
                Name2Found = ParceGrandlistName(grandListRow[U.Name2_col].ToString().Trim(), ContactLastName2_textBox, ContactFirstName2_textBox);
                Type_textBox.Text = (grandListId < 90000) ? UU.ShowGrandListType(grandListRow[U.WhereOwnerLiveID_col].ToChar()) : "A";
                ContactPhone_textBox.Text = "";
                ContactCellPhone_textBox.Text = "";
                ContactEMail_textBox.Text = "";
                permitType = 'A';
                ContactPhone_textBox.Text = grandListRow[U.Phone_col].ToString();
                ContactCellPhone_textBox.Text = grandListRow[U.CellPhone_col].ToString();
                ContactEMail_textBox.Text = grandListRow[U.EMail_col].ToString();
            }
            else
            {
                ContactLastName1_textBox.Text = permitRow[U.LastName_col].ToString();
                ContactFirstName1_textBox.Text = permitRow[U.FirstName_col].ToString();
                permitType = permitRow[U.PermitType_col].ToChar();
                if (permitType == 'H')
                {
                    Name2Found = ParceGrandlistName(grandListRow[U.Name2_col].ToString().Trim(), ContactLastName2_textBox, ContactFirstName2_textBox);
                }
                Type_textBox.Text = UU.ShowGrandListType(permitType);
                ContactPhone_textBox.Text = permitRow[U.Phone_col].ToString();
                ContactEMail_textBox.Text = permitRow[U.EMail_col].ToString();
                ContactCellPhone_textBox.Text = permitRow[U.CellPhone_col].ToString();
            }
            if (permitType == 'R' || permitType == 'F')
            {
                ContactAddress1_textBox.Text = grandListRow[U.StreetNum_col].ToString() + " " + grandListRow[U.StreetName_col].ToString();
                ContactAddress2_textBox.Text = "";
                ContactCity_textBox.Text = "Jamaica";
                ContactState_textBox.Text = "VT";
                ContactZip_textBox.Text = "05343";
            }
            else
            {
                ContactAddress1_textBox.Text = grandListRow[U.AddressA_col].ToString();
                ContactAddress2_textBox.Text = grandListRow[U.AddressB_col].ToString();
                ContactCity_textBox.Text = grandListRow[U.City_col].ToString();
                ContactState_textBox.Text = grandListRow[U.State_col].ToString();
                ContactZip_textBox.Text = grandListRow[U.Zip_col].ToString();
            }
            SetTextboxEnabled(grandListRow, m_GrandListId >= 90000);
            SetRowColLocation(Name2Found);
        }
        //****************************************************************************************************************************
        private void GetInfoFromDatabase(int grandListId, string span, out DataRow grandListRow, out DataRow permitRow)
        {
            permitRow = null;
            if (grandListId < 90000)
            {
                m_grandListTbl = SQL.GetGrandList(grandListId);
                grandListRow = m_grandListTbl.Rows[0];
                m_ContactsTable = null;
                if (span.IndexOf('-') < 0)
                {
                    int permitId = span.ToInt();
                    permitRow = SQL.GetWasteDisposalPermit(permitId);
                }
            }
            else
            {
                m_ContactsTable = SQL.GetContactPropertyByID(grandListId);
                grandListRow = m_ContactsTable.Rows[0];
            }
        }
        //****************************************************************************************************************************
        private bool ParceGrandlistName(string name, TextBox lastNameTextbox, TextBox FirstNameTextbox)
        {
            string lastName, firstName;
            U.GrandListFirstnameLastName(name, out lastName, out firstName);
            lastNameTextbox.Text = lastName;
            FirstNameTextbox.Text = firstName;
            return !String.IsNullOrEmpty(name);
        }
        //****************************************************************************************************************************
        private void SetTextboxEnabled(DataRow grandListRow, bool editContact)
        {
            ContactLastName1_textBox.Enabled = editContact;
            ContactFirstName1_textBox.Enabled = editContact;
            ContactLastName2_textBox.Enabled = editContact;
            ContactFirstName2_textBox.Enabled = editContact;
            ContactAddress1_textBox.Enabled = editContact;
            ContactAddress2_textBox.Enabled = editContact;
            ContactCity_textBox.Enabled = editContact;
            ContactState_textBox.Enabled = editContact;
            ContactZip_textBox.Enabled = editContact;
            ContactPhone_textBox.Enabled = true;
            ContactCellPhone_textBox.Enabled = true;
            ContactEMail_textBox.Enabled = true;
            ContactSave_button.Enabled = true;
            ContactCancel_button.Enabled = true;
            NewContact_button.Enabled = false;
            ContactName_button.Enabled = false;
            if (editContact)
            {
                ContactLastName1_textBox.Focus();
            }
            else
            {
                ContactEMail_textBox.Focus();
            }
        }
        //****************************************************************************************************************************
        private void ShowPermitValues()
        {
            DataRow row = m_PermitTable.Rows[0];
            m_iPermitID = row[U.PermitID_col].ToInt();
            NumClients_label.Visible = false;
            NumClients_textBox.Visible = false;
            m_bEditCaretaker = false;
            Permit_textBox.Text = row[U.PermitNumber_col].ToString();
            LastName_textBox.Text = row[U.LastName_col].ToString();
            FirstName_textBox.Text = row[U.FirstName_col].ToString();
            Apartment_textBox.Text = row[U.Apartment_col].ToString();
            Phone_textBox.Text = row[U.Phone_col].ToString();
            CellPhone_textBox.Text = row[U.CellPhone_col].ToString();
            EMail_textBox.Text = row[U.EMail_col].ToString();
            NumClients_textBox.Text = row[U.NumClients_col].ToString();
            ShowPermitType(row[U.PermitType_col].ToString()[0]);
            ShowStatus(row[U.Status_col].ToString()[0]);
            NumCards_textBox.Text = row[U.NumberCards_col].ToString();
            m_iCareTakerID = row[U.CareTakerID_col].ToInt();
            int grandListId = row[U.GrandListID_col].ToInt();
            if (m_iCareTakerID != 0)
            {
                if (!SQL.GetCareTaker(m_CaretakerTable, m_iCareTakerID) ||
                    m_CaretakerTable.Rows.Count == 0)
                {
                    MessageBox.Show("Invalid Grand List ID");
                }
                else
                {
                    NumClients_label.Visible = true;
                    NumClients_textBox.Visible = true;
                    m_bEditCaretaker = true;
                    ShowCareTakerValues(m_CaretakerTable.Rows[0]);
                }
            }
            else if (grandListId > 0)
            {
                DataTable tbl = SQL.GetGrandListPropertyByGrandListID(grandListId);
                if (tbl.Rows.Count == 0)
                    MessageBox.Show("Invalid Grand List ID");
                else
                    ShowPropertyValues(tbl.Rows[0]);
            }
            SetToUnmodified();
        }
        //****************************************************************************************************************************
        private int ShowPermit(DataTable tbl, int grandListId)
        {
            CGridPermits GridPermits = new CGridPermits(ref tbl);
            GridPermits.ShowDialog();
            m_iPermitID = 0;
            int iPermitID = GridPermits.SelectedPermitID;
            if (iPermitID != 0)
            {
                m_PermitTable.Clear();
                if (SQL.GetWasteDisposalPermit(m_PermitTable, iPermitID))
                {
                    ShowPermitValues();
                }
            }
            return iPermitID;
        }
        //****************************************************************************************************************************
        private void ShowPropertyValues(DataRow row)
        {
            StreetName_textBox.Text = row[U.StreetName_col].ToString();
            StreetNum_textBox.Text = row[U.StreetNum_col].ToString();
            OwnerName1_textBox.Text = row[U.Name1_col].ToString();
            OwnerName2_textBox.Text = row[U.Name2_col].ToString();
            Address1_textBox.Text = row[U.AddressA_col].ToString();
            Address2_textBox.Text = row[U.AddressB_col].ToString();
            City_textBox.Text = row[U.City_col].ToString() + ", " + row[U.State_col].ToString();
            Zip_textBox.Text = row[U.Zip_col].ToString();
            ActiveStatus_radioButton.Checked = row[U.ActiveStatus_col].ToChar() == 'A';
            InactiveStatus_radioButton.Checked = !ActiveStatus_radioButton.Checked;
            Vacant_radioButton.Checked = row[U.VacantLand_col].ToChar() == '1';
            Developed_radioButton.Checked = !Vacant_radioButton.Checked;
        }
        //****************************************************************************************************************************
        private void ShowCareTakerValues(DataRow row)
        {
            StreetName_textBox.Enabled = true;
            StreetNum_textBox.Enabled = true;
            PropertyID_textBox.Enabled = true;
            PropertyID_label.Text = "Town";
            StreetName_textBox.Text = row[U.StreetName_col].ToString();
            StreetNum_textBox.Text = row[U.StreetNum_col].ToString();
            PropertyID_textBox.Text = row[U.Town_col].ToString();
            OwnerName1_textBox.Text = row[U.CaretakerName_col].ToString();
            OwnerName2_textBox.Text = "";
        }
        //****************************************************************************************************************************
        private void InitializeGrandListWindow()
        {
            PropertyID_textBox.Text = "";
            StreetName_textBox.Text = "";
            OwnerName1_textBox.Text = "";
            OwnerName2_textBox.Text = "";
            Address1_textBox.Text = "";
            Address2_textBox.Text = "";
            City_textBox.Text = "";
            Zip_textBox.Text = "";
            ActiveStatus_radioButton.Checked = false;
            InactiveStatus_radioButton.Checked = false;
            Vacant_radioButton.Checked = false;
            Developed_radioButton.Checked = false;
            m_GrandListId = 0;
            m_Span = "";
        }
        //****************************************************************************************************************************
        private string GetProperty(DataTable tbl, out string span)
        {
            CGridPropertyID GridPropertyID = new CGridPropertyID(ref tbl);
            GridPropertyID.ShowDialog();
            string grandListId = GridPropertyID.SelectedPropertyID;
            if (grandListId.Length == 0) // || sNewPropertyID == PropertyID_textBox.Text.ToString())
            {
                span = "";
                return "";
            }
            span = GridPropertyID.SelectedSpan;
            if (grandListId.ToInt() >= 90000)
            {
                InitializeGrandListWindow();
            }
            else 
            {
                DataTable grandListTbl = SQL.GetGrandList(grandListId.ToInt());
                DataRow grandListRow = grandListTbl.Rows[0];
                PropertyID_textBox.Text = grandListId;
                StreetNum_textBox.Text = grandListRow[U.StreetNum_col].ToString();
                StreetName_textBox.Text = grandListRow[U.StreetName_col].ToString();
                OwnerName1_textBox.Text = grandListRow[U.Name1_col].ToString();
                OwnerName2_textBox.Text = grandListRow[U.Name2_col].ToString();
                Address1_textBox.Text = grandListRow[U.AddressA_col].ToString();
                Address2_textBox.Text = grandListRow[U.AddressB_col].ToString();
                City_textBox.Text = grandListRow[U.City_col] + ", " + grandListRow[U.State_col];
                Zip_textBox.Text = grandListRow[U.Zip_col].ToString();
                ActiveStatus_radioButton.Checked = grandListRow[U.ActiveStatus_col].ToChar() == 'A';
                InactiveStatus_radioButton.Checked = !ActiveStatus_radioButton.Checked;
                Vacant_radioButton.Checked = grandListRow[U.VacantLand_col].ToChar() == '1';
                Developed_radioButton.Checked = !Vacant_radioButton.Checked;
            }
            return GridPropertyID.SelectedPropertyID;
        }
        //****************************************************************************************************************************
        private string GetStreetName(string streetName)
        {
            if (String.IsNullOrEmpty(streetName))
            {
                return "";
            }
            if (streetName.Substring(0, 2) == "0 ")
            {
                return streetName.Substring(1).Trim();
            }
            int indexOfSpace = streetName.IndexOf(' ');
            if (indexOfSpace > 0)
            {
                int streetnum = streetName.Substring(0, indexOfSpace).ToInt();
                if (streetnum != 0)
                {
                    StreetNum_textBox.Text = streetnum.ToString();
                    streetName = streetName.Substring(indexOfSpace).Trim();
                }
            }
            return streetName;
        }
        //****************************************************************************************************************************
        private void NextPermitNumberFromDatabase()
        {
            int iLastPermitID = SQL.GetMaxValue(U.WasteDisposalPermits_Table, U.PermitNumber_col).ToInt();
            Permit_textBox.Text = (iLastPermitID + 1).ToString();
        }
        //****************************************************************************************************************************
        private void ShowContact(DataTable tbl)
        {
            string span;
            m_GrandListId = GetProperty(tbl, out span).ToInt();
            if (m_GrandListId == 0)
            {
                return;
            }
            ShowContactValues(m_GrandListId, span);
        }
        //****************************************************************************************************************************
        private bool ShowProperty(DataTable tbl, string searchName = "")
        {
            string span;
            string sNewPropertyID = GetProperty(tbl, out span);
            if (sNewPropertyID.Length == 0)
            {
                return false;
            }
            DataTable permitTbl = SQL.GetPermitByGrandListID(sNewPropertyID.ToInt());
            if (permitMode)
            {
                if (permitTbl.Rows.Count == 0)
                {
                    ShowNewPermit(searchName);
                }
                else
                {
                    int permitId = ShowPermit(permitTbl, sNewPropertyID.ToInt());
                    if (permitId == 0)
                    {
                        ShowNewPermit(searchName);
                    }
                }
            }
            else 
            {
                DataTable grandListTbl = SQL.GetGrandList(sNewPropertyID.ToInt());
                DataTable returnGrandListTbl = SQL.ReturnPermitsAndGrandListEntries(permitTbl, grandListTbl);
                ShowContact(returnGrandListTbl);
            }
            SetEditMode(true);
            SetToUnmodified();
            return true;
        }
        //****************************************************************************************************************************
        private void ShowNewPermit(string searchName)
        {
            NextPermitNumberFromDatabase();
            LastName_textBox.Text = GetSelectedLastName(searchName);
            LastName_textBox.Modified = false;
            doCheckRadioButtons = true;
            Homeowner_radioButton.Checked = true;
            Active_radioButton.Checked = true;
            LastName_textBox.Focus();
            this.LastName_textBox.Leave += new System.EventHandler(this.LastNameLeave_Click);
        }
        //****************************************************************************************************************************
        private string GetSelectedLastName(string searchName)
        {
            string OwnerName1 = OwnerName1_textBox.Text.Trim();
            string OwnerName2 = OwnerName2_textBox.Text.Trim();
            string lastName1, lastName2, firstName1, firstName2;
            U.GrandListFirstnameLastName(OwnerName1, out lastName1, out firstName1);
            U.GrandListFirstnameLastName(OwnerName2, out lastName2, out firstName2);
            if (String.IsNullOrEmpty(searchName) || String.IsNullOrEmpty(lastName2))
            {
                return lastName1;
            }
            if (lastName1.ToLower().Contains(searchName.ToLower()))
            {
                return lastName1;
            }
            if (lastName2.ToLower().Contains(searchName.ToLower()))
            {
                return lastName2;
            }
            if (OwnerName2.ToLower().Contains(searchName.ToLower()))
            {
                return U.GetSearchLastName(OwnerName2, searchName);
            }
            return lastName1;
        }
        //****************************************************************************************************************************
        private void NewContact_Click(object sender, EventArgs e)
        {
            ContactLastName1_textBox.Text = "";
            InitializeGrandListWindow();
            SetRowColLocation(false);
            SetContactsBlank(true);
            m_GrandListId = 90000;
            m_ContactsTable = SQL.DefineContactsTable();
            ContactLastName1_textBox.Focus();
        }
        //****************************************************************************************************************************
        private void GetPropertyIDByStreetName_Click(object sender, EventArgs e)
        {
            object NameSearch = (permitMode) ? StreetName_textBox.Text : ContactLastName1_textBox.Text;
            DataTable tbl = SQL.GetGrandListPropertiesSortByName(NameSearch.ToString());
            ShowProperty(tbl, NameSearch.ToString());
        }
        //****************************************************************************************************************************
        private void GetContactName_Click(object sender, EventArgs e)
        {
            string NameSearch = (permitMode) ? StreetName_textBox.Text : ContactLastName1_textBox.Text;
            if (string.IsNullOrEmpty(NameSearch))
            {
                return;
            }
            DataTable tbl = SQL.GetAllContacts(NameSearch.ToString());
            ShowContact(tbl);
        }
        //****************************************************************************************************************************
        private void PermitByID_Click(object sender, EventArgs e)
        {
            int iPermitNumber = Permit_textBox.Text.ToInt();
            if (iPermitNumber == 0)
            {
                DataTable tbl = new DataTable();
                if (SQL.GetAllPermits(tbl))
                {
                    int iNum = ShowPermit(tbl, 0);
                    if (iNum != 0)
                        SetEditMode(true);
                }
            }
            else
            if (SQL.GetPermitByPermitNumber(m_PermitTable, iPermitNumber))
            {
                ShowPermitValues();
                SetEditMode(true);
            }
            else
                MessageBox.Show("Invalid Permit Number");
        }
        //****************************************************************************************************************************
        private void PermitByName_Click(object sender, EventArgs e)
        {
            DataTable tbl = SQL.GetPermitLikeNames(Permit_textBox.Text.ToString());
            if (tbl.Rows.Count != 0)
            {
                if (ShowPermit(tbl, 0) != 0)
                    SetEditMode(true);
            }
        }
        //****************************************************************************************************************************
        private void GetPropertyID_Click(object sender, EventArgs e)
        {
            string searchStreet = StreetName_textBox.Text.ToString();
            if (string.IsNullOrEmpty(searchStreet))
            {
                return;
            }
            DataTable tbl = SQL.GetGrandListProperties(0, searchStreet);
            if (m_bChangePropertyMode)
            {
                string span;
                string sNewPropertyID = GetProperty(tbl, out span);
                if (sNewPropertyID.Length != 0)
                    PropertyID_textBox.Modified = true;
                SetEditMode(true);
                GrandListEdit(true);
                m_bChangePropertyMode = false;
            }
            else
            {
                if (ShowProperty(tbl))
                {
                    if (permitMode)
                    {
                        LastName_textBox.Focus();
                    }
                    else
                    {
                        ContactEMail_textBox.Focus();
                    }
                }
            }
        }
        //****************************************************************************************************************************
        private void PrintPermitReport_Click(object sender, EventArgs e)
        {
            DataTable tbl = new DataTable();
            SQL.SelectAll(U.WasteDisposalPermits_Table, SQL.OrderBy(U.LastName_col, U.FirstName_col), tbl);
            CPrintReport PrintReport = new CPrintReport();
            PrintReport.PrintReport(tbl);
        }
        //****************************************************************************************************************************
        private void PrintOneCard_Click(object sender, EventArgs e)
        {
            int iPermitNumber = Permit_textBox.Text.ToInt();
            if (iPermitNumber == 0)
            {
                MessageBox.Show("Please Specity a Permit Number");
                return;
            }
            DataTable tbl = new DataTable();
            SQL.SelectAll(U.WasteDisposalPermits_Table, tbl, new NameValuePair(U.PermitNumber_col, iPermitNumber.ToString()));
            if (tbl.Rows.Count == 0)
            {
                MessageBox.Show("Invalid Permit Number");
                return;
            }
            int iNumCards = tbl.Rows[0][U.NumberCards_col].ToInt();
            int iResponse = 1;
            if (iNumCards > 1)
            {
                SubmitButton Submit = new SubmitButton("Card Number", "All Cards");
                Submit.ShowDialog();
                iResponse = Submit.GetResponse();
                if (iResponse != 99 && iResponse > iNumCards)
                {
                    iResponse = iNumCards;
                }
            }
            if (iResponse > 0)
            {
                CPrintCards PrintCards = new CPrintCards();
                PrintCards.PrintCards(tbl, iResponse);
            }
        }

        //****************************************************************************************************************************
        private void PrintMultipleCards_Click(object sender, EventArgs e)
        {
            int iPermitNumber = Permit_textBox.Text.ToInt();
            if (iPermitNumber == 0)
            {
                MessageBox.Show("Please Specity a starting Permit Number");
                return;
            }
            SubmitButton Submit = new SubmitButton("Final Permit Number",
                                                   "All Remaining Permits");
            Submit.ShowDialog();
            int iResponse = Submit.GetResponse();
            if (iResponse < iPermitNumber)
                iResponse = 99999999;
            DataTable tbl = new DataTable();
            SQL.SelectAllBetween(U.WasteDisposalPermits_Table, SQL.OrderBy(U.LastName_col, U.FirstName_col), tbl,
                                                   new NameValuePair(U.PermitNumber_col, iPermitNumber.ToString()),
                                                   new NameValuePair(U.PermitNumber_col, iResponse.ToString()));
            CPrintCards PrintCards = new CPrintCards();
            PrintCards.PrintCards(tbl, 0);
        }
        //****************************************************************************************************************************
        private void RadioButtonChanged_Click(object sender, EventArgs e)
        {
            m_bRadioButtonChanged = true;
        }
        //****************************************************************************************************************************
        private void ChangeProperty_Click(object sender, EventArgs e)
        {
            SetEditMode(false);
            GrandListEdit(false);
            m_bChangePropertyMode = true;
        }
        //****************************************************************************************************************************
        private void FirstNameEnter_Click(object sender, EventArgs e)
        {
        }
        //****************************************************************************************************************************
        private void FirstNameLeave_Click(object sender, EventArgs e)
        {
            if (Homeowner_radioButton.Checked || Family_radioButton.Checked)
            {
                string lastName = LastName_textBox.Text.Trim();
                string firstName = FirstName_textBox.Text.Trim();
                string OwnerName1 = OwnerName1_textBox.Text.Trim();
                string OwnerName2 = OwnerName2_textBox.Text.Trim();
                bool alternateSpelling = false;
                if (SQL.CheckPermitNameOnGrandList(alternateFirstNamesTbl, OwnerName1, OwnerName2, lastName.Trim(), firstName.Trim(), ref alternateSpelling))
                {
                    if (!Homeowner_radioButton.Checked)
                    {
                        MessageBox.Show("Permit Type Set To Homwowner");
                    }
                    if (alternateSpelling)
                    {
                        MessageBox.Show("Check Spelling of First Name");
                    }
                    Homeowner_radioButton.Checked = true;
                    Family_radioButton.Checked = false;
                }
                else
                {
                    if (!Family_radioButton.Checked)
                    {
                        MessageBox.Show("Permit Type Set To Family");
                    }
                    Homeowner_radioButton.Checked = false;
                    Family_radioButton.Checked = true;
                }

            }
        }
        //****************************************************************************************************************************
        private void LastNameEnter_Click(object sender, EventArgs e)
        {
            LastNameLeaveAlreadyFired = false;
        }
        //****************************************************************************************************************************
        private void LastNameLeave_Click(object sender, EventArgs e)
        {
            if (initializeMode)
            {
                return;
            }
            string lastName = LastName_textBox.Text.Trim();
            if (String.IsNullOrEmpty(lastName))
            {
                MessageBox.Show("Last Name is Required");
                return;
            }
            if (CheckForSpaceInLastName())
            {
                return;
            }
            string OwnerName1 = OwnerName1_textBox.Text.Trim();
            string OwnerName2 = OwnerName2_textBox.Text.Trim();
            string glLastName1, glLastName2, glFirstName1, glFirstName2;
            U.GrandListFirstnameLastName(OwnerName1, out glLastName1, out glFirstName1);
            U.GrandListFirstnameLastName(OwnerName2, out glLastName2, out glFirstName2);
            if (glLastName1.ToLower().Contains(lastName.ToLower()))
            {
                SetHomeownerChecked();
                return;
            }
            if (glLastName2.ToLower().Contains(lastName.ToLower()))
            {
                SetHomeownerChecked();
                return;
            }
            if (OwnerName2.ToLower().Contains(lastName.ToLower()))
            {
                SetHomeownerChecked();
                return;
            }
            if (Homeowner_radioButton.Checked)
            {
                Homeowner_radioButton.Checked = false;
                Family_radioButton.Checked = true;
                MessageBox.Show("Please Check Permit Type");
            }
        }
        //****************************************************************************************************************************
        private void SetHomeownerChecked()
        {
            if (LastNameLeaveAlreadyFired)
            {
                LastNameLeaveAlreadyFired = false;
                return;
            }
            LastNameLeaveAlreadyFired = true;
            if (Family_radioButton.Checked || Renter_radioButton.Checked)
            {
                bool alternateSpelling = false;
                string lastName = LastName_textBox.Text.Trim();
                string firstName = FirstName_textBox.Text.Trim();
                string OwnerName1 = OwnerName1_textBox.Text.Trim();
                string OwnerName2 = OwnerName2_textBox.Text.Trim();
                if (SQL.CheckPermitNameOnGrandList(alternateFirstNamesTbl, OwnerName1, OwnerName2, lastName.Trim(), firstName.Trim(), ref alternateSpelling))
                {
                    if (MessageBox.Show("Change Permit Type to Homeowner?", "", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                    {
                        Homeowner_radioButton.Checked = true;
                        Family_radioButton.Checked = false;
                        Renter_radioButton.Checked = false;
                        LowImpactBusiness_radioButton.Checked = false;
                        Caretaker_radioButton.Checked = false;
                    }
                }
            }
        }
        //****************************************************************************************************************************
        private void CheckRadioButtons_Click(object sender, EventArgs e)
        {
            if (Homeowner_radioButton.Checked == false &&
                Family_radioButton.Checked == false &&
                Renter_radioButton.Checked == false &&
                LowImpactBusiness_radioButton.Checked == false &&
                Caretaker_radioButton.Checked == false)
            {
                if (String.IsNullOrEmpty(StreetName_textBox.Text))
                {
                    return;
                }
                SetDefaultRadioButton();
            }
            else
            {
                NumClients_label.Visible = false;
                NumClients_textBox.Visible = false;
                if (Renter_radioButton.Checked)
                {
                    Apartment_label.Text = "Apartment";
                    Apartment_label.Visible = true;
                    Apartment_textBox.Visible = true;
                    LastName_textBox.Focus();
                }
                else if (Caretaker_radioButton.Checked)
                {
                    Apartment_label.Text = "Business Name";
                    Apartment_label.Visible = true;
                    Apartment_textBox.Visible = true;
                    NumClients_label.Visible = true;
                    NumClients_textBox.Visible = true;
                    NumClients_textBox.Enabled = true;
                    LastName_textBox.Focus();
                }
                else if (LowImpactBusiness_radioButton.Checked)
                {
                    Apartment_label.Text = "Business Name";
                    Apartment_label.Visible = true;
                    Apartment_textBox.Visible = true;
                    LastName_textBox.Focus();
                }
                else
                {
                    Apartment_label.Visible = false;
                    Apartment_textBox.Visible = false;
                    LastName_textBox.Focus();
                }
            }
        }
        //****************************************************************************************************************************
        private void SetDefaultRadioButton()
        {
            if (Save_button.Focused ||
                NewPermit_button.Focused ||
                DeletePermit_button.Focused ||
                ChangeProperty_button.Focused)
            {
                m_bWasNewPermitClicked = true;
            }
            if (doCheckRadioButtons && !m_bWasNewPermitClicked)
            {
                m_bWasNewPermitClicked = false;
                doCheckRadioButtons = false;
                MessageBox.Show("Permit set to Homeowner");
                LastName_textBox.Focus();
                Homeowner_radioButton.Checked = true;
                Family_radioButton.Checked = false;
                Apartment_label.Visible = false;
                NumClients_label.Visible = false;
                NumClients_textBox.Visible = false;
                m_bRadioButtonChanged = false;
            }
        }
        //****************************************************************************************************************************
        private void ImportGrandList_Click(object sender, EventArgs e)
        {
            CImportGrandList ImportGrandList = new CImportGrandList();
        }
        //****************************************************************************************************************************
        private void ContactsToolStrip_MenuItem_Click(object sender, EventArgs e)
        {
            if (CheckIfSaveCanceled())
            {
                return;
            }
            if (permitMode)
            {
                InitializePermit();
                permitMode = false;
            }
            else
            {
                permitMode = true;
            }
            ContactsToolStrip_MenuItem.Text = (permitMode) ? "Contacts" : "Permits";
            SetGroupBoxesVisible();
        }
        private void SetGroupBoxesVisible()
        {
            Contacts_groupBox.Visible = !permitMode;
            Permit_groupBox.Visible = permitMode;
            OutOfTownCareTakerToolStrip_MenuItem.Visible = permitMode;
            StreetName_button.Visible = permitMode;
        }
        //****************************************************************************************************************************
        private void Maintenance_Click(object sender, EventArgs e)
        {
            PrintNamelist();
            //CreateFirstNameAlternateSpellingsTable();
            MessageBox.Show("Report Complete");
        }
        //****************************************************************************************************************************
        public static void CreateFirstNameAlternateSpellingsTable()
        {
            DataTable AlternateFirstNameTbl = SQL.DefineAlternativeSpellingsTable(U.AlternativeSpellingsFirstName_Table);
            string sFileName = @"C:\WasteDisposalPermits\AlternateFirstNameTable.txt";
            TextReader tr = UU.OpenInputFile(sFileName);
            try
            {
                string sStr = tr.ReadLine();
                while (sStr != null)
                {
                    string[] words = sStr.Split(',');
                    if (words.Length != 3)
                    {
                        if (sStr[sStr.Length - 1] != ';')
                        {
                            MessageBox.Show("Invalid str: " + sStr);
                            tr.Close();
                            return;
                        }
                        else
                        {
                        }
                    }
                    string name1 = words[0].Substring(2, words[0].Length - 3);
                    string name2 = words[1].Substring(1, words[1].Length - 3);
                    DataRow AlternateFirstNameRow = AlternateFirstNameTbl.NewRow();
                    AlternateFirstNameRow[U.NameSpelling1_Col] = name1;
                    AlternateFirstNameRow[U.NameSpelling2_Col] = name2;
                    AlternateFirstNameTbl.Rows.Add(AlternateFirstNameRow);
                    sStr = tr.ReadLine();
                }
                SqlCommand insertCommand = SQL.InsertCommand(AlternateFirstNameTbl, U.AlternativeSpellingsFirstName_Table, false);
                SQL.InsertWithDA(AlternateFirstNameTbl, insertCommand);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            tr.Close();
        }
        //****************************************************************************************************************************
        private void PrintNamelist()
        {
            ArrayList nameList = SQL.CheckPermits();
            CPrintReport printReport = new CPrintReport();
            //printReport.PrintReport(nameList);
        }
        //****************************************************************************************************************************
        private void OutOfTownCaretaker_Click(object sender, EventArgs e)
        {
            m_bEditCaretaker = true;
            NumClients_label.Visible = true;
            NumClients_textBox.Visible = true;
            StreetName_button.Visible = false;
            SearchAddress_button.Visible = false;
            PermitByName_button.Visible = false;
            PermitByID_button.Visible = false;
            ChangeProperty_button.Visible = false;
            StreetNum_textBox.Enabled = true;
            StreetName_textBox.Enabled = true;
            PropertyID_textBox.Enabled = true;
            Caretaker_radioButton.Checked = true;
            Save_button.Enabled = true;
            NewPermit_button.Enabled = true;
            Permit_textBox.Enabled = false;
            Apartment_textBox.Enabled = true;
            Apartment_label.Text = "Business";
            LastName_textBox.Enabled = true;
            FirstName_textBox.Enabled = true;
            Phone_textBox.Enabled = true;
            NumCards_textBox.Enabled = true;
            PropertyID_label.Text = "Town";
            StreetNum_textBox.Focus();
            NextPermitNumberFromDatabase();
        }

        private void Contacts_groupBox_Enter(object sender, EventArgs e)
        {

        }

        private void ContactZip_textBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
