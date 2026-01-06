using System;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Xml;
//using SAPbobsCOM;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;


/// <summary>
/// Summary description for SqlObjs
/// </summary>
public class SqlObjs
{
    protected SqlDb db;
    protected string sap_db;
    protected string whs_code;

    public SqlObjs()
    {
        db = new SqlDb();
        db.Connect();

        sap_db = ConfigurationSettings.AppSettings.Get("sap_db");
        whs_code = ConfigurationSettings.AppSettings.Get("whs_code");
    }

    public DataTable GetCompanies()
    {

        db.Connect();

        DataTable dt = new DataTable();

        string sql = "";

        sql = @"select 0 as CompanyId, 'Selecciona' as Companycode, 'Selecciona' as CompanyName
                union
                select CompanyId, Companycode, Companycode + ' - ' + CompanyName as CompanyName
	                from  [dbo].[SMM_COMPANIES]
                order by 1";

        try
        {
            db.adapter = new SqlDataAdapter(sql, db.Conn);
            db.adapter.Fill(dt);

        }

        catch (Exception ex)
        {
            throw new Exception("Caught exception in procedure GetCompanies" + ex.Message + ' ' + sql);


        }

        finally
        {
            db.Disconnect();
        }

        return dt;


    }


}

