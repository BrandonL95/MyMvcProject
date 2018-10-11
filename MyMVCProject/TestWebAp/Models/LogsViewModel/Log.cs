﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestWebAp.Models.DocsViewModel;

namespace TestWebAp.Models.LogsViewModel
{
    public class Log
    {
        public string OwnerEmail { get; set; }

        public string UpdatedBy { get; set; }

        public string FileName { get; set; }

        public string action { get; set; }

        public string date { get; set; }

        public string ConnectionString { get; set; }


        public Log() { }

        public Log(string Connection)
        {
            this.ConnectionString = Connection;
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        public List<Log> GetLog(string OwnerID, string FileName)
        {
            List<Log> Mylog = new List<Log>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT ownerEmail, updatedBy, filename, action, dateOfAction FROM logtable WHERE ownerEmail = '" + GetEmail(OwnerID) + "' AND filename = '" + FileName + "';", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Mylog.Add(new Log()
                        {
                            OwnerEmail = reader["ownerEmail"].ToString(),
                            UpdatedBy = reader["updatedBy"].ToString(),
                            FileName = reader["filename"].ToString(),
                            action = reader["action"].ToString(),
                            date = reader["dateOfAction"].ToString()
                        });
                    }
                }

                return Mylog;
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
    }
}
