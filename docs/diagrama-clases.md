# Diagrama de clases / servicios

```mermaid
classDiagram
    class SqlDb {
        +SqlConnection Conn
        +SqlCommand cmd
        +SqlDataAdapter adapter
        +Connect()
        +Disconnect()
        +SISINV_GET_ACCESSTYPE_PRC()
        +SearchItemByBarCodes()
    }

    class DFBUYINGdb {
        +SqlConnection Conn
        +SqlCommand cmd
        +Connect()
        +Disconnect()
    }

    class DataManager {
        -SqlDb sqlDB
        -DFBUYINGdb dfbuyingDB
        +GetOrdenesAbiertas()
        +GetBinesDesp()
        +GetDetalledeItemsporBins()
        +GetBarcodeByProduct()
    }

    class Transfer {
        -SqlDb db
        +GetTransferDrafts()
        +GetTransferDetails()
        +GetTransferDraftDetails()
        +UpdateTransferStatus()
    }

    class Reports {
        +GetReport()
        +GetReportData()
    }

    class Admin {
        +GetUsers()
        +InsertUser()
        +UpdateUser()
        +GetLogins()
    }

    class WMS {
        +GetItemBin()
    }

    DataManager --> SqlDb
    DataManager --> DFBUYINGdb
    Transfer --> SqlDb
    Reports --> SqlDb
    Admin --> SqlDb
    WMS --> SqlDb
```

Referencias:
- `App_Code/SqlDb.cs`
- `App_Code/DFBUYINGdb.cs`
- `App_Code/DataManager.cs`
- `App_Code/Transfer.cs`
- `App_Code/Reports.cs`
- `App_Code/Admin.cs`
- `App_Code/WMS.cs`
