using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OnlineReceiptPrintingApp
{
    public class clsCommon
    {

        // variables de control al imprimir
        public static int maxCar_Centrar = 47;
        public static int maxCar_Guiones_y_Astericos = 47;
        public static int maxCarTotales = 50;
        public static int maxCarTextoIzquierda = 80;
        public static int maxCarTextoDerecha = 40;
        public static int articulo_rigth_space = 19;
        public static int productMax_lenght = 27;



        // método para determinar si una cadena tiene acentos o la letra ñ/Ñ
        public static bool TraeAcentos(string texto)
        {
            //si el texto es vacío devolvemos false
            if (string.IsNullOrEmpty(texto)) return false;

            // expresión regular / caracteres a validar
            string patron = "[ñÑáéíóúÁÉÍÓÚ]";

            // crear instancia de Regex 
            Regex regex = new Regex(patron);

            //ejecutamos validación
            MatchCollection coincidencias = regex.Matches(texto);

            //si coincidencias mayor a 0, devolvemos el count como valor boolean
            return coincidencias.Count > 0;
        }

    }
}