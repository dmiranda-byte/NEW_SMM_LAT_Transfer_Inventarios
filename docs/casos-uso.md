# Casos de uso principales

Este documento describe los casos de uso clave. Para el diagrama general de actores/relaciones, ver **[diagrama-casos-uso.md](diagrama-casos-uso.md)**.

## 1) Crear transferencia de inventario
**Actor principal:** Operador de bodega/tienda

**Descripción:** El usuario crea un borrador de transferencia seleccionando bodega origen/destino y artículos.

**Diagrama (flujo):**
```mermaid
flowchart TD
    A[Usuario ingresa a CreateTransfer.aspx] --> B[Validación de sesión y rol]
    B -->|Autorizado| C[Carga bodegas y grupos]
    C --> D[Usuario selecciona artículos]
    D --> E[Guardar borrador en SMM_ODRF/SMM_DRF1]
    E --> F[Mostrar confirmación]
    B -->|No autorizado| X[Redirige a Default/Login]
```

## 2) Despachar transferencia
**Actor principal:** Operador de despacho

**Descripción:** Se despacha un borrador y se marca el estado de despacho en las tablas de discrepancias.

**Diagrama (secuencia):**
```mermaid
sequenceDiagram
    participant U as Usuario
    participant UI as Transfers.aspx
    participant S as Transfer.cs
    participant DB as SQL Server

    U->>UI: Selecciona borrador
    UI->>S: Consultar detalle
    S->>DB: Consulta SMM_DRF1/SMM_ODRF
    DB-->>S: Datos de líneas
    S-->>UI: Detalle
    U->>UI: Confirmar despacho
    UI->>DB: Actualiza smm_Transdiscrep_odrf
    DB-->>UI: OK
```

## 3) Recibir transferencia
**Actor principal:** Operador de recepción

**Descripción:** Se registran cantidades recibidas y se cierra el proceso.

**Diagrama (flujo):**
```mermaid
flowchart LR
    A[Recepción] --> B[Consulta borrador/estado]
    B --> C[Captura cantidades]
    C --> D[Actualiza smm_Transdiscrep_odrf]
    D --> E[Marca como recibido]
```

## 4) Reportes operativos
**Actor principal:** Analista/Administrador

**Descripción:** Consulta reportes de inventario, bines, min/max y kardex.

**Diagrama (flujo):**
```mermaid
flowchart TD
    A[Usuario selecciona reporte R_*.aspx] --> B[Formulario de filtros]
    B --> C[Consulta SQL/Stored Procedure]
    C --> D[Renderiza reporte/tabla]
```

## 5) Administración de usuarios
**Actor principal:** Administrador

**Descripción:** Mantiene usuarios, roles y accesos.

**Diagrama (secuencia):**
```mermaid
sequenceDiagram
    participant Admin as Administrador
    participant UI as Módulo Admin
    participant A as Admin.cs
    participant DB as SQL Server

    Admin->>UI: Crear/editar usuario
    UI->>A: Insert/Update
    A->>DB: Insert/Update smm_users/smm_login
    DB-->>A: Resultado
    A-->>UI: OK/Error
```
