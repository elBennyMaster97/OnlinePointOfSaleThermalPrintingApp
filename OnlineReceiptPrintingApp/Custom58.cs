using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using ZXing.Common;
using ZXing;
using System.Reflection;
using ZXing.QrCode.Internal;



namespace OnlineReceiptPrintingApp
{
    public class Custom58
    {


        public void PrintReceipt58(List<Tuple<string, bool, float, string>> companyLines, List<Tuple<string, string, float>> headerLines, dynamic printdetail, dynamic detail, List<Tuple<string, string>> totalesLines, List<Tuple<string, string>> payMethod, bool center = false, float margin = 0)
        {
            PrintDocument pd = new PrintDocument();
            pd.PrinterSettings.PrinterName = ConfigManager.objConfig.Printer;

            // establecer márgenes del documento a cero
            pd.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);

            pd.PrintPage += (sender, ev) =>
            {
                // ==================================================================== //
                //                  imprimir logo
                float yPos = margin;
                float logoWidth = 160;// ev.PageBounds.Width * 0.8f;  // ajusta según tus necesidades en porcentaje
                float logoHeight = 80;// (logoWidth / logo.Width) * logo.Height; 

                if (File.Exists(Application.StartupPath + "\\config\\logo.png"))
                {
                    Image logo = Image.FromFile(Application.StartupPath + "\\config\\logo.png");

                    // dibujar logo al inicio del documento
                    ev.Graphics.DrawImage(logo, (ev.PageBounds.Width - logoWidth) / 2, 0, logoWidth, logoHeight);
                    yPos += logoHeight;// + 10;
                }

                // posición Y del primer texto después del logo



                // ==================================================================== //


                // alineacion datos empresa
                SolidBrush drawBrush = new SolidBrush(Color.Black);
                StringFormat sf = new StringFormat();
                StringFormat companySF = new StringFormat();
                if (center)
                {
                    sf.Alignment = StringAlignment.Center;
                }
                else
                {
                    sf.Alignment = StringAlignment.Near;
                }




                // ==================================================================== //
                //          INFO DE LA EMPRESA / NEGOCIO
                // ==================================================================== //
                foreach (var line in companyLines)
                {
                    string text = line.Item1;
                    bool bold = line.Item2;
                    companySF.Alignment = (line.Item4 == "Center" ? StringAlignment.Center : StringAlignment.Near);

                    Font drawFont = new Font("Segoe UI", line.Item3, bold ? FontStyle.Bold : FontStyle.Regular);

                    // ajustar el rectángulo para que el texto se centre en el medio
                    RectangleF rect = new RectangleF(ev.MarginBounds.Left, yPos, ev.MarginBounds.Width, ev.MarginBounds.Height);

                    ev.Graphics.DrawString(text, drawFont, drawBrush, rect, companySF);

                    // ajustar la posición y para la siguiente línea
                    yPos += drawFont.GetHeight(ev.Graphics); // +5
                    //currentLine++;
                }

                // separador
                yPos += 10;
                yPos = TablePrinter58.PrintSeparator(ev.Graphics, ev, yPos, "dotted");
                

                // ==================================================================== //
                //              FOLIO, FECHA, CLIENTE
                // ==================================================================== //
                foreach (var sbLine in headerLines)
                {
                    string text = sbLine.Item1;
                    string alignment = sbLine.Item2;

                    Font drawFont = new Font("Segoe UI", sbLine.Item3, FontStyle.Regular);

                    // ajustar la alineación del texto según el parámetro alignment
                    if (alignment == "Center")
                    {
                        sf.Alignment = StringAlignment.Center;
                    }
                    else if (alignment == "Right")
                    {
                        sf.Alignment = StringAlignment.Far;
                    }
                    else // alineación defecto
                    {
                        sf.Alignment = StringAlignment.Near;
                    }

                    // ajustar el rectángulo para que el texto se centre 
                    RectangleF rect = new RectangleF(ev.MarginBounds.Left, yPos + 10, ev.MarginBounds.Width, ev.MarginBounds.Height);

                    ev.Graphics.DrawString(text, drawFont, drawBrush, rect, sf);

                    // ajustar la posición y para la siguiente línea
                    yPos += drawFont.GetHeight(ev.Graphics);// +5;
                    //currentLine++;
                }



                // ==================================================================== //
                //                      DETALLE / PRODUCTOS
                // ==================================================================== //
                decimal qty = 0;
                decimal subtotal = 0;
                string description = null;

                Font fontDetail = new Font("Segoe UI", 8, FontStyle.Regular);
                Font font = new Font("Segoe UI", 8);
                Font fontHeaders = new Font("Segoe UI", 7, FontStyle.Bold);

                int spaceWidth = (int)ev.Graphics.MeasureString(" ", font).Width;

                StringFormat itemSf = new StringFormat();

                // valores base para calcular altura de filas y posicionar contenidos en la tabla concptos
                int rowHeight = 20;
                int leftMargin = 0;
                int rightMargin = 5;

                // bordes y estilo de la tabla cocneptos
                Graphics graphics = ev.Graphics;
                Pen pen = new Pen(Color.Black, 0.5f);
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

                // definir ancho y posición de las columnas                
                float[] columnWidths = { ev.PageBounds.Width * 0.55f, ev.PageBounds.Width * 0.15f, ev.PageBounds.Width * 0.20f };
                float[] columnPositions = { margin, margin + columnWidths[0], margin + columnWidths[0] + columnWidths[1] };


                yPos += 15; // +5Espacio antes de la cabecera

                // dibujar cabecera de la tabla conceptos
                string[] headers = { "PRODUCTO", "CANT", "IMPORTE" };
                for (int i = 0; i < headers.Length; i++)
                {
                    float headerX = columnPositions[i];
                    ev.Graphics.DrawString(headers[i], fontHeaders, drawBrush, headerX, yPos);
                }

                // dibujar bordes tabla conceptos
                // Dibujar bordes de la tabla conceptos justo después de los encabezados
                graphics.DrawRectangle(pen, leftMargin, yPos, columnWidths[0], rowHeight);
                graphics.DrawRectangle(pen, leftMargin + columnWidths[0], yPos, columnWidths[1], rowHeight);
                graphics.DrawRectangle(pen, leftMargin + columnWidths[0] + columnWidths[1], yPos, columnWidths[2], rowHeight);

                yPos += rowHeight;


                foreach (var item in detail) //DETALLE PRODUCTOS
                {
                    qty = Convert.ToDecimal(item.quantity);
                    //price = Convert.ToDecimal(item.regular_price);
                    subtotal = Convert.ToDecimal(item.amount);
                    //subtotal = Convert.ToDecimal(qty * price);
                    //subtotal = Convert.ToDecimal((qty * price) - (((qty * price) * discount) / 100));
                    //subtotal = Math.Round(subtotal, 2);
                   // total += subtotal;

                    description = item.product_name;

                    string[] content = { description, qty.ToString(), App.moneyFormat(subtotal).ToString() };
                    float startYPosition = ev.MarginBounds.Top + rowHeight; // posición inicial en Y para este producto

                    // calcula el número de líneas de texto en este producto                       
                    int numLines = (int)Math.Ceiling(graphics.MeasureString(description, font, (int)columnWidths[0]).Height / rowHeight);

                    for (int j = 0; j < content.Length; j++)
                    {
                        float contentX = columnPositions[j];
                        RectangleF contentRect = new RectangleF(contentX, yPos, columnWidths[j], 20);
                        sf.Alignment = j > 0 ? StringAlignment.Far : StringAlignment.Near;
                        ev.Graphics.DrawString(content[j], fontDetail, drawBrush, contentRect, sf);
                    }



                    // 
                    // ajustar la posición para la siguiente línea
                    yPos += (int)font.GetHeight();
                    //yPos += rowHeight;

                }

                // separador
                yPos += 10;
                yPos = TablePrinter58.PrintSeparator(ev.Graphics, ev, yPos, "dotted");//-20




                // ==================================================================== //
                //                          TOTALES
                // ==================================================================== //                                 
                yPos = TablePrinter58.PrintSubtotalTable(ev.Graphics, ev, totalesLines, yPos, true);
                yPos += 10;


                // separador
                yPos = TablePrinter58.PrintSeparator(ev.Graphics, ev, yPos, "dotted");
                yPos += 10;

                // ==================================================================== //
                //                     FORMAS DE PAGO
                // ==================================================================== //

                yPos = TablePrinter58.PrintPayMethodTable(ev.Graphics, ev, payMethod, yPos);
                yPos += 10;

                // separador
                yPos = TablePrinter58.PrintSeparator(ev.Graphics, ev, yPos, "dotted");
                yPos += 10;


                // ==================================================================== //
                //              Imprime el código de barras / qrcode
                // ==================================================================== //

                //yPos = TablePrinter58.PrintBarcode(ev.Graphics, ev,yPos, barcode);
                //yPos = TablePrinter58.PrintQrCode(ev.Graphics, ev, yPos, printdetail.website);
                // yPos += 10;





                // ==================================================================== //
                //                          leyenda final
                // ==================================================================== //

                //RectangleF leyendRect = new RectangleF(ev.MarginBounds.Left, yPos + 20, ev.MarginBounds.Width, ev.MarginBounds.Height);
                //ev.Graphics.DrawString(printdetail.description, new Font("Segoe UI", 7, FontStyle.Bold), drawBrush, leyendRect, companySF);





            };

            // imprimir documento
             pd.Print();

            //previsualizar documento
            /*PrintPreviewDialog ppd = new PrintPreviewDialog();
            ppd.Document = pd;
            ppd.WindowState = FormWindowState.Maximized; // vista previa maximizada
            ppd.PrintPreviewControl.Zoom = 2.0; // establecer el zoom al 200%
            ppd.ShowDialog();*/


            // =========================
            // CORTE PRECISO DE PAPEL + APERTURA DE CAJA
            // =========================
            StringBuilder command = new StringBuilder();

            // Si la impresora necesita un feed mínimo antes del corte, podemos usar 1-3 líneas máximo
            if (ConfigManager.objConfig.BreakLines == true)
            {
                // imprimir espacios al final del ticket para agregar márgen después del branding / leyenda
                // se imprimen 5 espacios, esto debes ajustarlo en función de tu impresora, porque algunas impresoras asiáticas sacan mucho papel blanco al final
                // y algunas ocuparás imprimir más de 5 espacios

                // feed mínimo 1 línea para dar margen debajo del contenido, no más
                command.Append("\x1B" + "d" + "\x03"); // feed mínimo 3 línea
            }

            // Corte total del ticket (corta a la cuchilla)
            if (ConfigManager.objConfig.Cut == true)
            {
                // Corte total del ticket (corta a la cuchilla)
                command.Append("\x1D" + "V" + "\x00"); // corte completo (sin dejar papel extra)
            }

            // Abrir cajón de efectivo
            if (ConfigManager.objConfig.Drawer == true)
            {
                // Abrir cajón de efectivo
                command.Append("\x1B" + "p" + "\x00" + "\x0F" + "\x96"); // Pin 0, pulso 15ms, intervalo 150ms
            }

            // Enviar comandos a la impresora
            RawPrinterHelper.SendStringToPrinter(ConfigManager.objConfig.Printer, command.ToString());
            command.Clear();


        }

        public void PrintCashDrawerCut58(
            List<Tuple<string, bool, float, string>> companyLines,
            List<Tuple<string, bool, float, string>> enCajaLines,
            List<Tuple<string, bool, float, string>> ventasLines,
            List<Tuple<string, bool, float, string>> otrosDatosLines,
            List<Tuple<string, bool, float, string>> entradaLines,
            List<Tuple<string, bool, float, string>> salidaLines,
            List<Tuple<string, bool, float, string>> consultadoLines,
            List<Tuple<string, string, float>> headerLines, dynamic detalleGeneral, dynamic ventas, dynamic otrosDatos, dynamic entradaEfectivo, dynamic salidaEfectivo,
            List<Tuple<string, string>> totalEnCajaLines,
            List<Tuple<string, string>> totalVentasLines,
            List<Tuple<string, string>> totalEntradaLines,
            List<Tuple<string, string>> totalSalidaLines,
            List<Tuple<string, string>> payMethod, bool center = false, float margin = 0)
        {
            PrintDocument pd = new PrintDocument();
            pd.PrinterSettings.PrinterName = ConfigManager.objConfig.Printer;

            // establecer márgenes del documento a cero
            pd.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);

            pd.PrintPage += (sender, ev) =>
            {
                // ==================================================================== //
                //                  imprimir logo
                float yPos = margin;
                float logoWidth = 160;// ev.PageBounds.Width * 0.8f;  // ajusta según tus necesidades en porcentaje
                float logoHeight = 80;// (logoWidth / logo.Width) * logo.Height; 

                if (File.Exists(Application.StartupPath + "\\config\\logo.png"))
                {
                    Image logo = Image.FromFile(Application.StartupPath + "\\config\\logo.png");

                    // dibujar logo al inicio del documento
                    ev.Graphics.DrawImage(logo, (ev.PageBounds.Width - logoWidth) / 2, 0, logoWidth, logoHeight);
                    yPos += logoHeight;// + 10;
                }

                // posición Y del primer texto después del logo



                // ==================================================================== //


                // alineacion datos empresa
                SolidBrush drawBrush = new SolidBrush(Color.Black);
                StringFormat sf = new StringFormat();
                StringFormat companySF = new StringFormat();
                if (center)
                {
                    sf.Alignment = StringAlignment.Center;
                }
                else
                {
                    sf.Alignment = StringAlignment.Near;
                }


                // ==================================================================== //
                //          INFO DE LA EMPRESA / NEGOCIO
                // ==================================================================== //
                foreach (var line in companyLines)
                {
                    string text = line.Item1;
                    bool bold = line.Item2;
                    companySF.Alignment = (line.Item4 == "Center" ? StringAlignment.Center : StringAlignment.Near);

                    Font drawFont = new Font("Segoe UI", line.Item3, bold ? FontStyle.Bold : FontStyle.Regular);

                    // ajustar el rectángulo para que el texto se centre en el medio
                    RectangleF rect = new RectangleF(ev.MarginBounds.Left, yPos, ev.MarginBounds.Width, ev.MarginBounds.Height);

                    ev.Graphics.DrawString(text, drawFont, drawBrush, rect, companySF);

                    // ajustar la posición y para la siguiente línea
                    yPos += drawFont.GetHeight(ev.Graphics); // +5
                    //currentLine++;
                }

                // ==================================================================== //
                //              FOLIO, FECHA, USUARIO
                // ==================================================================== //
                foreach (var sbLine in headerLines)
                {
                    string text = sbLine.Item1;
                    string alignment = sbLine.Item2;

                    Font drawFont = new Font("Segoe UI", sbLine.Item3, FontStyle.Regular);

                    // ajustar la alineación del texto según el parámetro alignment
                    if (alignment == "Center")
                    {
                        sf.Alignment = StringAlignment.Center;
                    }
                    else if (alignment == "Right")
                    {
                        sf.Alignment = StringAlignment.Far;
                    }
                    else // alineación defecto
                    {
                        sf.Alignment = StringAlignment.Near;
                    }

                    // ajustar el rectángulo para que el texto se centre 
                    RectangleF rect = new RectangleF(ev.MarginBounds.Left, yPos + 10, ev.MarginBounds.Width, ev.MarginBounds.Height);

                    ev.Graphics.DrawString(text, drawFont, drawBrush, rect, sf);

                    // ajustar la posición y para la siguiente línea
                    yPos += drawFont.GetHeight(ev.Graphics);// +5;
                    //currentLine++;
                }


                // separador
                yPos += 15;
                yPos = TablePrinter58.PrintSeparator(ev.Graphics, ev, yPos, "dotted");
                yPos += 10;


                // ==================================================================== //
                //          ENCABEZADO EFECTIVO EN CAJA
                // ==================================================================== //
                foreach (var line in enCajaLines)
                {
                    string text = line.Item1;
                    bool bold = line.Item2;
                    companySF.Alignment = (line.Item4 == "Center" ? StringAlignment.Center : StringAlignment.Near);

                    Font drawFont = new Font("Segoe UI", line.Item3, bold ? FontStyle.Bold : FontStyle.Regular);

                    // ajustar el rectángulo para que el texto se centre en el medio
                    RectangleF rect = new RectangleF(ev.MarginBounds.Left, yPos, ev.MarginBounds.Width, ev.MarginBounds.Height);

                    ev.Graphics.DrawString(text, drawFont, drawBrush, rect, companySF);

                    // ajustar la posición y para la siguiente línea
                    yPos += drawFont.GetHeight(ev.Graphics); // +5
                    //currentLine++;
                }

                // ==================================================================== //
                //                      DETALLE / EN CAJA
                // ==================================================================== //
                int conta = 0;
                string qty = null;
                string description = null;

                Font fontDetail = new Font("Segoe UI", 8, FontStyle.Regular);
                Font font = new Font("Segoe UI", 8);
                Font fontHeaders = new Font("Segoe UI", 7, FontStyle.Bold);

                int spaceWidth = (int)ev.Graphics.MeasureString(" ", font).Width;

                StringFormat itemSf = new StringFormat();

                // valores base para calcular altura de filas y posicionar contenidos en la tabla concptos
                int rowHeight = 20;
                int leftMargin = 0;
                int rightMargin = 5;

                // bordes y estilo de la tabla cocneptos
                Graphics graphics = ev.Graphics;
                Pen pen = new Pen(Color.Black, 0.5f);
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

                // definir ancho y posición de las columnas                
                float[] columnWidths = { ev.PageBounds.Width * 0.10f, ev.PageBounds.Width * 0.55f, ev.PageBounds.Width * 0.25f };
                float[] columnPositions = { margin, margin + columnWidths[0], margin + columnWidths[0] + columnWidths[1] };


                yPos += 15; // +5Espacio antes de la cabecera

                // dibujar cabecera de la tabla conceptos
                string[] headers = { "No.", "Movimientos", "Efectivo" };
                for (int i = 0; i < headers.Length; i++)
                {
                    float headerX = columnPositions[i];
                    ev.Graphics.DrawString(headers[i], fontHeaders, drawBrush, headerX, yPos);
                }

                // dibujar bordes tabla conceptos
                // Dibujar bordes de la tabla conceptos justo después de los encabezados
                graphics.DrawRectangle(pen, leftMargin, yPos, columnWidths[0], rowHeight);
                graphics.DrawRectangle(pen, leftMargin + columnWidths[0], yPos, columnWidths[1], rowHeight);
                graphics.DrawRectangle(pen, leftMargin + columnWidths[0] + columnWidths[1], yPos, columnWidths[2], rowHeight);

                yPos += rowHeight;


                foreach (var item in detalleGeneral) //DETALLE movimiento
                {
                    conta++;
                    qty = item.efectivo;
                    description = item.movimiento;

                    string[] content = { conta.ToString(), description, qty };
                    float startYPosition = ev.MarginBounds.Top + rowHeight; // posición inicial en Y para este movimiento

                    // calcula el número de líneas de texto en este movimiento                       
                    int numLines = (int)Math.Ceiling(graphics.MeasureString(description, font, (int)columnWidths[0]).Height / rowHeight);

                    for (int j = 0; j < content.Length; j++)
                    {
                        float contentX = columnPositions[j];
                        RectangleF contentRect = new RectangleF(contentX, yPos, columnWidths[j], 20);
                        sf.Alignment = j > 0 ? StringAlignment.Far : StringAlignment.Near;
                        ev.Graphics.DrawString(content[j], fontDetail, drawBrush, contentRect, sf);
                    }

                    // ajustar la posición para la siguiente línea
                    yPos += (int)font.GetHeight();
                    //yPos += rowHeight;

                }

                // ==================================================================== //
                //                          TOTAL EN CAJA
                // ==================================================================== //                                 
                yPos = TablePrinter58.PrintSubtotalTable(ev.Graphics, ev, totalEnCajaLines, yPos, true);
                yPos += 10;

                // separador
                yPos = TablePrinter58.PrintSeparator(ev.Graphics, ev, yPos, "dotted");
                yPos += 10;

                // ==================================================================== //
                //          ENCABEZADO VENTAS
                // ==================================================================== //
                foreach (var line in ventasLines)
                {
                    string text = line.Item1;
                    bool bold = line.Item2;
                    companySF.Alignment = (line.Item4 == "Center" ? StringAlignment.Center : StringAlignment.Near);

                    Font drawFont = new Font("Segoe UI", line.Item3, bold ? FontStyle.Bold : FontStyle.Regular);

                    // ajustar el rectángulo para que el texto se centre en el medio
                    RectangleF rect = new RectangleF(ev.MarginBounds.Left, yPos, ev.MarginBounds.Width, ev.MarginBounds.Height);

                    ev.Graphics.DrawString(text, drawFont, drawBrush, rect, companySF);

                    // ajustar la posición y para la siguiente línea
                    yPos += drawFont.GetHeight(ev.Graphics); // +5
                    //currentLine++;
                }

                // ==================================================================== //
                //                      DETALLE / VENTAS
                // ==================================================================== //

                int conta2 = 0;
                string qty2 = null;
                string description2 = null;


                yPos += 15; // +5Espacio antes de la cabecera

                // dibujar cabecera de la tabla conceptos
                string[] headers2 = { "No.", "Movimientos", "Efectivo" };
                for (int i = 0; i < headers2.Length; i++)
                {
                    float headerX = columnPositions[i];
                    ev.Graphics.DrawString(headers2[i], fontHeaders, drawBrush, headerX, yPos);
                }

                // dibujar bordes tabla conceptos
                // Dibujar bordes de la tabla conceptos justo después de los encabezados
                graphics.DrawRectangle(pen, leftMargin, yPos, columnWidths[0], rowHeight);
                graphics.DrawRectangle(pen, leftMargin + columnWidths[0], yPos, columnWidths[1], rowHeight);
                graphics.DrawRectangle(pen, leftMargin + columnWidths[0] + columnWidths[1], yPos, columnWidths[2], rowHeight);

                yPos += rowHeight;


                foreach (var itemVent in ventas) //DETALLE VENTAS
                {
                    conta2++;
                    qty2 = itemVent.efectivo;
                    description2 = itemVent.movimiento;

                    string[] content = { conta2.ToString(), description2, qty2 };
                    float startYPosition = ev.MarginBounds.Top + rowHeight; // posición inicial en Y para este movimiento

                    // calcula el número de líneas de texto en este movimiento                       
                    int numLines = (int)Math.Ceiling(graphics.MeasureString(description2, font, (int)columnWidths[0]).Height / rowHeight);

                    for (int j = 0; j < content.Length; j++)
                    {
                        float contentX = columnPositions[j];
                        RectangleF contentRect = new RectangleF(contentX, yPos, columnWidths[j], 20);
                        sf.Alignment = j > 0 ? StringAlignment.Far : StringAlignment.Near;
                        ev.Graphics.DrawString(content[j], fontDetail, drawBrush, contentRect, sf);
                    }


                    // 
                    // ajustar la posición para la siguiente línea
                    yPos += (int)font.GetHeight();
                    //yPos += rowHeight;

                }

                // ==================================================================== //
                //                          TOTALES VENTAS
                // ==================================================================== //                                 
                yPos = TablePrinter58.PrintSubtotalTable(ev.Graphics, ev, totalVentasLines, yPos, true);
                yPos += 10;


                //separador
                yPos = TablePrinter58.PrintSeparator(ev.Graphics, ev, yPos, "dotted");
                yPos += 10;


                // ==================================================================== //
                //          ENCABEZADO OTROS DATOS
                // ==================================================================== //
                foreach (var line in otrosDatosLines)
                {
                    string text = line.Item1;
                    bool bold = line.Item2;
                    companySF.Alignment = (line.Item4 == "Center" ? StringAlignment.Center : StringAlignment.Near);

                    Font drawFont = new Font("Segoe UI", line.Item3, bold ? FontStyle.Bold : FontStyle.Regular);

                    // ajustar el rectángulo para que el texto se centre en el medio
                    RectangleF rect = new RectangleF(ev.MarginBounds.Left, yPos, ev.MarginBounds.Width, ev.MarginBounds.Height);

                    ev.Graphics.DrawString(text, drawFont, drawBrush, rect, companySF);

                    // ajustar la posición y para la siguiente línea
                    yPos += drawFont.GetHeight(ev.Graphics); // +5
                    //currentLine++;
                }

                // ==================================================================== //
                //                      DETALLE / OTROS DATOS
                // ==================================================================== //

                int conta3 = 0;
                string qty3 = null;
                string description3 = null;


                yPos += 15; // +5Espacio antes de la cabecera

                // dibujar cabecera de la tabla conceptos
                string[] headers3 = { "No.", "Movimientos", "Efectivo" };
                for (int i = 0; i < headers3.Length; i++)
                {
                    float headerX = columnPositions[i];
                    ev.Graphics.DrawString(headers3[i], fontHeaders, drawBrush, headerX, yPos);
                }

                // dibujar bordes tabla conceptos
                // Dibujar bordes de la tabla conceptos justo después de los encabezados
                graphics.DrawRectangle(pen, leftMargin, yPos, columnWidths[0], rowHeight);
                graphics.DrawRectangle(pen, leftMargin + columnWidths[0], yPos, columnWidths[1], rowHeight);
                graphics.DrawRectangle(pen, leftMargin + columnWidths[0] + columnWidths[1], yPos, columnWidths[2], rowHeight);

                yPos += rowHeight;


                foreach (var itemOtros in otrosDatos) //DETALLE OTRO DATOS
                {
                    conta3++;
                    qty3 = itemOtros.efectivo;
                    description3 = itemOtros.movimiento;

                    string[] content = { conta3.ToString(), description3, qty3 };
                    float startYPosition = ev.MarginBounds.Top + rowHeight; // posición inicial en Y para este movimiento

                    // calcula el número de líneas de texto en este movimiento                       
                    int numLines = (int)Math.Ceiling(graphics.MeasureString(description3, font, (int)columnWidths[0]).Height / rowHeight);

                    for (int j = 0; j < content.Length; j++)
                    {
                        float contentX = columnPositions[j];
                        RectangleF contentRect = new RectangleF(contentX, yPos, columnWidths[j], 20);
                        sf.Alignment = j > 0 ? StringAlignment.Far : StringAlignment.Near;
                        ev.Graphics.DrawString(content[j], fontDetail, drawBrush, contentRect, sf);
                    }


                    // 
                    // ajustar la posición para la siguiente línea
                    yPos += (int)font.GetHeight();
                    //yPos += rowHeight;

                }

                // separador
                yPos += 10;
                yPos = TablePrinter58.PrintSeparator(ev.Graphics, ev, yPos, "dotted");
                yPos += 10;

                // ==================================================================== //
                //          ENCABEZADO ENTRADA DE EFECTIVO
                // ==================================================================== //
                foreach (var line in entradaLines)
                {
                    string text = line.Item1;
                    bool bold = line.Item2;
                    companySF.Alignment = (line.Item4 == "Center" ? StringAlignment.Center : StringAlignment.Near);

                    Font drawFont = new Font("Segoe UI", line.Item3, bold ? FontStyle.Bold : FontStyle.Regular);

                    // ajustar el rectángulo para que el texto se centre en el medio
                    RectangleF rect = new RectangleF(ev.MarginBounds.Left, yPos, ev.MarginBounds.Width, ev.MarginBounds.Height);

                    ev.Graphics.DrawString(text, drawFont, drawBrush, rect, companySF);

                    // ajustar la posición y para la siguiente línea
                    yPos += drawFont.GetHeight(ev.Graphics); // +5
                    //currentLine++;
                }


                // ==================================================================== //
                //                      DETALLE / ENTRADA DE EFECTIVO
                // ==================================================================== //

                int conta4 = 0;
                string qty4 = null;
                string description4 = null;


                yPos += 15; // +5Espacio antes de la cabecera

                // dibujar cabecera de la tabla conceptos
                string[] headers4 = { "No.", "Concepto", "Efectivo" };
                for (int i = 0; i < headers4.Length; i++)
                {
                    float headerX = columnPositions[i];
                    ev.Graphics.DrawString(headers4[i], fontHeaders, drawBrush, headerX, yPos);
                }

                // dibujar bordes tabla conceptos
                // Dibujar bordes de la tabla conceptos justo después de los encabezados
                graphics.DrawRectangle(pen, leftMargin, yPos, columnWidths[0], rowHeight);
                graphics.DrawRectangle(pen, leftMargin + columnWidths[0], yPos, columnWidths[1], rowHeight);
                graphics.DrawRectangle(pen, leftMargin + columnWidths[0] + columnWidths[1], yPos, columnWidths[2], rowHeight);

                yPos += rowHeight;


                foreach (var itemEntrada in entradaEfectivo) //DETALLE ENTRADA DE EFECTIVO
                {
                    conta4++;
                    qty4 = itemEntrada.cash;
                    description4 = itemEntrada.concept;

                    string[] content = { conta4.ToString(), description4, qty4 };
                    float startYPosition = ev.MarginBounds.Top + rowHeight; // posición inicial en Y para este movimiento

                    // calcula el número de líneas de texto en este movimiento                       
                    int numLines = (int)Math.Ceiling(graphics.MeasureString(description4, font, (int)columnWidths[0]).Height / rowHeight);

                    for (int j = 0; j < content.Length; j++)
                    {
                        float contentX = columnPositions[j];
                        RectangleF contentRect = new RectangleF(contentX, yPos, columnWidths[j], 20);
                        sf.Alignment = j > 0 ? StringAlignment.Far : StringAlignment.Near;
                        ev.Graphics.DrawString(content[j], fontDetail, drawBrush, contentRect, sf);
                    }


                    // 
                    // ajustar la posición para la siguiente línea
                    yPos += (int)font.GetHeight();
                    //yPos += rowHeight;

                }

                // ==================================================================== //
                //                          TOTALES ENTRADA DE EFECTIVO
                // ==================================================================== //                                 
                yPos = TablePrinter58.PrintSubtotalTable(ev.Graphics, ev, totalEntradaLines, yPos, true);
                yPos += 10;


                //separador
                yPos = TablePrinter58.PrintSeparator(ev.Graphics, ev, yPos, "dotted");
                yPos += 10;


                // ==================================================================== //
                //          ENCABEZADO SALIDA DE EFECTIVO
                // ==================================================================== //
                foreach (var line in salidaLines)
                {
                    string text = line.Item1;
                    bool bold = line.Item2;
                    companySF.Alignment = (line.Item4 == "Center" ? StringAlignment.Center : StringAlignment.Near);

                    Font drawFont = new Font("Segoe UI", line.Item3, bold ? FontStyle.Bold : FontStyle.Regular);

                    // ajustar el rectángulo para que el texto se centre en el medio
                    RectangleF rect = new RectangleF(ev.MarginBounds.Left, yPos, ev.MarginBounds.Width, ev.MarginBounds.Height);

                    ev.Graphics.DrawString(text, drawFont, drawBrush, rect, companySF);

                    // ajustar la posición y para la siguiente línea
                    yPos += drawFont.GetHeight(ev.Graphics); // +5
                    //currentLine++;
                }


                // ==================================================================== //
                //                      DETALLE / SALIDA DE EFECTIVO
                // ==================================================================== //

                int conta5 = 0;
                string qty5 = null;
                string description5 = null;


                yPos += 15; // +5Espacio antes de la cabecera

                // dibujar cabecera de la tabla conceptos
                string[] headers5 = { "No.", "Concepto", "Efectivo" };
                for (int i = 0; i < headers5.Length; i++)
                {
                    float headerX = columnPositions[i];
                    ev.Graphics.DrawString(headers5[i], fontHeaders, drawBrush, headerX, yPos);
                }

                // dibujar bordes tabla conceptos
                // Dibujar bordes de la tabla conceptos justo después de los encabezados
                graphics.DrawRectangle(pen, leftMargin, yPos, columnWidths[0], rowHeight);
                graphics.DrawRectangle(pen, leftMargin + columnWidths[0], yPos, columnWidths[1], rowHeight);
                graphics.DrawRectangle(pen, leftMargin + columnWidths[0] + columnWidths[1], yPos, columnWidths[2], rowHeight);

                yPos += rowHeight;


                foreach (var itemSalida in salidaEfectivo) //DETALLE ENTRADA DE EFECTIVO
                {
                    conta5++;
                    qty5 = itemSalida.cash;
                    description5 = itemSalida.concept;

                    string[] content = { conta5.ToString(), description5, qty5 };
                    float startYPosition = ev.MarginBounds.Top + rowHeight; // posición inicial en Y para este movimiento

                    // calcula el número de líneas de texto en este movimiento                       
                    int numLines = (int)Math.Ceiling(graphics.MeasureString(description5, font, (int)columnWidths[0]).Height / rowHeight);

                    for (int j = 0; j < content.Length; j++)
                    {
                        float contentX = columnPositions[j];
                        RectangleF contentRect = new RectangleF(contentX, yPos, columnWidths[j], 20);
                        sf.Alignment = j > 0 ? StringAlignment.Far : StringAlignment.Near;
                        ev.Graphics.DrawString(content[j], fontDetail, drawBrush, contentRect, sf);
                    }


                    // 
                    // ajustar la posición para la siguiente línea
                    yPos += (int)font.GetHeight();
                    //yPos += rowHeight;

                }

                // ==================================================================== //
                //                          TOTALES SALIDA DE EFECTIVO
                // ==================================================================== //                                 
                yPos = TablePrinter58.PrintSubtotalTable(ev.Graphics, ev, totalSalidaLines, yPos, true);
                yPos += 10;


                //separador
                yPos = TablePrinter58.PrintSeparator(ev.Graphics, ev, yPos, "dotted");
                yPos += 10;


                // ==================================================================== //
                //          ENCABEZADO CONSULTADO
                // ==================================================================== //
                foreach (var line in consultadoLines)
                {
                    string text = line.Item1;
                    bool bold = line.Item2;
                    companySF.Alignment = (line.Item4 == "Center" ? StringAlignment.Center : StringAlignment.Near);

                    Font drawFont = new Font("Segoe UI", line.Item3, bold ? FontStyle.Bold : FontStyle.Regular);

                    // ajustar el rectángulo para que el texto se centre en el medio
                    RectangleF rect = new RectangleF(ev.MarginBounds.Left, yPos, ev.MarginBounds.Width, ev.MarginBounds.Height);

                    ev.Graphics.DrawString(text, drawFont, drawBrush, rect, companySF);

                    // ajustar la posición y para la siguiente línea
                    yPos += drawFont.GetHeight(ev.Graphics); // +5
                    //currentLine++;
                }

                //separador
                /*  yPos += 10;
                  yPos = TablePrinter58.PrintSeparator(ev.Graphics, ev, yPos, "dotted");
                  yPos += 10;*/



            };

            // imprimir documento
            pd.Print();

            //previsualizar documento
            /*PrintPreviewDialog ppd = new PrintPreviewDialog();
             ppd.Document = pd;
             ppd.WindowState = FormWindowState.Maximized; // vista previa maximizada
             ppd.PrintPreviewControl.Zoom = 2.0; // establecer el zoom al 200%
             ppd.ShowDialog();*/




            //cortar papel
            StringBuilder command = new StringBuilder();
            /*command.AppendLine("");
            command.AppendLine("");
            command.AppendLine("");
            command.AppendLine("");
            command.AppendLine("");*/
            ///command.AppendLine("\x1B" + "m");
            command.AppendLine("\x1B" + "d" + "\x0");
            command.AppendLine("\x1B" + "p" + "\x00" + "\x0F" + "\x96"); // caracteres de apertura cajon 0

            RawPrinterHelper.SendStringToPrinter1(ConfigManager.objConfig.Printer, command.ToString());
            command.Clear();

        }



        // método para dividir una cadena en líneas de longitud predefinida
        public static string[] SplitText(string text, int maxLength)
        {

            int length = text.Length;
            int numLines = (int)Math.Ceiling((double)length / maxLength);
            string[] lines = new string[numLines];

            for (int i = 0; i < numLines; i++)
            {
                int startIndex = i * maxLength;
                int charsToTake = Math.Min(maxLength, length - startIndex);
                lines[i] = text.Substring(startIndex, charsToTake);
            }

            return lines;
        }

    }



}


