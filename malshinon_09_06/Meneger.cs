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

        public string SetFirstNameOfReporter()
        {
            Console.WriteLine("enter your first name");
            string first_name = Console.ReadLine();
            Validation(first_name);
            return first_name;
        }
        public string SetLastNameOfReporter()
        {
            Console.WriteLine("enter your last name");
            string last_name = Console.ReadLine();
            Validation(last_name);
            return last_name;
        }
        public string SetFirstNameOfTarget()
        {
            Console.WriteLine("enter target first name");
            string first_name = Console.ReadLine();
            Validation(first_name);
            return first_name;
        }
        public string SetLastNameOfTarget()
        {
            Console.WriteLine("enter target last name");
            string last_name = Console.ReadLine();
            Validation(last_name);
            return last_name;
        }
        public string SetInformation()
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
            string get_FirstNameOfReporter = SetFirstNameOfReporter();
            string get_FirstNameOfTarget = SetFirstNameOfTarget();

            bool reporter = people.CheckPersonIfExsist(get_FirstNameOfReporter);
            bool target = people.CheckPersonIfExsist(get_FirstNameOfTarget);

            if(reporter&&target)
            {
                dAL.insertReports(get_FirstNameOfReporter, get_FirstNameOfTarget, SetInformation());
            }
            else if(reporter)
            {
                dAL.InsertPeople(get_FirstNameOfTarget, SetLastNameOfTarget(),"target");
                dAL.insertReports(get_FirstNameOfReporter, get_FirstNameOfTarget, SetInformation());
            }
            else if(target)
            {
                dAL.InsertPeople(get_FirstNameOfReporter, SetLastNameOfReporter(), "reporter");
                dAL.insertReports(get_FirstNameOfReporter, get_FirstNameOfTarget, SetInformation());
            }
            else
            {
                dAL.InsertPeople(get_FirstNameOfReporter, SetLastNameOfReporter(), "reporter" );
                dAL.InsertPeople(get_FirstNameOfTarget, SetLastNameOfTarget(), "target");
                dAL.insertReports(get_FirstNameOfReporter, get_FirstNameOfTarget, SetInformation());
            }
        }
    }
}
