using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace OnlineReceiptPrintingApp
{
    public partial class frmConfig : Form
    {
        public frmConfig()
        {
            InitializeComponent();
        }

        private void frmConfig_Load(object sender, EventArgs e)
        {
            LLenarImpresoras(cmbImpresoraRecibos);
            LLenarImpresoras(cmbImpresoraEtiquetas);


            if (cmbImpresoraRecibos.Items.Count > 0) cmbImpresoraRecibos.SelectedIndex = 0;
            cmbRecibosSize.SelectedIndex = 0;

            if (cmbImpresoraEtiquetas.Items.Count > 0) cmbImpresoraEtiquetas.SelectedIndex = 0;
            cmbEtiquetasSize.SelectedIndex = 0;

            cmbCortarPapel.SelectedIndex = 0;
            cmbAbrirCajon.SelectedIndex = 0;
            cmbSaltos.SelectedIndex = 0;


            if (ConfigManager.objConfig != null)
            {
                cmbImpresoraRecibos.Text = ConfigManager.objConfig.Printer;
                cmbRecibosSize.Text = ConfigManager.objConfig.PrinterSize;


                cmbImpresoraEtiquetas.Text = ConfigManager.objConfig.PrinterLabel;
                cmbEtiquetasSize.Text = ConfigManager.objConfig.PrinterLabelSize;


                cmbCortarPapel.Text = ConfigManager.objConfig.Cut == true ? "SI" : "NO";
                cmbAbrirCajon.Text = ConfigManager.objConfig.Drawer == true ? "SI" : "NO";
                cmbSaltos.Text = ConfigManager.objConfig.BreakLines == true ? "SI" : "NO";
            }


            //cargar logo si
            //if (File.Exists(Application.StartupPath + "\\config\\logo.png")) {
            //    picLogo.Image = Image.FromFile(Application.StartupPath + "\\config\\logo.png");
            //}

            string imagePath = Path.Combine(Application.StartupPath, "config", "logo.png");
            if (File.Exists(imagePath))
            {
                using (FileStream fs = File.OpenRead(imagePath))
                {

                    //crear la imagen desde el flujo de datos
                    Image img = Image.FromStream(fs);

                    picLogo.Image = img;
                }
            }




        }

        #region "METODOS GENERALES"
        public void LLenarImpresoras(ComboBox combobox)
        {

            string impresoraPredeterminada = new PrinterSettings().PrinterName;

            foreach (string impresora in PrinterSettings.InstalledPrinters)
            {

                combobox.Items.Add(impresora);

                if (impresora == impresoraPredeterminada)
                {

                    var defaultPrinter = combobox.Items[combobox.Items.Count - 1];

                    combobox.Items.RemoveAt(combobox.Items.Count - 1);

                    combobox.Items.Insert(0, defaultPrinter); ;
                }
            }
            combobox.SelectedIndex = 0;
        }
        #endregion

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialogo = new OpenFileDialog
            {
                Filter = "Imágenes|*.jpg;*.png"
            };

            if (dialogo.ShowDialog() == DialogResult.OK)
            {

                string rutaImagen = dialogo.FileName;
                string extension = Path.GetExtension(rutaImagen).ToLower();

                if (extension == ".jpg" || extension == ".png")
                {

                    Image imagen = Image.FromFile(rutaImagen);
                    picLogo.Image = imagen;
                    picLogo.SizeMode = PictureBoxSizeMode.StretchImage;

                    string nombreImagen = Path.GetFileName(rutaImagen);
                    txtNombreLogo.Text = nombreImagen;
                }
                else
                {
                    MessageBox.Show("Por favor selecciona una imagen de tipo .jpg / .png");
                }


            }

        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {

            // limpiamos los errores anteriores
            errorProvider1.Clear();

            if (string.IsNullOrEmpty(cmbImpresoraRecibos.Text))
            {
                errorProvider1.SetError(cmbImpresoraRecibos, "Ops!, Selecciona una impresora de recibos");
                return;
            }

            if (string.IsNullOrEmpty(cmbRecibosSize.Text))
            {
                errorProvider1.SetError(cmbRecibosSize, "Ops!, Selecciona un tamaño de recibos");
                return;
            }

            if (string.IsNullOrEmpty(cmbImpresoraEtiquetas.Text))
            {
                errorProvider1.SetError(cmbImpresoraEtiquetas, "Ops!, Selecciona una impresora de etiquetas");
                return;
            }

            if (string.IsNullOrEmpty(cmbEtiquetasSize.Text))
            {
                errorProvider1.SetError(cmbEtiquetasSize, "Ops!, Selecciona un tamaño de etiquetas");
                return;
            }

            if (string.IsNullOrEmpty(cmbCortarPapel.Text))
            {
                errorProvider1.SetError(cmbCortarPapel, "El valor seleccionado para el corte de papel es inválido");
                return;
            }

            if (string.IsNullOrEmpty(cmbAbrirCajon.Text))
            {
                errorProvider1.SetError(cmbAbrirCajon, "El valor seleccionado para abrir el cajón es inválido");
                return;
            }

            if (string.IsNullOrEmpty(cmbSaltos.Text))
            {
                errorProvider1.SetError(cmbSaltos, "El valor seleccionado para saltos de línea es inválido");
                return;
            }


            bool cut = cmbCortarPapel.Text.ToUpper() == "SI" ? true : false;
            bool drawer = cmbAbrirCajon.Text.ToUpper() == "SI" ? true : false;
            bool breaklines = cmbSaltos.Text.ToUpper() == "SI" ? true : false;

            ConfigManager.SaveConfig(cmbImpresoraRecibos.Text, cmbRecibosSize.Text, cmbImpresoraEtiquetas.Text, cmbEtiquetasSize.Text, cut, drawer, breaklines);

            SaveLogo(this.picLogo);

            MessageBox.Show("Configuración guardada con éxito", "info", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }



        public void SaveLogo(PictureBox pictureBox)
        {

            try
            {
                if (pictureBox.Image == null)
                {
                    return;
                }

                //comprobar size de la image
                if (pictureBox.Image.Width > 600 || pictureBox.Image.Height > 400)
                {
                    MessageBox.Show("La imagen es demasiado grande, Debe ser 600x400 o menos.", "info", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                using (var ms = new MemoryStream())
                {
                    pictureBox.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                    if (ms.Length > 2 * 1024 * 1024)
                    {
                        //mayor a 2 MB
                        MessageBox.Show("El archivo es demasido grande, debe ser de 2MB  o menos.", "info", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                }


                //config/logo.png
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config");
                Directory.CreateDirectory(path);

                //guardar como png                          
                string fileName = Path.Combine(path, "logo.png");
                pictureBox.Image.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);


            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al intentar guardar el logo." + ex.Message, "info", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }
    }
}
