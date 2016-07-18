using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Web.Administration;


namespace MmsSetup
{
    public static class Setup
    {
        public static void CreateMmsSite()
        {
            try
            {
                var command = "";
                var source = @"C:\projects\ey\Code\Rmi\EY\Mms\Website\";
                var destination = @"C:\inetpub\MMS\";

                if (!Directory.Exists(destination))
                {
                    command = "mkdir " + destination;
                    ExecuteCommand(command);
                    //Directory.CreateDirectory(destination);
                }

                //command = "%systemroot%\\system32\\inetsrv\\APPCMD add site /name:MMS /id:2 /physicalPath:" + destination + " / bindings:http/*:80: ";
                //ExecuteCommand(command);

                // C:\projects\ey\Code\Rmi\EY\Mms\Website
                CopyAll(source, destination);

                command = "C:\\windows\\system32\\inetsrv\\appcmd set apppool /apppool.name:DefaultAppPool /enable32bitapponwin64:true";
                ExecuteCommand(command);

                command = "C:\\windows\\system32\\inetsrv\\appcmd set apppool /apppool.name:DefaultAppPool /managedRuntimeVersion:v4.0";
                ExecuteCommand(command);

                ConvertToApplicationSite(destination,"Mms");


            }
            catch (Exception e)
            {
                Console.WriteLine("The site creation fails {0}", e.StackTrace.ToString());
                throw;
            }
        }

        public static void CreateWebApiService()
        {
            try
            {

                var projectFolder = @"C:\projects\ey\Code\Rmi\EY\Mms\WebApi "; //  \Rmi.EY.Mms.WebApi.csproj";
                var publishLocation = @"C:\inetpub\MMSWebApiService\";
                var command = "";

                if (!Directory.Exists(publishLocation))
                {
                    command = "mkdir " + publishLocation;
                    ExecuteCommand(command);
                }
               
                CopyAll(projectFolder, publishLocation);
                command = "C:\\windows\\system32\\inetsrv\\appcmd set apppool /apppool.name:DefaultAppPool /enable32bitapponwin64:true";
                ExecuteCommand(command);

                command = "C:\\windows\\system32\\inetsrv\\appcmd set apppool /apppool.name:DefaultAppPool /managedRuntimeVersion:v4.0";
                ExecuteCommand(command);
                ConvertToApplicationSite(publishLocation, "MmsWebApiService");


            }
            catch (Exception e)
            {
                Console.WriteLine("The site creation fails {0}", e.StackTrace.ToString());
                throw;
            }
        }

        private static void InstallService(string exeFilename)
        {
            var installer = new AssemblyInstaller(exeFilename,null);
            installer.UseNewContext = true;
            installer.Install(null);
            installer.Commit(null);
        }

        private static void UninstallService(string exeFilename)
        {
            var installer = new AssemblyInstaller(exeFilename, null);
            installer.UseNewContext = true;
            installer.Uninstall(null);
        }
        public static void CreateMmsServiceManager()
        {
            try
            {
                var serviceName = @"C:\projects\ey\Code\Rmi\EY\Mms\ServiceManager\obj\Debug\Rmi.EY.Mms.ServiceManager.exe";
                InstallService(serviceName);
                Console.WriteLine("Service Manager is installed successfully...");
            }
            catch (Exception e)
            {
                Console.WriteLine("Service Manager failed due to Exception:{0}", e.ToString());
            }   
        }


        private static bool BuildWebApiSolution()
        {
            try
            {

                var cmdPath = @"C:\Program Files (x86)\MSBuild\12.0\Bin\MSBuild.exe\";
                var projFile = @"C:\projects\ey\Code\Rmi\EY\Mms\WebApi\";
                var command = cmdPath + " " + projFile;
                var procStartInfo = new ProcessStartInfo("cmd", "/c " + command + " 1> c:\\MMS_PublishBuild.txt");
                var proc = new Process();
                proc.StartInfo = procStartInfo;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                proc.Start();
                while (!proc.HasExited)
                {
                    Thread.Sleep(2000);
                    Console.WriteLine("Publish is in progress. Wait....");
                }
                var res = File.OpenText("c:\\MMS_PublishBuild.txt");
                while (!res.EndOfStream)
                {
                    string line = res.ReadLine();
                    if (line != null)
                    {
                        if (Convert.ToString(line).Contains("Publish succeeded"))
                        {
                            Console.WriteLine("Publish succeeded...");
                            return true;
                        }
                    }
                }
                Console.WriteLine("Publish failed ...");
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine("Publish failed due to Exception:{0}", e.ToString());
            }
            return false;
        }

        private static void ConvertToApplicationSite(string destination, string webSiteName)
        {

            //check if website name already exists in IIS
            var manager = new ServerManager();
            bool bWebsite = IsWebsiteExists(manager, webSiteName);
            if (!bWebsite)
            {
                Site defaultSite = manager.Sites["Default Web Site"];
                defaultSite.Applications.Add("/"+ webSiteName, destination);
                manager.CommitChanges();
                Console.WriteLine("New website {0} added sucessfully",webSiteName);
            }
            else
            {
                Console.WriteLine("Name should be unique {0}  is already exists. ", webSiteName);
            }
        }

        private static bool IsWebsiteExists(ServerManager serverMgr, string strWebsitename)
        {
            Boolean flagset = false;
            SiteCollection sitecollection = serverMgr.Sites;
            foreach (Site site in sitecollection)
            {
                if (site.Name.ToLower().Equals(strWebsitename.ToLower(), StringComparison.Ordinal))
                {
                    flagset = true;
                    break;
                }
                else
                {
                    flagset = false;
                }
            }
            return flagset;
        }
    

    private static void CopyAll(string sourcePath, string destinationPath)
        {
            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*",
                SearchOption.AllDirectories))
                Directory.CreateDirectory(destinationPath + dirPath.Remove(0, sourcePath.Length));

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*",
                SearchOption.AllDirectories))
                File.Copy(newPath, destinationPath + newPath.Remove(0, sourcePath.Length), true);
        }

        private static void ExecuteCommand(string command)
        {
            Console.WriteLine("Executed Command: {0}", command);
            var procStartInfo = new ProcessStartInfo("cmd", "/c " + command + " 2>> c:\\MMS_Setup.log");
            var proc = new Process();
            proc.StartInfo = procStartInfo;
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.Start();
            proc.WaitForExit();
        }


    }
}
