
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;
using wsCtrlUsuarioFull.models;

namespace wsCtrlUsuarioFull
{
    public partial class Formulario_web1 : System.Web.UI.Page
    {
        protected async void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                await cargaDatosApi();
            }
        }

        // ✅ ESTE MÉTODO ES EL QUE USA EL ASPX
        protected async void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            await cargaDatosApi();
        }

        // ============================
        // MÉTODO PRINCIPAL
        // ============================
        private async Task cargaDatosApi()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string apiUrl = System.Configuration.ConfigurationManager.AppSettings["ApiUsuarioBaseUrl"] + "vwrptusuario?nombre=" + Uri.EscapeDataString(TextBox1.Text);

                    HttpResponseMessage respuesta = await client.GetAsync(apiUrl);

                    if (respuesta.IsSuccessStatusCode)
                    {
                        string resultado = await respuesta.Content.ReadAsStringAsync();

                        clsApiStatus objRespuesta = JsonConvert.DeserializeObject<clsApiStatus>(resultado);

                        if (objRespuesta.datos != null && objRespuesta.datos["vwRptUsuario"] != null)
                        {
                            JArray jsonArray = (JArray)objRespuesta.datos["vwRptUsuario"];

                            DataTable dt = JsonConvert.DeserializeObject<DataTable>(jsonArray.ToString());

                            GridView1.DataSource = dt;
                            GridView1.DataBind();

                            LabelTotal.Text = dt.Rows.Count.ToString();
                        }
                        else
                        {
                            GridView1.DataSource = null;
                            GridView1.DataBind();

                            LabelTotal.Text = "0";
                        }
                    }
                    else
                    {
                        Response.Write("<script>alert('Error de conexión con webapi');</script>");
                    }
                }
            }
            catch (Exception)
            {
                Response.Write("<script>alert('Error inesperado ...');</script>");
            }
        }

        // ============================
        // BOTÓN BUSCAR
        // ============================
        protected async void ImageButton5_Click(object sender, ImageClickEventArgs e)
        {
            await cargaDatosApi();
        }
    }
}

