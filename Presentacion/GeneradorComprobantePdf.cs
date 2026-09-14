using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entidades;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Diagnostics;
using System.IO;

namespace Aura_Beauty
{
    /// <summary>
    /// Clase responsable de generar el comprobante PDF
    /// correspondiente a una venta de Aura Beauty.
    ///
    /// Pertenece a la capa Presentación porque su función
    /// es generar una salida visual para el usuario.
    ///
    /// No realiza consultas SQL ni aplica reglas de negocio.
    /// Recibe una venta ya registrada y utiliza esa información
    /// para construir el documento.
    /// </summary>
    public static class GeneradorComprobantePdf
    {
        /// <summary>
        /// Genera un archivo PDF con los datos de la venta.
        ///
        /// El archivo se guarda dentro de:
        /// Documentos\Aura Beauty\Comprobantes
        ///
        /// Devuelve la ruta completa del archivo generado.
        /// </summary>
        /// <param name="venta">
        /// Venta ya registrada en la base de datos.
        /// </param>
        /// <param name="usuario">
        /// Usuario o vendedor que realizó la operación.
        /// </param>
        /// <param name="cliente">
        /// Cliente asociado a la venta.
        ///
        /// Puede ser null cuando la operación corresponde
        /// a Consumidor Final.
        /// </param>
        /// <returns>
        /// Ruta completa donde quedó guardado el PDF.
        /// </returns>
        public static string Generar(
            VentaCabecera venta,
            Usuario usuario,
            Cliente cliente)
        {
            // =====================================================
            // VALIDACIONES BÁSICAS
            // =====================================================

            if (venta == null)
            {
                throw new ArgumentNullException(
                    nameof(venta),
                    "No se recibió la información de la venta."
                );
            }

            if (usuario == null)
            {
                throw new ArgumentNullException(
                    nameof(usuario),
                    "No se recibió el usuario que realizó la venta."
                );
            }

            if (venta.Detalles == null ||
                venta.Detalles.Count == 0)
            {
                throw new Exception(
                    "La venta no contiene productos para generar el comprobante."
                );
            }


            // =====================================================
            // CREAR CARPETA DE COMPROBANTES
            // =====================================================

            // Environment.GetFolderPath obtiene una carpeta
            // conocida de Windows.
            //
            // MyDocuments corresponde a "Documentos"
            // del usuario actual.
            string carpetaDocumentos =
                Environment.GetFolderPath(
                    Environment.SpecialFolder.MyDocuments
                );


            string carpetaAuraBeauty =
                Path.Combine(
                    carpetaDocumentos,
                    "Aura Beauty",
                    "Comprobantes"
                );


            // Directory.CreateDirectory crea la carpeta si no existe.
            //
            // Si ya existe, no produce error.
            Directory.CreateDirectory(
                carpetaAuraBeauty
            );


            // =====================================================
            // NOMBRE DEL ARCHIVO
            // =====================================================

            // Eliminamos caracteres que podrían generar problemas
            // al utilizar el número de comprobante como nombre.
            string numeroSeguro =
                venta.NroFactura
                    .Replace("/", "-")
                    .Replace("\\", "-")
                    .Replace(":", "-");


            string nombreArchivo =
                "Comprobante_"
                + numeroSeguro
                + ".pdf";


            string rutaCompleta =
                Path.Combine(
                    carpetaAuraBeauty,
                    nombreArchivo
                );


            // =====================================================
            // CREAR DOCUMENTO PDF
            // =====================================================

            PdfDocument documento =
                new PdfDocument();


            documento.Info.Title =
                "Comprobante de venta - Aura Beauty";


            documento.Info.Author =
                "Aura Beauty";


            documento.Info.Subject =
                "Comprobante de venta";


            // Agregamos una página al documento.
            PdfPage pagina =
                documento.AddPage();


            // Formato A4.
            pagina.Size =
                PdfSharp.PageSize.A4;


            // XGraphics funciona como una especie de "lienzo".
            //
            // Sobre este objeto escribimos textos,
            // líneas y figuras.
            XGraphics gfx =
                XGraphics.FromPdfPage(
                    pagina
                );


            // =====================================================
            // FUENTES
            // =====================================================

            XFont fuenteTitulo =
                new XFont(
                    "Arial",
                    20,
                    XFontStyleEx.Bold
                );


            XFont fuenteSubtitulo =
                new XFont(
                    "Arial",
                    12,
                    XFontStyleEx.Bold
                );


            XFont fuenteNormal =
                new XFont(
                    "Arial",
                    10,
                    XFontStyleEx.Regular
                );


            XFont fuenteNegrita =
                new XFont(
                    "Arial",
                    10,
                    XFontStyleEx.Bold
                );


            XFont fuenteTotal =
                new XFont(
                    "Arial",
                    14,
                    XFontStyleEx.Bold
                );


            // =====================================================
            // COLORES
            // =====================================================

            XColor rosaAura =
                XColor.FromArgb(
                    201,
                    143,
                    149
                );


            XColor textoOscuro =
                XColor.FromArgb(
                    94,
                    74,
                    74
                );


            XBrush pincelRosa =
                new XSolidBrush(
                    rosaAura
                );


            XBrush pincelTexto =
                new XSolidBrush(
                    textoOscuro
                );


            XBrush pincelBlanco =
                XBrushes.White;


            // =====================================================
            // MEDIDAS GENERALES
            // =====================================================

            double margenIzquierdo =
                45;


            double margenDerecho =
                550;


            double anchoContenido =
                margenDerecho
                -
                margenIzquierdo;


            double y =
                40;


            // =====================================================
            // ENCABEZADO ROSA
            // =====================================================

            gfx.DrawRectangle(
                pincelRosa,
                margenIzquierdo,
                y,
                anchoContenido,
                75
            );


            gfx.DrawString(
                "AURA BEAUTY",
                fuenteTitulo,
                pincelBlanco,
                new XRect(
                    margenIzquierdo + 20,
                    y + 15,
                    anchoContenido - 40,
                    30
                ),
                XStringFormats.TopLeft
            );


            gfx.DrawString(
                "Comprobante de venta",
                fuenteSubtitulo,
                pincelBlanco,
                new XRect(
                    margenIzquierdo + 20,
                    y + 47,
                    anchoContenido - 40,
                    20
                ),
                XStringFormats.TopLeft
            );


            y +=
                95;


            // =====================================================
            // DATOS GENERALES
            // =====================================================

            DibujarDato(
                gfx,
                "Venta N.º:",
                venta.IdVentaCabecera.ToString(),
                margenIzquierdo,
                ref y,
                fuenteNegrita,
                fuenteNormal,
                pincelTexto
            );


            DibujarDato(
                gfx,
                "Comprobante:",
                venta.NroFactura,
                margenIzquierdo,
                ref y,
                fuenteNegrita,
                fuenteNormal,
                pincelTexto
            );


            DibujarDato(
                gfx,
                "Tipo:",
                venta.TipoFactura,
                margenIzquierdo,
                ref y,
                fuenteNegrita,
                fuenteNormal,
                pincelTexto
            );


            string fechaTexto =
                venta.FechaVenta.HasValue
                    ? venta.FechaVenta.Value.ToString(
                        "dd/MM/yyyy HH:mm"
                    )
                    : DateTime.Now.ToString(
                        "dd/MM/yyyy HH:mm"
                    );


            DibujarDato(
                gfx,
                "Fecha:",
                fechaTexto,
                margenIzquierdo,
                ref y,
                fuenteNegrita,
                fuenteNormal,
                pincelTexto
            );


            string nombreVendedor =
                usuario.Nombre
                + " "
                + usuario.Apellido;


            DibujarDato(
                gfx,
                "Vendedor:",
                nombreVendedor,
                margenIzquierdo,
                ref y,
                fuenteNegrita,
                fuenteNormal,
                pincelTexto
            );


            // =====================================================
            // CLIENTE
            // =====================================================

            string nombreCliente;


            if (cliente == null)
            {
                nombreCliente =
                    "Consumidor Final";
            }
            else
            {
                nombreCliente =
                    cliente.Apellido
                    + ", "
                    + cliente.Nombre
                    + " - DNI "
                    + cliente.Dni;
            }


            DibujarDato(
                gfx,
                "Cliente:",
                nombreCliente,
                margenIzquierdo,
                ref y,
                fuenteNegrita,
                fuenteNormal,
                pincelTexto
            );


            y +=
                20;


            // =====================================================
            // TABLA - ENCABEZADO
            // =====================================================

            double xProducto =
                margenIzquierdo;

            double xCantidad =
                290;

            double xPrecio =
                355;

            double xSubtotal =
                455;


            gfx.DrawRectangle(
                pincelRosa,
                margenIzquierdo,
                y,
                anchoContenido,
                28
            );


            gfx.DrawString(
                "Producto",
                fuenteNegrita,
                pincelBlanco,
                xProducto + 8,
                y + 18
            );


            gfx.DrawString(
                "Cant.",
                fuenteNegrita,
                pincelBlanco,
                xCantidad,
                y + 18
            );


            gfx.DrawString(
                "Precio",
                fuenteNegrita,
                pincelBlanco,
                xPrecio,
                y + 18
            );


            gfx.DrawString(
                "Subtotal",
                fuenteNegrita,
                pincelBlanco,
                xSubtotal,
                y + 18
            );


            y +=
                35;


            // =====================================================
            // TABLA - PRODUCTOS
            // =====================================================

            foreach (
                VentaDetalle detalle
                in venta.Detalles
            )
            {
                string nombreProducto =
                    detalle.oProducto != null
                        ? detalle.oProducto.Nombre
                        : "Producto";


                gfx.DrawString(
                    nombreProducto,
                    fuenteNormal,
                    pincelTexto,
                    xProducto + 8,
                    y
                );


                gfx.DrawString(
                    detalle.Cantidad.ToString(),
                    fuenteNormal,
                    pincelTexto,
                    xCantidad,
                    y
                );


                gfx.DrawString(
                    "$ "
                    + detalle.PrecioUnitario.ToString(
                        "N2"
                    ),
                    fuenteNormal,
                    pincelTexto,
                    xPrecio,
                    y
                );


                gfx.DrawString(
                    "$ "
                    + detalle.Subtotal.ToString(
                        "N2"
                    ),
                    fuenteNormal,
                    pincelTexto,
                    xSubtotal,
                    y
                );


                y +=
                    24;


                // Línea divisoria entre productos.
                gfx.DrawLine(
                    new XPen(
                        XColors.LightGray,
                        0.5
                    ),
                    margenIzquierdo,
                    y - 10,
                    margenDerecho,
                    y - 10
                );
            }


            // =====================================================
            // TOTAL
            // =====================================================

            y +=
                20;


            gfx.DrawLine(
                new XPen(
                    rosaAura,
                    2
                ),
                350,
                y,
                margenDerecho,
                y
            );


            y +=
                15;


            gfx.DrawString(
                "TOTAL:",
                fuenteTotal,
                pincelTexto,
                365,
                y
            );


            gfx.DrawString(
                "$ "
                + venta.Total.ToString(
                    "N2"
                ),
                fuenteTotal,
                pincelTexto,
                455,
                y
            );


            // =====================================================
            // PIE DEL COMPROBANTE
            // =====================================================

            y +=
                70;


            gfx.DrawString(
                "Gracias por elegir Aura Beauty",
                fuenteSubtitulo,
                pincelTexto,
                new XRect(
                    margenIzquierdo,
                    y,
                    anchoContenido,
                    25
                ),
                XStringFormats.Center
            );


            y +=
                25;


            gfx.DrawString(
                "Comprobante generado por el sistema Aura Beauty.",
                fuenteNormal,
                XBrushes.Gray,
                new XRect(
                    margenIzquierdo,
                    y,
                    anchoContenido,
                    20
                ),
                XStringFormats.Center
            );


            // =====================================================
            // GUARDAR DOCUMENTO
            // =====================================================

            documento.Save(
                rutaCompleta
            );


            documento.Close();


            return rutaCompleta;
        }


        // =========================================================
        // MÉTODO AUXILIAR PARA MOSTRAR DATOS
        // =========================================================

        /// <summary>
        /// Dibuja una etiqueta y su valor en una misma línea.
        ///
        /// Ejemplo:
        /// Cliente: Consumidor Final
        ///
        /// ref y permite que el método actualice automáticamente
        /// la posición vertical para la siguiente línea.
        /// </summary>
        private static void DibujarDato(
            XGraphics gfx,
            string etiqueta,
            string valor,
            double x,
            ref double y,
            XFont fuenteEtiqueta,
            XFont fuenteValor,
            XBrush pincel)
        {
            gfx.DrawString(
                etiqueta,
                fuenteEtiqueta,
                pincel,
                x,
                y
            );


            gfx.DrawString(
                valor ?? "",
                fuenteValor,
                pincel,
                x + 110,
                y
            );


            y +=
                22;
        }


        // =========================================================
        // ABRIR PDF
        // =========================================================

        /// <summary>
        /// Abre el comprobante utilizando el visor PDF
        /// predeterminado de Windows.
        /// </summary>
        public static void Abrir(
            string rutaArchivo)
        {
            if (
                string.IsNullOrWhiteSpace(
                    rutaArchivo
                )
            )
            {
                return;
            }


            if (
                !File.Exists(
                    rutaArchivo
                )
            )
            {
                throw new FileNotFoundException(
                    "No se encontró el comprobante PDF.",
                    rutaArchivo
                );
            }


            Process.Start(
                rutaArchivo
            );
        }
    }
}
