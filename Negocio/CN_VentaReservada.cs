
using System;
using System.Collections.Generic;
using System.Linq;
using Datos;
using Entidades;

namespace Negocio
{
    /// <summary>
    /// Resultado de una confirmación de venta reservada.
    ///
    /// Permite informar a Presentación si una operación
    /// fue registrada por primera vez o si ya estaba
    /// confirmada previamente.
    /// </summary>
    public class ResultadoVentaReservada
    {
        /// <summary>
        /// Identificador definitivo de VentaCabecera.
        /// </summary>
        public int IdVentaCabecera { get; set; }

        /// <summary>
        /// Indica si la operación ya existía.
        ///
        /// Es útil para impedir que un reintento
        /// genere dos comprobantes comerciales
        /// o que se informe una venta duplicada.
        /// </summary>
        public bool YaRegistrada { get; set; }

        /// <summary>
        /// Importe calculado de la venta.
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// Mensaje para mostrar en la interfaz.
        /// </summary>
        public string Mensaje { get; set; }
    }

    /// <summary>
    /// CAPA NEGOCIO - VENTAS CON RESERVAS.
    ///
    /// Responsabilidades:
    /// - Validar los datos de una venta.
    /// - Impedir cantidades inválidas.
    /// - Detectar productos duplicados.
    /// - Calcular los subtotales.
    /// - Calcular el total de la operación.
    /// - Enviar los datos validados a la capa Datos.
    ///
    /// No ejecuta consultas SQL.
    /// No descuenta directamente el stock.
    ///
    /// La confirmación definitiva se realiza
    /// mediante CD_VentaReservada.
    /// </summary>
    public class CN_VentaReservada
    {
        private readonly CD_VentaReservada datosVenta =
            new CD_VentaReservada();

        /// <summary>
        /// Confirma una venta que posee reservas activas.
        ///
        /// El carrito debe haber reservado todos sus
        /// productos antes de llamar a este método.
        ///
        /// El procedimiento almacenado volverá a verificar
        /// todas las reservas antes de registrar la venta.
        /// </summary>
        public ResultadoVentaReservada Confirmar(
            Guid idOperacion,
            VentaCabecera venta)
        {
            // ============================================
            // 1. IDENTIFICAR LA OPERACIÓN
            // ============================================

            if (idOperacion == Guid.Empty)
            {
                throw new ArgumentException(
                    "No se pudo identificar el carrito " +
                    "de la operación de venta.");
            }

            // ============================================
            // 2. VALIDAR DATOS GENERALES
            // ============================================

            ValidarVenta(venta);

            // ============================================
            // 3. CALCULAR TOTALES
            // ============================================

            CalcularTotales(venta);

            // ============================================
            // 4. CONFIRMAR EN SQL SERVER
            // ============================================

            /*
             * No llamamos a CN_Venta.Registrar(),
             * porque ese método utiliza el proceso
             * anterior y no comprueba reservas.
             *
             * La nueva operación se ejecuta mediante
             * el procedimiento transaccional
             * SP_Venta_ConfirmarReservas.
             */

            bool yaRegistrada;

            int idVenta = datosVenta.Confirmar(
                idOperacion,
                venta,
                out yaRegistrada);

            // ============================================
            // 5. ACTUALIZAR EL OBJETO EN MEMORIA
            // ============================================

            venta.IdVentaCabecera = idVenta;

            /*
             * La fecha real se registra en SQL Server.
             * Esta fecha local se utiliza únicamente
             * para representar la operación en memoria.
             */

            venta.FechaVenta = DateTime.Now;

            // ============================================
            // 6. CONSTRUIR RESULTADO
            // ============================================

            ResultadoVentaReservada resultado =
                new ResultadoVentaReservada
                {
                    IdVentaCabecera = idVenta,

                    YaRegistrada = yaRegistrada,

                    Total = venta.Total,

                    Mensaje = yaRegistrada
                        ? "Esta operación ya había sido " +
                          "confirmada. No se registró " +
                          "una segunda venta."
                        : "La venta fue registrada " +
                          "correctamente y el stock " +
                          "fue actualizado."
                };

            return resultado;
        }

        // ================================================
        // VALIDACIÓN GENERAL
        // ================================================

        /// <summary>
        /// Comprueba los requisitos de la cabecera
        /// y todos los productos de la operación.
        ///
        /// Las mismas reglas esenciales serán
        /// verificadas nuevamente por SQL Server
        /// durante la confirmación.
        /// </summary>
        private void ValidarVenta(
            VentaCabecera venta)
        {
            if (venta == null)
            {
                throw new ArgumentNullException(
                    "venta",
                    "La venta no puede ser nula.");
            }

            // ============================================
            // VENDEDOR
            // ============================================

            if (venta.IdUsuario <= 0)
            {
                throw new ArgumentException(
                    "No se pudo identificar al vendedor.");
            }

            // ============================================
            // CLIENTE
            // ============================================

            /*
             * IdCliente puede ser null.
             *
             * Esto corresponde a una operación
             * realizada a Consumidor Final.
             */

            if (venta.IdCliente.HasValue &&
                venta.IdCliente.Value <= 0)
            {
                throw new ArgumentException(
                    "El cliente seleccionado no es válido.");
            }

            // ============================================
            // TIPO DE FACTURA
            // ============================================

            if (string.IsNullOrWhiteSpace(
                venta.TipoFactura))
            {
                throw new ArgumentException(
                    "Debe indicar el tipo de factura.");
            }

            if (venta.TipoFactura.Trim().Length > 50)
            {
                throw new ArgumentException(
                    "El tipo de factura supera " +
                    "los 50 caracteres.");
            }

            // ============================================
            // NÚMERO DE COMPROBANTE
            // ============================================

            if (string.IsNullOrWhiteSpace(
                venta.NroFactura))
            {
                throw new ArgumentException(
                    "Debe indicar el número de comprobante.");
            }

            if (venta.NroFactura.Trim().Length > 50)
            {
                throw new ArgumentException(
                    "El número de comprobante supera " +
                    "los 50 caracteres.");
            }

            // ============================================
            // PRODUCTOS DEL CARRITO
            // ============================================

            if (venta.Detalles == null ||
                venta.Detalles.Count == 0)
            {
                throw new ArgumentException(
                    "El carrito no contiene productos.");
            }

            // ============================================
            // PRODUCTOS DUPLICADOS
            // ============================================

            /*
             * Cada producto debe aparecer solamente
             * una vez dentro del carrito.
             *
             * Para aumentar cantidades se modifica
             * la línea existente.
             */

            bool duplicados = venta.Detalles
                .Where(d => d != null)
                .GroupBy(d => d.IdProducto)
                .Any(grupo => grupo.Count() > 1);

            if (duplicados)
            {
                throw new ArgumentException(
                    "El carrito contiene un producto " +
                    "repetido en distintas líneas.");
            }

            // ============================================
            // VALIDAR CADA DETALLE
            // ============================================

            foreach (VentaDetalle detalle in
                venta.Detalles)
            {
                ValidarDetalle(detalle);
            }
        }

        // ================================================
        // VALIDACIÓN DE DETALLES
        // ================================================

        /// <summary>
        /// Comprueba que el producto, la cantidad
        /// y el precio unitario sean válidos.
        /// </summary>
        private void ValidarDetalle(
            VentaDetalle detalle)
        {
            if (detalle == null)
            {
                throw new ArgumentException(
                    "Existe un detalle de venta vacío.");
            }

            if (detalle.IdProducto <= 0)
            {
                throw new ArgumentException(
                    "El producto seleccionado no es válido.");
            }

            if (detalle.Cantidad <= 0)
            {
                throw new ArgumentException(
                    "La cantidad debe ser mayor que cero.");
            }

            if (detalle.PrecioUnitario <= 0m)
            {
                throw new ArgumentException(
                    "El precio unitario debe ser positivo.");
            }

            /*
             * El tipo de datos definido en SQL Server
             * utiliza DECIMAL(10,2).
             *
             * No admitimos precios con más de dos
             * decimales ni valores fuera de rango.
             */

            if (detalle.PrecioUnitario >
                99999999.99m)
            {
                throw new ArgumentException(
                    "El precio unitario supera " +
                    "el importe permitido.");
            }

            if (decimal.Round(
                detalle.PrecioUnitario, 2) !=
                detalle.PrecioUnitario)
            {
                throw new ArgumentException(
                    "El precio unitario no puede tener " +
                    "más de dos decimales.");
            }
        }

        // ================================================
        // CÁLCULO DE IMPORTES
        // ================================================

        /// <summary>
        /// Recalcula los subtotales y el total general.
        ///
        /// Fórmula del subtotal:
        ///
        /// Cantidad × Precio unitario.
        ///
        /// Fórmula del total:
        ///
        /// Suma de todos los subtotales.
        ///
        /// No utiliza importes calculados
        /// previamente por el formulario.
        /// </summary>
        private void CalcularTotales(
            VentaCabecera venta)
        {
            decimal total = 0m;

            foreach (VentaDetalle detalle in
                venta.Detalles)
            {
                decimal subtotal = checked(
                    detalle.Cantidad *
                    detalle.PrecioUnitario);

                if (subtotal > 99999999.99m)
                {
                    throw new ArgumentException(
                        "El subtotal de un producto " +
                        "supera el máximo permitido.");
                }

                detalle.Subtotal = subtotal;

                total = checked(total + subtotal);
            }

            if (total <= 0m ||
                total > 99999999.99m)
            {
                throw new ArgumentException(
                    "El total de la venta no es válido.");
            }

            venta.Total = total;

            /*
             * Conservamos los textos sin espacios
             * adicionales para evitar diferencias
             * innecesarias con SQL Server.
             */

            venta.TipoFactura =
                venta.TipoFactura.Trim();

            venta.NroFactura =
                venta.NroFactura.Trim();
        }
    }
}
