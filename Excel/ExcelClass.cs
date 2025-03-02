using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Collections;
using OfficeOpenXml;

namespace WasteDisposalPermits
{
    public class ExcelClass : IDisposable
    {
        ExcelPackage ExcelPackageOutput;
        protected OfficeOpenXml.ExcelWorksheet outputWorksheet;
        private bool _notYetDisposed = true;
        protected string workbookPath;
        //****************************************************************************************************************************
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Dispose();
                // dispose managed resources
            }
            // free native resources
        }
        //****************************************************************************************************************************
        protected void CreateNewExcelWorkbook(string name)
        {
            try
            {
                ExcelPackageOutput = new ExcelPackage();
                outputWorksheet = ExcelPackageOutput.Workbook.Worksheets.Add(name);
                workbookPath = @"c:\WasteDisposalPermits\" + name + ".xlsx";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //****************************************************************************************************************************
        public void Dispose()
        {
            if (_notYetDisposed)
            {
                if (ExcelPackageOutput != null)
                {
                    try
                    {
                        CloseOutput();
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Output File Must Be Open. Close and Try Again");
                        CloseOutput();
                    }
                }
                MessageBox.Show("Report Complete");
            }
        }
        //****************************************************************************************************************************
        protected void CloseOutput()
        {
            if (outputWorksheet != null)
            {
                outputWorksheet.Cells[outputWorksheet.Dimension.Address].AutoFitColumns();
                outputWorksheet.View.FreezePanes(2, 1);
                ExcelPackageOutput.SaveAs(new FileInfo(workbookPath));
                outputWorksheet = null;
            }
        }
    }
}
