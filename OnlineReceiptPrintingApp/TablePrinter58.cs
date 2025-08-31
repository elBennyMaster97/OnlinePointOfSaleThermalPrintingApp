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
    using static System.Net.Mime.MediaTypeNames;

    public class TablePrinter58
    {
        /* Para calcular con mayor precisión los espacios y alineación de texto, es recomendable utilizar fuentes monoespaciadas,
         * ya que  este tipo de fuente maneja un espacio constante entre caracteres. Para nuestro caso estamos utilizando la fuente: "Segoe UI".
        */

        // fuente general
        static Font Font58 = new Font("Segoe UI", 8);
        static Font Font58Bold = new Font("Segoe UI", 8, FontStyle.Bold);
        static Font Font58Separator = new Font("Segoe UI", 10);


        // Método para imprimir la tabla de subtotales
        public static float PrintSubtotalTable(Graphics graphics, PrintPageEventArgs e, List<Tuple<string, string>> rows, float yPos, bool center = false, bool borders = false)
        {
            // definir los márgenes y la altura de fila
            int leftMargin = 5;
            int rightMargin = 30;
            int rowHeight = 16;


            // Define columnas dinámicas
            float[] columnWidths = { (e.PageBounds.Width - leftMargin - rightMargin) * 0.7f, (e.PageBounds.Width - leftMargin - rightMargin) * 0.3f };

            // Define column positions
            float[] columnPositions = { leftMargin, leftMargin + columnWidths[0] };

            // alineación de texto
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Far;// center ? StringAlignment.Center : StringAlignment.Near; // puedes utilizar el argumento center para cambiar la alineación

            // Alineación de texto para la segunda columna (derecha)
            StringFormat sfRight = new StringFormat();
            sfRight.Alignment = StringAlignment.Far;


            // definir el pincel para dibujar los bordes(definido en el argumento que recibe este método (borders)
            Pen pen = new Pen(Color.Black, 0.5f);
            pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;

            // dibujar las filas de la tabla
            foreach (var row in rows)
            {
                // dibujar texto en la primera columna
                graphics.DrawString(row.Item1, Font58, Brushes.Black, new RectangleF(columnPositions[0], yPos, columnWidths[0], rowHeight), sf);

                // dibujar texto en la segunda columna
                graphics.DrawString(row.Item2, Font58, Brushes.Black, new RectangleF(columnPositions[1], yPos, columnWidths[1], rowHeight), sfRight);

                // dibujar los bordes
                if (borders)
                {
                    graphics.DrawRectangle(pen, leftMargin, yPos, columnWidths[0], rowHeight);
                    graphics.DrawRectangle(pen, leftMargin + columnWidths[0], yPos, columnWidths[1], rowHeight);
                }

                // incrementar la posición Y para la siguiente fila
                yPos += rowHeight;
            }
            return yPos;  // retorna la posicion de Y para que los siguientes métodos sigan imprimiendo hacia abajo
        }








        // método para imprimir la tabla métodos de pago
        public static float PrintPayMethodTable(Graphics graphics, PrintPageEventArgs e, List<Tuple<string, string>> rows, float yPos, bool borders = false)
        {
            // definir márgenes y altura de fila
            int leftMargin = 5;
            int rightMargin = 30;
            int rowHeight = 16;


            // calcular ancho de las columnas
            float fullWidth = e.PageBounds.Width - leftMargin - rightMargin;
            float headerWidth = fullWidth;
            float columnWidth = fullWidth / 2;

            // formatos alineación de texto
            StringFormat centerFormat = new StringFormat() { Alignment = StringAlignment.Center };
            StringFormat rightFormat = new StringFormat() { Alignment = StringAlignment.Far };

            // dibujar el encabezado para formas de pago      
            graphics.DrawString("FORMA DE PAGO:", Font58Bold, Brushes.Black, new RectangleF(leftMargin, yPos, headerWidth, rowHeight), centerFormat);
            yPos += rowHeight;


            // dibujar las filas de la tabla
            foreach (var row in rows)
            {
                // dibujar texto en la primera columna de la fila
                graphics.DrawString(row.Item1, Font58, Brushes.Black, new RectangleF(leftMargin, yPos, columnWidth, rowHeight), rightFormat);

                // dibujar texto en la segunda columna de la fila
                graphics.DrawString(row.Item2, Font58, Brushes.Black, new RectangleF(leftMargin + columnWidth, yPos, columnWidth, rowHeight), rightFormat);

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
                Format = BarcodeFormat.EAN_13,
                Options = new EncodingOptions { Height = 30, Width = 210, PureBarcode = true }
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
        }


    }

}