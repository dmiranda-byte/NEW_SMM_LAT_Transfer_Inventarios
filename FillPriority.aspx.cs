using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using Telerik;
using Telerik.Web.UI;
using System.Data.SqlClient;
using System.Drawing;

public partial class FillPriority : System.Web.UI.Page
{
    protected SqlDb db = new SqlDb();
    protected DFBUYINGdb dfbDB = new DFBUYINGdb();
    protected string lCurUser; 
    public DataTable dt = new DataTable();
    protected string sap_db;
    private static string TOOLTIP_TEMPLATE = @"
                <div class=""container"" style=""width:200px;margin-left:2px;"">
                    <div class=""row"" style=""margin-left:2px;"">Store: {0}</div>
                    <div class=""row"" style=""margin-left:2px;"">Active Promos: {1}</div>
                </div>";
    protected void Page_Load(object sender, EventArgs e)
    {
        if ((string)Session["UserId"] == "" || (string)Session["UserId"] == null)
        {
            Response.Redirect("Login1.aspx");
        }

        if ((string)Session["CompanyId"] == "" || (string)Session["CompanyId"] == null)
        {
            Response.Redirect("Login1.aspx");
        }

        sap_db = (string)Session["CompanyId"];

        ///////////////Begin New  Control de acceso por Roles
        lCurUser = (string)Session["UserId"];
        char flagokay = 'Y';
	    string lControlName = "FillPriority.aspx";
        string strAccessType = "";
        string strRole_Description = "";

        db.Connect();
        db.SISINV_GET_ACCESSTYPE_PRC(lCurUser, lControlName, ref strAccessType, ref strRole_Description);
        db.Disconnect();

        if (strAccessType == "N")
	    {
	        flagokay = 'N';
	        string message = "EL Usuario " + lCurUser + ", con Rol " + strRole_Description + " no tiene permisos para entrar a esta pantalla.";
	        string url = string.Format("Default.aspx");
	        string script = "{ alert('";
	        script += message;
	        script += "');";
	        script += "window.location = '";
	        script += url;
	        script += "'; }";
	        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "alert", script, true);
	    }

	    if (strAccessType == "R")
		{
		    rbtnSave.Enabled = false;
		    rbtnSave.ForeColor = Color.Silver;	    
		    labelForm.InnerText = "FillPriority (Acceso solo Lectura)";       
		}
		
	    if (strAccessType == "F")
		{
		    rbtnSave.Enabled = true;
		    labelForm.InnerText = "FillPriority (Acceso Completo)";     
		}		
        ///////////////End  New Control de acceso por Roles

	    if (flagokay == 'Y')
        {
		    try
		    {
		        if (!IsPostBack)
		        {
			        if ((string)this.Session["UserId"] == "" || (string)this.Session["UserId"] == null)
			        {
			            Response.Redirect("Login1.aspx");
			        }
			        else
			        {
                        LoadItemGroup();
			            LoadLocations();
			        }
		        }
		    }
		    catch (Exception ex)
		    {
		        ShowMasterPageMessage("Error", "Failed in Page_Load", ex.Message.ToString());
		        return;
		    }
        }
    }
    
    private void ShowMasterPageMessage(string v_Message_Type, string v_Message_Title, string v_Message)
    {
        try
        {
            SiteMaster sm = (SiteMaster)this.Master;
            sm.ShowDivMessage(v_Message_Type, v_Message_Title, v_Message);
        }
        catch (Exception ex)
        {
            ShowMasterPageMessage("Error", "Failed in ShowMasterPageMessage", ex.Message.ToString());
            return;
        }
    }
    private void LoadItemGroup()
    {
        DataTable dt = new DataTable();

        try
        {
            string sql =
            @"select 
               ItmsGrpCod GroupCode, 
                lower(ItmsGrpNam) GroupID,
               cast(ItmsGrpCod as varchar) + ' - ' + [dbo].[InitCap] (ItmsGrpNam) GroupName 
               --[dbo].[InitCap] (ItmsGrpNam) GroupName 
             from " + sap_db + @".dbo.oitb
             order by GroupName ";
            db.Connect();
            db.adapter = new SqlDataAdapter(sql, db.Conn);
            db.adapter.Fill(dt);

            rcbCategory.DataTextField = "GroupName";
            rcbCategory.DataValueField = "GroupCode";
            rcbCategory.DataSource = dt;
            rcbCategory.DataBind();
        }
        catch (Exception ex)
        {
            throw new Exception("Caught exception in function LoadItemGroup. ERROR MESSAGE : " + ex.Message);
        }
    }
    private void LoadLocations()
    {

        DataTable dt = new DataTable();

        try
        {
            //string sql = "SELECT COMPANYCODE, WHSCODE, WHSNAME, WHSCODE + ' - ' + WHSNAME WHS FROM COMPANY_WHS_VW WHERE LOWER(COMPANYCODE) = '" + sap_db.ToLower() + "'";
            //db.Connect();
            //db.adapter = new SqlDataAdapter(sql, db.Conn);
            //db.adapter.Fill(dt);

            //rcbWhs.DataTextField = "WHS";
            //rcbWhs.DataValueField = "WHSCODE";
            //rcbWhs.DataSource = dt;
            //rcbWhs.DataBind();
        }
        catch (Exception ex)
        {
            throw new Exception("Caught exception in function LoadItemGroup. ERROR MESSAGE : " + ex.Message);
        }
    }

    protected void rcbCategory_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
    {
        try
        {
            if (rcbCategory.SelectedValue != "")
            {
                rgPriority.DataSource = GetFillPriority();
                rgPriority.Rebind();
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Caught exception in function Category Index Changed. ERROR MESSAGE : " + ex.Message);
        }
    }
    private DataTable GetFillPriority()
    {
        DataTable dt = new DataTable();

        try
        {
            string sql = "select a.Company, a.[Location], b.WhsName AS LocationName, a.Dept, a.[Priority] from Repln_Location_Priority a " + Queries.WITH_NOLOCK + @"  INNER JOIN " + sap_db + ".dbo.OWHS b " + Queries.WITH_NOLOCK + @"  ON a.Location = b.WhsCode where lower(company) = lower('" + sap_db + "') and dept = " + rcbCategory.SelectedValue + " order by dept, priority";
            db.Connect();
            db.adapter = new SqlDataAdapter(sql, db.Conn);
            db.adapter.Fill(dt);
             
            //rgPriority.DataSource = dt;
            //rgPriority.Rebind();
        }
        catch (Exception ex)
        {
            throw new Exception("Caught exception in function LoadItemGroup. ERROR MESSAGE : " + ex.Message);
        }
        finally
        {
            db.Disconnect();
        }
        return dt;
    }

    //protected void rcbCompany_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
    //{
    //    if (rcbCompany.SelectedValue != "" && rcbCategory.SelectedValue != "")
    //    {
    //        rgPriority.DataSource = GetFillPriority();
    //        rgPriority.Rebind();
    //    }
    //}

    protected void rgPriority_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
    {
        if (rcbCategory.SelectedValue != "")
        {
            divGrid.Attributes.Add("style", "display:block");
            rgPriority.DataSource = GetFillPriority();
        }
    }
    protected void rgPriority_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            GridDataItem dataItem = e.Item as GridDataItem;
            RadComboBox rcbPriority = (RadComboBox)dataItem["Priority"].FindControl("rcbPriority");
            HiddenField hfPriority = (HiddenField)dataItem["Priority"].FindControl("hfPriority");

            rcbPriority.SelectedValue = hfPriority.Value;
        }
    }
    protected void rbtnSave_Click(object sender, EventArgs e)
    {
        if (rgPriority.MasterTableView.Items.Count > 0)
        {
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[2]{
                           new DataColumn("Location", typeof(string )),
                           new DataColumn("Priority", typeof(string))});

            foreach (GridDataItem item in rgPriority.MasterTableView.Items)
            {
                string v_Loc = item["LOCATION"].Text;
                RadComboBox rcbPriority = (RadComboBox)item["Priority"].FindControl("rcbPriority");
                string v_Priority = rcbPriority.SelectedValue;

                dt.Rows.Add(v_Loc, v_Priority);
            }

            var duplicates = dt.AsEnumerable().GroupBy(r => r[1]).Where(gr => gr.Count() > 1).ToList();
            if (duplicates.Count > 0)
            {
                ShowMasterPageMessage("Error", "Failed to save priorities", "Priorities cannot be saved as there are some duplicates.");
            }
            else
            {
                if (rcbCategory.SelectedValue != "")
                {
                    db.Connect();
                    foreach (DataRow dr in dt.Rows)
                    {  
                        string sql = "update Repln_Location_Priority set Priority = " + dr["Priority"] + " where lower(company) = lower('" + sap_db.ToString() + "') and dept = " + rcbCategory.SelectedValue + " and Location = '" + dr["Location"] + "'";

                        //db.adapter = new SqlDataAdapter(sql, db.Conn);
                        db.cmd.CommandText = sql;
                        db.cmd.CommandType = CommandType.Text;
                        db.cmd.ExecuteNonQuery();
                    }
                    db.Disconnect();

                    ShowMasterPageMessage("Ok", "Success", "Priorities updated successfully.");
                    rgPriority.Rebind();
                }
                else
                {
                    ShowMasterPageMessage("Error", "Error", "Please refresh the page and try again.");
                }
            }
        }
        else
        {
            ShowMasterPageMessage("Error", "Error", "No data to save.");
        }
    }
}