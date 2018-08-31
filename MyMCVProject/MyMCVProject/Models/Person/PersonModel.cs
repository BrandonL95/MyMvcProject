using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace MyMCVProject.Models.Person
{
    public class PersonModel
    {
        public string FName { get; set; }
        public string LName { get; set; }
        public string Email { get; set; }

        public PersonModel()
        { }

        public PersonModel(string FName, string LName)
        {
            this.FName = FName;
            this.LName = LName;
        }
    }
}
