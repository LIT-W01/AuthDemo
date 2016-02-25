using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthDemo.Data;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var mgr = new UserManager(Properties.Settings.Default.ConStr);
            //var user = mgr.AddUser("avrumi", "litisterrible", "Avrumi Friedman");
            //Console.WriteLine(user.Id);

            Console.WriteLine("Enter user name");
            string username = Console.ReadLine();

            string password = "";
            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    break;
                }
                password += keyInfo.KeyChar;
                Console.Write("*");
            }
            Console.WriteLine();
            User user = mgr.GetUser(username, password);
            if (user == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR!!!");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("SUCCESS!!!");
            }


            Console.ReadKey(true);

        }
    }
}
