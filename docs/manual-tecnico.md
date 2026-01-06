# Manual técnico (desarrolladores)

## Tecnologías
- **ASP.NET Web Forms** (.NET Framework 4.8)
- **SQL Server** (tablas SMM y SAP B1)
- **Telerik Web UI**
- ADO.NET (`SqlConnection`, `SqlCommand`, `SqlDataAdapter`)

## Organización del código
- `App_Code/` contiene la lógica compartida y acceso a datos.
- Las páginas `.aspx` usan code-behind `.aspx.cs`.
- El control de permisos se realiza con el SP `SISINV_GET_ACCESSTYPE_PRC`.

## Flujo típico de una página
1. `Page_Load` valida sesión/rol.
2. Se obtienen datos mediante clases de `App_Code/`.
3. Se renderiza HTML o controles Telerik.

Ejemplo: `CreateTransfer.aspx.cs` valida rol y carga bodegas antes de permitir crear borradores.

## Acceso a datos
- `SqlDb` maneja la conexión principal (`smm_latConnectionString`).
- `DFBUYINGdb` maneja conexión secundaria (`DFBUYING`).
- `Queries.cs` centraliza CTEs y consultas complejas.

### Buenas prácticas al extender
- Reusar `SqlDb` para nuevas consultas.
- Agregar nuevos CTEs/consultas en `Queries.cs` si son complejas.
- Mantener validación de `Session["UserId"]` y `Session["CompanyId"]`.

## Reportes
- Las páginas `R_*.aspx` usan consultas directas y `Reports.cs`.
- Para nuevos reportes, seguir el patrón de carga de filtros y renderizado.

## Seguridad y sesiones
- Windows Authentication (IIS) + roles internos.
- Si falta conexión o sesión, se redirige a `AccessDenied.aspx` o `Login1.aspx`.

## Deploy
- Publicar carpeta en IIS.
- Asegurar que `Web.config` contenga conexión a DB y appSettings correctos.

Ver también:
- **[arquitectura.md](arquitectura.md)**
- **[procedimiento-instalacion.md](procedimiento-instalacion.md)**
