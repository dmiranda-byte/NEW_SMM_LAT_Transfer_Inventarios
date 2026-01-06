using System;
using System.Data;
using System.Collections;
using System.Data.SqlClient;

public partial class Login1 : System.Web.UI.Page
{
    protected SqlDb db = new SqlDb();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //string v_UserID = this.Session["UserId"].ToString();
            if ((string)this.Session["UserId"] == "" || (string)this.Session["UserId"] == null)
            {
                string Lmsg = "Favor de registrarse en el sistema.";
                //Alert.Show(Lmsg);
                divMessage.InnerHtml = Lmsg;

                LogInBtn1.Visible = true;
                LogOutBtn1.Visible = false;
                Label1.Visible = true;
                lblCompany.Visible = true;
                UserField1.Visible = true;
                Label2.Visible = true;
                Passwd1.Visible = true;
                companyDDL.Visible = true;
                UserField1.Focus();
            }
            else
            {
                //LogInBtn1.Visible = false;
                //LogOutBtn1.Visible = true;
                //Label1.Visible = false;
                //lblCompany.Visible = false;
                //UserField1.Visible = false;
                //Label2.Visible = false;
                //Passwd1.Visible = false;
                //companyDDL.Visible = false;

                Session["FlagNoPerPag"] = "Y";
                Session["UserId"] = "";
                Response.Redirect("Login1.aspx");
                LogInBtn1.Enabled = true;
                LogOutBtn1.Enabled = false;
                LogOutBtn1.Visible = false;

                Session["UserId"] = "";
                Session["CompanyId"] = "";
                Label3.Text = "Registre su Usuario.";
            }
        }
    }

    protected void LogOutBtn1_Click(object sender, EventArgs e)
    {
        Session["FlagNoPerPag"] = "Y";
        Session["UserId"] = "";
        Response.Redirect("Login1.aspx");
        LogInBtn1.Enabled = true;
        LogOutBtn1.Enabled = false;
        LogOutBtn1.Visible = false;

        Session["UserId"] = "";
        Session["CompanyId"] = "";
        Label3.Text = "Registre su Usuario.";        
    }
    protected void LogInBtn1_Click(object sender, EventArgs e)
    {
        Session["FlagNoPerPag"] = "Y";

        if (companyDDL.SelectedValue == "Selecciona")
        {
            Alert.Show("Seleccione la Compañia");
            //divMessage.InnerText = "Seleccione la Compañia";
            return;
        }

        if (Passwd1.Text == "")
        {
            //Alert.Show("Digite su Clave");
            divMessage.InnerText = "Digite su Clave";
        }
        else
        {

            if (UserField1.Text == "")
            {
                //Alert.Show("Digite su Usuario");
                divMessage.InnerText = "Digite su Usuario";
            }
            else
            {
                Session["CompanyId"] = companyDDL.SelectedValue;
                Session["CompanyName"] = companyDDL.SelectedItem.Text;

                Label3.Text = "Compañia: " + (string)Session["CompanyId"];
                db.Connect();
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.Parameters.Clear();
                sqlCommand.CommandText = "smm_login_validations";
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.Add(new SqlParameter("@LoginId", SqlDbType.NVarChar));
                sqlCommand.Parameters["@LoginId"].Value = UserField1.Text;

                sqlCommand.Parameters.Add(new SqlParameter("@Passwd", SqlDbType.NVarChar));
                sqlCommand.Parameters["@Passwd"].Value = Passwd1.Text;
                
                sqlCommand.Parameters.Add(new SqlParameter("@CompanyId", SqlDbType.NVarChar));
                sqlCommand.Parameters["@CompanyId"].Value = (string)Session["CompanyId"];

                SqlParameter TextOut = new SqlParameter("@TextOut", SqlDbType.VarChar);
                TextOut.Direction = ParameterDirection.Output;
                TextOut.Size = 250;
                sqlCommand.Parameters.Add(TextOut);

                sqlCommand.Connection = db.Conn;

                //sqlCommand.ExecuteNonQuery();
                
                SqlDataAdapter storeSqlDataAdapter = new SqlDataAdapter();
                storeSqlDataAdapter.SelectCommand = sqlCommand;
                DataSet storeDataSet = new DataSet();
                storeSqlDataAdapter.Fill(storeDataSet, "LOGIN");
                DataTable storeDataTable = storeDataSet.Tables["LOGIN"];

                string sTextOut = (sqlCommand.Parameters["@TextOut"].Value).ToString();

               

                if(sTextOut!= "Login Okay.")
                {
                    //Alert.Show(sTextOut);
                    divMessage.InnerHtml = sTextOut;
                    return;
                }
                else
                {
                    Label3.Text = "Bienvenido, " + UserField1.Text + " a la Compañia "+ (string)this.Session["CompanyId"];
                    //Response.Redirect("")
                }

                string ErrorId = "";
                string ErrorMsg = "";

                ArrayList controles = new ArrayList(); //(ArrayList)this.Session["Controles"];
                ArrayList roles = new ArrayList(); //(ArrayList)this.Session["Roles"];
                ArrayList permissions = new ArrayList(); //(ArrayList)this.Session["Permissions"];

                foreach (DataRow storeDataRow in storeDataTable.Rows)
                {
                        if (Convert.ToString(storeDataRow["ErrorId"]) == "1")
                        {
                            ErrorId = Convert.ToString(storeDataRow["ErrorId"]);
                            ErrorMsg = Convert.ToString(storeDataRow["ErrorMsg"]);
                            Session["UserId"] = UserField1.Text;
                            if ((string)this.Session["UserId"] == "" || (string)this.Session["UserId"] == null)
                            {

                                Label3.Text = "Registre su Usuario.";
                                return;
                            }
                            
                            LogInBtn1.Visible = false;
                        }

                        if (Convert.ToString(storeDataRow["ErrorId"]) == "2")
                        {
                            controles.Add(Convert.ToString(storeDataRow["ErrorMsg"]));
                        }

                        if (Convert.ToString(storeDataRow["ErrorId"]) == "3")
                        {
                            roles.Add(Convert.ToString(storeDataRow["ErrorMsg"]));
                        }

                        if (Convert.ToString(storeDataRow["ErrorId"]) == "4")
                        {
                            permissions.Add(Convert.ToString(storeDataRow["ErrorMsg"]));
                        }                        
                }


                LogInBtn1.Visible = false;
                Label1.Visible = false;
                lblCompany.Visible = false;
                UserField1.Visible = false;
                Label2.Visible = false;
                Passwd1.Visible = false;
                companyDDL.Visible = false;
                LogOutBtn1.Enabled = true;
                LogOutBtn1.Visible = true;
                Session["Controles"] = controles;
                Session["Roles"] = roles;
                Session["Permissions"] = permissions;                
                Response.Redirect("Default.aspx");
            }
        }
    }
    protected void LogOutBtn1_PreRender(object sender, EventArgs e)
    {
        Session["FlagNoPerPag"] = "Y";
    }
    protected void LogInBtn1_PreRender(object sender, EventArgs e)
    {
        if ((string)this.Session["UserId"] == "" || (string)this.Session["UserId"] == null)
        {
            Label1.Visible = true;
            lblCompany.Visible = true;
            UserField1.Visible = true;
            Label2.Visible = true;
            Passwd1.Visible = true;
            LogInBtn1.Visible = true;
            companyDDL.Visible = true;
        }
    }
}


