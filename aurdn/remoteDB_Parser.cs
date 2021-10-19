using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace aurdn
{
    public class remoteDB_Parser
    {
        //This gets all packages from the AUR to check for updates.
        //https://aur.archlinux.org/rpc/?v=5&type=info&arg[]=PROGRAM1&arg[]=PROGRAM2
        const string baseURL = "https://aur.archlinux.org/rpc/?v=5&type=info";
        /// <summary>
        /// Using the localDB generated from installed packages, find all AUR packages that are out of date.
        /// </summary>
        /// <param name="toCheck">LocalDB</param>
        /// <returns>Dictionary of Outdated Applications, Formatted as Name and Version</returns>
        public Dictionary<string,string> GetRemoteDb(List<string> toCheck)
        {
            //Max 100 results per query for safety
            WebClient wc = new WebClient();
            var toQuery = Split(toCheck, 150);
            var Results = new List<string>();
            foreach (var query in toQuery)
            {
                string start = baseURL;
                foreach (var item in query)
                {
                    start += "&arg[]=" + item;
                }
                Results.Add(wc.DownloadString(start));
            }

            var RemoteResults = new Dictionary<string, string>();
            foreach (var list in Results)
            {
                JsonRoot Deserialized = JsonSerializer.Deserialize<JsonRoot>(list);
                if (Deserialized.resultcount != 0)
                {
                    foreach (var item in Deserialized.results)
                    {
                        RemoteResults.Add(item.Name, item.Version);
                    }
                }
            }

            return RemoteResults;
        }
        /// <summary>
        /// Splits up a List into arbitrarily sized chunks.
        /// </summary>
        /// <param name="source">Source List</param>
        /// <param name="size">How many objects per Lis</param>
        /// <returns>A list of split lists.</returns>
        private static List<List<T>> Split<T>(IList<T> source, int size)
        {
            return  source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / size)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }
        
        public class Result
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public int PackageBaseID { get; set; }
            public string PackageBase { get; set; }
            public string Version { get; set; }
            public string Description { get; set; }
            public string URL { get; set; }
            public int NumVotes { get; set; }
            public double Popularity { get; set; }
            public object OutOfDate { get; set; }
            public string Maintainer { get; set; }
            public int FirstSubmitted { get; set; }
            public int LastModified { get; set; }
            public string URLPath { get; set; }
            public List<string> Depends { get; set; }
            public List<string> MakeDepends { get; set; }
            public List<string> OptDepends { get; set; }
            public List<string> License { get; set; }
            public List<object> Keywords { get; set; }
        }

        public class JsonRoot
        {
            public int version { get; set; }
            public string type { get; set; }
            public int resultcount { get; set; }
            public List<Result> results { get; set; }
        }  
        
        /// <summary>
        /// Recursively get all the dependancies for the packages being installed or updated so they can be installed.
        /// Should be ordered by how soon they should be installed to meet dependencies. Pacman will handle any packages needed
        /// from the normal repos. Should not return any that are already installed, or on the list of packages being installed.
        /// </summary>
        /// <param name="packages">List of packages to get dependencies for.</param>
        /// <returns>Dictionary (for ease of passing around) of Name and Version of the applications to install.</returns>
        public Dictionary<string,string> GetPackageReqirements(Dictionary<string, string> packages)
        {
            //Take Package Name, Get Requirements, Return those packages
            WebClient wc = new WebClient();
            List<string> tmp = new List<string>();
            foreach (var item in packages)
            {
                tmp.Add(item.Key);
            }
            var toQuery = Split(tmp, 150);
            var Results = new List<string>();
            foreach (var query in toQuery)
            {
                string start = baseURL;
                foreach (var item in query)
                {
                    start += "&arg[]=" + item;
                }
                Results.Add(wc.DownloadString(start));
            }

            Dictionary<string, string> requirements = new Dictionary<string, string>();
            foreach (var list in Results)
            {
                JsonRoot Deserialized = JsonSerializer.Deserialize<JsonRoot>(list);
                if (Deserialized.resultcount != 0)
                {
                    foreach (var item in Deserialized.results)
                    {
                        requirements.Add(item.Name, item.Version);
                    }
                }
            }
            //TODO: Somehow drill down all the way to the bottom of the packges list and dig up any requiements that
            //Aren't already installed.
            return null;
        }
    }
}