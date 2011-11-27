using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DevExpress.Xpf.Core;
using System.Collections.ObjectModel;
using SRTSolutions.Elevate;
using System.Text.RegularExpressions;
using System.Reactive.Linq;
using System.ComponentModel;

namespace XBMCRemote
{

    public partial class LibraryWindow : Window
    {
        public LibraryWindow()
        {
            InitializeComponent();
            DataContext = new LibraryWindowViewModel();
            this.listbox.MouseDoubleClick += new MouseButtonEventHandler(listbox_MouseDoubleClick);
         }
                   
        async void listbox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var location = this.listbox.SelectedItem;
            System.Diagnostics.Debug.WriteLine("playing " + location);
            var response = await App.HttpClient.GetAsync(App.xbox + "/xbmcCmds/xbmcHttp?command=PlayFile(" + location + ")");
            System.Diagnostics.Debug.WriteLine(response.Content.ReadAsString());
        }
    }
    public class LibraryWindowViewModel : INotifyPropertyChanged
    {
        public string SearchString { get; set; }
        public ObservableCollection<string> LibraryItems { get; set; }
        public int FilteredLibraryItemsCount { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        ICollectionView view;

        IndexingService iservice = new IndexingService();

        public LibraryWindowViewModel() 
        {
            view = CollectionViewSource.GetDefaultView(LibraryItems);

            LibraryItems = new System.Collections.ObjectModel.ObservableCollection<string>(iservice.Files);
            iservice.Start();
            iservice.FilesUpdated += new EventHandler<EventArgs<IList<string>>>(iservice_FilesUpdated);

            this.PropertyChanged += (s, ea) => 
                System.Diagnostics.Debug.WriteLine(ea.PropertyName + " changed to " + 
                    this.GetType().GetProperty(ea.PropertyName).GetValue(this,null));

            Observable.FromEventPattern<System.ComponentModel.PropertyChangedEventArgs>(this, "PropertyChanged")
                .Where(et => et.EventArgs.PropertyName == "SearchString")
                .Throttle(TimeSpan.FromMilliseconds(250))
                .Subscribe(_ => {
                    System.Diagnostics.Debug.WriteLine("search string changed");
                    
                    if (SearchString.Trim() == string.Empty)
                        view.Filter = null;
                    else
                        view.Filter = o => Regex.IsMatch(o as string, SearchString, RegexOptions.IgnoreCase);
                        
                    view.Refresh();
                    FilteredLibraryItemsCount = LibraryItems.Where(o => Regex.IsMatch(o as string, SearchString, RegexOptions.IgnoreCase)).Count();
                });

            //Observable.FromEventPattern<System.ComponentModel.PropertyChangedEventArgs>(this, "PropertyChanged")
            //    .Where(et => et.EventArgs.PropertyName == "LibraryItems")
            //    .Do(et => view = CollectionViewSource.GetDefaultView(LibraryItems));
            
            

        }

        void iservice_FilesUpdated(object sender, EventArgs<IList<string>> e)
        {
            var newfiles = e.Arg;
            foreach (var newfile in newfiles)
            {
                this.LibraryItems.Add(newfile);
                System.Diagnostics.Debug.WriteLine("added " + newfile + " to list");
            }
        }
        
    }
}