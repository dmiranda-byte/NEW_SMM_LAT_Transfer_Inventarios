# Diagramas de flujo

## Flujo: creación de transferencia
```mermaid
flowchart TD
    A[Inicio] --> B[Validar sesión/rol]
    B -->|OK| C[Seleccionar bodega origen/destino]
    C --> D[Seleccionar artículos]
    D --> E[Guardar borrador en SMM_ODRF/SMM_DRF1]
    E --> F[Mostrar confirmación]
    B -->|No autorizado| X[Redirigir a Login]
```

## Flujo: despacho y recepción
```mermaid
flowchart TD
    A[Consultar borradores] --> B[Ver detalle (TransferDetails.aspx)]
    B --> C[Despachar]
    C --> D[Actualizar smm_Transdiscrep_odrf]
    D --> E[Recepción]
    E --> F[Actualizar recibido y cierre]
```

## Flujo: auditoría
```mermaid
flowchart TD
    A[Seleccionar auditoría] --> B[Consultar smm_odrf_audit]
    B --> C[Comparar con Transdiscrep]
    C --> D[Generar reporte/alertas]
```

Referencias:
- `Transfers.aspx`, `TransferDetails.aspx`
- `App_Code/Transfer.cs`, `App_Code/Queries.cs`
