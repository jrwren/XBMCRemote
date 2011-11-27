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
                System.Diagnostics.Debug.WriteLine(Files.Count + " files read from previous database.");
            }
            else
            {
                Files = new HashSet<string>();
                System.Diagnostics.Debug.WriteLine("0 files read from previous database.");
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
        public async Task addEntriesToIndex(string location)
        {
            //System.Diagnostics.Debug.Write(location +" ");            
            Func<string,IList<string>> process = (string result) =>
            {
                IList<string> responseList = xbmcListRepsonseParser.GetItemsFromRepsonse(result);
                var strippedresponseList = responseList.Select(f => f.Replace(location, "")).ToList();
                var filesAdded = new List<string>();
                foreach(var item in 
                    strippedresponseList )
                    if (!Files.Contains(item))
                    {
                        Files.Add(item);
                        filesAdded.Add(item);
                    }
                if (filesAdded.Count>0)
                {
                    FilesUpdated(this, new EventArgs<IList<string>>(filesAdded));
                    System.Diagnostics.Debug.WriteLine(filesAdded.Count+" new items added for total of "+Files.Count + " items");
                }
                return responseList;
            };
            var message = await App.HttpClient.GetAsync(new Uri(App.xbox + "/xbmcCmds/xbmcHttp?command=GetDirectory(" + location + ")"));
            var entries = process(message.Content.ReadAsString());
            foreach (var dir in entries.Where(f => f.EndsWith("/")))
                await addEntriesToIndex(dir);
        }
        public event EventHandler<EventArgs<IList<string>>> FilesUpdated;
    }
    public class EventArgs<T> : EventArgs
    {
        public T Arg {get; private set;}
        public EventArgs(T arg) { Arg = arg; }
    }
}