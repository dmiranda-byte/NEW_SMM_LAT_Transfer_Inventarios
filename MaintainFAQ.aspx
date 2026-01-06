<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMaster.master" AutoEventWireup="true" CodeFile="MaintainFAQ.aspx.cs" Inherits="MaintainFAQ" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="tel" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphMain" runat="Server">
    <div class="container-fluid">
        <div class="row">
            <div class="col-md-5">
                <asp:Panel ID="pnlAddApps" runat="server" CssClass="Panel" Height="210px">
                    <label class="PanelHeading">Agregar Preguntas Frecuentes</label>
                    <div class="row">
                        <div class="col-md-12">
                            <ul class="myUL">
                                <li>
                                    <label class="myLabel">Casos</label>
                                    <tel:RadTextBox ID="rtbCasos" runat="server" Width="300px" />
                                </li>
                                <li>
                                    <label class="myLabel">Soluciones</label>
                                    <tel:RadTextBox ID="rtbSoluciones" runat="server" Width="300px" />
                                </li>
                                <li>
                                    <label class="myLabel">Solucion Link</label>
                                    <tel:RadTextBox ID="rtbSolucionLink" runat="server" Width="300px" />
                                </li>
                                <li>
                                    <label class="myLabel">Subir</label>
                                    <asp:FileUpload runat="server" ID="fuDocs" CssClass="btn btn-warning btn-sm" />
                                </li>
                                <li>&nbsp;
                                </li>
                                <li>
                                    <label class="myLabel">&nbsp;</label>
                                    <tel:RadButton ID="rbtnAddFaq" runat="server" Text="Agregar" OnClick="rbtnAddFaq_Click" ToolTip="Agregar">
                                        <Icon SecondaryIconCssClass="fa fa-plus icon-green" SecondaryIconRight="4" SecondaryIconTop="5" />
                                    </tel:RadButton>
                                </li>
                            </ul>
                        </div>
                    </div>
                </asp:Panel>
            </div>
            <div class="col-md-5">
                <asp:Panel ID="Panel1" runat="server" CssClass="Panel" Height="210px">
                    <label class="PanelHeading">Preguntas Frecuentes Sobre Actualizaciones</label>
                    <div class="row">
                        <div class="col-md-12">
                            <ul class="myUL">
                                <li>
                                    <label class="myLabel">Seleccione Casos</label>
                                    <tel:RadComboBox ID="rcbCasos" runat="server" Height="120px" DropDownAutoWidth="Disabled" Width="300px"
                                        HighlightTemplatedItems="true"
                                        AppendDataBoundItems="true"
                                        EmptyMessage="Seleccione Casos" AutoPostBack="true" OnSelectedIndexChanged="rcbCasos_SelectedIndexChanged"
                                        Font-Italic="false">
                                        <ExpandAnimation Type="OutQuart" Duration="500" />
                                        <CollapseAnimation Type="OutQuint" Duration="300" />
                                    </tel:RadComboBox>
                                </li>
                                <li>
                                    <label class="myLabel">Casos</label>
                                    <tel:RadTextBox ID="rtbUCasos" runat="server" Width="300px" />
                                </li>
                                <li>
                                    <label class="myLabel">Soluciones</label>
                                    <tel:RadTextBox ID="rtbUSoluciones" runat="server" Width="300px" />
                                </li>
                                <li>
                                    <label class="myLabel">Solucion Link</label>
                                    <tel:RadTextBox ID="rtbUSolucionLink" runat="server" Width="300px" />
                                </li>
                                <li>
                                    <label class="myLabel">Subir</label>
                                    <asp:FileUpload runat="server" ID="fuUDocs" CssClass="btn btn-warning btn-sm" />
                                </li>
                                <li>&nbsp;
                                </li>
                                <li>
                                    <label class="myLabel">&nbsp;</label>
                                    <tel:RadButton ID="rtbUpdate" runat="server" Text="Actualizar" OnClick="rtbUpdate_Click" ToolTip="Actualizar">
                                        <Icon SecondaryIconCssClass="fa fa-edit icon-green" SecondaryIconRight="4" SecondaryIconTop="5" />
                                    </tel:RadButton>
                                    <tel:RadButton ID="rtbDelete" runat="server" Text="Borrar" OnClick="rtbDelete_Click" ToolTip="Borrar">
                                        <Icon SecondaryIconCssClass="fa fa-trash-o icon-red" SecondaryIconRight="4" SecondaryIconTop="5" />
                                    </tel:RadButton>
                                </li>
                            </ul>
                        </div>
                    </div>
                </asp:Panel>
            </div>
        </div>
    </div>
</asp:Content>

