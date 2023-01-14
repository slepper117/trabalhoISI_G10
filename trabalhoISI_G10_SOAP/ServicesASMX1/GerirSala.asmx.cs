using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Microsoft.SqlServer.Server;
using System.ComponentModel;
using System.Reflection;
using System.ComponentModel;

namespace ExemploSoa.Services
{
    /// <summary>
    /// Serviço para gerir reservas de salas 
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que esse serviço da web seja chamado a partir do script, usando ASP.NET AJAX, remova os comentários da linha a seguir. 
    // [System.Web.Script.Services.ScriptService]

    public class GerirSala: System.Web.Services.WebService
    {

        
        [WebMethod]
        public DataTable Get()
        {
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using(SqlCommand cmd = new SqlCommand("select *from Users"))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter())
                    {
                        cmd.Connection = con;
                        da.SelectCommand = cmd;
                        using (DataTable dt = new DataTable())
                        {
                            dt.TableName = "Users";
                            //if(dt.Rows.Count > 0)
                            da.Fill(dt);
                            return dt;
                        }
                    }
                }
            }
            //DataTable dt = new DataTable();
        }
        [WebMethod]
        public string Put(int id, string username, string password, string tag, string schedule, string status)
        {
            
           // string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            string result = "";
            // string con = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
            SqlCommand cmd = new SqlCommand();

            string Query = "UPDATE Users SET username = @username, password = @password, tag = @tag, schedule = @schedule, status = @status where id =@id";
            cmd = new SqlCommand(Query, con);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@password", password);
            cmd.Parameters.AddWithValue("@tag", tag);
            cmd.Parameters.AddWithValue("@schedule", schedule);
            cmd.Parameters.AddWithValue("@status", status);
           
            //DataObjectFieldAttribute valores; if(valores.primaryKey == true)

            if (id > 0)
            {
                try
                {
                    con.Open();
                }
                catch (Exception)
                {
                    throw;
                }
            }
            cmd.ExecuteNonQuery();
            result = " Update sucessfully!";
            con.Close();
            return result;

        }
        [WebMethod]
        public string Post(int id, string username, string password, string tag, string schedule, string status)
        {
            string result = "";

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
            SqlCommand cmd = new SqlCommand();

            string Query = " insert into Users values('" + id + "', '" + username + "','" + password + "', '" + tag + "', '" + schedule + "', '" + status + "')";
            cmd = new SqlCommand(Query, con);
            con.Open();
            cmd.ExecuteNonQuery();
            result = "Insert Successfully!";
            try
            {
                con.Close();
            }
            catch (Exception)
            {

                throw;
            }
         
            return result;
        }

        [WebMethod]
        public string Delete(int id)
        {
            //constr
            string result = "";
            SqlConnection con;
            con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
            SqlCommand cmd = new SqlCommand();
            
            string Query = " Delete from Users where id ='"+id+"'";
            cmd = new SqlCommand(Query, con);
            
            con.Open();
            cmd.ExecuteNonQuery();  
            result = "Deleted Successfully!";
            try
            {
                con.Close();
            }
            catch (Exception)
            {

                throw;
            }
            return result;
        }
    }
}
