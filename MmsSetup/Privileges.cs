using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MmsSetup
{
    public class Privileges
    {

        public bool checkFolder()
        {

            // Specify the directory you want to manipulate.
            string path = @"d:\temp";
            try
            {
                // Determine whether the directory exists.
                var pathValue = Directory.Exists(path.ToString());
                if (pathValue)
                {
                    Console.WriteLine("That path exists already. {0}, {1}", path, pathValue);
                    return true;
                }
                Console.WriteLine("Creating the directory {0} and confirm [y/n]", path);
                var inputString = Console.ReadLine();
                if (inputString != null)
                {
                    if (inputString.ToLower() == "y")
                    {
                        // Try to create the directory.
                        var di = Directory.CreateDirectory(path);
                        Console.WriteLine("The directory was created successfully at {0}.", Directory.GetCreationTime(path));
                        return true;
                    }
                }
                else
                {
                    return false;
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
                return false;
            }
            finally { }

            return false;
        }


        public bool BuildSolution()
        {
            Console.ReadLine();
            Console.WriteLine("You need to install MSBuild at \"C:\\Program Files (x86)\\MSBuild\\12.0\\Bin\\MSBuild.exe\"; confirm if that's done:[Y/N]");
            try
            {
                var inputString = Console.ReadLine();
                if (inputString != null)
                {
                    if (inputString.ToLower() != "y")
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }

                var mmsSoluPath = @"C:\Projects\ey\Code\Rmi\EY\Mms\Mms.sln"; //(contains WebApi, Services, etc)
                var command = "\"C:\\Program Files (x86)\\MSBuild\\12.0\\Bin\\MSBuild.exe\" " + mmsSoluPath + "";
                Console.WriteLine("Executing Command: {0}", command);
                var procStartInfo = new ProcessStartInfo("cmd", "/c " + command + " 1> c:\\MMS_Build.txt");
                var proc = new Process();
                proc.StartInfo = procStartInfo;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                proc.Start();
                while (!proc.HasExited)
                {
                    Thread.Sleep(2000);
                    Console.WriteLine("Build is in progress. Wait....");
                }
                var res = File.OpenText("c:\\MMS_Build.txt");
                while (!res.EndOfStream)
                {
                    string line = res.ReadLine();
                    if (line != null)
                    {
                        if (Convert.ToString(line).Contains("Build succeeded"))
                        {
                            Console.WriteLine("Build succeeded...");
                            return true;
                        }
                    }
                }
                Console.WriteLine("Build failed ...");
            }
            catch (Exception e)
            {
                Console.WriteLine("Build failed due to Exception:{0}", e.ToString());
            }
            return false;
        }

        

        public bool CheckExists(string value)
        {
            var bvalue = value == "Y" ? true : false;
            return bvalue;
        }




    }
}
