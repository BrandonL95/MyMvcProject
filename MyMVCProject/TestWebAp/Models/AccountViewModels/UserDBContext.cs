using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace TestWebAp.Models.AccountViewModels
{
    public class UserDBContext
    {
        public string ConnectionString { get; set; }
        public string email { get; set; }

        public UserDBContext() { }

        public UserDBContext(string Connection)
        {
            this.ConnectionString = Connection;
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        public List<UserDBContext> GetAllUsers()
        {
            List<UserDBContext> users = new List<UserDBContext>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT EMAIL FROM users", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new UserDBContext()
                        {
                            email = reader["Email"].ToString(),
                        });
                    }
                }


                return users;
            }
        }

    }
}
