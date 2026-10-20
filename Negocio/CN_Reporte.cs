
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Datos;
using Entidades;

namespace Negocio
{
    /// <summary>
    /// CAPA NEGOCIO: Centro de Reportes de Aura Beauty.
    ///
    /// Calcula sumas, promedios, porcentajes,
    /// agrupaciones y rankings comerciales.
    ///
    /// No ejecuta SQL directamente.
    /// No crea procedimientos almacenados.
    ///
    /// Se conservan las firmas públicas utilizadas
    /// por los otros módulos del sistema.
    /// </summary>
    public class CN_Reporte
    {
        private readonly CD_Reporte datos =
            new CD_Reporte();

        private readonly CD_Producto datosProductos =
            new CD_Producto();

        private readonly CD_Categoria datosCategorias =
            new CD_Categoria();

        private readonly CD_Usuario datosUsuarios =
            new CD_Usuario();

        private readonly CD_Cliente datosClientes =
            new CD_Cliente();

        private readonly CD_SolicitudStock datosSolicitudes =
            new CD_SolicitudStock();

        // =====================================================
        // 1. CONSULTAS ORIGINALES
        // =====================================================

        /// <summary>
        /// Recupera el listado de ventas mediante Datos.
        /// Mantiene el método utilizado por otros módulos.
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
        /// Recupera las cabeceras de las ventas
        /// para calcular estadísticas.
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
        /// Conserva la consulta existente de
        /// movimientos de stock para Solicitudes.
        ///
        /// No se utiliza para incluir productos
        /// sin ventas en el reporte comercial.
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

        // =====================================================
        // 2. RESUMEN DE VENTAS
        // =====================================================

        /// <summary>
        /// Calcula:
        /// - Facturación total.
        /// - Cantidad de operaciones.
        /// - Ticket promedio.
        /// - Unidades vendidas.
        /// - Venta máxima y mínima.
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

            ResumenVentas resumen =
                new ResumenVentas();

            if (ventas.Count == 0)
                return resumen;

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

            resumen.FacturacionTotal = total;
            resumen.CantidadVentas = ventas.Count;

            resumen.TicketPromedio =
                total / ventas.Count;

            resumen.UnidadesVendidas =
                detalles.Sum(d => d.Cantidad);

            resumen.VentaMaxima = maximo;
            resumen.VentaMinima = minimo;

            return resumen;
        }

        /// <summary>
        /// Agrupa las ventas por fecha.
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

            decimal totalActual =
                actual.Sum(v => v.Total);

            decimal totalAnterior =
                anterior.Sum(v => v.Total);

            return new ComparacionVentas
            {
                FacturacionActual = totalActual,

                FacturacionAnterior = totalAnterior,

                DiferenciaFacturacion =
                    totalActual - totalAnterior,

                VariacionPorcentual =
                    totalAnterior > 0m
                    ? (decimal?)(
                        (totalActual - totalAnterior) *
                        100m / totalAnterior)
                    : null,

                CantidadVentasActual = actual.Count,

                CantidadVentasAnterior = anterior.Count
            };
        }

        // =====================================================
        // 3. FILTROS SEGÚN LA SECCIÓN
        // =====================================================

        /// <summary>
        /// Devuelve las opciones para el filtro
        /// correspondiente a cada reporte.
        ///
        /// Productos: solo productos vendidos.
        /// Categorías: solo categorías con ventas.
        /// Vendedores: solo vendedores con operaciones.
        /// </summary>
        public List<OpcionReporte> ObtenerOpciones(
            string seccion,
            DateTime desde,
            DateTime hasta)
        {
            ValidarFechas(desde, hasta);

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
                    if (seccion == "Vendedores")
                    {
                        int operaciones =
                            datos.ObtenerVentasAnalisis(
                                desde,
                                hasta,
                                usuario.IdUsuario).Count;

                        if (operaciones == 0)
                            continue;
                    }

                    opciones.Add(new OpcionReporte
                    {
                        Id = usuario.IdUsuario,

                        Nombre = usuario.Apellido +
                            ", " + usuario.Nombre
                    });
                }
            }
            else if (seccion == "Productos" ||
                     seccion == "Categorías")
            {
                AnalisisProductosCategorias analisis =
                    ObtenerAnalisisProductosCategorias(
                        desde, hasta, null);

                if (seccion == "Productos")
                {
                    opciones.Add(new OpcionReporte
                    {
                        Id = 0,
                        Nombre = "Todos los productos"
                    });

                    foreach (ReporteProducto p in
                        analisis.Productos
                        .OrderBy(p => p.NombreProducto))
                    {
                        opciones.Add(new OpcionReporte
                        {
                            Id = p.IdProducto,
                            Nombre = p.NombreProducto
                        });
                    }
                }
                else
                {
                    opciones.Add(new OpcionReporte
                    {
                        Id = 0,
                        Nombre = "Todas las categorías"
                    });

                    foreach (ReporteCategoria c in
                        analisis.Categorias
                        .OrderBy(c => c.NombreCategoria))
                    {
                        opciones.Add(new OpcionReporte
                        {
                            Id = c.IdCategoria,
                            Nombre = c.NombreCategoria
                        });
                    }
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

                foreach (Cliente c in
                    datosClientes.Listar()
                    .OrderBy(c => c.Apellido))
                {
                    opciones.Add(new OpcionReporte
                    {
                        Id = c.IdCliente,

                        Nombre = c.Apellido +
                            ", " + c.Nombre
                    });
                }
            }

            return opciones;
        }

        // =====================================================
        // 4. SELECCIÓN DEL REPORTE
        // =====================================================

        /// <summary>
        /// Recibe la sección y los filtros del formulario.
        /// Devuelve una VistaReporte ya calculada.
        /// </summary>
        public VistaReporte ConstruirReporte(
            string seccion,
            DateTime desde,
            DateTime hasta,
            int idFiltro)
        {
            ValidarFechas(desde, hasta);

            switch (seccion)
            {
                case "Ventas":
                    return ReporteVentas(
                        desde, hasta, idFiltro);

                case "Productos":
                    return ReporteProductos(
                        desde, hasta, idFiltro);

                case "Categorías":
                    return ReporteCategorias(
                        desde, hasta, idFiltro);

                case "Solicitudes":
                    return ReporteSolicitudes(
                        desde, hasta, idFiltro);

                case "Vendedores":
                    return ReporteVendedores(
                        desde, hasta, idFiltro);

                case "Clientes":
                    return ReporteClientes(
                        desde, hasta, idFiltro);

                default:
                    throw new ArgumentException(
                        "Sección no reconocida: " + seccion);
            }
        }

        // =====================================================
        // 5. MÉTODOS AUXILIARES
        // =====================================================

        /// <summary>
        /// Comprueba que el período sea válido.
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

        /// <summary>
        /// Formatea un importe monetario.
        /// </summary>
        private static string Moneda(decimal cantidad)
        {
            return "$ " + cantidad.ToString("N2");
        }

        /// <summary>
        /// Formatea cantidades sin decimales.
        /// </summary>
        private static string Numero(int cantidad)
        {
            return cantidad.ToString("N0");
        }

        /// <summary>
        /// Formatea un porcentaje con dos decimales.
        /// </summary>
        private static string Porcentaje(decimal cantidad)
        {
            return cantidad.ToString("N2") + " %";
        }

        /// <summary>
        /// Calcula qué porcentaje representa una parte
        /// respecto de un total.
        /// </summary>
        private static decimal Participacion(
            decimal parte,
            decimal total)
        {
            return total > 0m
                ? parte * 100m / total
                : 0m;
        }

        /// <summary>
        /// Prepara un punto del gráfico.
        /// </summary>
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
        /// Crea las columnas de una tabla de resultados.
        /// </summary>
        private static DataTable Tabla(
            params string[] columnas)
        {
            DataTable tabla = new DataTable();

            foreach (string columna in columnas)
            {
                tabla.Columns.Add(
                    columna, typeof(string));
            }

            return tabla;
        }

        /// <summary>
        /// Lee una columna de un DataRow si existe.
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
        /// Convierte un dato de la tabla a entero.
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

        // =====================================================
        // 6. REPORTE DE VENTAS
        // =====================================================

        /// <summary>
        /// Mantiene los indicadores y gráficos de ventas.
        /// </summary>
        private VistaReporte ReporteVentas(
            DateTime desde,
            DateTime hasta,
            int filtro)
        {
            int? vendedor =
                filtro == 0 ? (int?)null : filtro;

            ResumenVentas r =
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
                Moneda(r.FacturacionTotal),
                Numero(r.CantidadVentas),
                Moneda(r.TicketPromedio),
                Numero(r.UnidadesVendidas),
                Moneda(r.VentaMaxima),
                Moneda(r.VentaMinima)
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
                new DateTime(
                    hasta.Year, hasta.Month, 1);

            DateTime anterior =
                mes.AddMonths(-1);

            ComparacionVentas comparacion =
                CompararVentas(
                    mes,
                    mes.AddMonths(1).AddDays(-1),
                    anterior,
                    mes.AddDays(-1),
                    vendedor);

            vista.PuntosGrafico2.Add(
                Punto(
                    anterior.ToString("MM/yyyy"),
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
                "Comparación de meses completos. " +
                "El detalle respeta las fechas elegidas.";

            return vista;
        }

        // =====================================================
        // 7. PRODUCTOS: SOLO VENTAS EFECTIVAS
        // =====================================================

        /// <summary>
        /// Calcula estadísticas exclusivamente a partir
        /// de productos que tienen unidades vendidas.
        ///
        /// Ya no une los productos vendidos con los que
        /// solamente tuvieron movimientos de inventario.
        ///
        /// Se conservan las ventas históricas aunque un
        /// artículo ya no figure en el catálogo activo.
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

            List<DetalleVentaAnalisis> ventas =
                datos.ObtenerDetallesVentasAnalisis(
                    desde, hasta, vendedor)

                .Where(d => d.Cantidad > 0)

                .ToList();

            Dictionary<int, Producto> productosPorId =
                catalogo

                .GroupBy(p => p.IdProducto)

                .ToDictionary(
                    g => g.Key,
                    g => g.First());

            AnalisisProductosCategorias analisis =
                new AnalisisProductosCategorias();

            analisis.FacturacionTotal =
                ventas.Sum(d => d.Subtotal);

            analisis.UnidadesVendidas =
                ventas.Sum(d => d.Cantidad);

            // Solo aparecen productos de ventas reales.
            foreach (var grupo in ventas.GroupBy(
                d => d.IdProducto))
            {
                Producto producto;

                bool existeEnCatalogo =
                    productosPorId.TryGetValue(
                        grupo.Key,
                        out producto);

                decimal facturacion =
                    grupo.Sum(d => d.Subtotal);

                analisis.Productos.Add(
                    new ReporteProducto
                    {
                        IdProducto = grupo.Key,

                        NombreProducto =
                            existeEnCatalogo
                                ? producto.Nombre
                                : "Producto histórico #" +
                                  grupo.Key,

                        IdCategoria =
                            existeEnCatalogo
                                ? producto.IdCategoria
                                : 0,

                        NombreCategoria =
                            existeEnCatalogo &&
                            producto.oCategoria != null
                                ? producto.oCategoria.Nombre
                                : "Categoría no disponible",

                        EstaEnCatalogoActivo =
                            existeEnCatalogo,

                        StockActual =
                            existeEnCatalogo
                                ? producto.Stock
                                : 0,

                        UnidadesVendidas =
                            grupo.Sum(d => d.Cantidad),

                        FacturacionTotal =
                            facturacion,

                        PorcentajeFacturacion =
                            Participacion(
                                facturacion,
                                analisis.FacturacionTotal)
                    });
            }

            analisis.Productos =
                analisis.Productos

                .OrderByDescending(
                    p => p.FacturacionTotal)

                .ThenBy(p => p.NombreProducto)

                .ToList();

            analisis.ProductosConVentas =
                analisis.Productos.Count;

            analisis.ProductosSinVentas = 0;

            // Solo categorías con productos vendidos.
            foreach (Categoria categoria in
                categorias

                .GroupBy(c => c.IdCategoria)

                .Select(g => g.First()))
            {
                List<ReporteProducto> productos =
                    analisis.Productos

                    .Where(p =>
                        p.IdCategoria ==
                        categoria.IdCategoria)

                    .ToList();

                if (productos.Count == 0)
                    continue;

                decimal facturacion =
                    productos.Sum(
                        p => p.FacturacionTotal);

                analisis.Categorias.Add(
                    new ReporteCategoria
                    {
                        IdCategoria =
                            categoria.IdCategoria,

                        NombreCategoria =
                            categoria.Nombre,

                        ProductosConVentas =
                            productos.Count,

                        UnidadesVendidas =
                            productos.Sum(
                                p => p.UnidadesVendidas),

                        FacturacionTotal =
                            facturacion,

                        PorcentajeFacturacion =
                            Participacion(
                                facturacion,
                                analisis.FacturacionTotal)
                    });
            }

            // No se descartan ventas históricas
            // de productos ya retirados del catálogo.
            HashSet<int> idsCategorias =
                new HashSet<int>(
                    categorias.Select(
                        c => c.IdCategoria));

            List<ReporteProducto> sinCategoria =
                analisis.Productos

                .Where(p =>
                    p.IdCategoria == 0 ||
                    !idsCategorias.Contains(
                        p.IdCategoria))

                .ToList();

            if (sinCategoria.Count > 0)
            {
                decimal facturacion =
                    sinCategoria.Sum(
                        p => p.FacturacionTotal);

                analisis.Categorias.Add(
                    new ReporteCategoria
                    {
                        IdCategoria = -1,

                        NombreCategoria =
                            "Categoría no disponible",

                        ProductosConVentas =
                            sinCategoria.Count,

                        UnidadesVendidas =
                            sinCategoria.Sum(
                                p => p.UnidadesVendidas),

                        FacturacionTotal =
                            facturacion,

                        PorcentajeFacturacion =
                            Participacion(
                                facturacion,
                                analisis.FacturacionTotal)
                    });
            }

            analisis.Categorias =
                analisis.Categorias

                .OrderByDescending(
                    c => c.FacturacionTotal)

                .ThenBy(c => c.NombreCategoria)

                .ToList();

            analisis.CategoriasConVentas =
                analisis.Categorias.Count;

            analisis.CategoriasSinVentas = 0;

            return analisis;
        }

        // =====================================================
        // 8. REPORTE DE PRODUCTOS
        // =====================================================

        /// <summary>
        /// Muestra exclusivamente los productos vendidos.
        ///
        /// Desaparecen:
        /// - Movimiento sin venta.
        /// - Solo movimientos.
        /// - Productos sin ventas.
        /// </summary>
        private VistaReporte ReporteProductos(
            DateTime desde,
            DateTime hasta,
            int id)
        {
            AnalisisProductosCategorias analisis =
                ObtenerAnalisisProductosCategorias(
                    desde, hasta, null);

            List<ReporteProducto> productos =
                analisis.Productos

                .Where(p =>
                    id == 0 || p.IdProducto == id)

                .ToList();

            VistaReporte vista = new VistaReporte
            {
                Seccion = "Productos",

                TituloGrafico1 =
                    "Unidades vendidas por producto",

                TituloGrafico2 =
                    "Facturación por producto",

                Grafico1EsCantidad = true
            };

            vista.Detalle = Tabla(
                "Producto",
                "Categoría",
                "Unidades vendidas",
                "Facturación",
                "Participación");

            foreach (ReporteProducto p in productos)
            {
                vista.Detalle.Rows.Add(
                    p.NombreProducto,

                    p.NombreCategoria,

                    Numero(p.UnidadesVendidas),

                    Moneda(p.FacturacionTotal),

                    Porcentaje(
                        p.PorcentajeFacturacion));
            }

            foreach (ReporteProducto p in productos
                .OrderByDescending(
                    p => p.UnidadesVendidas)
                .Take(7))
            {
                vista.PuntosGrafico1.Add(
                    Punto(
                        p.NombreProducto,
                        p.UnidadesVendidas));
            }

            foreach (ReporteProducto p in productos
                .OrderByDescending(
                    p => p.FacturacionTotal)
                .Take(7))
            {
                vista.PuntosGrafico2.Add(
                    Punto(
                        p.NombreProducto,
                        p.FacturacionTotal));
            }

            if (id == 0)
            {
                ReporteProducto lider =
                    analisis.Productos.FirstOrDefault();

                vista.TitulosIndicadores = new[]
                {
                    "PRODUCTOS VENDIDOS",
                    "FACTURACIÓN TOTAL",
                    "UNIDADES VENDIDAS",
                    "PROMEDIO POR PRODUCTO",
                    "PRODUCTO LÍDER",
                    "PARTICIPACIÓN LÍDER"
                };

                vista.ValoresIndicadores = new[]
                {
                    Numero(analisis.Productos.Count),

                    Moneda(analisis.FacturacionTotal),

                    Numero(analisis.UnidadesVendidas),

                    Moneda(
                        analisis.Productos.Count > 0
                        ? analisis.FacturacionTotal /
                          analisis.Productos.Count
                        : 0m),

                    lider == null
                        ? "-"
                        : lider.NombreProducto,

                    Porcentaje(
                        lider == null
                        ? 0m
                        : lider.PorcentajeFacturacion)
                };
            }
            else
            {
                ReporteProducto p =
                    productos.FirstOrDefault();

                vista.TitulosIndicadores = new[]
                {
                    "FACTURACIÓN",
                    "UNIDADES VENDIDAS",
                    "PARTICIPACIÓN",
                    "INGRESO POR UNIDAD",
                    "CATEGORÍA",
                    "PRODUCTO"
                };

                vista.ValoresIndicadores = p == null
                    ? new[]
                    {
                        "$ 0,00",
                        "0",
                        "0 %",
                        "$ 0,00",
                        "-",
                        "Sin ventas"
                    }
                    : new[]
                    {
                        Moneda(p.FacturacionTotal),

                        Numero(p.UnidadesVendidas),

                        Porcentaje(
                            p.PorcentajeFacturacion),

                        Moneda(
                            p.FacturacionTotal /
                            p.UnidadesVendidas),

                        p.NombreCategoria,

                        p.NombreProducto
                    };
            }

            vista.Aviso =
                "Se muestran solamente productos vendidos " +
                "entre " +
                desde.ToString("dd/MM/yyyy") +
                " y " +
                hasta.ToString("dd/MM/yyyy") + ".";

            return vista;
        }

        // =====================================================
        // 9. REPORTE DE CATEGORÍAS
        // =====================================================

        /// <summary>
        /// Agrupa las ventas por categoría.
        /// Excluye categorías sin productos vendidos.
        /// </summary>
        private VistaReporte ReporteCategorias(
            DateTime desde,
            DateTime hasta,
            int id)
        {
            AnalisisProductosCategorias analisis =
                ObtenerAnalisisProductosCategorias(
                    desde, hasta, null);

            List<ReporteCategoria> categorias =
                analisis.Categorias

                .Where(c =>
                    id == 0 || c.IdCategoria == id)

                .ToList();

            VistaReporte vista = new VistaReporte
            {
                Seccion = "Categorías",

                TituloGrafico1 =
                    "Unidades vendidas por categoría",

                TituloGrafico2 =
                    "Facturación por categoría",

                Grafico1EsCantidad = true
            };

            vista.Detalle = Tabla(
                "Categoría",
                "Productos vendidos",
                "Unidades vendidas",
                "Facturación",
                "Participación");

            foreach (ReporteCategoria c in categorias)
            {
                vista.Detalle.Rows.Add(
                    c.NombreCategoria,

                    Numero(c.ProductosConVentas),

                    Numero(c.UnidadesVendidas),

                    Moneda(c.FacturacionTotal),

                    Porcentaje(
                        c.PorcentajeFacturacion));

                vista.PuntosGrafico1.Add(
                    Punto(
                        c.NombreCategoria,
                        c.UnidadesVendidas));

                vista.PuntosGrafico2.Add(
                    Punto(
                        c.NombreCategoria,
                        c.FacturacionTotal));
            }

            if (id == 0)
            {
                ReporteCategoria lider =
                    analisis.Categorias.FirstOrDefault();

                vista.TitulosIndicadores = new[]
                {
                    "CATEGORÍAS CON VENTAS",
                    "FACTURACIÓN TOTAL",
                    "UNIDADES VENDIDAS",
                    "PROMEDIO POR CATEGORÍA",
                    "CATEGORÍA LÍDER",
                    "PARTICIPACIÓN LÍDER"
                };

                vista.ValoresIndicadores = new[]
                {
                    Numero(analisis.Categorias.Count),

                    Moneda(analisis.FacturacionTotal),

                    Numero(analisis.UnidadesVendidas),

                    Moneda(
                        analisis.Categorias.Count > 0
                        ? analisis.FacturacionTotal /
                          analisis.Categorias.Count
                        : 0m),

                    lider == null
                        ? "-"
                        : lider.NombreCategoria,

                    Porcentaje(
                        lider == null
                        ? 0m
                        : lider.PorcentajeFacturacion)
                };
            }
            else
            {
                ReporteCategoria c =
                    categorias.FirstOrDefault();

                vista.TitulosIndicadores = new[]
                {
                    "FACTURACIÓN CATEGORÍA",
                    "UNIDADES VENDIDAS",
                    "PRODUCTOS VENDIDOS",
                    "PARTICIPACIÓN",
                    "CATEGORÍA",
                    "PERÍODO"
                };

                vista.ValoresIndicadores = c == null
                    ? new[]
                    {
                        "$ 0,00",
                        "0",
                        "0",
                        "0 %",
                        "-",
                        "-"
                    }
                    : new[]
                    {
                        Moneda(c.FacturacionTotal),

                        Numero(c.UnidadesVendidas),

                        Numero(c.ProductosConVentas),

                        Porcentaje(
                            c.PorcentajeFacturacion),

                        c.NombreCategoria,

                        desde.ToString("dd/MM") +
                        " - " +
                        hasta.ToString("dd/MM")
                    };
            }

            vista.Aviso =
                "Se incluyen únicamente categorías " +
                "con ventas durante el período.";

            return vista;
        }

        // =====================================================
        // 10. REPORTE DE SOLICITUDES
        // =====================================================


        /// <summary>
        /// Genera el reporte de solicitudes de stock.
        ///
        /// Recupera:
        /// 1. Las solicitudes pendientes actuales.
        /// 2. Todas las filas devueltas por el procedimiento
        ///    existente SP_ReporteMovimientosStock.
        ///
        /// No descarta registros por estados desconocidos
        /// ni por fechas que no puedan interpretarse.
        ///
        /// No modifica datos ni ejecuta SQL directamente.
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
                    "Solicitudes por estado",

                TituloGrafico2 =
                    "Unidades solicitadas por tipo",

                Grafico1EsCantidad = true,
                Grafico2EsCantidad = true
            };

            vista.Detalle = Tabla(
                "N° solicitud",
                "Fecha",
                "Producto",
                "Tipo",
                "Cantidad",
                "Estado",
                "Motivo"
            );

            // Se usa DateTime? para permitir fechas nulas
            // sin inventar una fecha de solicitud.
            var registros = new List<Tuple<
                int,
                DateTime?,
                string,
                string,
                int,
                string,
                string>>();

            // =================================================
            // 1. SOLICITUDES PENDIENTES
            // =================================================

            // Se recuperan todas las pendientes actuales.
            // No se descartan por fecha.
            List<SolicitudStock> pendientes =
                datosSolicitudes.ListarPendientes();

            foreach (SolicitudStock solicitud in pendientes)
            {
                registros.Add(Tuple.Create(
                    solicitud.IdSolicitud,

                    (DateTime?)solicitud.FechaSolicitud,

                    solicitud.oProducto != null
                        ? solicitud.oProducto.Nombre
                        : "Producto no identificado",

                    solicitud.TipoMovimiento ?? "",

                    solicitud.Cantidad,

                    "PENDIENTE",

                    solicitud.Motivo ?? ""
                ));
            }

            // =================================================
            // 2. CONSULTAR MOVIMIENTOS EXISTENTES
            // =================================================

            string mensajeMovimientos;

            DataTable movimientos =
                ObtenerReporteMovimientosStock(
                    desde,
                    hasta,
                    out mensajeMovimientos
                );

            int cantidadMovimientos =
                movimientos == null
                    ? 0
                    : movimientos.Rows.Count;

            // Evita duplicar solicitudes con ID válido.
            HashSet<int> idsAgregados = new HashSet<int>(
                registros
                    .Where(r => r.Item1 > 0)
                    .Select(r => r.Item1)
            );

            if (movimientos != null)
            {
                foreach (DataRow fila in movimientos.Rows)
                {
                    // Nombres reales que devuelve el
                    // procedimiento almacenado del proyecto.
                    int idSolicitud =
                        Entero(fila, "IdSolicitud");

                    if (idSolicitud > 0 &&
                        idsAgregados.Contains(idSolicitud))
                    {
                        continue;
                    }

                    string producto =
                        Texto(fila, "Producto");

                    string tipo =
                        Texto(fila, "TipoMovimiento");

                    int cantidad =
                        Entero(fila, "Cantidad");

                    string motivo =
                        Texto(fila, "Motivo");

                    string estado =
                        Texto(fila, "Estado")
                        .Trim()
                        .ToUpperInvariant();

                    // Normalizamos los estados conocidos,
                    // pero NO descartamos otros estados.
                    if (estado == "APROBADO")
                    {
                        estado = "APROBADA";
                    }
                    else if (estado == "RECHAZADO")
                    {
                        estado = "RECHAZADA";
                    }
                    else if (string.IsNullOrWhiteSpace(estado))
                    {
                        estado = "SIN ESTADO";
                    }

                    // El procedimiento devuelve la columna
                    // fecha_solicitud. Si una fecha no puede
                    // leerse, se conserva igualmente la fila.
                    DateTime fechaLeida;
                    DateTime? fechaSolicitud = null;

                    if (DateTime.TryParse(
                        Texto(fila, "fecha_solicitud"),
                        out fechaLeida))
                    {
                        fechaSolicitud = fechaLeida;
                    }

                    registros.Add(Tuple.Create(
                        idSolicitud,

                        fechaSolicitud,

                        producto,

                        tipo,

                        cantidad,

                        estado,

                        motivo
                    ));

                    if (idSolicitud > 0)
                    {
                        idsAgregados.Add(idSolicitud);
                    }
                }
            }

            // =================================================
            // 3. FILTRAR POR ESTADO
            // =================================================

            // El filtro "Todos los estados" muestra todas
            // las filas, incluso las de estado no reconocido.
            var seleccionadas = registros
                .Where(r =>
                    filtro == 0 ||
                    (filtro == 1 &&
                     r.Item6 == "PENDIENTE") ||
                    (filtro == 2 &&
                     r.Item6 == "APROBADA") ||
                    (filtro == 3 &&
                     r.Item6 == "RECHAZADA")
                )
                .OrderByDescending(r => r.Item2)
                .ToList();

            // =================================================
            // 4. TABLA DE RESULTADOS
            // =================================================

            foreach (var solicitud in seleccionadas)
            {
                string fechaTexto =
                    solicitud.Item2.HasValue
                        ? solicitud.Item2.Value.ToString(
                            "dd/MM/yyyy")
                        : "Sin fecha";

                vista.Detalle.Rows.Add(
                    solicitud.Item1 > 0
                        ? solicitud.Item1.ToString()
                        : "-",

                    fechaTexto,

                    solicitud.Item3,

                    solicitud.Item4,

                    Numero(solicitud.Item5),

                    solicitud.Item6,

                    solicitud.Item7
                );
            }

            // =================================================
            // 5. INDICADORES ESTADÍSTICOS
            // =================================================

            int total = seleccionadas.Count;

            int totalPendientes =
                seleccionadas.Count(r =>
                    r.Item6 == "PENDIENTE");

            int totalAprobadas =
                seleccionadas.Count(r =>
                    r.Item6 == "APROBADA");

            int totalRechazadas =
                seleccionadas.Count(r =>
                    r.Item6 == "RECHAZADA");

            int unidades =
                seleccionadas.Sum(r => r.Item5);

            string tipoMasFrecuente =
                seleccionadas
                    .Where(r =>
                        !string.IsNullOrWhiteSpace(r.Item4))
                    .GroupBy(r => r.Item4)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .FirstOrDefault() ?? "-";

            vista.TitulosIndicadores = new[]
            {
                "SOLICITUDES CONSULTADAS",
                "PENDIENTES",
                "APROBADAS",
                "RECHAZADAS",
                "UNIDADES SOLICITADAS",
                "TIPO MÁS FRECUENTE"
            };

            vista.ValoresIndicadores = new[]
            {
                Numero(total),

                Numero(totalPendientes),

                Numero(totalAprobadas),

                Numero(totalRechazadas),

                Numero(unidades),

                tipoMasFrecuente
            };

            // =================================================
            // 6. GRÁFICOS
            // =================================================

            foreach (var grupo in seleccionadas
                .GroupBy(r => r.Item6))
            {
                vista.PuntosGrafico1.Add(
                    Punto(
                        grupo.Key,
                        grupo.Count()
                    )
                );
            }

            foreach (var grupo in seleccionadas
                .Where(r =>
                    !string.IsNullOrWhiteSpace(r.Item4))
                .GroupBy(r => r.Item4))
            {
                vista.PuntosGrafico2.Add(
                    Punto(
                        grupo.Key,
                        grupo.Sum(r => r.Item5)
                    )
                );
            }

            // =================================================
            // 7. INFORMACIÓN Y CONTROL
            // =================================================

            vista.Aviso =
                "Solicitudes pendientes actuales y " +
                "movimientos del período consultado.";

            // Por ahora conservamos el diagnóstico
            // para comprobar las consultas.
            vista.Aviso +=
                " Pendientes: " + pendientes.Count +
                " | Movimientos: " + cantidadMovimientos +
                " | Mostradas: " + total + ".";

            if (!string.IsNullOrWhiteSpace(mensajeMovimientos))
            {
                vista.Aviso +=
                    " Error al consultar movimientos: " +
                    mensajeMovimientos;
            }

            if (total == 0 &&
                string.IsNullOrWhiteSpace(mensajeMovimientos))
            {
                vista.Aviso +=
                    " No se encontraron solicitudes " +
                    "con los filtros seleccionados.";
            }

            return vista;
        }


        // =====================================================
        // 11. COMPETENCIA ENTRE VENDEDORES
        // =====================================================

        /// <summary>
        /// Compara a los vendedores que realizaron ventas.
        ///
        /// La competencia se ordena por:
        /// 1. Mayor facturación.
        /// 2. Mayor cantidad de operaciones.
        /// 3. Apellido y nombre, para desempatar.
        ///
        /// Calcula ranking, ticket promedio,
        /// participación y distancia con el líder.
        ///
        /// Al seleccionar un vendedor, los gráficos
        /// continúan mostrando a todos para permitir
        /// una comparación real.
        /// </summary>
        private VistaReporte ReporteVendedores(
            DateTime desde,
            DateTime hasta,
            int id)
        {
            List<Usuario> usuarios =
                datosUsuarios.Listar()

                .Where(u => u.IdRol == 2)

                .OrderBy(u => u.Apellido)

                .ThenBy(u => u.Nombre)

                .ToList();

            List<DetalleVentaAnalisis> detalles =
                datos.ObtenerDetallesVentasAnalisis(
                    desde, hasta, null);

            // Cada fila contiene:
            // Usuario, facturación, ventas y unidades.
            var participantes =
                new List<Tuple<
                    Usuario, decimal, int, int>>();

            foreach (Usuario usuario in usuarios)
            {
                List<ReporteVenta> ventas =
                    ObtenerVentasAnalisis(
                        desde,
                        hasta,
                        usuario.IdUsuario);

                if (ventas.Count == 0)
                    continue;

                participantes.Add(
                    Tuple.Create(
                        usuario,

                        ventas.Sum(v => v.Total),

                        ventas.Count,

                        detalles
                            .Where(d =>
                                d.IdUsuario ==
                                usuario.IdUsuario)
                            .Sum(d => d.Cantidad)));
            }

            // Ranking competitivo por facturación.
            var ranking = participantes

                .OrderByDescending(v => v.Item2)

                .ThenByDescending(v => v.Item3)

                .ThenBy(v => v.Item1.Apellido)

                .ThenBy(v => v.Item1.Nombre)

                .ToList();

            decimal totalFacturado =
                ranking.Sum(v => v.Item2);

            decimal importeLider =
                ranking.Count > 0
                    ? ranking[0].Item2
                    : 0m;

            decimal promedioFacturacion =
                ranking.Count > 0
                    ? totalFacturado / ranking.Count
                    : 0m;

            VistaReporte vista = new VistaReporte
            {
                Seccion = "Vendedores",

                TituloGrafico1 =
                    "COMPETENCIA: facturación por vendedor",

                TituloGrafico2 =
                    "COMPETENCIA: ventas realizadas",

                Grafico2EsCantidad = true
            };

            vista.Detalle = Tabla(
                "Puesto",
                "Vendedor",
                "Ventas",
                "Unidades",
                "Facturación",
                "Ticket promedio",
                "% del total",
                "Detrás del líder");

            for (int i = 0; i < ranking.Count; i++)
            {
                var vendedor = ranking[i];

                string nombre =
                    vendedor.Item1.Apellido +
                    ", " +
                    vendedor.Item1.Nombre;

                decimal ticket =
                    vendedor.Item3 > 0
                        ? vendedor.Item2 /
                          vendedor.Item3
                        : 0m;

                decimal distancia =
                    importeLider - vendedor.Item2;

                string puesto =
                    (i + 1).ToString() + "°";

                if (i == 0)
                    puesto += " LÍDER";

                if (i == 1)
                    puesto += " SEGUNDO";

                if (i == 2)
                    puesto += " TERCERO";

                vista.Detalle.Rows.Add(
                    puesto,

                    nombre,

                    Numero(vendedor.Item3),

                    Numero(vendedor.Item4),

                    Moneda(vendedor.Item2),

                    Moneda(ticket),

                    Porcentaje(
                        Participacion(
                            vendedor.Item2,
                            totalFacturado)),

                    Moneda(distancia));

                // El puesto aparece en el gráfico.
                vista.PuntosGrafico1.Add(
                    Punto(
                        (i + 1) + "° " + nombre,
                        vendedor.Item2));

                vista.PuntosGrafico2.Add(
                    Punto(
                        (i + 1) + "° " + nombre,
                        vendedor.Item3));
            }

            // ---------------------------------------------
            // TODOS: PODIO DE LOS TRES PRIMEROS
            // ---------------------------------------------

            if (id == 0)
            {
                string primero =
                    ranking.Count > 0
                    ? ranking[0].Item1.Apellido +
                      ", " +
                      ranking[0].Item1.Nombre
                    : "-";

                string segundo =
                    ranking.Count > 1
                    ? ranking[1].Item1.Apellido +
                      ", " +
                      ranking[1].Item1.Nombre
                    : "Sin rival";

                string tercero =
                    ranking.Count > 2
                    ? ranking[2].Item1.Apellido +
                      ", " +
                      ranking[2].Item1.Nombre
                    : "Sin tercer puesto";

                vista.TitulosIndicadores = new[]
                {
                    "1° PUESTO - LÍDER",
                    "2° PUESTO",
                    "3° PUESTO",
                    "FACTURACIÓN TOTAL",
                    "DIFERENCIA 1° Y 2°",
                    "VENDEDORES CON VENTAS"
                };

                vista.ValoresIndicadores = new[]
                {
                    primero,

                    segundo,

                    tercero,

                    Moneda(totalFacturado),

                    ranking.Count > 1
                        ? Moneda(
                            importeLider -
                            ranking[1].Item2)
                        : "Sin rival",

                    Numero(ranking.Count)
                };
            }
            else
            {
                // -----------------------------------------
                // VENDEDOR INDIVIDUAL: SU POSICIÓN
                // -----------------------------------------

                var elegido =
                    ranking.FirstOrDefault(v =>
                        v.Item1.IdUsuario == id);

                int posicion =
                    ranking.FindIndex(v =>
                        v.Item1.IdUsuario == id);

                vista.TitulosIndicadores = new[]
                {
                    "PUESTO EN EL RANKING",
                    "FACTURACIÓN",
                    "VENTAS REALIZADAS",
                    "PARTICIPACIÓN",
                    "DISTANCIA AL LÍDER",
                    "VS PROMEDIO DE VENDEDORES"
                };

                if (elegido == null)
                {
                    vista.ValoresIndicadores = new[]
                    {
                        "Sin ventas",
                        "$ 0,00",
                        "0",
                        "0 %",
                        "-",
                        "-"
                    };
                }
                else
                {
                    decimal diferenciaPromedio =
                        elegido.Item2 -
                        promedioFacturacion;

                    vista.ValoresIndicadores = new[]
                    {
                        (posicion + 1) +
                        "° de " + ranking.Count,

                        Moneda(elegido.Item2),

                        Numero(elegido.Item3),

                        Porcentaje(
                            Participacion(
                                elegido.Item2,
                                totalFacturado)),

                        Moneda(
                            importeLider -
                            elegido.Item2),

                        (diferenciaPromedio >= 0m
                            ? "+"
                            : "-") +
                        Moneda(
                            Math.Abs(
                                diferenciaPromedio))
                    };
                }
            }

            vista.Aviso =
                "Ranking por facturación del " +
                desde.ToString("dd/MM/yyyy") +
                " al " +
                hasta.ToString("dd/MM/yyyy") +
                ". Los gráficos comparan a todos " +
                "los vendedores con ventas.";

            return vista;
        }

        // =====================================================
        // 12. REPORTE DE CLIENTES
        // =====================================================

        /// <summary>
        /// Mantiene la consulta de clientes.
        ///
        /// Se utilizan las compras que puedan
        /// relacionarse con el campo Cliente
        /// del reporte de ventas existente.
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
                    "Se muestran los clientes, " +
                    "pero no se pudieron consultar " +
                    "las compras: " + ex.Message;
            }

            decimal total = 0m;
            int operaciones = 0;
            int clientesConCompras = 0;

            foreach (Cliente c in elegidos)
            {
                List<ReporteVenta> compras =
                    comprasDisponibles

                    ? ventas.Where(v =>
                        v.Cliente ==
                            c.IdCliente.ToString() ||

                        v.Cliente ==
                            c.Nombre + " " +
                            c.Apellido ||

                        v.Cliente ==
                            c.Apellido + ", " +
                            c.Nombre)

                        .ToList()

                    : new List<ReporteVenta>();

                int cantidad = compras.Count;

                decimal facturacion =
                    compras.Sum(v => v.Total);

                if (cantidad > 0)
                    clientesConCompras++;

                total += facturacion;
                operaciones += cantidad;

                string nombre =
                    c.Apellido +
                    ", " +
                    c.Nombre;

                vista.Detalle.Rows.Add(
                    nombre,

                    c.Dni.ToString(),

                    comprasDisponibles
                        ? Numero(cantidad)
                        : "Sin dato",

                    comprasDisponibles
                        ? Moneda(facturacion)
                        : "Sin dato",

                    comprasDisponibles
                        ? Moneda(
                            cantidad == 0
                            ? 0m
                            : facturacion / cantidad)
                        : "Sin dato");

                if (comprasDisponibles)
                {
                    vista.PuntosGrafico1.Add(
                        Punto(nombre, cantidad));

                    vista.PuntosGrafico2.Add(
                        Punto(nombre, facturacion));
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
                    ? Numero(clientesConCompras)
                    : "Sin dato",

                comprasDisponibles
                    ? Numero(operaciones)
                    : "Sin dato",

                comprasDisponibles
                    ? Moneda(total)
                    : "Sin dato",

                comprasDisponibles
                    ? Moneda(
                        operaciones == 0
                        ? 0m
                        : total / operaciones)
                    : "Sin dato",

                comprasDisponibles
                    ? Numero(
                        elegidos.Count -
                        clientesConCompras)
                    : "Sin dato"
            };

            if (comprasDisponibles)
            {
                vista.Aviso =
                    "Ventas asociadas mediante el campo " +
                    "Cliente del reporte existente.";
            }

            return vista;
        }
    }
}
