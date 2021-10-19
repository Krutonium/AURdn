using System;
using System.Collections.Generic;

namespace aurdn
{
    public class FindUpdates
    {  
        /// <summary>
        /// Compares the local and remote databases, and returns a Dictionary of packages that have mismatched version numbers.
        /// </summary>
        /// <param name="LocalDB">Local DB from localDBParser</param>
        /// <param name="RemoteDB">Remote DB from remoteDBParser</param>
        /// <returns>Dictionary(Package Name, Version String)</returns>
        public Dictionary<string,string> CheckForUpdates(Dictionary<string, string> LocalDB,
            Dictionary<string, string> RemoteDB)
        {
            Dictionary<string, string> toUpdate = new Dictionary<string, string>();

            foreach (var item in RemoteDB)
            {
                if (LocalDB.ContainsKey(item.Key))
                {
                    string localVer = LocalDB[item.Key];
                    if (item.Value != localVer)
                    {
                        toUpdate.Add(item.Key, item.Value);
                    }
                }
            }
            return toUpdate;
        }
    }
}