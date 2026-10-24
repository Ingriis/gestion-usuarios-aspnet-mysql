<%@ Page Async="true" Language="C#" AutoEventWireup="true" CodeBehind="wpAcceso.aspx.cs" Inherits="wsCtrlUsuarioFull.wpAcceso" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Acceso | Gestión de Usuarios</title>
    <link href="bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <link href="App_Themes/principal/mpPrincipal.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <main class="login-page">
            <section class="login-card" aria-labelledby="titulo-acceso">
                <div class="login-marca">Gestión de Usuarios · ITPachuca</div>
                <h1 id="titulo-acceso">Acceso al sistema</h1>
                <p class="login-subtitulo">Ingresa tus datos para continuar.</p>

                <div class="login-campo">
                    <label for="TextBox1">Usuario</label>
                    <asp:TextBox ID="TextBox1" runat="server" CssClass="form-control" placeholder="Nombre de usuario" aria-label="Usuario"></asp:TextBox>
                </div>
                <div class="login-campo">
                    <label for="TextBox2">Contraseña</label>
                    <asp:TextBox ID="TextBox2" runat="server" CssClass="form-control" TextMode="Password" placeholder="Contraseña" aria-label="Contraseña"></asp:TextBox>
                </div>
                <div class="login-acciones">
                    <asp:Button ID="Button1" runat="server" Text="Acceder" CssClass="btn btn-primary" OnClick="Button1_Click1" />
                    <asp:Button ID="Button2" runat="server" Text="Cancelar" CssClass="btn btn-light" />
                </div>
            </section>
        </main>
    </form>
</body>
</html>
