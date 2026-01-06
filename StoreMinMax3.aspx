<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMaster.master" AutoEventWireup="true" CodeFile="StoreMinMax3.aspx.cs" Inherits="StoreMinMax3" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="tel" %>

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
                <div id="divMessage" runat="server" class="alert-danger" />
                <br />
            </div>
        </div>
        <div class="row">
            <div class="col-md-12">
                <asp:Panel ID="pnlHeader" runat="server" CssClass="page-curl shadow-bottom">
                    <label ID="labelForm" runat="server" class="PanelHeading">Valores Mínimo-Máximo</label>
                    <div class="row">
                        <div class="col-md-5">
                            <ul class="myUL">
                                <li>
                                    <label class="myLabel">Operación</label>
                                    <asp:DropDownList ID="drpToWhsCode" runat="server" DataTextField="WhsName" DataValueField="WhsCode" CssClass="myDdlLarge" />
                                </li>
                                <li>
                                    <label class="myLabel">Categoría</label>
                                    <asp:DropDownList ID="DropDownItmGrp" runat="server" DataTextField="GroupName" DataValueField="GroupCode" CssClass="myDdlLarge" OnDataBinding="DropDownItmGrp_DataBinding" OnDataBound="DropDownItmGrp_DataBound" OnLoad="DropDownItmGrp_Load" OnPreRender="DropDownItmGrp_PreRender" OnSelectedIndexChanged="DropDownItmGrp_SelectedIndexChanged" OnTextChanged="DropDownItmGrp_TextChanged" AutoPostBack="True" />
                                </li>
                                <li>
                                    <label class="myLabel" style="vertical-align: top;">Marca</label>
                                    <asp:ListBox ID="lstItemGroups" runat="server" CssClass="myDdlLarge" DataTextField="Brand" DataValueField="Brand" Height="105px" />
                                </li>
                                <li>
                                    <!-- //2019-ABR-09: Modificado por Aldo Reina, para la búsqueda por código de barras: -->
                                    <%--<label class="myLabel" style="vertical-align: top;">Busqueda por Producto</label>--%>
                                    <label class="myLabel" style="vertical-align: top;">Cod. Producto &#47; Cod. Barras</label>
                                    <asp:TextBox ID="ItemTextBox" runat="server" CssClass="myDdlLarge"></asp:TextBox>
                                </li>
                                <!-- //2019-ABR-09: Agregado por Aldo Reina, para la búsqueda por código de barras: -->
                                <li>
                                    <label class="myLabel" style="vertical-align:top;"></label>
                                    <asp:DropDownList ID="ItemList" ClientIDMode="Static" runat="server" Visible="false" AutoPostBack="True" OnSelectedIndexChanged="ItemList_SelectedIndexChanged" CssClass="myDdlLarge"></asp:DropDownList>
                                    <telerik:RadButton RenderMode="Lightweight" ID="rbtnCancel" runat="server" Text=" Cancel" OnClick="RbtnCancel_Click" Visible="false">
                                        <Icon PrimaryIconCssClass="rbCancel" PrimaryIconLeft="5" PrimaryIconBottom="11"></Icon>
                                    </telerik:RadButton>
                                </li>
                            </ul>
                        </div>
                        <div class="col-md-7">
                            <ul class="myUL">
                                <li class="radio">
                                    <asp:RadioButton ID="radioAllItems" runat="server" Checked="true" GroupName="grpItemsToDisplay" Text="Ver Todos los Artículos" CssClass="myLabel" />
                                    <asp:RadioButton ID="radioHoldItems" runat="server" GroupName="grpItemsToDisplay" Text="Ver sólo Artículos aguardados (Hold)" CssClass="myLabel" />
                                </li>
                                <li>
                                    <asp:Button ID="btnCreateWorksheet" runat="server" Text="Ver valores" OnClick="btnCreateWorksheet_Click" CssClass="mybtnmeduim" ToolTip="Create Worksheet" />&nbsp; 
                                    <asp:Button ID="btnSaveChanges" runat="server" OnClick="btnSaveChanges_Click" Text="Salvar cambios" CssClass="mybtnmeduim" ToolTip="Save Changes" />
                                    &nbsp; 
                                    <asp:Button ID="MinUpdBotton" runat="server" OnClick="MinUpdBotton_Click" Text="Actualiza Minimos al" CssClass="mybtnlarge" ToolTip="Min Up" />
                                    &nbsp; 
                                    <asp:TextBox ID="PorcentajeTBox" runat="server" Width="20px" Text="80" />&nbsp;<asp:Label ID="PorcLabel" runat="server" Text="%" />
                                </li>
                                <li>&nbsp;
                                </li>
                                <li>
                                    <asp:CheckBox ID="NotMinMaxPlannedCheckBox" runat="server" Text="Productos no planificados con inventario disponible en Bodegas" AutoPostBack="True" OnCheckedChanged="NotMinMaxPlannedCheckBox_CheckedChanged" CssClass="myCheckboxList myLabel" />
                                </li>
                                <li>
                                    <asp:Label ID="GreenLabel" runat="server" ForeColor="#009933" Text="Verde representa artículos que tienen menos de 30 días en la operación" Visible="False" />
                                </li>
                                <li>
                                    <asp:CheckBox ID="NotInSapCheckBox" runat="server" Text="Productos que no existen en SAP aun" AutoPostBack="True" OnCheckedChanged="NotInSapCheckBox_CheckedChanged" CssClass="myCheckboxList myLabel" />
                                </li>
                                <li>
                                    <asp:Button ID="btnExport" runat="server" Text="Exportar a Excel" OnClick="btnExport_Click" CssClass="mybtnmeduim" ToolTip="Export To Excel" />&nbsp; 
                                </li>
                            </ul>
                        </div>
                    </div>
                    <div class="row">&nbsp;</div>
                </asp:Panel>
            </div>
        </div>
        <div class="row">&nbsp;</div>
        <div class="row">&nbsp;</div>
        <div class="row">
            <div class="col-md-12">
                <tel:RadGrid ID="GridView1" runat="server" Width="100%" ShowStatusBar="true" AutoGenerateColumns="False" HeaderStyle-Font-Underline="true"
                    AllowSorting="true" AllowMultiRowSelection="False" AllowPaging="false" PageSize="15" CssClass="Panel"
                    DataSourceID="ObjectDataSource1" OnItemDataBound="GridView1_ItemDataBound">
                    <PagerStyle Mode="Slider"></PagerStyle>
                    <SortingSettings EnableSkinSortStyles="false" />
                    <ExportSettings OpenInNewWindow="true" IgnorePaging="true" Excel-Format="Xlsx" HideStructureColumns="true" FileName="MinMaxExport" />
                    <MasterTableView Width="100%" AllowNaturalSort="false" DataKeyNames="ITEM" CommandItemDisplay="Top">
                        <CommandItemSettings ShowExportToExcelButton="false" ShowAddNewRecordButton="false" ShowRefreshButton="false" />
                        <ColumnGroups>
                            <tel:GridColumnGroup HeaderText="Inventario" Name="WhsOnHand" HeaderStyle-HorizontalAlign="Center" />
                        </ColumnGroups>
                        <Columns>
                            <tel:GridTemplateColumn UniqueName="TemplateColumn1" HeaderText="Artículo" SortExpression="ITEM" HeaderStyle-Width="70px" DataType="System.String">
                                <ItemTemplate>
                                </ItemTemplate>
                            </tel:GridTemplateColumn>
                            <tel:GridBoundColumn SortExpression="BarCode" HeaderText="Bar Code" HeaderButtonType="TextButton" DataField="BarCode" UniqueName="BarCode" HeaderStyle-Width="100px" />
                            <tel:GridBoundColumn SortExpression="ITEMNAME" HeaderText="Descripción" HeaderButtonType="TextButton" DataField="ITEMNAME" UniqueName="ITEMNAME" HeaderStyle-Width="400px" />
                            <tel:GridBoundColumn SortExpression="CASE_PACK" HeaderText="Artículos por Caja" HeaderButtonType="TextButton" DataField="CASE_PACK" UniqueName="CASE_PACK" Display="false" />
                            <tel:GridBoundColumn SortExpression="OnHand" HeaderText="OnHand" HeaderButtonType="TextButton" DataField="OnHand" UniqueName="OnHand" DataFormatString="{0:N0}" HeaderStyle-Width="60px" />
                            <tel:GridTemplateColumn HeaderText="Mínimo" SortExpression="MIN_QTY" HeaderStyle-Width="65px">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtMin" runat="server" CssClass="txt60" Text='<%#Bind("MIN_QTY","{0:#}") %>' Width="55px"></asp:TextBox>
                                </ItemTemplate>
                            </tel:GridTemplateColumn>
                            <tel:GridTemplateColumn HeaderText="Máximo" SortExpression="MAX_QTY" HeaderStyle-Width="65px">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtMax" runat="server" CssClass="txt60" Text='<%#Bind("MAX_QTY","{0:#}") %>' Width="55px"></asp:TextBox>
                                </ItemTemplate>
                            </tel:GridTemplateColumn>
                            <tel:GridTemplateColumn HeaderText="Hold" HeaderStyle-Width="50px" UniqueName="HOLD">
                                <ItemTemplate>
                                    <asp:CheckBox ID="chkHold" runat="server" Checked='<% #Bind("HOLD")%>' />
                                </ItemTemplate>
                            </tel:GridTemplateColumn>
                            <tel:GridTemplateColumn HeaderText="Pri" HeaderStyle-Width="50px" UniqueName="PRIOR">
                                <ItemTemplate>
                                    <asp:CheckBox ID="chkPriotity" runat="server" Checked='<% #Bind("PRIOR")%>' />
                                </ItemTemplate>
                            </tel:GridTemplateColumn>
                            <tel:GridTemplateColumn HeaderText="Replacement Item" SortExpression="REPLACEMENT_ITEM" Display="false">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtReplacementItem" runat="server" CssClass="txt60" Text='<%#Bind("replacement_item")%>'></asp:TextBox>
                                </ItemTemplate>
                            </tel:GridTemplateColumn>
                            <tel:GridTemplateColumn HeaderText="Comment" SortExpression="COMMENT" Display="false">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtComment" runat="server" CssClass="txt160" Text='<%#Bind("COMMENT")%>'></asp:TextBox>
                                </ItemTemplate>
                            </tel:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                    <ClientSettings>
                        <Resizing AllowColumnResize="true" />
                        <Selecting AllowRowSelect="true" />
                        <Scrolling AllowScroll="true" UseStaticHeaders="true" />
                    </ClientSettings>
                </tel:RadGrid>
                <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" SelectMethod="GetMinMaxQuantities" TypeName="Reports" OnSelected="ObjectDataSource1_Selected">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="drpToWhsCode" Name="store" PropertyName="SelectedValue" Type="String" />
                        <asp:Parameter Name="depts" Type="String" />
                        <asp:Parameter Name="brands" Type="String" />
                        <asp:Parameter Name="displayAll" Type="String" />
                        <asp:Parameter Name="NoPlanned" Type="String" />
                        <asp:Parameter Name="NoInSap" Type="String" />
                        <asp:Parameter Name="Item" Type="String" />
                        <asp:Parameter Name="companyId" Type="String" />
                        <asp:Parameter Name="control" Type="String" />
                        <asp:Parameter Name="whsTypes" Type="String" />
                    </SelectParameters>
                </asp:ObjectDataSource>

                <%--<asp:GridView ID="GridView1" runat="server" CssClass="GridViewPanel"
                    DataSourceID="ObjectDataSource1" GridLines="Vertical" AllowSorting="True"
                    AutoGenerateColumns="False" PageSize="1000" EnableModelValidation="True"
                    OnRowCreated="GridView1_RowCreated" OnRowDataBound="GridView1_RowDataBound" AllowPaging="True">
                    <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                    <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                    <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                    <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
                    <HeaderStyle BackColor="#004990" Font-Bold="True" ForeColor="White" HorizontalAlign="Center" />
                    <AlternatingRowStyle BackColor="Gainsboro" />

                    <Columns>

                        <asp:TemplateField HeaderText="Categoría" SortExpression="itmsgrpnam" Visible="False">
                            <ItemTemplate>
                                <%# Eval("itmsgrpnam").ToString().Trim()%>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Left" />
                            <HeaderStyle HorizontalAlign="Left" VerticalAlign="Bottom" />
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Artículo" SortExpression="item" HeaderStyle-Width="100px" ItemStyle-Width="100px">
                            <ItemTemplate>
                                <%# Eval("item").ToString().Trim()%>
                                <asp:HiddenField ID="hdnLoc" runat="server" Value='<%#Eval("loc").ToString().Trim()%>' />
                                <asp:HiddenField ID="hdnItem" runat="server" Value='<%#Eval("item").ToString().Trim()%>' />
                                <asp:HiddenField ID="hdnItemDesc" runat="server" Value='<%#Eval("itemname").ToString().Trim()%>' />
                                <asp:HiddenField ID="hdnCasePack" runat="server" Value='<%#Eval("case_pack").ToString().Trim()%>' />
                                <asp:HiddenField ID="hdnOnHand" runat="server" Value='<%#Eval("Onhand").ToString().Trim()%>' />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" Font-Bold="True" />
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Bottom" />
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Descripción" SortExpression="itemname" HeaderStyle-Width="350px" ItemStyle-Width="350px">
                            <ItemTemplate>
                                <%# Eval("itemname").ToString().Trim()%>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Left" />
                            <HeaderStyle HorizontalAlign="Left" VerticalAlign="Bottom" />
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Artículos por Caja" SortExpression="case_pack" Visible="False">
                            <ItemTemplate>
                                <%# Eval("case_pack").ToString().Trim()%>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" Width="80px" />
                            <HeaderStyle HorizontalAlign="Left" VerticalAlign="Bottom" />
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="OnHand" SortExpression="Onhand" HeaderStyle-Width="100px" ItemStyle-Width="100px">
                            <ItemTemplate>
                                <span title="Store onhand"><%# Eval("Onhand").ToString().Trim()%></span>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Right" />
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Bottom" />
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Mínimo" SortExpression="min_qty" HeaderStyle-Width="100px" ItemStyle-Width="100px">
                            <ItemTemplate>
                                <asp:TextBox ID="txtMin" runat="server" CssClass="txt60" Text='<%#Bind("min_qty","{0:#}") %>'></asp:TextBox>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Right" />
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Bottom" />
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Máximo" SortExpression="max_qty" HeaderStyle-Width="100px" ItemStyle-Width="100px">
                            <ItemTemplate>
                                <asp:TextBox ID="txtMax" runat="server" CssClass="txt60" Text='<%#Bind("max_qty","{0:#}") %>'></asp:TextBox>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Right" />
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Bottom" />
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="A" Visible="True">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkHold" runat="server"
                                    Checked='<% #Bind("hold")%>' />
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Bottom" Font-Overline="False" Font-Underline="True" />
                            <ItemStyle HorizontalAlign="Center" Width="30px" />
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Replacement&lt;br&gt;Item" SortExpression="replacement_item" Visible="False">
                            <ItemTemplate>
                                <asp:TextBox ID="txtReplacementItem" runat="server" CssClass="txt60" Text='<%#Bind("replacement_item")%>'></asp:TextBox>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Bottom" />
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Comment" SortExpression="Comment" Visible="False">
                            <ItemTemplate>
                                <asp:TextBox ID="txtComment" runat="server" CssClass="txt160" Text='<%#Bind("Comment")%>'></asp:TextBox>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Left" />
                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Bottom" />
                        </asp:TemplateField>

                    </Columns>

                </asp:GridView>--%>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        function generateWorksheet() {
            url = "MinMaxWorksheet.aspx";
            var depts = "";
            var store = "";

            var lstItemGroups = document.getElementById("<%=lstItemGroups.ClientID%>");
            var drpToWhsCode = document.getElementById("<%=drpToWhsCode.ClientID%>");

            store = drpToWhsCode.options[drpToWhsCode.selectedIndex].value;

            for (var i = 0; i < lstItemGroups.options.length; i++) {
                if (lstItemGroups.options[i].selected) {
                    depts += lstItemGroups.options[i].value + ",";
                }
            }

            if (depts.length > 0 && store.length > 0) {
                depts = depts.substr(0, depts.length - 1);
                url += "?store=" + store + "&depts=" + depts;

                popUpReport(url);
            }
            else {

            }
        }

        function generateReport() {
            url = "MinMaxReport.aspx";
            var depts = "";
            var store = "";

            var lstItemGroups = document.getElementById("<%=lstItemGroups.ClientID%>");
            var drpToWhsCode = document.getElementById("<%=drpToWhsCode.ClientID%>");

            store = drpToWhsCode.options[drpToWhsCode.selectedIndex].value;

            for (var i = 0; i < lstItemGroups.options.length; i++) {
                if (lstItemGroups.options[i].selected) {
                    depts += lstItemGroups.options[i].value + ",";
                }
            }

            if (depts.length > 0 && store.length > 0) {
                depts = depts.substr(0, depts.length - 1);
                url += "?store=" + store + "&depts=" + depts;

                popUpReport(url);
            }
            else {

            }
        }
    </script>


</asp:Content>

