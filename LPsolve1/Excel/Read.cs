using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LPsolve1.Excel
{
    public class Read
    {

        
        public static void readColumns()
        {
            string excelFinalPath = @"C:\Users\Ali_Nadjar\Documents\Visual Studio 2017\Projects\LPsolve1\LPsolve1\Excel\data.xlsx";
            Microsoft.Office.Interop.Excel.Application application = new Microsoft.Office.Interop.Excel.Application();
            Workbook workBook = application.Workbooks.Open(excelFinalPath);
            /////////////////////////////////////////////////////////////////

            for (int i = 1; i <= workBook.Sheets.Count; i++)
            {
                Worksheet worksheet = workBook.Worksheets[i];
                object cellValue = ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[2, 1]).Value;
                //listBox1.Items.Add(cellValue);
                //MessageBox.Show(cellValue.ToString());
            }

            /////////////////////////////////////////////////////////////////
            workBook.Close(false, excelFinalPath, null);
            Marshal.ReleaseComObject(workBook);
        }


        public static DataSet LoadAllSheets_2_Dataset()
        {
            string excelFinalPath = @"C:\Users\Ali\Desktop\LPsolve1\LPsolve1\LPsolve1\Excel\data.xlsx";
            Microsoft.Office.Interop.Excel.Application application = new Microsoft.Office.Interop.Excel.Application();
            Workbook workBook = application.Workbooks.Open(excelFinalPath);
            /////////////////////////////////////////////////////////////////
            DataSet dataSet = new DataSet();

            for (int i = 1; i <= workBook.Sheets.Count; i++)
            {
                Worksheet worksheet = workBook.Worksheets[i];

                Range excelCell = worksheet.UsedRange;
                Object[,] sheetValues = (Object[,])excelCell.Value;
                int noOfRows = sheetValues.GetLength(0); // first dimention size
                int noOfColumns = sheetValues.GetLength(1);//second dimention size

                //add column names to datatable
                System.Data.DataTable dataTable = new System.Data.DataTable();
                for (int j = 1; j <= noOfColumns; j++)
                {
                    dataTable.Columns.Add(new DataColumn(((Range)worksheet.Cells[1, j]).Value));
                }

                //as first column has header, start at second row
                for (int k = 2; k <= noOfRows; k++)
                {
                    DataRow dataRow = dataTable.NewRow();
                    for (int l = 1; l <= noOfColumns; l++)
                    {
                        dataRow[l - 1] = ((Range)worksheet.Cells[k, l]).Value;
                    }
                    dataTable.Rows.Add(dataRow);
                }
                dataSet.Tables.Add(dataTable);
            }
            /////////////////////////////////////////////////////////////////
            workBook.Close(false, excelFinalPath, null);
            Marshal.ReleaseComObject(workBook);

            return dataSet;
        }



        
    }
}
