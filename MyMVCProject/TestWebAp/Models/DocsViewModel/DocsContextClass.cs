using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace TestWebAp.Models.DocsViewModel
{
    public class DocsContextClass
    {
        public string ConnectionString { get; set; }

        public DocsContextClass(string Connection)
        {
            this.ConnectionString = Connection;
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        public void AddDocs(string UserID, string filename, string filePath)
        {
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();

                    MySqlCommand cmd = new MySqlCommand("INSERT INTO documents (UserID, FileName, FilePath) VALUES (" + "'" + UserID + "','" + filename + "','" + filePath + "');", conn);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
        }

        public List<DocsClass> GetAllFiles()
        {
            List<DocsClass> Myfiles = new List<DocsClass>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT FileName FROM documents", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Myfiles.Add(new DocsClass()
                        {
                            Myfilenames = reader["FileName"].ToString(),
                        });
                    }
                }

                return Myfiles;
            }
        }
    }
}
