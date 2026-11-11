# Aura Beauty

## Sistema de escritorio para una tienda de maquillaje

**Aura Beauty** es una aplicación de escritorio desarrollada en C# y Windows Forms, sobre .NET Framework 4.7.2, con persistencia de datos en Microsoft SQL Server LocalDB mediante ADO.NET.

El proyecto implementa una arquitectura de cuatro capas y reúne las operaciones de una tienda: administración de usuarios, catálogo, clientes, stock, ventas, consultas, reportes y respaldos de la base de datos.

**Tecnologías:** C# · .NET Framework 4.7.2 · Windows Forms · SQL Server LocalDB · ADO.NET · PDFsharp-GDI · Git

**Contenido**
- [Funcionalidades](#funcionalidades)
- [Roles y permisos](#roles-y-permisos)
- [Ventas y control de stock](#ventas-y-control-de-stock)
- [Respaldos y restauración](#respaldos-y-restauración)
- [Arquitectura](#arquitectura)
- [Requisitos](#requisitos)
- [Instalación y ejecución](#instalación-y-ejecución)
- [Estructura del repositorio](#estructura-del-repositorio)
- [Consideraciones técnicas](#consideraciones-técnicas)
- [Finalidad académica](#finalidad-académica)

## Funcionalidades
| Modulo                  |                                         Funciones principales                                                        |
|-------------------------|----------------------------------------------------------------------------------------------------------------------|
| Usuarios y roles        | Inicio de sesión, altas, modificaciones, bajas lógicas, reactivación y permisos por rol.                             |
| Productos y categorías  | Gestión del catálogo, búsqueda, filtros, bajas lógicas y alertas de stock bajo.                                      |
| Stock                   | Consulta de existencias y solicitudes justificadas de ingreso o egreso, sujetas a autorización.                      |
| Clientes                | Registro, edición y búsqueda de clientes; ventas a clientes registrados o a Consumidor Final.                        |
| Ventas                  | Carrito, cálculo de totales, reservas temporales de productos, confirmación transaccional y comprobantes PDF.        |
| Mis Ventas              | Historial de ventas del vendedor autenticado, con filtros por fechas.                                                |
| Cierre de Caja          | Resumen diario de operaciones y total vendido por el vendedor autenticado.                                           |
| Consultas               | Historial de ventas y movimientos de stock, con filtros y exportación a PDF.                                         |
| Reportes y estadísticas | Análisis de ventas, productos, categorías, solicitudes, vendedores y clientes, con indicadores y gráficos.           |
| Centro de Respaldos     | Creación de archivos .bak, historial, solicitudes de restauración, autorización por otro administrador y auditoría.  |

### Reglas de negocio destacadas
- Los productos con 5 unidades o menos se identifican como stock bajo.
- Las bajas de usuarios, productos y categorías son lógicas, preservando los registros históricos.
- Una categoría con productos activos asociados no puede darse de baja.
- Una venta puede asociarse a un cliente registrado o realizarse como Consumidor Final, sin crear un cliente ficticio.
- El stock no se modifica directamente desde el módulo de solicitudes: los movimientos requieren un motivo y una resolución administrativa.

## Roles y permisos
La interfaz presenta las opciones disponibles según el rol del usuario autenticado.
 
|             Administrador	                 |                Vendedor                  |                           Repositor                           |
|--------------------------------------------|------------------------------------------|---------------------------------------------------------------|
| Gestiona usuarios y roles.	                | Consulta productos.	                     | Gestiona el catálogo de productos y categorías.               |
| Consulta el catálogo.	                     | Registra ventas y gestiona clientes.	    | Consulta existencias y solicita movimientos de stock.         |
| Aprueba o rechaza solicitudes de stock.	   | Consulta Mis Ventas y el Cierre de Caja.	| Sus solicitudes de stock requieren resolución administrativa. |
| Accede a consultas, reportes y respaldos.  |	No administra respaldos ni usuarios.	    | No registra ventas ni administra usuarios.                    |

**Separación de responsabilidades:** en esta versión, el **Administrador no registra ventas ni edita directamente productos o existencias.** Los vendedores realizan operaciones comerciales; los repositores gestionan el catálogo y solicitan movimientos de stock.

## Ventas y control de stock
### Punto de venta
El módulo de ventas permite agregar y quitar productos del carrito, modificar cantidades, consultar disponibilidad, calcular subtotales y registrar el total de la operación.

Para reducir conflictos entre vendedores que operan al mismo tiempo, el sistema utiliza reservas temporales de productos (vigencia configurada de 10 minutos). La confirmación se realiza mediante un procedimiento almacenado que registra la cabecera y el detalle de la venta, actualiza el stock y consolida las reservas dentro de una transacción.

Si falla la confirmación, la transacción permite revertir los cambios correspondientes.

### Comprobantes
Luego de registrar una venta, la aplicación puede generar un comprobante PDF con el identificador interno, fecha, vendedor, cliente o Consumidor Final, detalle de productos, cantidades, precios y total.

La generación utiliza PDFsharp-GDI. Los comprobantes son internos y no constituyen documentación fiscal electrónica oficial.

### Solicitudes de stock
El repositor puede solicitar ingresos o egresos de unidades e indicar el motivo. El administrador consulta las solicitudes pendientes y decide su aprobación o rechazo. Una aprobación actualiza las existencias bajo las reglas del sistema.

## Respaldos y restauración
El Centro de Respaldos, disponible para administradores, permite:
1. Crear y registrar copias de seguridad completas de AuraBeautyDB en archivos .bak.
2. Consultar el historial y solicitar la restauración de un respaldo existente.
3. Presentar las solicitudes pendientes a otro administrador; el solicitante no puede autorizar su propia solicitud.
4. Validar la contraseña del administrador que autoriza, verificar el respaldo y generar una copia preventiva del estado actual.
5. Restaurar AuraBeautyDB y registrar la ejecución en una base administrativa independiente.

### Dos bases de datos
| Base        	      |                                   Propósito                                                                     |
|--------------------|-----------------------------------------------------------------------------------------------------------------|
| AuraBeautyDB	      | Información operativa: usuarios, catálogo, stock, clientes, ventas y solicitudes.                               |
| AuraBeautyDB_Admin | Auditoría de restauraciones, separada de la base operativa para conservar el registro cuando esta se reemplaza. |


La auditoría almacena el estado de la ejecución y datos de identificación de la solicitud para detectar intentos de procesamiento duplicado.

**[!CAUTION]**
Restaurar reemplaza el contenido actual de AuraBeautyDB por el contenido del respaldo seleccionado. Antes de autorizar una restauración, deben cerrarse las demás sesiones, verificarse el archivo y conservarse la copia preventiva. Esta funcionalidad incluye validaciones del servidor y de las rutas físicas de los archivos de base de datos configuradas para el entorno de desarrollo; no debe ejecutarse en otra computadora ni en un entorno con datos importantes sin adaptar y probar previamente esas validaciones. No es una operación de importación parcial.

## Arquitectura
La solución separa responsabilidades en cuatro proyectos:

<img width="846" height="912" alt="image" src="https://github.com/user-attachments/assets/f7f34db1-9e03-4172-adcf-70b7f98402b0" />


|        Capa	         |                                       Responsabilidad                                        |
|----------------------|----------------------------------------------------------------------------------------------|
| Presentacion	        | Formularios, navegación, controles visuales e interacción con el usuario.                    |
| Negocio	             | Validaciones, reglas comerciales y coordinación de operaciones.                              |
| Datos	               | Conexión a SQL Server, consultas parametrizadas, procedimientos almacenados y transacciones. |
| Entidades	           | Modelos de usuarios, roles, productos, reservas, ventas, solicitudes y reportes.             |

## Requisitos
- Windows.
- Visual Studio con soporte para soluciones .slnx y desarrollo de escritorio con .NET.
- .NET Framework 4.7.2, incluido su paquete de desarrollo (Developer Pack) para compilar.
- SQL Server LocalDB.
- SQL Server Management Studio (SSMS) para ejecutar y revisar scripts SQL.
- Acceso a restauración de paquetes NuGet (incluido PDFsharp-GDI).

## Instalación y ejecución
### 1. Obtener el proyecto
Cloná el repositorio o descargá su contenido desde GitHub.
Abrí la solución:
```text
Aura_Beauty.slnx
```

### 2. Configurar SQL Server LocalDB
En SSMS, conectate mediante Autenticación de Windows a:
```text
(localdb)\MSSQLLocalDB
```

La conexión de la aplicación está definida en:
```text
Datos/Conexion.cs
```
El catálogo operativo utilizado es AuraBeautyDB. Si tu instancia tiene otro nombre, primero deberás ajustar y comprobar la configuración de conexión.

### 3. Preparar la base de datos
El script completo se encuentra en:
```text
BaseDeDatos/AuraBeautyDB_Instalacion_Completa_2026.sql
```
**[!WARNING]
IMPORTANTE:** este script detiene la ejecucion si ```AuraBeautyDB``` ya existe. REQUIERE SSMS CON Query > SQLCMD Mode ACTIVADO.

El archivo incluye estructura y registros de demostración. Los módulos incorporados posteriormente requieren objetos complementarios que también están en ```BaseDeDatos/```:

| Orden lógico |         Archivos                                                   |	                    Qué incorporan                                 |
|--------------|--------------------------------------------------------------------|--------------------------------------------------------------------|
|      1	      | SQLQuery1.sql	                                                     | Tabla de reservas de productos.
|      2	      | SQLQuery4.sql	                                                     | Tipo tabular utilizado para confirmar ventas.
|      3	      | SQLQuery2.sql, SQLQuery3.sql, SQLQuery5.sql	                       | Procedimientos de reservas y confirmación de ventas.
|      4	      | SQLQuery6.sql	                                                     | Procedimientos adicionales de reportes.
|      5	      | SQLQuery7.sql	                                                     | Tablas de backups y solicitudes de restauración.
|      6	      | SQLQuery8.sql, SQLQuery9.sql	                                      | Registro y consulta del historial de respaldos.
|      7	      | SP_BackUp_ValidarAutorizador.sql, SP_Backup_ValidarClaveSesion.sql | Validaciones administrativas del módulo de respaldos.
|      8	      | SP_Restore_Solicitar.sql, SP_Restore_Autorizar.sql, SP_Restore_Rechazar.sql, SP_Restore_ListarPendientes.sql |	Solicitudes, autorizaciones, rechazos y        consultas de restauración. El último archivo incluye también los procedimientos de historial y consulta de estado.
|      9	      | 01_Crear_BaseAdministrativa_Auditoria.sql                          |	Creación de AuraBeautyDB_Admin y su tabla de auditoría.
|      10	     | 02_Actualizar_Auditoria_Solicitudes.sql	                           | Columnas e índice de identificación de solicitudes.

**Atención con la instalación desde cero:** ```SQLQuery2.sql```, ```SQLQuery3.sql``` y ```SQLQuery5.sql``` se encuentran exportados con ```ALTER PROCEDURE```. En una base recién creada, esos procedimientos todavía no existen, por lo que **hay que adaptar sus encabezados a CREATE OR ALTER PROCEDURE antes de ejecutarlos por primera vez**. El resto de los scripts también debe verificarse sobre la versión concreta de SQL Server antes de usarlo fuera del entorno de desarrollo.

**Para actualizar una base existente**, aplicá únicamente los cambios que falten, conservando sus datos. Los scripts ```01_...``` y ```02_...``` de auditoría contienen comprobaciones de existencia, pero no sustituyen un plan de migración revisado.

### 4. Compilar y ejecutar
1. Abrí ```Aura_Beauty.slnx``` en Visual Studio.
2. Restaurá los paquetes NuGet si Visual Studio lo solicita.
3. Establecé Presentacion como proyecto de inicio.
4. Ejecutá Compilar → Recompilar solución.
5. Iniciá la aplicación con F5.

## Estructura del repositorio
```text
TallerII/
├── Aura_Beauty.slnx
├── Entidades/             # Modelos del dominio
├── Datos/                 # Acceso a SQL Server
├── Negocio/               # Reglas y validaciones
├── Presentacion/          # Interfaz Windows Forms y PDF
├── BaseDeDatos/           # Script inicial y scripts complementarios
└── README.md
```

## Consideraciones técnicas
**- Alcance académico:** el sistema permite demostrar permisos por rol, operaciones concurrentes, procedimientos almacenados y control transaccional. No debe asumirse listo para producción sin pruebas adicionales.
**- Contraseñas:** el proyecto utiliza el esquema de autenticación implementado para la práctica, que compara contraseñas almacenadas sin un mecanismo moderno de hash. Para uso real se requiere migrar a almacenamiento seguro de contraseñas y revisar la gestión de credenciales.
**- Datos de ejemplo:** el script inicial incluye registros de demostración. Revisá y reemplazá las cuentas y credenciales de prueba antes de compartir o utilizar una instalación.
**- Portabilidad de restauraciones:** ```CD_RestauracionSegura.cs``` verifica el entorno y rutas físicas concretas de la máquina de desarrollo. Es una medida de protección que no debe eliminarse simplemente para hacer funcionar la restauración en otra PC; debe adaptarse y validarse sobre un entorno de pruebas independiente.
**- Archivos de respaldo:** publiques archivos ```.bak```, ```.mdf``` o ```.ldf``` con información de la aplicación. Conservá los respaldos fuera del repositorio y verificá las exclusiones de Git antes de hacer push.
**- PDF:** los comprobantes y reportes exportados son documentos internos del sistema, no facturas fiscales oficiales.

## Finalidad académica
Aura Beauty fue desarrollado como proyecto académico para integrar programación orientada a objetos, arquitectura en capas, bases de datos relacionales, ADO.NET, procedimientos almacenados, transacciones, concurrencia, interfaces gráficas, permisos por rol, reportes y control de versiones con Git y GitHub.

**Aura Beauty** · Proyecto académico de gestión comercial
