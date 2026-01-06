# Estructura del proyecto

## Estructura general (resumen)
```
/
├── App_Code/                 # Lógica de negocio y acceso a datos
├── Images/                   # Recursos gráficos
├── ImgPreguntasFrecuentes/   # Imágenes del módulo FAQ
├── Scripts/                  # JS/CSS (Telerik y scripts propios)
├── admin/                    # Recursos administrativos (si aplica)
├── aspnet_client/            # Recursos IIS clásicos
├── temp/                     # Archivos temporales
├── *.aspx / *.aspx.cs         # Páginas Web Forms y code-behind
├── *.master / *.master.cs     # Plantilla principal
├── Web.config                 # Configuración de IIS, conexiones, appSettings
├── packages.config            # Paquetes NuGet
└── docs/                      # Documentación (este paquete)
```

## Descripción de componentes clave
- **`App_Code/`**: contiene la lógica compartida (SQL, transferencias, reportes, administración).
- **`*.aspx`**: páginas funcionales del sistema (transferencias, inventario, reportes).
- **`Web.config`**: define autenticación, handlers Telerik, y conexiones DB.
- **`Scripts/`**: scripts front-end utilizados en las páginas.

## Módulos funcionales por páginas
- **Transferencias**: `Transfers.aspx`, `CreateTransfer.aspx`, `TransferDetails.aspx`.
- **Carga masiva**: `createTransferByExcel.aspx`, `MinMaxByExcel.aspx`, `BinByExcel.aspx`.
- **Inventario/Bines**: `WhsItemBin.aspx`, `StoreBines.aspx`, `ProductLocator.aspx`.
- **Reportes**: `R_*.aspx`.
- **Administración**: `MaintainFAQ.aspx`, `MaintainVideos*.aspx`.

Para la descripción completa del flujo, ver **[manual-funcional.md](manual-funcional.md)**.
