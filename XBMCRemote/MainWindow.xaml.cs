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
using System.Net;
using System.Text.RegularExpressions;

namespace XBMCRemote
{
    public partial class MainWindow : DXWindow
    {
        private IndexingService iservice = new IndexingService();
        private bool settingPosition;
        //http://xbmc.svn.sourceforge.net/viewvc/xbmc/trunk/guilib/Key.h?view=markup
        private WebClient GetWc(string note)
        {
            WebClient wc = new WebClient();
            wc.DownloadStringCompleted += (o, ea) =>
            {

                System.Diagnostics.Debug.WriteLine(note+ (ea.Error == null ? wc.BaseAddress+"\n"+ea.Result : ea.Error.ToString()));
                status.Text = ea.Error == null ? ea.Result.Replace("<html>\n<li>", "").Replace("</html>", "").Split('\n')[0].Replace("Filename:", "") : ea.Error.ToString();
            };
            return wc;
        }

        private void getVolume()
        {
            var volwc = new WebClient();
            volwc.DownloadStringCompleted += (o, ea) => 
                volume.Value = ea.Error==null 
                ? Convert.ToDouble(ea.Result.Replace("<html>\n<li>", "").Replace("</html>", "").Split('\n')[0])
                : volume.Value 
                ;
            volwc.DownloadStringAsync(new Uri(Xbox + "/xbmcCmds/xbmcHttp?command=GetVolume()"));
        }
        public MainWindow()
        {
            InitializeComponent();
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += (_, __) =>
            {
                WebClient wc = new WebClient();
                wc.DownloadStringCompleted += (o, ea) =>
                {
                    if (ea.Error == null)
                    {
                        string[] results = ea.Result.Replace("<html>\n<li>", "").Replace("</html>", "").Split('\n');
                        filename.Text =
                            Regex.Replace(
                                System.Web.HttpUtility.UrlDecode(results[0].Replace("Filename:", "")),
                                "smb://[^/]*",
                                ""
                            );
                        if (results.Length > 5)
                        {
                            currentposition.Text = results.First(r=>r.StartsWith("<li>Time")).Replace("<li>Time:", "");
                            tracklength.Text = results.First(r => r.StartsWith("<li>Duration")).Replace("<li>Duration:", "");
                            settingPosition = true;
                            position.Value = Convert.ToDouble(results.First(r => r.StartsWith("<li>Percentage")).Replace("<li>Percentage:", ""));
                            settingPosition = false;
                        }
                    }
                };
                wc.DownloadStringAsync(new Uri(Xbox + "/xbmcCmds/xbmcHttp?command=getCurrentlyPlaying"));
                getVolume();
            };
            timer.IsEnabled = true;
            //var twc = GetWc("shares");
            //twc.DownloadStringAsync(new Uri(xbox + "/xbmcCmds/xbmcHttp?command=GetShares(video)"));
            //twc = GetWc("gml(v)");
            //twc.DownloadStringAsync(new Uri(xbox + "/xbmcCmds/xbmcHttp?command=GetMediaLocation(video)"));
            
            //var twc = GetWc("gml(v;tv)");
            //twc.DownloadStringAsync(new Uri(xbox + "/xbmcCmds/xbmcHttp?command=GetMediaLocation(video;tv)"));//error invalid media type
            //does work: twc.DownloadStringAsync(new Uri(xbox + "/xbmcCmds/xbmcHttp?command=GetDirectory(smb://xbox:omgxbox@utonium/d/)"));
            //iservice.Start();
        }

        void power_click(object sender, EventArgs ea)
        {
            WebClient wc = GetWc("power_click");
            wc.DownloadStringAsync(new Uri(Xbox + "/xbmcCmds/xbmcHttp?command=SendKey(0XF053)"));
        }
        void esc_click(object sender, EventArgs ea)
        {
            var wc = GetWc("esc_click");
            wc.DownloadStringAsync(new Uri(Xbox + "/xbmcCmds/xbmcHttp?command=SendKey(0XF01B)"));
        }
        void show_hide_subtitles_click(object sender, EventArgs ea)
        {
            var wc = GetWc("show_hide_subtitles_click");
            wc.DownloadStringAsync(new Uri(Xbox + "/xbmcCmds/xbmcHttp?command=Action(25)"));
        }
        void back_to_movie_click(object sender, EventArgs ea)
        {
            var wc = GetWc("back_to_movie_click");
            wc.DownloadStringAsync(new Uri(Xbox + "/xbmcCmds/xbmcHttp?command=SendKey(0XF009)"));
        }
        void back_click(object sender, EventArgs ea)
        {
            var wc = GetWc("back_click");
            wc.DownloadStringAsync(new Uri(Xbox + "/xbmcCmds/xbmcHttp?command=Action(9)"));
        }
        void up(object sender, EventArgs ea)
        {
            Console.WriteLine("up_click");
            var wc = GetWc("up_click");
            wc.DownloadStringAsync(new Uri(Xbox + "/xbmcCmds/xbmcHttp?command=Action(3)"));
        }
        void down(object sender, EventArgs ea)
        {
            var wc = GetWc("down_click");
            wc.DownloadStringAsync(new Uri(Xbox + "/xbmcCmds/xbmcHttp?command=Action(4)"));
        }
        void left(object sender, EventArgs ea)
        {
            var wc = GetWc("left_click");
            wc.DownloadStringAsync(new Uri(Xbox + "/xbmcCmds/xbmcHttp?command=Action(1)"));
        }
        void right(object sender, EventArgs ea)
        {
            var wc = GetWc("right_click");
            wc.DownloadStringAsync(new Uri(Xbox + "/xbmcCmds/xbmcHttp?command=Action(2)"));
        }
        void enter(object sender, EventArgs ea)
        {
            var wc = GetWc("enter_click");
            wc.DownloadStringAsync(new Uri(Xbox + "/xbmcCmds/xbmcHttp?command=SendKey(0XF00D)"));
        }

        void stop_click(object sender, EventArgs ea)
        {
            var wc = GetWc("stop_click");
            wc.DownloadStringAsync(new Uri(Xbox + "/xbmcCmds/xbmcHttp?command=Stop()"));
        }
        void i_click(object sender, EventArgs ea)
        {
            var wc = GetWc("i_click");
            wc.DownloadStringAsync(new Uri(Xbox + "/xbmcCmds/xbmcHttp?command=SendKey(0XF049)"));
        }
        void pause_click(object sender, EventArgs ea)
        {
            var wc = GetWc("pause_click");
            wc.DownloadStringAsync(new Uri(Xbox + "/xbmcCmds/xbmcHttp?command=Pause()"));
        }
        void leftscrub_click(object sender, EventArgs ea)
        {
            var wc = GetWc("");
            wc.DownloadStringAsync(new Uri(Xbox + "/xbmcCmds/xbmcHttp?command=SeekPercentageRelative(-1)"));
        }
        void rightscrub_click(object sender, EventArgs ea)
        {
            var wc = GetWc("");
            wc.DownloadStringAsync(new Uri(Xbox + "/xbmcCmds/xbmcHttp?command=SeekPercentageRelative(1)"));
        }
        void mute_click(object sender, EventArgs ea)
        {
            var wc = GetWc("");
            wc.DownloadStringAsync(new Uri(Xbox + "/xbmcCmds/xbmcHttp?command=Mute()"));
        }
        void volume_changed(object sender, RoutedPropertyChangedEventArgs<double> ea)
        {
            var wc = GetWc("");
            var volume = Convert.ToInt16(ea.NewValue);
            wc.DownloadStringAsync(new Uri(Xbox + "/xbmcCmds/xbmcHttp?command=SetVolume(" + volume + ")"));
        }
        void position_changed(object sender, RoutedPropertyChangedEventArgs<double> ea)
        {
            if (settingPosition)
                return;
            var wc = GetWc("");
            var position = Convert.ToInt16(ea.NewValue);
            wc.DownloadStringAsync(new Uri(Xbox + "/xbmcCmds/xbmcHttp?command=SeekPercentage(" + position + ")"));
        }
        public string Xbox
        {
            get
            {
                return App.xbox;
            }
        }

        private void Library_Click(object sender, RoutedEventArgs e)
        {
            var LibraryWindow = new LibraryWindow();
            LibraryWindow.LibraryItems = new System.Collections.ObjectModel.ObservableCollection<string>(iservice.Files);
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            LibraryWindow.Show();
            stopwatch.Stop();
            Console.WriteLine("show took "+stopwatch.ElapsedMilliseconds+"ms");
        }
    }
}