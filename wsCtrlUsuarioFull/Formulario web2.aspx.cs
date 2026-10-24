using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using wsCtrlUsuarioFull.models;
using System.IO;
using System.Linq;

namespace wsCtrlUsuarioFull
{
    public partial class Formulario_web2 : System.Web.UI.Page
    {
        private readonly string urlBase = System.Configuration.ConfigurationManager.AppSettings["ApiUsuarioBaseUrl"];

        protected async void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                await cargaDatosTipoUsuario();
        }

        private async Task cargaDatosTipoUsuario()
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(urlBase + "vwTipoUsuario");
                string json = await response.Content.ReadAsStringAsync();

                clsApiStatus obj = JsonConvert.DeserializeObject<clsApiStatus>(json);
                var key = obj.datos.Properties().First().Name;
                JArray arr = (JArray)obj.datos[key];

                DataTable dt = JsonConvert.DeserializeObject<DataTable>(arr.ToString());

                DropDownList1.DataSource = dt;
                DropDownList1.DataTextField = "descripcion";
                DropDownList1.DataValueField = "clave";
                DropDownList1.DataBind();
            }
        }

        protected async void TextBox1_TextChanged(object sender, EventArgs e)
        {
            await buscarUsuario();
        }

        protected async void ButtonBuscar_Click(object sender, EventArgs e)
        {
            await buscarUsuario();
        }

        private async Task buscarUsuario()
        {
            using (HttpClient client = new HttpClient())
            {
                string url = urlBase + "getusuario?id=" + TextBox1.Text;

                var response = await client.GetAsync(url);
                string json = await response.Content.ReadAsStringAsync();

                clsApiStatus obj = JsonConvert.DeserializeObject<clsApiStatus>(json);
                var key = obj.datos.Properties().First().Name;
                JArray arr = (JArray)obj.datos[key];

                if (arr.Count > 0)
                {
                    var u = arr[0];

                    TextBox1.Text = u["USU_CVE_USUARIO"].ToString();
                    TextBox2.Text = u["USU_NOMBRE"].ToString();
                    TextBox3.Text = u["USU_APELLIDO_PATERNO"].ToString();
                    TextBox4.Text = u["USU_APELLIDO_MATERNO"].ToString();
                    TextBox5.Text = u["USU_USUARIO"].ToString();

                    HiddenRuta.Value = u["USU_RUTA"]?.ToString() ?? "";

                    if (u["TIP_CVE_TIPOUSUARIO"] != null)
                        DropDownList1.SelectedValue = u["TIP_CVE_TIPOUSUARIO"].ToString();
                }
                else
                {
                    Response.Write("<script>alert('Usuario no encontrado');</script>");
                }
            }
        }

        protected async void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TextBox6.Text))
                {
                    Response.Write("<script>alert('Ingresa una contraseña para el nuevo usuario');</script>");
                    return;
                }

                string rutaImagen = "";

                if (!FileUpload1.HasFile)
                {
                    Response.Write("<script>alert('Selecciona una imagen');</script>");
                    return;
                }

                string extension = Path.GetExtension(FileUpload1.FileName).ToLower();

                if (extension != ".jpg" && extension != ".png" && extension != ".gif")
                {
                    Response.Write("<script>alert('Solo jpg, png o gif');</script>");
                    return;
                }

                string nombre = Guid.NewGuid().ToString() + extension;

                string carpeta = Server.MapPath("~/imagenes/fotos/");

                // 🔥 asegura que la carpeta exista
                if (!Directory.Exists(carpeta))
                {
                    Directory.CreateDirectory(carpeta);
                }

                string rutaFisica = Path.Combine(carpeta, nombre);

                FileUpload1.SaveAs(rutaFisica);

                // 🔥 ruta que se guarda en BD
                rutaImagen = "imagenes/fotos/" + nombre;

                using (HttpClient client = new HttpClient())
                {
                    var obj = new
                    {
                        nombre = TextBox2.Text,
                        apellidoPaterno = TextBox3.Text,
                        apellidoMaterno = TextBox4.Text,
                        usuario = TextBox5.Text,
                        contrasena = TextBox6.Text,
                        ruta = rutaImagen,
                        tipo = int.Parse(DropDownList1.SelectedValue)
                    };

                    string json = JsonConvert.SerializeObject(obj);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(urlBase + "spinsusuario", content);
                    string resultado = await response.Content.ReadAsStringAsync();

                    clsApiStatus res = JsonConvert.DeserializeObject<clsApiStatus>(resultado);

                    if (res.ban == 0)
                    {
                        Response.Write("<script>alert('Usuario registrado correctamente');</script>");
                        Response.Redirect(Request.RawUrl);
                    }
                    else if (res.ban == 1)
                        Response.Write("<script>alert('Nombre ya registrado');</script>");
                    else if (res.ban == 2)
                        Response.Write("<script>alert('Usuario ya existe');</script>");
                    else if (res.ban == 3)
                        Response.Write("<script>alert('Tipo inválido');</script>");
                    else
                        Response.Write("<script>alert('Código desconocido: " + res.ban + "');</script>");
                }
            }
            catch (Exception ex)
            {
                // 🔥 ahora sí ves el error real
                Response.Write("<script>alert('Error real: " + ex.Message + "');</script>");
            }
        }

        protected async void Button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(TextBox1.Text))
                {
                    Response.Write("<script>alert('ID inválido');</script>");
                    return;
                }

                string rutaImagen = HiddenRuta.Value;
                string contrasena = TextBox6.Text;

                if (FileUpload1.HasFile)
                {
                    string extension = Path.GetExtension(FileUpload1.FileName).ToLower();

                    if (extension != ".jpg" && extension != ".png" && extension != ".gif")
                    {
                        Response.Write("<script>alert('Formato inválido');</script>");
                        return;
                    }

                    string nombre = Guid.NewGuid().ToString() + extension;
                    string rutaFisica = Server.MapPath("~/imagenes/fotos/") + nombre;

                    FileUpload1.SaveAs(rutaFisica);
                    rutaImagen = "imagenes/fotos/" + nombre;
                }

                using (HttpClient client = new HttpClient())
                {
                    var obj = new
                    {
                        id = int.Parse(TextBox1.Text),
                        nombre = TextBox2.Text,
                        apellidoPaterno = TextBox3.Text,
                        apellidoMaterno = TextBox4.Text,
                        usuario = TextBox5.Text,
                        contrasena = contrasena,
                        ruta = rutaImagen,
                        tipo = int.Parse(DropDownList1.SelectedValue)
                    };

                    string json = JsonConvert.SerializeObject(obj);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PutAsync(urlBase + "updateusuario", content);
                    string resultado = await response.Content.ReadAsStringAsync();

                    clsApiStatus res = JsonConvert.DeserializeObject<clsApiStatus>(resultado);

                    if (res.ban == 0)
                    {
                        Response.Write("<script>alert('Actualizado correctamente');</script>");
                        Response.Redirect(Request.RawUrl);
                    }
                    else if (res.ban == 1)
                        Response.Write("<script>alert('No existe usuario');</script>");
                    else if (res.ban == 3)
                        Response.Write("<script>alert('Usuario duplicado');</script>");
                    else if (res.ban == 4)
                        Response.Write("<script>alert('Tipo inválido');</script>");
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Error: " + ex.Message + "');</script>");
            }
        }

        protected async void Button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(TextBox1.Text))
                {
                    Response.Write("<script>alert('ID inválido');</script>");
                    return;
                }

                using (HttpClient client = new HttpClient())
                {
                    var response = await client.DeleteAsync(urlBase + "delusuario?id=" + TextBox1.Text);
                    string resultado = await response.Content.ReadAsStringAsync();

                    clsApiStatus res = JsonConvert.DeserializeObject<clsApiStatus>(resultado);

                    if (res.ban == 0)
                    {
                        Response.Write("<script>alert('Eliminado correctamente');</script>");

                        TextBox1.Text = "";
                        TextBox2.Text = "";
                        TextBox3.Text = "";
                        TextBox4.Text = "";
                        TextBox5.Text = "";
                        TextBox6.Text = "";
                        HiddenRuta.Value = "";
                    }
                    else
                    {
                        Response.Write("<script>alert('El usuario ya no existe');</script>");
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Error: " + ex.Message + "');</script>");
            }
        }
    }
}
