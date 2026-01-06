# Procedimiento de instalación y despliegue

## Requisitos
- Windows Server con **IIS**.
- **.NET Framework 4.8**.
- SQL Server con bases `SMM_LATI` y `UETA` (o equivalentes productivas).
- Acceso a tablas SAP B1 (mismo SQL Server o linked server según infraestructura).
- Librerías Telerik (`Telerik.Web.UI`) disponibles en el servidor.

## Pasos de instalación

### 1) Preparar base de datos
1. Restaurar/crear las bases necesarias en SQL Server.
2. Verificar tablas y vistas usadas por el sistema (ver **[diagrama-bd.md](diagrama-bd.md)**).
3. Confirmar existencia del SP `SISINV_GET_ACCESSTYPE_PRC`.

### 2) Configurar IIS
1. Crear un **sitio** o **aplicación** en IIS.
2. Asignar el **App Pool** con .NET Framework 4.0+.
3. Habilitar **Windows Authentication**.

### 3) Configurar `Web.config`
Actualizar los valores:
- `connectionStrings`: `smm_latConnectionString`, `DFBUYING`.
- `appSettings`: `smm_db`, `tienda_db`, `serverIP`, `serverUserName`, `serverPwd`, `licenseServerIP`, `whs_code`.

### 4) Publicar la aplicación
- Copiar el contenido del repositorio al directorio del sitio IIS.
- Asegurar permisos de lectura/ejecución para la cuenta del App Pool.

### 5) Validación
- Abrir la URL del sitio.
- Confirmar que carga `Default.aspx` y que se puede acceder a `Transfers.aspx`.

## Despliegue en producción
- Ajustar `Web.config` con credenciales y servidores productivos.
- Verificar que los handlers de Telerik estén activos.

## Configuración adicional
- **Tiempo de sesión**: `system.web/sessionState` (actualmente 120 min).
- **Tamaño de carga**: `httpRuntime maxRequestLength` (20 MB).

Ver también **[troubleshooting.md](troubleshooting.md)**.
