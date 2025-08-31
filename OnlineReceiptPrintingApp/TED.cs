
using System;
using System.Drawing;

namespace OnlineReceiptPrintingApp
{
    public class TED
    {

        public static Bitmap getTEDNode(string JsonXml)
        {

            string xml3 = JsonXml.Replace("TED version=\"1.0\"", "TED version=\'1.0\'")
                .Replace("CAF version=\"1.0\"", "CAF version=\'1.0\'")
                .Replace("\"SHA1withRSA\"", "\'SHA1withRSA\'");



            return Creapdf417(xml3);

        }


        // Método para genera la imagen del timbre Chileno con formato pdf417, en nuestro ejercicio no la utilizaremos
        public static Bitmap Creapdf417(String dd)
        {
            // para generar el timbre pdf417 se requiere:
            // 1)-. instalar el package (iTextSharp) v5.4.5.0
            // 2)-. importar  using iTextSharp.text.pdf;

            Bitmap imagen = null;

            try
            {
                //BarcodePDF417 pdf417 = new BarcodePDF417();
                //pdf417.Options = BarcodePDF417.PDF417_USE_ASPECT_RATIO;
                //pdf417.ErrorLevel = 8;

                //pdf417.Options = BarcodePDF417.PDF417_FORCE_BINARY;

                //Encoding iso = Encoding.GetEncoding("ISO-8859-1");
                //byte[] isoBytes = iso.GetBytes(dd);

                //pdf417.Text = isoBytes;


                //imagen = new Bitmap(pdf417.CreateDrawingImage(Color.Black, Color.White));
                //imagen.Save(Application.StartupPath + "\\timbres\\timbre.png");

                return imagen;


            }
            catch (Exception)
            {
                return imagen;
            }





        }

    }
}
