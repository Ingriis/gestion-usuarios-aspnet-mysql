using System;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;

namespace apiRestCtrlUsuariosFull.Models
{
    public class clsUsuario
    {
        public int id { get; set; }
        public string cve { get; set; }
        public string nombre { get; set; }
        public string apellidoPaterno { get; set; }
        public string apellidoMaterno { get; set; }
        public string usuario { get; set; }
        public string contrasena { get; set; }
        public string ruta { get; set; }
        public int tipo { get; set; }

        private string cadConn = ConfigurationManager
            .ConnectionStrings["bdControlAcceso"]
            .ConnectionString;


        public clsUsuario() { }

        public clsUsuario(string usuario, string contrasena)
        {
            this.usuario = usuario;
            this.contrasena = contrasena;
        }


        public clsUsuario(string nombre,
                          string apellidoPaterno,
                          string apellidoMaterno,
                          string usuario,
                          string contrasena,
                          string ruta,
                          int tipo)
        {
            this.nombre = nombre;
            this.apellidoPaterno = apellidoPaterno;
            this.apellidoMaterno = apellidoMaterno;
            this.usuario = usuario;
            this.contrasena = contrasena;
            this.ruta = ruta;
            this.tipo = tipo;
        }

        public DataSet spInsUsuario()
        {
            using (MySqlConnection cnn = new MySqlConnection(cadConn))
            using (MySqlCommand cmd = new MySqlCommand("spInsUsuario", cnn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("nombre", this.nombre);
                cmd.Parameters.AddWithValue("paterno", this.apellidoPaterno);
                cmd.Parameters.AddWithValue("materno", this.apellidoMaterno);
                cmd.Parameters.AddWithValue("usuario", this.usuario);
                cmd.Parameters.AddWithValue("contrasena", PasswordHasher.HashPassword(this.contrasena));
                cmd.Parameters.AddWithValue("ruta", this.ruta);
                cmd.Parameters.AddWithValue("tipo", this.tipo);

                DataSet ds = new DataSet();
                using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                    da.Fill(ds, "spInsUsuario");
                return ds;
            }
        }

        public DataSet spValidarAcceso()
        {
            using (MySqlConnection cnn = new MySqlConnection(cadConn))
            using (MySqlCommand cmd = new MySqlCommand("spValidarAcceso", cnn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("usuario", this.usuario);

                DataSet ds = new DataSet();
                using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                    da.Fill(ds, "spValidarAcceso");
                return ds;
            }
        }

        public bool updatePasswordHash(string usuario, string hash)
        {
            using (MySqlConnection cnn = new MySqlConnection(cadConn))
            using (MySqlCommand cmd = new MySqlCommand(
                "UPDATE usuario SET USU_CONTRASENA = @hash WHERE USU_USUARIO = @usuario", cnn))
            {
                cmd.Parameters.AddWithValue("@hash", hash);
                cmd.Parameters.AddWithValue("@usuario", usuario);
                cnn.Open();
                return cmd.ExecuteNonQuery() == 1;
            }
        }

        public DataSet vwRptUsuario()
        {
            bool filtrar = !string.IsNullOrEmpty(this.nombre);
            string cadSQL = filtrar
                ? "SELECT * FROM vwRptUsuario WHERE (Nombre LIKE @filtro OR Usuario LIKE @filtro OR Rol LIKE @filtro)"
                : "SELECT * FROM vwRptUsuario";

            using (MySqlConnection cnn = new MySqlConnection(cadConn))
            using (MySqlCommand cmd = new MySqlCommand(cadSQL, cnn))
            {
                if (filtrar)
                    cmd.Parameters.Add("@filtro", MySqlDbType.VarChar).Value = "%" + this.nombre + "%";

                DataSet ds = new DataSet();
                using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                    da.Fill(ds, "vwRptUsuario");
                return ds;
            }
        }

        public DataSet vwTipoUsuario(string filtro)
        {
            bool filtrar = !string.IsNullOrEmpty(filtro);
            string cadSQL = filtrar
                ? "SELECT * FROM vwTipoUsuario WHERE descripcion LIKE @filtro"
                : "SELECT * FROM vwTipoUsuario";

            using (MySqlConnection cnn = new MySqlConnection(cadConn))
            using (MySqlCommand cmd = new MySqlCommand(cadSQL, cnn))
            {
                if (filtrar)
                    cmd.Parameters.Add("@filtro", MySqlDbType.VarChar).Value = "%" + filtro + "%";

                DataSet ds = new DataSet();
                using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                    da.Fill(ds, "vwTipoUsuario");
                return ds;
            }
        }

        public DataSet getUsuario()
        {
            using (MySqlConnection cnn = new MySqlConnection(cadConn))
            using (MySqlCommand cmd = new MySqlCommand(
                "SELECT USU_CVE_USUARIO, USU_NOMBRE, USU_APELLIDO_PATERNO, USU_APELLIDO_MATERNO, " +
                "USU_USUARIO, USU_RUTA, TIP_CVE_TIPOUSUARIO FROM usuario WHERE USU_CVE_USUARIO = @id", cnn))
            {
                cmd.Parameters.AddWithValue("@id", this.id);
                DataSet ds = new DataSet();
                using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                    da.Fill(ds, "getUsuario");
                return ds;
            }
        }

        public DataSet spUpdUsuario()
        {
            using (MySqlConnection cnn = new MySqlConnection(cadConn))
            using (MySqlCommand cmd = new MySqlCommand("spUpdUsuario", cnn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("clave", this.id);
                cmd.Parameters.AddWithValue("nombre", this.nombre);
                cmd.Parameters.AddWithValue("paterno", this.apellidoPaterno);
                cmd.Parameters.AddWithValue("materno", this.apellidoMaterno);
                cmd.Parameters.AddWithValue("usuario", this.usuario);
                cmd.Parameters.AddWithValue("pwd", string.IsNullOrWhiteSpace(this.contrasena)
                    ? (object)DBNull.Value
                    : PasswordHasher.HashPassword(this.contrasena));
                cmd.Parameters.AddWithValue("ruta", this.ruta);
                cmd.Parameters.AddWithValue("tipo", this.tipo);

                DataSet ds = new DataSet();
                using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                    da.Fill(ds, "spUpdUsuario");
                return ds;
            }
        }


        public DataSet spDelUsuario()
        {
            using (MySqlConnection cnn = new MySqlConnection(cadConn))
            using (MySqlCommand cmd = new MySqlCommand("spDelUsuario", cnn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("clave", MySqlDbType.Int32).Value = this.id;

                DataSet ds = new DataSet();
                using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                    da.Fill(ds, "spDelUsuario");
                return ds;
            }
        }
    }
}