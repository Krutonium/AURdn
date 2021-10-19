using System;
using System.Collections.Generic;

namespace aurdn
{
    class Program
    {
        static void Main(string[] args)
        {
            localDB_Parser parser = new localDB_Parser();
            var localPackages = parser.GetLocalPackages();
            remoteDB_Parser remoteDbParser = new remoteDB_Parser();
            var toCheckForUpdates = new List<string>();
            foreach (var prog in localPackages)
            {
                toCheckForUpdates.Add(prog.Key);
            }
            var remotePackages = remoteDbParser.GetRemoteDb(toCheckForUpdates);
            FindUpdates updates = new FindUpdates();
            var Updates = updates.CheckForUpdates(localPackages, remotePackages);
            foreach (var item in Updates)
            {
                Console.Write(item + " ");
            }
            Console.WriteLine();
            Console.WriteLine("Update?");
            DownloadAndInstallUpdates installUpdates = new DownloadAndInstallUpdates();
            installUpdates.Update(localPackages, Updates);
        }
    }
}