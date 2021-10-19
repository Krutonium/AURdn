using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace aurdn
{
    public class DownloadAndInstallUpdates
    {
        //makepkg -si
        //git clone https://aur.archlinux.org/<package>.git

        public void Update(Dictionary<string, string> LocalDB, Dictionary<string, string> Updates)
        {
            //Instead of using a library, it's probably better to use the system git binary.
            if (!Directory.Exists("/tmp/builddir"))
            {
                Directory.CreateDirectory("/tmp/builddir");
            }
            else
            {
                Directory.Delete("/tmp/builddir", true);
                Directory.CreateDirectory("/tmp/builddir");
            }
            ProcessStartInfo PSI = new ProcessStartInfo();
            PSI.EnvironmentVariables.Add("PKGDEST", "/tmp/pkgdir/");
            PSI.FileName = "git";
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

            foreach (var file in "/tmp/pkgdir")
            {
                PSI.FileName = "sudo";
                PSI.Arguments = "pacman -U *.tar.zst";
            }
        }
    }
}