using System;
using System.Data;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public partial class TransferDiscreOrdf : System.Web.UI.Page
{
    protected SqlDb db = new SqlDb();
    protected string sap_db;

    private static readonly HashSet<string> AllowedUsers =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        { "YFELICIANO", "AGALVEZ", "MMARTINEZBOD", "AABREGO", "DGILTIE", "YFELICIANOBOD", "WFERNANDEZBOD", "WFERNANDEZ" ,"ICASTILLO" ,"ICASTILLOBOD"};

    string LvParameters = null;
    string LvDispatched = null;
    string LvReceived = null;
    string LvUserDisp = null;
    int GloVarDocEntry = 0;
    char flagPerDeskay = 'N';
    char flagPerReckay = 'N';
    char GloVarDesRec = 'X';


    protected void Page_Load(object sender, EventArgs e)
    {
        LabelCurUser.Text = "Usuario: " + (string)this.Session["UserId"];
	if (!IsPostBack){
            var user = (string)this.Session["UserId"];   // e.g. "YFELICIANO" HACE VISIBLE USUARIO PERMITIDOS
            ZeroCheckBox.Visible = AllowedUsers.Contains(user);
			}
    }

    protected void Panel1_Load(object sender, EventArgs e)
    {
        if ((string)this.Session["UserId"] == "" || (string)this.Session["UserId"] == null)
        {
            Response.Redirect("Login1.aspx");
        }

        sap_db = (string)this.Session["CompanyId"];
        CompanyLabel.Text = sap_db;

        ArrayList controles = new ArrayList();
        controles = (ArrayList)this.Session["Controles"];

        ArrayList roles = new ArrayList();
        roles = (ArrayList)this.Session["Roles"];

        ArrayList permissions = new ArrayList();
        permissions = (ArrayList)this.Session["Permissions"];

        string thiscontrol = "";
        char flagokay = 'N';

        for (int i = 0; i < controles.Count; i++)
        {
            thiscontrol = (controles[i].ToString());
            if ((thiscontrol == "TransferDiscreOrdf.aspx") || (thiscontrol == "ATOTAL"))
            {
                flagokay = 'Y';
            }
        }

        if (flagokay == 'N')
        {
            Response.Write("<script type=\"text/javascript\">alert('" + "Este Usuario no tiene permisos para esta opción, favor de registrarse con otro usuario." + "');</script>");
            Response.End();
        }

        string thispermission = "";

        for (int i = 0; i < permissions.Count; i++)
        {
            thispermission = "";
            thispermission = (permissions[i].ToString());

            if ((thispermission == "DESPATCH") || (thispermission == "ATOTAL"))
            {
                flagPerDeskay = 'Y';
            }
        }

        for (int i = 0; i < permissions.Count; i++)
        {
            thispermission = "";
            thispermission = (permissions[i].ToString());

            if ((thispermission == "RECEIVE") || (thispermission == "ATOTAL"))
            {
                flagPerReckay = 'Y';
            }
        }

        db.Connect();

        string DocEntry = "";

        if (Request.QueryString["DocEntry"] == null)
        {
            db.Disconnect();
            Response.Write("ERROR: No document entry number specified in querystring.<br>");
            Response.End();
        }
        else
        {
            DocEntry = Request.QueryString["DocEntry"].ToString();
            GloVarDocEntry = Convert.ToInt32(DocEntry);
            DocEntryLabel.Text = DocEntry;

            //btnPrint.OnClientClick = "window.open('DisTransferDetails.aspx?DocEntry=" + DocEntry + "','PrintWindow','status=0,toolbar=0,resizable=1,scrollbars=1')";

            List<DocumentsPrint> docEntries = new List<DocumentsPrint>
            {
                new DocumentsPrint(DocEntry, 1)
            };

            Session["docEntries"] = docEntries;

            btnPrint.OnClientClick = "window.open('DisTransferDetailsPrint.aspx?DocEntry=" + DocEntry + "','PrintWindow','status=0,toolbar=0,resizable=1,scrollbars=1')";
			
            DataTable lDataTable;
            DataSet lDataSet = new DataSet();
            SqlCommand sqlCommand = new SqlCommand();

            string sql = Queries.With_SmmDraftHeader() + @"
select docstatus from SmmDraftHeader where docentry = {1}";

            sql = string.Format(sql, sap_db, DocEntry);

            sqlCommand.CommandText = sql;
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.Connection = db.Conn;

            SqlDataAdapter lSqlDataAdapter = new SqlDataAdapter
            {
                SelectCommand = sqlCommand
            };

            //lSqlDataAdapter.Fill(lDataSet, "smm_draft_header_vw");
            //lDataTable = lDataSet.Tables["smm_draft_header_vw"];

            lSqlDataAdapter.Fill(lDataSet, "SmmDraftHeader");
            lDataTable = lDataSet.Tables["SmmDraftHeader"];

            string docstatus = "";

            foreach (DataRow lDataRow in lDataTable.Rows)
            {
                docstatus = Convert.ToString(lDataRow["docstatus"]);
            }

            if (docstatus == "O")
            {
                try
                {
                    sqlCommand.Parameters.Clear();
                    sqlCommand.CommandText = "smm_populate_discrep_odrf";
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@DocEntry", SqlDbType.Int));
                    sqlCommand.Parameters["@DocEntry"].Value = Convert.ToInt32(DocEntry);
                    sqlCommand.Parameters.Add(new SqlParameter("@CompanyId", SqlDbType.NVarChar));
                    sqlCommand.Parameters["@CompanyId"].Value = sap_db;
                    sqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Response.Write("Error when smm_populate_discrep_odrf was called.");
                    Response.Write(ex.Message);
                }
                finally
                {
                    db.Disconnect();
                }               
            }
            else
            {
                try
                {
                    
                    sqlCommand.CommandText = "select count(1) numrows from smm_Transdiscrep_odrf where CompanyId = '" + sap_db + "' and docentry = " + DocEntry;
                    sqlCommand.CommandType = CommandType.Text;
                    sqlCommand.Connection = db.Conn;

                    lSqlDataAdapter.SelectCommand = sqlCommand;

                    lSqlDataAdapter.Fill(lDataSet, "smm_Transdiscrep_odrf");
                    lDataTable = lDataSet.Tables["smm_Transdiscrep_odrf"];


                    string Lnumrows = "";

                    foreach (DataRow lDataRow in lDataTable.Rows)
                    {
                        Lnumrows = Convert.ToString(lDataRow["numrows"]);
                    }

                    int Lnmrows = Convert.ToInt32(Lnumrows);

                    if (Lnmrows == 0)
                    {
                        db.Disconnect();
                        Response.Write("Nota: Este Transfer No fue realizado por Control de Discrepancias.<br>");
                        Response.End();
                    }
                    else
                    {
                        ObjectDataSource1.SelectParameters["DocEntry"].DefaultValue = DocEntry;
                        ObjectDataSource2.SelectParameters["DocEntry"].DefaultValue = DocEntry;
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    db.Disconnect();
                }
            }
        }

        db.Disconnect();
    }

    protected void GridView1_DataBound(object sender, EventArgs e)
    {
        iniviews(sender, e);
    }

    protected void GridView2_DataBound(object sender, EventArgs e)
    {

    }

    protected void GridView1_PreRender(object sender, EventArgs e)
    {
        LvDispatched = GridView1.Rows[0].Cells[6].Text; // from header
        LvReceived = GridView1.Rows[0].Cells[8].Text; // from header
        LvUserDisp = GridView1.Rows[0].Cells[13].Text; // from header
        LabelMsg.Text = "";

        LabelCurUser.Text = "Usuario: " + (string)this.Session["UserId"];

        if (LvDispatched == "N")
        {
            GridView2.Columns[3].Visible = true;
            GridView2.Columns[4].Visible = false;
            GridView2.Columns[5].Visible = false;
            GridView2.Columns[6].Visible = false;
            GridView2.Columns[7].Visible = true;
            GridView2.Columns[8].Visible = false;

            if (flagPerDeskay != 'Y')
            {
                Button1.Visible = false;
                GridView2.Columns[7].Visible = false;
                //Button2.Enabled = false;
                //btnPrint.Enabled = false;
                LabelMsg.Text = "Mensaje: " + "Este usuario no tiene permisos para despachar.";
                Alert.Show(LabelMsg.Text);
            }
        }
        else
        {
            if (LvReceived == "N")
            {
                GridView2.Columns[3].Visible = false;
                GridView2.Columns[4].Visible = true;
                GridView2.Columns[5].Visible = false;
                GridView2.Columns[6].Visible = false;
                GridView2.Columns[7].Visible = true;
                GridView2.Columns[8].Visible = true;

                if (flagPerReckay != 'Y')
                {
                    Button1.Visible = false;
                    GridView2.Columns[7].Visible = false;
                    Button2.Enabled = false;
                    btnPrint.Enabled = true;
                    LabelMsg.Text = "Mensaje: " + "Este usuario no tiene permisos para recibir.";
                    //Alert.Show(LabelMsg.Text);
                }

                if ((string)this.Session["UserId"] == LvUserDisp)
                {
                    Button1.Enabled = false;
                    Button2.Enabled = false;
                    btnPrint.Enabled = true;
                    LabelMsg.Text = "Mensaje: Despacho realizado exitosamente. EL usuario que realizará el recibo debe ser distinto al que hizo el despacho.";
                    //Alert.Show(LabelMsg.Text);
                }

                string lToWhs = GridView1.Rows[0].Cells[4].Text;

                if (lToWhs == "R2 - RESEARCH STORES")
                {
                    Button1.Enabled = false;
                    Button2.Enabled = false;
                    btnPrint.Enabled = true;
                    LabelMsg.Text = "Mensaje: Despacho realizado exitosamente. No se Permite Recibir en R2.";
                }
            }
            else
            {
                GridView2.Columns[3].Visible = true;
                GridView2.Columns[4].Visible = true;
                GridView2.Columns[5].Visible = true;
                GridView2.Columns[6].Visible = false;
                GridView2.Columns[7].Visible = false;
                GridView2.Columns[8].Visible = true;
                Button1.Enabled = false;
                LabelMsg.Text = "Mensaje: Esta Orden ha sido cerrada y enviada a SAP BO.";
            }
        }
    }


    protected void DisRec()
    {
        string LvParameters = GridView1.Rows[0].Cells[0].Text;
        string disOrRec = null;
        string LvDispatched = GridView1.Rows[0].Cells[6].Text;
        string LvReceived = GridView1.Rows[0].Cells[8].Text;
        string LvUserDisp = GridView1.Rows[0].Cells[13].Text; // from header
        LabelMsg.Text = "";

        LabelCurUser.Text = "Usuario: " + (string)this.Session["UserId"];
        sap_db = (string)Session["CompanyId"];

        string LvuserApp = (string)this.Session["UserId"];

        char LvFlag1 = 'Y';

        //logTrace("TransferDiscreOrdf-"+ GloVarDocEntry, "Tp1 LvDispatched: "+ LvDispatched);
        //logTrace("TransferDiscreOrdf-"+ GloVarDocEntry, "Tp2 LvReceived: " + LvReceived);

        string sloginTypeWhs = null;
        string sTypeWhs = null;

        ////////////////////
        ////Get type og user and warehose Bodega/Tienda
        try
        {
            db.Connect();

            db.cmd.Parameters.Clear();
            db.cmd.CommandText = "SMM_GET_LOGIN_WHS_TYPE_PRC";
            db.cmd.CommandType = CommandType.StoredProcedure;
            db.cmd.Connection = db.Conn;

            db.cmd.Parameters.Add(new SqlParameter("@LoginId", SqlDbType.NVarChar));
            db.cmd.Parameters["@LoginId"].Value = LvuserApp;

            db.cmd.Parameters.Add(new SqlParameter("@DocEntry", SqlDbType.NVarChar));
            db.cmd.Parameters["@DocEntry"].Value = GloVarDocEntry;

            db.cmd.Parameters.Add(new SqlParameter("@CompanyId", SqlDbType.NVarChar));
            db.cmd.Parameters["@CompanyId"].Value = sap_db;


            //db.cmd.ExecuteNonQuery();

            SqlDataAdapter sAdapter = new SqlDataAdapter
            {
                SelectCommand = db.cmd
            };

            DataSet dSet = new DataSet();
            sAdapter.Fill(dSet, "loginTypeWhs");
            DataTable dtable = dSet.Tables["loginTypeWhs"];

            foreach (DataRow errorRow in dtable.Rows)
            {
                sloginTypeWhs = errorRow["loginTypeWhs"].ToString();
                sTypeWhs = errorRow["TypeWhs"].ToString();
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Caught exception in call procedure SMM_GET_LOGIN_WHS_TYPE_PRC. ERROR MESSAGE : " + ex.Message);
        }
        finally
        {
            db.Disconnect();
        }

        if (sloginTypeWhs == "NOSETUP" || sTypeWhs == "NOSETUP")
        {
            Button1.Enabled = false;
            Button2.Enabled = false;
            btnPrint.Enabled = true;
            LvFlag1 = 'N';
            LabelMsg.Text = "Mensaje: Usuario/Destino no esta configurado.";
            Alert.Show(LabelMsg.Text);
        }

        if (sloginTypeWhs != "BODEGA")
        {
            if (sloginTypeWhs != "TIENDA")
            {
                Button1.Enabled = false;
                Button2.Enabled = false;
                btnPrint.Enabled = true;
                LvFlag1 = 'N';
                LabelMsg.Text = "Mensaje: Usuario no esta configurado en Bodega o Tienda.";
                Alert.Show(LabelMsg.Text);
            }
        }

        if (sloginTypeWhs != "TIENDA")
        {
            if (sloginTypeWhs != "BODEGA")
            {
                Button1.Enabled = false;
                Button2.Enabled = false;
                btnPrint.Enabled = true;
                LvFlag1 = 'N';
                LabelMsg.Text = "Mensaje: Usuario no esta configurado en Bodega o Tienda.";
                Alert.Show(LabelMsg.Text);
            }
        }

        ////////////////////

        if (LvDispatched == "Y")
        {
            if (LvReceived == "N")
            {
                if ((string)this.Session["UserId"] == LvUserDisp)
                {
                    Button1.Enabled = false;
                    Button2.Enabled = false;
                    btnPrint.Enabled = true;
                    LvFlag1 = 'N';
                    LabelMsg.Text = "Mensaje: EL usuario que realizará el recibo debe ser distinto al que hizo el despacho.";
                    Alert.Show(LabelMsg.Text);
                }

                string lToWhs = GridView1.Rows[0].Cells[4].Text;

                if (lToWhs == "R2 - RESEARCH STORES")
                {
                    Button1.Enabled = false;
                    Button2.Enabled = false;
                    btnPrint.Enabled = true;
                    LvFlag1 = 'N';
                    LabelMsg.Text = "Mensaje: No se Permite Recibir en R2.";
                    Alert.Show(LabelMsg.Text);
                }

                if (sloginTypeWhs != sTypeWhs)
                {
                    Button1.Enabled = false;
                    Button2.Enabled = false;
                    btnPrint.Enabled = true;
                    LvFlag1 = 'N';
                    LabelMsg.Text = "Mensaje: " + sloginTypeWhs + " NO PUEDE RECIBIR EN " + sTypeWhs + ".";
                    Alert.Show(LabelMsg.Text);
                }
            }
        }
        else
        {
            if (sloginTypeWhs != sTypeWhs)
            {
                Button1.Enabled = false;
                Button2.Enabled = false;
                btnPrint.Enabled = true;
                LvFlag1 = 'N';
                LabelMsg.Text = "Mensaje: " + sloginTypeWhs + " NO PUEDE DESPACHAR EN " + sTypeWhs + ".";
                Alert.Show(LabelMsg.Text);
            }
        }

        if (LvFlag1 == 'Y')
        {
            int TmpQty = 0;
            int LvDQty = 0;
            int LvLinNum = 0;
            TextBox LvTxB1 = new TextBox();
            int rg2count = GridView2.Rows.Count;
            string Lmsg = "Revise cantidades en lineas ";
            int x = 0;

            for (int i = 0; i < rg2count; i++)
            {
                LvLinNum = Convert.ToInt32(GridView2.Rows[i].Cells[0].Text);
                LvTxB1.Text = ((TextBox)(GridView2.Rows[i].Cells[7].Controls[1])).Text;

                //if(!int.TryParse(LvTxB1.Text, out x))
                //{
                //    Lmsg = Lmsg + ' ' + LvLinNum + ',';
                //    LvFlag1 = 'N';
                //}

                try
                {
                    x = int.Parse(LvTxB1.Text);
                }
                catch (Exception)
                {
                    Lmsg = Lmsg + ' ' + LvLinNum + ',';
                    LvFlag1 = 'N';
                }
            }

            if (LvFlag1 == 'N')
            {
                Lmsg = Lmsg + ", Digite solo números enteros.";
                Alert.Show(Lmsg);
                return;
            }

            if (LvDispatched == "N")
            {
                for (int i = 0; i < rg2count; i++)
                {
                    LvLinNum = Convert.ToInt32(GridView2.Rows[i].Cells[0].Text);
                    LvTxB1.Text = ((TextBox)(GridView2.Rows[i].Cells[7].Controls[1])).Text;
                    TmpQty = Convert.ToInt32(LvTxB1.Text);

                    if (LvDispatched == "N")
                    {
                        LvTxB1.Text = GridView2.Rows[i].Cells[3].Text;
                        LvDQty = Convert.ToInt32(LvTxB1.Text);
                    }

                    if (LvDQty < TmpQty || TmpQty < 0)
                    {
                        Lmsg = Lmsg + ' ' + LvLinNum + ',';
                        LvFlag1 = 'N';
                    }
                }
            }

            if (LvFlag1 == 'N')
            {
                Lmsg = Lmsg + ". Estas deben ser menores o iguales a las del Draft o cero";
                Alert.Show(Lmsg);
                return;
            }

            if (LvDispatched == "Y")
            {
                for (int i = 0; i < rg2count; i++)
                {
                    LvLinNum = Convert.ToInt32(GridView2.Rows[i].Cells[0].Text);
                    LvTxB1.Text = ((TextBox)(GridView2.Rows[i].Cells[7].Controls[1])).Text;
                    TmpQty = Convert.ToInt32(LvTxB1.Text);

                    if (TmpQty < 0)
                    {
                        Lmsg = Lmsg + ' ' + LvLinNum + ',';
                        LvFlag1 = 'N';
                    }
                }

                if (LvFlag1 == 'N')
                {
                    Lmsg = Lmsg + ". Estas deben ser mayores o iguales a cero";
                    Alert.Show(Lmsg);
                    return;
                }
            }

            if (LvFlag1 == 'N')
            {
                return;
            }

            db.Connect();

            for (int i = 0; i < rg2count; i++)
            {
                LvLinNum = Convert.ToInt32(GridView2.Rows[i].Cells[0].Text);
                LvTxB1.Text = ((TextBox)(GridView2.Rows[i].Cells[7].Controls[1])).Text;
                TmpQty = Convert.ToInt32(LvTxB1.Text);

                if (LvDispatched == "N")
                {
                    LvTxB1.Text = GridView2.Rows[i].Cells[3].Text;
                    LvDQty = Convert.ToInt32(LvTxB1.Text);
                }
                else
                {
                    if (LvReceived == "N")
                    {
                        LvTxB1.Text = GridView2.Rows[i].Cells[4].Text;
                        LvDQty = Convert.ToInt32(LvTxB1.Text);
                    }
                }

                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.Parameters.Clear();
                sqlCommand.CommandText = "update_discrep_drf1";
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Connection = db.Conn;

                
                sqlCommand.Parameters.Add(new SqlParameter("@CompanyId", SqlDbType.NVarChar));
                sqlCommand.Parameters["@CompanyId"].Value = CompanyLabel.Text;

                sqlCommand.Parameters.Add(new SqlParameter("@DocEntry", SqlDbType.Int));
                sqlCommand.Parameters["@DocEntry"].Value = GloVarDocEntry;

                sqlCommand.Parameters.Add(new SqlParameter("@Linenum", SqlDbType.SmallInt));
                sqlCommand.Parameters["@Linenum"].Value = LvLinNum;

                sqlCommand.Parameters.Add(new SqlParameter("@TmpQty", SqlDbType.SmallInt));
                sqlCommand.Parameters["@TmpQty"].Value = TmpQty;

                try
                {
                    sqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Response.Write("Error when update_discrep_drf1 was called.");
                    Response.Write(ex.Message);
                }
            }

            db.Disconnect();

            if (LvDispatched == "N")
            {
                LvParameters = LvParameters + ' ' + 'D';
                disOrRec = "D";
                LvFlag1 = 'Y';
                GloVarDesRec = 'D';

                //if (ZeroCheckBox.Checked)
                //{


                //}
                //else
                if (!ZeroCheckBox.Checked)
                {

                    db.Connect();

                    db.cmd.Parameters.Clear();
                    db.cmd.CommandText = "Smm_ValDispatching_Order_Prc";
                    db.cmd.CommandType = CommandType.StoredProcedure;
                    db.cmd.Connection = db.Conn;

                    db.cmd.Parameters.Add(new SqlParameter("@CompanyId", SqlDbType.NVarChar));
                    db.cmd.Parameters["@CompanyId"].Value = CompanyLabel.Text;

                    db.cmd.Parameters.Add(new SqlParameter("@DocEntry", SqlDbType.Int));
                    db.cmd.Parameters["@DocEntry"].Value = GloVarDocEntry;

                    SqlParameter textOut = new SqlParameter("@textOut", SqlDbType.VarChar);
                    textOut.Direction = ParameterDirection.Output;
                    textOut.Size = 250;
                    db.cmd.Parameters.Add(textOut);

                    string lTextOut = null;

                    try
                    {
                        db.cmd.ExecuteNonQuery();
                        lTextOut = db.cmd.Parameters["@textOut"].Value.ToString();

                        db.Disconnect();

                        if (lTextOut != "Orden correcta.")
                        {

                            Alert.Show(lTextOut);
                            LvFlag1 = 'N';
                            return;

                        }

                    }

                    catch (Exception ex)
                    {
                        db.Disconnect();
                        Response.Write("Error when Smm_ValDispatching_Order_Prc was called.");
                        Response.Write(ex.Message);
                    }
                }
            }
            else
            {
                if (LvReceived == "N")
                {
                    LvParameters = LvParameters + ' ' + 'R';
                    disOrRec = "R";
                    LvFlag1 = 'Y';
                    GloVarDesRec = 'R';

                    //if (ZeroCheckBox.Checked)
                    //{

                    //}
                    //else
                    if (!ZeroCheckBox.Checked)
                    {

                        db.Connect();

                        db.cmd.Parameters.Clear();
                        db.cmd.CommandText = "Smm_ValReciving_Order_Prc";
                        db.cmd.CommandType = CommandType.StoredProcedure;
                        db.cmd.Connection = db.Conn;

                        db.cmd.Parameters.Add(new SqlParameter("@CompanyId", SqlDbType.NVarChar));
                        db.cmd.Parameters["@CompanyId"].Value = CompanyLabel.Text;

                        db.cmd.Parameters.Add(new SqlParameter("@DocEntry", SqlDbType.Int));
                        db.cmd.Parameters["@DocEntry"].Value = GloVarDocEntry;

                        SqlParameter textOut = new SqlParameter("@textOut", SqlDbType.VarChar);
                        textOut.Direction = ParameterDirection.Output;
                        textOut.Size = 250;
                        db.cmd.Parameters.Add(textOut);

                        string lTextOut = null;

                        try
                        {
                            db.cmd.ExecuteNonQuery();
                            lTextOut = db.cmd.Parameters["@textOut"].Value.ToString();

                            db.Disconnect();

                            if (lTextOut != "Orden correcta.")
                            {
                                Alert.Show(lTextOut);
                                LvFlag1 = 'N';
                                return;
                            }

                        }
                        catch (Exception ex)
                        {
                            db.Disconnect();
                            Response.Write("Error when Smm_ValReciving_Order_Prc was called.");
                            Response.Write(ex.Message);
                        }
                    }

                }
                else
                {
                    LvFlag1 = 'N';
                }
            }

            //logTrace("TransferDiscreOrdf-"+ GloVarDocEntry, "Tp11 LvFlag1 < " + LvFlag1 + ">");



            /////////////////////////////////

            string url;
            string script;
            string message = null;

	//private static readonly HashSet<string> AllowedUsers =
       // new HashSet<string>(StringComparer.OrdinalIgnoreCase)
       // { "YFELICIANO", "AGALVEZ", "MMARTINEZBOD", "AABREGO", "WFERNANDEZ" ,"DGILTIE"};


            if (LvFlag1 == 'Y') // Empieza la parte batch
            {
                //logTrace("TransferDiscreOrdf-" + GloVarDocEntry, " dentro del flag = Y");

                try
                {
                    db.Connect();

                    SqlCommand sqlCommand = new SqlCommand();
                    db.cmd.Parameters.Clear();
                    db.cmd.CommandText = "Smm_populate_whs_transfers_Batch";
                    db.cmd.CommandType = CommandType.StoredProcedure;

                    db.cmd.Parameters.Add(new SqlParameter("@CompanyId", SqlDbType.NVarChar));
                    db.cmd.Parameters["@CompanyId"].Value = CompanyLabel.Text;

                    db.cmd.Parameters.Add(new SqlParameter("@DocEntryDrf", SqlDbType.VarChar));
                    db.cmd.Parameters["@DocEntryDrf"].Value = GloVarDocEntry;

                    db.cmd.Parameters.Add(new SqlParameter("@TypeTran", SqlDbType.VarChar));
                    db.cmd.Parameters["@TypeTran"].Value = disOrRec;

                    db.cmd.Parameters.Add(new SqlParameter("@UserApp", SqlDbType.VarChar));
                    db.cmd.Parameters["@UserApp"].Value = LvuserApp;

                    db.cmd.Connection = db.Conn;
                    db.cmd.CommandTimeout = 0;
                    db.cmd.ExecuteNonQuery();                    
                }
                catch (Exception ex)
                {
                    throw new Exception("Caught exception in procedure la_populate_whs_transfers, ERROR MESSAGE: " + ex.Message);
                }
                finally
                {
                    db.Disconnect();
                }


                GridView1.DataBind();
                GridView2.DataBind();

                LvDispatched = GridView1.Rows[0].Cells[6].Text;

                //logTrace("TransferDiscreOrdf-" + GloVarDocEntry, " Antes de createUserAudit ");
                createUserAudit();

                if (disOrRec == "D")
                {
                    db.Connect();

                    db.cmd.Parameters.Clear();
                    db.cmd.CommandText = "Smm_Get_DispCompleted_Prc";
                    db.cmd.CommandType = CommandType.StoredProcedure;
                    db.cmd.Connection = db.Conn;

                    db.cmd.Parameters.Add(new SqlParameter("@CompanyId", SqlDbType.NVarChar));
                    db.cmd.Parameters["@CompanyId"].Value = CompanyLabel.Text;

                    db.cmd.Parameters.Add(new SqlParameter("@DocEntry", SqlDbType.Int));
                    db.cmd.Parameters["@DocEntry"].Value = Convert.ToInt32(GloVarDocEntry);

                    SqlParameter pDispCompleted = new SqlParameter("@DispCompleted", SqlDbType.VarChar);
                    pDispCompleted.Direction = ParameterDirection.Output;
                    pDispCompleted.Size = 100;
                    db.cmd.Parameters.Add(pDispCompleted);

                    string lDispCompleted = null;

                    try
                    {
                        db.cmd.ExecuteNonQuery();
                        lDispCompleted = db.cmd.Parameters["@DispCompleted"].Value.ToString();

                        if (lDispCompleted == "N")
                        {
                            createUserAudit();
                            LabelMsg.Text = "Orden Despachada. Como hubo discrepancias, se enviarán a bodega de investigación las diferencias.";
                            message = "Orden Despachada. Como hubo discrepancias, se enviarán a bodega de investigación las diferencias.";
                        }
                        else
                        {
                            LabelMsg.Text = "Orden Despachada.";
                            message = "Orden Despachada.";
                        }

                        db.Disconnect();

                    }
                    catch (Exception ex)
                    {
                        db.Disconnect();
                        Response.Write("Error when Smm_Get_DispCompleted_Prc was called.");
                        Response.Write(ex.Message);
                    }
                }

                if (disOrRec == "R")
                {
                    LabelMsg.Text = "La transacción se procesará en el próximo recibo en lote, favor de esperar.";
                    message = "La transacción se procesará en el próximo recibo en lote, favor de esperar.";
                }

                url = string.Format("TransferDiscreOrdf.aspx?Docentry={0}", GloVarDocEntry);
                script = "{ alert('";
                script += message;
                script += "');";
                script += "window.location = '";
                script += url;
                script += "'; }";
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "alert", script, true);
            }
        }
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        DisRec();
    }

    protected void Button2_Click(object sender, EventArgs e)
    {

    }
    protected void btnPrint_Click(object sender, EventArgs e)
    {

    }
    protected void Button2_Click1(object sender, EventArgs e)
    {
        DisRec();
    }

    protected void iniviews(object sender, EventArgs e)
    {
        GridView gridView = (GridView)sender;
        LvParameters = gridView.Rows[0].Cells[0].Text;
        LvDispatched = gridView.Rows[0].Cells[6].Text;
        LvReceived = gridView.Rows[0].Cells[8].Text;

        if (LvDispatched == "N")
        {
            Button1.Text = "Dispatch";
	      //  ZeroCheckBox.Visible = true;
            btnPrint.Visible = false;
            Button2.Visible = false;
            Button1.Height = 38;
            Button1.Width = 65;
        }
        else if (LvReceived == "N")
        {
            Button1.Text = "Receive";
            btnPrint.Visible = true;
            Button2.Height = 28;
            Button2.Width = 66;
            btnPrint.Height = 28;
            btnPrint.Width = 66;
            Button1.Visible = false;
            Button2.Visible = true;

           // var user = LvUserDisp;   // e.g. "YFELICIANO" HACE VISIBLE USUARIO PERMITIDOS
            //ZeroCheckBox.Visible = AllowedUsers.Contains(user);

            if ((string)this.Session["UserId"] == LvUserDisp)
            {
                Button1.Enabled = false;
                Button2.Enabled = false;
                btnPrint.Enabled = true;
                LabelMsg.Text = "Mensaje: Despacho realizado exitosamente. EL usuario que realizará el recibo debe ser distinto al que hizo el despacho.";
                //Alert.Show(LabelMsg.Text);
            }
            else
            {
                string lToWhs = GridView1.Rows[0].Cells[4].Text;

                if (lToWhs == "R2 - RESEARCH STORES")
                {
                    Button1.Enabled = false;
                    Button2.Enabled = false;
                    btnPrint.Enabled = true;
                    LabelMsg.Text = "Mensaje: Despacho realizado exitosamente. No se Permite Recibir en R2.";
                }
            }

        }
        else
        {
            Button1.Visible = false;
            Button2.Visible = false;
            btnPrint.Visible = false;
            GridView2.Columns[4].HeaderText = "Dispatched";
            GridView2.Columns[5].HeaderText = "Received";
            GridView2.Columns[8].Visible = true;
            LabelMsg.Text = "Mensaje: Esta Orden ha sido cerrada y enviada a SAP BO.";
        }
    }

    protected void createUserAudit()
    {

        //logTrace("TransferDiscreOrdf-" + GloVarDocEntry, " Antes de createUserAudit sap_db " + sap_db);
        //logTrace("TransferDiscreOrdf-" + GloVarDocEntry, " Antes de createUserAudit DocEntry " + GloVarDocEntry);
        //logTrace("TransferDiscreOrdf-" + GloVarDocEntry, " Antes de createUserAudit GloVarDesRec " + GloVarDesRec);

        db.Connect();

        try
        {
            db.cmd.Parameters.Clear();
            db.cmd.CommandText = "smm_insert_Transdiscrep_audit_odrf";
            db.cmd.CommandType = CommandType.StoredProcedure;

            db.cmd.Parameters.Add(new SqlParameter("@CompanyId", SqlDbType.NVarChar));
            db.cmd.Parameters["@CompanyId"].Value = CompanyLabel.Text;

            db.cmd.Parameters.Add(new SqlParameter("@DocEntry", SqlDbType.Int));
            db.cmd.Parameters["@DocEntry"].Value = Convert.ToInt32(GloVarDocEntry);

            db.cmd.Parameters.Add(new SqlParameter("@TypeTrans", SqlDbType.NVarChar));
            db.cmd.Parameters["@TypeTrans"].Value = GloVarDesRec;

            db.cmd.Parameters.Add(new SqlParameter("@SourceTrans", SqlDbType.NVarChar));
            db.cmd.Parameters["@SourceTrans"].Value = "SISINV";

            db.cmd.Connection = db.Conn;
            db.cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            throw new Exception("Caught exception in call procedure createUserAudit. ERROR MESSAGE : " + ex.Message);
        }
        finally
        {
            db.Disconnect();
        }
    }



    protected void ReopenRecButton_Click(object sender, EventArgs e)
    {

    }

    protected void logTrace(string sObjectName, string sLogMessage)
    {
        db.Connect();

        try
        {

            db.cmd.Parameters.Clear();
            db.cmd.CommandText = "SMM_LOGTRACE_PRC";
            db.cmd.CommandType = CommandType.StoredProcedure;

            db.cmd.Parameters.Add(new SqlParameter("@ObjectName", SqlDbType.NVarChar));
            db.cmd.Parameters["@ObjectName"].Value = sObjectName;

            db.cmd.Parameters.Add(new SqlParameter("@LogMessage", SqlDbType.NVarChar));
            db.cmd.Parameters["@LogMessage"].Value = sLogMessage;


            db.cmd.Connection = db.Conn;


            db.cmd.ExecuteNonQuery();

        }
        catch (Exception ex)
        {
            db.Disconnect();
            throw new Exception("Caught exception in call procedure logTrace. ERROR MESSAGE : " + ex.Message);
        }

        db.Disconnect();

    }
}
