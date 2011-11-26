using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Collections.ObjectModel;

namespace XBMCRemote
{
    public class IndexingService
    {
        string filename = "index.dat";
        public IndexingService()
        {
            if (System.IO.File.Exists(filename))
            {
                using (var file = System.IO.File.OpenRead(filename))
                    Files = (HashSet<string>)new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter().Deserialize(file);
                Console.WriteLine(Files.Count + " files read from previous database.");
            }
            else
            {
                Files = new HashSet<string>();
                Console.WriteLine("0 files read from previous database.");
            }
            FilesUpdated += (_, __) => { };
        }
        Task task;
        XbmcListRepsonseParser xbmcListRepsonseParser = new XbmcListRepsonseParser();
        public HashSet<string> Files {get;set;}

        public void Start()
        {
            if (task!=null && !task.IsCompleted) return;
            task = Task.Factory.StartNew(indexFromRoot);
        }
        public void indexFromRoot()
        {
            //WebClient wc = new WebClient();
            //wc.DownloadStringAsync(new Uri(App.xbox + "/xbmcCmds/xbmcHttp?command=GetMediaLocation(video)"));

            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            timer.Start();
            addEntriesToIndex("smb://xbox:omgxbox@utonium/d/tv/");
            timer.Stop();
            System.Diagnostics.Debug.WriteLine("completed in "+timer.ElapsedMilliseconds+"ms");
            using(var file = System.IO.File.OpenWrite(filename))
                new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter().Serialize(file,Files);
        }
        public void addEntriesToIndex(string location)
        {
            //System.Diagnostics.Debug.Write(location +" ");
            
            WebClient wc = new WebClient();
            Action<string> process = (string result) =>
            {
                var newfiles = false;
                IList<string> responseList = xbmcListRepsonseParser.GetItemsFromRepsonse(result);
                IList<string> strippedresponseList = responseList.Select(f => f.Replace(location, "")).ToList();
                foreach(var item in 
                    strippedresponseList )
                    if (!Files.Contains(item))
                    {
                        Files.Add(item);
                        newfiles = true;
                    }
                if (newfiles)
                {
                    FilesUpdated(this, new EventArgs<IList<string>>(strippedresponseList));
                    System.Diagnostics.Debug.WriteLine(Files.Count + " items");
                }
                foreach (var dir in responseList.Where(f => f.EndsWith("/")))
                    addEntriesToIndex(dir);
            };
            //wc.DownloadStringCompleted += (o, ea) => process(ea.Result);
            var result1 = wc.DownloadString(new Uri(App.xbox + "/xbmcCmds/xbmcHttp?command=GetDirectory(" + location + ")"));
            process(result1);
        }
        public event EventHandler<EventArgs<IList<string>>> FilesUpdated;
    }
    public class EventArgs<T> : EventArgs
    {
        public T Arg {get; private set;}
        public EventArgs(T arg) { Arg = arg; }
    }
}