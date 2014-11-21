using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Gate.WebReference;

namespace Gate
{
    static class Global
    {
        public static CardService cs = new CardService();

        public static string ErrorCode(int code)
        {
            switch (code)
            {
                case 1:
                    return "Not on file";
                case 2:
                    return "Access time";
                case 3:
                    return "Access day";
                case 4:
                    return "Access error";
                case 5:
                    return "Invalid site";
                case 7:
                    return "Passback error";
                case 8:
                    return "Pass empty";
                case 9:
                    return "Expired";
                default:
                    return "OK";
            }
        }
    }

}