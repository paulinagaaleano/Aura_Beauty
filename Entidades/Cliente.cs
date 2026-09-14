using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Entidades
{
    /// <summary>
    /// Representa a un cliente de Aura Beauty.
    ///
    /// Esta clase pertenece a la capa de Entidades.
    /// Su función es transportar los datos de un cliente
    /// entre las diferentes capas del sistema.
    ///
    /// No contiene consultas SQL, controles visuales
    /// ni reglas de negocio.
    /// </summary>
    public class Cliente
    {
        /// <summary>
        /// Identificador único del cliente.
        /// Corresponde a Id_cliente en SQL Server.
        /// </summary>
        public int IdCliente { get; set; }


        /// <summary>
        /// Nombre del cliente.
        /// </summary>
        public string Nombre { get; set; }


        /// <summary>
        /// Apellido del cliente.
        /// </summary>
        public string Apellido { get; set; }


        /// <summary>
        /// Documento Nacional de Identidad del cliente.
        /// </summary>
        public int Dni { get; set; }


        /// <summary>
        /// Fecha de nacimiento.
        ///
        /// El signo ? indica que DateTime puede admitir null.
        /// Esto coincide con la tabla SQL, donde
        /// fecha_nacimiento permite valores NULL.
        /// </summary>
        public DateTime? FechaNacimiento { get; set; }


        /// <summary>
        /// Fecha en que el cliente fue registrado
        /// en el sistema.
        /// </summary>
        public DateTime? FechaRegistro { get; set; }


        /// <summary>
        /// Domicilio o dirección del cliente.
        /// </summary>
        public string Domicilio { get; set; }


        /// <summary>
        /// Correo electrónico del cliente.
        /// </summary>
        public string Correo { get; set; }
    }
}
