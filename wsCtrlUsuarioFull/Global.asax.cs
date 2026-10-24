using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace wsCtrlUsuarioFull
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            // VARIABLES DE APLICACION (WEB GLOBAL)
            Application["nomEmpresa"] = "ITPachuca - ecommerce";
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            // VARIABLES DE SESION (WEB LOCAL)
            Session["nomUsuario"] = "";
            Session["usuUsuario"] = "";
            Session["rolUsuario"] = "";
            Session["urlUsuario"] = "";
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}