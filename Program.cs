using System;
using System.IO;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Text;
using Utilities;
using SQL_Library;

namespace WasteDisposalPermits
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(String[] args)
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                if (args.Length == 0)
                {
                    args = new string[] { "Contacts" };
                    //args = new string[] { "Permits" };
                }
                Application.Run(new PermitForm(args));
            }
            catch (HistoricJamaicaException e)
            {
                UU.ShowErrorMessage(e);
            }
            catch (Exception e)
            {
                MessageBox.Show("Fatal Error: " + e.Message);
            }
        }
    }
}
