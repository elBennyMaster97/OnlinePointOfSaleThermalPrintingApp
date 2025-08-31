using System.IO.Compression;
using System.IO;
using System;
using System.Text;


namespace OnlineReceiptPrintingApp
{
    public class clsFunciones
    {

        public static string Base64Decode(string base64EncodedData)
        {

            // obtenemos el array de bytes
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);

            // obtener cadena con la secuencia de bytes decodificados
            string result = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);

            return result;
        }

        public static string Decompress(string base64Encoded)
        {

            string jsonOriginal = null;

            try
            {
                byte[] compressedData = Convert.FromBase64String(base64Encoded);

                using (MemoryStream ms = new MemoryStream(compressedData))
                {

                    using (GZipStream gzip = new GZipStream(ms, CompressionMode.Decompress))
                    {

                        using (StreamReader reader = new StreamReader(gzip, Encoding.UTF8))
                        {

                            jsonOriginal = reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            return jsonOriginal;
        }


    }
}
