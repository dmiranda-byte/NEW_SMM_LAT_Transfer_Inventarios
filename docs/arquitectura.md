# Arquitectura detallada

## Visión general
La solución es una **aplicación ASP.NET Web Forms** orientada a la gestión de transferencias de inventario. La arquitectura es monolítica en un solo proyecto web, con capas lógicas separadas por convención:

- **Presentación**: páginas `.aspx` y `SiteMaster.master`.
- **Aplicación/Negocio**: clases en `App_Code/`.
- **Datos**: ADO.NET con `SqlDb`/`DFBUYINGdb` y consultas SQL/Stored Procedures.
- **Integraciones**: acceso directo a tablas SAP B1 y vistas WMS.

Diagrama de componentes: **[diagrama-componentes.md](diagrama-componentes.md)**.

## Capas y componentes

### 1) Presentación (Web Forms)
- **Páginas funcionales**: `Transfers.aspx`, `CreateTransfer.aspx`, `TransferDetails.aspx`, `TransferDelete.aspx`, etc.
- **Reportes**: páginas `R_*.aspx` (kardex, inventario, bines, órdenes).
- **Administración**: `MaintainFAQ.aspx`, `MaintainVideos*.aspx`, `PreguntasFrecuentes.aspx`.

Patrones de UI:
- `Page_Load` controla autorización y sesión (ej. `CreateTransfer.aspx.cs`).
- Uso de `Session["UserId"]`, `Session["CompanyId"]`.
- Componentes Telerik (`Telerik.Web.UI`) configurados en `Web.config`.

### 2) Lógica de negocio (App_Code)
Principales clases:

| Clase | Responsabilidad principal |
| --- | --- |
| `SqlDb` | Conexión a SQL Server y utilidades comunes (roles, búsquedas). |
| `DFBUYINGdb` | Conexión a la BD `DFBUYING` (SQL Server). |
| `Transfer` | Consultas y lógica de transferencias y borradores. |
| `DataManager` | Consultas transversales (órdenes, bines, inventario). |
| `Reports`/`PagedReport` | Preparación de reportes y paginación. |
| `Admin` | Administración de usuarios y accesos. |
| `WMS` | Consultas específicas de bines/WMS. |

### 3) Persistencia y SQL
- **Conexiones**: definidas en `Web.config` (`smm_latConnectionString`, `DFBUYING`).
- **Acceso**: ADO.NET mediante `SqlConnection`, `SqlCommand`, `SqlDataAdapter`.
- **Consultas complejas**: centralizadas en `App_Code/Queries.cs` (CTEs con `WITH`).
- **Stored Procedures**: por ejemplo `SISINV_GET_ACCESSTYPE_PRC` (control de accesos).

### 4) Integración con SAP B1 y WMS
- Tablas SAP consultadas directamente: `OITM`, `OWHS`, `OITB`, `ITM1`, `OBCD`.
- Vistas WMS: `Wms_Whs_Item_Bin_vw` (bines por artículo/tienda).
- Tablas internas de transferencia: `SMM_ODRF`, `SMM_DRF1`, `smm_Transdiscrep_*`, `SMM_DRAFT_VW`.

## Flujos principales

### A) Creación de transferencia
1. Usuario accede a `CreateTransfer.aspx`.
2. Se valida sesión y rol (`SISINV_GET_ACCESSTYPE_PRC`).
3. Se consultan bodegas y grupos de artículos.
4. Se genera borrador (tablas `SMM_ODRF`, `SMM_DRF1`).

### B) Despacho/recepción
1. `Transfers.aspx` lista borradores.
2. `TransferDetails.aspx` muestra detalle y genera impresión.
3. Se actualiza estado en `smm_Transdiscrep_odrf`.

### C) Auditoría y discrepancias
1. `Transfers_Audit*.aspx` consolida históricos.
2. Se comparan borradores vs. movimientos reales.
3. Se registran inconsistencias (`smm_Transdiscrep_odrf_bads`, `smm_TransXsap_odrf_bads`).

Ver diagramas en **[diagrama-flujo.md](diagrama-flujo.md)** y **[diagrama-bd.md](diagrama-bd.md)**.

## Seguridad
- Autenticación: **Windows Authentication** (IIS).
- Autorización: roles y permisos por pantalla (`SISINV_GET_ACCESSTYPE_PRC`).
- Sesiones: controladas vía `Session`.

## Puntos de extensión recomendados
- Nueva funcionalidad: crear página Web Forms + clase en `App_Code`.
- Consultas complejas: agregar CTE/SQL en `Queries.cs`.
- Reportes: seguir patrón de páginas `R_*.aspx` y `Reports.cs`.

Más detalles en **[manual-tecnico.md](manual-tecnico.md)**.
