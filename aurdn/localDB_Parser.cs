using System;
using System.Collections.Generic;
using System.IO;

namespace aurdn
{
    public class localDB_Parser
    {
        static string dbPath = "/var/lib/pacman/local/";

        public Dictionary<string, string> GetLocalPackages()
        {
            var packageList = new Dictionary<string, string>();
            foreach (var dir in Directory.GetDirectories(dbPath))
            {
                var desc = File.ReadAllLines(dir + "/desc");
                //Console.WriteLine(desc);
                string name = "";
                string version = "";
                for (int i = 0; i < desc.Length; i++)
                {
                    switch (desc[i])
                    {
                        case "%NAME%":
                            name = desc[i + 1];
                            break;
                        case "%VERSION%":
                            version = desc[i + 1];
                            break;
                        //You'd need to modify the return type to add more info, but this is all we need afaik.
                        default:
                            break;
                    }
                }
                packageList.Add(name, version);
            }
            return packageList;
        }
    }
}