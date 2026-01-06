using System;
using System.Data;
using System.Data.OleDb;
using System.Configuration;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Drawing;


public partial class createTransferByExcel : System.Web.UI.Page
{
    protected SqlDb db = new SqlDb();
    DataTable dtToWhs;
    protected string usr;
    protected string Loc;
    protected string Item;
    protected string sap_db;
    protected string serverIP;
    protected string serverUserName;
    protected string serverPwd;
    protected string dbUserName;
    protected string dbPwd;
    protected string licenseServerIP;
    protected string xmlPath;
    protected string lfullFilePath = "";
    protected string lFileExt = "";
    protected string appUserName;
    protected string lCurUser;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            ValidaSesionUsuarioCia();

            sap_db = (string)Session["CompanyId"];

            appUserName = (string)Session["UserId"];
            char flagokay = 'N';
            Label2.Visible = false;
            Label3.Visible = false;

            ///////////////Begin New  Control de acceso por Roles
            lCurUser = (string)Session["UserId"];
            flagokay = 'Y';

            string lControlName = "createTransferByExcel.aspx";
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
                btnCreateDraft.Enabled = false;
                btnCreateDraft.ForeColor = Color.Silver;
                labelForm.InnerText = "Crear Transferencia por Excel (Acceso solo Lectura)";
            }

            if (strAccessType == "F")
            {
                btnCreateDraft.Enabled = true;
                labelForm.InnerText = "Crear Transferencia por Excel (Acceso Completo)";
            }
            ///////////////End  New Control de acceso por Roles

            if (flagokay == 'Y')
            {
                SqlDataSource1.SelectParameters["DocEntry"].DefaultValue = "13";

                serverIP = ConfigurationManager.AppSettings.Get("serverIP");
                serverUserName = ConfigurationManager.AppSettings.Get("serverUserName");
                serverPwd = ConfigurationManager.AppSettings.Get("serverPwd");
                dbUserName = ConfigurationManager.AppSettings.Get("dbUserName");
                dbPwd = ConfigurationManager.AppSettings.Get("dbPwd");
                licenseServerIP = ConfigurationManager.AppSettings.Get("licenseServerIP");

                //usr = User.Identity.Name.ToLower().Replace("lgihome\\", "");
                string x = User.Identity.Name.ToLower();
                usr = x.Substring(x.IndexOf("\\") + 1, x.Length - x.IndexOf("\\") - 1);

                divMessage.InnerHtml = "";

                if (!IsPostBack)
                {
                    db.Connect();
                    LoadWarehouses();
                    InitializeForm();
                    db.Disconnect();

                    btnCreateTransfer.Visible = false;
                    btnUdateDraft.Visible = false;
                    btnCancel.Visible = false;
                }
            }
        }
        catch (Exception)
        {
            db.Disconnect();
            throw;
        }
    }

    private void GoToLogin()
    {
        Response.Redirect("Login1.aspx");
    }

    private void ValidaSesionUsuarioCia()
    {
        if (Session["UserId"] == null || (string)Session["UserId"] == "" || Session["CompanyId"] == null || (string)Session["CompanyId"] == "")
        {
            GoToLogin();
        }
    }

    private void ValidaSesionBodegas()
    {
        if(Session["toWhs"] == null || (DataTable)Session["toWhs"] == null)
        {
            GoToLogin();
        }
    }

    private void InitializeForm()
    {
        divMessage.InnerHtml = "";
    }

    private void LoadWarehouses()
    {
        DataTable dt = new DataTable();

        try
        {
            string sql =
            @"select O.WhsCode, O.WhsName, R.Control 
	          from " + sap_db + @".dbo.owhs O " + Queries.WITH_NOLOCK + @" , RSS_OWHS_CONTROL R " + Queries.WITH_NOLOCK + @" 
	          where O.WhsCode  = R.WhsCode
	            and R.Control IN ('CRETRAFROMXSAP', 'CRETRATOEXCEL')
	            AND R.CompanyId = '" + sap_db + @"' 
              order by 1";

            db.adapter = new SqlDataAdapter(sql, db.Conn);
            db.adapter.Fill(dt);
        }
        catch (Exception ex)
        {
            throw new Exception("Caught exception in function LoadWarehouses. ERROR MESSAGE : " + ex.Message);
        }

        DataTable dt1 = dt.AsEnumerable()
            .Where(x => x.Field<string>("Control") == "CRETRAFROMXSAP")
            .CopyToDataTable();

        drpFromWhsCode.DataSource = dt1;
        drpFromWhsCode.DataBind();

        DataTable dt2 = dt.AsEnumerable()
            .Where(x => x.Field<string>("Control") == "CRETRATOEXCEL")
            .CopyToDataTable();

        drpToWhsCode.DataSource = dt2;
        drpToWhsCode.DataBind();

        Session["toWhs"] = dt2;

        ListItem li = new ListItem("Select a location", "0");

        drpFromWhsCode.Items.Insert(0, li);
        drpToWhsCode.Items.Insert(0, li);
    }

    protected void btnCreateTransfer_Click(object sender, EventArgs e)
    {
        int execRes = 0;
        SqlCommand sqlCommand2;

        if (string.IsNullOrEmpty((string)Session["UserId"]) || string.IsNullOrEmpty((string)Session["CompanyId"]))
        {
            Response.Redirect("Login1.aspx");
        }

        sap_db = (string)Session["CompanyId"];

        string lFlag = null;

        lFlag = Validate1();

        if (lFlag == "N")
        {
            return;
        }

        UpdateQties();

        db.Connect();

        sqlCommand2 = new SqlCommand
        {
            Connection = db.Conn,
            CommandType = CommandType.StoredProcedure,
            CommandText = "submit_DraftXsap",
            CommandTimeout = 240,
            Parameters =
            {
                new SqlParameter
                {
                    ParameterName = "@CompanyId",
                    SqlDbType = SqlDbType.VarChar,
                    Value = sap_db
                },
                new SqlParameter
                {
                    ParameterName = "@DocEntry",
                    SqlDbType = SqlDbType.Int,
                    Value = DocEntry.Text
                },
                new SqlParameter
                {
                    ParameterName = "@userDraft",
                    SqlDbType = SqlDbType.Char,
                    Value = appUserName
                },
            }
        };

        try
        {
            execRes = sqlCommand2.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Response.Write("Error when submit_DraftXsap.");
            Response.Write(ex.Message);
        }
        finally
        {
            db.Disconnect();
        }

        LabDocEntry.Text = " Draft " + DocEntry.Text + " Realizado con Exito.";
        btnCreateDraft.Visible = false;
        btnCreateTransfer.Visible = false;
        btnUdateDraft.Visible = false;
        btnCancel.Text = "Create New Draft";
        btnCancel.Visible = true;
        Label2.Visible = false;
    }

    protected void btnCreateDraft_Click(object sender, EventArgs e)
    {
        ValidaSesionUsuarioCia();

        int execRes = 0;
        SqlCommand sqlCommand;
        SqlCommand admCmd;
        SqlDataReader admDr;

        sap_db = (string)Session["CompanyId"];

        string fromLoc = drpFromWhsCode.SelectedValue;
        string toLoc = drpToWhsCode.SelectedValue;

        int lDocEntry = 0;


        if (fromLoc == "0")
        {
            divMessage.InnerHtml = "Seleccione el Origen";
            Alert.Show("Seleccione el Origen");
            drpFromWhsCode.Focus();
            return;
        }
        //else fromLoc = "'" + fromLoc + "'";


        if (toLoc == "0")
        {
            divMessage.InnerHtml = "Seleccione el Destino'";
            Alert.Show("Seleccione el Destino");
            drpToWhsCode.Focus();
            return;
        }
        //else toLoc = "'" + toLoc + "'";

        if (fromLoc == toLoc)
        {
            divMessage.InnerHtml = "El origen y el Destino deben ser diferentes";
            Alert.Show("El origen y el Destino deben ser diferentes");
            drpToWhsCode.Focus();
            return;
        }

        if(db.DbConnectionState == ConnectionState.Closed)
        {
            db.Connect();
        }

        try
        {
            admCmd = new SqlCommand()
            {
                Connection = db.Conn,
                CommandType = CommandType.Text,
                CommandText = "get_TransXsap_sequence",
                CommandTimeout = 240
            };

            admDr = admCmd.ExecuteReader();

            while (admDr.Read())
            {
                lDocEntry = Convert.ToInt32((admDr.GetString(0)));
            }
            admDr.Close();
        }

        catch (Exception ex)
        {
            Response.Write("Error when get_TransXsap_sequence: ");
            Response.Write(ex.Message);
        }
        finally
        {
            db.Disconnect();
        }

        DocEntry.Text = lDocEntry.ToString();
        LabDocEntry.Text = "Draft Number: " + lDocEntry.ToString();

        UploadExcelFile();

        if(db.DbConnectionState == ConnectionState.Closed)
        {
            db.Connect();
        }
        
        try
        {
            sqlCommand = new SqlCommand
            {
                Connection = db.Conn,
                CommandType = CommandType.StoredProcedure,
                CommandText = "smm_populate_TransXexcel_odrf",
                CommandTimeout = 240,
                Parameters =
                {
                    new SqlParameter
                    {
                        ParameterName = "@CompanyId",
                        SqlDbType = SqlDbType.NVarChar,
                        Value = sap_db
                    },
                    new SqlParameter
                    {
                        ParameterName = "@fromLoc",
                        SqlDbType = SqlDbType.VarChar,
                        Value = fromLoc
                    },
                    new SqlParameter
                    {
                        ParameterName = "@toLoc",
                        SqlDbType = SqlDbType.VarChar,
                        Value = toLoc
                    },
                    new SqlParameter
                    {
                        ParameterName = "@DocEntry",
                        SqlDbType = SqlDbType.BigInt,
                        Value = lDocEntry
                    },
                    new SqlParameter
                    {
                        ParameterName = "@userDraft",
                        SqlDbType = SqlDbType.NVarChar,
                        Value = appUserName
                    },
                }
            };

            execRes = sqlCommand.ExecuteNonQuery();
        }

        catch (Exception ex)
        {
            Response.Write("Error when btnCreateDraft_Click was called.");
            Response.Write(ex.Message);
        }
        finally
        {
            db.Disconnect();
        }

        GridView1.DataBind();
        GridView2.DataBind();

        int rgcount = GridView1.Rows.Count;

        Label2.Visible = true;

        if (rgcount > 0)
        {
            btnCreateDraft.Visible = false;
            btnCreateTransfer.Visible = true;
            btnUdateDraft.Visible = true;
            btnCancel.Visible = true;
            Label3.Visible = true;
            LabDocEntry.Visible = true;
            Label3.Text = "Productos en SAP con OnHand";
        }
        else
        {
            btnCreateDraft.Visible = true;
            btnCreateTransfer.Visible = false;
            btnUdateDraft.Visible = false;
            btnCancel.Visible = false;
            Label3.Visible = true;
            LabDocEntry.Visible = false;
            Label3.Text = "No se encontraron productos en SAP con OnHand";
            Alert.Show("No se Encontraron productos en SAP con OnHand");
        }
    }

    protected void btnUdateDraft_Click(object sender, EventArgs e)
    {
        ValidaSesionUsuarioCia();

        sap_db = (string)Session["CompanyId"];

        string lFlag = null;

        lFlag = Validate1();

        if (lFlag == "N")
        {
            return;
        }
        
        UpdateQties();
    }

    protected void UpdateQties()
    {
        int execRes = 0;
        float TmpQty = 0;
        int LvLinNum = 0;
        string qtystr = null;
        string strTmpQty = null;
        int charpos = 0;

        SqlCommand sqlCommand, sqlCommand2, sqlCommand3;

        int rg2count = GridView1.Rows.Count;

        for (int i = 0; i < rg2count; i++)
        {
            LvLinNum = Convert.ToInt32(GridView1.Rows[i].Cells[0].Text);
            strTmpQty = ((TextBox)(GridView1.Rows[i].Cells[5].Controls[1])).Text;
            //HiddenField hdn = GridView1.Rows[i].FindControl("hdnDraftQuantity");
            //string hdnValue = hdn.Value;

            qtystr = strTmpQty;

            charpos = qtystr.IndexOf(".");

            if (charpos != -1)
            {
                strTmpQty = qtystr.Substring(0, charpos);
            }

            TmpQty = Convert.ToInt32(strTmpQty);

            if (TmpQty != 0)
            {
                if(db.DbConnectionState == ConnectionState.Closed)
                {
                    db.Connect();
                }

                try
                {
                    sqlCommand = new SqlCommand
                    {
                        Connection = db.Conn,
                        CommandType = CommandType.StoredProcedure,
                        CommandText = "update_TransXsap_drf1",
                        CommandTimeout = 240,
                        Parameters =
                        {
                            new SqlParameter
                            {
                                ParameterName = "@DocEntry",
                                SqlDbType = SqlDbType.Int,
                                Value = DocEntry.Text
                            },
                            new SqlParameter
                            {
                                ParameterName = "@Linenum",
                                SqlDbType = SqlDbType.Int,
                                Value = LvLinNum
                            },
                            new SqlParameter
                            {
                                ParameterName = "@TmpQty",
                                SqlDbType = SqlDbType.Int,
                                Value = TmpQty
                            },
                            new SqlParameter
                            {
                                ParameterName = "@CompanyId",
                                SqlDbType = SqlDbType.NVarChar,
                                Value = sap_db
                            }
                        }
                    };

                    execRes = sqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    db.Disconnect();
                    Response.Write("Error when update_TransXsap_drf1.");
                    Response.Write(ex.Message);
                }
            }
        }

        if (db.DbConnectionState == ConnectionState.Closed)
        {
            db.Connect();
        }

        try
        {
            sqlCommand2 = new SqlCommand
            {
                Connection = db.Conn,
                CommandType = CommandType.StoredProcedure,
                CommandText = "delete_TransXsap_drf1",
                CommandTimeout = 240,
                Parameters =
                {
                    new SqlParameter
                    {
                        ParameterName = "@DocEntry",
                        SqlDbType = SqlDbType.Int,
                        Value = DocEntry.Text
                    },
                    new SqlParameter
                    {
                        ParameterName = "@CompanyId",
                        SqlDbType = SqlDbType.NVarChar,
                        Value = sap_db
                    }
                }
            };

            execRes = sqlCommand2.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            db.Disconnect();
            Response.Write("Error when delete_TransXsap_drf1.");
            Response.Write(ex.Message);
        }

        if (db.DbConnectionState == ConnectionState.Closed)
        {
            db.Connect();
        }

        try
        {
            sqlCommand3 = new SqlCommand
            {
                Connection = db.Conn,
                CommandType = CommandType.StoredProcedure,
                CommandText = "reseq_TransXsap_drf1",
                CommandTimeout = 240,
                Parameters =
                {
                    new SqlParameter
                    {
                        ParameterName = "@DocEntry",
                        SqlDbType = SqlDbType.Int,
                        Value = DocEntry.Text
                    },
                    new SqlParameter
                    {
                        ParameterName = "@CompanyId",
                        SqlDbType = SqlDbType.NVarChar,
                        Value = sap_db
                    }
                }
            };

            execRes = sqlCommand3.ExecuteNonQuery();

            GridView1.DataBind();

            Label2.Visible = false;
        }
        catch (Exception ex)
        {
            Response.Write("Error when reseq_TransXsap_drf1.");
            Response.Write(ex.Message);
        }
        finally
        {
            db.Disconnect();
        }
    }

    protected string Validate1()
    {
        //string ErrEx = null;
        string strTmpQty = null;
        int TmpQty = 0;
        int LvLinNum = 0;
        int acumQty = 0;
        int OnHand = 0;
        char LvFlag1;
        char LvFlagIntNumbers = 'Y';
        char LvFlagMoreOrEqualsToZero = 'Y';
        char LvFlagLessOrEqualsToOnHand = 'Y';

        int rg2count = GridView1.Rows.Count;

        string Lmsg = "Revise cantidades en lineas ";

        for (int i = 0; i < rg2count; i++)
        {
            LvFlag1 = 'Y';

            LvLinNum = Convert.ToInt32(GridView1.Rows[i].Cells[0].Text);
            OnHand = Convert.ToInt32(GridView1.Rows[i].Cells[3].Text);
            strTmpQty = ((TextBox)GridView1.Rows[i].Cells[5].Controls[1]).Text;

            if (strTmpQty == "0.000000")
            {
                strTmpQty = "0";
            }

            if (!int.TryParse(strTmpQty, out TmpQty))
            {
                Lmsg = Lmsg + " " + LvLinNum + ",";
                LvFlag1 = 'N';
                LvFlagIntNumbers = 'N';
                //ErrEx = "Invalid number: " + strTmpQty;
            }

            if (LvFlag1 == 'Y')
            {
                if (TmpQty < 0)
                {
                    Lmsg = Lmsg + " " + LvLinNum + ",";
                    LvFlag1 = 'N';
                    LvFlagMoreOrEqualsToZero = 'N';
                }

                if (TmpQty > OnHand)
                {
                    Lmsg = Lmsg + ' ' + LvLinNum + ',';
                    LvFlag1 = 'N';
                    LvFlagLessOrEqualsToOnHand = 'N';
                }

                if (LvFlag1 == 'Y')
                {
                    acumQty = acumQty + TmpQty;
                }
            }
        }

        if (LvFlagIntNumbers == 'N')
        {
            Lmsg = Lmsg + ". Digite solo números enteros. ";
            Alert.Show(Lmsg);
            return ("N");
        }

        if (LvFlagMoreOrEqualsToZero == 'N')
        {
            Lmsg = Lmsg + ". Estas deben ser mayores o iguales a cero. ";
            Alert.Show(Lmsg);
            return ("N");
        }

        if (LvFlagLessOrEqualsToOnHand == 'N')
        {
            Lmsg = Lmsg + ". Estas deben ser menores o iguales al OnHand. ";
            Alert.Show(Lmsg);
            return ("N");
        }

        if (acumQty == 0)
        {
            Lmsg = "Ponga cantidad mayor a cero, al menos a un artículo.";
            Alert.Show(Lmsg);
            LvFlag1 = 'N';
            return ("N");
        }

        btnUdateDraft.Visible = true;

        return ("Y");
    }

    //protected string Validate1()
    //{

    //    int TmpQty = 0;
    //    int LvLinNum = 0;
    //    char LvFlag1 = 'Y';
    //    string ErrEx = null;

    //    TextBox LvTxB1 = new TextBox();

    //    int rg2count = GridView1.Rows.Count;

    //    string Lmsg = "Revise cantidades en lineas ";

    //    for (int i = 0; i < rg2count; i++)
    //    {
    //        LvLinNum = Convert.ToInt32(GridView1.Rows[i].Cells[0].Text);
    //        LvTxB1.Text = ((TextBox)(GridView1.Rows[i].Cells[5].Controls[1])).Text;

    //        if (LvTxB1.Text == "0.000000")
    //        {
    //            LvTxB1.Text = "0";
    //        }

    //        try
    //        {
    //            int x = int.Parse(LvTxB1.Text);
    //        }
    //        catch (Exception ex)
    //        {
    //            Lmsg = Lmsg + ' ' + LvLinNum + ',';
    //            LvFlag1 = 'N';
    //            ErrEx = ex.Message;
    //        }
    //    }

    //    if (LvFlag1 == 'N')
    //    {
    //        Lmsg = Lmsg + ", Digite solo numeros enteros. ";
    //        Alert.Show(Lmsg);
    //        return ("N");
    //    }

    //    {

    //        int acumQty = 0;
    //        for (int i = 0; i < rg2count; i++)
    //        {
    //            LvLinNum = Convert.ToInt32(GridView1.Rows[i].Cells[0].Text);
    //            LvTxB1.Text = ((TextBox)(GridView1.Rows[i].Cells[5].Controls[1])).Text;


    //            if (LvTxB1.Text == "0.000000")
    //            {
    //                LvTxB1.Text = "0";
    //            }

    //            TmpQty = Convert.ToInt32(LvTxB1.Text);
    //            acumQty = acumQty + TmpQty;

    //            if (TmpQty < 0)
    //            {
    //                Lmsg = Lmsg + ' ' + LvLinNum + ',';
    //                LvFlag1 = 'N';
    //            }
    //        }

    //        if (LvFlag1 == 'N')
    //        {
    //            Lmsg = Lmsg + ". Estas deben ser mayores o iguales a cero";
    //            Alert.Show(Lmsg);
    //            return ("N");
    //        }

    //        if (acumQty == 0)
    //        {
    //            Lmsg = "Ponga cantidad mayor a cero a al menos un artÃ­culo.";
    //            Alert.Show(Lmsg);
    //            LvFlag1 = 'N';
    //            return ("N");
    //        }
    //    }

    //    int OnHand = 0;

    //    for (int i = 0; i < rg2count; i++)
    //    {
    //        LvLinNum = Convert.ToInt32(GridView1.Rows[i].Cells[0].Text);
    //        LvTxB1.Text = ((TextBox)(GridView1.Rows[i].Cells[5].Controls[1])).Text;

    //        OnHand = Convert.ToInt32(GridView1.Rows[i].Cells[3].Text);

    //        TmpQty = Convert.ToInt32(LvTxB1.Text);

    //        if (TmpQty > OnHand)
    //        {
    //            Lmsg = Lmsg + ' ' + LvLinNum + ',';
    //            LvFlag1 = 'N';
    //        }
    //    }

    //    if (LvFlag1 == 'N')
    //    {
    //        Lmsg = Lmsg + ". Estas deben ser menores o iguales al ohHand";
    //        Alert.Show(Lmsg);
    //        return ("N");
    //    }

    //    btnUdateDraft.Visible = true;

    //    return ("Y");
    //}

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        ValidaSesionUsuarioCia();

        int execRes = 0;

        sap_db = (string)Session["CompanyId"];

        if (btnCancel.Text == "Borrar Draft")
        {
            db.Connect();
            SqlCommand sqlCommand1;

            try
            {
                sqlCommand1 = new SqlCommand
                {
                    Connection = db.Conn,
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "delete_TransXsap",
                    CommandTimeout = 240,
                    Parameters =
                    {
                        new SqlParameter
                        {
                            ParameterName = "@DocEntry",
                            SqlDbType = SqlDbType.Int,
                            Value = DocEntry.Text
                        },
                        new SqlParameter
                        {
                            ParameterName = "@CompanyId",
                            SqlDbType = SqlDbType.NVarChar,
                            Value = sap_db
                        }
                    }
                };

                execRes = sqlCommand1.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Response.Write("Error when delete_TransXsap");
                Response.Write(ex.Message);
            }
            finally
            {
                db.Disconnect();
            }

            string url = HttpContext.Current.Request.Url.AbsoluteUri;

            Response.AppendHeader("Refresh", "0; URL=" + url);
        }

        if (btnCancel.Text == "Create New Draft")
        {
            string url = HttpContext.Current.Request.Url.AbsoluteUri;
            Response.AppendHeader("Refresh", "0; URL=" + url);
        }

        Label2.Visible = false;
    }

    protected void UploadExcelFile()
    {
        try
        {
            if (FileUpload1.HasFile)
            {
                if (!SaveFile())
                {
                    divMessage.InnerHtml = "<br>*El Archivo no fue cargado.";
                    return;
                }
                else
                {
                    string lFile = lfullFilePath;

                    lFileExt = System.IO.Path.GetExtension(FileUpload1.FileName).ToLower();

                    string connStrXls = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source= " + lFile + ";Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1;\"";

                    string connStrXlsx = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + lFile + ";Extended Properties='Excel 8.0;HDR={1};IMEX=1'";

                    if (lFileExt == ".xls")
                    {
                        OleDbConnection oledbConn = new OleDbConnection(connStrXls);
                        importData(oledbConn);
                    }
                    else
                    {
                        if (lFileExt == ".xlsx")
                        {
                            OleDbConnection oledbConn = new OleDbConnection(connStrXlsx);
                            importData(oledbConn);
                        }
                        else
                        {
                            divMessage.InnerHtml = "<br>*El Archivo no fue cargado debido a que no es tipo Excel.";
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            divMessage.InnerHtml = ex.Message.ToString();
            return;
        }
    }


    protected void Button1_Click(object sender, EventArgs e)
    {

    }

    protected void importData(OleDbConnection loledbConn)
    {
        try
        {
            loledbConn.Open();
            // Create OleDbCommand object and select data from worksheet Sheet1
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM [Import$]", loledbConn);

            // Create new OleDbDataAdapter
            OleDbDataAdapter oleda = new OleDbDataAdapter();

            oleda.SelectCommand = cmd;

            // Create a DataSet which will hold the data extracted from the worksheet.
            DataSet ds = new DataSet();

            // Fill the DataSet from the data extracted from the worksheet.
            oleda.Fill(ds, "myExcelData");
            DataTable dt = new DataTable();

            dt = ds.Tables[0];

            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                if (dt.Rows[i][0] == DBNull.Value && dt.Rows[i][1] == DBNull.Value && dt.Rows[i][2] == DBNull.Value)
                {
                    dt.Rows[i].Delete();
                }
            }

            dt.AcceptChanges();

            // Bind the data to the GridView
            GridView2.DataSource = dt;
            GridView2.DataBind();
        }
        catch (Exception theException)
        {
            divMessage.InnerHtml = divMessage.InnerHtml + theException.Message;
        }
        finally
        {
            // Close connection
            loledbConn.Close();
        }

        UploadDataToDataBase();
    }

    protected void UploadDataToDataBase()
    {
        int execRes = 0;
        int LvLinNum = 0;

        int rg2count = GridView2.Rows.Count;

        string lItem = "";
        string lItemDesc = "";
        int lQty = 0;
        SqlCommand sqlCommand;

        for (int i = 0; i < rg2count; i++)
        {
            LvLinNum++;
            lItem = Convert.ToString(GridView2.Rows[i].Cells[0].Text);

            if (lItem == "&nbsp;")
            {
                continue;
            }

            lItemDesc = Convert.ToString(GridView2.Rows[i].Cells[1].Text);
            lQty = Convert.ToInt32(GridView2.Rows[i].Cells[2].Text);

            if(db.DbConnectionState == ConnectionState.Closed)
            {
                db.Connect();
            }
            
            try
            {
                sqlCommand = new SqlCommand
                {
                    Connection = db.Conn,
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "smm_insTransferXexcel_stg",
                    CommandTimeout = 240,
                    Parameters =
                    {
                        new SqlParameter
                        {
                            ParameterName = "@DocEntry",
                            SqlDbType = SqlDbType.Int,
                            Value = DocEntry.Text
                        },
                        new SqlParameter
                        {
                            ParameterName = "@Linenum",
                            SqlDbType = SqlDbType.Int,
                            Value = LvLinNum
                        },
                        new SqlParameter
                        {
                            ParameterName = "@FromWhsCode",
                            SqlDbType = SqlDbType.VarChar,
                            Value = drpFromWhsCode.Text
                        },
                        new SqlParameter
                        {
                            ParameterName = "@ToWhsCode",
                            SqlDbType = SqlDbType.VarChar,
                            Value = drpToWhsCode.Text
                        },
                        new SqlParameter
                        {
                            ParameterName = "@ItemCode",
                            SqlDbType = SqlDbType.VarChar,
                            Value = lItem
                        },
                        new SqlParameter
                        {
                            ParameterName = "@ItemNameXls",
                            SqlDbType = SqlDbType.VarChar,
                            Value = lItemDesc
                        },
                        new SqlParameter
                        {
                            ParameterName = "@userSap",
                            SqlDbType = SqlDbType.VarChar,
                            Value = appUserName
                        },
                        new SqlParameter
                        {
                            ParameterName = "@TmpQty",
                            SqlDbType = SqlDbType.Int,
                            Value = lQty
                        }
                    }
                };

                execRes = sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                db.Disconnect();
                Response.Write("Error when smm_insTransferXexcel_stg.");
                Response.Write(ex.Message);
            }
        }

        if(db.DbConnectionState == ConnectionState.Open)
        {
            db.Disconnect();
        }
    }

    protected bool SaveFile()
    {
        string fileName = FileUpload1.FileName;
        string filePath = Server.MapPath("temp") + "\\" + fileName;
        string ext = fileName.Substring(FileUpload1.FileName.LastIndexOf("."), 4).ToLower();


        if (FileUpload1.HasFile)
        {
            //check for correct file type
            if (ext == ".xls" || ext == ".xlsx")
            {
                // save the file for processing
                FileUpload1.SaveAs(filePath);
                lfullFilePath = filePath;
                lFileExt = ext;
                return true; // InsertUserItems(filePath);
            }
            else
            {
                divMessage.InnerText += "*El archivo debe ser tipo Excel .xls or .xlsx .";
                return false;
            }
        }
        else
        {
            divMessage.InnerHtml += "*El Archivo no fue cargado";
            return false;
        }
    }

    protected void drpFromWhsCode_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            ValidaSesionUsuarioCia();
            ValidaSesionBodegas();

            dtToWhs = (DataTable)Session["toWhs"];

            if (dtToWhs.Rows.Count <= 0)
            {
                Alert.Show("No hay destinos disponibles");
            }
            else
            {
                DataTable dt2 = dtToWhs.AsEnumerable()
                    .Where(x => x.Field<string>("WhsCode") != drpFromWhsCode.SelectedValue)
                    .CopyToDataTable();

                drpToWhsCode.DataSource = dt2;
                drpToWhsCode.DataBind();
            }

            ListItem li = new ListItem("Select a location", "0");
            drpToWhsCode.Items.Insert(0, li);
        }
        catch (Exception ex)
        {
            throw new Exception("Caught exception in function LoadWarehouses for toloc. ERROR MESSAGE : " + ex.Message);
        }
    }
}
