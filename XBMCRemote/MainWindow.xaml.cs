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
using System.Text.RegularExpressions;

namespace XBMCRemote
{
    public class StatusUpdateHandler : System.Net.Http.DelegatingHandler
    {
        System.Net.Http.WebRequestHandler webRequestHandler;
        readonly Action<string> a;
        public StatusUpdateHandler(Action<string> a)
            : base(new System.Net.Http.WebRequestHandler())
        {
            this.a = a;
            this.webRequestHandler = base.InnerHandler as System.Net.Http.WebRequestHandler;
        }
        protected override System.Net.Http.HttpResponseMessage Send(System.Net.Http.HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            var response = base.Send(request, cancellationToken);
            //)a(response.Content.ReadAsString());
            return response;
        }

        protected override async System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> SendAsync(System.Net.Http.HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);
            //a(response.Content.ReadAsString())
            return response;
        }
    }
    public partial class MainWindow : DXWindow
    {
        bool settingPosition;
        System.Net.Http.HttpClient httpclient;
        //http://xbmc.svn.sourceforge.net/viewvc/xbmc/trunk/guilib/Key.h?view=markup
        public virtual System.Net.Http.HttpClient GetHC(string note)
        {
            if (httpclient == null)
            {
                var crazyHandler = new StatusUpdateHandler(s =>
                {
                    status.Text = !string.IsNullOrEmpty(s) ? s.Replace("<html>\n<li>", "").Replace("</html>", "").Split('\n')[0].Replace("Filename:", "") : "empty response";
                });
                httpclient = new System.Net.Http.HttpClient(crazyHandler);
            }
            return httpclient;
        }

        async System.Threading.Tasks.Task getCurrentlyPlaying()
        {
            var hc = GetHC("position tick");
            var response = await hc.GetAsync(App.xbox + "/xbmcCmds/xbmcHttp?command=getCurrentlyPlaying");
            string[] results = response.Content.ReadAsString().Replace("<html>\n<li>", "").Replace("</html>", "").Split('\n');
            filename.Text =
                Regex.Replace(
                    System.Web.HttpUtility.UrlDecode(results[0].Replace("Filename:", "")),
                    "smb://[^/]*",
                    ""
                );
            if (results.Length > 5)
            {
                currentposition.Text = results.First(r => r.StartsWith("<li>Time")).Replace("<li>Time:", "");
                tracklength.Text = results.First(r => r.StartsWith("<li>Duration")).Replace("<li>Duration:", "");
                settingPosition = true;
                position.Value = Convert.ToDouble(results.First(r => r.StartsWith("<li>Percentage")).Replace("<li>Percentage:", ""));
                settingPosition = false;
            }
        }

        async System.Threading.Tasks.Task getVolume()
        {
            var volwc = GetHC("get volume");
            var response = await volwc.GetAsync(Xbox + "/xbmcCmds/xbmcHttp?command=GetVolume()");
            string responseContent = response.Content.ReadAsString();
            System.Diagnostics.Debug.WriteLine("getVolume " + responseContent);
            string line1 = responseContent.Replace("<html>\n<li>", "").Replace("</html>", "").Split('\n')[0];
            volume.Value = Convert.ToDouble(line1);
        }
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded+=new RoutedEventHandler(MainWindow_Loaded);
        }

        async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //while(true) await backgroundtimer();
        }
        public async System.Threading.Tasks.Task backgroundtimer(){
            await System.Threading.Tasks.TaskEx.Delay(1000);
            await getCurrentlyPlaying();
            await getVolume();
            //var twc = GetWc("shares");
            //twc.GetAsync(xbox + "/xbmcCmds/xbmcHttp?command=GetShares(video)"));
            //twc = GetWc("gml(v)");
            //twc.GetAsync(xbox + "/xbmcCmds/xbmcHttp?command=GetMediaLocation(video)"));
            
            //var twc = GetWc("gml(v;tv)");
            //twc.GetAsync(xbox + "/xbmcCmds/xbmcHttp?command=GetMediaLocation(video;tv)"));//error invalid media type
            //does work: twc.GetAsync(xbox + "/xbmcCmds/xbmcHttp?command=GetDirectory(smb://xbox:omgxbox@utonium/d/)"));
            //iservice.Start();
        }

        void power_click(object sender, EventArgs ea)
        {
            var hc = GetHC("power_click");
            hc.GetAsync(Xbox + "/xbmcCmds/xbmcHttp?command=SendKey(0XF053)");
        }
        void esc_click(object sender, EventArgs ea)
        {
            var hc = GetHC("esc_click");
            hc.GetAsync(Xbox + "/xbmcCmds/xbmcHttp?command=SendKey(0XF01B)");
        }
        void show_hide_subtitles_click(object sender, EventArgs ea)
        {
            var hc = GetHC("show_hide_subtitles_click");
            hc.GetAsync(Xbox + "/xbmcCmds/xbmcHttp?command=Action(25)");
        }
        void back_to_movie_click(object sender, EventArgs ea)
        {
            var wc = GetHC("back_to_movie_click");
            wc.GetAsync(Xbox + "/xbmcCmds/xbmcHttp?command=SendKey(0XF009)");
        }
        void back_click(object sender, EventArgs ea)
        {
            var wc = GetHC("back_click");
            wc.GetAsync(Xbox + "/xbmcCmds/xbmcHttp?command=Action(9)");
        }
        void up(object sender, EventArgs ea)
        {
            Console.WriteLine("up_click");
            var wc = GetHC("up_click");
            wc.GetAsync(Xbox + "/xbmcCmds/xbmcHttp?command=Action(3)");
        }
        void down(object sender, EventArgs ea)
        {
            var wc = GetHC("down_click");
            wc.GetAsync(Xbox + "/xbmcCmds/xbmcHttp?command=Action(4)");
        }
        void left(object sender, EventArgs ea)
        {
            var wc = GetHC("left_click");
            wc.GetAsync(Xbox + "/xbmcCmds/xbmcHttp?command=Action(1)");
        }
        void right(object sender, EventArgs ea)
        {
            var wc = GetHC("right_click");
            wc.GetAsync(Xbox + "/xbmcCmds/xbmcHttp?command=Action(2)");
        }
        void enter(object sender, EventArgs ea)
        {
            var wc = GetHC("enter_click");
            wc.GetAsync(Xbox + "/xbmcCmds/xbmcHttp?command=SendKey(0XF00D)");
        }

        void stop_click(object sender, EventArgs ea)
        {
            var wc = GetHC("stop_click");
            wc.GetAsync(Xbox + "/xbmcCmds/xbmcHttp?command=Stop()");
        }
        void i_click(object sender, EventArgs ea)
        {
            var wc = GetHC("i_click");
            wc.GetAsync(Xbox + "/xbmcCmds/xbmcHttp?command=SendKey(0XF049)");
        }
        void pause_click(object sender, EventArgs ea)
        {
            var wc = GetHC("pause_click");
            wc.GetAsync(Xbox + "/xbmcCmds/xbmcHttp?command=Pause()");
        }
        void leftscrub_click(object sender, EventArgs ea)
        {
            var wc = GetHC("");
            wc.GetAsync(Xbox + "/xbmcCmds/xbmcHttp?command=SeekPercentageRelative(-1)");
        }
        void rightscrub_click(object sender, EventArgs ea)
        {
            var wc = GetHC("");
            wc.GetAsync(Xbox + "/xbmcCmds/xbmcHttp?command=SeekPercentageRelative(1)");
        }
        void mute_click(object sender, EventArgs ea)
        {
            var wc = GetHC("");
            wc.GetAsync(Xbox + "/xbmcCmds/xbmcHttp?command=Mute()");
        }
        void volume_changed(object sender, RoutedPropertyChangedEventArgs<double> ea)
        {
            var wc = GetHC("");
            var volume = Convert.ToInt16(ea.NewValue);
            wc.GetAsync(Xbox + "/xbmcCmds/xbmcHttp?command=SetVolume(" + volume + ")");
        }
        void position_changed(object sender, RoutedPropertyChangedEventArgs<double> ea)
        {
            if (settingPosition)
                return;
            var wc = GetHC("");
            var position = Convert.ToInt16(ea.NewValue);
            wc.GetAsync(Xbox + "/xbmcCmds/xbmcHttp?command=SeekPercentage(" + position + ")");
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
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            LibraryWindow.Show();
            stopwatch.Stop();
            Console.WriteLine("show took "+stopwatch.ElapsedMilliseconds+"ms");
            
        }
    }
}