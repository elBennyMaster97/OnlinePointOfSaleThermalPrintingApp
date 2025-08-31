
using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace OnlineReceiptPrintingApp
{

    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Printing;
    using System.Linq;
    using ZXing.Common;
    using ZXing;
    using Newtonsoft.Json.Linq;
    using System.Text;
    using System.IO;
    using System.Diagnostics;


    public class TablePrinter80LLPOS
    {
        /* Para calcular con mayor precisión los espacios y alineación de texto, es recomendable utilizar fuentes monoespaciadas,
         * ya que  este tipo de fuente maneja un espacio constante entre caracteres. Para nuestro caso estamos utilizando la fuente: "Segoe UI".
        */

        // fuente general
        static Font Font58 = new Font("Segoe UI", 8);
        static Font Font58Bold = new Font("Segoe UI", 8, FontStyle.Bold);
        static Font Font58Separator = new Font("Segoe UI", 10);



        public static float PrintLogo(Graphics graphics, PrintPageEventArgs ev, PrintDocument pd, float yPos)
        {
            float logoWidth = 160;
            float logoHeight = 80;

            if (File.Exists(Application.StartupPath + "\\config\\logo.png"))
            {
                Image logo = Image.FromFile(Application.StartupPath + "\\config\\logo.png");

                ev.Graphics.DrawImage(logo, (ev.PageBounds.Width - logoWidth) / 2, 0, logoWidth, logoHeight);
                yPos += logoHeight;
            }
            return yPos;
        }


        // Método para imprimir el recuadro sii, si eres de Chile te funciona perfecto, en nuestro ejercicio no lo utilizaremos.
        public static float PrintRecuadro(Graphics graphics, PrintPageEventArgs ev, PrintDocument pd, dynamic venta, float yPos, dynamic empresa, string Folio)
        {
            StringFormat stringFormat = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            int sizeFont = 11;
            var font2 = new Font("Segoe UI", sizeFont, System.Drawing.FontStyle.Regular);
            using (Font font1 = new Font("Segoe UI", sizeFont, FontStyle.Bold, GraphicsUnit.Point))
            {

                int leftMargin = 10; // Ajustar margen izquierdo
                int rightMargin = 15;
                int paperWidth = ev.PageBounds.Width - leftMargin - rightMargin;

                // Ancho del recuadro
                int rectWidth = (int)(paperWidth * 0.9); // 90% del ancho del papel

                // Calcular la posición x para centrar el rectángulo
                int xPos = (paperWidth - rectWidth) / 2;

                Rectangle rect2 = new Rectangle(xPos, (int)yPos, rectWidth, 60);

                //podemos hacer dinámico el recuadro cargando los valores de un txt o agregarlo al frmConfig
                //if (App.existsSettings)
                //{
                //    rect2 = App.ValoresRectangulo();
                //}

                string tipoDocName = "FACTURA ELECTRÓNICA";


                string textToPrint = $"R.U.T: {empresa["rut"]}\n{tipoDocName}\nN° {Folio}";

                ev.Graphics.DrawRectangle(Pens.Black, rect2);
                ev.Graphics.DrawString(textToPrint, font2, Brushes.Black, rect2, stringFormat);
                yPos += rect2.Height;

                RectangleF rect3 = new RectangleF(xPos, yPos, rectWidth, 20);
                ev.Graphics.DrawString($"S.I.I. - {empresa["comuna"]}", font2, Brushes.Black, rect3, stringFormat);
                yPos += rect3.Height + 5;
            }

            return yPos;
        }

        // Método para imprimir la tabla de subtotales
        public static float PrintSubtotalTable(Graphics graphics, PrintPageEventArgs e, dynamic rows, float yPos, bool center = false, bool borders = false)
        {
            // Definir los márgenes y la altura de fila
            int leftMargin = 10;
            int rightMargin = 10;
            int rowHeight = 16;

            // Definir ancho imprimible de la página
            float printableWidth = e.PageBounds.Width - leftMargin - rightMargin;

            // Define columnas dinámicas: 40%, 40%, 20%
            float[] columnWidths = { printableWidth * 0.6f, printableWidth * 0.3f, printableWidth * 0.1f };

            // Define posiciones de las columnas
            float[] columnPositions = { leftMargin, leftMargin + columnWidths[0], leftMargin + columnWidths[0] + columnWidths[1] };

            // Alineación de texto para la primera columna (derecha)
            StringFormat sfRight = new StringFormat();
            sfRight.Alignment = StringAlignment.Far;

            // Alineación de texto para la segunda columna (izquierda)
            StringFormat sfLeft = new StringFormat();
            sfLeft.Alignment = StringAlignment.Far;

            // Alineación de texto para la tercera columna (izquierda)
            StringFormat sfCenter = new StringFormat();
            sfCenter.Alignment = StringAlignment.Near;

            // Definir el pincel para dibujar los bordes (definido en el argumento que recibe este método)
            Pen pen = new Pen(Color.Black, 0.5f);
            pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;

            // Dibujar las filas de la tabla
            foreach (JProperty property in rows)
            {
                // Dibujar texto en la primera columna
                graphics.DrawString(property.Name.ToString().Trim().ToUpper() + ":", Font58Bold, Brushes.Black, new RectangleF(columnPositions[0], yPos, columnWidths[0], rowHeight), sfRight);

                // Dibujar texto en la segunda columna
                graphics.DrawString($"{App.moneyFormat(property.Value)}", Font58, Brushes.Black, new RectangleF(columnPositions[1], yPos, columnWidths[1], rowHeight), sfLeft);

                string thirdColumnValue = ""; // Reemplaza esto con el valor real de la tercera columna (en mi caso lo utilizo como espacio a la derecha)
                graphics.DrawString(thirdColumnValue, Font58, Brushes.Black, new RectangleF(columnPositions[2], yPos, columnWidths[2], rowHeight), sfCenter);

                // Dibujar los bordes
                if (borders)
                {
                    graphics.DrawRectangle(pen, leftMargin, yPos, columnWidths[0], rowHeight);
                    graphics.DrawRectangle(pen, leftMargin + columnWidths[0], yPos, columnWidths[1], rowHeight);
                    graphics.DrawRectangle(pen, leftMargin + columnWidths[0] + columnWidths[1], yPos, columnWidths[2], rowHeight);
                }

                // Incrementar la posición Y para la siguiente fila
                yPos += rowHeight;
            }
            return yPos; // Retorna la posición de Y para que los siguientes métodos sigan imprimiendo hacia abajo
        }


        // Método genérico para imprimir texto con alineación
        public static float PrintText(Graphics graphics, string text, Font font, Brush brush, StringFormat format, RectangleF marginBounds, float yPos)
        {
            // crear rectángulo para dibujar el texto
            RectangleF rect = new RectangleF(marginBounds.Left, yPos, marginBounds.Width, marginBounds.Height - yPos);

            // dimensioes del texto a imprimir
            SizeF textSize = graphics.MeasureString(text, font, rect.Size, format);

            // imprimir texto
            graphics.DrawString(text, font, brush, rect, format);

            // actualizar yPos hacia la siguiente línea
            yPos += textSize.Height;

            // Retorna la posición de Y actualizada
            return yPos;
        }


        // Método para imprimir las formas de pago
        public static float PrintPayMethodTable(Graphics graphics, PrintPageEventArgs e, List<Tuple<string, string>> rows, float yPos, bool borders = false)
        {
            // Definir márgenes y altura de fila
            int leftMargin = 5;
            int rightMargin = 5;
            int rowHeight = 16;

            // Calcular ancho de las columnas
            float fullWidth = e.PageBounds.Width - leftMargin - rightMargin;
            float[] columnWidths = { fullWidth * 0.6f, fullWidth * 0.3f, fullWidth * 0.1f }; // Anchos de las columnas

            // Formatos de alineación de texto
            StringFormat centerFormat = new StringFormat() { Alignment = StringAlignment.Center };
            StringFormat rightFormat = new StringFormat() { Alignment = StringAlignment.Far };

            // Dibujar el encabezado para formas de pago      
            graphics.DrawString("FORMA DE PAGO:", Font58Bold, Brushes.Black, new RectangleF(leftMargin, yPos, fullWidth, rowHeight), centerFormat);
            yPos += rowHeight;

            // Dibujar las filas de la tabla
            foreach (var row in rows)
            {
                // Dibujar texto en la primera columna de la fila
                graphics.DrawString(row.Item1, Font58, Brushes.Black, new RectangleF(leftMargin, yPos, columnWidths[0], rowHeight), rightFormat);

                // Dibujar texto en la segunda columna de la fila
                graphics.DrawString(row.Item2, Font58, Brushes.Black, new RectangleF(leftMargin + columnWidths[0], yPos, columnWidths[1], rowHeight), rightFormat);

                // Dibujar texto en la tercera columna de la fila
                graphics.DrawString("", Font58, Brushes.Black, new RectangleF(leftMargin + columnWidths[0] + columnWidths[1], yPos, columnWidths[2], rowHeight), rightFormat);

                // Dibujar los bordes (opcional)
                if (borders)
                {
                    graphics.DrawRectangle(Pens.Black, leftMargin, yPos, columnWidths[0], rowHeight);
                    graphics.DrawRectangle(Pens.Black, leftMargin + columnWidths[0], yPos, columnWidths[1], rowHeight);
                    graphics.DrawRectangle(Pens.Black, leftMargin + columnWidths[0] + columnWidths[1], yPos, columnWidths[2], rowHeight);
                }

                // Incrementar la posición Y para la siguiente fila
                yPos += rowHeight;
            }

            return yPos + 5;// 10; // Retorna la posición de Y para que los siguientes métodos sigan imprimiendo hacia abajo
        }


        // Método para imprimir las notas / observaciones de la venta    
        public static float PrintNotes(Graphics graphics, PrintPageEventArgs e, string detalle, float yPos, bool borders = false)
        {
            // definir márgenes y altura de fila
            int leftMargin = 5;
            int rightMargin = 5;
            int rowHeight = 16;


            // calcular ancho de las columnas
            float fullWidth = e.PageBounds.Width - leftMargin - rightMargin;
            float columnWidth = fullWidth; // 2;

            // formatos alineación de texto            
            StringFormat rightFormat = new StringFormat() { Alignment = StringAlignment.Near };

            // incrementar la posición Y para la siguiente fila                
            yPos += rowHeight;


            // dibujar las filas de la tabla            
            graphics.DrawString(detalle, Font58, Brushes.Black, new RectangleF(leftMargin, yPos, columnWidth, rowHeight), rightFormat);


            // dibujar los bordes (opcional)
            if (borders)
            {
                graphics.DrawRectangle(Pens.Black, leftMargin, yPos, columnWidth, rowHeight);
                graphics.DrawRectangle(Pens.Black, leftMargin + columnWidth, yPos, columnWidth, rowHeight);
            }

            // incrementar la posición Y para la siguiente fila
            yPos += rowHeight;


            return yPos + 10; // retorna la posicion de Y para que los siguientes métodos sigan imprimiendo hacia abajo
        }


        // Método para imprimir pie de página / leyenda / branding, etc...
        public static float PrintTimbreFooter(Graphics graphics, PrintPageEventArgs e, List<Tuple<string>> rows, float yPos, bool borders = false)
        {
            // definir márgenes y altura de fila
            int leftMargin = 5;
            int rightMargin = 15;
            int rowHeight = 13;

            Font FontFooter = new Font("Segoe UI", 8, FontStyle.Regular);


            // calcular ancho de las columnas
            float fullWidth = e.PageBounds.Width - leftMargin - rightMargin;
            float columnWidth = fullWidth; // 2;

            // formatos alineación de texto
            StringFormat rightFormat = new StringFormat() { Alignment = StringAlignment.Center };



            // dibujar las filas de la tabla
            foreach (var row in rows)
            {
                graphics.DrawString(row.Item1, FontFooter, Brushes.Black, new RectangleF(leftMargin, yPos, columnWidth, rowHeight), rightFormat);


                // dibujar los bordes (opcional)
                if (borders)
                {
                    graphics.DrawRectangle(Pens.Black, leftMargin, yPos, columnWidth, rowHeight);
                    graphics.DrawRectangle(Pens.Black, leftMargin + columnWidth, yPos, columnWidth, rowHeight);
                }

                // incrementar la posición Y para la siguiente fila
                yPos += rowHeight;
            }

            return yPos + 10; // retorna la posicion de Y para que los siguientes métodos sigan imprimiendo hacia abajo
        }


        // Método genérico para imprimir filas con alineación
        public static float PrintRows(Graphics graphics, PrintPageEventArgs e, List<Tuple<string>> rows, float yPos, int rowHeight = 12, string align = "center", bool bold = true, bool borders = false)
        {
            // definir márgenes y altura de fila
            int leftMargin = 5;
            int rightMargin = 5;
            // int rowHeight = 12;

            Font FontFooter = new Font("Segoe UI", 8, FontStyle.Regular);
            Font FontFooterBold = new Font("Segoe UI", 8, FontStyle.Bold);

            // calcular ancho de las columnas
            float fullWidth = e.PageBounds.Width - leftMargin - rightMargin;
            float columnWidth = fullWidth; // 2;

            // formatos alineación de texto
            StringFormat rightFormat = new StringFormat();
            rightFormat.Alignment = StringAlignment.Center;
            if (align != "center")
            {
                rightFormat.Alignment = (align == "left" ? StringAlignment.Near : StringAlignment.Far);
            }


            // dibujar las filas de la tabla
            foreach (var row in rows)
            {
                if (row.Item1 == "Delivery:")
                {
                    graphics.DrawString(row.Item1, FontFooterBold, Brushes.Black, new RectangleF(leftMargin, yPos, columnWidth, rowHeight), rightFormat);
                }
                else
                {
                    graphics.DrawString(row.Item1, FontFooter, Brushes.Black, new RectangleF(leftMargin, yPos, columnWidth, rowHeight), rightFormat);
                }


                // dibujar los bordes (opcional)
                if (borders)
                {
                    graphics.DrawRectangle(Pens.Black, leftMargin, yPos, columnWidth, rowHeight);
                    graphics.DrawRectangle(Pens.Black, leftMargin + columnWidth, yPos, columnWidth, rowHeight);
                }

                // incrementar la posición Y para la siguiente fila
                yPos += rowHeight;
            }

            return yPos + 10; // retorna la posicion de Y para que los siguientes métodos sigan imprimiendo hacia abajo
        }


        #region "SEPARADOR, BARCODE y QRCODE"

        // método para imprimir separador de contenido / etxto
        public static float PrintSeparator(Graphics graphics, PrintPageEventArgs e, float yPos, string lineStyle = "solid", char lineChar = '=', int thickness = 1)
        {
            // márgenes para calcular el ancho total disponible e imprimir el separador/línea
            int leftMargin = 5;
            int rightMargin = 5;
            float width = e.PageBounds.Width - leftMargin - rightMargin;

            // determinar el estilo de la línea
            switch (lineStyle)
            {
                case "solid":
                    using (Pen pen = new Pen(Color.Black, thickness))
                    {
                        graphics.DrawLine(pen, leftMargin, yPos, leftMargin + width, yPos);
                    }
                    break;
                case "dotted":
                    using (Pen pen = new Pen(Color.Black, thickness) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash })
                    {
                        graphics.DrawLine(pen, leftMargin, yPos, leftMargin + width, yPos);
                    }
                    break;
                case "double":
                    using (Pen pen = new Pen(Color.Black, thickness))
                    {
                        graphics.DrawLine(pen, leftMargin, yPos, leftMargin + width, yPos);
                        graphics.DrawLine(pen, leftMargin, yPos + 2 * thickness, leftMargin + width, yPos + 2 * thickness);
                    }
                    break;
                case "character":
                    string line = new string(lineChar, (int)(width / graphics.MeasureString(lineChar.ToString(), Font58Separator).Width));
                    graphics.DrawString(line, Font58, Brushes.Black, new PointF(leftMargin, yPos));
                    break;
            }

            // retorna la posicion de Y para que los siguientes métodos sigan imprimiendo hacia abajo
            return yPos + thickness * 2;
        }





        // método para imprimir código de barras
        public static float PrintBarcode(Graphics graphics, PrintPageEventArgs ev, float yPos, string barcodeText)
        {

            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.CODE_128,
                Options = new EncodingOptions { Height = 30, Width = 250, PureBarcode = true }
            };
            var barcodeBitmap = writer.Write(barcodeText);

            // ajustar el rectángulo para que el código de barras se centre
            RectangleF barcodeRect = new RectangleF((ev.MarginBounds.Width - barcodeBitmap.Width) / 2, yPos, barcodeBitmap.Width, barcodeBitmap.Height);

            ev.Graphics.DrawImage(barcodeBitmap, barcodeRect);

            // formato de fuente para el código de barras
            Font barcodeFont = new Font("Segoe UI", 8, FontStyle.Regular);
            SolidBrush drawBrushBC = new SolidBrush(Color.Black);
            StringFormat sfBC = new StringFormat();
            sfBC.Alignment = StringAlignment.Center; // centrar el texto

            // ajustar la posición y para la siguiente línea
            yPos += barcodeBitmap.Height - 3;

            // ajustar el rectángulo para que el texto se centre
            RectangleF textRect = new RectangleF(ev.MarginBounds.Left, yPos, ev.MarginBounds.Width, barcodeFont.GetHeight(ev.Graphics));

            ev.Graphics.DrawString(barcodeText, barcodeFont, drawBrushBC, textRect, sfBC);


            return yPos; // retorna la posicion de Y para que los siguientes métodos sigan imprimiendo hacia abajo
        }






        // método para imprimir qr code (agrega try/catch por cualquier error)
        public static float PrintQrCode(Graphics graphics, PrintPageEventArgs ev, float yPos, string barcodeText, int height = 100, int width = 100)
        {
            try
            {

                // establecer configuraciones y dimensiones de la imagen
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

                Bitmap qrCode = writer.Write(barcodeText);


                RectangleF barcodeRect = new RectangleF((ev.MarginBounds.Width - qrCode.Width) / 2, yPos, qrCode.Width, qrCode.Height);

                ev.Graphics.DrawImage(qrCode, barcodeRect);

                // ajustar la posición y para la siguiente línea
                yPos += qrCode.Height;

                return yPos; // retorna la posicion de Y para que los siguientes métodos sigan imprimiendo hacia abajo

            }catch {
                return yPos; // retorna la posicion de Y para que los siguientes métodos sigan imprimiendo hacia abajo
            }
        }

        #endregion


        // Método para imprimir el detalle/productos de la venta
        public static float PrintDetailTable(Graphics graphics, PrintPageEventArgs e, PrintDocument pd, float yPos, dynamic products)
        {


            Graphics g = graphics;
            Font font = Font58;
            Pen blackPen = new Pen(Color.Black, 1);
            float lineHeight = font.GetHeight(g);
            float y = yPos;// 0;
            float leftMargin = 0;
            float rightMargin = pd.DefaultPageSettings.PaperSize.Width - pd.DefaultPageSettings.Margins.Right;
            float width = rightMargin - leftMargin;
            float[] columnWidths = { width * 0.48f, width * 0.12f, width * 0.20f, width * 0.20f };

            // Imprimir las cabeceras
            string[] headers = { "DESCRIPCIÓN", "CANT", "PRECIO", "IMPORTE" };
            for (int i = 0; i < headers.Length; i++)
            {
                StringFormat format = new StringFormat();
                if (i == 0)
                    format.Alignment = StringAlignment.Near;
                else
                    format.Alignment = StringAlignment.Center;

                g.DrawString(headers[i], Font58Bold, Brushes.Black, new RectangleF(leftMargin + columnWidths.Take(i).Sum(), y, columnWidths[i], lineHeight), format);
                g.DrawRectangle(blackPen, leftMargin + columnWidths.Take(i).Sum(), y, columnWidths[i], lineHeight);
            }
            y += lineHeight;
            yPos += lineHeight;


            foreach (var fila in products)
            {
                string descripcion = fila.product_name;
                //subtotal = Convert.ToDecimal( (qty * price) - ( ((qty * price) * discount) / 100) );
                decimal subtotal = (Convert.ToDecimal(fila.amount));// * Convert.ToDecimal(fila.price)) - (((Convert.ToDecimal(fila.quantity) * Convert.ToDecimal(fila.price)) * Convert.ToDecimal(fila.discount)) / 100);

                // Alineación al centro para la columna "Cantidad"
                StringFormat formatQty = new StringFormat();
                formatQty.Alignment = StringAlignment.Center;
                g.DrawString(fila.quantity.ToString(), font, Brushes.Black, new RectangleF(leftMargin + columnWidths[0], y, columnWidths[1], lineHeight), formatQty);

                StringFormat formatPrice = new StringFormat();
                formatPrice.Alignment = StringAlignment.Center;
                g.DrawString(App.moneyFormat(fila.regular_price), font, Brushes.Black, new RectangleF(leftMargin + columnWidths[0] + columnWidths[1], y, columnWidths[2], lineHeight), formatPrice);

                g.DrawString(App.moneyFormat(subtotal), font, Brushes.Black, new RectangleF(leftMargin + columnWidths[0] + columnWidths[1] + columnWidths[2], y, columnWidths[3], lineHeight), formatPrice);

                // se imprime la descripción del producto y hace salto de linea cada 26 caracteres para seguir imprimiendo el producto hasta el final,
                // si no deseas imprimir todo el texto del producto, puedes truncar o imprimir solo los primeros 20 caracteres descripcion.Substring(0, 20)
                int start = 0;
                while (start < descripcion.Length)
                {
                    int length = Math.Min(26, descripcion.Length - start);
                    g.DrawString(descripcion.Substring(start, length), font, Brushes.Black, new RectangleF(leftMargin, y, columnWidths[0], lineHeight));
                    start += length;
                    y += lineHeight;
                    yPos += lineHeight;
                }
            }

            return yPos;
        }

        // Método para imprimir el detalle/productos de la venta DEL DIA
        public static float PrintSaleDetailTable(Graphics graphics, PrintPageEventArgs e, PrintDocument pd, float yPos, dynamic products)
        {


            Graphics g = graphics;
            Font font = Font58;
            Pen blackPen = new Pen(Color.Black, 1);
            float lineHeight = font.GetHeight(g);
            float y = yPos;// 0;
            float leftMargin = 0;
            float rightMargin = pd.DefaultPageSettings.PaperSize.Width - pd.DefaultPageSettings.Margins.Right;
            float width = rightMargin - leftMargin;
            float[] columnWidths = { width * 0.52f, width * 0.14f, width * 0.20f, width * 0.14f };

            // Imprimir las cabeceras
            string[] headers = { "PRODUCTOS", "CANT", "TOTAL", "STK" };
            for (int i = 0; i < headers.Length; i++)
            {
                StringFormat format = new StringFormat();
                if (i == 0)
                    format.Alignment = StringAlignment.Near;
                else
                    format.Alignment = StringAlignment.Center;

                g.DrawString(headers[i], Font58Bold, Brushes.Black, new RectangleF(leftMargin + columnWidths.Take(i).Sum(), y, columnWidths[i], lineHeight), format);
                g.DrawRectangle(blackPen, leftMargin + columnWidths.Take(i).Sum(), y, columnWidths[i], lineHeight);
            }
            y += lineHeight;
            yPos += lineHeight;


            foreach (var fila in products)
            {

                string descripcion = fila.product;

                // Alineación al centro para la columna "Cantidad"
                StringFormat formatQty = new StringFormat();
                formatQty.Alignment = StringAlignment.Center;
                g.DrawString(fila.quantity.ToString(), font, Brushes.Black, new RectangleF(leftMargin + columnWidths[0], y, columnWidths[1], lineHeight), formatQty);

                // Alineación al centro para la columna "Sale"
                StringFormat formatPrice = new StringFormat();
                formatPrice.Alignment = StringAlignment.Center;
                g.DrawString(App.moneyFormat(fila.sale), font, Brushes.Black, new RectangleF(leftMargin + columnWidths[0] + columnWidths[1], y, columnWidths[2], lineHeight), formatPrice);

                // Alineación al centro para la columna "Stock"
                StringFormat formatStk = new StringFormat();
                formatStk.Alignment = StringAlignment.Center;
                g.DrawString(fila.stock.ToString(), font, Brushes.Black, new RectangleF(leftMargin + columnWidths[0] + columnWidths[1] + columnWidths[2], y, columnWidths[3], lineHeight), formatStk);

                // se imprime la descripción del producto y hace salto de linea cada 31 caracteres para seguir imprimiendo el producto hasta el final,
                // si no deseas imprimir todo el texto del producto, puedes truncar o imprimir solo los primeros 31 caracteres descripcion.Substring(0, 20)
                int start = 0;
                while (start < descripcion.Length)
                {
                    int length = Math.Min(31, descripcion.Length - start);
                    g.DrawString(descripcion.Substring(start, length), font, Brushes.Black, new RectangleF(leftMargin, y, columnWidths[0], lineHeight));
                    start += length;
                    y += lineHeight;
                    yPos += lineHeight;
                }

            }

            return yPos;
        }


        // Método para imprimir el detalle/productos comprobante de comida
        public static float PrintDetailTableExtra(Graphics graphics, PrintPageEventArgs e, PrintDocument pd, float yPos, dynamic products)
        {


            Graphics g = graphics;
            Font font = Font58;
            Pen blackPen = new Pen(Color.Black, 1);
            float lineHeight = font.GetHeight(g);
            float y = yPos;// 0;
            float leftMargin = 0;
            float rightMargin = pd.DefaultPageSettings.PaperSize.Width - pd.DefaultPageSettings.Margins.Right;
            float width = rightMargin - leftMargin;
            //float[] columnWidths = { width * 0.45f, width * 0.14f, width * 0.20f, width * 0.21f };
            float[] columnWidths = { width * 0.48f, width * 0.12f, width * 0.20f, width * 0.20f };

            // Imprimir las cabeceras
            string[] headers = { "COMIDA", "CANT", "PRECIO", "IMPORT" };
            for (int i = 0; i < headers.Length; i++)
            {
                StringFormat format = new StringFormat();
                if (i == 0)
                    format.Alignment = StringAlignment.Near;
                else
                    format.Alignment = StringAlignment.Center;

                g.DrawString(headers[i], Font58Bold, Brushes.Black, new RectangleF(leftMargin + columnWidths.Take(i).Sum(), y, columnWidths[i], lineHeight), format);
                g.DrawRectangle(blackPen, leftMargin + columnWidths.Take(i).Sum(), y, columnWidths[i], lineHeight);
            }
            y += lineHeight;
            yPos += lineHeight;


            foreach (var fila in products)
            {
                if (fila.product.category_id == 2)// 2 equivale a la categoria comida, cuenta cuantas veces se repite
                {
                    string descripcion = fila.product.name;
                    //subtotal = Convert.ToDecimal( (qty * price) - ( ((qty * price) * discount) / 100) );
                    decimal subtotal = (Convert.ToDecimal(fila.quantity) * Convert.ToDecimal(fila.price)) - (((Convert.ToDecimal(fila.quantity) * Convert.ToDecimal(fila.price)) * Convert.ToDecimal(fila.discount)) / 100);

                    // Alineación al centro para la columna "Cantidad"
                    StringFormat formatQty = new StringFormat();
                    formatQty.Alignment = StringAlignment.Center;
                    g.DrawString(fila.quantity.ToString(), font, Brushes.Black, new RectangleF(leftMargin + columnWidths[0], y, columnWidths[1], lineHeight), formatQty);

                    StringFormat formatPrice = new StringFormat();
                    formatPrice.Alignment = StringAlignment.Center;
                    g.DrawString(App.moneyFormat(fila.price), font, Brushes.Black, new RectangleF(leftMargin + columnWidths[0] + columnWidths[1], y, columnWidths[2], lineHeight), formatPrice);

                    g.DrawString(App.moneyFormat(subtotal), font, Brushes.Black, new RectangleF(leftMargin + columnWidths[0] + columnWidths[1] + columnWidths[2], y, columnWidths[3], lineHeight), formatPrice);

                    // se imprime la descripción del producto y hace salto de linea cada 20 caracteres para seguir imprimiendo el producto hasta el final,
                    // si no deseas imprimir todo el texto del producto, puedes truncar o imprimir solo los primeros 20 caracteres descripcion.Substring(0, 20)
                    int start = 0;
                    while (start < descripcion.Length)
                    {
                        int length = Math.Min(26, descripcion.Length - start);
                        g.DrawString(descripcion.Substring(start, length), font, Brushes.Black, new RectangleF(leftMargin, y, columnWidths[0], lineHeight));
                        start += length;
                        y += lineHeight;
                        yPos += lineHeight;
                    }
                }
            }

            return yPos;
        }


        // Método para imprimir la tabla de firma sii, si eres de Chile te viene perfecto, en nuestro ejercicio no la utilizaremos
        public static float PrintSignTable(Graphics graphics, PrintPageEventArgs e, PrintDocument pd, float yPos)
        {
            Font font = new Font("Segoe UI", 7);
            Pen blackPen = new Pen(Color.Black, 1);
            blackPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            float lineHeight = font.GetHeight(graphics);
            float y = yPos;

            int leftMargin = 5;
            int rightMargin = 25;
            int tableWidth = e.PageBounds.Width - leftMargin - rightMargin;

            // textos recuadro firma
            string[] textos = {
                "Nombre:",
                "Rut:",
                "Fecha:",
                "Recinto:",
                "Firma:",
                "Acuse de recibo..."
            };

            for (int i = 0; i < textos.Length; i++)
            {
                StringFormat format = new StringFormat();
                format.Alignment = StringAlignment.Near;
                format.LineAlignment = StringAlignment.Near;
                format.Trimming = StringTrimming.Word;
                format.FormatFlags = StringFormatFlags.LineLimit;

                float rectHeight = i < 5 ? lineHeight : (i == textos.Length - 1 ? 61 : lineHeight * 2); // disminuir al máximo la altura de las primeras 5 filas

                graphics.DrawString(textos[i], font, Brushes.Black, new RectangleF(leftMargin, y, tableWidth, rectHeight), format);
                graphics.DrawRectangle(blackPen, leftMargin, y, tableWidth, rectHeight); // dibujar el borde
                y += rectHeight;
                yPos += rectHeight;
            }

            return yPos;
        }


    }

}
