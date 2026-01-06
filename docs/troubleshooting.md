# Troubleshooting

## Error: AccessDenied.aspx
**Causa probable:** fallo de conexión a la base de datos o permisos insuficientes.

**Acciones:**
- Validar `connectionStrings` en `Web.config`.
- Verificar permisos del App Pool en SQL Server.

## Error: "Favor de registrarse en el sistema"
**Causa probable:** sesión inválida o sin `UserId`/`CompanyId`.

**Acciones:**
- Verificar la autenticación Windows en IIS.
- Revisar que el usuario exista en `smm_users`/`smm_login`.

## No aparecen transferencias
**Causa probable:** filtros demasiado restrictivos o datos inexistentes.

**Acciones:**
- Quitar filtros y revisar estado de borradores.
- Verificar tablas `SMM_ODRF`, `SMM_DRF1`.

## Reportes en blanco
**Causa probable:** parámetros nulos o consultas con `WITH(NOLOCK)` sin datos.

**Acciones:**
- Revisar filtros en la UI.
- Validar tablas de origen (ej. `smm_Transdiscrep_odrf`).

## Error al cargar Excel
**Causa probable:** formato incorrecto o tamaño excedido.

**Acciones:**
- Verificar límites de carga (`httpRuntime maxRequestLength`).
- Confirmar plantilla esperada por el módulo.

## Problemas con Telerik
**Causa probable:** handlers o assembly no disponibles.

**Acciones:**
- Revisar `Web.config` (handlers de Telerik).
- Confirmar que el assembly `Telerik.Web.UI` exista en `Bin/`.
