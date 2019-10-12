using LPsolve1.GroupPacking;
using LPsolve1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPsolve1.Excel
{
    public class Write
    {
        public static bool Write_2_Excel()
        {
            try
            {
                Microsoft.Office.Interop.Excel.Application oXL;
                Microsoft.Office.Interop.Excel._Workbook oWB;
                Microsoft.Office.Interop.Excel._Worksheet oSheet;
                Microsoft.Office.Interop.Excel.Range oRng;
                object misvalue = System.Reflection.Missing.Value;

                
                







                //Start Excel and get Application object.
                oXL = new Microsoft.Office.Interop.Excel.Application();
                oXL.Visible = true;


                


                //Get a new workbook.
                oWB = (Microsoft.Office.Interop.Excel._Workbook)(oXL.Workbooks.Add(""));
                oSheet = (Microsoft.Office.Interop.Excel._Worksheet)oWB.ActiveSheet;
                oSheet.EnableSelection = Microsoft.Office.Interop.Excel.XlEnableSelection.xlNoSelection;



                //Add table headers going cell by cell.
                oSheet.Cells[1, 1] = "First Name";
                oSheet.Cells[1, 2] = "Last Name";
                oSheet.Cells[1, 3] = "Full Name";
                oSheet.Cells[1, 4] = "Salary";


                oSheet.Cells[2, 1] = "First Name";
                oSheet.Cells[2, 2] = "Last Name";
                oSheet.Cells[2, 3] = "Full Name";
                oSheet.Cells[2, 4] = "Salary";

                //Format A1:D1 as bold, vertical alignment = center.
                //oSheet.get_Range("A1", "D1").Font.Bold = true;
                //oSheet.get_Range("A1", "D1").VerticalAlignment =
                //    Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;

                // Create an array to multiple values at once.
                string[,] saNames = new string[5, 2];

                saNames[0, 0] = "John";
                saNames[0, 1] = "Smith";
                saNames[1, 0] = "Tom";

                saNames[4, 1] = "Johnson";

                //Fill A2:B6 with an array of values (First and Last Names).
                oSheet.get_Range("A2", "B6").Value2 = saNames;

                //Fill C2:C6 with a relative formula (=A2 & " " & B2).
                oRng = oSheet.get_Range("C2", "C6");
                oRng.Formula = "=A2 & \" \" & B2";

                //Fill D2:D6 with a formula(=RAND()*100000) and apply format.
                oRng = oSheet.get_Range("D2", "D6");
                oRng.Formula = "=RAND()*100000";
                oRng.NumberFormat = "$0.00";

                //AutoFit columns A:D.
                oRng = oSheet.get_Range("A1", "D1");
                oRng.EntireColumn.AutoFit();

                oXL.Visible = false;
                oXL.UserControl = false;
                oWB.SaveAs(@"C:\test.xlsx", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing,
                    false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                oWB.Close();

            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        internal static bool Log_2_Excel_logger_GroupPack(List<Logger> logger)
        {
            try
            {
                Microsoft.Office.Interop.Excel.Application oXL;
                Microsoft.Office.Interop.Excel._Workbook oWB;
                Microsoft.Office.Interop.Excel._Worksheet oSheet;
                //Microsoft.Office.Interop.Excel.Range oRng;
                object misvalue = System.Reflection.Missing.Value;





                //Start Excel and get Application object.
                oXL = new Microsoft.Office.Interop.Excel.Application();

                oWB = oXL.Workbooks.Add();
                //oXL.Visible = true;
                //oXL.Quit();

                //Add table headers going cell by cell.
                oXL.Cells[1, 1] = "CheqID";
                oXL.Cells[1, 2] = "مبلغ پایه چک";                
                oXL.Cells[1, 3] = "باقیمانده چک";
                oXL.Cells[1, 4] = "فاکتور";
                oXL.Cells[1, 5] = "مانده فاکتور";



                //Get a new workbook.
                //oWB = (Microsoft.Office.Interop.Excel._Workbook)(oXL.Workbooks);
                //oSheet = (Microsoft.Office.Interop.Excel._Worksheet)oWB.ActiveSheet;
                //oSheet.EnableSelection = Microsoft.Office.Interop.Excel.XlEnableSelection.xlNoSelection;



                int row = 2;
                foreach (var log in logger)
                {
                    oXL.Cells[row, 2] = log.Base;
                    oXL.Cells[row, 1] = log.CheqID;
                    oXL.Cells[row, 3] = log.currentValue;
                    oXL.Cells[row, 4] = log.factor;
                    oXL.Cells[row, 5] = log.mandeFactor;




                    //Console.WriteLine(bin.Title + "  ----------------------------- " + "Base = " + bin.Base + "  current = " + bin.CurrentBedehi);
                    //foreach (var i in bin.Container)
                    //    Console.WriteLine("         ||| ID Cheq: " + i.ID + " Left: " + i.ValueCurrent + " form : " + i.ValueBase);

                    row++;
                }




                //oXL.Visible = false;
                //oXL.UserControl = false;
                //oWB.SaveAs(@"C:\test.xlsx", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing,
                //    false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange,
                //    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);


                // This works.
                oWB.SaveAs(@"C:\MappingResult2.xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal,
                    System.Reflection.Missing.Value, System.Reflection.Missing.Value, false, false,
                    Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlShared, false, false, System.Reflection.Missing.Value,
                    System.Reflection.Missing.Value, System.Reflection.Missing.Value);

                // This does not!?
                //excelWorkBook.SaveAs("C:\\MyExcelTestTest.xlsx", Excel.XlFileFormat.xlWorkbookNormal,
                //    System.Reflection.Missing.Value, System.Reflection.Missing.Value, false, false,
                //    Excel.XlSaveAsAccessMode.xlShared, false, false, System.Reflection.Missing.Value,
                //    System.Reflection.Missing.Value, System.Reflection.Missing.Value);

                oWB.Close(misvalue, misvalue, misvalue);
                //oWB.Close();
                oXL.Quit(); //------------

            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        internal static bool Log_2_Excel_4_GroupPack(List<Bin> bins)
        {
            try
            {
                Microsoft.Office.Interop.Excel.Application oXL;
                Microsoft.Office.Interop.Excel._Workbook oWB;
                Microsoft.Office.Interop.Excel._Worksheet oSheet;
                //Microsoft.Office.Interop.Excel.Range oRng;
                object misvalue = System.Reflection.Missing.Value;





                //Start Excel and get Application object.
                oXL = new Microsoft.Office.Interop.Excel.Application();

                oWB = oXL.Workbooks.Add();
                //oXL.Visible = true;
                //oXL.Quit();

                //Add table headers going cell by cell.
                oXL.Cells[1, 1] = "Markaz";
                oXL.Cells[1, 2] = "Company";
                oXL.Cells[1, 3] = "Cheq ID";
                oXL.Cells[1, 4] = "remain";
                oXL.Cells[1, 5] = "value";
                oXL.Cells[1, 6] = "Base Bedehi";
                oXL.Cells[1, 7] = "Current Bedehi";



                //Get a new workbook.
                //oWB = (Microsoft.Office.Interop.Excel._Workbook)(oXL.Workbooks);
                //oSheet = (Microsoft.Office.Interop.Excel._Worksheet)oWB.ActiveSheet;
                //oSheet.EnableSelection = Microsoft.Office.Interop.Excel.XlEnableSelection.xlNoSelection;



                int row = 2;
                foreach (var bin in bins)
                {
                    oXL.Cells[row, 1] = bin.CodeMarkaz;
                    oXL.Cells[row, 2] = bin.Title;                    
                    oXL.Cells[row, 6] = bin.Base;
                    oXL.Cells[row, 7] = bin.CurrentBedehi;

                    foreach (var i in bin.CheqDetails)
                    {
                        oXL.Cells[row, 3] = i.Key;
                        oXL.Cells[row, 4] = i.Value;
                        row++;
                    }


                    //Console.WriteLine(bin.Title + "  ----------------------------- " + "Base = " + bin.Base + "  current = " + bin.CurrentBedehi);
                    //foreach (var i in bin.Container)
                    //    Console.WriteLine("         ||| ID Cheq: " + i.ID + " Left: " + i.ValueCurrent + " form : " + i.ValueBase);

                    row++;
                }




                //oXL.Visible = false;
                //oXL.UserControl = false;
                //oWB.SaveAs(@"C:\test.xlsx", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing,
                //    false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange,
                //    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);


                // This works.
                oWB.SaveAs(@"C:\MappingResult.xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal,
                    System.Reflection.Missing.Value, System.Reflection.Missing.Value, false, false,
                    Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlShared, false, false, System.Reflection.Missing.Value,
                    System.Reflection.Missing.Value, System.Reflection.Missing.Value);

                // This does not!?
                //excelWorkBook.SaveAs("C:\\MyExcelTestTest.xlsx", Excel.XlFileFormat.xlWorkbookNormal,
                //    System.Reflection.Missing.Value, System.Reflection.Missing.Value, false, false,
                //    Excel.XlSaveAsAccessMode.xlShared, false, false, System.Reflection.Missing.Value,
                //    System.Reflection.Missing.Value, System.Reflection.Missing.Value);

                oWB.Close(misvalue, misvalue, misvalue);
                //oWB.Close();
                oXL.Quit(); //------------

            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public static bool Log_2_Excel(List<Models.Bin> bins)
        {
            try
            {
                Microsoft.Office.Interop.Excel.Application oXL;
                Microsoft.Office.Interop.Excel._Workbook oWB;
                Microsoft.Office.Interop.Excel._Worksheet oSheet;
                //Microsoft.Office.Interop.Excel.Range oRng;
                object misvalue = System.Reflection.Missing.Value;





                //Start Excel and get Application object.
                oXL = new Microsoft.Office.Interop.Excel.Application();

                oWB = oXL.Workbooks.Add();
                //oXL.Visible = true;
                //oXL.Quit();

                //Add table headers going cell by cell.
                oXL.Cells[1, 1] = "Company";
                oXL.Cells[1, 2] = "Cheq ID";
                oXL.Cells[1, 3] = "remain";
                oXL.Cells[1, 4] = "value";
                oXL.Cells[1, 5] = "Base Bedehi";
                oXL.Cells[1, 6] = "Current Bedehi";



                //Get a new workbook.
                //oWB = (Microsoft.Office.Interop.Excel._Workbook)(oXL.Workbooks);
                //oSheet = (Microsoft.Office.Interop.Excel._Worksheet)oWB.ActiveSheet;
                //oSheet.EnableSelection = Microsoft.Office.Interop.Excel.XlEnableSelection.xlNoSelection;



                int row = 2;
                foreach (var bin in bins)
                {
                    oXL.Cells[row, 1] = bin.Title;
                    oXL.Cells[row, 5] = bin.Base;
                    oXL.Cells[row, 6] = bin.CurrentBedehi;

                    foreach (Cheq i in bin.Container)
                    {
                        oXL.Cells[row, 2] = i.ID;
                        oXL.Cells[row, 3] = i.ValueCurrent;
                        oXL.Cells[row, 4] = i.ValueBase;
                        row++;
                    }


                    //Console.WriteLine(bin.Title + "  ----------------------------- " + "Base = " + bin.Base + "  current = " + bin.CurrentBedehi);
                    //foreach (var i in bin.Container)
                    //    Console.WriteLine("         ||| ID Cheq: " + i.ID + " Left: " + i.ValueCurrent + " form : " + i.ValueBase);

                    row++;
                }




                //oXL.Visible = false;
                //oXL.UserControl = false;
                //oWB.SaveAs(@"C:\test.xlsx", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing,
                //    false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange,
                //    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);


                // This works.
                oWB.SaveAs(@"C:\MappingResult.xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal,
                    System.Reflection.Missing.Value, System.Reflection.Missing.Value, false, false,
                    Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlShared, false, false, System.Reflection.Missing.Value,
                    System.Reflection.Missing.Value, System.Reflection.Missing.Value);

                // This does not!?
                //excelWorkBook.SaveAs("C:\\MyExcelTestTest.xlsx", Excel.XlFileFormat.xlWorkbookNormal,
                //    System.Reflection.Missing.Value, System.Reflection.Missing.Value, false, false,
                //    Excel.XlSaveAsAccessMode.xlShared, false, false, System.Reflection.Missing.Value,
                //    System.Reflection.Missing.Value, System.Reflection.Missing.Value);

                oWB.Close(misvalue, misvalue, misvalue);
                //oWB.Close();
                oXL.Quit(); //------------

            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }
    }
}
