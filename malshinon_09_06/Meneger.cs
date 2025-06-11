using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace malshinon_09_06
{
    internal class Meneger
    {
        People people = new People();
        DAL dAL = new DAL();

        public string GetFirstNameOfReporter()
        {
            Console.WriteLine("enter your first name");
            string first_name = Console.ReadLine();
            Validation(first_name);
            return first_name;
        }
        public string GetLastNameOfReporter()
        {
            Console.WriteLine("enter your last name");
            string last_name = Console.ReadLine();
            Validation(last_name);
            return last_name;
        }
        public string GetFirstNameOfTarget()
        {
            Console.WriteLine("enter target first name");
            string first_name = Console.ReadLine();
            Validation(first_name);
            return first_name;
        }
        public string GetLastNameOfTarget()
        {
            Console.WriteLine("enter target last name");
            string last_name = Console.ReadLine();
            Validation(last_name);
            return last_name;
        }

        public string GetInformation()
        {
            Console.WriteLine("please enter the information");
            string infomation = Console.ReadLine();
            Validation(infomation);
            return infomation;
        }

        public string Validation(string text)
        {
            while (text == "")
            {
                Console.WriteLine("enter again");
                text = Console.ReadLine();
            }
            return text;
        }
        public void Starter()
        {
            string get_FirstNameOfReporter = GetFirstNameOfReporter();
            string get_FirstNameOfTarget = GetFirstNameOfTarget();

            bool reporter = people.CheckMalshin(get_FirstNameOfReporter);
            bool target = people.CheckTarget(get_FirstNameOfTarget);

            if(reporter&&target)
            {
                dAL.insertReports(get_FirstNameOfReporter, get_FirstNameOfTarget, GetInformation());
            }
            else if(reporter)
            {
                dAL.InsertPeople(get_FirstNameOfTarget, GetLastNameOfTarget());
                dAL.insertReports(get_FirstNameOfReporter, get_FirstNameOfTarget, GetInformation());
            }
            else if(target)
            {
                dAL.InsertPeople(get_FirstNameOfReporter, GetLastNameOfReporter());
                dAL.insertReports(get_FirstNameOfReporter, get_FirstNameOfTarget, GetInformation());
            }
            else
            {
                dAL.InsertPeople(get_FirstNameOfReporter, GetLastNameOfReporter());
                dAL.InsertPeople(get_FirstNameOfTarget, GetLastNameOfTarget());
                dAL.insertReports(get_FirstNameOfReporter, get_FirstNameOfTarget, GetInformation());
            }
        }
    }
}
