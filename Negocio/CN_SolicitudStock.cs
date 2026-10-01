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
    /// Capa de Negocio para las solicitudes de stock.
    ///
    /// Contiene las reglas que deben cumplirse
    /// antes de registrar una solicitud.
    /// </summary>
    public class CN_SolicitudStock
    {
        /// <summary>
        /// Objeto de la capa de Datos que realizará
        /// las operaciones contra SQL Server.
        /// </summary>
        private readonly CD_SolicitudStock cdSolicitudStock =
            new CD_SolicitudStock();

        private readonly CD_Usuario cdUsuario =
            new CD_Usuario();

        /// <summary>
        /// Registra una solicitud de modificación de stock
        /// después de verificar las reglas de negocio.
        /// </summary>
        public bool Registrar(SolicitudStock solicitud)
        {
            /*
             * La solicitud debe existir.
             */
            if (solicitud == null)
            {
                throw new ArgumentException(
                    "La solicitud no puede estar vacía."
                );
            }


            /*
             * Debe haberse seleccionado
             * un producto válido.
             */
            if (solicitud.IdProducto <= 0)
            {
                throw new ArgumentException(
                    "Debe seleccionar un producto."
                );
            }


            /*
             * La solicitud debe pertenecer
             * a un usuario válido.
             */
            if (solicitud.IdUsuarioSolicitante <= 0)
            {
                throw new ArgumentException(
                    "No se pudo identificar al usuario solicitante."
                );
            }


            /*
             * La cantidad debe ser mayor que cero.
             */
            if (solicitud.Cantidad <= 0)
            {
                throw new ArgumentException(
                    "La cantidad debe ser mayor que cero."
                );
            }


            /*
             * Solamente admitimos los dos tipos
             * de movimiento definidos por el sistema.
             */
            string tipo =
                solicitud.TipoMovimiento == null
                ? ""
                : solicitud.TipoMovimiento
                    .Trim()
                    .ToUpper();


            if (
                tipo != "INGRESO"
                &&
                tipo != "EGRESO"
            )
            {
                throw new ArgumentException(
                    "El tipo de movimiento debe ser INGRESO o EGRESO."
                );
            }


            /*
             * El motivo es obligatorio porque justamente
             * buscamos evitar modificaciones de stock
             * sin una justificación.
             */
            if (string.IsNullOrWhiteSpace(
                    solicitud.Motivo))
            {
                throw new ArgumentException(
                    "Debe indicar el motivo de la solicitud."
                );
            }


            /*
             * Evitamos motivos excesivamente largos
             * porque la columna de SQL admite
             * hasta 250 caracteres.
             */
            if (solicitud.Motivo.Trim().Length > 250)
            {
                throw new ArgumentException(
                    "El motivo no puede superar los 250 caracteres."
                );
            }


            /*
             * Normalizamos los datos antes
             * de enviarlos a Datos.
             */
            solicitud.TipoMovimiento = tipo;

            solicitud.Motivo =
                solicitud.Motivo.Trim();


            /*
             * Si todas las reglas se cumplen,
             * la capa de Datos registra la solicitud.
             *
             * Todavía NO se modifica Producto.stock.
             */
            return cdSolicitudStock.Registrar(
                solicitud
            );
        }


        /// <summary>
        /// Devuelve las solicitudes que todavía
        /// necesitan ser resueltas por un Administrador.
        /// </summary>
        public List<SolicitudStock> ListarPendientes()
        {
            return cdSolicitudStock.ListarPendientes();
        }


        /// <summary>
        /// Aprueba una solicitud de modificación de stock.
        ///
        /// La capa de Negocio verifica que los identificadores
        /// recibidos sean válidos y luego delega la operación
        /// a la capa de Datos.
        /// 
        /// Solamente un usuario activo con rol Administrador
        /// puede autorizar la modificación.
        /// </summary>
        public bool Aprobar(
            int idSolicitud,
            int idUsuarioAutorizador
        )
        {
            if (idSolicitud <= 0)
            {
                throw new ArgumentException(
                    "Debe seleccionar una solicitud válida."
                );
            }

            if (idUsuarioAutorizador <= 0)
            {
                throw new ArgumentException(
                    "No se pudo identificar al Administrador."
                );
            }

            /*
             * Regla de negocio:
             *
             * solamente un Administrador ACTIVO
             * puede aprobar una solicitud de stock.
             *
             * Reutilizamos el método que ya existe
             * en CD_Usuario.
             */
            if (!cdUsuario.EsAdministradorActivo(
                    idUsuarioAutorizador))
            {
                throw new Exception(
                    "Solamente un Administrador activo " +
                    "puede aprobar solicitudes de stock."
                );
            }

            return cdSolicitudStock.Aprobar(
                idSolicitud,
                idUsuarioAutorizador
            );
        }


        /// <summary>
        /// Rechaza una solicitud de modificación de stock.
        ///
        /// Rechazar la solicitud no modifica las existencias
        /// del producto.
        /// 
        /// Solamente un usuario activo con rol Administrador
        /// puede rechazarla.
        /// </summary>
        public bool Rechazar(
            int idSolicitud,
            int idUsuarioAutorizador
        )
        {
            if (idSolicitud <= 0)
            {
                throw new ArgumentException(
                    "Debe seleccionar una solicitud válida."
                );
            }

            if (idUsuarioAutorizador <= 0)
            {
                throw new ArgumentException(
                    "No se pudo identificar al Administrador."
                );
            }

            /*
             * Aplicamos la misma regla de autorización
             * utilizada para aprobar.
             */
            if (!cdUsuario.EsAdministradorActivo(
                    idUsuarioAutorizador))
            {
                throw new Exception(
                    "Solamente un Administrador activo " +
                    "puede rechazar solicitudes de stock."
                );
            }


            return cdSolicitudStock.Rechazar(
                idSolicitud,
                idUsuarioAutorizador
            );
        }
    }
}
