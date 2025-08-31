using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ZXing.PDF417.Internal;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;


namespace OnlineReceiptPrintingApp
{
    public class Documents
    {

        public static void PrintDoc(string json)
        {

            //partes json
            string[] parts = json.Split('|');


            //tipo documento y medida del documento
            dynamic info = JsonConvert.DeserializeObject<dynamic>(parts[0]);

            string typeDoc = Convert.ToString(info.type_paper);

            string sizeDoc = Convert.ToString(info.paper_size);

            if (typeDoc == "openDrawer")
            {
                Custom80 obj = new Custom80();
                obj.AbreCajon();
            }
            else if (typeDoc == "receipt" && sizeDoc == "80")
            {
                //printFactura(json, true);
                printSaleReceipt(json, true);
            }
            else if (typeDoc == "receipt" && sizeDoc == "58")
            {
                Receipt58(json);
            }
            else if (typeDoc == "drawercut" && sizeDoc == "58")
            {
                printCashDrawerCut58(json);
            }
            else if (typeDoc == "drawercut" && sizeDoc == "80")
            {
                printCashDrawerCut58(json);
            }

            else if (typeDoc == "productsSold" && sizeDoc == "58")
            {
                productsSoldReceipt80(json, true);
            }
            else if (typeDoc == "productsSold" && sizeDoc == "80")
            {
                productsSoldReceipt80(json, true);
            }


            else if (typeDoc == "label")
            {
                MessageBox.Show("LA FUNCIÓN IMPRIMIR ETIQUETAS; NO ESTÁ DISPONIBLE", "APLICACION DE IMPRESION DE COMPROBANTES EN LINEA", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                System.Environment.Exit(0);
            }
            else if (typeDoc == "noDefinido")
            {
                MessageBox.Show("NO HA DEFINIDO EL TIPO DE IMPRESIÓN, REALICE LAS CONFIGURACIONES NECESARIAS EN LA APP Y EN LA PLATAFORMA ", "APLICACION DE IMPRESION DE COMPROBANTES EN LINEA", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                System.Environment.Exit(0);
            }
            else if (sizeDoc == "52X25" || sizeDoc == "4X6" || sizeDoc == "6X4" || sizeDoc == "4X3")
            {
                MessageBox.Show("EL TIPO DE PAPEL SELECCIONADO, NO ESTA DISPONIBLE [52X25, 4X6, 6X4, 4X3]", "APLICACION DE IMPRESION DE COMPROBANTES EN LINEA", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                System.Environment.Exit(0);
            }

        }

        #region Receipt58PrintDocument
        public static void Receipt58(string json)
        {

            //partes json
            string[] parts = json.Split('|');

            //deserealización
            dynamic info = JsonConvert.DeserializeObject<dynamic>(parts[0]);
            dynamic sale = JsonConvert.DeserializeObject<dynamic>(parts[1]);
            dynamic detail = JsonConvert.DeserializeObject<dynamic>(parts[2]);
            dynamic customer = JsonConvert.DeserializeObject<dynamic>(parts[3]);
            dynamic seller = JsonConvert.DeserializeObject<dynamic>(parts[4]);
            dynamic company = JsonConvert.DeserializeObject<dynamic>(parts[5]);
            dynamic totals = JsonConvert.DeserializeObject<dynamic>(parts[6]);
            dynamic change = JsonConvert.DeserializeObject<dynamic>(parts[7]);

            Custom58 obj = new Custom58();

            // info de la empresa

            // Inicializamos la lista vacía (siempre existe)
            List<Tuple<string, bool, float, string>> companyLines = new List<Tuple<string, bool, float, string>>();

            // Solo agregamos elementos si el usuario desea mostrar la info
            if (info.business_information == true)
            {
                // Normalizamos los campos opcionales de manera segura
                string taxpayerId = (company.taxpayer_id == null) ? string.Empty : company.taxpayer_id.ToString().Trim();
                string phoneTenant = (company.phone == null) ? string.Empty : company.phone.ToString().Trim();

                // Agregamos los elementos
                companyLines.Add(new Tuple<string, bool, float, string>(Convert.ToString(company.business_name), true, 7, "Center"));
                companyLines.Add(new Tuple<string, bool, float, string>(Convert.ToString(company.business_activity), false, 7, "Center"));
                companyLines.Add(new Tuple<string, bool, float, string>(Convert.ToString(company.address), false, 7, "Center"));
                //companyLines.Add(new Tuple<string, bool, float, string>((string)company.phone, false, 7, "Center"));

                // RFC solo si tiene valor
                if (!string.IsNullOrWhiteSpace(taxpayerId))
                {
                    companyLines.Add(new Tuple<string, bool, float, string>($"RFC: {taxpayerId}", false, 7f, "Center"));
                }

                // Teléfono solo si tiene valor
                if (!string.IsNullOrWhiteSpace(phoneTenant))
                {
                    companyLines.Add(new Tuple<string, bool, float, string>($"TEL: {phoneTenant}", false, 7f, "Center"));
                }

            }
            // A partir de aquí, companyLines siempre existe
            // Si info.business_information es false, la lista estará vacía y no mostrará nada
            

            // información general headers
            string saletype = (sale.type == "cash" || sale.type == "card") ? "Contado" : (sale.type == "credit" ? "Crédito" : (sale.type == "deposit" ? "Depósito/Transferencia" : (sale.type == "free" ? "Libre" : "No definido")));
            
            // Normalizamos los campos opcionales de manera segura
            string customerKey = (customer.customer_key == "none") ? string.Empty : customer.customer_key.ToString().Trim();

            List<Tuple<string, string, float>> headerLines = new List<Tuple<string, string, float>>
            {
                 new Tuple<string,string, float>($"Folio: {sale.folio}","Left", 7),
                 new Tuple<string,string, float>($"Fecha: {Convert.ToDateTime(info.sale_date).ToString("dd/MM/yy hh:mm tt")}","Left", 7),
                 new Tuple<string,string, float>($"T.v.: {saletype}","Left", 7),
                 // cliente solo si tiene valor
                 //new Tuple<string,string, float>(!string.IsNullOrWhiteSpace(customerKey) ? $"Cliente: {customerKey}" : string.Empty,"Left", 7),
                 new Tuple<string,string, float>($"Atendió: {seller.user_key}","Left", 7),
                // new Tuple<string,string, float>(sale.type == "cash" ? "Tipo: Contado" : "Tipo: Crédito","Left",7),
            };
            // Cliente solo si tiene valor
            if (!string.IsNullOrWhiteSpace(customerKey))
            {
                headerLines.Insert(3, new Tuple<string, string, float>($"Cliente: {customerKey}", "Left", 7));
            }

            // totales (cálculos) 
            decimal items = Convert.ToDecimal(totals.articulos);
            decimal subtotal = decimal.Round(Convert.ToDecimal(totals.subtotal), 2);
            decimal descuento = decimal.Round(Convert.ToDecimal(totals.descuento), 2);
            decimal iva = decimal.Round(Convert.ToDecimal(totals.iva), 2);
            decimal adicional = decimal.Round(Convert.ToDecimal(totals.adicional), 2);
            decimal envio = decimal.Round(Convert.ToDecimal(totals.envio), 2);
            decimal total = decimal.Round(Convert.ToDecimal(totals.total), 2);

            // Inicializamos la lista vacía
            List<Tuple<string, string>> totalesLines = new List<Tuple<string, string>>();

            // Siempre mostramos artículos
            totalesLines.Add(new Tuple<string, string>("ARTÍCULOS:", items.ToString()));

            // Agregamos solo si el valor es distinto de 0.0
            if (subtotal != 0.0m) totalesLines.Add(new Tuple<string, string>("SUBTOTAL:", App.moneyFormat(subtotal).ToString()));
            if (descuento != 0.0m) totalesLines.Add(new Tuple<string, string>("DESCUENTO:", App.moneyFormat(descuento).ToString())); 
            if (iva != 0.0m) totalesLines.Add(new Tuple<string, string>("IVA:", App.moneyFormat(iva).ToString())); 
            if (adicional != 0.0m) totalesLines.Add(new Tuple<string, string>("ADICIONAL:", App.moneyFormat(adicional).ToString())); 
            if (envio != 0.0m) totalesLines.Add(new Tuple<string, string>("ENVIO:", App.moneyFormat(envio).ToString())); 
            if (total != 0.0m) totalesLines.Add(new Tuple<string, string>("TOTAL:", App.moneyFormat(total).ToString())); 



            // formas de pago
            List<Tuple<string, string>> payMethod = new List<Tuple<string, string>>();

            if (sale.type == "cash")
            {
                payMethod.Add(new Tuple<string, string>("EFECTIVO:", Convert.ToString(App.moneyFormat(sale.cash))));
                decimal saleChange = Convert.ToDecimal(sale.change);
                if (saleChange > 0)
                {
                    payMethod.Add(new Tuple<string, string>("CAMBIO:", Convert.ToString(App.moneyFormat(sale.change))));

                    if (change.pending_cash_change == true)
                    {
                        string changeStatus = change.status == "pending" ? "PENDIENTE" : (change.status == "delivered" ? "ENTREGADO" : (change.status == "cancelled" ? "CANCELADO" : "SIN DEFINIR"));
                        
                        payMethod.Add(new Tuple<string, string>("ESTATUS: ", changeStatus));
                        payMethod.Add(new Tuple<string, string>("C.F.:", Convert.ToString(App.moneyFormat(change.pending_change))));
                        payMethod.Add(new Tuple<string, string>("CONSERVE EL TICKET", ":)"));
                    }
                }

            }
            if (sale.type == "card")
            {
                payMethod.Add(new Tuple<string, string>("TARJETA:", Convert.ToString(App.moneyFormat(totals.total))));
            }
            if (sale.type == "deposit")
            {
                payMethod.Add(new Tuple<string, string>("TRANSFERENCIA:", Convert.ToString(App.moneyFormat(totals.total))));
            }
            if (sale.type == "credit")
            {
                payMethod.Add(new Tuple<string, string>("CRÉDITO:", Convert.ToString(App.moneyFormat(totals.total))));
            }
            if (sale.type == "free")
            {
                payMethod.Add(new Tuple<string, string>("LIBRE:", Convert.ToString(App.moneyFormat(0))));

            }


            //imprimir
            // string codeandnotes = Convert.ToString(info).PadLeft(12, '0');
            obj.PrintReceipt58(companyLines, headerLines, info, detail, totalesLines, payMethod);
        }
        #endregion

        #region "printSaleReceipt PrintDocument POS"
        public static void printSaleReceipt(string jsonData, bool printLogo, bool printTimbre = false, bool cedible = false)
        {
            /*
             * {"id":19,"user_id":1,"customer_id":2,"total":"899.00","items":1,"status":"pending","type":"credit","cash":"0.00","change":"0.00","created_at":"2024-06-30T14:38:07.000000Z"}
             * |[{"product_id":1,"quantity":1,"sale_price":"899.00","discount":"0.00","product":{"id":1,"name":"PC Gaming"}}]
             * |{"id":2,"name":"Melisa Luna"}
             * |{"id":1,"name":"Luis Fax"}
             * |{"size":80,"type":"receipt"}
             * |{"id":1,"business_name":"IT COMPANY","address":"REFORMA 501, M\u00c9XICO D.F.","phone":"55-379-22170","taxpayer_id":"RUT123456","vat":16,"printer_name":"80mm","leyend":"Gracias por su compra!","website":"luisfaxacademy.com","created_at":"2024-05-04T17:11:22.000000Z","updated_at":"2024-05-04T18:00:47.000000Z","credit_days":15}
             * |{"articulos":1,"subtotal":775,"iva":124,"total":"899.00"}
             * |null|null|null|null
             */

            PrintPreviewDialog ppd = new PrintPreviewDialog();
            float margin = 0;
            Bitmap selloTED = null;
            string Folio = null;
            PictureBox timbre_ted;

            string[] parts = jsonData.Split('|');
            dynamic info = JsonConvert.DeserializeObject<dynamic>(parts[0]);
            dynamic venta = JsonConvert.DeserializeObject(parts[1]);
            dynamic detalle = JsonConvert.DeserializeObject(parts[2]);
            dynamic cliente = JsonConvert.DeserializeObject(parts[3]);
            dynamic user = JsonConvert.DeserializeObject(parts[4]);
            dynamic empresa = JsonConvert.DeserializeObject(parts[5]);
            dynamic totales = JsonConvert.DeserializeObject(parts[6]);
            dynamic cambio = JsonConvert.DeserializeObject(parts[7]);



            dynamic timbre = null;
            Folio = (string)venta.folio;


            // forma de pago
            List<Tuple<string, string>> payMethod = new List<Tuple<string, string>>();
            if (venta.type == "cash")
            {
                payMethod.Add(new Tuple<string, string>("EFECTIVO:", Convert.ToString(App.moneyFormat(venta.cash))));
            }

            if (venta.type == "card")
            {
                payMethod.Add(new Tuple<string, string>("TARJETA:", Convert.ToString(App.moneyFormat(totales.total))));
            }

            if (venta.type == "deposit")
            {
                payMethod.Add(new Tuple<string, string>("TRANSFERENCIA:", Convert.ToString(App.moneyFormat(totales.total))));
            }

            if (venta.type == "credit")
            {
                payMethod.Add(new Tuple<string, string>("CRÉDITO:", Convert.ToString(App.moneyFormat(totales.total))));
            }

            if (venta.type == "free")
            {
                payMethod.Add(new Tuple<string, string>("LIBRE:", Convert.ToString(App.moneyFormat(0))));
            }



            // cambio en caja
            decimal change = Convert.ToDecimal(venta.change);

            if (change > 0)
            {
                payMethod.Add(new Tuple<string, string>("CAMBIO:", Convert.ToString(App.moneyFormat(change))));

                if (cambio.pending_cash_change == true)
                {
                    string changeStatus = cambio.status == "pending" ? "PENDIENTE" : (cambio.status == "delivered" ? "ENTREGADO" : (cambio.status == "cancelled" ? "CANCELADO" : "SIN DEFINIR"));
                    payMethod.Add(new Tuple<string, string>("ESTATUS: ", changeStatus));
                    payMethod.Add(new Tuple<string, string>("C.F.", Convert.ToString(App.moneyFormat(cambio.pending_change))));
                    payMethod.Add(new Tuple<string, string>("CONSERVE EL TICKET", ":)"));
                }
            }


            dynamic delivery = null;
            if (parts.Length > 7 && parts[8] != "null")
            {
                delivery = JsonConvert.DeserializeObject(parts[7]);
            }


            if (parts.Length > 8 && parts[9] != "null")
            {
                selloTED = TED.getTEDNode(timbre);

            }

            if (parts.Length > 10 && parts[11] != "null")
            {
                Folio = parts[9];
            }

            //promos                 


            PrintDocument pd = new PrintDocument();
            //pd.PrinterSettings.PrinterName = "80mm";// ConfigManager.objConfig.Printer;
            pd.PrinterSettings.PrinterName = ConfigManager.objConfig.Printer;

            // establecer márgenes del documento a cero
            pd.DefaultPageSettings.Margins = new Margins(0, 25, 0, 0);

            // Obtener la configuración del tamaño de papel desde la impresora (ejemplo: "80mm")
            string printerSize80mm = ConfigManager.objConfig.PrinterSize;
            // Quitar la parte "mm" de la cadena y convertirla a entero para usarlo como ancho en mm
            int printerSize80 = int.Parse(printerSize80mm.Replace("MM", "")); // Resultado: 80

            // calcular 80mm de papel personalizado,
            // esto se puede tomar automático de los settings de la impresora, pero si es asiática o tiene driver genérico no siempre funciona
            int paperWidth = (int)(printerSize80 / 25.4 * 100);
            int largeHeight = 5000; // establecer un tamaño de página muy alto para permitir impresión continua y previsualizar completo el documento       
            pd.DefaultPageSettings.PaperSize = new PaperSize("Custom", paperWidth, largeHeight);


            pd.PrintPage += (sender, ev) =>
            {
                // ==================================================================== //
                //                              LOGO
                // ==================================================================== //
                float yPos = margin;
                float logoWidth = 160;
                float logoHeight = 80;

                if (File.Exists(Application.StartupPath + "\\config\\logo.png"))
                {
                    Image logo = Image.FromFile(Application.StartupPath + "\\config\\logo.png");

                    ev.Graphics.DrawImage(logo, (ev.PageBounds.Width - logoWidth) / 2, 0, logoWidth, logoHeight);

                    yPos += logoHeight;
                }



                // ==================================================================== //
                //                              EMPRESA
                // ==================================================================== //           
                SolidBrush drawBrush = new SolidBrush(Color.Black);
                // para alinear distintos bloques de info en posiciones diferentes
                StringFormat genericSF = new StringFormat();

                // alinear al centro la info de la empresa
                StringFormat companySF = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                };

                // fuente info empresa
                Font drawFont = new Font("Segoe UI", 8, FontStyle.Regular);

                // ajustar el rectángulo para centrar el texto
                RectangleF rect = new RectangleF(ev.MarginBounds.Left, yPos, ev.MarginBounds.Width, ev.MarginBounds.Height);

                if (info.business_information == true)
                {
                    // Se convierte el valor a string de manera segura
                    string telefono = empresa["phone"]?.ToString();
                    string rfc = empresa["taxpayer_id"]?.ToString();

                    // Construye el texto solo si hay teléfono
                    string textoTelefono = !string.IsNullOrEmpty(telefono) ? $"TEL: {telefono}" : "";
                    string textoRfc = !string.IsNullOrEmpty(rfc) ? $"RFC: {rfc}" : "";

                    // datos de la empresa ( puedes quitar y/o agregar los que necesites, solo debes enviarlos desde la web )               
                    yPos = TablePrinter80LLPOS.PrintText(ev.Graphics, Convert.ToString(empresa["business_name"]), drawFont, drawBrush, companySF, ev.MarginBounds, yPos);
                    yPos = TablePrinter80LLPOS.PrintText(ev.Graphics, Convert.ToString(empresa["business_activity"]), drawFont, drawBrush, companySF, ev.MarginBounds, yPos);
                    yPos = TablePrinter80LLPOS.PrintText(ev.Graphics, Convert.ToString(empresa["address"]), drawFont, drawBrush, companySF, ev.MarginBounds, yPos);
                    //yPos = TablePrinter80LLPOS.PrintText(ev.Graphics, Convert.ToString( "empresa["taxpayer_id"]), drawFont, drawBrush, companySF, ev.MarginBounds, yPos);
                    //yPos = TablePrinter80LLPOS.PrintText(ev.Graphics, $"TEL: {(string)empresa["phone"]}", drawFont, drawBrush, companySF, ev.MarginBounds, yPos);
                    yPos = TablePrinter80LLPOS.PrintText(ev.Graphics, textoRfc, drawFont, drawBrush, companySF, ev.MarginBounds, yPos);
                    yPos = TablePrinter80LLPOS.PrintText(ev.Graphics, textoTelefono, drawFont, drawBrush, companySF, ev.MarginBounds, yPos);
                }

                // separador
                yPos += 10;
                yPos = TablePrinter80LLPOS.PrintSeparator(ev.Graphics, ev, yPos, "dotted");


                // ==================================================================== //
                //                       CLIENTE
                // ==================================================================== //             
                int miClienteID = (int)cliente.id;

                if (miClienteID != 0)
                {
                    genericSF.Alignment = StringAlignment.Near;

                    yPos = TablePrinter80LLPOS.PrintText(ev.Graphics, $"CLIENTE: {Convert.ToString(cliente["customer_key"])} ", drawFont, drawBrush, genericSF, ev.MarginBounds, yPos);
                    //yPos = TablePrinter80LLPOS.PrintText(ev.Graphics, Convert.ToString(cliente["address"]), drawFont, drawBrush, companySF, ev.MarginBounds, yPos);
                    //yPos = TablePrinter80LLPOS.PrintText(ev.Graphics, Convert.ToString(cliente["phone"]), drawFont, drawBrush, companySF, ev.MarginBounds, yPos);

                    //separador
                    yPos = TablePrinter80LLPOS.PrintSeparator(ev.Graphics, ev, yPos, "dotted");
                }

                // ==================================================================== //
                //                      FOLIO Y REFERENCIA
                // ==================================================================== //

                string tipoVnta = (venta.type == "cash" || venta.type == "card")? "CONTADO" : (venta.type == "credit" ? "CRÉDITO" : (venta.type == "deposit" ? "DEPÓSITO/TRANSFERENCIA" : (venta.type == "free" ? "LIBRE" : "NO DEFINIDO")));

                yPos = TablePrinter80LLPOS.PrintText(ev.Graphics, $"FOLIO: {Folio}", drawFont, drawBrush, genericSF, ev.MarginBounds, yPos);
                yPos = TablePrinter80LLPOS.PrintText(ev.Graphics, "FECHA: " + Convert.ToDateTime(info.sale_date).ToString("dd/MM/yy hh:mm tt"), drawFont, drawBrush, genericSF, ev.MarginBounds, yPos);
                yPos = TablePrinter80LLPOS.PrintText(ev.Graphics, $"T.V.: {tipoVnta}", drawFont, drawBrush, genericSF, ev.MarginBounds, yPos);
                yPos = TablePrinter80LLPOS.PrintText(ev.Graphics, "ATENDIÓ: " + Convert.ToString(user["user_key"]) , drawFont, drawBrush, genericSF, ev.MarginBounds, yPos);
                //yPos = TablePrinter80LLPOS.PrintText(ev.Graphics, "ATENDIÓ: " + Convert.ToString(user["name"]) + " [" + Convert.ToString(user["user_key"]) + "]", drawFont, drawBrush, genericSF, ev.MarginBounds, yPos);





                // ==================================================================== //
                //                      DETALLE / PRODUCTOS
                // ==================================================================== //

                Font font = new Font("Segoe UI", 7);

                yPos = TablePrinter80LLPOS.PrintDetailTable(ev.Graphics, ev, pd, yPos, detalle);
                yPos += 10;


                // separador
                yPos += 20;// 10;
                yPos = TablePrinter80LLPOS.PrintSeparator(ev.Graphics, ev, yPos, "dotted");//-20



                // ==================================================================== //
                //                          TOTALES
                // ==================================================================== //                                 
                yPos = TablePrinter80LLPOS.PrintSubtotalTable(ev.Graphics, ev, totales, yPos, true);
                yPos += 10;



                yPos = TablePrinter80LLPOS.PrintSeparator(ev.Graphics, ev, yPos, "dotted");
                yPos += 10;

                // ==================================================================== //
                //                     FORMAS DE PAGO
                // ==================================================================== //

                yPos = TablePrinter80LLPOS.PrintPayMethodTable(ev.Graphics, ev, payMethod, yPos);
                yPos += 10;

                // separador
                yPos += 10;
                yPos = TablePrinter80LLPOS.PrintSeparator(ev.Graphics, ev, yPos, "dotted");


                // ==================================================================== //
                //                          DELIVERY
                // ==================================================================== //
                if (!(delivery is null))
                {
                    //implementar
                }



                // ==================================================================== //
                //                      NOTA DE VENTA
                // ==================================================================== //
                string notes = Convert.ToString(venta.notes);
                if (!string.IsNullOrEmpty(notes))
                {
                    yPos = TablePrinter80LLPOS.PrintNotes(ev.Graphics, ev, notes, yPos);
                    yPos += 10;
                }


                // ==================================================================== //
                //                          leyenda final
                // ==================================================================== //
                string notesleyend = Convert.ToString(empresa.leyend);
                int yPosAdds = 20; //espacio para la publicidad
                int yPosAddsBarcode = 85; //espacio para el QR

                if (!string.IsNullOrEmpty(notesleyend))
                {
                     yPosAdds = 85;
                     yPosAddsBarcode = 150;

                    RectangleF leyendRect = new RectangleF(ev.MarginBounds.Left, yPos + 20, ev.MarginBounds.Width, ev.MarginBounds.Height);
                    ev.Graphics.DrawString((string)empresa.leyend, new Font("Segoe UI", 7, FontStyle.Bold), drawBrush, leyendRect, companySF);
                }

                if (info.with_advertising == true)
                {
                    RectangleF advertisingNote = new RectangleF(ev.MarginBounds.Left, yPos + yPosAdds, ev.MarginBounds.Width, ev.MarginBounds.Height);
                    ev.Graphics.DrawString((string)info.advertising_note + info.description, new Font("Segoe UI", 7, FontStyle.Bold), drawBrush, advertisingNote, companySF);
                
                    // ==================================================================== //
                    //              Imprime el código de barras / qrcode
                    // ==================================================================== //
                    string barcode = Convert.ToString(info.website);
                    yPos = TablePrinter80LLPOS.PrintQrCode(ev.Graphics, ev, yPos + yPosAddsBarcode, barcode.PadLeft(12, '0'));
                    // yPos += 10;
                }


                ev.HasMorePages = false;

            };



            // imprimir documento
            pd.Print();

            //previsualizar documento
            //(la previsualización te muestra un 90% de lo que realmente se imprime, tiene un margen de variación y deberás hacer pruebas de impresión
            //física para ajustar cualquier detalle
            /* ppd.Document = pd;
              ppd.WindowState = FormWindowState.Maximized; // vista previa maximizada
              ppd.PrintPreviewControl.Zoom = 2.0; // establecer el zoom al 200%
              ppd.ShowDialog();*/

            // =========================
            // CORTE PRECISO DE PAPEL + APERTURA DE CAJA
            // =========================
            StringBuilder command = new StringBuilder();

            // Si la impresora necesita un feed mínimo antes del corte, podemos usar 1-2 líneas máximo
            // if (!App.NoBreakLines())
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
                // Se verifica la configuración de la impresora para determinar si se debe cortar el ticket automáticamente.
                // ConfigManager.objConfig.Cut es un valor booleano que indica si la impresora tiene habilitado el corte automático.
                // Corte total del ticket (corta a la cuchilla)
                command.Append("\x1D" + "V" + "\x00"); // corte completo (sin dejar papel extra)
                // Se envía el comando de corte a la impresora:
                // "\x1D"  → GS (Group Separator), inicio del comando de corte
                // "V"     → instrucción de corte (GS V)
                // "\x00"  → tipo de corte completo (corte total de la hoja en la posición actual)
                // Este comando asegura que la impresora corte exactamente donde termina la impresión,
                // sin avanzar líneas adicionales ni dejar papel sobrante sobre la cuchilla.
            }

            // Abrir cajón de efectivo
            if (ConfigManager.objConfig.Drawer == true)
            {
                // Se verifica la configuración de la impresora para determinar si se debe abrir automáticamente el cajón de efectivo.
                // ConfigManager.objConfig.Drawer es un valor booleano que indica si el cajón está habilitado para abrirse desde la impresora.
                // Abrir cajón de efectivo
                command.Append("\x1B" + "p" + "\x00" + "\x0F" + "\x96"); // Pin 0, pulso 15ms, intervalo 150ms
                // Comando ESC/POS para abrir el cajón de efectivo conectado a la impresora:
                // "\x1B" → ESC (Escape), inicio de comando
                // "p"    → instrucción para activar el pin del cajón (pulse)
                // "\x00" → selecciona el Pin 0 (la línea de activación del cajón; algunas impresoras tienen Pin 2 o Pin 1)
                // "\x0F" → tiempo de pulso en milisegundos (15 ms)
                // "\x96" → intervalo entre pulsos (150 ms)
                // Este comando genera un pulso eléctrico al pin indicado para abrir el cajón de manera segura
                // sin necesidad de intervención manual, asegurando que el cajón se abra justo después de imprimir el ticket.
            }


            // Enviar comandos a la impresora
            RawPrinterHelper.SendStringToPrinter(ConfigManager.objConfig.Printer, command.ToString());
            command.Clear();


        }
        #endregion

        #region "printFactura PrintDocument"
        public static void printFactura(string jsonData, bool printLogo, bool printTimbre = false, bool cedible = false)
        {

            PrintPreviewDialog ppd = new PrintPreviewDialog();
            float margin = 0;
            Bitmap selloTED = null;
            string Folio = null;
            PictureBox timbre_ted;

            //json parts
            string[] parts = jsonData.Split('|');
            dynamic venta = JsonConvert.DeserializeObject(parts[0]);
            dynamic detalle = JsonConvert.DeserializeObject(parts[1]);
            dynamic cliente = JsonConvert.DeserializeObject(parts[2]);
            dynamic user = JsonConvert.DeserializeObject(parts[3]);
            dynamic info = JsonConvert.DeserializeObject(parts[4]); //document / typeDoc / tipoDocumento
            dynamic empresa = JsonConvert.DeserializeObject(parts[5]);
            dynamic arrayTotales = JsonConvert.DeserializeObject(parts[6]);


            dynamic timbre = null;
            string emision = null;
            Folio = (string)venta.id;

            //forma de pago
            List<Tuple<string, string>> payMethod = new List<Tuple<string, string>>();
            if (venta.type == "cash")
            {
                payMethod.Add(new Tuple<string, string>("EFECTIVO:", Convert.ToString(App.moneyFormat(venta.cash))));
            }
            if (venta.type == "deposit")
            {
                payMethod.Add(new Tuple<string, string>("TRANSFERENCIA:", Convert.ToString(App.moneyFormat(venta.total))));
            }
            if (venta.type == "credit")
            {
                payMethod.Add(new Tuple<string, string>("CRÉDITO:", Convert.ToString(App.moneyFormat(venta.total))));
            }
            //cambio
            decimal change = Convert.ToDecimal(venta.change);
            if (change > 0)
            {
                payMethod.Add(new Tuple<string, string>("CAMBIO:", Convert.ToString(App.moneyFormat(change))));
            }
            //delivery
            dynamic delivery = null;
            if (parts.Length > 6 && parts[7] != "null")
            {
                delivery = JsonConvert.DeserializeObject(parts[7]);
            }
            //timbre fiscal
            if (parts.Length > 7 && parts[8] != "null")
            {
                selloTED = TED.getTEDNode(timbre);
            }
            // folio fiscal
            if (parts.Length > 8 && parts[9] != "null")
            {
                Folio = parts[9];
            }
            //promociones


            string NoResol = (string)empresa["numeroResolucion"];
            string fechaResol = (string)empresa["fechaResolucion"];
            string anioResol = (fechaResol != null ? fechaResol.Substring(0, 4) : "");


            PrintDocument pd = new PrintDocument();
            pd.PrinterSettings.PrinterName = ConfigManager.objConfig.Printer; //definiendo la impresora

            //márgenes del documento
            pd.DefaultPageSettings.Margins = new Margins(0, 25, 0, 0);
            int paperWith = (int)(80 / 25.4 * 100); //80mm
            int largeHeith = 5000; //alto del documento
            pd.DefaultPageSettings.PaperSize = new PaperSize("Custom", paperWith, largeHeith);

            pd.PrintPage += (sender, ev) => {

                // logo
                float yPos = margin;
                float logoWith = 180;
                float logoHeight = 100;
                if (File.Exists(Application.StartupPath + "\\config\\logo.png"))
                {
                    Image logo = Image.FromFile(Application.StartupPath + "\\config\\logo.png");
                    ev.Graphics.DrawImage(logo, (ev.PageBounds.Width - logoWith) / 2, 0, logoWith, logoHeight);
                    yPos += logoHeight;
                }

                // recuadro sii
                //yPos =   TablePrinter80.PrintRecuadro(ev.Graphics, ev, pd, venta, yPos, empresa, Folio);

                //empresa
                SolidBrush drawBrush = new SolidBrush(Color.Black); //color de texto
                StringFormat genericSF = new StringFormat(); //alinear bloque info general
                // alinear el texto de la empresa
                StringFormat companySF = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                };
                //fuente
                Font drawFont = new Font("Segoe UI", 8, FontStyle.Regular);

                RectangleF rect = new RectangleF(ev.MarginBounds.Left, yPos, ev.MarginBounds.Width, ev.MarginBounds.Height);

                yPos = TablePrinter80.PrintText(ev.Graphics, Convert.ToString(empresa["business_name"]), drawFont, drawBrush, companySF, ev.MarginBounds, yPos);
                yPos = TablePrinter80.PrintText(ev.Graphics, Convert.ToString(empresa["address"]), drawFont, drawBrush, companySF, ev.MarginBounds, yPos);
                yPos = TablePrinter80.PrintText(ev.Graphics, Convert.ToString(empresa["taxpayer_id"]), drawFont, drawBrush, companySF, ev.MarginBounds, yPos);
                yPos = TablePrinter80.PrintText(ev.Graphics, $"TELÉFONO: {(string)empresa["phone"]}", drawFont, drawBrush, companySF, ev.MarginBounds, yPos);


                //separador
                yPos = TablePrinter80.PrintSeparator(ev.Graphics, ev, yPos, "dotted");


                // cliente
                genericSF.Alignment = StringAlignment.Near;
                yPos = TablePrinter80.PrintText(ev.Graphics, Convert.ToString("Cliente: " + cliente["name"]), drawFont, drawBrush, genericSF, ev.MarginBounds, yPos);


                //separador
                yPos = TablePrinter80.PrintSeparator(ev.Graphics, ev, yPos, "dotted");

                //folio referencia
                string tipoVenta = venta.type == "cash" ? "CONTADO" : "CRÉDITO";

                yPos = TablePrinter80.PrintText(ev.Graphics, $"FOLIO: {Folio}", drawFont, drawBrush, genericSF, ev.MarginBounds, yPos);
                yPos = TablePrinter80.PrintText(ev.Graphics, "DOCUMENTOS DE REFERENCIA", drawFont, drawBrush, genericSF, ev.MarginBounds, yPos);
                yPos = TablePrinter80.PrintText(ev.Graphics, "TIPO DOCUMENTO: FACTURA ELECTRONICA", drawFont, drawBrush, genericSF, ev.MarginBounds, yPos);
                yPos = TablePrinter80.PrintText(ev.Graphics, $"TIPO VENTA: {tipoVenta}", drawFont, drawBrush, genericSF, ev.MarginBounds, yPos);
                yPos = TablePrinter80.PrintText(ev.Graphics, $"FECHA:{Convert.ToDateTime(venta.created_at).ToString("dd/MM/yyyy hh:mm")}", drawFont, drawBrush, genericSF, ev.MarginBounds, yPos);


                //detalle  / productos
                Font font = new Font("Segoe UI", 7);
                yPos = TablePrinter80.PrintDetailTable(ev.Graphics, ev, pd, yPos, detalle);
                yPos += 10;


                //separador
                yPos += 20;
                yPos = TablePrinter80.PrintSeparator(ev.Graphics, ev, yPos, "dotted");



                // totales de la venta
                yPos = TablePrinter80.PrintSubtotalTable(ev.Graphics, ev, arrayTotales, yPos, true);
                yPos += 10;


                //separador
                yPos = TablePrinter80.PrintSeparator(ev.Graphics, ev, yPos, "dotted");
                yPos += 10;


                // formas de pago
                yPos = TablePrinter80.PrintPayMethodTable(ev.Graphics, ev, payMethod, yPos);
                yPos += 10;



                yPos = TablePrinter80.PrintSeparator(ev.Graphics, ev, yPos, "dotted");
                yPos += 10;

                // delivery              
                if (!(delivery is null))
                {  //if(delivery != null)
                    if (Convert.ToString(delivery) != "null")
                    {


                        string del_type = (string)delivery["type"];
                        string del_direccion = (string)delivery["direccion"];
                        string del_comuna = (string)delivery["comuna"];
                        string del_numero = (string)delivery["numero"];
                        string del_costo = (string)delivery["costo"];
                        string del_detalles = (string)delivery["detalles"];
                        StringFormat aligDely = new StringFormat();
                        aligDely.Alignment = StringAlignment.Center;


                        List<Tuple<string>> deliveryDetails = new List<Tuple<string>>();

                        deliveryDetails.Add(new Tuple<string>("Delivery:"));

                        deliveryDetails.Add(new Tuple<string>(del_type.Trim()));


                        if (!String.IsNullOrEmpty(del_direccion))
                        {
                            deliveryDetails.Add(new Tuple<string>(del_direccion.Trim()));
                        }

                        if (!String.IsNullOrEmpty(del_numero))
                        {
                            deliveryDetails.Add(new Tuple<string>("Número: " + del_numero.Trim()));
                        }

                        if (!String.IsNullOrEmpty(del_comuna))
                        {
                            deliveryDetails.Add(new Tuple<string>("Comuna: " + del_comuna.Trim()));
                        }

                        if (!String.IsNullOrEmpty(del_detalles))
                        {
                            deliveryDetails.Add(new Tuple<string>("Número: " + del_detalles.Trim()));
                        }

                        if (!String.IsNullOrEmpty(del_costo))
                        {
                            deliveryDetails.Add(new Tuple<string>("Costo: $" + del_costo.Trim()));
                        }

                        yPos = TablePrinter80.PrintRows(ev.Graphics, ev, deliveryDetails, yPos, 14, "left");
                        yPos += 10;

                    }


                }

                //recuadro firma
                if (cedible)
                {

                    //yPos = TablePrinter80.PrintSignTable(ev.Graphics, ev, pd, yPos);
                    //yPos += 20;

                }



                // timbre fiscal
                StringFormat alignTimbre = new StringFormat();
                alignTimbre.Alignment = StringAlignment.Center;

                if (printTimbre && timbre != "null")
                {
                    timbre_ted = new PictureBox();
                    timbre_ted.Image = selloTED;

                    // Calcular la posición x para centrar la imagen
                    float xPos = (ev.PageBounds.Width - 220) / 2; // puedes jugar con el valor 220 según tus necesidades

                    RectangleF rectImage = new RectangleF(xPos, yPos, 220, 67);
                    ev.Graphics.DrawImage(timbre_ted.Image, rectImage);
                    yPos += 67;


                    List<Tuple<string>> timbreFoot = new List<Tuple<string>> {
                         new Tuple<string>("TIMBRE ELECTRONICO SII"),
                         new Tuple<string>("RES. Nro. " + NoResol + " del anio " + anioResol),
                         new Tuple<string>("Verifique Documento en www.sii.cl")
                  };


                    yPos = TablePrinter80.PrintTimbreFooter(ev.Graphics, ev, timbreFoot, yPos);
                    yPos += 10;


                }



                if (cedible)
                {
                    List<Tuple<string>> rowList = new List<Tuple<string>> {
                         new Tuple<string>(cedible ? "CEDIBLE" : "ORIGINAL"),
                         new Tuple<string>("Contrata factura electrónica en www.uvo.cl"),
                         new Tuple<string>("v5.0")
                  };

                    yPos = TablePrinter80.PrintRows(ev.Graphics, ev, rowList, yPos);
                    yPos += 10;
                }




                /// codigo de  barras
                string barcode = Convert.ToString(venta.id);
                yPos = TablePrinter80.PrintBarcode(ev.Graphics, ev, yPos, barcode.PadLeft(12, '0')); // 000000000010
                //yPos = TablePrinter58.PrintQrCode(ev.Graphics, ev, yPos, "https://www.luisfaxacademy.com/ninjas-community");
                yPos += 10;
                /// codigo QR


                //leyenda
                RectangleF leyendRect = new RectangleF(ev.MarginBounds.Left, yPos + 20, ev.MarginBounds.Width, ev.MarginBounds.Height);
                ev.Graphics.DrawString((string)empresa.leyend, new Font("Segoe UI", 7, FontStyle.Bold), drawBrush, leyendRect, companySF);




                ev.HasMorePages = false;


            };

            // imprimir documento
            //pd.Print();


            // previsualizar documento
            //previsualizar documento
            //(la previsualización te muestra un 90% de lo que realmente se imprime, tiene un margen de variación y deberás hacer pruebas de impresión
            //física para ajustar cualquier detalle
            ppd.Document = pd;
            ppd.WindowState = FormWindowState.Maximized;
            ppd.PrintPreviewControl.Zoom = 2.0; // zoom al 200%
            ppd.ShowDialog();




            //cortar papel
            StringBuilder command = new StringBuilder();
            if (!App.NoBreakLines())
            {
                // imprimir espacios al final del ticket para agregar márgen después del branding / leyenda
                // se imprimen 5 espacios, esto debes ajustarlo en función de tu impresora, porque algunas impresoras asiáticas sacan mucho papel blanco al final
                // y algunas ocuparás imprimir más de 5 espacios
                command.AppendLine("");
                command.AppendLine("");
                command.AppendLine("");
                command.AppendLine("");
                command.AppendLine("");
            }
            // estos comandos de corte pueden variar según el modelo de impresora
            command.AppendLine("\x1B" + "m");
            command.AppendLine("\x1B" + "d" + "\x0");
            RawPrinterHelper.SendStringToPrinter(ConfigManager.objConfig.Printer, command.ToString());
            command.Clear();



            // este método se puede incluir o no dentro del código C#, ya que si configuras el Cash Drawer en los settings de la impresora,
            // el cajón se abre automáticamente           
            // tener este método es útil cuando deseas abrir el cajón para guardar o entregar cambio, voucher, corte de caja, etc.
            // AbreCajon();


        }
        #endregion

        #region CashDrawerCutPrintDocument
        public static void printCashDrawerCut58(string json)
        {

            //partes json
            string[] parts = json.Split('|');

            //deserealización
            dynamic corteDeCaja = JsonConvert.DeserializeObject<dynamic>(parts[1]);
            dynamic detalleGeneral = JsonConvert.DeserializeObject<dynamic>(parts[2]);
            dynamic ventas = JsonConvert.DeserializeObject<dynamic>(parts[3]);
            dynamic otrosDatos = JsonConvert.DeserializeObject<dynamic>(parts[4]);
            dynamic entradaEfectivo = JsonConvert.DeserializeObject<dynamic>(parts[5]);
            dynamic salidaEfectivo = JsonConvert.DeserializeObject<dynamic>(parts[6]);
            dynamic usuario = JsonConvert.DeserializeObject<dynamic>(parts[7]);
            dynamic caja = JsonConvert.DeserializeObject<dynamic>(parts[8]);
            //dynamic empresa = JsonConvert.DeserializeObject<dynamic>(parts[9]);
            // dynamic consultado = JsonConvert.DeserializeObject<dynamic>(parts[10]);


            Custom58 obj = new Custom58();

            // info de la empresa
            List<Tuple<string, bool, float, string>> companyLines = new List<Tuple<string, bool, float, string>> {
                new Tuple<string,bool, float,string>(Convert.ToString("CORTE DE CAJA"), true, 8, "Center"),
                /*new Tuple<string,bool, float,string>(Convert.ToString(empresa.name), true, 8, "Center"),
                new Tuple<string,bool, float,string>(Convert.ToString(empresa.description), false, 7, "Center"),
                new Tuple<string,bool, float,string>(Convert.ToString(empresa.address), false, 7, "Center")*/
            };


            // información general headers
            List<Tuple<string, string, float>> headerLines = new List<Tuple<string, string, float>>
            {
                 new Tuple<string,string, float>($"{caja.name}","Left", 7),
                 new Tuple<string,string, float>($"Folio: {corteDeCaja.id}","Left", 7),
                 new Tuple<string,string, float>($"Fecha: {corteDeCaja.cut_date}","Left", 7),
                 new Tuple<string,string, float>($"Usuario: {usuario.name}","Left", 7),
                 new Tuple<string,string, float>($"Email: {usuario.email}","Left", 7)
            };

            // seccion en caja
            List<Tuple<string, bool, float, string>> enCajaLines = new List<Tuple<string, bool, float, string>> {
                new Tuple<string,bool, float,string>(Convert.ToString("EFECTIVO EN CAJA"), true, 8, "Center")
            };
            // totales en caja
            List<Tuple<string, string>> totalEnCajaLines = new List<Tuple<string, string>>
            {
                new Tuple<string, string>("Total: ", corteDeCaja.cash_in_cashregister.ToString("c"))

             };

            // seccion ventas
            List<Tuple<string, bool, float, string>> ventasLines = new List<Tuple<string, bool, float, string>> {
                new Tuple<string,bool, float,string>(Convert.ToString("VENTAS"), true, 8, "Center")
            };
            // totales ventas
            List<Tuple<string, string>> totalVentasLines = new List<Tuple<string, string>>
            {
                new Tuple<string, string>("Vetas Totales:", corteDeCaja.total_cash_sales.ToString("c"))

            };

            // seccion otros datos
            List<Tuple<string, bool, float, string>> otrosDatosLines = new List<Tuple<string, bool, float, string>> {
                new Tuple<string,bool, float,string>(Convert.ToString("OTROS DATOS"), true, 8, "Center")
            };

            // seccion entrada de efectivo
            List<Tuple<string, bool, float, string>> entradaLines = new List<Tuple<string, bool, float, string>> {
                new Tuple<string,bool, float,string>(Convert.ToString("ENTRADA DE EFECTIVO"), true, 8, "Center")
            };
            // totales entrada de efectivo
            List<Tuple<string, string>> totalEntradaLines = new List<Tuple<string, string>>
            {
                new Tuple<string, string>("Depósitos Totales:", corteDeCaja.cash_inflow.ToString("c"))

            };

            // seccion salida de efectivo
            List<Tuple<string, bool, float, string>> salidaLines = new List<Tuple<string, bool, float, string>> {
                new Tuple<string,bool, float,string>(Convert.ToString("SALIDA DE EFECTIVO"), true, 8, "Center")
            };
            // totales salida de efectivo
            List<Tuple<string, string>> totalSalidaLines = new List<Tuple<string, string>>
            {
                new Tuple<string, string>("Retiros Totales:", corteDeCaja.cash_outflow.ToString("c"))

            };

            // seccion consultado
            List<Tuple<string, bool, float, string>> consultadoLines = new List<Tuple<string, bool, float, string>>
            {
                /* new Tuple<string,bool, float,string>(Convert.ToString("CONSULTADO"), true, 8, "Center"),
                 new Tuple<string,bool, float,string>(Convert.ToString(consultado.usuario), true, 8, "Center"),
                 new Tuple<string,bool, float,string>(Convert.ToString(consultado.email), false, 7, "Center"),
                 new Tuple<string,bool, float,string>(Convert.ToString(consultado.fecha), false, 7, "Center")*/
            };


            // formas de pago
            List<Tuple<string, string>> payMethod = new List<Tuple<string, string>>();
            //payMethod.Add(new Tuple<string, string>("EFECTIVO:", Convert.ToDecimal(sale.cash).ToString("c"))); // $1500
            //payMethod.Add(new Tuple<string, string>("CAMBIO:", Convert.ToDecimal(cash_sale - total_sale).ToString("c")));


            //imprimir
            obj.PrintCashDrawerCut58(
                companyLines, enCajaLines, ventasLines, otrosDatosLines, entradaLines, salidaLines, consultadoLines,
                headerLines, detalleGeneral, ventas, otrosDatos, entradaEfectivo, salidaEfectivo,
                totalEnCajaLines, totalVentasLines, totalEntradaLines, totalSalidaLines,
                payMethod);

        }
        #endregion

        #region productsSoldPrintDocument
        public static void productsSoldReceipt80(string jsonData, bool printLogo, bool printTimbre = false, bool cedible = false)
        {


            PrintPreviewDialog ppd = new PrintPreviewDialog();
            float margin = 0;
            string Folio = null;

            string[] parts = jsonData.Split('|');
            dynamic info = JsonConvert.DeserializeObject<dynamic>(parts[0]);
            dynamic detalle = JsonConvert.DeserializeObject(parts[1]);
            dynamic venta = JsonConvert.DeserializeObject(parts[2]);



            Folio = (string)detalle.folio;




            PrintDocument pd = new PrintDocument();
            //pd.PrinterSettings.PrinterName = "80mm";// ConfigManager.objConfig.Printer;
            pd.PrinterSettings.PrinterName = ConfigManager.objConfig.Printer;



            // establecer márgenes del documento a cero
            pd.DefaultPageSettings.Margins = new Margins(0, 25, 0, 0);

            // calcular 80mm de papel personalizado,
            // esto se puede tomar automático de los settings de la impresora, pero si es asiática o tiene driver genérico no siempre funciona
            int paperWidth = (int)(80 / 25.4 * 100);
            int largeHeight = 5000; // establecer un tamaño de página muy alto para permitir impresión continua y previsualizar completo el documento       
            pd.DefaultPageSettings.PaperSize = new PaperSize("Custom", paperWidth, largeHeight);


            pd.PrintPage += (sender, ev) =>
            {
                // ==================================================================== //
                //                              LOGO
                // ==================================================================== //
                float yPos = margin;
                float logoWidth = 160;
                float logoHeight = 80;

                if (File.Exists(Application.StartupPath + "\\config\\logo.png"))
                {
                    Image logo = Image.FromFile(Application.StartupPath + "\\config\\logo.png");

                    ev.Graphics.DrawImage(logo, (ev.PageBounds.Width - logoWidth) / 2, 0, logoWidth, logoHeight);

                    yPos += logoHeight;
                }



                // ==================================================================== //
                //                              EMPRESA
                // ==================================================================== //           
                SolidBrush drawBrush = new SolidBrush(Color.Black);
                // para alinear distintos bloques de info en posiciones diferentes
                StringFormat genericSF = new StringFormat();

                // alinear al centro la info de la empresa
                StringFormat companySF = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                };

                // fuente info empresa
                Font drawFont = new Font("Segoe UI", 8, FontStyle.Regular);

                // ajustar el rectángulo para centrar el texto
                RectangleF rect = new RectangleF(ev.MarginBounds.Left, yPos, ev.MarginBounds.Width, ev.MarginBounds.Height);


                //separador
                yPos = TablePrinter80LLPOS.PrintSeparator(ev.Graphics, ev, yPos, "dotted");

                yPos = TablePrinter80LLPOS.PrintText(ev.Graphics, Convert.ToString(detalle["operation"]), drawFont, drawBrush, companySF, ev.MarginBounds, yPos);

                //separador
                yPos = TablePrinter80LLPOS.PrintSeparator(ev.Graphics, ev, yPos, "dotted");


                // ==================================================================== //
                //                      FOLIO Y REFERENCIA
                // ==================================================================== //



                yPos = TablePrinter80LLPOS.PrintText(ev.Graphics, $"FOLIO: {Folio}", drawFont, drawBrush, genericSF, ev.MarginBounds, yPos);
                yPos = TablePrinter80LLPOS.PrintText(ev.Graphics, $"TOTAL: ${Convert.ToString(App.moneyFormat(detalle.total))}", drawFont, drawBrush, genericSF, ev.MarginBounds, yPos);
                yPos = TablePrinter80LLPOS.PrintText(ev.Graphics, $"PRODUCTOS: {Convert.ToString(detalle["items"])}", drawFont, drawBrush, genericSF, ev.MarginBounds, yPos);
                yPos = TablePrinter80LLPOS.PrintText(ev.Graphics, $"STATUS: {Convert.ToString(detalle["status"])}", drawFont, drawBrush, genericSF, ev.MarginBounds, yPos);
                yPos = TablePrinter80LLPOS.PrintText(ev.Graphics, $"FECHA: {Convert.ToString(detalle["date"])}", drawFont, drawBrush, genericSF, ev.MarginBounds, yPos);
                // yPos = TablePrinter80LLPOS.PrintText(ev.Graphics, $"USUARIO: {Convert.ToString(detalle["name"])}", drawFont, drawBrush, genericSF, ev.MarginBounds, yPos);
                //yPos = TablePrinter80LLPOS.PrintText(ev.Graphics, $"EMAIL: {Convert.ToString(detalle["email"])}", drawFont, drawBrush, genericSF, ev.MarginBounds, yPos);

                // separador
                yPos = TablePrinter80LLPOS.PrintSeparator(ev.Graphics, ev, yPos, "dotted");


                // ==================================================================== //
                //                       CLIENTE & USUARIO
                // ==================================================================== //             

                genericSF.Alignment = StringAlignment.Near;

                yPos = TablePrinter80LLPOS.PrintText(ev.Graphics, $"USUARIO: {Convert.ToString(detalle["user"])}", drawFont, drawBrush, genericSF, ev.MarginBounds, yPos);
                //yPos = TablePrinter80LLPOS.PrintText(ev.Graphics, Convert.ToString(cliente["address"]), drawFont, drawBrush, companySF, ev.MarginBounds, yPos);
                //yPos = TablePrinter80LLPOS.PrintText(ev.Graphics, Convert.ToString(cliente["phone"]), drawFont, drawBrush, companySF, ev.MarginBounds, yPos);
                yPos = TablePrinter80LLPOS.PrintText(ev.Graphics, $"EMAIL: {Convert.ToString(detalle["email"])}", drawFont, drawBrush, genericSF, ev.MarginBounds, yPos);




                // ==================================================================== //
                //                      DETALLE / PRODUCTOS
                // ==================================================================== //

                Font font = new Font("Segoe UI", 7);

                yPos = TablePrinter80LLPOS.PrintSaleDetailTable(ev.Graphics, ev, pd, yPos, venta);
                yPos += 10;


                // separador
                yPos += 20;// 10;
                yPos = TablePrinter80LLPOS.PrintSeparator(ev.Graphics, ev, yPos, "dotted");//-20




                ev.HasMorePages = false;

            };



            // imprimir documento
            pd.Print();

            //previsualizar documento
            //(la previsualización te muestra un 90% de lo que realmente se imprime, tiene un margen de variación y deberás hacer pruebas de impresión
            //física para ajustar cualquier detalle
            /*ppd.Document = pd;
            ppd.WindowState = FormWindowState.Maximized; // vista previa maximizada
            ppd.PrintPreviewControl.Zoom = 2.0; // establecer el zoom al 200%
            ppd.ShowDialog();*/




            //cortar papel
            StringBuilder command = new StringBuilder();
            if (!App.NoBreakLines())
            {
                // imprimir espacios al final del ticket para agregar márgen después del branding / leyenda
                // se imprimen 5 espacios, esto debes ajustarlo en función de tu impresora, porque algunas impresoras asiáticas sacan mucho papel blanco al final
                // y algunas ocuparás imprimir más de 5 espacios
                command.AppendLine("");
                command.AppendLine("");
                // command.AppendLine("");
                //command.AppendLine("");
                //command.AppendLine("");
            }
            // estos comandos de corte pueden variar según el modelo de impresora
            //command.AppendLine("\x1B" + "m");
            command.AppendLine("\x1B" + "d" + "\x0");

            RawPrinterHelper.SendStringToPrinter(ConfigManager.objConfig.Printer, command.ToString());
            command.Clear();


        }
        #endregion
    }
}


