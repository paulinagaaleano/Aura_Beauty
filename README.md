# Aura Beauty

## Sistema de Gestión Comercial para Tienda de Maquillaje

**Aura Beauty** es una aplicación de escritorio desarrollada en **C# con .NET Framework y Windows Forms**, utilizando **SQL Server LocalDB** como sistema de base de datos.

El sistema fue desarrollado aplicando una **arquitectura en capas**, separando las responsabilidades de Presentación, Negocio, Acceso a Datos y Entidades.

---
# Instalación y ejecución

## 1. Clonar el repositorio

Clonar el proyecto mediante Git o descargar el repositorio.

## 2. Requisitos

- Windows.
- Visual Studio.
- .NET Framework 4.7.2.
- SQL Server LocalDB.
- SQL Server Management Studio (SSMS), recomendado para instalar y administrar la base de datos.

## 3. Crear la base de datos

Abrir **SQL Server Management Studio (SSMS)**.

Conectarse al servidor:

```text
(localdb)\MSSQLLocalDB
```

utilizando autenticación de Windows. Luego abrir el archivo:

```text
BaseDeDatos/AuraBeautyDB.sql
```

y ejecutar el script completo. Esto creará la base:

```text
AuraBeautyDB
```

junto con sus tablas y datos iniciales.

> Si ya existe una base de datos llamada `AuraBeautyDB`, verificarla antes de ejecutar el script para evitar conflictos con una base existente.

## 4. Abrir la solución

Abrir el archivo de solución de **Aura Beauty** en Visual Studio.

Si Visual Studio solicita restaurar los paquetes NuGet, aceptar la restauración.

## 5. Verificar proyecto de inicio

El proyecto de inicio debe ser:

```text
Presentacion
```

## 6. Compilar y Ejecutar
 
---

## Funcionalidades principales

El sistema permite gestionar las principales operaciones comerciales de una tienda de productos de maquillaje.

### Inicio de sesión y roles

El acceso al sistema se realiza mediante correo electrónico y contraseña.

Se implementaron tres roles:

- **Administrador**
  - Gestión de usuarios y roles.
  - Gestión de productos y categorías.
  - Gestión de stock.
  - Gestión de clientes.
  - Registro de ventas.
  - Consulta de reportes.

- **Vendedor**
  - Consulta de productos.
  - Gestión de clientes.
  - Registro de ventas.

- **Repositor**
  - Gestión del catálogo de productos.
  - Gestión y actualización de stock.

---

## Productos y categorías

El sistema permite:

- Registrar productos.
- Editar productos.
- Realizar bajas lógicas.
- Clasificar los productos por categoría.
- Buscar y filtrar productos.
- Controlar el stock disponible.
- Identificar visualmente productos con stock bajo.

Se definió un umbral de **5 unidades** para generar la alerta visual de stock bajo.

Las categorías también utilizan baja lógica.

Además, una categoría que tenga productos activos asociados no puede darse de baja, evitando inconsistencias en el catálogo.

---

## Gestión de stock

El módulo de stock permite:

- Consultar las existencias de los productos.
- Agregar unidades.
- Quitar unidades.
- Buscar productos.
- Filtrar por categoría.
- Visualizar alertas de stock bajo.

El stock también se actualiza automáticamente cuando se registra una venta.

---

## Gestión de clientes

El sistema permite registrar y editar clientes con información como:

- DNI.
- Nombre.
- Apellido.
- Domicilio.
- Correo electrónico.
- Fecha de nacimiento.

También permite buscar clientes por DNI o apellido.

Las ventas pueden realizarse a un cliente registrado o como **Consumidor Final**.

En este último caso no se crea un cliente ficticio en la base de datos: la venta se registra sin un cliente específico.

---

## Ventas

El módulo de ventas funciona como punto de venta (POS).

Permite:

- Buscar productos.
- Seleccionar productos.
- Indicar cantidades.
- Agregar productos al carrito.
- Aumentar o disminuir cantidades.
- Quitar productos del carrito.
- Verificar disponibilidad de stock.
- Calcular subtotales.
- Calcular el total de la venta.
- Seleccionar un cliente registrado o Consumidor Final.
- Registrar el vendedor que realizó la operación.

Al confirmar una venta se registran la cabecera y sus detalles y se descuenta automáticamente el stock correspondiente.

El registro de la venta y la actualización del stock se realizan dentro de una **transacción**, permitiendo revertir la operación si ocurre un error durante el proceso.

---

## Comprobantes PDF

Después de registrar una venta, Aura Beauty puede generar un comprobante en formato PDF.

El comprobante contiene:

- Número interno del comprobante.
- Fecha.
- Tipo de comprobante.
- Vendedor.
- Cliente o Consumidor Final.
- Productos.
- Cantidades.
- Precios.
- Subtotales.
- Total de la venta.

La generación de PDF utiliza la biblioteca **PDFsharp**.

> El número generado por el sistema corresponde a un identificador interno del comprobante y no constituye numeración fiscal oficial.

---

## Reportes

El sistema dispone de un módulo de reportes de ventas.

Permite consultar ventas:

- Por rango de fechas.
- Por vendedor.
- Para todos los vendedores.

El reporte muestra información de cada operación y calcula:

- Cantidad de ventas.
- Total vendido.

---

# Arquitectura del proyecto

Aura Beauty utiliza una arquitectura organizada en capas.

### Entidades

Contiene las clases que representan y transportan los datos utilizados por el sistema.

Ejemplos:

- Usuario
- Rol
- Categoria
- Producto
- Cliente
- VentaCabecera
- VentaDetalle
- ReporteVenta

### Datos

Es responsable del acceso a SQL Server.

Contiene:

- Conexión a la base de datos.
- Consultas SQL.
- Altas y modificaciones.
- Bajas lógicas.
- Lectura de registros.
- Transacciones.
- Actualización de stock.

Las consultas utilizan parámetros para evitar concatenar directamente los valores ingresados por el usuario.

### Negocio

Contiene las reglas y validaciones del sistema.

Por ejemplo:

- Validaciones de usuarios.
- Validaciones de clientes.
- Validaciones de productos.
- Control de cantidades y stock.
- Reglas de categorías.
- Validación de ventas.
- Validación de filtros de reportes.

### Presentación

Contiene la interfaz gráfica desarrollada con **Windows Forms**.

La capa de Presentación utiliza las capas de Negocio y Entidades y no realiza consultas SQL directamente.

---


# Base de datos

El proyecto utiliza:

```text
SQL Server LocalDB
```

Base de datos:

```text
AuraBeautyDB
```

Servidor utilizado durante el desarrollo:

```text
(localdb)\MSSQLLocalDB
```

La cadena de conexión se encuentra configurada en la capa **Datos**.

---

# Tecnologías utilizadas

- C#
- .NET Framework 4.7.2
- Windows Forms
- SQL Server
- SQL Server LocalDB
- ADO.NET
- PDFsharp
- Git
- GitHub

---

# Finalidad académica

Aura Beauty fue desarrollado como proyecto académico con el objetivo de aplicar de manera integrada:

- Programación orientada a objetos.
- Arquitectura en capas.
- Acceso a bases de datos relacionales.
- Reglas de negocio.
- Interfaces gráficas de escritorio.
- Gestión de usuarios y permisos.
- Operaciones comerciales.
- Transacciones.
- Generación de comprobantes.
- Reportes.
- Control de versiones mediante Git.