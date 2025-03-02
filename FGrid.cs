using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using SQL_Library;
using Utilities;

namespace WasteDisposalPermits
{

    public class FGrid : Form
    {
        protected int m_iNumGridElements = 750;
        protected DataGridViewWithDoubleClick General_DataGridView = new DataGridViewWithDoubleClick();
        protected Panel buttonPane = new Panel();
        protected Button Abort_Button = new Button();
        protected Button Filter_Button = new Button();
        protected int m_iWidthOfButtonPane = 99;
        protected int m_iScreenHeight;
        protected int m_iScreenWidth;

        //****************************************************************************************************************************
        public FGrid()
        {
            m_iScreenHeight = Screen.PrimaryScreen.Bounds.Height;
            m_iScreenWidth = Screen.PrimaryScreen.Bounds.Width;
            this.Load += new EventHandler(FGridDataView_Load);
            InitializeComponent();
        }
        //****************************************************************************************************************************
        private void FGridDataView_Load(System.Object sender, System.EventArgs e)
        {
            SetupLayout();
            SetupDataGridView();
            ChangePropertiesOfGridIfNecessary();
            ShowAllValues();
        }
        protected virtual void ShowAllValues() { }
        protected virtual void PopulateDataGridViewValues() { }
        protected virtual void SetupDataGridView() { }
        protected virtual void ChangePropertiesOfGridIfNecessary() { }
        protected virtual void SelectRowButton_DoubleClick(object sender, EventArgs e) { }
        //****************************************************************************************************************************
        protected void SetSizeOfGrid(int iWidth)
        {
            int iHeight = 74 + (m_iNumGridElements + 1) * 20;
            int iMaxScreenHeight = m_iScreenHeight - 100;
            int iMaxScreenWidth = m_iScreenWidth - 40;
            if (iHeight > iMaxScreenHeight)
                iHeight = iMaxScreenHeight;
            if (iWidth > iMaxScreenWidth)
                iWidth = iMaxScreenWidth;
            this.Size = new Size(iWidth, iHeight);
            buttonPane.Height = 17;
            buttonPane.Width = m_iWidthOfButtonPane;
            buttonPane.Location = new Point(2, iHeight - 69);
        }
        //****************************************************************************************************************************
        protected virtual void SetupLayout()
        {
            SetSizeOfGrid(m_iScreenWidth);
            General_DataGridView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(SelectRowButton_DoubleClick);
        }
        //****************************************************************************************************************************
        protected void General_DataGridView_CellFormatting(object sender,
            System.Windows.Forms.DataGridViewCellFormattingEventArgs e)
        {
            if (this.General_DataGridView.Columns[e.ColumnIndex].Name == "Release Date")
            {
                if (e != null)
                {
                    if (e.Value != null)
                    {
                        try
                        {
                            e.Value = DateTime.Parse(e.Value.ToString())
                                .ToLongDateString();
                            e.FormattingApplied = true;
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine("{0} is not a valid date.", e.Value.ToString());
                        }
                    }
                }
            }
        }
        //****************************************************************************************************************************
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // FGridDataView
            // 
            this.BackColor = System.Drawing.Color.LightSteelBlue;
            General_DataGridView.BackgroundColor = Color.LightSteelBlue;
            this.ClientSize = new System.Drawing.Size(1072, 693);
            this.Location = new System.Drawing.Point(100, 60);
            this.Name = "FGridDataView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;

            Filter_Button.Text = "Filter";
            Filter_Button.Location = new Point(10, 0);
            Filter_Button.Size = new Size(75, 17);
            Abort_Button.Text = "Abort";
            Abort_Button.Location = new Point(112, 0);
            Abort_Button.Size = new Size(75, 17);

            buttonPane.Controls.Add(Abort_Button);
            buttonPane.Controls.Add(Filter_Button);
            buttonPane.Dock = DockStyle.None;
            this.Controls.Add(this.buttonPane);
            
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        //****************************************************************************************************************************
    }
}
