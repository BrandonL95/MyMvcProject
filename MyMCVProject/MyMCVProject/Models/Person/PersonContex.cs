using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.DependencyInjection;

namespace MyMCVProject.Models.Person
{
    public class PersonContex
    {
        public string ConnectionString { get; set; }

        public PersonContex(string Connection)
        {
            this.ConnectionString = Connection;
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        public List<PersonModel> GetAllPeople()
        {
            List<PersonModel> people = new List<PersonModel>();

                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT * FROM people", conn);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                          people.Add(new PersonModel()
                          {
                              FName = reader["Fname"].ToString(),
                              LName = reader["Lname"].ToString(),
                          });
                        }
                    }
                }

            return people;
        }

        public bool validateEmail(string email, string password)
        {
            bool valid = false;

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT email, password FROM people", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if(email == reader["email"].ToString() && password == reader["password"].ToString())
                        {
                            valid = true;
                            break;
                        }
                    }
                }
            }

            return valid;
        }

    }
}
