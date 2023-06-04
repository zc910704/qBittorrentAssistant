using QBittorrent.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace qBittorrentAssistant.Models
{
    public static class TorrentHelper
    {
        //private static Dictionary<TorrentInfo, IList<TorrentContent>> _TorrentDict = new Dictionary<TorrentInfo, IList<TorrentContent>>();

        private static Dictionary<string, TorrentInfo> _ContentTorrentDict = new Dictionary<string, TorrentInfo>();

        public static void Add(TorrentInfo torrentInfo, TorrentContent torrentContent)
        {

            //if (_TorrentDict.ContainsKey(torrentInfo))
            //{
            //    _TorrentDict.Add(torrentInfo, new List<TorrentContent>());
            //}
            //else
            //{
            //    _TorrentDict[torrentInfo].Add(torrentContent);
            //}

            var path = Path.Combine(torrentInfo.SavePath, torrentContent.Name.Replace("/",@"\"));

           if (!_ContentTorrentDict.ContainsKey(path))
            {
                _ContentTorrentDict.Add(path, torrentInfo);
            }
        }

        public static bool IsContainAnyTorrent(string path)
        {
            foreach (var contentPath in _ContentTorrentDict.Keys)
            {
                if (Path.Equals(contentPath, path))
                {
                    return true;
                }
                else
                {
                    if (Path.GetFullPath(contentPath).StartsWith(Path.GetFullPath(path)))
                    {
                        return true;
                    }
                }                
            }
            return false;
        }

        public static TorrentInfo GetTorrentInfo(string path)
        {
            foreach (var entity in _ContentTorrentDict)
            {
                if (Path.Equals(entity.Key, path))
                {
                    return entity.Value;
                }
                else
                {
                    if (entity.Key.Contains(path))
                    { 
                        return entity.Value;
                    }
                }
            }
            return null;
        }
    }
}
