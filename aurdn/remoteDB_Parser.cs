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
        public static List<List<T>> Split<T>(IList<T> source, int size)
        {
            return  source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / size)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }
        
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
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
    }
}