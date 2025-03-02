using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using SQL_Library;
using Utilities;

namespace WasteDisposalPermits
{
    class CGridPropertyID : FGrid
    {
        private DataTable m_tbl;
        private int m_iNumElements = 0;
        private int m_iSelectedIDLocation = U.Exception;
        private int m_iSelectedSpanLocation = 0;
        private int m_iSelectedOwnerName1Location = 0;
        private int m_iSelectedOwnerName2Location = 0;
        private int m_iSelectedAddress1Location = 0;
        private int m_iSelectedAddress2Location = 0;
        private int m_iSelectedCityLocation = 0;
        private int m_iSelectedZipLocation = 0;
        private int m_iSelectedStreetNameLocation = 0;
        private DataGridViewCellEventArgs mouseLocation;
        ContextMenuStrip strip = new ContextMenuStrip();
        private string m_SelectedPropertyID = "";
        private string m_SelectedSpan = "";
        private string m_OwnerName1 = "";
        private string m_OwnerName2 = "";
        private string m_Address1 = "";
        private string m_Address2 = "";
        private string m_City = "";
        private string m_Zip = "";
        private string m_StreetName = "";
        private int m_StreetNum = 0;
        //****************************************************************************************************************************
        public string SelectedPropertyID
        {
            get { return m_SelectedPropertyID; }
        }
        //****************************************************************************************************************************
        public string SelectedSpan
        {
            get { return m_SelectedSpan; }
        }
        //****************************************************************************************************************************
        public string OwnerName1
        {
            get { return m_OwnerName1; }
        }
        //****************************************************************************************************************************
        public string OwnerName2
        {
            get { return m_OwnerName2; }
        }
        //****************************************************************************************************************************
        public string Address1
        {
            get { return m_Address1; }
        }
        //****************************************************************************************************************************
        public string Address2
        {
            get { return m_Address2; }
        }
        //****************************************************************************************************************************
        public string City
        {
            get { return m_City; }
        }
        //****************************************************************************************************************************
        public string Zip
        {
            get { return m_Zip; }
        }
        //****************************************************************************************************************************
        public string StreetName
        {
            get { return m_StreetName; }
        }
        //****************************************************************************************************************************
        public int StreetNum
        {
            get { return m_StreetNum; }
        }
        //****************************************************************************************************************************
        public CGridPropertyID(ref DataTable tbl)
        {
            m_tbl = tbl;
            buttonPane.Visible = true;
            this.Text = "Grand List";
            Abort_Button.Visible = false;
        }
        //****************************************************************************************************************************
        protected override void SelectRowButton_DoubleClick(object sender, EventArgs e)
        {
            int iSelectedRow = General_DataGridView.SelectedRows[0].Index;
            if (iSelectedRow >= m_iNumElements)
            {
                m_SelectedPropertyID = "";
                m_SelectedSpan = "";
                m_OwnerName1 = "";
                m_OwnerName2 = "";
                m_Address1 = "";
                m_Address2 = "";
                m_City = "";
                m_Zip = "";
                m_StreetName = "";
                m_StreetNum = 0;
            }
            else
            {
                DataGridViewRow s = General_DataGridView.Rows[iSelectedRow];
                m_SelectedPropertyID = s.Cells[m_iSelectedIDLocation].Value.ToString();
                m_SelectedSpan = s.Cells[m_iSelectedSpanLocation].Value.ToString();
                m_OwnerName1 = s.Cells[m_iSelectedOwnerName1Location].Value.ToString();
                m_OwnerName2 = s.Cells[m_iSelectedOwnerName2Location].Value.ToString();
                m_Address1 = s.Cells[m_iSelectedAddress1Location].Value.ToString();
                m_Address2 = s.Cells[m_iSelectedAddress2Location].Value.ToString();
                m_City = s.Cells[m_iSelectedCityLocation].Value.ToString();
                m_Zip = s.Cells[m_iSelectedZipLocation].Value.ToString();
                m_StreetName = s.Cells[m_iSelectedStreetNameLocation].Value.ToString();
            }
            Close();
        }
        //****************************************************************************************************************************
        private int GridHeight()
        {
            if (m_iNumGridElements > 19)
                return 60;
            else 
                return 422;
        }
        //****************************************************************************************************************************
        protected override void SetupDataGridView()
        {
            this.Controls.Add(General_DataGridView);
            General_DataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.Black;
            General_DataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            General_DataGridView.ColumnHeadersDefaultCellStyle.Font = new Font(General_DataGridView.Font, FontStyle.Bold);
            General_DataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            General_DataGridView.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            General_DataGridView.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            General_DataGridView.GridColor = Color.Black;
            General_DataGridView.RowHeadersVisible = false;
            General_DataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            General_DataGridView.MultiSelect = false;
            General_DataGridView.Dock = DockStyle.Fill;
            General_DataGridView.CellFormatting += new DataGridViewCellFormattingEventHandler(General_DataGridView_CellFormatting);
            General_DataGridView.ColumnHeadersVisible = true;
            General_DataGridView.ColumnCount = 10;
            General_DataGridView.Columns[0].Name = "Street Name";
            General_DataGridView.Columns[0].Width = 200;
            General_DataGridView.Columns[1].Name = "";
            General_DataGridView.Columns[1].Width = 80;
            General_DataGridView.Columns[2].Name = "Name1";
            General_DataGridView.Columns[2].Width = 200;
            General_DataGridView.Columns[3].Name = "Name2";
            General_DataGridView.Columns[3].Width = 200;
            General_DataGridView.Columns[4].Name = "City";
            General_DataGridView.Columns[4].Width = 150;
            General_DataGridView.Columns[5].Name = "AddressA";
            General_DataGridView.Columns[5].Width = 200;
            General_DataGridView.Columns[6].Name = "AddressB";
            General_DataGridView.Columns[6].Width = 200;
            General_DataGridView.Columns[7].Name = "Zip";
            General_DataGridView.Columns[7].Width = 50;
            General_DataGridView.Columns[8].Name = "PropID";
            General_DataGridView.Columns[8].Width = 80;
            General_DataGridView.Columns[9].Name = "Span";
            General_DataGridView.Columns[9].Width = 80;
            m_iSelectedOwnerName1Location = 2;
            m_iSelectedOwnerName2Location = 3;
            m_iSelectedCityLocation = 4;
            m_iSelectedAddress1Location = 5;
            m_iSelectedAddress2Location = 6;
            m_iSelectedZipLocation = 7;
            m_iSelectedIDLocation = 8;
            m_iSelectedSpanLocation = 9;
            m_iSelectedStreetNameLocation = 0;

            General_DataGridView.CellMouseEnter += dataGridView_CellMouseEnter;
            foreach (DataGridViewColumn column in General_DataGridView.Columns)
            {
                column.ContextMenuStrip = strip;
            }
        }
        //****************************************************************************************************************************
        protected override void ShowAllValues()
        {
            m_iNumElements = 0;
            General_DataGridView.Rows.Clear();
            foreach (DataRow row in m_tbl.Rows)
            {
                int grandListId = row[U.GrandListID_col].ToInt();
                string streetName = "";
                string span = row[U.Span_col].ToString();
                int permitid = (span.IndexOf('-') > 0) ? 0 : span.ToInt();
                char permitType;
                if (grandListId >= 90000)
                {
                    permitType = 'A';
                    streetName = "Contact";
                }
                else
                {
                    permitType = row[U.WhereOwnerLiveID_col].ToChar();
                    streetName = UU.GetStreetName(row[U.StreetNum_col].ToInt(), row[U.StreetName_col].ToString());
                }
                string whereLives = UU.ShowGrandListType(permitType);
                string city = (String.IsNullOrEmpty(row[U.City_col].ToString())) ? "" : row[U.City_col] + ", " + row[U.State_col];
                General_DataGridView.Rows.Add(streetName, whereLives, 
                                              row[U.Name1_col], row[U.Name2_col], city, 
                                              row[U.AddressA_col], row[U.AddressB_col], row[U.Zip_col], grandListId, span);
                m_iNumElements++;
            }
            m_iNumGridElements = m_tbl.Rows.Count;
            this.Location = new Point(100, 80);
//            SetSizeOfGrid(U.iMaxSizeOfGrid-200);
            this.Size = new Size(900, 600);
        }
        //****************************************************************************************************************************
        private void dataGridView_CellMouseEnter(object sender, DataGridViewCellEventArgs location)
        {
            mouseLocation = location;
        }
    }
}
