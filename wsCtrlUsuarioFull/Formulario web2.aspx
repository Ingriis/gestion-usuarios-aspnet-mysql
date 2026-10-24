<%@ Page Async="true" Title="" Language="C#" MasterPageFile="~/mpPrincipal.Master" AutoEventWireup="true" CodeBehind="Formulario web2.aspx.cs" Inherits="wsCtrlUsuarioFull.Formulario_web2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <section class="console-page-header" aria-labelledby="titulo-gestion">
        <nav class="breadcrumb-console" aria-label="Ruta de navegación"><span>Administración</span><span>/</span><strong>Usuarios</strong><span>/</span><strong>Administrar</strong></nav>
        <div class="console-title-row">
            <div>
                <h1 id="titulo-gestion">Administrar usuarios</h1>
                <p>Crea una cuenta o busca un usuario para actualizar su información.</p>
            </div>
        </div>
    </section>

    <section class="form-card" aria-labelledby="titulo-formulario-usuario">
        <div class="form-card-heading">
            <div>
                <h2 id="titulo-formulario-usuario">Información de la cuenta</h2>
                <p>Los campos se guardan en el directorio de usuarios.</p>
            </div>
        </div>
        <div class="formulario-usuarios">
            <div class="needs-validation" novalidate>
                <div class="campo-formulario grupo-busqueda">
                    <div>
                        <label for="ContentPlaceHolder1_TextBox1">Clave del usuario</label>
                        <asp:TextBox ID="TextBox1" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="TextBox1_TextChanged" placeholder="Ingresa una clave para buscar"></asp:TextBox>
                    </div>
                    <asp:Button ID="ButtonBuscar" runat="server" Text="Buscar" CssClass="btn btn-info" OnClick="ButtonBuscar_Click" />
                </div>
                <div class="campo-formulario">
                    <label for="ContentPlaceHolder1_TextBox2">Nombre</label>
                    <asp:TextBox ID="TextBox2" runat="server" CssClass="form-control" placeholder="Nombre"></asp:TextBox>
                </div>
                <div class="campo-formulario">
                    <label for="ContentPlaceHolder1_TextBox3">Apellido paterno</label>
                    <asp:TextBox ID="TextBox3" runat="server" CssClass="form-control" placeholder="Apellido paterno"></asp:TextBox>
                </div>
                <div class="campo-formulario">
                    <label for="ContentPlaceHolder1_TextBox4">Apellido materno</label>
                    <asp:TextBox ID="TextBox4" runat="server" CssClass="form-control" placeholder="Apellido materno"></asp:TextBox>
                </div>
                <div class="campo-formulario">
                    <label for="ContentPlaceHolder1_TextBox5">Nombre de usuario</label>
                    <asp:TextBox ID="TextBox5" runat="server" CssClass="form-control" placeholder="Usuario para iniciar sesión"></asp:TextBox>
                </div>
                <div class="campo-formulario campo-completo">
                    <label for="ContentPlaceHolder1_TextBox6">Contraseña</label>
                    <asp:TextBox ID="TextBox6" runat="server" CssClass="form-control" TextMode="Password" placeholder="Escribe una contraseña"></asp:TextBox>
                    <small>Al modificar, deja este campo vacío para conservar la contraseña actual.</small>
                </div>
                <div class="campo-formulario">
                    <label for="ContentPlaceHolder1_FileUpload1">Fotografía de perfil</label>
                    <asp:FileUpload ID="FileUpload1" runat="server" CssClass="form-control" />
                </div>
                <div class="campo-formulario">
                    <label for="ContentPlaceHolder1_DropDownList1">Rol de usuario</label>
                    <asp:DropDownList ID="DropDownList1" runat="server" CssClass="form-control"></asp:DropDownList>
                </div>
                <asp:HiddenField ID="HiddenRuta" runat="server" />
                <div class="acciones-formulario campo-completo">
                    <asp:Button ID="Button1" runat="server" Text="Crear usuario" CssClass="btn btn-primary" OnClick="Button1_Click" />
                    <asp:Button ID="Button2" runat="server" Text="Guardar cambios" CssClass="btn btn-warning" OnClick="Button2_Click" />
                    <asp:Button ID="Button3" runat="server" Text="Eliminar usuario" CssClass="btn btn-danger" OnClick="Button3_Click" />
                </div>
            </div>
        </div>
    </section>

    <script>
        (function () {
            'use strict';
            window.addEventListener('load', function () {
                var forms = document.getElementsByClassName('needs-validation');
                Array.prototype.filter.call(forms, function (form) {
                    form.addEventListener('submit', function (event) {
                        if (!form.checkValidity()) {
                            event.preventDefault();
                            event.stopPropagation();
                        }
                        form.classList.add('was-validated');
                    }, false);
                });
            }, false);
        })();
    </script>
</asp:Content>
