using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using TestWebAp.Controllers;

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

        public bool AddPublicDocs(string UserID, string filename, string filePath, string md5, DateTime dateUploaded, double size)
        {
            bool success = false;

            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();

                    string sizeMB = (size / 1024).ToString("0.0") + " KB";

                    MySqlCommand cmd = new MySqlCommand("INSERT INTO publicdocs (OwnerID, FileName, FilePath, md5Checksum, DateUploaded, fileSize) VALUES (" + "'" + UserID + "','" + filename + "','" + filePath + "','" + md5 + "','" + dateUploaded + "','" + sizeMB + "');", conn);

                    if (cmd.ExecuteNonQuery() == 1)
                        success = true;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return success;
        }

        public bool AddPrivateDocs(string UserID, string filename, string filePath, string md5, DateTime dateUploaded, double size)
        {
            bool success = false;

            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();

                    string sizeMB = (size / 1024).ToString("0.0") + " KB";

                    MySqlCommand cmd = new MySqlCommand("INSERT INTO privatedocs (OwnerID, FileName, FilePath, md5Checksum, DateUploaded, fileSize) VALUES (" + "'" + UserID + "','" + filename + "','" + filePath + "','" + md5 + "','" + dateUploaded + "','" + sizeMB + "');", conn);

                    if (cmd.ExecuteNonQuery() == 1)
                        success = true;
                }
            }
            catch (Exception)
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

        public List<DocsClass> GetAllPrivateFiles(string userID)
        {
            List<DocsClass> Myfiles = new List<DocsClass>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT FileName FROM privatedocs Where OwnerID = '" + userID + "';", conn);

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

        public string GetEmail(string userID)
        {
            string email = "";

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();

                DocsClass myFile = new DocsClass();

                MySqlCommand cmd = new MySqlCommand("SELECT Email FROM users WHERE Id ='" + userID + "';", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        email = reader["Email"].ToString();
                    }
                }
            }
            return email;
        }

        public bool getIDandFilename(string UserID, string Filename)
        {
            bool ifExists = false;
            string id = "", filename = "";

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();

                DocsClass myFile = new DocsClass();

                MySqlCommand cmd = new MySqlCommand("SELECT OwnerID, FileName FROM publicdocs", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (!(UserID == id) && !(Filename == filename))
                        {
                            id = reader["id"].ToString();
                            filename = reader["FileName"].ToString();
                        }
                        else
                        {
                            ifExists = true;
                            break;
                        }
                    }
                }
            }
            return ifExists;
        }

        public string getMd5Hash(string userID, string filename)
        {
            string md5Hash = "";

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();

                DocsClass myFile = new DocsClass();

                MySqlCommand cmd = new MySqlCommand("SELECT md5Checksum FROM publicdocs WHERE OwnerID ='" + userID + "', FileName = '" + filename + "';", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        md5Hash = reader["md5Checksum"].ToString();
                    }
                }

                return md5Hash;
            }
        }

        public string getPath(string email, string FileName)
        {
            using (MySqlConnection conn = GetConnection())
            {
                List<string> collaberators = new List<string>();
                string MyPath = "";

                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT OwnerID, FileName FROM privatedocs", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string filename = reader["FileName"].ToString();
                        string ownerID = reader["OwnerID"].ToString();

                        collaberators = readColaberatorFile(ownerID, filename);
                        int c = collaberators.Count;
                        for (int k = 0; k < collaberators.Count; k++)
                        {
                            if (collaberators[k].ToString() == email && FileName == filename)
                            {
                                MyPath = "C:\\Users\\brand\\Desktop\\Userfiles\\" + GetEmail(ownerID) + "\\" + FileName;
                                break;
                            }
                        }
                    }
                }
                return MyPath;
            }
        }

        public void writeColaberatorFile(string ownerID, string userID, string filename)
        {
            try
            {
                StreamWriter sw = new StreamWriter("C:\\Users\\brand\\Desktop\\Userfiles\\" + GetEmail(ownerID) + "\\" + filename.Remove(filename.LastIndexOf("."),1) + "Collaberators.txt", true);

                sw.WriteLine(userID);

                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }

        public List<string> readColaberatorFile(string owner, string filename)
        {
            List<string> colaberators = new List<string>();

            try
            {
                using (StreamReader reader = new StreamReader("C:\\Users\\brand\\Desktop\\Userfiles\\" + GetEmail(owner) + "\\" + filename.Remove(filename.LastIndexOf("."), 1) + "Collaberators.txt"))
                {
                    string Line = reader.ReadLine();
                    while (Line != null)
                    {
                        colaberators.Add(Line);
                        Line = reader.ReadLine();
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }

            return colaberators;
        }

        public List<DocsClass> GetSharedFiles(string email)
        {
            List<DocsClass> Myfiles = new List<DocsClass>();

            using (MySqlConnection conn = GetConnection())
            {
                List<string> collaberators = new List<string>();

                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT OwnerID, FileName FROM privatedocs", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string filename = reader["FileName"].ToString();
                        string ownerID = reader["OwnerID"].ToString();

                        collaberators = readColaberatorFile(ownerID, filename);
                        int c = collaberators.Count;
                        for (int k = 0; k < collaberators.Count; k++)
                        {
                            if (collaberators[k].ToString() == email)
                            {
                                Myfiles.Add(new DocsClass()
                                {
                                    Myfilenames = reader["FileName"].ToString(),
                                });
                            }
                        }
                    }
                }

                return Myfiles;
            }

        }
    }
}
