using ByteSizeLib;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qBittorrentAssistant.Models
{
    internal partial class DirectoryTreeItem : ObservableObject
    {
        public string FullPath { get; set; }

        public string Name { get; set; }

        public BindingList<DirectoryTreeItem> Children { get; set; } = new BindingList<DirectoryTreeItem>();

        public ByteSize Size { get; set; }

        public DirectoryTreeItem(string fullPath, string name)
        {
            FullPath = fullPath;
            Name = name;
        }

        private bool _IsGetDirectoryItems = false;

        private bool _IsExpand = false;
        public bool IsExpanded
        {
            get => _IsExpand;
            set
            {
                // first expand
                if (!_IsExpand && value && !_IsGetDirectoryItems)
                {
                    foreach (var item in Directory.GetFileSystemEntries(FullPath))
                    {
                        if (item.Count() > FullPath.Count())
                        {
                            var name = item.Split(new char[] { '\\', '/' }).Last();
                            Children.Add(new DirectoryTreeItem(item, name));
                        }
                        else
                        {
                            Logger.Error("DirectoryTreeItem::SetIsExpanded |item.Count() <= FullPath.Count()", null);
                        }
                    }
                }
                _IsExpand = value;
                _IsGetDirectoryItems = true;
            }
        }

        public static IEnumerable<DirectoryTreeItem> GetRootDirectoryItems()
        {
            foreach (string path in Directory.GetLogicalDrives())
            {
                yield return new DirectoryTreeItem(path, path);
            }
        }
    }
}
