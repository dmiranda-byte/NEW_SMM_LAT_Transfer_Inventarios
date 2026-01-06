# Documentación de endpoints (UI Web Forms)

> **Nota:** el sistema no expone una API REST/JSON formal. Los "endpoints" son páginas Web Forms (`.aspx`) que renderizan HTML y consumen la lógica del servidor.

## Autenticación
- **Login**: `Login1.aspx`
- **Mecanismo**: Windows Authentication (IIS) + validaciones internas por sesión/rol.

## Endpoints principales

### Transferencias
| Página | Método | Parámetros | Descripción |
| --- | --- | --- | --- |
| `Transfers.aspx` | GET/POST | Filtros (fecha, estado, origen/destino) | Listado de borradores/transferencias. |
| `CreateTransfer.aspx` | GET/POST | Selección de bodegas, artículos | Crear transferencia Min/Max. |
| `CreateTransferXsap.aspx` | GET/POST | Campos de transferencia por SAP | Crear transferencia con integración SAP. |
| `createTransferByExcel.aspx` | POST | Archivo Excel | Carga masiva de transferencias. |
| `TransferDetails.aspx` | GET | `DocEntry` | Detalle/imprimir transferencia. |
| `TransferDetailsPrint.aspx` | GET | `DocEntry` | Versión impresión. |
| `TransferDelete.aspx` | POST | `DocEntry` | Eliminar o anular. |
| `TransferDiscreOrdf.aspx` | GET/POST | DocEntry/Filtros | Gestiona discrepancias. |
| `TransferErrors.aspx` | GET | - | Errores en transferencias. |

### Auditoría
| Página | Método | Parámetros | Descripción |
| --- | --- | --- | --- |
| `Transfers_Audit.aspx` | GET/POST | filtros | Auditoría de transferencias. |
| `Transfers_Audit_Item.aspx` | GET/POST | filtros | Auditoría por ítems. |

### Inventario / Bines
| Página | Método | Parámetros | Descripción |
| --- | --- | --- | --- |
| `WhsItemBin.aspx` | GET/POST | bodega, bin, ítem | Consulta bin por bodega. |
| `StoreBines.aspx` | GET/POST | filtros | Consulta bines de tienda. |
| `ProductLocator.aspx` | GET/POST | ítem/código de barras | Localización de producto. |
| `sapInventory.aspx` | GET/POST | filtros | Inventario SAP. |

### Reportes (`R_*.aspx`)
| Página | Descripción |
| --- | --- |
| `R_Kardex.aspx` | Kardex de inventario. |
| `R_BinesDesp.aspx` / `R_BinesDespTiendas.aspx` | Bines despachados. |
| `R_OrdenesAbiertas.aspx` | Órdenes abiertas. |
| `R_OrdenesConProblemas.aspx` | Órdenes con problemas. |
| `R_ReciboBodegavsSAP.aspx` | Comparativos recibo vs SAP. |
| `R_InventarioDeTiendasTocumen.aspx` | Inventario por tiendas. |

### Administración y contenidos
| Página | Descripción |
| --- | --- |
| `MaintainFAQ.aspx` | Mantener FAQ. |
| `MaintainVideosFinancieros.aspx` | Mantener videos financieros. |
| `MaintainVideosOperativos.aspx` | Mantener videos operativos. |
| `PreguntasFrecuentes.aspx` | FAQ para usuarios. |

## Códigos y errores
- Errores suelen presentarse como mensajes HTML o `Response.Write` en la página.
- Redirección a `AccessDenied.aspx` si falla la conexión DB o permisos.

## Referencias de implementación
- Acceso a datos: `App_Code/SqlDb.cs`, `App_Code/Transfer.cs`, `App_Code/DataManager.cs`.
- Sesiones y permisos: ver `CreateTransfer.aspx.cs` y `SqlDb.SISINV_GET_ACCESSTYPE_PRC`.
