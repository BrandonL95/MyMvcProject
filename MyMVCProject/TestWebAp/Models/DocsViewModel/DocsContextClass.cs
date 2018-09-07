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

        public bool AddPublicDocs(string UserID, string filename, string filePath)
        {
            bool success = false;

            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();

                    MySqlCommand cmd = new MySqlCommand("INSERT INTO publicdocs (UserID, FileName, FilePath) VALUES (" + "'" + UserID + "','" + filename + "','" + filePath + "');", conn);

                    if (cmd.ExecuteNonQuery() == 1)
                        success = true;
                    
                }
            }
            catch (Exception ex)
            {
                return false;
            }

           return success;
        }

        public bool AddPrivateDocs(string UserID, string filename, string filePath)
        {
            bool success = false;

            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();

                    MySqlCommand cmd = new MySqlCommand("INSERT INTO privatedocs (UserID, FileName, FilePath) VALUES (" + "'" + UserID + "','" + filename + "','" + filePath + "');", conn);

                    if (cmd.ExecuteNonQuery() == 1)
                        success = true;

                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return success;
        }

        public List<DocsClass> GetAllFiles()
        {
            List<DocsClass> Myfiles = new List<DocsClass>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT FileName FROM publicdocs", conn);

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
