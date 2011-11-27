using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace XBMCRemote
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static readonly string xbox = "http://192.168.15.121";
        public static System.Net.Http.HttpClient httpclient;
        public static System.Net.Http.HttpClient HttpClient
        {
            get
            {
                if (httpclient == null)
                {
                    httpclient = new System.Net.Http.HttpClient();
                }
                return httpclient;
            }
        }
    }
}