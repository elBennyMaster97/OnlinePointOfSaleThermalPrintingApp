using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineReceiptPrintingApp
{
    public class App
    {

        public static string moneyFormat(JToken valor)
        {

            try
            {
                decimal numero = Convert.ToDecimal(valor);

                NumberFormatInfo formatoUSA = new CultureInfo("en-us").NumberFormat;

                string numberResult = numero.ToString("#,##0.00", formatoUSA);

                return numberResult;
            }
            catch (Exception)
            {
                return "formatErr";
            }
        }

        public static string moneyFormat(decimal numero)
        {

            try
            {

                NumberFormatInfo formatoUSA = new CultureInfo("en-us").NumberFormat;

                string numberResult = numero.ToString("#,##0.00", formatoUSA);

                return numberResult;
            }
            catch (Exception)
            {
                return "formatErr";
            }
        }


        public static bool NoBreakLines()
        {

            string rootFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string textFile = rootFolder + @"\no_breaklines.txt";

            return (File.Exists(textFile));
        }

    }
}
