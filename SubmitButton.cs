using System;
using System.Data;
using System.Text;
using System.Windows.Forms;
using SQL_Library;

namespace WasteDisposalPermits
{
    public partial class SubmitButton : Form
    {
        private string m_sDefault;
        private bool m_bIntergerResultDesired = true;
        public SubmitButton(string slabel,
                            string sDefault)
        {
            if (sDefault.Length == 0)
                m_bIntergerResultDesired = false;
            InitializeComponent();
            Submit_label.Text = slabel;
            Submit_textBox.Text = sDefault;
            m_sDefault = sDefault;
        }
        //****************************************************************************************************************************
        public char GetPermitType()
        {
            this.Close();
            return Submit_textBox.Text.ToString().ToUpper()[0];
        }
        //****************************************************************************************************************************
        public int GetResponse()
        {
            this.Close();
            return Submit_textBox.Text.ToInt();
        }
        //****************************************************************************************************************************
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!ExitForm())
            {
                e.Cancel = true;
            }
        }
        //****************************************************************************************************************************
        private bool ExitForm()
        { 
            if (Submit_textBox.Text == m_sDefault)
                Submit_textBox.Text = "99";
            else if (Submit_textBox.Text.ToString().Length == 0)
                Submit_textBox.Text = "0";
            else if (m_bIntergerResultDesired && Submit_textBox.Text.ToString() != "0")
            {
                int iResponse = Submit_textBox.Text.ToInt();
                if (iResponse == 0)
                {
                    MessageBox.Show("Invalid numeric result");
                    return false;
                }
            }
            return true;
        }
        //****************************************************************************************************************************
        private void TextBoxKeyUp(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char) Keys.Enter)
            {
                this.Close();
                e.Handled = true;
            }
            else if (e.KeyChar == (char) Keys.Escape)
            {
                Submit_textBox.Text = "0";
                this.Close();
                e.Handled = true;
            }
        }
    }
}
