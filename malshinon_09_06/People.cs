using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
namespace malshinon_09_06
{
    internal class People
    {
        public int Id;
        public string FirstName;
        public string LastName;
        public string Secretode;
        public string Type;
        public int NumReports=0;
        public int NumMentions=0;

        DAL dAL = new DAL();
        public bool CheckPersonIfExsist(string firstName)
        {
            foreach (var name in dAL.GetAllNames())
            {
                if (name == firstName)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
