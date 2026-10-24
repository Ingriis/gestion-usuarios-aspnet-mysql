<%@ Page Async="true" Title="" Language="C#" MasterPageFile="~/mpPrincipal.Master" AutoEventWireup="true" CodeBehind="Formulario web1.aspx.cs" Inherits="wsCtrlUsuarioFull.Formulario_web1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="App_Themes/principal/principal.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <section class="console-page-header" aria-labelledby="titulo-reporte">
        <nav class="breadcrumb-console" aria-label="Ruta de navegación"><span>Administración</span><span>/</span><strong>Usuarios</strong></nav>
        <div class="console-title-row">
            <div>
                <h1 id="titulo-reporte">Usuarios</h1>
                <p>Consulta y administra las cuentas registradas en el sistema.</p>
            </div>
            <a class="console-primary-action" href="Formulario%20web2.aspx">
                <svg viewBox="0 0 24 24" aria-hidden="true"><path d="M12 5v14M5 12h14"/></svg>
                <span>Nuevo usuario</span>
            </a>
        </div>
        <div class="console-summary"><span class="summary-dot"></span><span>Usuarios registrados</span><asp:Label ID="LabelTotal" runat="server" CssClass="summary-count"></asp:Label></div>
    </section>

    <section class="data-panel" aria-labelledby="titulo-tabla-usuarios">
        <div class="data-panel-heading">
            <div>
                <h2 id="titulo-tabla-usuarios">Directorio de usuarios</h2>
                <p>Perfiles, nombres de usuario y roles de acceso.</p>
            </div>
        </div>
        <div class="panel-toolbar">
            <div class="toolbar-copy"><strong>Todos los usuarios</strong><span>Busca por clave o nombre de usuario.</span></div>
            <div class="busqueda-usuarios">
                <asp:TextBox ID="TextBox1" runat="server" CssClass="control-busqueda" placeholder="Buscar usuarios" aria-label="Buscar por clave o nombre de usuario"></asp:TextBox>
                <asp:ImageButton ID="ImageButton5" runat="server" ImageUrl="~/imagenes/icons/search.svg" AlternateText="Buscar" ToolTip="Buscar usuarios" CssClass="boton-buscar" OnClick="ImageButton5_Click" />
            </div>
        </div>
        <div class="tabla-responsive">
            <asp:GridView ID="GridView1" runat="server" CssClass="grid-usuarios" GridLines="None"
                AllowPaging="True" AutoGenerateColumns="False" PageSize="5"
                OnPageIndexChanging="GridView1_PageIndexChanging"
                EmptyDataText="No hay usuarios para mostrar">
                <Columns>
                    <asp:BoundField DataField="Clave" HeaderText="ID" />
                    <asp:BoundField DataField="Nombre" HeaderText="Nombre" />
                    <asp:BoundField DataField="Usuario" HeaderText="Usuario" />
                    <asp:BoundField DataField="Rol" HeaderText="Rol" />
                    <asp:TemplateField HeaderText="Perfil">
                        <ItemTemplate>
                            <a href='<%# Eval("Foto") %>' target="_blank" rel="noopener" aria-label="Ver foto de usuario">
                                <img src='<%# Eval("Foto") %>' width="50" height="50" alt="Foto de usuario" onerror="this.onerror=null;this.src='imagenes/avatar-placeholder.svg';this.parentElement.href=this.src;" />
                            </a>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <PagerStyle HorizontalAlign="Center" />
            </asp:GridView>
        </div>
    </section>
</asp:Content>
