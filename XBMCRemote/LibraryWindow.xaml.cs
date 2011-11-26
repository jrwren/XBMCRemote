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
    /// <summary>
    /// Interaction logic for LibraryWindow.xaml
    /// </summary>
    public partial class LibraryWindow : Window, System.ComponentModel.INotifyPropertyChanged
    {
        public string SearchString { get; set; }
        public ObservableCollection<string> LibraryItems { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        DateTime lastFilter;
        ICollectionView view;

        public LibraryWindow()
        {
            InitializeComponent();
            DataContext = this;
            view = CollectionViewSource.GetDefaultView(LibraryItems);
            Observable.FromEventPattern<System.ComponentModel.PropertyChangedEventArgs>(this, "PropertyChanged")
                .Where(et => et.EventArgs.PropertyName == "SearchString")
                .Throttle(TimeSpan.FromMilliseconds(250))
                .Subscribe(_ => {
                    Console.WriteLine("search string changed");
                    
                    if (SearchString.Trim() == string.Empty)
                        //Dispatcher.BeginInvoke(new Action(() =>
                        view.Filter = null;
                    else
                        //Dispatcher.BeginInvoke(new Action(() =>
                        view.Filter = o => Regex.IsMatch(o as string, SearchString, RegexOptions.IgnoreCase);
                        
                    view.Refresh();
                });
            Observable.FromEventPattern<System.ComponentModel.PropertyChangedEventArgs>(this, "PropertyChanged")
                .Where(et => et.EventArgs.PropertyName == "LibraryItems")
                .Do(et => view = CollectionViewSource.GetDefaultView(LibraryItems));
            this.listbox.MouseDoubleClick += new MouseButtonEventHandler(listbox_MouseDoubleClick);
        }

        void listbox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var location = this.listbox.SelectedItem;
            var wc = new System.Net.WebClient();
            var result1 = wc.DownloadString(new Uri(App.xbox + "/xbmcCmds/xbmcHttp?command=PlayFile(" + location + ")"));
            System.Diagnostics.Debug.WriteLine("playing " + location);
        }
    }
}