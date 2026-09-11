using System;
using System.Windows.Forms;
using Aura_Beauty; // <--- Añade esto si el formulario usa este namespace

namespace Aura_Beauty
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmLogin());
        }
    }
}