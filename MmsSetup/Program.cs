using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MmsSetup
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //var checkFile = new Privileges();
                //var isFileExists = checkFile.checkFolder();

                //if (isFileExists)
                //{
                //    if (checkFile.BuildSolution())
                //    {
                //        Setup.CreateMmsSite();
                //        Setup.CreateWebApiService();
                //        Setup.CreateMmsServiceManager();

                //    }
                //}
            }
            catch (Exception)
            {
                Console.WriteLine("Exception stopped the installation");
            }


            Console.ReadLine();
        }
    }
}
