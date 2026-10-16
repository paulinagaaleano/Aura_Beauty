using System;
using System.Collections.Generic;
using System.Data;

namespace Entidades
{
    /// <summary>
    /// Resultado agregado de un producto en el período consultado.
    /// No es una tabla nueva ni ejecuta SQL.
    /// </summary>
    public class ReporteProducto
    {
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; }
        public int IdCategoria { get; set; }
        public string NombreCategoria { get; set; }
        public bool EstaEnCatalogoActivo { get; set; }
        public int StockActual { get; set; }
        public int UnidadesVendidas { get; set; }
        public decimal FacturacionTotal { get; set; }
        public decimal PorcentajeFacturacion { get; set; }
    }

    /// <summary>
    /// Acumulación de ventas por categoría; el porcentaje corresponde al total
    /// de ventas de productos, no solamente a la categoría seleccionada.
    /// </summary>
    public class ReporteCategoria
    {
        public int IdCategoria { get; set; }
        public string NombreCategoria { get; set; }
        public int UnidadesVendidas { get; set; }
        public int ProductosConVentas { get; set; }
        public decimal FacturacionTotal { get; set; }
        public decimal PorcentajeFacturacion { get; set; }
    }

    /// <summary>
    /// Resultado de un análisis de productos y categorías.
    /// Se calcula en Negocio con la información recuperada por Datos.
    /// </summary>
    public class AnalisisProductosCategorias
    {
        public AnalisisProductosCategorias()
        {
            Productos = new List<ReporteProducto>();
            Categorias = new List<ReporteCategoria>();
        }

        public List<ReporteProducto> Productos { get; set; }
        public List<ReporteCategoria> Categorias { get; set; }
        public decimal FacturacionTotal { get; set; }
        public int UnidadesVendidas { get; set; }
        public int ProductosConVentas { get; set; }
        public int ProductosSinVentas { get; set; }
        public int CategoriasConVentas { get; set; }
        public int CategoriasSinVentas { get; set; }
    }

    /// <summary>
    /// Opción para el selector contextual de reportes. El Id es un número
    /// usado por la capa Negocio; no requiere tablas ni procedimientos nuevos.
    /// </summary>
    public class OpcionReporte
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }

    /// <summary>
    /// Un punto de un gráfico; el valor monetario admite decimales y la
    /// cantidad de unidades se expresa como entero desde Negocio.
    /// </summary>
    public class PuntoReporte
    {
        public string Etiqueta { get; set; }
        public decimal Valor { get; set; }
    }

    /// <summary>
    /// Modelo de presentación construido enteramente en Negocio.
    /// Transporta títulos, indicadores, gráficos y detalle sin consultas SQL.
    /// </summary>
    public class VistaReporte
    {
        public VistaReporte()
        {
            TitulosIndicadores = new string[6];
            ValoresIndicadores = new string[6];
            PuntosGrafico1 = new List<PuntoReporte>();
            PuntosGrafico2 = new List<PuntoReporte>();
            Detalle = new DataTable();
            Aviso = "";
        }

        public string Seccion { get; set; }
        public string[] TitulosIndicadores { get; set; }
        public string[] ValoresIndicadores { get; set; }
        public string TituloGrafico1 { get; set; }
        public string TituloGrafico2 { get; set; }
        public bool Grafico1EsCantidad { get; set; }
        public bool Grafico2EsCantidad { get; set; }
        public List<PuntoReporte> PuntosGrafico1 { get; set; }
        public List<PuntoReporte> PuntosGrafico2 { get; set; }
        public DataTable Detalle { get; set; }
        public string Aviso { get; set; }
    }
}
