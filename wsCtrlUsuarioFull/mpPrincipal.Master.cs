using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace wsCtrlUsuarioFull
{
    public partial class mpPrincipal : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // VALIDACION DE LA SEGURIDAD PARA EL ACCESO AUTORIZADO
            if (Session["nomUsuario"].ToString() == "" &&
                Session["usuUsuario"].ToString() == "" &&
                Session["rolUsuario"].ToString() == "" &&
                Session["urlUsuario"].ToString() == "" )
            {
                Response.Write("<script languaje='javascript'>" +
                            "alert('Acceso NO autorizado ... por favor inicia sesión');" +
                            "document.location.href = 'wpAcceso.aspx'" +
                            "</script>");
            }

            // CONFIGURACION DE LA APLICACION Y SESION
            string paginaActual = System.IO.Path.GetFileName(Server.UrlDecode(Request.Url.AbsolutePath));
            LinkButton1.CssClass = paginaActual.Equals("Formulario web1.aspx", StringComparison.OrdinalIgnoreCase) ? "console-nav-link is-active" : "console-nav-link";
            LinkButton2.CssClass = paginaActual.Equals("Formulario web2.aspx", StringComparison.OrdinalIgnoreCase) ? "console-nav-link is-active" : "console-nav-link";

            Label1.Text = Application["nomEmpresa"].ToString();
            Label6.Text = Session["nomUsuario"].ToString() + " - [" + Session["rolUsuario"].ToString() + "]";
            Image2.ImageUrl = Session["urlUsuario"].ToString();
            Image2.Attributes["onerror"] = "this.onerror=null;this.src='imagenes/avatar-placeholder.svg';";
        }

        protected void ImageButton4_Click(object sender, ImageClickEventArgs e)
        {
            // CERRAR SESION DEL USUARIO
            // (GESTION DE VARIABLES)
            Session["nomUsuario"] = "";
            Session["usuUsuario"] = "";
            Session["rolUsuario"] = "";
            Session["urlUsuario"] = "";

            // Response.Write("Acceso Denegado ...");
            Response.Write("<script languaje='javascript'>" +
                            "alert('Sesión cerrada exitosamente!');" +
                            "document.location.href = 'wpAcceso.aspx'" +
                            "</script>");
        }
    }
}