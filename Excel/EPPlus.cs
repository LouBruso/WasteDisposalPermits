using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OfficeOpenXml;
using System.IO;

namespace WasteDisposalPermits
{
    //****************************************************************************************************************************
    public class EPPlus : IDisposable
    {
        public ExcelPackage ExcelPackageOutput;
        public ExcelPackage excelPackageInput;
        public OfficeOpenXml.ExcelWorksheet inputWorksheet;
        public int numRows;
        public int numCols;
        protected OfficeOpenXml.ExcelWorksheet outputWorksheet;
        protected string workbookPath;
        private bool _notYetDisposed = true;
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
        public void GetExcelInputFile(string folder, out string filename)
        {
            string sFilter = "Excel Files (xls)|*.xlsx";
            filename = SelectFile(sFilter, folder);
            if (String.IsNullOrEmpty(filename))
            {
                return;
            }
        }
        //****************************************************************************************************************************
        private string SelectFile(string sFilter,
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
                _notYetDisposed = false;
            }
        }
        //****************************************************************************************************************************
        public void OpenWithEPPlus(string workbookPathname)
        {
            if (!File.Exists(workbookPathname))
            {
                throw new Exception("Input file does not exist: " + workbookPathname);
            }
            excelPackageInput = OpenExcelWorkbook(workbookPathname);
            if (excelPackageInput.Workbook.Worksheets.Count == 0)
            {
                throw new Exception("Input file does not exist: " + workbookPathname);
            }
            this.inputWorksheet = excelPackageInput.Workbook.Worksheets[1];
            this.numRows = inputWorksheet.Dimension.End.Row;
            this.numCols = inputWorksheet.Dimension.End.Column;
        }
        //****************************************************************************************************************************
        protected ExcelPackage OpenExcelWorkbook(string workbookPathname)
        {
            var fi = new FileInfo(workbookPathname);
            ExcelPackage excelPackage = new ExcelPackage(fi);
            return excelPackage;
        }
        //****************************************************************************************************************************
        public void CreateNewExcelWorkbook(string fileName)
        {
            try
            {
                workbookPath = fileName;
                ExcelPackageOutput = new ExcelPackage();
                outputWorksheet = ExcelPackageOutput.Workbook.Worksheets.Add(workbookPath);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //****************************************************************************************************************************
        public void CloseOutput()
        {
            outputWorksheet.Cells[outputWorksheet.Dimension.Address].AutoFitColumns();
            outputWorksheet.View.FreezePanes(2, 1);
            ExcelPackageOutput.SaveAs(new FileInfo(workbookPath));
        }
        //****************************************************************************************************************************
        public void WriteCellValue(string value, int rowIndex, int colIndex)
        {
            outputWorksheet.Cells[rowIndex, colIndex].Value = value;
        }
        //****************************************************************************************************************************
        public string GetCellValue(int rowIndex, int colIndex)
        {
            if (inputWorksheet.Cells[rowIndex, colIndex] == null)
            {
                return "";
            }
            if (inputWorksheet.Cells[rowIndex, colIndex].Value == null)
            {
                return "";
            }
            return inputWorksheet.Cells[rowIndex, colIndex].Value.ToString().Trim();
        }
        //****************************************************************************************************************************
    }
}

