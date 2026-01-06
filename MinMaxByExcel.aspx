<%@ Page Title="Sube MinMax por Excel" Language="C#" MasterPageFile="~/SiteMaster.master" AutoEventWireup="true" CodeFile="MinMaxByExcel.aspx.cs" Inherits="MinMaxByExcel" %>


<asp:Content ID="Content2" ContentPlaceHolderID="cphMain" runat="Server">

    <style>
        .myLabelMedium, .myLabelSmall {
            padding-left: 5px;
        }

        .myImageButton {
            vertical-align: middle;
            height: 20px;
            width: 20px;
            margin-left: 5px;
        }
    </style>

    <div class="container" style="margin-left: 20px;">
        <div class="row">
            <div class="col-md-12">
                <div id="divMessage" runat="server" />
                <br />
            </div>
        </div>
        <div class="row">
            <div class="col-md-8">
                <asp:Panel ID="pnlAddApps" runat="server" CssClass="page-curl shadow-bottom">
                    <label ID="labelForm" runat="server" class="PanelHeading">Subir Min-Max por Excel</label>
                    <div class="row">
                        <div class="col-md-8">
                            <ul class="myUL">
                                <li>
                                    <label class="myLabel">Operacion</label>
                                    <asp:DropDownList ID="drpFromWhsCode" runat="server" DataTextField="WhsName" DataValueField="WhsCode" AutoPostBack="True" CssClass="myDdlExtraLarge" />
                                </li>
                                <li>
                                    <label class="myLabel">Archivo Excel</label>
                                    <asp:FileUpload ID="FileUpload1" runat="server" CssClass="mybtnXlarge" />
                                </li>
                                <li>
                                    <label class="myLabel">&nbsp;</label>
                                    <asp:Button ID="btnCreateDraft" runat="server" Text="Subir Min-Max" OnClick="btnCreateDraft_Click" OnClientClick="return confirm('ALERTA!!\n\nEsta Seguro Que Quiere Crear/Actualizar El Min-Max Con El Excel Seleccionado?\n\nClick OK para Crear o Cancel para No Crear')" CssClass="mybtnmeduim" />
                                </li>
                            </ul>
                        </div>
                        <div class="col-md-4">
                            <asp:Image runat="server" ID="imgSample" ImageUrl="~/Images/MinMaxExcelFormat.JPG" Width="150px" Height="100px" />
                        </div>
                    </div>
                    <div class="row">&nbsp;</div>
                </asp:Panel>
            </div>
        </div>
        <div class="row">&nbsp;</div>
        <div class="row">
            <div class="col-md-12">
                <asp:Label ID="Label2" runat="server" Font-Bold="True" Font-Size="Medium" ForeColor="#CC0000"></asp:Label>
            </div>
        </div>
        <div class="row">&nbsp;</div>
        <div class="row">
            <div class="col-md-4">
                <asp:GridView ID="GridView2" runat="server" CssClass="GridViewPanel"
                    EnableModelValidation="False" ForeColor="#333333" EnableTheming="False"
                    EnableViewState="False" HorizontalAlign="Left">
                    <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                    <EditRowStyle BackColor="#999999" />
                    <FooterStyle BackColor="#CCCCCC" Font-Bold="True" ForeColor="White" />
                    <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
                    <PagerStyle BackColor="#999999" ForeColor="White" HorizontalAlign="Left" />
                    <RowStyle BackColor="#EEEEEE" ForeColor="#333333" HorizontalAlign="Left" />
                    <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="#333333" />

                </asp:GridView>
            </div>
        </div>
    </div>
</asp:Content>

