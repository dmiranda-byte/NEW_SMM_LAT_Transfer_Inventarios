using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;


public class WMS
{
    protected SqlDb db;
    protected string sap_db;

    public WMS()
    {
        db = new SqlDb();
        db.Connect();
        //sap_db = ConfigurationManager.AppSettings["sap_db"].ToString();
    }

    public DataTable GetItemBin(string store, string depts, string brands, string lidArticulo, string lidBinTBox)
    {
        DataTable dt = new DataTable();

        //sap_db = CompanyId;

         string sql = null;

        if (!string.IsNullOrEmpty(lidBinTBox))
        {

            sql = @"select whscode, whsname, itemcode, itemname, itmsgrpcod, 
                       itmsgrpnam, u_brand, bin
	                   from [Wms_Whs_Item_Bin_vw]
                       where WhsCode = '" + store + @"' and 
                             bin = ltrim(rtrim('" + lidBinTBox + @"'))";
        }
        else
        {

            if (!string.IsNullOrEmpty(lidArticulo))
            {

                sql = @"select whscode, whsname, itemcode, itemname, itmsgrpcod, 
                       itmsgrpnam, u_brand, bin
	                   from [Wms_Whs_Item_Bin_vw]
                       where WhsCode = '" + store + @"' and 
                             itemcode = ltrim(rtrim('" +lidArticulo + @"'))";


            }
            else
            {

                if (depts == null || store == null || brands == null)
                    return dt;

                sql = @"select whscode, whsname, itemcode, itemname, itmsgrpcod, 
                       itmsgrpnam, u_brand, bin
	                   from [Wms_Whs_Item_Bin_vw]
                       where WhsCode = '" + store + @"' and 
                             itmsgrpcod in (" + depts + @")";


                if (brands != "'All Brands'")
                    sql += " and replace(u_brand, '''','_') in (" + brands + @")";

                

            }
        }

        sql += "    order by itemcode desc";




        try
        {
            db.adapter = new SqlDataAdapter(sql, db.Conn);
            db.adapter.Fill(dt);
        }
        catch (Exception ex)
        {
            throw new Exception("Caught exception in function GetItemBin. MESSAGE : " + ex.Message);
        }
        finally
        {
            db.Disconnect();
        }
        return dt;
    }
}

