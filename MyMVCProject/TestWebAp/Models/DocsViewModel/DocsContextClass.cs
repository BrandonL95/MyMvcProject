﻿using System;
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

        public bool AddPublicDocs(string UserID, string filename, string filePath, DateTime dateUploaded, double size)
        {
            bool success = false;

            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();

                    string sizeKB = (size / 1024).ToString("0.0") + " KB";

                    MySqlCommand cmd = new MySqlCommand("INSERT INTO publicdocs (OwnerID, FileName, FilePath, DateUploaded, FileSize) VALUES (@UserID, @filename, @filePath, @dateUploaded, @sizeKB);", conn);

                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    cmd.Parameters.AddWithValue("@filename", filename);
                    cmd.Parameters.AddWithValue("@filePath", filePath);
                    cmd.Parameters.AddWithValue("@dateUploaded", dateUploaded);
                    cmd.Parameters.AddWithValue("@sizeKB", sizeKB);

                    if (cmd.ExecuteNonQuery() == 1 && LogFile(UserID, UserID, filename, "Created"))
                        success = true;
                }
            }
            catch (Exception ex)
            {
                System.ArgumentException argEx = new System.ArgumentException("Document not inserted", "Public Document", ex);
                throw argEx;
            }

            return success;
        }

        public bool AddPrivateDocs(string UserID, string filename, string filePath, DateTime dateUploaded, double size)
        {
            bool success = false;

            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();

                    string sizeKB = (size / 1024).ToString("0.0") + " KB";

                    MySqlCommand cmd = new MySqlCommand("INSERT INTO privatedocs (OwnerID, FileName, FilePath, DateUploaded, FileSize) VALUES (@UserID , @filename, @filePath, @dateUploaded, @sizeKB);", conn);

                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    cmd.Parameters.AddWithValue("@filename", filename);
                    cmd.Parameters.AddWithValue("@filePath", filePath);
                    cmd.Parameters.AddWithValue("@dateUploaded", dateUploaded);
                    cmd.Parameters.AddWithValue("@sizeKB", sizeKB);

                    if (cmd.ExecuteNonQuery() == 1 && LogFile(UserID, UserID, filename, "Created"))
                        success = true;
                }
            }
            catch (Exception ex)
            {
                System.ArgumentException argEx = new System.ArgumentException("Document not inserted", "Private Document", ex);
                throw argEx;
            }

            return success;
        }

        public List<DocsClass> GetAllFiles()
        {
            List<DocsClass> Myfiles = new List<DocsClass>();
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT OwnerID, FileName, FilePath, FileSize FROM publicdocs", conn);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Myfiles.Add(new DocsClass()
                            {
                                Myfilenames = reader["FileName"].ToString(),
                                Email = GetEmail(reader["OwnerID"].ToString()),
                                OwnerID = reader["OwnerID"].ToString(),
                                docPath = reader["FilePath"].ToString(),
                                FileSize = reader["FileSize"].ToString()
                            });
                        }
                    }

                    return Myfiles;
                }
            }
            catch(Exception ex)
            {
                System.ArgumentException argEx = new System.ArgumentException("Empty List", "Get Public Files", ex);
                throw argEx;
            }
        }

        public List<DocsClass> GetAllPrivateFiles(string userID)
        {
            List<DocsClass> Myfiles = new List<DocsClass>();

            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT OwnerID, FileName, FilePath, FileSize FROM privatedocs Where OwnerID = @userID;", conn);

                    cmd.Parameters.AddWithValue("@userID", userID);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Myfiles.Add(new DocsClass()
                            {
                                Myfilenames = reader["FileName"].ToString(),
                                OwnerID = reader["OwnerID"].ToString(),
                                docPath = reader["FilePath"].ToString(),
                                FileSize = reader["FileSize"].ToString()

                            });
                        }
                    }

                    return Myfiles;
                }
            }
            catch(Exception ex)
            {
                System.ArgumentException argEx = new System.ArgumentException("Empty List", "Get Private Files", ex);
                throw argEx;
            }
        }

        public string GetEmail(string userID)
        {
            string email = "";

            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();

                    DocsClass myFile = new DocsClass();

                    MySqlCommand cmd = new MySqlCommand("SELECT Email FROM users WHERE Id = @userID;", conn);

                    cmd.Parameters.AddWithValue("@userID", userID);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            email = reader["Email"].ToString();
                        }
                    }
                }
                return email;
            }
            catch(Exception ex)
            {
                System.ArgumentException argEx = new System.ArgumentException("Invalid UserID", "Get Email", ex);
                throw argEx;
            }
        }     

        public string getPath(string email, string FileName)
        {
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    List<string> collaberators = new List<string>();
                    string MyPath = "";

                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT OwnerID, FileName FROM privatedocs", conn);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
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
            catch(Exception ex)
            {
                System.ArgumentException argEx = new System.ArgumentException("Invalid Path", "Path", ex);
                throw argEx;
            }
        }

        public void ReWriteColabFile(string ownerID, string filename, string userEmail)
        {
            List<string> Colabs = readColaberatorFile(ownerID, filename);

            try
            {
                StreamWriter sw = new StreamWriter("C:\\Users\\brand\\Desktop\\Userfiles\\" + GetEmail(ownerID) + "\\" + filename.Remove(filename.LastIndexOf("."), 1) + "Collaberators1.txt");

                foreach (string email in Colabs)
                {
                    if (email != userEmail)
                    {                       
                        sw.WriteLine(email);
                    }
                }
                sw.Close();

                File.Delete("C:\\Users\\brand\\Desktop\\Userfiles\\" + GetEmail(ownerID) + "\\" + filename.Remove(filename.LastIndexOf("."), 1) + "Collaberators.txt");
                File.Move("C:\\Users\\brand\\Desktop\\Userfiles\\" + GetEmail(ownerID) + "\\" + filename.Remove(filename.LastIndexOf("."), 1) + "Collaberators1.txt", "C:\\Users\\brand\\Desktop\\Userfiles\\" + GetEmail(ownerID) + "\\" + filename.Remove(filename.LastIndexOf("."), 1) + "Collaberators.txt");
            }
            catch (Exception ex)
            {
                System.ArgumentException argEx = new System.ArgumentException("File Not Written", "Colaberator File", ex);
                throw argEx;
            }

        }

        public void writeColaberatorFile(string ownerID, string email, string filename)
        {
            try
            {
                StreamWriter sw = new StreamWriter("C:\\Users\\brand\\Desktop\\Userfiles\\" + GetEmail(ownerID) + "\\" + filename.Remove(filename.LastIndexOf("."),1) + "Collaberators.txt", true);

                sw.WriteLine(email);

                sw.Close();
            }
            catch (Exception ex)
            {
                System.ArgumentException argEx = new System.ArgumentException("File Not Written", "Colaberator File", ex);
                throw argEx;
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
            catch (Exception ex)
            {
                System.ArgumentException argEx = new System.ArgumentException("File Not Read", "Colaberator File", ex);
                throw argEx;
            }

            return colaberators;
        }

        public List<DocsClass> GetColabView(string email, string fileName)
        {
            List<DocsClass> emails = new List<DocsClass>();

                using (MySqlConnection conn = GetConnection())
                {
                    List<string> collaberators = new List<string>();

                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT OwnerID, FileName FROM privatedocs Where FileName = @filename;", conn);

                    cmd.Parameters.AddWithValue("@filename", fileName);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string filename = reader["FileName"].ToString();
                            string ownerID = reader["OwnerID"].ToString();

                            collaberators = readColaberatorFile(ownerID, filename);
                        
                            for (int k = 0; k < collaberators.Count; k++)
                            {
                                    emails.Add(new DocsClass()
                                    {
                                        Email = collaberators[k],
                                    });
                            }
                        }
                    }

                    return emails;
                }
        }

        public List<DocsClass> GetSharedFiles(string email)
        {
            List<DocsClass> Myfiles = new List<DocsClass>();

            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    List<string> collaberators = new List<string>();

                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT OwnerID, FileName, FileSize FROM privatedocs", conn);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
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
                                        OwnerID = reader["OwnerID"].ToString(),
                                        Myfilenames = reader["FileName"].ToString(),
                                        Email = GetEmail(reader["OwnerID"].ToString()),
                                        FileSize = reader["FileSize"].ToString()
                                    });
                                }
                            }
                        }
                    }

                    return Myfiles;
                }
            }
            catch(Exception ex)
            {
                System.ArgumentException argEx = new System.ArgumentException("Empty List", "Shared Files", ex);
                throw argEx;
            }

        }

        public bool DeletePrivateFile(string OwnerID, string filename)
        {
            bool deleted = false;

            try
            {
                using (MySqlConnection Conn = GetConnection())
                {
                    Conn.Open();

                    MySqlCommand cmd = new MySqlCommand("DELETE FROM privatedocs WHERE OwnerID = @OwnerID AND FileName = @filename;", Conn);

                    cmd.Parameters.AddWithValue("@OwnerID", OwnerID);
                    cmd.Parameters.AddWithValue("@filename", filename);

                    if (cmd.ExecuteNonQuery() == 1 && LogFile(OwnerID, OwnerID, filename, "Deleted"))
                        deleted = true;
                }
            }
            catch(Exception ex)
            {
                System.ArgumentException argEx = new System.ArgumentException("File Not Deleted", "Private File", ex);
                throw argEx;
            }

            return deleted;
        }

        public bool DeletePublicFile(string OwnerID, string filename)
        {
            bool deleted = false;

            try
            {
                using (MySqlConnection Conn = GetConnection())
                {
                    Conn.Open();

                    MySqlCommand cmd = new MySqlCommand("DELETE FROM publicdocs WHERE OwnerID = @OwnerID AND FileName = @filename;", Conn);

                    cmd.Parameters.AddWithValue("@OwnerID", OwnerID);
                    cmd.Parameters.AddWithValue("@filename", filename);

                    if (cmd.ExecuteNonQuery() == 1 && LogFile(OwnerID, OwnerID, filename, "Deleted"))
                        deleted = true;
                }
            }
            catch(Exception ex)
            {
                System.ArgumentException argEx = new System.ArgumentException("File Not Deleted", "Public File", ex);
                throw argEx;
            }

            return deleted;
        }

        public bool updatePublicFile(string OwnerID, string currentUser, string oldfilename, string NewFileName, double fileSize)
        {
            bool updated = false;

            try
            {
                string sizeKB = (fileSize / 1024).ToString("0.0") + " KB";
                string OF = OwnerID + "  " + oldfilename;

                using (MySqlConnection Conn = GetConnection())
                {
                    Conn.Open();

                    MySqlCommand cmd = new MySqlCommand("UPDATE publicdocs SET FileName = @NewFileName, DateUploaded = @CurrentDate, FileSize = @sizeKB WHERE OwnerID = @OwnerID AND FileName = @oldfilename;", Conn);

                    cmd.Parameters.AddWithValue("@NewFileName", NewFileName);
                    cmd.Parameters.AddWithValue("@CurrentDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@sizeKB", sizeKB);
                    cmd.Parameters.AddWithValue("@OwnerID", OwnerID);
                    cmd.Parameters.AddWithValue("@oldfilename", oldfilename);

                    if (cmd.ExecuteNonQuery() == 1 && LogFile(OwnerID, currentUser, NewFileName, "Updated"))
                        updated = true;
                }
            }
            catch(Exception ex)
            {
                System.ArgumentException argEx = new System.ArgumentException("File Not Updated", "Public File", ex);
                throw argEx;
            }

            return updated;
        }

        public bool updatePrivateFile(string currentUser, string OwnerID, string oldfilename, string NewFileName, double fileSize)
        {
            bool updated = false;

            try
            {
                string sizeKB = (fileSize / 1024).ToString("0.0") + " KB";

                using (MySqlConnection Conn = GetConnection())
                {
                    Conn.Open();

                    MySqlCommand cmd = new MySqlCommand("UPDATE privatedocs SET FileName = @NewFileName, DateUploaded = @CurrentDate, FileSize = @sizeKB WHERE OwnerID = @OwnerID AND FileName = @oldfilename;", Conn);

                    cmd.Parameters.AddWithValue("@NewFileName", NewFileName);
                    cmd.Parameters.AddWithValue("@CurrentDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@sizeKB", sizeKB);
                    cmd.Parameters.AddWithValue("@OwnerID", OwnerID);
                    cmd.Parameters.AddWithValue("@oldfilename", oldfilename);

                    if (cmd.ExecuteNonQuery() == 1 && LogFile(OwnerID, currentUser, NewFileName, "Updated"))
                        updated = true;
                }
            }
            catch(Exception ex)
            {
                System.ArgumentException argEx = new System.ArgumentException("File Not Updated", "Private File", ex);
                throw argEx;
            }

            return updated;
        }

        public bool LogFile(string ownerID, string updatedBy, string filename, string action)
        {
            bool logWritten = false;

            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();

                    MySqlCommand cmd = new MySqlCommand("INSERT INTO logtable (ownerEmail, updatedBy, filename, action, dateOfAction) VALUES (@OwnerEmail, @Email, @filename, @action, @CurrentDate);", conn);

                    cmd.Parameters.AddWithValue("@OwnerEmail", GetEmail(ownerID));
                    cmd.Parameters.AddWithValue("@Email", GetEmail(updatedBy));
                    cmd.Parameters.AddWithValue("@filename", filename);
                    cmd.Parameters.AddWithValue("@action", action);
                    cmd.Parameters.AddWithValue("@CurrentDate", DateTime.Now.ToShortDateString());

                    if (cmd.ExecuteNonQuery() == 1)
                        logWritten = true;
                }
            }
            catch (Exception ex)
            {
                System.ArgumentException argEx = new System.ArgumentException("LogFile Not Inserted", "LogFile", ex);
                throw argEx;
            }

            return logWritten;
        }
    }
}
