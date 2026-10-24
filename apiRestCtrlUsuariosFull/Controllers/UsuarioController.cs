using apiRestCtrlUsuariosFull.Models;
using Newtonsoft.Json.Linq;
using System.Data;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace apiRestCtrlUsuariosFull.Controllers
{
    public class UsuarioController : ApiController
    {
        [HttpPost]
        [Route("full/usuario/spinsusuario")]
        public clsApiStatus spInsUsuario([FromBody] clsUsuario modelo)
        {
            clsApiStatus objRespuesta = new clsApiStatus();
            JObject jsonResp = new JObject();

            try
            {
                clsUsuario objUsuario = new clsUsuario(
                    modelo.nombre,
                    modelo.apellidoPaterno,
                    modelo.apellidoMaterno,
                    modelo.usuario,
                    modelo.contrasena,
                    modelo.ruta,
                    modelo.tipo
                );

                DataSet ds = objUsuario.spInsUsuario();

                objRespuesta.statusExec = true;
                objRespuesta.msg = "Usuario registrado exitosamente !";
                objRespuesta.ban = int.Parse(ds.Tables[0].Rows[0][0].ToString());

                jsonResp.Add("msgData", "Usuario registrado exitosamente !");
                objRespuesta.datos = jsonResp;
            }
            catch (Exception e)
            {
                objRespuesta.statusExec = false;
                objRespuesta.msg = "Usuario NO registrado ...";
                objRespuesta.ban = -1;
                jsonResp.Add("msgData", e.Message.ToString());
                objRespuesta.datos = jsonResp;
            }

            return objRespuesta;
        }

        [HttpPost]
        [Route("full/usuario/spvalidaracceso")]
        public clsApiStatus spValidarAcceso([FromBody] clsUsuario modelo)
        {
            clsApiStatus objRespuesta = new clsApiStatus();
            JObject jsonResp = new JObject();
            DataSet ds = new DataSet();

            try
            {
                clsUsuario objUsuario = new clsUsuario(
                    modelo.usuario,
                    modelo.contrasena
                );

                ds = objUsuario.spValidarAcceso();

                DataRow row = ds.Tables[0].Rows.Count > 0 ? ds.Tables[0].Rows[0] : null;
                bool accesoValido = false;
                bool necesitaActualizacion = false;
                string hashGuardado = row != null && row.Table.Columns.Count > 5
                    ? row[5].ToString()
                    : null;

                if (row != null && int.Parse(row[0].ToString()) == 1)
                    accesoValido = PasswordHasher.Verify(modelo.contrasena, hashGuardado, out necesitaActualizacion);

                if (accesoValido && necesitaActualizacion)
                {
                    string nuevoHash = PasswordHasher.HashPassword(modelo.contrasena);
                    if (!objUsuario.updatePasswordHash(modelo.usuario, nuevoHash))
                        throw new InvalidOperationException("No se pudo actualizar el almacenamiento de la contraseña.");
                }

                objRespuesta.statusExec = true;
                objRespuesta.ban = accesoValido ? 1 : 0;

                if (accesoValido)
                {
                    objRespuesta.msg = "Usuario validado exitosamente!";
                    jsonResp.Add("usu_nombre_completo", row[1].ToString());
                    jsonResp.Add("usu_ruta", row[2].ToString());
                    jsonResp.Add("usu_usuario", row[3].ToString());
                    jsonResp.Add("tip_descripcion", row[4].ToString());
                }
                else
                {
                    objRespuesta.msg = "Acceso denegado, verificar ...";
                    jsonResp.Add("msgData", "Acceso denegado, verificar ...");
                }

                objRespuesta.datos = jsonResp;
            }
            catch (Exception ex)
            {
                objRespuesta.statusExec = false;
                objRespuesta.ban = -1;
                objRespuesta.msg = "Error de conexión con el servicio de datos";
                jsonResp.Add("msgData", ex.Message.ToString());
                objRespuesta.datos = jsonResp;
            }

            return objRespuesta;
        }

        [HttpGet]
        [Route("full/usuario/vwrptusuario")]
        public clsApiStatus vwRptUsuario(string nombre = "", string usuario = "")
        {
            clsApiStatus objRespuesta = new clsApiStatus();
            JObject jsonResp = new JObject();
            DataSet ds = new DataSet();

            try
            {
                clsUsuario objUsuario = new clsUsuario();

                objUsuario.nombre = nombre;
                objUsuario.usuario = usuario;

                ds = objUsuario.vwRptUsuario();

                objRespuesta.statusExec = true;
                objRespuesta.ban = ds.Tables[0].Rows.Count;
                objRespuesta.msg = "Consulta de usuario realizada exitosamente";

                string jsonString = JsonConvert.SerializeObject(ds.Tables[0], Formatting.Indented);
                jsonResp = JObject.Parse($"{{\"{ds.Tables[0].TableName}\": {jsonString}}}");

                objRespuesta.datos = jsonResp;
            }
            catch (Exception ex)
            {
                objRespuesta.statusExec = false;
                objRespuesta.ban = -1;
                objRespuesta.msg = ex.Message;
                jsonResp.Add("msgData", ex.ToString());
                objRespuesta.datos = jsonResp;
            }

            return objRespuesta;
        }

        [HttpGet]
        [Route("full/usuario/getusuario")]
        public clsApiStatus getUsuario(int id)
        {
            clsApiStatus objRespuesta = new clsApiStatus();
            JObject jsonResp = new JObject();
            DataSet ds = new DataSet();

            try
            {
                clsUsuario objUsuario = new clsUsuario();
                objUsuario.id = id;

                ds = objUsuario.getUsuario();

                objRespuesta.statusExec = true;
                objRespuesta.ban = ds.Tables[0].Rows.Count;
                objRespuesta.msg = "Usuario encontrado";

                string jsonString = JsonConvert.SerializeObject(ds.Tables[0], Formatting.Indented);
                jsonResp = JObject.Parse($"{{\"{ds.Tables[0].TableName}\": {jsonString}}}");

                objRespuesta.datos = jsonResp;
            }
            catch (Exception ex)
            {
                objRespuesta.statusExec = false;
                objRespuesta.msg = ex.Message;
                objRespuesta.ban = -1;
            }

            return objRespuesta;
        }

        [HttpPut]
        [Route("full/usuario/updateusuario")]
        public clsApiStatus updateUsuario([FromBody] clsUsuario modelo)
        {
            clsApiStatus objRespuesta = new clsApiStatus();

            try
            {
                clsUsuario objUsuario = new clsUsuario
                {
                    id = modelo.id,
                    nombre = modelo.nombre,
                    apellidoPaterno = modelo.apellidoPaterno,
                    apellidoMaterno = modelo.apellidoMaterno,
                    usuario = modelo.usuario,
                    contrasena = modelo.contrasena,
                    ruta = modelo.ruta,
                    tipo = modelo.tipo
                };

                DataSet ds = objUsuario.spUpdUsuario();

                int ban = int.Parse(ds.Tables[0].Rows[0][0].ToString());

                objRespuesta.ban = ban;
                objRespuesta.statusExec = true;
                objRespuesta.msg = "Usuario actualizado correctamente";
            }
            catch (Exception ex)
            {
                objRespuesta.statusExec = false;
                objRespuesta.msg = ex.Message;
            }

            return objRespuesta;
        }

        [HttpDelete]
        [Route("full/usuario/delusuario")]
        public clsApiStatus delUsuario(int id)
        {
            clsApiStatus objRespuesta = new clsApiStatus();

            try
            {
                clsUsuario objUsuario = new clsUsuario();
                objUsuario.id = id;

                DataSet ds = objUsuario.spDelUsuario();

                int ban = int.Parse(ds.Tables[0].Rows[0][0].ToString());

                objRespuesta.ban = ban;
                objRespuesta.statusExec = true;

                

                if (ban == 0)
                    objRespuesta.msg = "Usuario eliminado correctamente";
                else
                    objRespuesta.msg = "Usuario no existe";
            }
            catch (Exception ex)
            {
                objRespuesta.statusExec = false;
                objRespuesta.msg = ex.Message;
            }

            return objRespuesta;
        }

        [HttpGet]
        [Route("full/usuario/vwTipoUsuario")]
        public clsApiStatus vwTipoUsuario(string filtro = "")
        {
            clsApiStatus objRespuesta = new clsApiStatus();
            JObject jsonResp = new JObject();
            DataSet ds = new DataSet();

            try
            {
                clsUsuario objUsuario = new clsUsuario();

                ds = objUsuario.vwTipoUsuario(filtro);

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    objRespuesta.statusExec = true;
                    objRespuesta.ban = ds.Tables[0].Rows.Count;
                    objRespuesta.msg = "Consulta de tipo usuario realizada exitosamente";

                    string jsonString = JsonConvert.SerializeObject(ds.Tables[0], Formatting.Indented);

                    jsonResp["data"] = JArray.Parse(jsonString);

                    objRespuesta.datos = jsonResp;
                }
                else
                {
                    objRespuesta.statusExec = false;
                    objRespuesta.ban = 0;
                    objRespuesta.msg = "No se encontraron registros";

                    jsonResp.Add("msgData", "Sin resultados para el filtro aplicado");
                    objRespuesta.datos = jsonResp;
                }
            }
            catch (Exception ex)
            {
                objRespuesta.statusExec = false;
                objRespuesta.ban = -1;
                objRespuesta.msg = "Fallo en consulta de tipo usuario";

                jsonResp.Add("msgData", ex.Message);
                objRespuesta.datos = jsonResp;
            }

            return objRespuesta;
        }
    }
}
