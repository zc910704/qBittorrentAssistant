using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using QBittorrent.Client;

namespace qBittorrentAssistant
{
    internal partial class MainViewModel: ObservableObject
    {
        public MainViewModel()
        {
            //var client = new QBittorrentClient(new Uri("http://localhost:8080/"));
            ////await client.LoginAsync("login", "password");
            //var result = client.GetTorrentListAsync();
            //var r = result.Result;
        }
    }
}
