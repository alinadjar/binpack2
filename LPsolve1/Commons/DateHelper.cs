using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPsolve1.Commons
{
    public class DateHelper
    {
        public static string GetDate_Standard()
        {
            PersianCalendar pc = new PersianCalendar();
            int month = pc.GetMonth(DateTime.Now);
            int day = pc.GetDayOfMonth(DateTime.Now);

            string Y = pc.GetYear(DateTime.Now).ToString();
            string M = month >= 10 ? month.ToString() : "0" + month.ToString();
            string D = day >= 10 ? day.ToString() : "0" + day.ToString();

            return Y + "/" + M + "/" + D;
        }

        public static string GetTime_24Standdard()
        {
            return DateTime.Now.ToString("HH:mm");

            //date.ToString("HH:mm:ss"); // for 24hr format
            //date.ToString("hh:mm:ss"); // for 12hr format, it shows AM/PM
        }


        public static int Convert_StndDate_2_Int(string input)
        {
            string[] strsplit = input.Split('/');

            string mon = strsplit[1];
            string day = strsplit[2];

            return Convert.ToInt32(strsplit[0] + mon + day);
        }
    }
}
