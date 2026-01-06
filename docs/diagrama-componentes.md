# Diagrama de componentes

```mermaid
flowchart LR
    subgraph UI[ASP.NET Web Forms]
        A1[SiteMaster.master]
        A2[Transfers.aspx]
        A3[CreateTransfer.aspx]
        A4[Reportes R_*.aspx]
        A5[Admin & FAQ]
    end

    subgraph BL[App_Code (Lógica de negocio)]
        B1[SqlDb / DFBUYINGdb]
        B2[Transfer]
        B3[DataManager]
        B4[Reports]
        B5[Admin]
        B6[WMS]
    end

    subgraph DB[SQL Server]
        D1[SMM_LATI (Tablas SMM_*)]
        D2[UETA / DFBUYING]
        D3[SAP B1 tablas (OITM, OWHS, OITB, ITM1, OBCD)]
        D4[Vistas WMS]
    end

    UI --> BL
    B2 --> D1
    B3 --> D2
    B1 --> D1
    B4 --> D1
    B5 --> D1
    B6 --> D4
    D1 --> D3
```

Para flujos de procesos, ver **[diagrama-flujo.md](diagrama-flujo.md)**.
