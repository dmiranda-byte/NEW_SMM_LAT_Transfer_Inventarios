# SMM LAT Transfer Inventarios — Documentación

## Resumen del proyecto
Este repositorio contiene una aplicación web ASP.NET Web Forms para la gestión de **transferencias de inventario** entre bodegas/tiendas (SMM LAT). El sistema permite crear borradores de transferencia, despachar y recibir mercancía, revisar discrepancias, auditar operaciones y consultar reportes operativos/financieros.

La solución corre sobre IIS con **.NET Framework 4.8** y usa SQL Server como motor principal. Hay integración con tablas SAP Business One (por ejemplo `OITM`, `OWHS`, `OITB`, `ITM1`, `OBCD`) a través de consultas directas (ver `App_Code/Queries.cs`).

> Para una vista detallada del diseño, consulte **[arquitectura.md](arquitectura.md)** y los diagramas en **[docs/](.)**.

## Funcionalidades clave
- **Gestión de transferencias**: creación, consulta, despachos, recepción y cancelación.
- **Control de discrepancias**: comparación entre borradores y movimientos reales.
- **Reportes operativos**: inventario, min/max, bines, kardex, órdenes con problemas.
- **Administración**: usuarios, roles, accesos y FAQ.
- **Soporte a procesos**: carga por Excel, validaciones de acceso, auditoría.

## Arquitectura (resumen)
- **UI**: páginas `.aspx` con code-behind `.aspx.cs`.
- **Lógica de negocio**: clases en `App_Code/` (por ejemplo `Transfer`, `DataManager`, `Reports`).
- **Persistencia**: acceso ADO.NET con `SqlDb`/`DFBUYINGdb` a SQL Server.
- **Integración**: consultas a tablas SAP B1 y vistas WMS (ej. `Wms_Whs_Item_Bin_vw`).

Diagrama general en **[diagrama-componentes.md](diagrama-componentes.md)**.

## Setup básico
1. **Requisitos**
   - Windows Server + IIS
   - .NET Framework 4.8
   - SQL Server con las bases `SMM_LATI` y `UETA` (o equivalentes)
   - Telerik Web UI (referencias en `Web.config`)
2. **Configurar conexiones**
   - `Web.config` → `connectionStrings` y `appSettings`.
3. **Publicar en IIS**
   - Crear sitio/aplicación y apuntar al directorio del repositorio.
4. **Autenticación**
   - La app usa **Windows Authentication** (ver `Web.config`).

Más detalle en **[procedimiento-instalacion.md](procedimiento-instalacion.md)**.

## Documentación relacionada
- Arquitectura: **[arquitectura.md](arquitectura.md)**
- Casos de uso: **[casos-uso.md](casos-uso.md)**
- API/Endpoints: **[documentacion-api.md](documentacion-api.md)**
- Estructura del proyecto: **[estructura-proyecto.md](estructura-proyecto.md)**
- Manual funcional: **[manual-funcional.md](manual-funcional.md)**
- Manual técnico: **[manual-tecnico.md](manual-tecnico.md)**
- Troubleshooting: **[troubleshooting.md](troubleshooting.md)**
