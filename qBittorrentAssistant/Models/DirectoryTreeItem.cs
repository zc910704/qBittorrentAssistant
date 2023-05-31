using ByteSizeLib;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace qBittorrentAssistant.Models
{
    internal partial class DirectoryTreeItem : ObservableObject
    {
        public string FullPath { get; set; }

        public string Name { get; set; }

        private bool? _IsDirectory = null;
        public bool IsDirectory 
        {
            get
            {
                if (_IsDirectory == null)
                {
                    FileAttributes attr = File.GetAttributes(FullPath);
                    _IsDirectory = attr.HasFlag(FileAttributes.Directory);
                }
                return _IsDirectory.Value;
            }
        }
        public BindingList<DirectoryTreeItem> Childrens { get; set; } = new BindingList<DirectoryTreeItem>();

        public BindingList<DirectoryTreeItem> FilesInCurrent { get; set; } = new BindingList<DirectoryTreeItem>();

        public BindingList<DirectoryTreeItem> DirectoryInCurrent { get; set; } = new BindingList<DirectoryTreeItem>();

        [ObservableProperty]
        public ByteSize? _Size;

        public DirectoryTreeItem(string fullPath, string name)
        {
            FullPath = fullPath;
            Name = name;
        }

        [ObservableProperty]
        private bool _IsSelected;

        [ObservableProperty]
        private bool _IsContainByTorrent;

        private bool _IsExpand = false;
        public bool IsExpanded
        {
            get => _IsExpand;
            set
            {
                // first expand
                if (!_IsExpand && value && IsDirectory)
                {
                    foreach (var item in Directory.GetFileSystemEntries(FullPath))
                    {
                        if (item.Count() > FullPath.Count())
                        {
                            var name = item.Split(new char[] { '\\', '/' }).Last();
                            var itemInFullPath = new DirectoryTreeItem(item, name);
                            if (itemInFullPath.IsDirectory)
                            {
                                DirectoryInCurrent.Add(itemInFullPath);
                            }
                            else
                            {
                                FilesInCurrent.Add(itemInFullPath);
                            }
                            Childrens.Add(itemInFullPath);
                        }
                        else
                        {
                            Logger.Error("DirectoryTreeItem::SetIsExpanded |item.Count() <= FullPath.Count()", null);
                        }
                    }
                }
                _IsExpand = value;
            }
        }

        public static IEnumerable<DirectoryTreeItem> GetRootDirectoryItems()
        {
            foreach (string root in Directory.GetLogicalDrives())
            {
                yield return new DirectoryTreeItem(root, root);
            }
        }

    }
}
