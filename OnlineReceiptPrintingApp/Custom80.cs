using System;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;


namespace OnlineReceiptPrintingApp
{
    public class Custom80
    {

        //crear un objeto de la clase StringBuilder, en este objeto agregaremos las lineas a imprimir
        StringBuilder linea = new StringBuilder();

        //variable para almacenar el numero maximo de caracteres que soporta el papel 80mm
        int maxCarTextIzq = clsCommon.maxCarTextoIzquierda;

        //variable para controlar el corte del texto cuando rebase el límite de caracteres soportados por el papel
        int cortar;

        //variable para controlar e imprimir texto alineado a la derecha
        int maxCarTextDer = clsCommon.maxCarTextoDerecha;

        //variable para controlar e imprimir texto alineado al centro
        int maxCar_Centrar = clsCommon.maxCar_Centrar;

        //variable para imprimir separador de texto a través de asteriscos y/o guiones
        int maxCar_Guiones_y_Astericos = clsCommon.maxCar_Guiones_y_Astericos;

        //varaiable para controlar la impresión de espacios vacíos dinámicamente en la cabecera o conceptos de la venta
        int espaciosDinamicos = clsCommon.articulo_rigth_space;

        // variable para controlar los espacios al alinear los totales de la venta hacia la derecha
        int tNumEspaciosTotales = clsCommon.maxCarTotales;

        // variable para controlar la logitud de la descripción del producto y corte de línea hacia abajo cuando la descripción supera la cantidad de caracteres permitidos
        int productLenght = clsCommon.productMax_lenght;


        // método para imprimir el logotipo
        public void printLogo(string printerName)
        {

            PrintDocument printDoc = new PrintDocument();
            printDoc.PrintController = new StandardPrintController();
            printDoc.PrinterSettings.PrinterName = printerName;
            printDoc.PrintPage += new PrintPageEventHandler(DibujarLogo80mm);
            printDoc.Print();
        }

        // método para dibujar el logotipo
        public void DibujarLogo80mm(object sender, PrintPageEventArgs e)
        {
            PictureBox logo_ticket = new PictureBox();

            if (File.Exists(Application.StartupPath + "\\config\\logo.png"))
            {
                logo_ticket.Image = Image.FromFile(Application.StartupPath + "\\config\\logo.png");

                e.Graphics.DrawImage(logo_ticket.Image, 20, 0, 220, 115);
            }
        }

        // método para imprimir el qrcode
        public void PrintQrCode(Bitmap qrCode, string printerName)
        {
            var printDocument = new PrintDocument();
            printDocument.PrintPage += (sender, args) =>
            {
                // dimensiones del qr
                int imageWidth = 70;
                int imageHeight = 70;
                int marginLeft = (args.PageBounds.Width - 85) / 2;
                Rectangle imageRect = new Rectangle(marginLeft, 10, imageWidth, imageHeight);

                // dibujar la imagen en el rectángulo
                args.Graphics.DrawImage(qrCode, imageRect);
            };
            printDocument.PrinterSettings.PrinterName = printerName;
            printDocument.Print();
        }


        // método para imprimir texto centrado
        public void PrintTextCenter(string texto, float fontSize = 10, bool bold = false)
        {
            string stringToPrint = texto;
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;

            PrintDocument pd = new PrintDocument();
            pd.PrintPage += (sender, ev) =>
            {
                // denifir fuente
                Font drawFont = new Font("Courier New", fontSize, bold ? FontStyle.Bold : FontStyle.Regular);
                SolidBrush drawBrush = new SolidBrush(System.Drawing.Color.Black);

                System.Drawing.RectangleF rect = new System.Drawing.RectangleF(-5, 0, 300, ev.MarginBounds.Height);

                ev.Graphics.DrawString(stringToPrint, drawFont, drawBrush, rect, sf);

                ev.HasMorePages = false;
            };
            pd.PrinterSettings.PrinterName = ConfigManager.objConfig.Printer;
            pd.Print();
        }

        public void barcode(System.Drawing.Image barcode)
        {

            // Definir los parámetros de impresión
            float labelWidth = 80; // Ancho de la etiqueta en mm
            float labelHeight = 50; // Alto de la etiqueta en mm
            float marginLeft = 5; // Margen izquierdo en mm
            float centerY = 10; // Posición vertical del centro en mm

            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;

            // Calcula el nuevo ancho y alto de la imagen para mantener la proporción
            float barcodeOriginalWidth = barcode.Width;
            float barcodeOriginalHeight = barcode.Height;
            float imageWidth = labelWidth - marginLeft * 2;
            float imageHeight = barcodeOriginalHeight * (imageWidth / barcodeOriginalWidth);

            // centra la imagen
            float imageX = marginLeft;
            float imageY = centerY + labelHeight - imageHeight / 2;

            // configurar documento de impresión
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += (sender, ev) =>
            {
                // dibujar la imagen del código de barras centrada en la página
                ev.Graphics.DrawImage(barcode, new RectangleF(imageX, imageY, 250f, 30f));
            };

            // Imprimir el documento
            pd.PrinterSettings.PrinterName = ConfigManager.objConfig.Printer;
            pd.Print();
        }

        // método para imprimir con acentos y ñ's, alineando el texto a la izquierda, centro y derecha
        public void PrintText(string texto, string alineacion = "Izquierda", int fontSize = 10, bool boldText = false)
        {
            PrintDocument pd = new PrintDocument();
            pd.DefaultPageSettings.PaperSize = new PaperSize("A4", 595, 842);
            pd.DefaultPageSettings.Margins = new Margins(5, 5, 2, 2);
            pd.PrinterSettings.PrinterName = ConfigManager.objConfig.Printer;

            pd.PrintPage += (sender, e) =>
            {
                Font font = new Font("Courier New", fontSize, FontStyle.Bold);

                float x = e.MarginBounds.Left;
                float y = e.MarginBounds.Top;

                StringFormat format = new StringFormat();
                if (alineacion == "Derecha")
                {
                    format.Alignment = StringAlignment.Far;
                }
                else if (alineacion == "Centro")
                {
                    format.Alignment = StringAlignment.Center;
                }
                else
                {
                    format.Alignment = StringAlignment.Near; // Izquierda por defecto
                }

                e.Graphics.DrawString(texto, font, Brushes.Black, x, y, format);
            };


            pd.Print();
        }




        // método para imprimir el separador de texto con guiones
        public string lineasGuion()
        {
            string lineasGuion = "";
            for (int i = 0; i < maxCar_Guiones_y_Astericos; i++)
            {
                lineasGuion += "-";//Agregara un guio hasta llegar la numero maximo de caracteres.
            }
            return linea.AppendLine(lineasGuion).ToString(); //Devolvemos la lineaGuion
        }


        // método para imprimir el separador de texto con asteriscos
        public string lineasAsteriscos()
        {
            string lineasAsterisco = "";
            for (int i = 0; i < maxCar_Guiones_y_Astericos; i++)
            {
                lineasAsterisco += "*";//Agregara un asterisco hasta llegar la numero maximo de caracteres.
            }
            return linea.AppendLine(lineasAsterisco).ToString(); //Devolvemos la linea con asteriscos
        }

        // método para imprimir el separador de texto con el signo igual ( = )
        public string lineasIgual()
        {
            string lineasIgual = "";
            for (int i = 0; i < maxCar_Guiones_y_Astericos; i++)
            {
                lineasIgual += "=";//Agregara un igual hasta llegar la numero maximo de caracteres.
            }
            return linea.AppendLine(lineasIgual).ToString(); //Devolvemos la lienas con iguales
        }

        // método para imprimir los conceptos o cabecera de la venta
        public void EncabezadoVenta()
        {
            //ejemplo de salida cabeceras o conceptos para 80mm:
            //linea.AppendLine("ARTICULO                   |CANT|PRECIO|IMPORTE");

            string lineaGenerada = GenerarLineaConEspaciosDinamicos("ARTICULO", "|CANT|PRECIO|IMPORTE", espaciosDinamicos);

            linea.AppendLine(lineaGenerada);

        }

        // método para imprimir y posicionar los conceptos de venta
        public string GenerarLineaConEspaciosDinamicos(string parteIzquierda, string parteDerecha, int espacios)
        {
            int longitudTotal = parteIzquierda.Length + parteDerecha.Length;

            StringBuilder linea = new StringBuilder(parteIzquierda);

            for (int i = 0; i < espacios; i++)
            {
                linea.Append(' ');
            }

            linea.Append(parteDerecha);

            return linea.ToString();
        }

        // método para poner el texto a la izquierda
        public void TextoIzquierda(string texto)
        {

            if (clsCommon.TraeAcentos(texto))
            {
                PrintText(texto);
                return;
            }


            //Si la longitud del texto es mayor al numero maximo de caracteres permitidos, realizar el siguiente procedimiento.
            if (texto.Length > maxCarTextIzq)
            {
                int caracterActual = 0;//Nos indicara en que caracter se quedo al bajar el texto a la siguiente linea
                for (int longitudTexto = texto.Length; longitudTexto > maxCarTextIzq; longitudTexto -= maxCarTextIzq)
                {
                    //Agregamos los fragmentos que salgan del texto
                    linea.AppendLine(texto.Substring(caracterActual, maxCarTextIzq));
                    caracterActual += maxCarTextIzq;
                }
                //agregamos el fragmento restante
                linea.AppendLine(texto.Substring(caracterActual, texto.Length - caracterActual));
            }
            else
            {
                //Si no es mayor solo agregarlo.
                linea.AppendLine(texto);
            }
        }

        // método para imprimir texto alineado a la derecha
        public void TextoDerecha(string texto)
        {

            if (clsCommon.TraeAcentos(texto))
            {
                PrintText(texto, "Derecha");
                return;
            }

            //Si la longitud del texto es mayor al numero maximo de caracteres permitidos, realizar el siguiente procedimiento          
            if (texto.Length > maxCarTextDer)
            {
                int caracterActual = 0;//indica en que caracter se queda al bajar el texto a la siguiente linea
                for (int longitudTexto = texto.Length; longitudTexto > maxCarTextDer; longitudTexto -= maxCarTextDer)
                {
                    //agregar la subcadena a la linea
                    linea.AppendLine(texto.Substring(caracterActual, maxCarTextDer));
                    caracterActual += maxCarTextDer;
                }
                // espacios restantes
                string espacios = "";

                //obtener longitud del texto restante
                for (int i = 0; i < (maxCarTextDer - texto.Substring(caracterActual, texto.Length - caracterActual).Length); i++)
                {
                    espacios += " "; // agregar espacios para alinear a la derecha
                }

                //agregar la subcadena restante
                linea.AppendLine(espacios + texto.Substring(caracterActual, texto.Length - caracterActual));
            }
            else
            {
                string espacios = "";
                // obtener la subcadena / longitud del texto restante
                for (int i = 0; i < (maxCarTextDer - texto.Length); i++)
                {
                    espacios += " "; // agregar espacios para alinear a la derecha
                }
                //agregar el texto con los espacios 
                linea.AppendLine(espacios + texto);
            }
        }

        //Metodo para centrar el texto
        public void TextoCentrado(string texto)
        {
            if (string.IsNullOrEmpty(texto)) texto = "";
            if (clsCommon.TraeAcentos(texto))
            {
                PrintText(texto, "Centro");
                return;
            }

            if (texto.Length > maxCar_Centrar)
            {
                int caracterActual = 0;// indica en cual caracter se queda al bajar el texto a la siguiente línea
                for (int longitudTexto = texto.Length; longitudTexto > maxCar_Centrar; longitudTexto -= maxCar_Centrar)
                {
                    // agregar la subcadena del texto
                    linea.AppendLine(texto.Substring(caracterActual, maxCar_Centrar));
                    caracterActual += maxCar_Centrar;
                }
                // espacios restantes
                string espacios = "";

                //sacamos la cantidad de espacios libres y el resultado lo dividimos entre dos
                int centrar = (maxCar_Centrar - texto.Substring(caracterActual, texto.Length - caracterActual).Length) / 2;

                // obtener la subcadena/longitud del texto restante
                for (int i = 0; i < centrar; i++)
                {
                    espacios += " "; // agregar espacios para centrar
                }

                // agrega el texto con los espacios
                linea.AppendLine(espacios + texto.Substring(caracterActual, texto.Length - caracterActual));
            }
            else
            {
                //si el texto supera la cantidad de caracteres máximos

                string espacios = ""; //espacios restantes
                //obtener cantidad de espacios libres y el resultado lo dividimos entre dos
                int centrar = (maxCar_Centrar - texto.Length) / 2;

                // obtener la subcadena/longitud del texto restante
                for (int i = 0; i < centrar; i++)
                {
                    espacios += " "; // agrega espacios para centrar
                }

                //agregamos el texto restante precedido con los espacios
                linea.AppendLine(espacios + texto);

            }
        }



        // método para agregar los totales de la venta
        public void AgregarTotales(string texto, decimal total)
        {
            //Variables que usaremos
            string subtexto, valor, textoCompleto, espacios = "";

            // si es mayor a 25 lo cortamos
            if (texto.Length > 25)
            {
                cortar = texto.Length - 25;
                subtexto = texto.Remove(25, cortar);
            }
            else
            {
                subtexto = texto;
            }

            textoCompleto = subtexto;
            valor = total.ToString("#,#.00");// formato

            // obtener número de espacios restantes para alinearlos a la derecha    
            int nroEspacios = tNumEspaciosTotales - (subtexto.Length + valor.Length);

            // agregar los espacios
            for (int i = 0; i < nroEspacios; i++)
            {
                espacios += " ";
            }
            textoCompleto += espacios + valor;
            linea.AppendLine(textoCompleto);
        }

        // método para agregar los artículos después de las cabeceras de la venta
        public void AgregarProducto(string articulo, decimal cant, decimal precio, decimal subtotal)
        {

            // validar que la longitud de (cantidad, precio e importe) esten dentro del rango permitido
            if (cant.ToString().Length <= 6 && precio.ToString().Length <= 7 && subtotal.ToString().Length <= 8)
            {
                string elemento = "", espacios = "";
                bool bandera = false;//Indicara si es la primera linea que se escribe cuando bajemos a la segunda si el nombre del articulo no entra en la primera linea
                int nroEspacios = 0;

                // si la longitud o cantidad de caracteres en el nombre del artículo es mayor a 20, hacemos breakline para bajar a la siguiente línea
                if (articulo.Length > productLenght)
                {
                    // colocar la cantidad a la derecha
                    nroEspacios = (5 - cant.ToString().Length);
                    espacios = "";
                    for (int i = 0; i < nroEspacios; i++)
                    {
                        espacios += " ";// agregar los espacios necesarios para alinear a la derecha
                    }
                    elemento += espacios + cant.ToString(); // agregar la cantidad + espacios

                    // colocar precio a la derecha
                    nroEspacios = (7 - precio.ToString().Length);
                    espacios = "";
                    for (int i = 0; i < nroEspacios; i++)
                    {
                        espacios += " "; // generar los espacios
                    }

                    // agregamr precio a la variable elemento
                    elemento += espacios + precio.ToString();

                    // colocar el importe a la derecha
                    nroEspacios = (8 - subtotal.ToString().Length);
                    espacios = "";
                    for (int i = 0; i < nroEspacios; i++)
                    {
                        espacios += " "; // espacios
                    }
                    // agregamos el importe alineado a la derecha
                    elemento += espacios + subtotal.ToString();

                    int caracterActual = 0; //indica en cuál caracter se queda al bajar a la siguiente línea

                    // por cada 20 caracteres se agregara una línea siguiente
                    for (int longitudTexto = articulo.Length; longitudTexto > productLenght; longitudTexto -= productLenght)
                    {
                        if (bandera == false)//si es false o la primera línea en iterar, continuar
                        {
                            //agregar los primeros 20 caracteres del nombre del artículo + lo que ya tiene la variable elemento
                            linea.AppendLine(articulo.Substring(caracterActual, productLenght) + elemento);
                            bandera = true; // cambiamos a verdadero
                        }
                        else
                        {
                            // solo agregar el nombre del artículo
                            linea.AppendLine(articulo.Substring(caracterActual, productLenght));
                        }

                        caracterActual += productLenght; // incrementar en 20 el valor de la variable caracterActual
                    }
                    // agregar el resto del subtexto del  nombre del artículo
                    linea.AppendLine(articulo.Substring(caracterActual, articulo.Length - caracterActual));

                }
                else //si no es mayor solo agregamos el artículo sin saltos de línea
                {
                    for (int i = 0; i < (productLenght - articulo.Length); i++)
                    {
                        espacios += " "; // agregar espacios para completar los 20 caracteres
                    }
                    elemento = articulo + espacios;

                    // colocar la cantidad a la derecha
                    nroEspacios = (5 - cant.ToString().Length);
                    espacios = "";
                    for (int i = 0; i < nroEspacios; i++)
                    {
                        espacios += " ";
                    }
                    elemento += espacios + cant.ToString();

                    // colocar precio a la derecha
                    nroEspacios = (7 - precio.ToString().Length);
                    espacios = "";
                    for (int i = 0; i < nroEspacios; i++)
                    {
                        espacios += " "; // espacios
                    }
                    elemento += espacios + precio.ToString();

                    // colocar importe a la derecha
                    nroEspacios = (8 - subtotal.ToString().Length);
                    espacios = "";
                    for (int i = 0; i < nroEspacios; i++)
                    {
                        espacios += " "; // espacios
                    }
                    elemento += espacios + subtotal.ToString();

                    //finalmente agregamos al stringBuilder todo el elemento (nombre del artículo, cantidad, precio, importe)
                    linea.AppendLine(elemento);
                }
            }
            else
            {
                //lanzamos una excepción para mostrar al usuario el desbordamiento
                linea.AppendLine("La longitud de cantidad,precio o importe");
                linea.AppendLine("supera el ancho permitido");
                throw new Exception("Los valores de alguna fila supera el ancho/longitud permitido");
            }
        }

        // métodos para enviar secuencias de escape a la impresora / cortar el papel
        public void CortaTicket()
        {
            linea.AppendLine("\x1B" + "m"); // caracteres de corte (estos comandos pueden varian según el tipo de impresora)
            linea.AppendLine("\x1B" + "d" + "\x0"); // avanzar 9 renglones(pueden variar, revisa la doc de tu impresora)
        }
        public void Cortar()
        {
            string GS = Convert.ToString((char)29);
            string ESC = Convert.ToString((char)27);
            string COMMAND = "";
            COMMAND = ESC + "@";
            COMMAND += GS + "V" + (char)48;

            linea.AppendLine(COMMAND);

        }

        // método para intentar abrir el cajon del dinero (cash drawer)
        public void AbreCajon()
        {
            // estos comandos pueden variar (revisa la doc de tu impresora)
            linea.AppendLine("\x1B" + "p" + "\x00" + "\x0F" + "\x96"); // caracteres de apertura cajon 0
            //linea.AppendLine("\x1B" + "p" + "\x01" + "\x0F" + "\x96"); //Caracteres de apertura cajon 1
        }

        // método para imprimir el documento final
        public void ImprimirTicket(string impresora)
        {
            RawPrinterHelper.SendStringToPrinter1(impresora, linea.ToString());
            linea.Clear();
        }


    }


    // clase para enviar a imprimir texto plano a la impresora, se conoce como rawprint / impresión directa de solo texto
    public class RawPrinterHelper
    {
        // Structure and API declarions:
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public class DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDocName1;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pOutputFile1;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDataType1;
        }
        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool OpenPrinter1([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool ClosePrinter1(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartDocPrinter1(IntPtr hPrinter, Int32 level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

        [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndDocPrinter1(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartPagePrinter1(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndPagePrinter1(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool WritePrinter1(IntPtr hPrinter, IntPtr pBytes, Int32 dwCount, out Int32 dwWritten);

        // SendBytesToPrinter()
        // When the function is given a printer name and an unmanaged array
        // of bytes, the function sends those bytes to the print queue.
        // Returns true on success, false on failure.
        public static bool SendBytesToPrinter1(string szPrinterName, IntPtr pBytes, Int32 dwCount)
        {
            Int32 dwError = 0, dwWritten = 0;
            IntPtr hPrinter = new IntPtr(0);
            DOCINFOA di = new DOCINFOA();
            bool bSuccess = false; // Assume failure unless you specifically succeed.

            di.pDocName1 = "Sale_Receipt";
            di.pDataType1 = "RAW";

            // Open the printer.
            if (OpenPrinter1(szPrinterName.Normalize(), out hPrinter, IntPtr.Zero))
            {
                // Start a document.
                if (StartDocPrinter1(hPrinter, 1, di))
                {
                    // Start a page.
                    if (StartPagePrinter1(hPrinter))
                    {
                        // Write your bytes.
                        bSuccess = WritePrinter1(hPrinter, pBytes, dwCount, out dwWritten);
                        EndPagePrinter1(hPrinter);
                    }
                    EndDocPrinter1(hPrinter);
                }
                ClosePrinter1(hPrinter);
            }
            // If you did not succeed, GetLastError may give more information
            // about why not.
            if (bSuccess == false)
            {
                dwError = Marshal.GetLastWin32Error();
            }
            return bSuccess;
        }

        public static void SendStringToPrinter1(string printerName, string text)
        {
            IntPtr pBytes;
            Int32 dwCount;

            dwCount = text.Length;
            pBytes = Marshal.StringToCoTaskMemAnsi(text);

            SendBytesToPrinter1(printerName, pBytes, dwCount);
            Marshal.FreeCoTaskMem(pBytes);
        }
        public static bool SendStringToPrinter(string szPrinterName, string szString)
        {
            IntPtr pBytes;
            Int32 dwCount;
            // How many characters are in the string?
            dwCount = szString.Length;
            // Assume that the printer is expecting ANSI text, and then convert
            // the string to ANSI text.
            pBytes = Marshal.StringToCoTaskMemAnsi(szString);
            // Send the converted ANSI string to the printer.
            SendBytesToPrinter1(szPrinterName, pBytes, dwCount);
            Marshal.FreeCoTaskMem(pBytes);
            return true;
        }
        public static bool SendStringToPrinterENCODING(string szPrinterName, string szString)
        {
            IntPtr pBytes;
            Int32 dwCount;

            // Convert the string to bytes using UTF-8 encoding
            byte[] bytes = Encoding.UTF8.GetBytes(szString);

            dwCount = bytes.Length;
            pBytes = Marshal.AllocCoTaskMem(dwCount);
            Marshal.Copy(bytes, 0, pBytes, dwCount);

            SendBytesToPrinter1(szPrinterName, pBytes, dwCount);

            Marshal.FreeCoTaskMem(pBytes);
            return true;
        }



    }
}
