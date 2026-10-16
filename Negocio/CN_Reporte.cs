
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Datos;
using Entidades;

namespace Negocio
{
    /// <summary>
    /// CAPA NEGOCIO - REPORTES DE AURA BEAUTY.
    ///
    /// Calcula indicadores, acumuladores, porcentajes
    /// y agrupaciones con los datos recuperados
    /// por la capa Datos.
    ///
    /// Reutiliza los procedimientos del módulo original:
    /// SP_ReporteVentas
    /// SP_ReporteMovimientosStock
    /// SP_ReporteVentasAnalisis
    /// SP_ReporteVentasDetallesAnalisis
    ///
    /// Para solicitudes pendientes reutiliza
    /// CD_SolicitudStock.ListarPendientes().
    ///
    /// No ejecuta sentencias SQL directamente.
    /// </summary>
    public class CN_Reporte
    {
        private readonly CD_Reporte datos =
            new CD_Reporte();

        private readonly CD_Producto datosProductos =
            new CD_Producto();

        private readonly CD_Categoria datosCategorias =
            new CD_Categoria();

        private readonly CD_Stock datosStock =
            new CD_Stock();

        private readonly CD_Usuario datosUsuarios =
            new CD_Usuario();

        private readonly CD_Cliente datosClientes =
            new CD_Cliente();

        private readonly CD_SolicitudStock datosSolicitudes =
            new CD_SolicitudStock();

        // ==================================================
        // CONSULTAS ORIGINALES
        // ==================================================

        /// <summary>
        /// Recupera ventas mediante el método original
        /// de la capa Datos.
        /// </summary>
        public List<ReporteVenta> ObtenerVentas(
            DateTime desde,
            DateTime hasta,
            int? vendedor)
        {
            ValidarFechas(desde, hasta);

            return datos.ObtenerVentas(
                desde, hasta, vendedor);
        }

        /// <summary>
        /// Recupera las cabeceras utilizadas
        /// en el análisis estadístico.
        /// </summary>
        public List<ReporteVenta> ObtenerVentasAnalisis(
            DateTime desde,
            DateTime hasta,
            int? vendedor)
        {
            ValidarFechas(desde, hasta);

            return datos.ObtenerVentasAnalisis(
                desde, hasta, vendedor);
        }

        /// <summary>
        /// Conserva el reporte original de movimientos.
        ///
        /// El mensaje de error se devuelve mediante
        /// un parámetro de salida.
        /// </summary>
        public DataTable ObtenerReporteMovimientosStock(
            DateTime desde,
            DateTime hasta,
            out string mensaje)
        {
            mensaje = "";

            if (desde.Date > hasta.Date)
            {
                mensaje =
                    "La fecha inicial es posterior a la final.";

                return new DataTable();
            }

            return datos.ObtenerReporteMovimientosStock(
                desde, hasta, out mensaje);
        }

        // ==================================================
        // RESUMEN DE VENTAS
        // ==================================================

        /// <summary>
        /// Calcula:
        /// - Facturación total.
        /// - Cantidad de ventas.
        /// - Ticket promedio.
        /// - Unidades vendidas.
        /// - Venta máxima y mínima.
        ///
        /// Utiliza acumuladores y comparaciones.
        /// </summary>
        public ResumenVentas ObtenerResumenVentas(
            DateTime desde,
            DateTime hasta,
            int? vendedor)
        {
            List<ReporteVenta> ventas =
                ObtenerVentasAnalisis(
                    desde, hasta, vendedor);

            List<DetalleVentaAnalisis> detalles =
                datos.ObtenerDetallesVentasAnalisis(
                    desde, hasta, vendedor);

            ResumenVentas resultado =
                new ResumenVentas();

            if (ventas.Count == 0)
                return resultado;

            decimal total = 0m;

            decimal maximo = ventas[0].Total;
            decimal minimo = ventas[0].Total;

            foreach (ReporteVenta venta in ventas)
            {
                total += venta.Total;

                if (venta.Total > maximo)
                    maximo = venta.Total;

                if (venta.Total < minimo)
                    minimo = venta.Total;
            }

            int unidades = 0;

            foreach (DetalleVentaAnalisis detalle in detalles)
                unidades += detalle.Cantidad;

            resultado.FacturacionTotal = total;
            resultado.CantidadVentas = ventas.Count;
            resultado.TicketPromedio = total / ventas.Count;
            resultado.UnidadesVendidas = unidades;
            resultado.VentaMaxima = maximo;
            resultado.VentaMinima = minimo;

            return resultado;
        }

        /// <summary>
        /// Agrupa las ventas por día y suma sus importes.
        /// </summary>
        public List<VentaPorDia> ObtenerVentasPorDia(
            DateTime desde,
            DateTime hasta,
            int? vendedor)
        {
            return ObtenerVentasAnalisis(
                desde, hasta, vendedor)
                .GroupBy(v => v.FechaVenta.Date)
                .OrderBy(g => g.Key)
                .Select(g => new VentaPorDia
                {
                    Fecha = g.Key,
                    Total = g.Sum(v => v.Total),
                    CantidadVentas = g.Count()
                })
                .ToList();
        }

        /// <summary>
        /// Compara la facturación de dos períodos.
        ///
        /// Variación porcentual:
        /// (actual - anterior) * 100 / anterior.
        ///
        /// Si el período anterior registra cero,
        /// el porcentaje se devuelve como null.
        /// </summary>
        public ComparacionVentas CompararVentas(
            DateTime desde,
            DateTime hasta,
            DateTime desdeAnterior,
            DateTime hastaAnterior,
            int? vendedor)
        {
            List<ReporteVenta> actual =
                ObtenerVentasAnalisis(
                    desde, hasta, vendedor);

            List<ReporteVenta> anterior =
                ObtenerVentasAnalisis(
                    desdeAnterior,
                    hastaAnterior,
                    vendedor);

            decimal a = actual.Sum(v => v.Total);
            decimal b = anterior.Sum(v => v.Total);

            return new ComparacionVentas
            {
                FacturacionActual = a,
                FacturacionAnterior = b,
                DiferenciaFacturacion = a - b,

                VariacionPorcentual = b > 0m
                    ? (decimal?)((a - b) * 100m / b)
                    : null,

                CantidadVentasActual = actual.Count,
                CantidadVentasAnterior = anterior.Count
            };
        }

        // ==================================================
        // FILTROS ESPECÍFICOS
        // ==================================================

        /// <summary>
        /// Carga las opciones correspondientes a cada
        /// sección del dashboard.
        ///
        /// Ventas: vendedores.
        /// Productos: productos.
        /// Categorías: categorías.
        /// Stock: productos.
        /// Solicitudes: estados.
        /// Vendedores: vendedores.
        /// Clientes: clientes.
        ///
        /// El identificador cero indica "Todos".
        /// </summary>
        public List<OpcionReporte> ObtenerOpciones(
            string seccion)
        {
            List<OpcionReporte> opciones =
                new List<OpcionReporte>();

            if (seccion == "Ventas" ||
                seccion == "Vendedores")
            {
                opciones.Add(new OpcionReporte
                {
                    Id = 0,
                    Nombre = "Todos los vendedores"
                });

                foreach (Usuario usuario in
                    datosUsuarios.Listar()
                    .Where(u => u.IdRol == 2)
                    .OrderBy(u => u.Apellido))
                {
                    opciones.Add(new OpcionReporte
                    {
                        Id = usuario.IdUsuario,
                        Nombre = usuario.Apellido +
                            ", " + usuario.Nombre
                    });
                }
            }
            else if (seccion == "Productos" ||
                     seccion == "Stock")
            {
                opciones.Add(new OpcionReporte
                {
                    Id = 0,
                    Nombre = "Todos los productos"
                });

                foreach (Producto producto in
                    datosProductos.Listar()
                    .OrderBy(p => p.Nombre))
                {
                    opciones.Add(new OpcionReporte
                    {
                        Id = producto.IdProducto,
                        Nombre = producto.Nombre
                    });
                }
            }
            else if (seccion == "Categorías")
            {
                opciones.Add(new OpcionReporte
                {
                    Id = 0,
                    Nombre = "Todas las categorías"
                });

                foreach (Categoria categoria in
                    datosCategorias.Listar()
                    .OrderBy(c => c.Nombre))
                {
                    opciones.Add(new OpcionReporte
                    {
                        Id = categoria.IdCategoria,
                        Nombre = categoria.Nombre
                    });
                }
            }
            else if (seccion == "Solicitudes")
            {
                opciones.Add(new OpcionReporte
                {
                    Id = 0,
                    Nombre = "Todos los estados"
                });

                opciones.Add(new OpcionReporte
                {
                    Id = 1,
                    Nombre = "PENDIENTE"
                });

                opciones.Add(new OpcionReporte
                {
                    Id = 2,
                    Nombre = "APROBADA"
                });

                opciones.Add(new OpcionReporte
                {
                    Id = 3,
                    Nombre = "RECHAZADA"
                });
            }
            else if (seccion == "Clientes")
            {
                opciones.Add(new OpcionReporte
                {
                    Id = 0,
                    Nombre = "Todos los clientes"
                });

                foreach (Cliente cliente in
                    datosClientes.Listar()
                    .OrderBy(c => c.Apellido))
                {
                    opciones.Add(new OpcionReporte
                    {
                        Id = cliente.IdCliente,
                        Nombre = cliente.Apellido +
                            ", " + cliente.Nombre
                    });
                }
            }

            return opciones;
        }

        // ==================================================
        // SELECCIÓN DEL REPORTE
        // ==================================================

        /// <summary>
        /// Punto de entrada de los reportes estadísticos.
        ///
        /// Recibe la sección y los filtros, y devuelve
        /// una VistaReporte lista para mostrar.
        /// </summary>
        public VistaReporte ConstruirReporte(
            string seccion,
            DateTime desde,
            DateTime hasta,
            int idFiltro)
        {
            ValidarFechas(desde, hasta);

            if (seccion == "Ventas")
                return ReporteVentas(
                    desde, hasta, idFiltro);

            if (seccion == "Productos")
                return ReporteProductos(
                    desde, hasta, idFiltro);

            if (seccion == "Categorías")
                return ReporteCategorias(
                    desde, hasta, idFiltro);

            if (seccion == "Stock")
                return ReporteStock(
                    desde, hasta, idFiltro);

            if (seccion == "Solicitudes")
                return ReporteSolicitudes(
                    desde, hasta, idFiltro);

            if (seccion == "Vendedores")
                return ReporteVendedores(
                    desde, hasta, idFiltro);

            if (seccion == "Clientes")
                return ReporteClientes(
                    desde, hasta, idFiltro);

            throw new ArgumentException(
                "Sección no reconocida: " + seccion);
        }

        // ==================================================
        // MÉTODOS AUXILIARES
        // ==================================================

        /// <summary>
        /// Verifica que el período tenga orden cronológico.
        /// </summary>
        private static void ValidarFechas(
            DateTime desde,
            DateTime hasta)
        {
            if (desde.Date > hasta.Date)
            {
                throw new ArgumentException(
                    "La fecha Desde no puede superar a Hasta.");
            }
        }

        private static string Moneda(decimal valor)
        {
            return "$ " + valor.ToString("N2");
        }

        private static string Numero(int valor)
        {
            return valor.ToString("N0");
        }

        private static string Porcentaje(decimal valor)
        {
            return valor.ToString("N2") + " %";
        }

        /// <summary>
        /// Calcula qué porcentaje representa una parte
        /// respecto de un total, sin dividir por cero.
        /// </summary>
        private static decimal Participacion(
            decimal parte,
            decimal total)
        {
            return total > 0m
                ? parte * 100m / total
                : 0m;
        }

        private static PuntoReporte Punto(
            string nombre,
            decimal valor)
        {
            return new PuntoReporte
            {
                Etiqueta = nombre,
                Valor = valor
            };
        }

        /// <summary>
        /// Crea una tabla de detalle con columnas de texto.
        /// </summary>
        private static DataTable Tabla(
            params string[] columnas)
        {
            DataTable tabla = new DataTable();

            foreach (string columna in columnas)
                tabla.Columns.Add(
                    columna, typeof(string));

            return tabla;
        }

        /// <summary>
        /// Lee una columna del DataTable si existe.
        /// </summary>
        private static string Texto(
            DataRow fila,
            string columna)
        {
            return fila.Table.Columns.Contains(columna) &&
                   fila[columna] != DBNull.Value
                ? Convert.ToString(fila[columna])
                : "";
        }

        /// <summary>
        /// Convierte una columna en número entero.
        /// </summary>
        private static int Entero(
            DataRow fila,
            string columna)
        {
            int valor;

            return int.TryParse(
                Texto(fila, columna),
                out valor)
                ? valor
                : 0;
        }

        // ==================================================
        // REPORTE: VENTAS
        // ==================================================

        /// <summary>
        /// Prepara los indicadores, gráficos y tabla
        /// del reporte de ventas.
        /// </summary>
        private VistaReporte ReporteVentas(
            DateTime desde,
            DateTime hasta,
            int filtro)
        {
            int? vendedor =
                filtro == 0 ? (int?)null : filtro;

            ResumenVentas resumen =
                ObtenerResumenVentas(
                    desde, hasta, vendedor);

            VistaReporte vista = new VistaReporte
            {
                Seccion = "Ventas",
                TituloGrafico1 = "Ventas por día",
                TituloGrafico2 = "Comparación mensual"
            };

            vista.TitulosIndicadores = new[]
            {
                "FACTURACIÓN",
                "OPERACIONES",
                "TICKET PROMEDIO",
                "UNIDADES",
                "VENTA MÁXIMA",
                "VENTA MÍNIMA"
            };

            vista.ValoresIndicadores = new[]
            {
                Moneda(resumen.FacturacionTotal),
                Numero(resumen.CantidadVentas),
                Moneda(resumen.TicketPromedio),
                Numero(resumen.UnidadesVendidas),
                Moneda(resumen.VentaMaxima),
                Moneda(resumen.VentaMinima)
            };

            foreach (VentaPorDia dia in
                ObtenerVentasPorDia(
                    desde, hasta, vendedor))
            {
                vista.PuntosGrafico1.Add(
                    Punto(
                        dia.Fecha.ToString("dd/MM"),
                        dia.Total));
            }

            DateTime mes =
                new DateTime(hasta.Year, hasta.Month, 1);

            DateTime mesAnterior = mes.AddMonths(-1);

            ComparacionVentas comparacion =
                CompararVentas(
                    mes,
                    mes.AddMonths(1).AddDays(-1),
                    mesAnterior,
                    mes.AddDays(-1),
                    vendedor);

            vista.PuntosGrafico2.Add(
                Punto(
                    mesAnterior.ToString("MM/yyyy"),
                    comparacion.FacturacionAnterior));

            vista.PuntosGrafico2.Add(
                Punto(
                    mes.ToString("MM/yyyy"),
                    comparacion.FacturacionActual));

            vista.Detalle = Tabla(
                "ID venta",
                "Fecha",
                "Total");

            foreach (ReporteVenta venta in
                ObtenerVentasAnalisis(
                    desde, hasta, vendedor))
            {
                vista.Detalle.Rows.Add(
                    venta.IdVenta.ToString(),
                    venta.FechaVenta.ToString(
                        "dd/MM/yyyy HH:mm"),
                    Moneda(venta.Total));
            }

            vista.Aviso =
                "Comparación entre meses completos. " +
                "La tabla corresponde al filtro de fechas.";

            return vista;
        }

        // ==================================================
        // ANÁLISIS DE PRODUCTOS Y CATEGORÍAS
        // ==================================================

        /// <summary>
        /// Agrupa los detalles de venta por producto.
        ///
        /// Utiliza el subtotal histórico, nunca
        /// el precio actual para reconstruir ventas.
        ///
        /// Los productos fuera del catálogo activo
        /// se conservan mediante su identificador.
        /// </summary>
        public AnalisisProductosCategorias
            ObtenerAnalisisProductosCategorias(
                DateTime desde,
                DateTime hasta,
                int? vendedor)
        {
            ValidarFechas(desde, hasta);

            List<Producto> catalogo =
                datosProductos.Listar();

            List<Categoria> categorias =
                datosCategorias.Listar();

            List<DetalleVentaAnalisis> detalles =
                datos.ObtenerDetallesVentasAnalisis(
                    desde, hasta, vendedor);

            var activos = catalogo.ToDictionary(
                p => p.IdProducto);

            var grupos = detalles
                .GroupBy(d => d.IdProducto)
                .ToDictionary(
                    g => g.Key,
                    g => g.ToList());

            var ids = catalogo
                .Select(p => p.IdProducto)
                .Union(
                    detalles.Select(d => d.IdProducto))
                .Distinct();

            AnalisisProductosCategorias respuesta =
                new AnalisisProductosCategorias();

            respuesta.FacturacionTotal =
                detalles.Sum(d => d.Subtotal);

            respuesta.UnidadesVendidas =
                detalles.Sum(d => d.Cantidad);

            foreach (int id in ids)
            {
                Producto producto;

                bool conocido =
                    activos.TryGetValue(
                        id, out producto);

                List<DetalleVentaAnalisis> filas;

                if (!grupos.TryGetValue(id, out filas))
                {
                    filas = new List<DetalleVentaAnalisis>();
                }

                decimal ingreso =
                    filas.Sum(d => d.Subtotal);

                ReporteProducto resultado =
                    new ReporteProducto
                    {
                        IdProducto = id,

                        NombreProducto = conocido
                            ? producto.Nombre
                            : "Producto no incluido en catálogo activo #" +
                              id,

                        IdCategoria = conocido
                            ? producto.IdCategoria
                            : 0,

                        NombreCategoria =
                            conocido &&
                            producto.oCategoria != null
                                ? producto.oCategoria.Nombre
                                : "Categoría no disponible",

                        EstaEnCatalogoActivo = conocido,

                        StockActual = conocido
                            ? producto.Stock
                            : 0,

                        UnidadesVendidas =
                            filas.Sum(d => d.Cantidad),

                        FacturacionTotal = ingreso,

                        PorcentajeFacturacion =
                            Participacion(
                                ingreso,
                                respuesta.FacturacionTotal)
                    };

                respuesta.Productos.Add(resultado);
            }

            respuesta.Productos =
                respuesta.Productos
                .OrderByDescending(p => p.FacturacionTotal)
                .ToList();

            respuesta.ProductosConVentas =
                respuesta.Productos.Count(
                    p => p.UnidadesVendidas > 0);

            respuesta.ProductosSinVentas =
                respuesta.Productos.Count(
                    p => p.EstaEnCatalogoActivo &&
                         p.UnidadesVendidas == 0);

            foreach (Categoria categoria in categorias)
            {
                List<ReporteProducto> lista =
                    respuesta.Productos
                    .Where(p =>
                        p.IdCategoria ==
                        categoria.IdCategoria)
                    .ToList();

                decimal ingreso =
                    lista.Sum(p => p.FacturacionTotal);

                respuesta.Categorias.Add(
                    new ReporteCategoria
                    {
                        IdCategoria = categoria.IdCategoria,
                        NombreCategoria = categoria.Nombre,

                        UnidadesVendidas =
                            lista.Sum(p => p.UnidadesVendidas),

                        ProductosConVentas =
                            lista.Count(
                                p => p.UnidadesVendidas > 0),

                        FacturacionTotal = ingreso,

                        PorcentajeFacturacion =
                            Participacion(
                                ingreso,
                                respuesta.FacturacionTotal)
                    });
            }

            // Las ventas sin información de categoría
            // no se asignan a una categoría inventada.
            List<ReporteProducto> noIdentificados =
                respuesta.Productos
                .Where(p => p.IdCategoria == 0)
                .ToList();

            if (noIdentificados.Count > 0)
            {
                decimal ingreso = noIdentificados
                    .Sum(p => p.FacturacionTotal);

                respuesta.Categorias.Add(
                    new ReporteCategoria
                    {
                        IdCategoria = 0,
                        NombreCategoria =
                            "Categoría no disponible",

                        UnidadesVendidas =
                            noIdentificados.Sum(
                                p => p.UnidadesVendidas),

                        ProductosConVentas =
                            noIdentificados.Count(
                                p => p.UnidadesVendidas > 0),

                        FacturacionTotal = ingreso,

                        PorcentajeFacturacion =
                            Participacion(
                                ingreso,
                                respuesta.FacturacionTotal)
                    });
            }

            respuesta.Categorias =
                respuesta.Categorias
                .OrderByDescending(c => c.FacturacionTotal)
                .ToList();

            respuesta.CategoriasConVentas =
                respuesta.Categorias.Count(
                    c => c.UnidadesVendidas > 0);

            respuesta.CategoriasSinVentas =
                respuesta.Categorias.Count(
                    c => c.IdCategoria > 0 &&
                         c.UnidadesVendidas == 0);

            return respuesta;
        }

        // ==================================================
        // REPORTE: PRODUCTOS
        // ==================================================

        /// <summary>
        /// Permite consultar todos los productos
        /// o uno específico mediante su ID.
        /// </summary>
        private VistaReporte ReporteProductos(
            DateTime desde,
            DateTime hasta,
            int id)
        {
            AnalisisProductosCategorias analisis =
                ObtenerAnalisisProductosCategorias(
                    desde, hasta, null);

            List<ReporteProducto> lista =
                analisis.Productos
                .Where(p =>
                    id == 0 || p.IdProducto == id)
                .ToList();

            VistaReporte vista = new VistaReporte
            {
                Seccion = "Productos",
                TituloGrafico1 = "Unidades por producto",
                TituloGrafico2 = "Facturación por producto",
                Grafico1EsCantidad = true
            };

            vista.Detalle = Tabla(
                "Producto",
                "Categoría",
                "Unidades",
                "Facturación",
                "Participación",
                "Stock actual");

            foreach (ReporteProducto producto in lista)
            {
                vista.Detalle.Rows.Add(
                    producto.NombreProducto,
                    producto.NombreCategoria,
                    Numero(producto.UnidadesVendidas),
                    Moneda(producto.FacturacionTotal),
                    Porcentaje(
                        producto.PorcentajeFacturacion),
                    producto.EstaEnCatalogoActivo
                        ? Numero(producto.StockActual)
                        : "Sin dato actual");
            }

            foreach (ReporteProducto producto in lista
                .OrderByDescending(
                    p => p.UnidadesVendidas)
                .Take(7))
            {
                vista.PuntosGrafico1.Add(
                    Punto(
                        producto.NombreProducto,
                        producto.UnidadesVendidas));
            }

            foreach (ReporteProducto producto in lista
                .OrderByDescending(
                    p => p.FacturacionTotal)
                .Take(7))
            {
                vista.PuntosGrafico2.Add(
                    Punto(
                        producto.NombreProducto,
                        producto.FacturacionTotal));
            }

            if (id == 0)
            {
                ReporteProducto lider =
                    analisis.Productos.FirstOrDefault();

                vista.TitulosIndicadores = new[]
                {
                    "FACTURACIÓN",
                    "PRODUCTOS VENDIDOS",
                    "UNIDADES",
                    "SIN VENTAS",
                    "LÍDER ($)",
                    "PARTICIPACIÓN LÍDER"
                };

                vista.ValoresIndicadores = new[]
                {
                    Moneda(analisis.FacturacionTotal),
                    Numero(analisis.ProductosConVentas),
                    Numero(analisis.UnidadesVendidas),
                    Numero(analisis.ProductosSinVentas),

                    Moneda(lider == null
                        ? 0m
                        : lider.FacturacionTotal),

                    Porcentaje(lider == null
                        ? 0m
                        : lider.PorcentajeFacturacion)
                };
            }
            else
            {
                ReporteProducto producto =
                    lista.FirstOrDefault();

                vista.TitulosIndicadores = new[]
                {
                    "FACTURACIÓN PRODUCTO",
                    "UNIDADES",
                    "PARTICIPACIÓN",
                    "STOCK ACTUAL",
                    "CATEGORÍA",
                    "EN CATÁLOGO ACTIVO"
                };

                vista.ValoresIndicadores =
                    producto == null
                    ? new[]
                    {
                        "$ 0,00",
                        "0",
                        "0 %",
                        "Sin datos",
                        "Sin datos",
                        "No"
                    }
                    : new[]
                    {
                        Moneda(producto.FacturacionTotal),

                        Numero(producto.UnidadesVendidas),

                        Porcentaje(
                            producto.PorcentajeFacturacion),

                        producto.EstaEnCatalogoActivo
                            ? Numero(producto.StockActual)
                            : "Sin dato",

                        producto.NombreCategoria,

                        producto.EstaEnCatalogoActivo
                            ? "Sí"
                            : "No"
                    };
            }

            vista.Aviso =
                "Las ventas históricas se calculan " +
                "con los subtotales registrados. " +
                "Catálogo: productos activos.";

            return vista;
        }

        // ==================================================
        // REPORTE: CATEGORÍAS
        // ==================================================

        /// <summary>
        /// Filtra las estadísticas por una categoría
        /// concreta, no por vendedor.
        /// </summary>
        private VistaReporte ReporteCategorias(
            DateTime desde,
            DateTime hasta,
            int id)
        {
            AnalisisProductosCategorias analisis =
                ObtenerAnalisisProductosCategorias(
                    desde, hasta, null);

            List<ReporteCategoria> lista =
                analisis.Categorias
                .Where(c =>
                    id == 0 || c.IdCategoria == id)
                .ToList();

            VistaReporte vista = new VistaReporte
            {
                Seccion = "Categorías",
                TituloGrafico1 = "Unidades por categoría",
                TituloGrafico2 = "Facturación por categoría",
                Grafico1EsCantidad = true
            };

            vista.Detalle = Tabla(
                "Categoría",
                "Productos vendidos",
                "Unidades",
                "Facturación",
                "Participación");

            foreach (ReporteCategoria categoria in lista)
            {
                vista.Detalle.Rows.Add(
                    categoria.NombreCategoria,
                    Numero(categoria.ProductosConVentas),
                    Numero(categoria.UnidadesVendidas),
                    Moneda(categoria.FacturacionTotal),
                    Porcentaje(
                        categoria.PorcentajeFacturacion));

                vista.PuntosGrafico1.Add(
                    Punto(
                        categoria.NombreCategoria,
                        categoria.UnidadesVendidas));

                vista.PuntosGrafico2.Add(
                    Punto(
                        categoria.NombreCategoria,
                        categoria.FacturacionTotal));
            }

            if (id == 0)
            {
                ReporteCategoria lider =
                    analisis.Categorias.FirstOrDefault();

                vista.TitulosIndicadores = new[]
                {
                    "FACTURACIÓN",
                    "CATEGORÍAS CON VENTAS",
                    "UNIDADES",
                    "CATEGORÍAS SIN VENTAS",
                    "CATEGORÍA LÍDER",
                    "PARTICIPACIÓN LÍDER"
                };

                vista.ValoresIndicadores = new[]
                {
                    Moneda(analisis.FacturacionTotal),
                    Numero(analisis.CategoriasConVentas),
                    Numero(analisis.UnidadesVendidas),
                    Numero(analisis.CategoriasSinVentas),

                    lider == null
                        ? "-"
                        : lider.NombreCategoria,

                    Porcentaje(lider == null
                        ? 0m
                        : lider.PorcentajeFacturacion)
                };
            }
            else
            {
                ReporteCategoria categoria =
                    lista.FirstOrDefault();

                vista.TitulosIndicadores = new[]
                {
                    "FACTURACIÓN CATEGORÍA",
                    "UNIDADES",
                    "PRODUCTOS VENDIDOS",
                    "PARTICIPACIÓN",
                    "CATEGORÍA",
                    "PERÍODO"
                };

                vista.ValoresIndicadores =
                    categoria == null
                    ? new[]
                    {
                        "$ 0,00", "0", "0",
                        "0 %", "-", "-"
                    }
                    : new[]
                    {
                        Moneda(categoria.FacturacionTotal),
                        Numero(categoria.UnidadesVendidas),
                        Numero(categoria.ProductosConVentas),

                        Porcentaje(
                            categoria.PorcentajeFacturacion),

                        categoria.NombreCategoria,

                        desde.ToString("dd/MM") +
                        " - " + hasta.ToString("dd/MM")
                    };
            }

            vista.Aviso =
                "La participación de cada categoría " +
                "corresponde a todas las ventas de " +
                "productos del período.";

            return vista;
        }

        // ==================================================
        // REPORTE: STOCK
        // ==================================================

        /// <summary>
        /// Consulta el stock actual y calcula:
        /// - Productos disponibles.
        /// - Unidades existentes.
        /// - Productos sin stock.
        /// - Productos con stock bajo.
        /// - Valor estimado a precio de venta.
        ///
        /// IMPORTANTE: el stock actual no es un
        /// historial de inventario.
        /// </summary>
        private VistaReporte ReporteStock(
            DateTime desde,
            DateTime hasta,
            int id)
        {
            List<Producto> todos = datosStock.Listar();

            List<Producto> seleccion =
                todos.Where(p =>
                    id == 0 || p.IdProducto == id)
                .ToList();

            VistaReporte vista = new VistaReporte
            {
                Seccion = "Stock",
                TituloGrafico1 =
                    "Unidades actuales por producto",
                TituloGrafico2 =
                    "Unidades por categoría",
                Grafico1EsCantidad = true,
                Grafico2EsCantidad = true
            };

            vista.TitulosIndicadores = new[]
            {
                "PRODUCTOS",
                "UNIDADES EN STOCK",
                "SIN STOCK",
                "STOCK BAJO (1-5)",
                "MAYOR EXISTENCIA",
                "VALOR A PRECIO VENTA"
            };

            int maximo = seleccion.Count == 0
                ? 0
                : seleccion.Max(p => p.Stock);

            decimal valor =
                seleccion.Sum(p => p.Stock * p.Precio);

            vista.ValoresIndicadores = new[]
            {
                Numero(seleccion.Count),
                Numero(seleccion.Sum(p => p.Stock)),
                Numero(seleccion.Count(p => p.Stock == 0)),

                Numero(seleccion.Count(p =>
                    p.Stock > 0 && p.Stock <= 5)),

                Numero(maximo),
                Moneda(valor)
            };

            vista.Detalle = Tabla(
                "Producto",
                "Categoría",
                "Stock",
                "Precio de venta",
                "Valor estimado",
                "Estado");

            foreach (Producto producto in
                seleccion.OrderBy(p => p.Stock))
            {
                vista.Detalle.Rows.Add(
                    producto.Nombre,

                    producto.oCategoria == null
                        ? ""
                        : producto.oCategoria.Nombre,

                    Numero(producto.Stock),
                    Moneda(producto.Precio),
                    Moneda(producto.Stock * producto.Precio),

                    producto.Stock == 0
                        ? "SIN STOCK"
                        : producto.Stock <= 5
                            ? "BAJO"
                            : "NORMAL");
            }

            foreach (Producto producto in
                seleccion
                .OrderByDescending(p => p.Stock)
                .Take(7))
            {
                vista.PuntosGrafico1.Add(
                    Punto(
                        producto.Nombre,
                        producto.Stock));
            }

            foreach (var grupo in seleccion
                .GroupBy(p =>
                    p.oCategoria == null
                        ? "Sin categoría"
                        : p.oCategoria.Nombre)
                .OrderByDescending(g =>
                    g.Sum(p => p.Stock))
                .Take(7))
            {
                vista.PuntosGrafico2.Add(
                    Punto(
                        grupo.Key,
                        grupo.Sum(p => p.Stock)));
            }

            string mensaje;

            DataTable movimientos =
                ObtenerReporteMovimientosStock(
                    desde, hasta, out mensaje);

            vista.Aviso =
                "Stock actual, no histórico. " +
                "Solicitudes recuperadas en el período: " +
                movimientos.Rows.Count + ".";

            if (!string.IsNullOrWhiteSpace(mensaje))
            {
                vista.Aviso +=
                    " Movimientos no disponibles: " + mensaje;
            }

            return vista;
        }

        // ==================================================
        // REPORTE: SOLICITUDES
        // ==================================================

        /// <summary>
        /// Combina las solicitudes pendientes actuales
        /// y los movimientos recuperados por el
        /// procedimiento almacenado existente.
        ///
        /// No afirma que los datos sean un historial
        /// completo de todas las solicitudes.
        /// </summary>
        private VistaReporte ReporteSolicitudes(
            DateTime desde,
            DateTime hasta,
            int filtro)
        {
            VistaReporte vista = new VistaReporte
            {
                Seccion = "Solicitudes",
                TituloGrafico1 =
                    "Solicitudes recuperadas por estado",
                TituloGrafico2 =
                    "Unidades solicitadas por tipo",
                Grafico1EsCantidad = true,
                Grafico2EsCantidad = true
            };

            var pendientes =
                datosSolicitudes.ListarPendientes();

            vista.Detalle = Tabla(
                "ID", "Fecha", "Producto", "Tipo",
                "Cantidad", "Estado", "Motivo");

            var filas =
                new List<Tuple<
                    int, DateTime, string, string,
                    int, string, string>>();

            foreach (SolicitudStock solicitud in pendientes
                .Where(s =>
                    s.FechaSolicitud.Date >= desde.Date &&
                    s.FechaSolicitud.Date <= hasta.Date))
            {
                filas.Add(Tuple.Create(
                    solicitud.IdSolicitud,
                    solicitud.FechaSolicitud,

                    solicitud.oProducto == null
                        ? ""
                        : solicitud.oProducto.Nombre,

                    solicitud.TipoMovimiento,
                    solicitud.Cantidad,
                    "PENDIENTE",
                    solicitud.Motivo));
            }

            string mensaje;

            DataTable movimientos =
                ObtenerReporteMovimientosStock(
                    desde, hasta, out mensaje);

            foreach (DataRow fila in movimientos.Rows)
            {
                int identificador =
                    Entero(fila, "IdSolicitud");

                if (filas.Any(f =>
                    f.Item1 == identificador))
                    continue;

                DateTime fecha;

                if (!DateTime.TryParse(
                    Texto(fila, "fecha_solicitud"),
                    out fecha))
                {
                    fecha = desde;
                }

                filas.Add(Tuple.Create(
                    identificador,
                    fecha,
                    Texto(fila, "Producto"),
                    Texto(fila, "TipoMovimiento"),
                    Entero(fila, "Cantidad"),
                    Texto(fila, "Estado").ToUpperInvariant(),
                    Texto(fila, "Motivo")));
            }

            var elegidos = filas
                .Where(f =>
                    filtro == 0 ||
                    (filtro == 1 &&
                     f.Item6 == "PENDIENTE") ||
                    (filtro == 2 &&
                     f.Item6 == "APROBADA") ||
                    (filtro == 3 &&
                     f.Item6 == "RECHAZADA"))
                .OrderByDescending(f => f.Item2)
                .ToList();

            foreach (var fila in elegidos)
            {
                vista.Detalle.Rows.Add(
                    fila.Item1.ToString(),
                    fila.Item2.ToString("dd/MM/yyyy"),
                    fila.Item3,
                    fila.Item4,
                    Numero(fila.Item5),
                    fila.Item6,
                    fila.Item7);
            }

            int pendientesN =
                elegidos.Count(f =>
                    f.Item6 == "PENDIENTE");

            int aprobadas =
                elegidos.Count(f =>
                    f.Item6 == "APROBADA");

            int rechazadas =
                elegidos.Count(f =>
                    f.Item6 == "RECHAZADA");

            vista.TitulosIndicadores = new[]
            {
                "RECUPERADAS",
                "PENDIENTES",
                "APROBADAS RECUPERADAS",
                "RECHAZADAS RECUPERADAS",
                "UNIDADES SOLICITADAS",
                "TIPO MÁS FRECUENTE"
            };

            string tipo = elegidos
                .GroupBy(f => f.Item4)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault() ?? "-";

            vista.ValoresIndicadores = new[]
            {
                Numero(elegidos.Count),
                Numero(pendientesN),
                Numero(aprobadas),
                Numero(rechazadas),

                Numero(elegidos.Sum(f => f.Item5)),
                tipo
            };

            foreach (var grupo in elegidos
                .GroupBy(f => f.Item6))
            {
                vista.PuntosGrafico1.Add(
                    Punto(grupo.Key, grupo.Count()));
            }

            foreach (var grupo in elegidos
                .GroupBy(f => f.Item4))
            {
                vista.PuntosGrafico2.Add(
                    Punto(
                        grupo.Key,
                        grupo.Sum(f => f.Item5)));
            }

            vista.Aviso =
                "Pendientes actuales + resoluciones " +
                "recuperadas por el procedimiento existente; " +
                "no equivale a un historial completo.";

            if (!string.IsNullOrWhiteSpace(mensaje))
                vista.Aviso += " Movimientos: " + mensaje;

            return vista;
        }

        // ==================================================
        // REPORTE: VENDEDORES
        // ==================================================

        /// <summary>
        /// Reúne las ventas de cada usuario vendedor.
        /// Calcula operaciones, facturación y promedio.
        /// </summary>
        private VistaReporte ReporteVendedores(
            DateTime desde,
            DateTime hasta,
            int id)
        {
            List<Usuario> vendedores =
                datosUsuarios.Listar()
                .Where(u =>
                    u.IdRol == 2 &&
                    (id == 0 || u.IdUsuario == id))
                .ToList();

            VistaReporte vista = new VistaReporte
            {
                Seccion = "Vendedores",
                TituloGrafico1 =
                    "Operaciones por vendedor",
                TituloGrafico2 =
                    "Facturación por vendedor",
                Grafico1EsCantidad = true
            };

            vista.Detalle = Tabla(
                "Vendedor",
                "Operaciones",
                "Facturación",
                "Ticket promedio");

            decimal total = 0m;
            int cantidad = 0;
            decimal mejor = 0m;
            string lider = "-";

            foreach (Usuario usuario in vendedores)
            {
                List<ReporteVenta> ventas =
                    ObtenerVentasAnalisis(
                        desde,
                        hasta,
                        usuario.IdUsuario);

                decimal importe =
                    ventas.Sum(v => v.Total);

                int operaciones = ventas.Count;

                vista.Detalle.Rows.Add(
                    usuario.Apellido + ", " + usuario.Nombre,
                    Numero(operaciones),
                    Moneda(importe),

                    Moneda(operaciones == 0
                        ? 0m
                        : importe / operaciones));

                vista.PuntosGrafico1.Add(
                    Punto(
                        usuario.Apellido + ", " +
                        usuario.Nombre,
                        operaciones));

                vista.PuntosGrafico2.Add(
                    Punto(
                        usuario.Apellido + ", " +
                        usuario.Nombre,
                        importe));

                total += importe;
                cantidad += operaciones;

                if (importe > mejor)
                {
                    mejor = importe;

                    lider = usuario.Apellido +
                        ", " + usuario.Nombre;
                }
            }

            vista.TitulosIndicadores = new[]
            {
                "VENDEDORES",
                "OPERACIONES",
                "FACTURACIÓN",
                "TICKET PROMEDIO",
                "MAYOR FACTURACIÓN",
                "VENDEDOR LÍDER"
            };

            vista.ValoresIndicadores = new[]
            {
                Numero(vendedores.Count),
                Numero(cantidad),
                Moneda(total),

                Moneda(cantidad == 0
                    ? 0m
                    : total / cantidad),

                Moneda(mejor),
                lider
            };

            vista.Aviso =
                "Se consideran los usuarios con rol Vendedor " +
                "que aparecen en el catálogo activo.";

            return vista;
        }

        // ==================================================
        // REPORTE: CLIENTES
        // ==================================================

        /// <summary>
        /// Recupera los clientes y procura relacionar
        /// sus compras con el campo Cliente del reporte.
        ///
        /// Si el procedimiento no devuelve información
        /// que pueda utilizarse, informa la limitación.
        ///
        /// No crea compras ni registros ficticios.
        /// </summary>
        private VistaReporte ReporteClientes(
            DateTime desde,
            DateTime hasta,
            int id)
        {
            List<Cliente> clientes =
                datosClientes.Listar();

            List<Cliente> elegidos =
                clientes
                .Where(c =>
                    id == 0 || c.IdCliente == id)
                .ToList();

            VistaReporte vista = new VistaReporte
            {
                Seccion = "Clientes",
                TituloGrafico1 =
                    "Compras por cliente",
                TituloGrafico2 =
                    "Facturación por cliente",
                Grafico1EsCantidad = true
            };

            vista.Detalle = Tabla(
                "Cliente",
                "DNI",
                "Operaciones",
                "Facturación",
                "Ticket promedio");

            List<ReporteVenta> ventas =
                new List<ReporteVenta>();

            bool comprasDisponibles = true;

            try
            {
                ventas = ObtenerVentas(
                    desde, hasta, null);
            }
            catch (Exception ex)
            {
                comprasDisponibles = false;

                vista.Aviso =
                    "El padrón de clientes está disponible, " +
                    "pero SP_ReporteVentas no permitió " +
                    "recuperar compras: " + ex.Message;
            }

            decimal total = 0m;
            int operacionesTotal = 0;
            int conCompras = 0;

            foreach (Cliente cliente in elegidos)
            {
                List<ReporteVenta> compras =
                    comprasDisponibles
                    ? ventas.Where(v =>
                        v.Cliente == cliente.IdCliente.ToString() ||
                        v.Cliente == cliente.Nombre + " " +
                            cliente.Apellido ||
                        v.Cliente == cliente.Apellido + ", " +
                            cliente.Nombre)
                      .ToList()
                    : new List<ReporteVenta>();

                int cantidad = compras.Count;

                decimal importe =
                    compras.Sum(v => v.Total);

                if (cantidad > 0)
                    conCompras++;

                operacionesTotal += cantidad;
                total += importe;

                vista.Detalle.Rows.Add(
                    cliente.Apellido + ", " +
                        cliente.Nombre,

                    cliente.Dni.ToString(),

                    comprasDisponibles
                        ? Numero(cantidad)
                        : "Sin dato",

                    comprasDisponibles
                        ? Moneda(importe)
                        : "Sin dato",

                    comprasDisponibles
                        ? Moneda(cantidad == 0
                            ? 0m
                            : importe / cantidad)
                        : "Sin dato");

                if (comprasDisponibles)
                {
                    vista.PuntosGrafico1.Add(
                        Punto(
                            cliente.Apellido + ", " +
                            cliente.Nombre,
                            cantidad));

                    vista.PuntosGrafico2.Add(
                        Punto(
                            cliente.Apellido + ", " +
                            cliente.Nombre,
                            importe));
                }
            }

            vista.PuntosGrafico1 =
                vista.PuntosGrafico1
                .OrderByDescending(p => p.Valor)
                .Take(7)
                .ToList();

            vista.PuntosGrafico2 =
                vista.PuntosGrafico2
                .OrderByDescending(p => p.Valor)
                .Take(7)
                .ToList();

            vista.TitulosIndicadores = new[]
            {
                "CLIENTES",
                "CON COMPRAS",
                "OPERACIONES",
                "FACTURACIÓN",
                "TICKET PROMEDIO",
                "SIN COMPRAS REGISTRADAS"
            };

            vista.ValoresIndicadores = new[]
            {
                Numero(elegidos.Count),

                comprasDisponibles
                    ? Numero(conCompras)
                    : "Sin dato",

                comprasDisponibles
                    ? Numero(operacionesTotal)
                    : "Sin dato",

                comprasDisponibles
                    ? Moneda(total)
                    : "Sin dato",

                comprasDisponibles
                    ? Moneda(operacionesTotal == 0
                        ? 0m
                        : total / operacionesTotal)
                    : "Sin dato",

                comprasDisponibles
                    ? Numero(elegidos.Count - conCompras)
                    : "Sin dato"
            };

            if (comprasDisponibles)
            {
                vista.Aviso =
                    "Compras vinculadas por el campo Cliente " +
                    "de SP_ReporteVentas. " +
                    "Las ventas sin cliente no se asignan.";
            }

            return vista;
        }
    }
}
