using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qBittorrentAssistant
{
    public static class Logger
    {
        public static void Error(string msg, Exception? exception) { }

        public static void Info(string msg) { }
    }
}
