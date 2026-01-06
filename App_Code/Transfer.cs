using System;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;


/// <summary>
/// Summary description for Transfer
/// </summary>
public class Transfer
{
    protected SqlDb db;
    protected string sap_db;
    protected string whs_code;
    
    public Transfer()
    {
        db = new SqlDb();
        db.Connect();

        sap_db = ConfigurationManager.AppSettings.Get("sap_db");
        whs_code = ConfigurationManager.AppSettings.Get("whs_code");
    }


    public DataTable GetTransferDrafts(string statusDoc, string txtDocNum, string  FromDateTxt, string toDateTxt, string fromLocTxt, string toLocTxt, string categoryTxt, string andOr1, string andOr2, string andOr3, string companyId)
    {
        sap_db = companyId;
        DataTable dt = new DataTable();
        string sql = "";
        int dummy = 0;
        int Lflag1 = 0;
        string sDate = null;
        string sLoc = null;
        string sStatus = null;
        string sDocNum = null;

        try
        {
            sql = Queries.With_SmmDraftHeader() + @"
    SELECT t1.*, isnull(t2.dispatched,'P') dispatched, isnull(t2.received,'P') received, isnull(t2.DispCompleted,'P') DispCompleted, isnull(t2.ReceCompleted,'P') ReceCompleted, isnull(substring(t2.InputType,1,1),'I') InputType, isnull(substring(t2.ScanStatus,1,1),'N') ScanStatus
    FROM SmmDraftHeader t1 " + Queries.WITH_NOLOCK + @" 
    LEFT JOIN TransdiscrepODRF t2 " + Queries.WITH_NOLOCK + @"  ON t1.docentry = t2.docentry AND t1.CompanyId = t2.CompanyId 
    WHERE t1.CompanyId = '{0}' AND ";

            sql = string.Format(sql, sap_db);

            ///---FromDateTxt

            if (string.IsNullOrEmpty(FromDateTxt) && string.IsNullOrEmpty(toDateTxt))
            {
                if (andOr1 == "OR")
                {
                    sDate = " (1=2) ";
                }
                else
                {
                    sDate = " (1=1) ";
                }
            }
            else
            {
                if (string.IsNullOrEmpty(FromDateTxt))
                {
                    FromDateTxt = "01/01/2007";
                }
                else
                {
                    if (FromDateTxt.Length < 1)
                    {
                        FromDateTxt = "01/01/2007";
                    }
                }
                
                if (string.IsNullOrEmpty(toDateTxt))
                {
                    toDateTxt = " getdate() + 1 ";
                    sDate = " (t1.DocDate between '"+FromDateTxt+"' and "+toDateTxt+") ";
                }
                else
                {
                    if (toDateTxt.Length < 1)
                    {
                        toDateTxt = " getdate() + 1 ";
                        sDate = " (t1.DocDate between '"+FromDateTxt+"' and "+toDateTxt+") ";
                    }
                    else
                    {
                        sDate = " (t1.DocDate between '"+FromDateTxt+"' and DATEADD(day,1,cast('"+toDateTxt+"' as date))) ";
                    }     
                }
            }

            ///---toLocTxt

            if (fromLocTxt == "0" && toLocTxt == "0")
            {
                if (andOr2 == "OR")
                {
                    sLoc = " (1=2) ";
                }
                else
                {
                    sLoc = " (1=1) ";
                }
            }
            else
            {
                if (fromLocTxt == "0")
                {
                    if (andOr2 == "OR")
                    {
                        sLoc = " (1=2 and ";
                    }
                    else
                    {
                        sLoc = " (1=1 and ";
                    } 
                }
                else
                {
                    sLoc = " ( t1.fromLoc =  '" + fromLocTxt + "' and " ;
                }

                if (toLocTxt == "0")
                {
                    if (andOr2 == "OR")
                    {
                        sLoc = sLoc + " (1 = 2)) ";
                    }
                    else
                    {
                        sLoc = sLoc + " (1 = 1)) ";                        
                    } 
                }
                else
                {
                    sLoc = sLoc + " t1.toLoc =  '" + toLocTxt + "' ) ";
                }
            }

            ///---status and categoryTxt

            if (categoryTxt == "0" && statusDoc == "All")
            {
                if (andOr3 == "OR")
                {
                    sStatus = " (1=2) ";

                }
                else
                {
                    sStatus = " (1=1) ";
                }
            }
            else
            {
                if (categoryTxt == "0")
                {
                    if (andOr3 == "OR")
                    {
                        sStatus = " (1=2  ";
                    }
                    else
                    {
                        sStatus = " (1=1  ";
                    }
                }
                else
                {
                    sStatus = " (t1.itmsGrpCod = '" + categoryTxt + "'";
                }

                if (statusDoc == "All")
                {
                    sStatus = sStatus + ") ";                                         
                }
                else
                {
                    if (andOr3 == "OR")
                    {
                        sStatus = sStatus + "  OR t1.docstatus =  '" + statusDoc  + "' ) ";
                    }
                    else
                    {
                        sStatus = sStatus + "  AND t1.docstatus =  '" + statusDoc + "' ) ";
                    }
                }
            }

            /////-------------sDocNum         
                
            if (int.TryParse(txtDocNum, out dummy))
            {
                sDocNum =  " (t1.docentry =  " + txtDocNum + ") " ;
                Lflag1++;
            }
            else
            {
                sDocNum = " (1=1) ";
            }

            sql +=  sDocNum + andOr1 + sDate + andOr2 + sLoc + andOr3 + sStatus;
            sql += " order by t1.DocEntry Desc";

            if(db.DbConnectionState == ConnectionState.Closed)
            {
                db.Connect();
            }
            
            db.adapter = new SqlDataAdapter(sql, db.Conn);
            db.adapter.Fill(dt);
        }
        catch (Exception ex)
        {
            throw new Exception("Caught exception in procedure GetTransferDrafts.  ERROR MESSAGE :" + ex.Message);
        }
        finally
        {
            db.Disconnect();
        }

        return dt;
    }

    public DataTable GetTransferDraftsToDelete(string CompanyId)
    {
        DataTable dt = new DataTable();
        string sql = "";
        
        try
        {
            //sql = @"select * from smm_draft_header_vw ";
            //sql = @"select t1.*, isnull((select isnull(dispatched,'_') + '/' + isnull(received,'_') + ' /   ' + isnull(DispCompleted,'_') + '/' + isnull(ReceCompleted,'_') from smm_Transdiscrep_odrf where docentry = t1.docentry),'___/___') drfst from smm_draft_header_vw t1  ";
            sql = Queries.With_SmmDraftHeader() + @"
             select t1.*, isnull((select isnull(a1.dispatched,'P') 
		    from smm_Transdiscrep_odrf a1 where a1.CompanyId = t1.CompanyId and a1.docentry = t1.docentry),'N') dispatched,
		    isnull((select isnull(a1.received,'P') 
		    from smm_Transdiscrep_odrf a1 where a1.CompanyId = t1.CompanyId and a1.docentry = t1.docentry),'N') received,
		    isnull((select isnull(a1.DispCompleted,'P') 
		    from smm_Transdiscrep_odrf a1 where a1.CompanyId = t1.CompanyId and a1.docentry = t1.docentry),'N') DispCompleted,
		    isnull((select isnull(a1.ReceCompleted,'P') 
                    from smm_Transdiscrep_odrf a1 where a1.CompanyId = t1.CompanyId and a1.docentry = t1.docentry),'N') ReceCompleted
                    --from smm_draft_header_vw t1 
                    from SmmDraftHeader t1
                    left outer join smm_Transdiscrep_odrf t2
                    on t1.CompanyId = t2.CompanyId and t1.docentry = t2.docentry
                    where  t1.DocStatus = 'O' 
                    --and isnull(t2.dispatched,'P') <> 'Y'
                    and isnull(t2.received,'P') <> 'Y' order by t1.docentry ";

            sql = string.Format(sql, CompanyId);
        
            db.adapter = new SqlDataAdapter(sql, db.Conn);
            db.adapter.Fill(dt);
        }
        catch (Exception ex)
        {
            throw new Exception("Caught exception in procedure GetTransferDrafts.  ERROR MESSAGE :" + ex.Message);
        }
        finally
        {
            db.Disconnect();
        }
        
        return dt;
    }
    
    public DataTable GetTransferErrors(bool ShowAll, string CompanyId)
    {
        sap_db = CompanyId;
        DataTable dt = new DataTable();
        string sql = "";

        try
        {

            //sql = @"select * from smm_draft_header_vw ";
            //sql = @"select t1.*, isnull((select isnull(dispatched,'_') + '/' + isnull(received,'_') + ' /   ' + isnull(DispCompleted,'_') + '/' + isnull(ReceCompleted,'_') from smm_Transdiscrep_odrf where docentry = t1.docentry),'___/___') drfst from smm_draft_header_vw t1  ";
            sql = @"select DocEntryOri, line, docdate, fromwhscode, 
                    towhscode, tooriwhscode, itemcode+' - '+ [dbo].[InitCap] (pludesc) itemcode, /*[dbo].[FIVEBCODEPRODS] (itemcode)*/ STUFF((SELECT ' - ' + RIGHT(BcdCode, 5) FROM " + sap_db + @".dbo.OBCD a1 " + Queries.WITH_NOLOCK + @" WHERE a1.ItemCode=la_transfer_errors.itemcode FOR XML PATH ('')), 1, 3, '') BarCode, 
                    convert(int,quantity) as quantity, userapp, error_message, case when fixed = 'N' then 0 else 1 end fixed 
                    from la_transfer_errors 
					where DocEntryOri in (select DocEntry from smm_Transdiscrep_odrf
					                         where CompanyId = '" + sap_db+"') ";

            if (!ShowAll)
                sql += " and fixed = 'N'";

            sql += " order by DocEntryOri Desc, line";

            db.adapter = new SqlDataAdapter(sql, db.Conn);
            db.adapter.Fill(dt);
        }
        catch (Exception ex)
        {
            throw new Exception("Caught exception in procedure GetTransferErrors.  ERROR MESSAGE :" + ex.Message);
        }
        finally
        {
            db.Disconnect();
        }

        return dt;
    }

    public DataTable GetStoreTransferDrafts(bool ShowAll, string CompanyId)
    {
        DataTable dt = new DataTable();
        string sql = "";

        try
        {

            sql = Queries.With_SmmDraftHeader() + @"
                select * from SmmDraftHeader";

            sql = string.Format(sql, CompanyId);

            if (!ShowAll)
                sql += " where DocStatus = 'O' and ToLoc like '%TIENDA%'";

            sql += " order by DocEntry Desc";

            db.adapter = new SqlDataAdapter(sql, db.Conn);
            db.adapter.Fill(dt);
        }
        catch (Exception ex)
        {
            throw new Exception("Caught exception in procedure GetTransferDrafts.  ERROR MESSAGE :" + ex.Message);
        }
        finally
        {
            db.Disconnect();
        }

        return dt;
    }

    public DataTable GetTransferDetails(string DocEntry, string companyId)
    {
        sap_db = companyId;
        DataTable dt = new DataTable();
        string sql = "";
    
        try
        {
            sql = @"select 
			    a.DocEntry ,
			    DocDate ,
			    FromLoc ,
			    ToLoc ,
			    FromLocName ,
			    ToLocName ,
			    DocStatus Status,
			    --LineNumber ,
			    LineNumber = ROW_NUMBER() OVER (ORDER BY ws.bins, LineNumber ASC) ,
			    DeptCode, 
			    DeptName ,
				--oi.ItemCode,
			    CASE WHEN oi.U_brand='REEBOK' and OI.U_class='FOOTWEAR' THEN oi.U_SIZE +'-'+OI.U_SUBCLASS ELSE oi.ItemCode  END ItemCode,
			    --oi.U_brand,
				CASE WHEN oi.U_brand='REEBOK' THEN OI.U_class ELSE oi.U_BRAND  END U_brand,
			    case when len(Dscription) > 60 then left(Dscription,60) + '...' else Dscription end  Description,
			    Qty ,
			    Price,
			    order_multiple,
			    a.u_bot,
			    case when ORDER_MULTIPLE = 'C' then isnull(a.u_bot,1) else 1 end case_pack,
			    Qty / case when ORDER_MULTIPLE = 'C' then isnull(a.u_bot,1) else null end cases,
			    (select isnull(max(Usuario_Originador), '_') 
			    from SMM_LAT_TRANSFERS_AUDIT_VW with (nolock) 
					    where [Draft_Numero] = a.DocEntry) as DraftUser,
			    (select 
			    isnull(max(Usuario_Despacho),'_')
			    from SMM_LAT_TRANSFERS_AUDIT_VW with (nolock) 
					    where [Draft_Numero] = a.DocEntry)  as DespUser,
			    (select 
			    isnull(max(Usuario_Recibo) , '_') 
			    from SMM_LAT_TRANSFERS_AUDIT_VW with (nolock) 
					    where [Draft_Numero] = a.DocEntry) as RecUser,
			    (select case when count(*) =  0 then 'ORIGINAL del ' else 'COPIA del ' end
			     from smm_Print_Control with (nolock) 
			    where docentry = a.DocEntry
			    ) as oricopy,
				ws.bins,
				CASE WHEN oi.U_brand='REEBOK' THEN STUFF((SELECT ' - ' + BcdCode FROM " + sap_db + @".dbo.OBCD a1 WITH(NOLOCK) WHERE a1.ItemCode=oi.ItemCode and  a1.BcdCode <> a1.ItemCode FOR XML PATH ('')), 1, 3, '') ELSE
				STUFF((SELECT ' - ' + RIGHT(BcdCode, 5) FROM " + sap_db + @".dbo.OBCD a1 with(nolock) WHERE a1.ItemCode=oi.ItemCode FOR XML PATH ('')), 1, 3, '') END BarCode,
			   (select convert(int,sum(TotalQty))  from smm_draft_header_vw where docentry =  " + DocEntry + @") TotalProds 
			from 
			    smm_draft_detail_vw a with (nolock) inner join rss_loc_dept_multiple b  with (nolock) 
				on a.ToLoc = b.loc 
				and a.DeptCode = b.dept
				and a.CompanyId = b.CompanyId
				and b.companyId = '" + sap_db + @"' 
			    INNER JOIN " + sap_db + @".dbo.oitm oi   with (nolock) 
				ON oi.itemcode = a.ItemCode
			    LEFT JOIN WMS_Item_Bins_Cons ws  with (nolock) 
				on ws.whscode = FromLoc 
				and ws.Itemcode = oi.ItemCode
		where a.DocEntry = " + DocEntry + " order by ws.bins, LineNumber";
            //            sql = Queries.With_SmmDraftDetail() + @"
            //SELECT a.DocEntry, DocDate, FromLoc, ToLoc, FromLocName, ToLocName, DocStatus Status,
            //    LineNumber = ROW_NUMBER() OVER (ORDER BY ws.bins, LineNumber ASC), 
            //    DeptCode, DeptName, oi.ItemCode, oi.U_brand, 
            //    CASE WHEN LEN(Dscription) > 60 THEN LEFT(Dscription,60) + '...' ELSE Dscription END Description,
            //    Qty, Price, order_multiple, a.u_bot, 
            //    CASE WHEN ORDER_MULTIPLE = 'C' THEN ISNULL(a.u_bot, 1) ELSE 1 END case_pack,
            //    Qty / CASE WHEN ORDER_MULTIPLE = 'C' THEN ISNULL(a.u_bot, 1) ELSE NULL END cases,    
            //(SELECT ISNULL(MAX(Usuario_Originador), '_') FROM TransfersAudit " + Queries.WITH_NOLOCK + @"  WHERE Draft_Numero = a.DocEntry) AS DraftUser,
            //(SELECT ISNULL(MAX(Usuario_Despacho), '_') FROM TransfersAudit " + Queries.WITH_NOLOCK + @"  WHERE Draft_Numero = a.DocEntry) AS DespUser,
            //(SELECT ISNULL(MAX(Usuario_Recibo), '_') FROM TransfersAudit " + Queries.WITH_NOLOCK + @"  WHERE Draft_Numero = a.DocEntry) AS RecUser,
            //(SELECT CASE WHEN COUNT(1) = 0 THEN 'ORIGINAL del ' ELSE 'COPIA del ' END FROM smm_Print_Control " + Queries.WITH_NOLOCK + @"  WHERE docentry = a.DocEntry) AS oricopy,
            //    ws.bins,
            //    /*dbo.FIVEBCODEPRODS(oi.ItemCode)*/ STUFF((SELECT ' - ' + RIGHT(BcdCode, 5) FROM " + sap_db + @".dbo.OBCD a1 " + Queries.WITH_NOLOCK + @" WHERE a1.ItemCode=oi.ItemCode FOR XML PATH ('')), 1, 3, '') BarCode,    
            //(SELECT CONVERT(INT, SUM(TotalQty)) FROM SmmDraftHeader t1 WHERE docentry = {1}) TotalProds 
            //FROM SmmDraftDetail a " + Queries.WITH_NOLOCK + @"  
            //    INNER JOIN rss_loc_dept_multiple b " + Queries.WITH_NOLOCK + @"  on a.ToLoc = b.loc and a.DeptCode = b.dept and a.CompanyId = b.CompanyId and b.companyId = '{0}' 
            //    INNER JOIN {0}.dbo.OITM oi " + Queries.WITH_NOLOCK + @"  ON oi.itemcode = a.ItemCode
            //    LEFT JOIN WMS_Item_Bins_Cons ws " + Queries.WITH_NOLOCK + @"  on ws.whscode = FromLoc and ws.Itemcode = oi.ItemCode
            //WHERE a.DocEntry = {1} 
            //ORDER BY ws.bins, LineNumber";
            ////where oi.itemcode = a.ItemCode and a.DocEntry = " + DocEntry + " order by oi.U_brand, Description";


            //sql = string.Format(sql, sap_db, DocEntry);

            db.Connect();
            db.adapter = new SqlDataAdapter(sql, db.Conn);
            db.adapter.Fill(dt);
        }
        catch (Exception ex)
        {
            throw new Exception("Caught exception in procedure GetTransferDetails.  ERROR MESSAGE :" + ex.Message + " - " + sql);
        }
        finally
        {
            db.Disconnect();
        }
    
        return dt;
    }

    public DataTable GetDisTransferDetails(string DocEntry, string companyId)
    {
        sap_db = companyId;
        DataTable dt = new DataTable();
        string sql = "";

        try
        {

            sql = @"select 
                    th.DocEntry ,
                    th.DocDate ,
                    th.FromWhsCode FromLoc ,
                    th.ToWhsCode ToLoc ,
                    th.FromWhsName FromLocName ,
                    th.ToWhsName ToLocName ,
                    th.DocStatus Status,
                    (td.LineNum + 1) LineNumber ,
                    'DeptCode' DeptCode, 
                    'DeptName' DeptName ,
                    --td.ItemCode ,
                    --oi.U_BRAND,
				--td.ItemCode,
			    CASE WHEN oi.U_brand='REEBOK' and oi.U_class='FOOTWEAR' THEN oi.U_SIZE +'-'+oi.U_SUBCLASS ELSE td.ItemCode  END ItemCode,
			    --oi.U_brand,
				CASE WHEN oi.U_brand='REEBOK' THEN oi.U_class ELSE oi.U_brand  END U_brand,

                    case when len(td.ItemName) > 35 then left(td.ItemName,35) + '...' else td.ItemName end  Description,
                    case th.Received
                         when 'Y' then td.receivedQuantity
                         else case th.Dispatched
                                   when 'Y' then td.DispatchQuantity
                                   else td.draftQuantity
                              end
                    end Qty ,
                    td.Price Price,
                    'om' order_multiple,
                    'ub' u_bot,
                    0 cases,
                    /*[dbo].[FIVEBCODEPRODS](td.ItemCode) STUFF((SELECT ' - ' + RIGHT(BcdCode, 5) FROM " + sap_db + @".dbo.OBCD a1 " + Queries.WITH_NOLOCK + @" WHERE a1.ItemCode=td.ItemCode FOR XML PATH ('')), 1, 3, '') BarCode 
                */
			CASE WHEN oi.U_brand='REEBOK' THEN STUFF((SELECT ' - ' + BcdCode FROM " + sap_db + @".dbo.OBCD a1 WITH(NOLOCK) WHERE a1.ItemCode=td.ItemCode and  a1.BcdCode <> a1.ItemCode FOR XML PATH ('')), 1, 3, '') ELSE
			STUFF((SELECT ' - ' + RIGHT(BcdCode, 5) FROM " + sap_db + @".dbo.OBCD a1 with(nolock) WHERE a1.ItemCode=td.ItemCode FOR XML PATH ('')), 1, 3, '') END BarCode
			from 
                    smm_Transdiscrep_odrf th,
                    smm_Transdiscrep_drf1 td,
                    {0}.dbo.oitm oi
                where th.DocEntry  = td.DocEntry 
                    and oi.ItemCode = td.ItemCode
                    and th.DocEntry = {1} order by td.LineNum";
            //and th.DocEntry = " + DocEntry + " order by oi.U_brand, Description";

            sql = string.Format(sql, sap_db, DocEntry);

            db.adapter = new SqlDataAdapter(sql, db.Conn);
            db.adapter.Fill(dt);
        }
        catch (Exception ex)
        {
            throw new Exception("Caught exception in procedure GetDisTransferDetails.  ERROR MESSAGE :" + ex.Message);
        }
        finally
        {
            db.Disconnect();
        }

        return dt;
    }

public DataTable GetTransferAudit(string statusDoc, string txtDocNum,
                                       string FromDateTxt, string toDateTxt,
                                       string fromLocTxt, string toLocTxt, string categoryTxt,
                                       string andOr1, string andOr2, string andOr3, string doQry, string CompanyId)
    {
        sap_db = CompanyId;
        DataTable dt = new DataTable();
        string sql = "";
        int dummy = 0;
        int Lflag1 = 0;
        string sDate = null;
        string sLoc = null;
        string sStatus = null;
        string sDocNum = null;

        try
        {

            sql = Queries.With_TransfersAudit() + @"
                select 
                    CompanyId,
                    Draft_Numero,
                    Codigo_Origen, 
                    Nombre_Origen, 
                    Codigo_Destino, 
                    Nombre_Destino, 
                    Estatus, 
                    Despachado, 
                    Recibido, 
                    Usuario_Originador,
                    Usuario_Despacho, 
                    Usuario_Recibo, 
                    Doc_Entry_Sap, 
                    Doc_Num_Sap,
                    Fecha_Originacion, 
                    Fecha_Despacho, 
                    Fecha_Recibo, 
                    Tiempo_Despachar,
                    Tiempo_Recibir,
                    substring(SistemaDes,4,3) SistemaDes,
		    substring(SistemaRec,4,3) SistemaRec
                    --from SMM_LAT_TRANSFERS_AUDIT_VW
                    FROM TransfersAudit
                    where ";

            if (int.TryParse(txtDocNum, out dummy))
            {
                Lflag1++;
                sql = string.Format(sql, sap_db, txtDocNum);
            }
            else
            {
                sql = string.Format(sql, sap_db, "-1");
            }
            

            ///---FromDateTxt

            if (string.IsNullOrEmpty(FromDateTxt) && string.IsNullOrEmpty(toDateTxt))
            {
                if (andOr1 == "OR")
                {
                    sDate = " (1=2) ";

                }
                else
                {
                    sDate = " (1=1) ";
                }

            }
            else
            {

                if (string.IsNullOrEmpty(FromDateTxt))
                {

                    FromDateTxt = "01/01/2013";
                }
                else
                {

                    if (FromDateTxt.Length < 1)
                    {
                        FromDateTxt = "01/01/2013";
                    }

                }


                if (string.IsNullOrEmpty(toDateTxt))
                {

                    toDateTxt = " getdate() + 1 ";
                    sDate = " (Fecha_Originacion between '" + FromDateTxt + "' and " + toDateTxt + ") ";
                }
                else
                {

                    if (toDateTxt.Length < 1)
                    {
                        toDateTxt = " getdate() + 1 ";
                        sDate = " (Fecha_Originacion between '" + FromDateTxt + "' and " + toDateTxt + ") ";
                    }
                    else
                    {
                        sDate = " (Fecha_Originacion between '" + FromDateTxt + "' and DATEADD(day,1,cast('" + toDateTxt + "' as date))) ";

                    }


                }

            }

            ///---toLocTxt

            if (fromLocTxt == "0" && toLocTxt == "0")
            {
                if (andOr2 == "OR")
                {
                    sLoc = " (1=2) ";

                }
                else
                {
                    sLoc = " (1=1) ";
                }
            }
            else
            {

                if (fromLocTxt == "0")
                {

                    if (andOr2 == "OR")
                    {
                        sLoc = " (1=2 and ";
                    }
                    else
                    {
                        sLoc = " (1=1 and ";
                    }



                }
                else
                {

                    sLoc = " ( Codigo_Origen =  '" + fromLocTxt + "' and ";

                }


                if (toLocTxt == "0")
                {

                    if (andOr2 == "OR")
                    {
                        sLoc = sLoc + " (1 = 2)) ";
                    }
                    else
                    {
                        sLoc = sLoc + " (1 = 1)) ";
                    }


                }
                else
                {

                    sLoc = sLoc + " Codigo_Destino =  '" + toLocTxt + "' ) ";

                }

            }


            ///---status and categoryTxt
            ///

            categoryTxt = "0";

            if (categoryTxt == "0" && statusDoc == "All")
            {
                if (andOr3 == "OR")
                {
                    sStatus = " (1=2) ";

                }
                else
                {
                    sStatus = " (1=1) ";
                }
            }
            else
            {

                if (categoryTxt == "0")
                {

                    if (andOr3 == "OR")
                    {
                        sStatus = " (1=2  ";
                    }
                    else
                    {
                        sStatus = " (1=1  ";
                    }



                }
                else
                {

                    sStatus = " (itmsGrpCod = '" + categoryTxt + "'";


                }


                if (statusDoc == "All")
                {

                    sStatus = sStatus + ") ";

                }
                else
                {


                    if (andOr3 == "OR")
                    {
                        sStatus = sStatus + "  OR Estatus =  '" + statusDoc + "' ) ";
                    }
                    else
                    {
                        sStatus = sStatus + "  AND Estatus =  '" + statusDoc + "' ) ";
                    }



                }

            }


            /////-------------sDocNum


            if (int.TryParse(txtDocNum, out dummy))
            {
                sDocNum = " (Draft_Numero =  " + txtDocNum + ") ";
                Lflag1++;


            }
            else
            {
                sDocNum = " (1=1) ";

            }



            if (doQry == "0")
            {
                doQry = " and 1=2 ";


            }
            else
            {
                doQry = " and 1=1 ";

            }


            if (Lflag1 == 1)
            {
                sql += sDocNum + doQry;
            }
            else
            {
                sql += sDocNum + andOr1 + sDate + andOr2 + sLoc + andOr3 + sStatus + doQry;
            }

            sql += " and CompanyId = '" + sap_db + "' order by Draft_Numero ";

            db.adapter = new SqlDataAdapter(sql, db.Conn);
            db.adapter.Fill(dt);
        }
        catch (Exception ex)
        {
            throw new Exception("Caught exception in procedure GetTransferAudit.  ERROR MESSAGE :" + ex.Message);
        }
        finally
        {
            db.Disconnect();
        }

        return dt;
    }

    
    public DataTable GetTransferAuditItem(string statusDoc, string txtDocNum, string ItemCodeTbox,
                                       string FromDateTxt, string toDateTxt,
                                       string fromLocTxt, string toLocTxt, string categoryTxt,
                                       string andOr1, string andOr2, string andOr3, string doQry, string CompanyId)
    {
        sap_db = CompanyId;
        DataTable dt = new DataTable();
        string sql = "";
        int dummy = 0;
        int Lflag1 = 0;
        string sDate = null;
        string sLoc = null;
        string sStatus = null;
        string sDocNum = null;
    
        try
        {
    
            sql = Queries.With_SmmTransferItems() + @"select             
		a.CompanyId,
		a.Draft_Numero, 
		a.Fecha_Originacion, 
		a.Codigo_Origen, 
		a.Nombre_Origen, 
		a.Codigo_Destino, 
		a.Nombre_Destino, 
		a.Estatus,
		a.Dispatched , 
		a.Received, 
		a.DocEntryTraRec2, 
		a.UserDispatch, 
		a.UserReceive, 
		a.Date_Created, 
		a.Created_By,
		cast(a.LineNum as int) LineNum, 
		a.ItemCode, 
        /*[dbo].[FIVEBCODEPRODS] (a.ItemCode)*/ STUFF((SELECT ' - ' + RIGHT(BcdCode, 5) FROM " + sap_db + @".dbo.OBCD a1 " + Queries.WITH_NOLOCK + @" WHERE a1.ItemCode=a.ItemCode FOR XML PATH ('')), 1, 3, '') BarCode, 
		substring(a.ItemName,1,60) ItemName, 
		cast(a.DraftQuantity as int) DraftQuantity, 
		case when a.Dispatched = 'Y' then cast(a.DispatchQuantity as int) else 0 end DispatchQuantity,
		case when a.Received = 'Y' then cast(isnull(w1.Quantity,0) as int) else 0 end ReceivedQuantity,
		cast(a.Price as int) Price
		--from SMM_TRANSFER_ITEMS_VIEW a 
        from SmmTransferItems a
        left join {0}.DBO.WTR1 w1 on a.DocEntryTraRec2 = w1.Docentry and a.Itemcode = w1.Itemcode
		where ";

            if (int.TryParse(txtDocNum, out dummy))
            {
                Lflag1++;
                sql = string.Format(sql, sap_db, txtDocNum);
            }
            else
            {
                sql = string.Format(sql, sap_db, "-1");
            }
            
    
            ///---FromDateTxt
    
            if (string.IsNullOrEmpty(FromDateTxt) && string.IsNullOrEmpty(toDateTxt))
            {
                if (andOr1 == "OR")
                {
                    sDate = " (1=2) ";
    
                }
                else
                {
                    sDate = " (1=1) ";
                }
    
            }
            else
            {
    
                if (string.IsNullOrEmpty(FromDateTxt))
                {
    
                    FromDateTxt = "01/01/2013";
                }
                else
                {
    
                    if (FromDateTxt.Length < 1)
                    {
                        FromDateTxt = "01/01/2013";
                    }
    
                }
    
    
                if (string.IsNullOrEmpty(toDateTxt))
                {
    
                    toDateTxt = " getdate() + 1 ";
                    sDate = " (a.Fecha_Originacion between '" + FromDateTxt + "' and " + toDateTxt + ") ";
                }
                else
                {
    
                    if (toDateTxt.Length < 1)
                    {
                        toDateTxt = " getdate() + 1 ";
                        sDate = " (a.Fecha_Originacion between '" + FromDateTxt + "' and " + toDateTxt + ") ";
                    }
                    else
                    {
                        sDate = " (a.Fecha_Originacion between '" + FromDateTxt + "' and DATEADD(day,1,cast('" + toDateTxt + "' as date))) ";
    
                    }
    
    
                }
    
            }
    
            ///---toLocTxt
    
            if (fromLocTxt == "0" && toLocTxt == "0")
            {
                if (andOr2 == "OR")
                {
                    sLoc = " (1=2) ";
    
                }
                else
                {
                    sLoc = " (1=1) ";
                }
            }
            else
            {
    
                if (fromLocTxt == "0")
                {
    
                    if (andOr2 == "OR")
                    {
                        sLoc = " (1=2 and ";
                    }
                    else
                    {
                        sLoc = " (1=1 and ";
                    }
    
    
    
                }
                else
                {
    
                    sLoc = " ( a.Codigo_Origen =  '" + fromLocTxt + "' and ";
    
                }
    
    
                if (toLocTxt == "0")
                {
    
                    if (andOr2 == "OR")
                    {
                        sLoc = sLoc + " (1 = 2)) ";
                    }
                    else
                    {
                        sLoc = sLoc + " (1 = 1)) ";
                    }
    
    
                }
                else
                {
    
                    sLoc = sLoc + " a.Codigo_Destino =  '" + toLocTxt + "' ) ";
    
                }
    
            }
    
    
            ///---status and categoryTxt
            ///
    
            categoryTxt = "0";
    
            if (categoryTxt == "0" && statusDoc == "All")
            {
                if (andOr3 == "OR")
                {
                    sStatus = " (1=2) ";
    
                }
                else
                {
                    sStatus = " (1=1) ";
                }
            }
            else
            {
    
                if (categoryTxt == "0")
                {
    
                    if (andOr3 == "OR")
                    {
                        sStatus = " (1=2  ";
                    }
                    else
                    {
                        sStatus = " (1=1  ";
                    }
    
    
    
                }
                else
                {
    
                    sStatus = " (a.itmsGrpCod = '" + categoryTxt + "'";
    
    
                }
    
    
                if (statusDoc == "All")
                {
    
                    sStatus = sStatus + ") ";
    
                }
                else
                {
    
    
                    if (andOr3 == "OR")
                    {
                        sStatus = sStatus + "  OR a.Estatus =  '" + statusDoc + "' ) ";
                    }
                    else
                    {
                        sStatus = sStatus + "  AND a.Estatus =  '" + statusDoc + "' ) ";
                    }
    
    
    
                }
    
            }
    
    
            /////-------------sDocNum
    
    
            if (int.TryParse(txtDocNum, out dummy))
            {
                sDocNum = " (a.Draft_Numero =  " + txtDocNum + ") ";
                Lflag1++;
    
    
            }
            else
            {
                sDocNum = " (1=1) ";
    
            }
    
    
    
            if (doQry == "0")
            {
                doQry = " and 1=2 ";
    
    
            }
            else
            {
                doQry = " and 1=1 ";
    
            }
    
    
            if (Lflag1 == 1)
            {
                sql += sDocNum + doQry;
            }
            else
            {
                sql += sDocNum + andOr1 + sDate + andOr2 + sLoc + andOr3 + sStatus + doQry;
            }
    
    
            string sItem = null;
    
            if (string.IsNullOrEmpty (ItemCodeTbox))
            {
                sItem = " and 1=1 ";
            }
            else
            {
                sItem = " and a.ItemCode = '" + ItemCodeTbox.Trim() + "' ";
            }
    
            sql += sItem;
    
            sql += " and CompanyId = '" + sap_db + "' order by Draft_Numero, LineNum ";

            db.adapter = new SqlDataAdapter(sql, db.Conn);
            db.adapter.Fill(dt);
        }
        catch (Exception ex)
        {
            throw new Exception("Caught exception in procedure GetTransferAuditItem.  ERROR MESSAGE :" + ex.Message + " Sql: " + sql);
        }
        finally
        {
            db.Disconnect();
        }
    
        return dt;
}
    public DataTable MobS_GetToReceive(string txtDocNum,
                                       //string FromDateTxt, string toDateTxt,
                                       string fromLocTxt,  string toLocTxt,
                                       string CompanyId
                                   )
    {
        sap_db = CompanyId;
        DataTable dt = new DataTable();
        string sql = "";
        int Lflag1 = 0;

        try
        {
            //// quitar , tempo
            //sql = @"select t1.*                        
            //        from smm_draft_header_vw t1,
            //        smm_Transdiscrep_odrf t2
            //        where t1.docentry = t2.docentry
            //        and t2.docentry = 101097 ";
            //Lflag1++;  
            //// hasta aqui quitar , tempo


            sql = Queries.With_SmmDraftHeader() + @"select t1.*                        
                    --from smm_draft_header_vw t1,
                    from SmmDraftHeader t1,
                    --smm_Transdiscrep_odrf t2
                    TransdiscrepODRF t2
                    where t1.CompanyId = t2.CompanyId and t1.docentry = t2.docentry
                    and t2.DocStatus = 'O'  
                    and t2.dispatched = 'Y'
                    and isnull(t2.ScanStatus,'N') <> 'C'
                    and isnull(t2.Received,'N') <> 'Y' 
                    and t2.CompanyId = '{0}' ";

            sql = string.Format(sql, sap_db);

            if (!string.IsNullOrEmpty(txtDocNum))
            {
                sql += @" and t1.docentry =  " + txtDocNum + " ";
                Lflag1++;
            }
            else
            {

                if (fromLocTxt != "0")
                {

                    sql += @" and t1.FromLoc = '" + fromLocTxt + "' ";
                    Lflag1++;
                }

                if (toLocTxt != "0")
                {

                    sql += @" and t1.ToLoc = '" + toLocTxt + "' ";
                    Lflag1++;
                }

            }

            sql += @" order by t1.DocEntry Desc";
            
            db.adapter = new SqlDataAdapter(sql, db.Conn);

            if(Lflag1 > 0)
            {
                db.adapter.Fill(dt);
            }
            
        }
        catch (Exception ex)
        {
            throw new Exception("Caught exception in procedure MobS_GetToReceive.  ERROR MESSAGE :" + ex.Message +' '+ sql);
        }
        finally
        {
            db.Disconnect();
        }

        return dt;
    }

    public DataTable MobS_GetEntryToReceive(string txtDocNum, string CompanyId)
    {
        sap_db = CompanyId;
        DataTable dt = new DataTable();
        string sql = "";

        if (String.IsNullOrEmpty(txtDocNum))
        {
            return dt;
        }

        try
        {

            sql = @"SELECT LineNum, ToWhsCode+' - '+ToWhsName ToWhs, 
                    ItemCode+' - '+ItemName Item,
                    convert(smallint,DraftQuantity) DraftQuantity, 
                    convert(smallint,DispatchQuantity) DispatchQuantity, 
                    convert(smallint,ReceivedQuantity) ReceivedQuantity, 
                    tmpQuantity, DocEntry 
                    FROM smm_Transdiscrep_drf1
                    WHERE CompanyId = '{0}' and DocEntry = " + txtDocNum + " order by LineNum";

            sql = string.Format(sql, sap_db);

            db.adapter = new SqlDataAdapter(sql, db.Conn);

                db.adapter.Fill(dt);

        }
        catch (Exception ex)
        {
            throw new Exception("Caught exception in procedure MobS_GetToReceive.  ERROR MESSAGE :" + ex.Message + ' ' + sql);
        }
        finally
        {
            db.Disconnect();
        }

        return dt;
    }

    public DataTable MobS_GetToDispatch(string txtDocNum,
                                       //string FromDateTxt, string toDateTxt,
                                       string fromLocTxt, string toLocTxt
                                   )
    {
        DataTable dt = new DataTable();
        string sql = "";
        int Lflag1 = 0;

        try
        {
            sql = Queries.With_SmmDraftHeader() + @"select t1.*
                    --from smm_draft_header_vw t1
                    from SmmDraftHeader t1
                    left join
                    --smm_Transdiscrep_odrf t2
                    TransdiscrepODRF t2
                    on t1.CompanyId = t2.CompanyId and t1.docentry = t2.docentry                  
                    WHERE t1.DocStatus = 'O'   
					and isnull(t2.dispatched,'N') = 'N'
                    and isnull(t2.ScanDesStatus,'O') = 'O'
                    and isnull(t2.Received,'N') = 'N' 
                    and t2.CompanyId = '{0}' ";

            sql = string.Format(sql, sap_db);

            if (!string.IsNullOrEmpty(txtDocNum))
            {
                sql += @" and t1.docentry =  " + txtDocNum + " ";
                Lflag1++;
            }
            else
            {

                if (fromLocTxt != "0")
                {

                    sql += @" and t1.FromLoc = '" + fromLocTxt + "' ";
                    Lflag1++;
                }

                if (toLocTxt != "0")
                {

                    sql += @" and t1.ToLoc = '" + toLocTxt + "' ";
                    Lflag1++;
                }

            }

            sql += @" order by t1.DocEntry Desc";


            db.adapter = new SqlDataAdapter(sql, db.Conn);

            if (Lflag1 > 0)
            {
                db.adapter.Fill(dt);
            }

        }
        catch (Exception ex)
        {
            throw new Exception("Caught exception in procedure MobS_GetToDispatch.  ERROR MESSAGE :" + ex.Message + ' ' + sql);
        }
        finally
        {
            db.Disconnect();
        }

        return dt;
    }

    public DataTable MobS_getDispatch()
    {
        DataTable dt = new DataTable();

        string sql = "";

        sql = @"select DispatchId from MOB_DISPATCH where DispatchStatus = 'O' order by DispatchId desc";

        try
        {
            db.adapter = new SqlDataAdapter(sql, db.Conn);
            db.adapter.Fill(dt);

        }
        catch (Exception)
        {
            throw new Exception("Caught exception in procedure MobS_getDispatch");
        }
        finally
        {
            db.Disconnect();
        }
        
        return dt;
    }

    public DataTable GetTransdiscrepOrder(string DocEntry, string companyId)
    {
        sap_db = companyId;
        DataTable dt = new DataTable();
        string sql = "";

        try
        {
            db.Connect();

            sql = @"select DocEntry, DocNum , DocDate, 
                    FromWhsCode+' - '+FromWhsName FromWhs, 
                    ToWhsCode+' - '+ToWhsName ToWhs,
                    DocStatus, Dispatched, DispCompleted, Received, ReceCompleted, 
                    rtrim(convert(char(10),DocNumTraDis)) DocDisDis, 
                    rtrim(convert(char(10),DocNumTraRec)) DocDisRec,
                    rtrim(convert(char(10),DocNumTraRec2)) DocDisRec2,
                    userdispatch, userreceive, DispatchType, ReceiveType
                    FROM smm_Transdiscrep_odrf  
                    WHERE CompanyId = '{0}' AND DocEntry = {1}";

            sql = string.Format(sql, sap_db, DocEntry);

            db.adapter = new SqlDataAdapter(sql, db.Conn);
            db.adapter.Fill(dt);
        }
        catch (Exception ex)
        {
            throw new Exception("Caught exception in procedure GetTransdiscrepOrder.  ERROR MESSAGE :" + ex.Message);
        }
        finally
        {
            db.Disconnect();
        }

        return dt;
    }

    public DataTable GetTransdiscrepOrderDtl(string DocEntry, string companyId)
    {
        sap_db = companyId;
        DataTable dt = new DataTable();
        string sql = "";

        try
        {
            db.Connect();

            sql = @"SELECT LineNum, ToWhsCode+' - '+ToWhsName ToWhs, 
                    ItemCode+' - '+ItemName Item, /*[dbo].[FIVEBCODEPRODS] (ItemCode)*/ STUFF((SELECT ' - ' + RIGHT(BcdCode, 5) FROM " + sap_db + @".dbo.OBCD a1 " + Queries.WITH_NOLOCK + @" WHERE a1.ItemCode=smm_Transdiscrep_drf1.ItemCode FOR XML PATH ('')), 1, 3, '') BarCode, 
                    convert(int,DraftQuantity) DraftQuantity, 
                    convert(int,DispatchQuantity) DispatchQuantity, 
                    convert(int,ReceivedQuantity) ReceivedQuantity, 
                    convert(int,tmpQuantity) tmpQuantity, userrecscanner, DocEntry 
                    FROM smm_Transdiscrep_drf1    
                    WHERE CompanyId = '{0}' AND DocEntry = {1}";

            sql = string.Format(sql, sap_db, DocEntry);

            db.adapter = new SqlDataAdapter(sql, db.Conn);
            db.adapter.Fill(dt);
        }
        catch (Exception ex)
        {
            throw new Exception("Caught exception in procedure GetTransdiscrepOrderDtl.  ERROR MESSAGE :" + ex.Message);
        }
        finally
        {
            db.Disconnect();
        }

        return dt;
    }

    public DataTable GetTransXsapDtl(string DocEntry, string companyId)
    {
        sap_db = companyId;
        DataTable dt = new DataTable();
        string sql = "";

        try
        {

            sql = Queries.With_SmmTranSapDRF1() + @"
SELECT [LineNum], [ItemCode], /*[dbo].[FIVEBCODEPRODS] (ItemCode)*/ STUFF((SELECT ' - ' + RIGHT(BcdCode, 5) FROM " + sap_db + @".dbo.OBCD a1 " + Queries.WITH_NOLOCK + @" WHERE a1.ItemCode=SmmTranSapDRF1.ItemCode FOR XML PATH ('')), 1, 3, '') BarCode, [ItemName], [DraftQuantity], [DocEntry], [onhand] 
FROM SmmTranSapDRF1 " + Queries.WITH_NOLOCK + @" 
WHERE DocEntry = {1} and CompanyId = '{0}' ORDER BY [LineNum]";

            sql = string.Format(sql, sap_db, DocEntry);
            db.adapter = new SqlDataAdapter(sql, db.Conn);
            db.adapter.SelectCommand.CommandTimeout = 600; // 10 minute timeout
            db.adapter.Fill(dt);
        }
        catch (Exception ex)
        {
            throw new Exception("Caught exception in procedure GetTransXsapDtl.  ERROR MESSAGE :" + ex.Message+" - "+sql);
        }
        finally
        {
            db.Disconnect();
        }

        return dt;
    }
    public DataTable TocLec_getEntriesBines(string PackingId)
    {
        DataTable dt = new DataTable();
        string sql = "";

        if (string.IsNullOrEmpty(PackingId))
        {
            return dt;
        }

        try
        {
            sql = @"select PackingOrdBinId, PackingId, WmsBin,CintilloBin1,CintilloBin2, OSDocNum 
                      from TOCLEC_PACKING_ORDEN_BINS 
                     WHERE PackingId = " + PackingId + @"
                       ORDER BY WmsBin";

            db.adapter = new SqlDataAdapter(sql, db.Conn);
            db.adapter.Fill(dt);

        }

        catch (Exception ex)
        {
            throw new Exception("Caught exception in function TocLec_getEntriesBines" + ex.Message + ' ' + sql);

        }

        finally
        {

            db.Disconnect();
        }

        return dt;


    }

    public DataTable TocLec_getReceivedBins(string ReceivingId)
    {
        DataTable dt = new DataTable();
        string sql = "";


        try
        {
            sql = @"select a.OSDocNum, a.WmsBin, a.CintilloBin1, a.CintilloBin2, 
                      b.ReceivingId, b.ReceivingOrdBinId, b.PackingOrdBinId
	                    from dbo.TOCLEC_PACKING_ORDEN_BINS a,
                                dbo.TOCLEC_RECEIVING_ORDEN_BINS b
	                    where a.[PackingOrdBinId] = b.[PackingOrdBinId]
	                      and b.ReceivingId = " + ReceivingId + @" Order by a.OSDocNum, a.WmsBin ";

            db.adapter = new SqlDataAdapter(sql, db.Conn);
            db.adapter.Fill(dt);

        }

        catch (Exception ex)
        {
            throw new Exception("Caught exception in function TocLec_getReceivedBins" + ex.Message + ' ' + sql);

        }

        finally
        {

            db.Disconnect();
        }

        return dt;


    }

    public DataTable TocLec_getUbiOrigen()
    {
        DataTable dt = new DataTable();

        string sql = "";

        sql = @"select WHSCODE from [dbo].[TOCLEC_OWHS_CONTROL]
                WHERE Control = 'FROM_PACKING'
                 order by WHSCODE ";

        try
        {
            db.adapter = new SqlDataAdapter(sql, db.Conn);
            db.adapter.Fill(dt);

        }

        catch (Exception ex)
        {
            throw new Exception("Caught exception in procedure TocLec_getUbiOrigen" + ex.Message + ' ' + sql);


        }

        finally
        {
            db.Disconnect();
        }

        return dt;


    }

    public DataTable TocLec_GetOpenPackingId()
    {
        DataTable dt = new DataTable();

        
        string sql = "";

        sql = @"select PackingId from [dbo].[TOCLEC_PACKING]
                WHERE PackingStatus = 'O'  order by PackingId desc";

        try
        {
            db.adapter = new SqlDataAdapter(sql, db.Conn);
            db.adapter.Fill(dt);

        }

        catch (Exception ex)
        {
            throw new Exception("Caught exception in procedure TocLec_GetOpenPackingId" + ex.Message + ' ' + sql);


        }

        finally
        {
            db.Disconnect();
        }

        return dt;


    }

    public DataTable TocLec_GetOpenReceivingId(string ReceivingWhscode)
    {
        DataTable dt = new DataTable();


        string sql = "";

        sql = @"select ReceivingId from [dbo].[TOCLEC_Receiving]
                WHERE ReceivingStatus = 'O' and ReceivingWhscode = '"+ ReceivingWhscode + @"' order by ReceivingId desc";

        try
        {
            db.adapter = new SqlDataAdapter(sql, db.Conn);
            db.adapter.Fill(dt);

        }

        catch (Exception ex)
        {
            throw new Exception("Caught exception in procedure TocLec_GetOpenReceivingId" + ex.Message + ' ' + sql);
        }

        finally
        {
            db.Disconnect();
        }

        return dt;


    }

}
