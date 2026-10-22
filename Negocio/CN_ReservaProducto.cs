
using System;
using Datos;
using Entidades;

namespace Negocio
{
    /// <summary>
    /// Resultado de una operación de reserva.
    /// </summary>
    public class ResultadoReservaProducto
    {
        public ReservaProducto Reserva { get; set; }

        public int StockFisico { get; set; }

        public int ReservadoPorOtros { get; set; }

        public int DisponibleRestante { get; set; }

        public string Mensaje { get; set; }
    }

    /// <summary>
    /// CAPA NEGOCIO - RESERVAS DE AURA BEAUTY.
    ///
    /// Permite:
    /// - Crear identificadores de operación.
    /// - Reservar cantidades.
    /// - Reducir cantidades.
    /// - Liberar un producto.
    /// - Liberar el carrito completo.
    /// - Consultar el tiempo restante.
    ///
    /// No descuenta el stock físico.
    /// No registra ventas definitivas.
    /// No ejecuta SQL directamente.
    /// </summary>
    public class CN_ReservaProducto
    {
        private readonly CD_ReservaProducto datosReserva =
            new CD_ReservaProducto();

        public const int MinutosReserva = 10;

        /// <summary>
        /// Genera un identificador único para un
        /// nuevo carrito de venta.
        ///
        /// Debe ejecutarse una sola vez al iniciar
        /// la operación, no al agregar cada producto.
        /// </summary>
        public Guid CrearNuevaOperacion()
        {
            return Guid.NewGuid();
        }

        /// <summary>
        /// Reserva la cantidad TOTAL solicitada.
        ///
        /// Ejemplo:
        /// Si existen 3 unidades reservadas y
        /// se solicita cantidad 2, se reducen a 2.
        ///
        /// Si se solicita cantidad 4, primero se
        /// comprueba la disponibilidad.
        /// </summary>
        public ResultadoReservaProducto Reservar(
            Guid idOperacion,
            int idProducto,
            int idUsuario,
            int cantidad)
        {
            ValidarOperacion(idOperacion, idUsuario);

            if (idProducto <= 0)
            {
                throw new ArgumentException(
                    "Debe seleccionar un producto válido.");
            }

            if (cantidad <= 0)
            {
                throw new ArgumentException(
                    "La cantidad debe ser mayor que cero.");
            }

            int stockFisico;
            int reservadoPorOtros;
            int disponibleRestante;

            ReservaProducto reserva =
                datosReserva.Reservar(
                    idOperacion,
                    idProducto,
                    idUsuario,
                    cantidad,
                    out stockFisico,
                    out reservadoPorOtros,
                    out disponibleRestante);

            return new ResultadoReservaProducto
            {
                Reserva = reserva,

                StockFisico = stockFisico,

                ReservadoPorOtros = reservadoPorOtros,

                DisponibleRestante =
                    disponibleRestante,

                Mensaje =
                    "Reserva realizada correctamente. " +
                    "Unidades reservadas: " +
                    reserva.Cantidad +
                    ". Disponibles para otros vendedores: " +
                    disponibleRestante + "."
            };
        }

        /// <summary>
        /// Libera un producto específico.
        ///
        /// Se utilizará cuando el vendedor presione
        /// "Quitar" en el carrito.
        ///
        /// No devuelve unidades al stock físico,
        /// porque nunca fueron descontadas.
        /// Simplemente deja de reservarlas.
        /// </summary>
        public int CancelarProducto(
            Guid idOperacion,
            int idUsuario,
            int idProducto)
        {
            ValidarOperacion(idOperacion, idUsuario);

            if (idProducto <= 0)
            {
                throw new ArgumentException(
                    "El producto no es válido.");
            }

            return datosReserva.Cancelar(
                idOperacion,
                idUsuario,
                idProducto);
        }

        /// <summary>
        /// Libera todas las reservas activas
        /// pertenecientes a un carrito.
        ///
        /// Se utilizará cuando el vendedor
        /// cancele la operación.
        ///
        /// Devuelve el total de unidades vigentes
        /// que dejaron de estar comprometidas.
        /// </summary>
        public int CancelarOperacion(
            Guid idOperacion,
            int idUsuario)
        {
            ValidarOperacion(idOperacion, idUsuario);

            return datosReserva.Cancelar(
                idOperacion,
                idUsuario,
                null);
        }

        /// <summary>
        /// Calcula cuánto falta para que venza una
        /// reserva, usando la fecha UTC devuelta
        /// por SQL Server.
        ///
        /// Es información orientativa para la pantalla.
        /// La verificación definitiva corresponde
        /// al servidor SQL.
        /// </summary>
        public TimeSpan ObtenerTiempoRestante(
            ReservaProducto reserva)
        {
            if (reserva == null)
            {
                throw new ArgumentNullException(
                    "reserva",
                    "La reserva no puede ser nula.");
            }

            DateTime vencimiento =
                reserva.FechaVencimientoUtc;

            if (vencimiento.Kind ==
                DateTimeKind.Unspecified)
            {
                vencimiento = DateTime.SpecifyKind(
                    vencimiento,
                    DateTimeKind.Utc);
            }

            TimeSpan restante =
                vencimiento.ToUniversalTime() -
                DateTime.UtcNow;

            return restante > TimeSpan.Zero
                ? restante
                : TimeSpan.Zero;
        }

        /// <summary>
        /// Validaciones compartidas por los métodos
        /// de reserva y cancelación.
        /// </summary>
        private static void ValidarOperacion(
            Guid idOperacion,
            int idUsuario)
        {
            if (idOperacion == Guid.Empty)
            {
                throw new ArgumentException(
                    "No se pudo identificar " +
                    "la operación de venta.");
            }

            if (idUsuario <= 0)
            {
                throw new ArgumentException(
                    "No se pudo identificar al vendedor.");
            }
        }
    }
}
