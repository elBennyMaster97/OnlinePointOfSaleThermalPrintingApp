using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OnlineReceiptPrintingApp
{
    public class ConfigManager
    {

        private const string Key = "YourSecretKey997";
        private const string IV = "1234567890654321";
        private static string filePath = null;
        public static ConfigData objConfig = null;

        public static void SaveConfig(string printer, string printerSize, string printerLabel, string printerLabelSize, bool cut, bool drawer, bool breaklines)
        {

            string data = $"{printer},{printerSize},{printerLabel},{printerLabelSize},{cut},{drawer},{breaklines}";

            //encriptar datos
            byte[] encryptedData = EncryptStringToBytes_Aes(data, Key, IV);

            //guardamos datos en el archivo
            filePath = Application.StartupPath + "\\config\\config.txt";
            File.WriteAllBytes(filePath, encryptedData);

        }


        public static void LoadConfig()
        {

            filePath = Application.StartupPath + "\\config\\config.txt";

            byte[] encryptedData = File.ReadAllBytes(filePath);

            string decryptedData = DecryptStringFromBytes_Aes(encryptedData, Key, IV);


            string[] parts = decryptedData.Split(',');

            if (parts.Length != 7)
            {
                throw new ArgumentException("El archivo de configuración es inválido");
            }


            objConfig = new ConfigData
            {
                Printer = parts[0],
                PrinterSize = parts[1],
                PrinterLabel = parts[2],
                PrinterLabelSize = parts[3],
                Cut = bool.Parse(parts[4]),
                Drawer = bool.Parse(parts[5]),
                BreakLines = bool.Parse(parts[6])
            };

        }


        //encriptar
        public static byte[] EncryptStringToBytes_Aes(string plainText, string key, string IV)
        {
            byte[] encrypted;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.IV = Encoding.UTF8.GetBytes(IV);

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            return encrypted;
        }


        //desencriptar
        private static string DecryptStringFromBytes_Aes(byte[] cipherText, string key, string IV)
        {
            string plaintext = null;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.IV = Encoding.UTF8.GetBytes(IV);

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
    }
    public class ConfigData
    {
        public string Printer { get; set; }
        public string PrinterSize { get; set; }
        public string PrinterLabel { get; set; }
        public string PrinterLabelSize { get; set; }
        public bool Cut { get; set; }
        public bool Drawer { get; set; }
        public bool BreakLines { get; set; }
    }
}

