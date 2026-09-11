using System.Collections.Generic;
using Datos;
using Entidades;

namespace Negocio
{
    public class CN_Usuario
    {
        private CD_Usuario obj_cd_usuario = new CD_Usuario();

        public List<Usuario> Listar()
        {
            return obj_cd_usuario.Listar();
        }
    }
}
