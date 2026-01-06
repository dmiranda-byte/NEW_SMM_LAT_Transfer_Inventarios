<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMaster.master" AutoEventWireup="true" CodeFile="CreateTransfer.aspx.cs" Inherits="CreateTransfer" %>


<asp:Content ID="Content2" ContentPlaceHolderID="cphMain" Runat="Server">

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
                <div id="divMessage" runat="server" class="alert-danger" />
                <br />
            </div>
        </div>
        <div class="row">
            <div class="col-md-6">
                <asp:Panel ID="pnlHeader" runat="server" CssClass="page-curl shadow-bottom">
                    <label id="labelForm" runat="server" class="PanelHeading">Crea Transferencia Min-Max</label>
                    <div class="row">
                        <div class="col-md-12">
                            <ul class="myUL">
                                <li>
                                    <label class="myLabel">From Location</label>
                                    <asp:DropDownList ID="drpFromWhsCode" runat="server" DataTextField="WhsName" DataValueField="WhsCode" AutoPostBack="True" CssClass="myDdlLarge" OnSelectedIndexChanged="drpFromWhsCode_SelectedIndexChanged" />
                                </li>
                                <li>
                                    <label class="myLabel">To Location</label>
                                    <asp:DropDownList ID="drpToWhsCode" runat="server" DataTextField="WhsName" DataValueField="WhsCode" AutoPostBack="False" CssClass="myDdlLarge" />
                                </li>
                                <li>
                                    <label class="myLabel">Item Groups</label>
                                    <asp:DropDownList ID="drpItemGroups" runat="server" CssClass="myDdlLarge" DataTextField="GroupName" DataValueField="GroupCode" AutoPostBack="True" OnSelectedIndexChanged="drpItemGroups_SelectedIndexChanged" />
                                </li>
                                <li>
                                    <label class="myLabel" style="vertical-align:top;">Brand</label>
                                    <asp:ListBox ID="lstItemGroups" runat="server" CssClass="myDdlLarge" DataTextField="Brand" DataValueField="Brand" Height="105px" /> 
                                </li>
                                <li>
                                    <label class="myLabel">&nbsp;</label>
                                    <asp:Button ID="btnCreateTransfer" CssClass="mybtnlarge" runat="server" Text="Crear Transferencia" onclick="btnCreateTransfer_Click"  OnClientClick="return confirm('ALERTA!!\n\nEsta Seguro Que Quiere Crear Este Transfer?\n\nClick OK para Crear o Cancel para No Crear')" onload="btnCreateTransfer_Load"/>
                                </li>
                            </ul>
                        </div>
                    </div>
                    <div class="row">&nbsp;</div>
                </asp:Panel>
            </div>
        </div>
    </div>
</asp:Content>

