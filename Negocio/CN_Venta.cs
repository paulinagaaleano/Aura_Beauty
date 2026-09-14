using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datos;
using Entidades;


namespace Negocio
{
    /// <summary>
    /// Clase de la capa de Negocio encargada de gestionar
    /// las reglas necesarias para registrar una venta.
    ///
    /// Su función es validar la información antes de enviarla
    /// a la capa de Datos.
    ///
    /// No contiene consultas SQL ni controles de Windows Forms.
    /// </summary>
    public class CN_Venta
    {
        /// <summary>
        /// Objeto de la capa de Datos utilizado para
        /// registrar definitivamente una venta.
        /// </summary>
        private readonly CD_Venta cdVenta =
            new CD_Venta();


        // =========================================================
        // REGISTRAR VENTA
        // =========================================================

        /// <summary>
        /// Valida y registra una venta completa.
        ///
        /// Devuelve el ID generado para la venta.
        /// </summary>
        public int Registrar(VentaCabecera venta)
        {
            // Primero comprobamos todas las reglas.
            ValidarVenta(venta);


            // Recalculamos subtotales y total en Negocio.
            //
            // De esta manera no confiamos únicamente
            // en los cálculos realizados por el formulario.
            CalcularTotales(venta);


            // Finalmente enviamos la venta validada
            // hacia la capa de Datos.
            return cdVenta.Registrar(venta);
        }


        // =========================================================
        // VALIDAR VENTA
        // =========================================================

        /// <summary>
        /// Comprueba que la venta tenga todos los datos
        /// necesarios antes de guardarla.
        /// </summary>
        private void ValidarVenta(VentaCabecera venta)
        {
            if (venta == null)
            {
                throw new Exception(
                    "Los datos de la venta no son válidos."
                );
            }


            // -----------------------------------------------------
            // USUARIO / VENDEDOR
            // -----------------------------------------------------

            // Toda venta debe quedar asociada al usuario
            // que está utilizando el sistema.
            if (venta.IdUsuario <= 0)
            {
                throw new Exception(
                    "No se pudo identificar al usuario que realiza la venta."
                );
            }


            // -----------------------------------------------------
            // CLIENTE
            // -----------------------------------------------------

            // IdCliente puede ser null.
            //
            // Esto NO es un error porque null representará
            // una venta realizada a Consumidor Final.
            //
            // Si tiene un valor, debe ser un ID válido.
            if (
                venta.IdCliente.HasValue
                &&
                venta.IdCliente.Value <= 0
            )
            {
                throw new Exception(
                    "El cliente seleccionado no es válido."
                );
            }


            // -----------------------------------------------------
            // TIPO DE FACTURA
            // -----------------------------------------------------

            if (
                string.IsNullOrWhiteSpace(
                    venta.TipoFactura
                )
            )
            {
                throw new Exception(
                    "Debe indicar el tipo de factura."
                );
            }


            if (
                venta.TipoFactura.Trim().Length > 50
            )
            {
                throw new Exception(
                    "El tipo de factura no puede superar los 50 caracteres."
                );
            }


            // -----------------------------------------------------
            // NÚMERO DE FACTURA
            // -----------------------------------------------------

            if (
                string.IsNullOrWhiteSpace(
                    venta.NroFactura
                )
            )
            {
                throw new Exception(
                    "Debe existir un número de factura."
                );
            }


            if (
                venta.NroFactura.Trim().Length > 50
            )
            {
                throw new Exception(
                    "El número de factura no puede superar los 50 caracteres."
                );
            }


            // -----------------------------------------------------
            // DETALLES / CARRITO
            // -----------------------------------------------------

            if (
                venta.Detalles == null
                ||
                venta.Detalles.Count == 0
            )
            {
                throw new Exception(
                    "La venta debe contener al menos un producto."
                );
            }


            // -----------------------------------------------------
            // PRODUCTOS DUPLICADOS
            // -----------------------------------------------------

            // En el carrito no queremos dos líneas diferentes
            // correspondientes al mismo producto.
            //
            // Si el usuario vuelve a agregar un producto,
            // frmVentas deberá aumentar su cantidad.
            bool existenDuplicados =
                venta.Detalles
                    .GroupBy(
                        d => d.IdProducto
                    )
                    .Any(
                        grupo =>
                            grupo.Count() > 1
                    );


            if (existenDuplicados)
            {
                throw new Exception(
                    "Un mismo producto aparece más de una vez en la venta."
                );
            }


            // -----------------------------------------------------
            // VALIDAR CADA DETALLE
            // -----------------------------------------------------

            foreach (
                VentaDetalle detalle
                in venta.Detalles
            )
            {
                ValidarDetalle(detalle);
            }
        }


        // =========================================================
        // VALIDAR DETALLE
        // =========================================================

        /// <summary>
        /// Comprueba las reglas correspondientes
        /// a cada producto incluido en la venta.
        /// </summary>
        private void ValidarDetalle(
            VentaDetalle detalle
        )
        {
            if (detalle == null)
            {
                throw new Exception(
                    "Uno de los detalles de la venta no es válido."
                );
            }


            if (detalle.IdProducto <= 0)
            {
                throw new Exception(
                    "Uno de los productos seleccionados no es válido."
                );
            }


            // No se puede vender cero unidades
            // ni una cantidad negativa.
            if (detalle.Cantidad <= 0)
            {
                throw new Exception(
                    "La cantidad de cada producto debe ser mayor que cero."
                );
            }


            // El precio debe ser positivo.
            if (detalle.PrecioUnitario <= 0)
            {
                throw new Exception(
                    "El precio de uno de los productos no es válido."
                );
            }
        }


        // =========================================================
        // CALCULAR TOTALES
        // =========================================================

        /// <summary>
        /// Calcula nuevamente el subtotal de cada producto
        /// y el total general de la venta.
        ///
        /// Esto se realiza en Negocio para que el importe
        /// definitivo no dependa únicamente del formulario.
        /// </summary>
        private void CalcularTotales(
            VentaCabecera venta
        )
        {
            decimal total =
                0;


            foreach (
                VentaDetalle detalle
                in venta.Detalles
            )
            {
                // Subtotal =
                // cantidad × precio unitario.
                detalle.Subtotal =
                    detalle.Cantidad
                    *
                    detalle.PrecioUnitario;


                total +=
                    detalle.Subtotal;
            }


            venta.Total =
                total;


            // Eliminamos espacios innecesarios.
            venta.TipoFactura =
                venta.TipoFactura.Trim();


            venta.NroFactura =
                venta.NroFactura.Trim();
        }
    }
}
