using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
namespace malshinon_09_06
{
    public class People
    {
        public int Id;
        public string FirstName;
        public string LastName;
        public string Secretode;
        public string Type;
        public int NumReports=0;
        public int NumMentions=0;
        public People(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }   
    }
}
