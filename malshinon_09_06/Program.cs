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
            //Meneger meneger = new Meneger();
            //meneger.Starter();
            DAL dAL = new DAL();
            dAL.GetNumReports("szs");
            //dAL.UpdateToPotentialAgent("szs");
            //Console.WriteLine(dAL.GetType("aa"));
            //Console.WriteLine(dAL.GetAverege("szs")); 

        }
    }
}

