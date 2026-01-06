# FAQ

## ¿Cómo inicio sesión?
El sistema usa **Windows Authentication**. Tu usuario de dominio se valida en IIS y luego se valida el acceso por rol. Si no tienes permisos, verás un mensaje de acceso denegado.

## ¿Dónde se configuran las conexiones a base de datos?
En `Web.config` dentro de `connectionStrings` y `appSettings` (servidor, usuario, contraseñas, company DB).

## ¿Por qué no veo bodegas o artículos?
Verifica que `Session["CompanyId"]` esté correctamente seteado y que existan datos en las tablas SAP (`OWHS`, `OITM`) para esa compañía.

## ¿Cómo genero un reporte?
Ingresa a un reporte `R_*.aspx`, selecciona filtros y ejecuta la consulta. Los reportes están conectados a consultas SQL directas.

## ¿Cómo se controlan los permisos?
El procedimiento `SISINV_GET_ACCESSTYPE_PRC` devuelve el nivel de acceso por pantalla (N/R/F).

## ¿Dónde están los errores de transferencia?
Se registran en tablas de discrepancias (`smm_Transdiscrep_*`) y se visualizan en `TransferErrors.aspx` / `DeliveryErrors.aspx`.

## ¿Cómo exporto la documentación?
Ejecuta `scripts/copiar_documentacion_es.sh` para generar un paquete de la carpeta `/docs`.
