using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace aurdn
{
    public class DownloadAndInstallUpdates
    {
        //makepkg -si
        //git clone https://aur.archlinux.org/<package>.git
        /// <summary>
        /// Downloads, Builds, and Installs packages from the AUR
        /// </summary>
        /// <param name="Updates">The packages we need to update.</param>
        public void Update(Dictionary<string, string> Updates)
        {
            if (!Directory.Exists("/tmp/builddir"))
            {
                Directory.CreateDirectory("/tmp/builddir");
            }
            else
            {
                Directory.Delete("/tmp/builddir", true);
                Directory.CreateDirectory("/tmp/builddir");
            }
            //Clean Build Directory OP
            
            
            ProcessStartInfo PSI = new ProcessStartInfo();
            PSI.EnvironmentVariables.Add("PKGDEST", "/tmp/pkgdir/");
            PSI.FileName = "git";
            remoteDB_Parser remoteDB = new remoteDB_Parser();
            
            //var requirements = remoteDB.GetPackageReqirements(Updates);
            //Update(requirements);
            
            //That function is not yet functional. Packages that require AUR packages in their makedepends that are not
            //already installed will fail.
            
            foreach (var item in Updates)
            {
                
                PSI.Arguments = "clone https://aur.archlinux.org/" + item.Key + ".git";
                PSI.WorkingDirectory = "/tmp/builddir/";
                Process.Start(PSI).WaitForExit();
                PSI.FileName = "makepkg";
                PSI.Arguments = "-s";
                PSI.WorkingDirectory="/tmp/builddir/" + item.Key;
                Process.Start(PSI).WaitForExit();
                PSI.FileName = "git";
            }
            PSI.FileName = "sudo";
            PSI.Arguments = "pacman -U *.tar.zst";
            Process.Start(PSI).WaitForExit();
        }
    }
}