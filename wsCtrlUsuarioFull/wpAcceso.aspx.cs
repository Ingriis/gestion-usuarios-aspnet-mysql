using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

// -------------------------------------
using wsCtrlUsuarioFull.models;

namespace wsCtrlUsuarioFull
{
    public partial class wpAcceso : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected async void Button1_Click1(object sender, EventArgs e)
        {
            await cargaDatosApi();
        }

        private async Task cargaDatosApi()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {

                    string datos = JsonConvert.SerializeObject(new { usuario = TextBox1.Text, contrasena = TextBox2.Text });

                    HttpContent contenido =
                            new StringContent(datos, Encoding.UTF8, "application/json");
                    string urlApi = System.Configuration.ConfigurationManager.AppSettings["ApiUsuarioBaseUrl"] + "spvalidaracceso";

                    HttpResponseMessage respuesta =
                            await client.PostAsync(urlApi, contenido);

                    clsApiStatus objRespuesta = new clsApiStatus();

                    if (respuesta.IsSuccessStatusCode)
                    {
                        string resultado =
                                        await respuesta.Content.ReadAsStringAsync();
                        objRespuesta = JsonConvert.DeserializeObject<clsApiStatus>(resultado);

                        if (objRespuesta.ban == 1)
                        {

                            Session["nomUsuario"] = objRespuesta.datos["usu_nombre_completo"].ToString();
                            Session["usuUsuario"] = objRespuesta.datos["usu_usuario"].ToString();
                            Session["rolUsuario"] = objRespuesta.datos["tip_descripcion"].ToString();
                            Session["urlUsuario"] = objRespuesta.datos["usu_ruta"].ToString();


                            Response.Write("<script languaje='javascript'>" +
                                            "alert('Bienvenido " + 
                                            Session["nomUsuario"]+ " - [" +
                                            Session["rolUsuario"] + "]');" +
                                            "document.location.href = 'Formulario web1.aspx';" +
                                            "</script>");

                        }
                        else
                        {
                            Session["nomUsuario"] = "";
                            Session["usuUsuario"] = "";
                            Session["rolUsuario"] = "";
                            Session["urlUsuario"] = "";


                            Response.Write("<script languaje='javascript'>" +
                                            "alert('Acceso Denegado ...');" +
                                            "</script>");
                        }

                    }
                    else
                    {

                        Response.Write("<script languaje='javascript'>" +
                                            "alert('El servicio no responde, intentar mas tarde.');" +
                                            "</script>");
                    }

                }

            }
            catch (Exception ex)
            {

                Console.WriteLine("Ocurrio un error en cargaDatosApi: ");
                Console.Write(ex.ToString());
                Response.Write("<script languaje='javascript'>" +
                                            "alert('Ha sucedido un error en la aplicacion" +
                                            ", contacte al soporte técnico');" +
                                            "</script>");
            }
        }
    }
}