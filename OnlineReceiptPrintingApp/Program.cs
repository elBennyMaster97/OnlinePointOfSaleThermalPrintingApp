using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace OnlineReceiptPrintingApp
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        [STAThread]
        static void Main(string[] args)
        {
            // crear protocolo
            CreateNinjaProtocol();

            //size 58mm            
            //2 productos
            // args = new string[] { "printpos://eNqtU9uO2jAU/JXUzwE5JuGSpyJaVitBWSEq9arobGzAUm71hZIC/95jCCyL1BUPzUtyjsczPjPOjkhO4o5PrBYqcd+BT1KrTZk3NfOJKQ1kJCYBjfptSolPpBG5Pq5pA8biJ6kA0YitK4FVCnqN1fGFGyNKTxvTNRQrBwhD1nSUACN4Aga7jLKwRaMW7S8YjWkY0y6i3PONHPbfd6RSJbepOZ/0l4XCSFMfCw2ZSColU8ffHwxO/FzqtLSFY2/O0HCQeEcamgJyt+dp5D1ALosVORz811rsLa0guleLXbRGVunSm4CCjchQ7+f+FjEVmdTgTWwBSAScK6Gd0UO1EniQY1fkIDGZwmYZaq3LQpyLJocp1KWSGJL+p9XhIhjEtBOHL1bjdaj4vVAuMnGGOu3D/tbXiZXaG8P25cAkw9YSYPt+5RrttMzPi8lGKLmUV4RXl2yYGrkRJ1+XMnPkQ46J4YQKeKneGrIXB0HM2D1D3kLdSFr+Qbmof7FWiVTIypCreZ+tlgWmlDSDPy680Wz6NPz09VWC84/j2Xw69CIa+N70h6U0HXx5HM28D+1xm1yCJFHU6vQGLcaCnrtLBrYV1M1vSeafFwHrhFEXVzZugqDrbJGFQUSj36e58zUTtSjcngcFqQTtVaXytPXQ9UrBO0T8Fs8a/+lLMFtIgYu8boL5H572Y0rjsHcFRVYuTcKhRlOC6PAXcdFacQ==" };
            //1 producto
            //args = new string[] { "print://eNqtU9uO2jAQ/ZXUz4DskHDxUxEt1UpQqhWVelU0aw9gKbf6Qsmy/HttCJSu1Gofmhd7xsdn5pyJD0RJwpMOcQZ1FvasQ4QztiraOO4QW1nICScsHfcoJR2iLBbmBDUWrPNbUoMHe2hTo48EmK2PTgsnMaXne2IL5SacJ6xNaASLMgN7gsVJl6ZdOlrFlLMBTxKPCt8Xcnz6eiC1rqQT9tLWDwelVbY5NwI5ZrVWAm8blcqIypWBvW2h5SD8QFqaEopwZ+q0qaI5aNhhTo7H70/PEQvMlYFo7krwRCClRhOkT/QGfSOnLBagvFWly3Nfa1uVeAlaZxbQVFp528xf5ScrNua0z5PBVb6fTy1fCpWY4wUaah9bJeyqZO6UiWaw/90wyX1qDbB/vQmJnqiKy2G2Q63W6obwZuwTYdUOz76uVR7IJ7JQpVeoQVb6XyKHnDEexy8R+RwaJBn16Mulo6u1GgWq2pIbvQ/OqNJPKWuF362i6XLxYfL+8x8TvH87W94vJlFKWSdafHOUivGnu+kyetOb9ch1kCRNu/3huBvHbBj+JQv7Gpr2nZD7jysW95N04E92QQEbBFtUaT2irT+iRfA1xwbLcOedBqHARHWlI+Mi73qt4ZVH/MQH41/ZdTB7ECCxaNrB/A9PR5xSngxvoJ5VKptJaMLjTo+/ADmOOH4=" };

            //3 PRODUCTOS 
            // args = new string[] { "printpos://eNqlWNtu2zgQ/RXCz1mHlHXN06ZJLwhiIECB9mGzD2OJtrmVREcii9rZ/vsOncQSZYqJswZiG5J8ODxz5swwj5NW7PjkYpLSydlEbTfm+6aRhc5V+1WWxeT3v4+TpSyFxBszSlkQ0YTGYYSPyw1vQAlZ4627p9/IlnzjdSEK/FLwklzfa0p5Afh0q0DpFh/9tr/GSnwIL+uWN3jxq0awPy6LStR4kVcgSrzaSKn+bM2tKTzfKkDxfSj355TdnwcRockFjS/CiGwqjPavx4koJhdpdPayD3z6C1QL3aw0b00kDxpqJdQWbzBq9t1CaTBDSumUPr/2Ecv8x+SC0d9nT6AJ7YPe4a8UkEogbimUHCBHHXBMp/ER7AGVsaAP+0EqqKElqwZqZHADrVBg2CT5WuayNPu3Vwq6lVjq2sHssIHQ2oDIgXxqjknpAYaRD4/RzBe6qFda1LIdct6LN3LFG7zgBwN8vVhAKSVRWollg2/jyEHiAO44n1En8JzXCjyg6TSOhqBRB8r6oFcv6SJXhpUhbtaDpdMgGiWB0dRDcgWGZFN4vM7XooRiyHfaoztw0Z10Spx5Vip0mQuykhUMFkh6C1CXXg4MWfL7+BzvEC7uEcNc8aZdwNRi/FMJWsH9uYJc7guG56Ie0B71g/WXO2PUW5joc2oYfA8+eaV0Qg94zhu+G2KHlqN4jcpylLkof2zJd9iSuajFKCYLXZisq8XUWTIOA+mBZtMsHmLGL5BpYtvRbgcmbQ/o03I8THfWWEds7CMWclhrU5QIB4U8keHDKhYXX6Qi13JlY81eUUJ2UHFqKeEzlKav7B3fBI0WMgrsdGfW2Sdzu9xa8OYnLDSvx6GTaRaMG0VqcXyjV5Lc6Ir/Ip81bGEBBP2MVOUoesR8PjSzbRRN4kGLkrejcG7bSbMXwMzu29UCdnIcLaPedKEALdPBPgQklzXZypVu1Po0GfQLLDnKFvn4SzW84mQuGyCXOz1OaRB6rT22gl7iYHWaqmadLQ7iHCswbKW8kSdq98BGZi1yLWvwjEC9gWUWeksiy+wWtHcBGEVzukCWHlJmJ6xpTCMeBQudYIeOEB2panyTryg0tnZ5udJA2DS6JbeiQp0arW44TmdydIEo9E5PiWV/tzrH/M8133/gPDIchfuF6gr8YIQzhxE+pV20m2E77MNG3naYBqO47on6NeTwIAAr4ju+aQXBguBlCQSPE0MH7AEHgZcJu9fiqMEbgaDjFAROB+wMOzgybHM++2m48Jp2n4vEFzKL4yPRfcCyXcpaEfZeIjJ7ktHLpajJClPXqeJtiQxm3kRmlj1+h+Wy9GBRLxazs9frjmZYl/+P5oBajNzyfM0JlBuN/WHpOMu9OX1HycO5zRumK29RdxhKBk1XE/N3tR9s3+kPlsDm0pQxP227rNuv/R+C/ghTdBUR+Ws48ko3tIfwWpDqSb+najf1y40xTz9eNuIfWR73pbd7HA5Xb+z3z+fPdy8Up+62dQMViHxcNSNn/b//A4Zvk4k=" };
            //args = new string[] { "printpos://eNqNkM1OwzAQhF/F8jm0dtqgNicC6aFSRBGBXiiHlW1aQ2Kb2D6U0ndnU5U/qUhYsrTeGc236x31+k3RnE4YTWjYur52nZVRBF/bRtL9+44+2UZbFFjG0jRjnE9TjnbrVAdBW4PSze2ivL+6W9RkObsu5yUW5awi5SoyJmTRh9sADc35mCdUB9V6mo8S6gOEiCVdHpy8mpdFje7oVYfdOiLjrJCtNthULWiMoJ214cL30gCOkoSgDhOuhixdDdOMMJ7zaZ5y4lpc4mFHtUT6+TT53A/txToC4YOsIpVurSECr1NOG4uRrxFM0GGLvhE+PTSHj+IDdjx9M1jxgrH75JjPsp/5l7izAU/WHRjVEAECNhEnJcqIjW5A/s0ZZ6c4X6B0xP4FegZnjfK/Mfwbw09hJvvHD8roocc=" };

            //58mm
            //args = new string[] { "web-print-service-app-http://eNplVF1vmzAU/SvIz0lkiDqlPI02aMvUJFNC99FtQo7tJFbBZv5Im7b577sGQmnHA9I9Pj7ce+69PKOKVFznRjxxFKOLCRoge6x4XsOAaE65qCzAG2eE5MbkQm6VLokVSqLYascH6EHYfU7YgWsrgLVD8ZYUBg4MKXjOiPXiEY4uhngyjCZBFMYYxzhCp5dnJBiKx5cDtFWFUMD7ulzjcOjZeBJNhtg/40vIoCKC5aRUTlqghXiEMaBMGNpCLXIgvcDYyrxGlJh9L9oTueOvsbC8BHLYeAA4JZrVGsQ6r+IzgNgZsKxOGzScsapsY+mKAi5zSaStAZCimkP9kLjte3CZ4XHjwaguEN+hV62zEH+0muTVsbRnxOxFVYHBOVWmAysuWY3VxXns9PLrGVVaMUebPKIB+usgKWGPdVKa71xBdF5pQXnPzP/cbUVQ3PQJdCQp/Q1jWBDaTbB1+hjcw+chdbkLAnQ6/WmbijuyVJL36svv+fGMdhPQka/SxSxbBp/T1eK3w5iGi2l6F6xHZ+Oby7d4nGXD7+HPTiHsnG8YzRxNP8z7w9t+Yz5bX6c3ySJNgnX66XYxXfZJhFpxqK1C2SxdTJNgmgbJVbJaLbN0DUxeElHA6YZLYdWea8N3H3ceHFFVetv2vrYYeUsZ0yAKwSqZp1/SQfBtdnOTwPs6W65mySBIp8t5+sNvHnmsyLGdJXSeZLCx4EfocYN1wwi1bgrO6iYJqJpxW2f13o02ajf5zaoXZMOLFmlmiWnyAAlQ95aoeaW0fcMU8qBgeN5hWwWjCS2mgkAq4ck3h8BfgbpCtaulrD9qJ8yf9+c3P+9k/QM5vXjdd69/CiR/Hw==" };
            //args = new string[] { "web-print-service-app-http://" };
            //80mm
            //args = new string[] { "web-print-service-app-http://eNplVF1vmzAU/SvIz0lkiDqlPI02aMvUJFNC99FtQo7tJFbBZv5Im7b577sGQmnHA9I9Pj6ce3zNM6pIxXVuxBNHMZpgNED2WPG8hgHRnHJRWYA3zgjJjcmF3CpdEiuURLHVjg/Qg7D7nLAD11YAa4fiLSkMLBhS8JwR68UjHF0M8WQYTYIojDGOcYROL89IMBSPLwdoqwqhgPd1ucbh0LPxJJoMsX/Gl+CgIoLlpFROWqCFeIS9XSYMbaEWOZBeYWxlXitKzL5X7Ync8ddaWF4COWwyAJwSzWoNYp1X8Q6gdgYiq22DhjNWlW0tXVHAZi6JtDUAUlRz6B+M234GlxkeNxmM6gbxHXrVOgvxR6tJXh1Le0bMXlQVBJxTZTqw4pLVWN2cx04vv55RpRVztPERDdBfB6aEPdamNN+5gui80oLyXpj/pduKoLg5J9CRpPQ7jGFBaDfB1uljcA+fB+tyFwTodPrTHiruyFJJ3usvv+fHM9pNQEe+ShezbBl8TleL3w5jGi6m6V2wHp2Dbzbf4nGWDb+HPzuFsEu+YTRzNP0w7w9v+435bH2d3iSLNAnW6afbxXTZJxFqxaGOCmWzdDFNgmkaJFfJarXM0jUweUlEAasbLoVVe64N333ceXBEVelj2/veYuQjZUyDKBSrZJ5+SQfBt9nNTQLv62y5miWDIJ0u5+kPf/PIY0WO7Syh8yRDjAU/whk3WDeM0Oum4Kw+JAFdM25rV+/TaKv2Jr+56gXZ8KJFmllimjyAAeo64sUE+c2V0vYNU8iDguF5h20VjCYcMRUErIQnfzgE/grUFaq9Wsr6pXbC/Hp/fvPznax/IKcXr/vu9Q/vSn8V" };
            //args = new string[] { "web-print-service-app-http://eNqFVFtv4jgU/iuWn1OakOlleXNJZiYSJDSXajXbUWQSU6wJccZ26NKW/77HuRRajbQ8AOfzd+6XV9zQhslc8ReGZ/jWxhbWh4blHQyIZAXjjQZ43SpeM6VyXm+E3FHNRY1nWrbMws9cb3Na7pnUHFhPI14yVUhQ76g4WK6yRULQgiTowQ9T+JlHIYhotSAp+RrFS4LuMx+lMQmTXkwzFPrfonkQIT9EsZ9ki5R4UTJBSZCkPjBWWZhGyPN7kygKF0HoQ7zPbK24NllttW7U7PHy8VLxpi3ZXkzWW9YINekIFla0YnlJO/LUnl5d2DcXzl/IsWdfvsymt/j49op5iWeOhTei4gJotuOYUglNK5Bcd2Kb0pVcFaKttSH0yJ6eCQriOEkFVdsz3WJL6yd2eobQdsCe9g0BvOMbI1S3xkxDISYLtwr6Z6JzwUartNgNct1WFSizmtY6H8IvJIM0y7yL6pRq6lz3qYJr8/mBT7ZGQ+xfLWneHHZ6RNSWNw10Oy+EegcbVpcd1mVnsOPbP6+4kaJsiz4O99bCv1uIiutDF5VkT21FZd5IXphMneu+BHQ3FHMEBit41vfDGKrpzqjcCXCKEroWEn2VTFF0Zdu7Ch+P1kfnjnv9f95vPnu/+aP3ztLgHhxLrqlCcbvZVEyh+5Ypge5E9UJfKETxcxgh+12lFjU7K3P+ix1GdJw395Qeq7kW6DuT9WNr28ypS/aCksnY/145s900vfCX9+cTOwxAz1hFie1cTP/2zhd68PHHN1povu8qhaHAUFiGVIueuBSPl/1bSc0UNlsT+DADtCyBqUaxYgcYCrBgNj27WwTzwCMe8hOUhQT1ew77O4+WWRjMyTwwKZZuiFbkG/GIhcIIrfw4iUKysLoDkaXBIvgBWsGDwRO09L0AflYkBhdxtDQHIySx+e9l8zRKLJT48QN4BlaEAs8nAN1lyZyEHlifdO3lUKeSacqr9x5/XqDhHn44mBVds2pA+oxLSZ+hK0X7Try6xUa5EVJ/YPJ6L2DsPmEbATsFQ1Fwc16co2knhdtatJUYbkJ/eVzXPJ3vXD7ekQ2tFDu+GZOfvv4DDCDiSw==" };
            //args = new string[] { "web-print-service-app-http://eNqFVNtu2kAQ/ZXVPgOxcZNQ3kjitEjEEGyqqqWyFnuAVY3X3QspJPx7Z30BgiKVB6M5e2bmzGX3lRasABkrvgfapz2HtqjeFRCXMCISEuCFRnhhFM9BqZjnSyE3THOR076WBlr0het1zNItSM2RtWrwFFQi0b2k0uHTZDYKB2Q0CMk3P4jw734coEkmo0E0eBxPnwbkeeaTaDoIwsqMZiTwv4zvh2PiB2Tqh7NRNHgYhx0SDsPIR8ZkFkRj8uBXIck4GA0DH/W+wEJxbataa12o/vxqfqV4YVLYis5iDYVQnZLQooplEKesJHed7nXbuW27n4nr9D996nd79PD2SnlK+26LLkXGBdIc17WtEpplaHlex7GtS7lKhMm1JVTIlp0ZCnWcrISp9Zlvsmb5Ck7HKG2D7G41EMRLvg3CtLFhCoaaWtQonJ9V52EMo7TY1HZusgydIWe5jmv5iQQsM41LVadSI/emKhVT298PeorVBIK/WrK42G10g6g1LwqcdpwIdQQLyNMSK6uz2OHt5ystpEhNUunwei36x6AqrnelKgkrkzEZF5IntlL3pmoB29TNbIA6Cu1X87CBcraxLncCk5KQLYQkjxIUI9eOs8no4dB6n9z1bv6X/fYy++2H2ctIdXpMLLlmikzNcpmBIs8GlCB3ItuzPUMVv+oVco4uucjhrM3xb9g1aLNv3qk8yLkW5CvIfG4cB9w8hT0JO838K+eZ40VR2396Pt/YegEqxmQcOm67+/3h/ELXOT48Y4nm27JTFBuMjQWiDFlxKeZX1VnK7BYWayu83gGWpshUjZnBDpcCI5Q95CgmBc14dmzk5ZbWj867VyljC8hqpAqbSvaCpSfmSLzuUetcCKnfMXm+FTjbC2wpcHGx8wm3d9g92J4xfMASk4n64lXX2/Ps0flix81lXbJMweHNhrz4/AOTWbjt" };

            //validar data
            if (args == null || args.Length == 0)
            {
                MessageBox.Show("NO HAY INFO PARA IMPRIMIR", "APLICACION DE IMPRESION DE COMPROBANTES EN LINEA", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                System.Environment.Exit(0);
            }



            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            if (!File.Exists(Application.StartupPath + "\\config\\config.txt"))
            {

                Application.Run(new frmConfig());
            }
            else
            {

                ConfigManager.LoadConfig();

                //acciones de imprimir
                //print://StringBase64
                string saleInfo = args[0].Substring(29);

                //decode base 64
                string re = clsFunciones.Base64Decode(saleInfo);

                //json original
                string planeJson = clsFunciones.Decompress(saleInfo);

                //deserealizar json
                Documents.PrintDoc(planeJson);

                //MessageBox.Show(re);
                // descomprimir

            }


        }



        static async Task MIIP()
        {
            try
            {
                // Obtener todas las interfaces de red
                NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();

                // Buscar la interfaz Ethernet
                NetworkInterface ethernetInterface = interfaces.FirstOrDefault(
                    ni => ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet && ni.OperationalStatus == OperationalStatus.Up);

                if (ethernetInterface != null)
                {
                    // Obtener las propiedades de configuración de la interfaz Ethernet
                    IPInterfaceProperties ipProps = ethernetInterface.GetIPProperties();

                    // Recorrer las direcciones IP asociadas a la interfaz Ethernet
                    foreach (UnicastIPAddressInformation ip in ipProps.UnicastAddresses)
                    {
                        // Filtrar direcciones IPv4
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            MessageBox.Show($"Dirección IP local de Ethernet: {ip.Address}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No se encontró un adaptador de Ethernet conectado.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }


        }


        private static void CreateProtocol()
        {

            try
            {
                // abrir la clave del registro para verifricar si el protocolo prnit ya esta registrado
                RegistryKey key = Registry.ClassesRoot.OpenSubKey(@"print");

                if (key == null)
                {

                    RegistryKey subKeyPrint = Registry.ClassesRoot.CreateSubKey("print");
                    subKeyPrint.SetValue(null, "URL:print");
                    subKeyPrint.SetValue("EditFlags", 2);
                    subKeyPrint.SetValue("URL Protocol", "");

                    //se crea la subclave shell
                    RegistryKey subKeyShell = subKeyPrint.CreateSubKey("shell");
                    subKeyShell.SetValue(null, "open");


                    // crear subclave open
                    RegistryKey subKeyOpen = subKeyShell.CreateSubKey("open");
                    subKeyOpen.SetValue(null, "");

                    // crear subkey command
                    RegistryKey subKeyCommand = subKeyOpen.CreateSubKey("command");

                    //"C:\apps\boletas\printerApp.exe" "%1"

                    string rootFoolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                    string pathFile = rootFoolder + @"\config\path.txt";

                    if (!File.Exists(pathFile))
                    {
                        subKeyCommand.SetValue(null, "\"C:\\apps\\printer\\elBennyPrinterAppForWebPOS.exe\" \"%1\"");
                    }
                    else
                    {

                        string letter = File.ReadAllText(pathFile);

                        subKeyCommand.SetValue(null, $"\"{letter}:\\apps\\printer\\elBennyPrinterAppForWebPOS.exe\" \"%1\"");
                    }

                    MessageBox.Show("PROTOCOLO PRINT REGISTRADO CORRECTAMENTE.", "Online Receipt Printing App", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Online Receipt Printing App", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }


        private static void CreateNinjaProtocol()
        {

            try
            {
                // abrir la clave del registro para verifricar si el protocolo print ya esta registrado
                RegistryKey key = Registry.ClassesRoot.OpenSubKey(@"web-print-service-app-http");

                if (key == null)
                {

                    RegistryKey subKeyPrint = Registry.ClassesRoot.CreateSubKey("web-print-service-app-http");
                    subKeyPrint.SetValue(null, "URL:print");
                    subKeyPrint.SetValue("EditFlags", 2);
                    subKeyPrint.SetValue("URL Protocol", "");

                    //se crea la subclave shell
                    RegistryKey subKeyShell = subKeyPrint.CreateSubKey("shell");
                    subKeyShell.SetValue(null, "open");


                    // crear subclave open
                    RegistryKey subKeyOpen = subKeyShell.CreateSubKey("open");
                    subKeyOpen.SetValue(null, "");

                    // crear subkey command
                    RegistryKey subKeyCommand = subKeyOpen.CreateSubKey("command");

                    //"C:\apps\boletas\printerApp.exe" "%1"

                    string rootFoolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                    string pathFile = rootFoolder + @"\config\path.txt";

                    if (!File.Exists(pathFile))
                    {
                        subKeyCommand.SetValue(null, "\"C:\\Apps\\OnRePriApp\\OnlineReceiptPrintingApp.exe\" \"%1\"");
                    }
                    else
                    {

                        string letter = File.ReadAllText(pathFile);

                        subKeyCommand.SetValue(null, $"\"{letter}:\\Apps\\OnRePriApp\\OnlineReceiptPrintingApp.exe\" \"%1\"");
                    }

                    MessageBox.Show("SE REGISTRÓ CORRECTAMENTE EL PROTOCOLO: web-print-service-app-http", "APLICACION DE IMPRESION DE COMPROBANTES EN LINEA", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "APLICACION DE IMPRESION DE COMPROBANTES EN LINEA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

    }


}
