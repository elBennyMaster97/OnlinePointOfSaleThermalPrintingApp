using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing.Common;
using ZXing;
using System.Drawing.Printing;

namespace OnlineReceiptPrintingApp
{
    public class barcodeHelper
    {

        // método para imprimir códigos de barras
        public static Bitmap createBarcode(string codigo, BarcodeFormat formatoCodigo, int height = 200, int width = 400)
        {


            try
            {
                // establecer configuraciones y dimensiones de la imagen
                BarcodeWriter barcodeWriter = new BarcodeWriter
                {
                    Format = formatoCodigo,
                    Options =
                    {
                        Height = height,
                        Width = width,
                        PureBarcode = true,
                    }

                };
                // generar código de barras
                Bitmap codigoBarraImg = barcodeWriter.Write(codigo);

                //retornar código / imagen
                return codigoBarraImg;


            }
            catch (Exception)
            {
                return null;
            }

        }

        // método para generar qrcode
        public static Bitmap createQrCode(string text, int height = 150, int width = 150)
        {


            try
            {
                var writer = new BarcodeWriter
                {

                    Format = BarcodeFormat.QR_CODE,
                    Options = new EncodingOptions
                    {
                        Width = width,
                        Height = height,
                        Margin = 1
                    }

                };
                return writer.Write(text);
            }
            catch (Exception)
            {
                return null;
            }

        }


    }
}

