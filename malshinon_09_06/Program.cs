using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace malshinon_09_06
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DAL dalPeople = new DAL();
            dalPeople.InsertPeople("david","m");
            Console.WriteLine(dalPeople.GetNumReports()); 
            
            
        }
    }
}

