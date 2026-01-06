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

/// <summary>
/// Summary description for SqlDb
/// </summary>
public class SqlDb
{
    protected string connStr;
    public SqlConnection Conn;
    public SqlCommand cmd;
    public SqlDataReader rdr;
    public SqlDataAdapter adapter;
    public SqlDataAdapter adapter2; 
    public System.Data.DataSet dataSet;

    //2019-ABR-09: Añadido por Aldo Reina para poder verificar el estado de la base de datos:
    public ConnectionState DbConnectionState
    {
        get
        {
            if (Conn != null)
            {
                return Conn.State;
            }
            else
            {
                return ConnectionState.Closed;
            }
        }
    }

    public void SISINV_GET_ACCESSTYPE_PRC(string lCurUser, string lControlName, ref string strAccessType, ref string strRole_Description)
    {
        try
        {
            cmd.Parameters.Clear();
            cmd.CommandText = "dbo.SISINV_GET_ACCESSTYPE_PRC";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = Conn;

            cmd.Parameters.Add(new SqlParameter("@LoginID", SqlDbType.VarChar));
            cmd.Parameters["@LoginID"].Value = lCurUser;

            cmd.Parameters.Add(new SqlParameter("@ControlName", SqlDbType.VarChar));
            cmd.Parameters["@ControlName"].Value = lControlName;

            SqlParameter lAccessType = new SqlParameter("@AccessType", SqlDbType.VarChar);
            lAccessType.Direction = ParameterDirection.Output;
            lAccessType.Size = 100000;
            cmd.Parameters.Add(lAccessType);

            SqlParameter lRole_Description = new SqlParameter("@Role_Description", SqlDbType.VarChar);
            lRole_Description.Direction = ParameterDirection.Output;
            lRole_Description.Size = 100000;
            cmd.Parameters.Add(lRole_Description);
            cmd.ExecuteNonQuery();

            strAccessType = cmd.Parameters["@AccessType"].Value.ToString();
            strRole_Description = cmd.Parameters["@Role_Description"].Value.ToString();
        }
        catch (Exception ex)
        {
            Disconnect();
            throw new Exception("Error when SISINV_GET_ACCESSTYPE_PRC was called: " + ex.Message);
        }
    }

    public SqlDb()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public void Connect()
    {
        try
        {
            connStr = ConfigurationManager.ConnectionStrings["smm_latConnectionString"].ConnectionString;

            this.Conn = new SqlConnection();
            this.cmd = new SqlCommand();
            this.adapter = new SqlDataAdapter();
            this.dataSet = new DataSet();

            this.Conn.ConnectionString = connStr;
            this.cmd.Connection = this.Conn;

            if (DbConnectionState == ConnectionState.Closed)
            {
                Conn.Open();
            }
        }
        catch (Exception)
        {
            HttpContext.Current.Response.Redirect("AccessDenied.aspx", true);
        }
    }

    //2019-ABR-09: Añadido por Aldo Reina para cerrar la conexión a la base de datos:
    public void Disconnect()
    {
        try
        {
            Conn.Close();
        }
        catch (Exception)
        {

        }
    }



    //2019-ABR-09: Añadido por Aldo Reina, para la búsqueda por código de barras. Se colocó esta función acá 
    //porque será utilizada en varios lugares, y es mejor escribirla una vez que varias veces:
    public DataTable SearchItemByBarCodes(string companyId, string barCode)
    {
        DataTable dt = new DataTable();
        try
        {
            string queryString = "";
            queryString = queryString + "SELECT ";
            queryString = queryString + "ItemCode = b.ItemCode, ";
            queryString = queryString + "ItemName = LTRIM(RTRIM(b.ItemCode)) + ' | ' + LTRIM(RTRIM(b.itemname)) ";
            queryString = queryString + "FROM {0}..OBCD a " + Queries.WITH_NOLOCK + " ";
            queryString = queryString + "INNER JOIN {0}..OITM b " + Queries.WITH_NOLOCK + " on b.ItemCode = a.ItemCode ";
            queryString = queryString + "WHERE a.BcdCode = '{1}'";
            queryString = String.Format(queryString, companyId, barCode);

            adapter = new SqlDataAdapter(queryString, Conn);
            adapter.Fill(dt);
        }
        catch (Exception ex)
        {
            Disconnect();
            throw new Exception("Error when SearchItemByBarCodes was called: " + ex.Message);
        }
        return dt;
    }

    public DataTable SearchBines(string companyId, string Bin)
    {
        DataTable dt = new DataTable();
        try
        {
            string queryString = "";
            queryString = queryString + "SELECT ";
            queryString = queryString + "ItemCode = b.ItemCode, ";
            queryString = queryString + "ItemName = LTRIM(RTRIM(b.ItemCode)) + ' | ' + LTRIM(RTRIM(b.itemname)) ";
            queryString = queryString + "FROM {0}..OBCD a " + Queries.WITH_NOLOCK + " ";
            queryString = queryString + "INNER JOIN {0}..OITM b " + Queries.WITH_NOLOCK + " on b.ItemCode = a.ItemCode ";
            queryString = queryString + "WHERE a.BcdCode = '{1}'";
            queryString = String.Format(queryString, companyId, Bin);

            adapter = new SqlDataAdapter(queryString, Conn);
            adapter.Fill(dt);
        }
        catch (Exception ex)
        {
            Disconnect();
            throw new Exception("Error when SearchItemByBarCodes was called: " + ex.Message);
        }
        return dt;
    }

    public DataTable SearchItems(string companyId, string Item)
    {
        DataTable dt = new DataTable();
        try
        {
            string queryString = "";
            queryString = queryString + "SELECT ";
            queryString = queryString + "ItemCode = b.ItemCode, ";
            queryString = queryString + "ItemName = LTRIM(RTRIM(b.ItemCode)) + ' | ' + LTRIM(RTRIM(b.itemname)) ";
            queryString = queryString + "FROM {0}..OBCD a " + Queries.WITH_NOLOCK + " ";
            queryString = queryString + "INNER JOIN {0}..OITM b " + Queries.WITH_NOLOCK + " on b.ItemCode = a.ItemCode ";
            queryString = queryString + "WHERE a.BcdCode = '{1}'";
            queryString = String.Format(queryString, companyId, Item);

            adapter = new SqlDataAdapter(queryString, Conn);
            adapter.Fill(dt);
        }
        catch (Exception ex)
        {
            Disconnect();
            throw new Exception("Error when SearchItemByBarCodes was called: " + ex.Message);
        }
        return dt;
    }

    public DataTable GetWhsByCiaIdAndControl(string sap_db, string control, string whsTypes)
    {
        DataTable dt = new DataTable();
        try
        {
            string queryString = "SELECT a.WhsCode AS WhsCode, LTRIM(RTRIM(a.WhsName)) AS WhsName ";
            queryString += " FROM {0}..OWHS a " + Queries.WITH_NOLOCK + " ";
            queryString += " INNER JOIN dbo.RSS_OWHS_CONTROL b " + Queries.WITH_NOLOCK + " ";
            queryString += " ON a.WhsCode = b.Whscode ";
            queryString += " INNER JOIN dbo.SMM_WHSTYPE c " + Queries.WITH_NOLOCK + " ";
            queryString += " ON a.WhsCode = c.WHSCODE ";
            queryString += " WHERE 1=1 ";
            queryString += " AND b.CompanyId = '{0}'";
            queryString += " AND c.COMPANYID = '{0}'";

            if (!string.IsNullOrEmpty(control))
            {
                queryString += " AND b.[Control] = '" + control + "'";
            }

            if (!string.IsNullOrEmpty(whsTypes))
            {
                string whsTypes2 = "";

                string[] whsTypesArray = whsTypes.Split(",".ToCharArray());

                foreach (string item in whsTypesArray)
                {
                    whsTypes2 += ",'" + item + "'";
                }

                if(!string.IsNullOrEmpty(whsTypes2))
                {
                    whsTypes2 = whsTypes2.Substring(1);
                }
                queryString += " AND c.TYPEWHS IN (" + whsTypes2 + ")";
            }

            queryString += " ORDER BY a.WhsCode ";

            queryString = String.Format(queryString, sap_db);

            adapter = new SqlDataAdapter(queryString, Conn);
            adapter.Fill(dt);
        }
        catch (Exception ex)
        {
            Disconnect();
            throw new Exception("Error when GetWhsByCiaIdAndControl was called: " + ex.Message);
        }
        return dt;
    }

    public DataTable GetInventarioDeTiendasTocumen(string v_Marca, string v_Grupo, string CompanyId, string whsType = "TODAS")
    {
        DataTable dt = new DataTable();
        string sap_db = CompanyId;

        try
        {
            Connect();
            cmd.Parameters.Clear();
            cmd.CommandText = "SP_INVENTORY_SAP_Location_dfa";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@MARCA", SqlDbType.VarChar)).Value = v_Marca;
            cmd.Parameters.Add(new SqlParameter("@GRUPO", SqlDbType.VarChar)).Value = v_Grupo;
            cmd.Parameters.Add(new SqlParameter("@CompanyId", SqlDbType.NVarChar)).Value = sap_db;

            if (whsType != "TODAS")
            {
                cmd.Parameters.Add(new SqlParameter("@WhsType", SqlDbType.NVarChar)).Value = whsType;
            }

            dt.Load(cmd.ExecuteReader());
        }
        catch (Exception ex)
        {
            dt.Columns.Add("ErrMsg", typeof(string));
            dt.Rows.Add(ex.Message);
            return dt;
        }
        finally
        {
            Disconnect();
        }
        return dt;
    }

    public DataTable GetOrdenesConProblemas(DateTime? v_Period, string sap_db)
    {
        DataTable dt = new DataTable();

        try
        {
            Connect();
            cmd.Parameters.Clear();
            cmd.CommandText = "sp_ordenesProblem";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(new SqlParameter("@CompanyId", SqlDbType.NVarChar)).Value = sap_db;

            if(v_Period.HasValue && v_Period.Value != null && v_Period.Value.ToString() != "")
            {
                cmd.Parameters.Add(new SqlParameter("@fperiodo", SqlDbType.SmallDateTime)).Value = v_Period.Value;
            }

            dt.Load(cmd.ExecuteReader());
        }
        catch (Exception ex)
        {
            dt.Columns.Add("ErrMsg", typeof(string));
            dt.Rows.Add(ex.Message);
            return dt;
        }
        finally
        {
            Disconnect();
        }
        return dt;
    }

    public DataTable GetGrupo(string CompanyId)
    {
        string sap_db = CompanyId;
        DataTable dt = new DataTable();

        try
        {
            Connect();

            cmd.CommandText = "select ItmsGrpCod,ItmsGrpNam from " + sap_db + ".dbo.OITB";
            cmd.CommandType = CommandType.Text;
            dt.Load(cmd.ExecuteReader());
        }
        catch (Exception ex)
        {
            dt.Columns.Add("ErrMsg", typeof(string));
            dt.Rows.Add(ex.Message);
            return dt;
        }
        finally
        {
            Disconnect();
        }
        return dt;
    }
    public DataTable GetMarcas(string v_Grupo, string CompanyId)
    {
        DataTable dt = new DataTable();

        try
        {
            Connect();

            cmd.CommandText = "select 'TODAS' u_Brand union all select distinct u_Brand from " + CompanyId + ".dbo.oitm " + Queries.WITH_NOLOCK + " where ItmsGrpCod in (" + v_Grupo + ") and u_Brand <> '' order by u_Brand";
            cmd.CommandType = CommandType.Text;
            dt.Load(cmd.ExecuteReader());
        }
        catch (Exception ex)
        {
            dt.Columns.Add("ErrMsg", typeof(string));
            dt.Rows.Add(ex.Message);
            return dt;
        }
        finally
        {
            Disconnect();
        }
        return dt;
    }
}
