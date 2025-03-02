using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using SQL_Library;
using Utilities;
namespace WasteDisposalPermits
{
    class CGridPermits : FGrid
    {
        private DataTable m_tbl;
        private int m_iNumElements = 0;
        private int m_iSelectedIDLocation = U.Exception;
        private DataGridViewCellEventArgs mouseLocation;
        ContextMenuStrip strip = new ContextMenuStrip();
        private int m_SelectedPermitID = 0;
        //****************************************************************************************************************************
        public CGridPermits(ref DataTable tbl)
        {
            m_tbl = tbl;
            buttonPane.Visible = true;
            this.Text = "Permits";
            Abort_Button.Visible = false;
        }
        //****************************************************************************************************************************
        public int SelectedPermitID
        {
            get { return m_SelectedPermitID; }
        }
        //****************************************************************************************************************************
        protected override void SelectRowButton_DoubleClick(object sender, EventArgs e)
        {
            int iSelectedRow = General_DataGridView.SelectedRows[0].Index;
            if (iSelectedRow >= m_iNumElements)
            {
                m_SelectedPermitID = 0;
            }
            else
            {
                DataGridViewRow s = General_DataGridView.Rows[iSelectedRow];
                m_SelectedPermitID = s.Cells[m_iSelectedIDLocation].Value.ToInt();
            }
            Close();
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
            General_DataGridView.Columns[0].Name = "Name";
            General_DataGridView.Columns[0].Width = 150;
            General_DataGridView.Columns[1].Name = "Street";
            General_DataGridView.Columns[1].Width = 150;
            General_DataGridView.Columns[2].Name = "Apartment";
            General_DataGridView.Columns[2].Width = 90;
            General_DataGridView.Columns[3].Name = "Permit Type";
            General_DataGridView.Columns[3].Width = 80;
            General_DataGridView.Columns[4].Name = "Permit";
            General_DataGridView.Columns[4].Width = 50;
            General_DataGridView.Columns[5].Name = "Status";
            General_DataGridView.Columns[5].Width = 50;
            General_DataGridView.Columns[6].Name = "Owner1";
            General_DataGridView.Columns[6].Width = 210;
            General_DataGridView.Columns[7].Name = "Owner2";
            General_DataGridView.Columns[7].Width = 210;
            General_DataGridView.Columns[8].Name = "Grand List ID";
            General_DataGridView.Columns[8].Width = 100;
            General_DataGridView.Columns[9].Name = "ID";
            General_DataGridView.Columns[9].Width = 40;
            m_iSelectedIDLocation = 9;

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
                int iGrandListID = row[U.GrandListID_col].ToInt();
                string sStreet = "";
                string sOwner1 = "";
                string sOwner2 = "";
                int iCatetakerID = row[U.CareTakerID_col].ToInt();
                DataTable GrandListTable = SQL.GetGrandListPropertyByGrandListID(iGrandListID);
                if (iCatetakerID != 0)
                {
                    DataTable Caretaker_tbl = new DataTable();
                    if (SQL.GetCareTaker(Caretaker_tbl, iCatetakerID))
                    {
                        sStreet = Caretaker_tbl.Rows[0][U.Town_col].ToString();
                    }
                }
                else
                if (GrandListTable.Rows.Count != 0)
                {
                    sStreet = GrandListTable.Rows[0][U.StreetNum_col].ToInt() + " " + GrandListTable.Rows[0][U.StreetName_col].ToString();
                    sOwner1 = GrandListTable.Rows[0][U.Name1_col].ToString();
                    sOwner2 = GrandListTable.Rows[0][U.Name2_col].ToString();
                }
                string permitType = UU.ShowGrandListType(row[U.PermitType_col].ToChar());
                string status;
                switch (row[U.Status_col].ToChar())
                {
                    case 'A': status = "Active"; break;
                    default: status = "Inactive"; break;
                }
                General_DataGridView.Rows.Add(row[U.LastName_col] + ", " + row[U.FirstName_col], sStreet,
                                              row[U.Apartment_col], permitType, row[U.PermitNumber_col], status,
                                              sOwner1, sOwner2, iGrandListID, row[U.PermitID_col]);
                m_iNumElements++;
            }
            m_iNumGridElements = m_tbl.Rows.Count;
            this.Location = new Point(100, 80);
//            SetSizeOfGrid(U.iMaxSizeOfGrid-200);
            this.Size = new Size(1000, 600);
        }
        //****************************************************************************************************************************
        private void dataGridView_CellMouseEnter(object sender, DataGridViewCellEventArgs location)
        {
            mouseLocation = location;
        }
    }
}
