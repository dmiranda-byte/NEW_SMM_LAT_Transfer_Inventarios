# Diagrama de base de datos (ERD)

> Basado en las consultas de `App_Code/Queries.cs`, `Transfer.cs`, `DataManager.cs` y módulos relacionados.

```mermaid
erDiagram
    SMM_ODRF {
        int DocEntry PK
        date DocDate
        string DocStatus
        string Filler "FromWhsCode"
        string Comments
        string CompanyId
    }

    SMM_DRF1 {
        int DocEntry FK
        int LineNum
        string ItemCode
        decimal Quantity
        string WhsCode "ToWhsCode"
        string CompanyId
    }

    smm_Transdiscrep_odrf {
        int DocEntry PK
        string CompanyId
        string FromWhsCode
        string ToWhsCode
        string DocStatus
        string dispatched
        string received
        string UserDispatch
        string UserReceive
    }

    smm_Transdiscrep_drf1 {
        int DocEntry FK
        int LineNum
        string ItemCode
        decimal DraftQuantity
        string ToWhsCode
        string CompanyId
    }

    smm_Transdiscrep_audit_odrf {
        int DocEntry FK
        string CompanyId
        date DispatchDate
        date ReceiveDate
        string DispatchType
        string ReceiveType
    }

    smm_odrf_audit {
        int DocEntry
        string CompanyId
        string UserOrigin
        date DocDate
    }

    SMM_DRAFT_VW {
        int DocEntry
        string CompanyId
        string Filler
        string WhsCode
        date DocDate
    }

    OITM {
        string ItemCode PK
        string ItemName
        int ItmsGrpCod
        string U_BOT
    }

    OITB {
        int ItmsGrpCod PK
        string ItmsGrpNam
    }

    OWHS {
        string WhsCode PK
        string WhsName
    }

    ITM1 {
        string ItemCode FK
        int PriceList
        decimal Price
    }

    OBCD {
        string ItemCode FK
        string BcdCode
    }

    SMM_ODRF ||--o{ SMM_DRF1 : "DocEntry"
    smm_Transdiscrep_odrf ||--o{ smm_Transdiscrep_drf1 : "DocEntry"
    smm_Transdiscrep_odrf ||--o{ smm_Transdiscrep_audit_odrf : "DocEntry"
    SMM_ODRF ||--o{ smm_odrf_audit : "DocEntry"

    SMM_DRF1 }o--|| OITM : "ItemCode"
    OITM }o--|| OITB : "ItmsGrpCod"
    SMM_DRF1 }o--|| OWHS : "WhsCode"
    SMM_ODRF }o--|| OWHS : "Filler"
    OITM ||--o{ ITM1 : "ItemCode"
    OITM ||--o{ OBCD : "ItemCode"
```

Referencias adicionales:
- Consultas CTE: `App_Code/Queries.cs`
- Acceso a datos: `App_Code/SqlDb.cs`, `App_Code/DFBUYINGdb.cs`
- Lógica de transferencias: `App_Code/Transfer.cs`
