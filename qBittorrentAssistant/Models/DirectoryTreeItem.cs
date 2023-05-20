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
        /// <summary>
        /// SubDirectorys In Directory
        /// </summary>
        public BindingList<DirectoryTreeItem> Children { get; set; } = new BindingList<DirectoryTreeItem>();
        /// <summary>
        /// Content Files In Directory
        /// </summary>
        public BindingList<DirectoryTreeItem> Items { get; set; } = new BindingList<DirectoryTreeItem>();

        public ByteSize Size { get; set; }

        public DirectoryTreeItem(string fullPath, string name)
        {
            FullPath = fullPath;
            Name = name;
        }

        [ObservableProperty]
        private bool _IsSelected;

        private bool _IsGetDirectoryItems = false;

        private bool _IsExpand = false;
        public bool IsExpanded
        {
            get => _IsExpand;
            set
            {
                // first expand
                if (!_IsExpand && value && !_IsGetDirectoryItems && IsDirectory)
                {
                    foreach (var item in Directory.GetFileSystemEntries(FullPath))
                    {
                        if (item.Count() > FullPath.Count())
                        {
                            var name = item.Split(new char[] { '\\', '/' }).Last();
                            var itemInFullPath = new DirectoryTreeItem(item, name);
                            if (itemInFullPath.IsDirectory)
                            {
                                Children.Add(itemInFullPath);
                            }
                            else
                            {
                                Items.Add(itemInFullPath);
                            }
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
            foreach (string root in Directory.GetLogicalDrives())
            {
                yield return new DirectoryTreeItem(root, root);
            }
        }

    }
}
