using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using QBittorrent.Client;
using qBittorrentAssistant.Models;
using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ByteSizeLib;
using NLog.LayoutRenderers;
using System.Security.Policy;

namespace qBittorrentAssistant
{
    internal partial class MainViewModel: ObservableObject
    {
        [ObservableProperty]
        private BindingList<DirectoryTreeItem> _DirectoryTreeItems = new BindingList<DirectoryTreeItem>();
        
        [ObservableProperty]
        private string _AddressColumnPath;

        [ObservableProperty]
        private DirectoryTreeItem _SelectedDirectoryTreeItem;

        public MainViewModel()
        {
            //var client = new QBittorrentClient(new Uri("http://localhost:8080/"));
            ////await client.LoginAsync("login", "password");
            //var result = client.GetTorrentListAsync();
            //var r = result.Result;
            GetDirectDataSource();
        }

        private void GetDirectDataSource()
        {
            foreach (DirectoryTreeItem item in DirectoryTreeItem.GetRootDirectoryItems()) 
            {
                DirectoryTreeItems.Add(item);
            }
        }


        [RelayCommand]
        public void NavigateToPath()
        {
            try
            {
                DirectoryTreeItem currentDir = null;

                if (string.IsNullOrEmpty(AddressColumnPath))
                    return;
                var allDirectoryInPath = AddressColumnPath
                    .Replace("\"","")
                    .Replace("\'", "")
                    .Split(new char[] { '\\', '/' });

                int searchIndex = 0;
                if (allDirectoryInPath.Length > 0)
                {
                    // d.Name : C:\ 
                    // allDirectoryInPath[searchIndex] : C:
                    if (DirectoryTreeItems.Any(d => d.Name.Contains(allDirectoryInPath[searchIndex])))
                    {
                        currentDir = DirectoryTreeItems.First(d => d.Name.Contains(allDirectoryInPath[searchIndex]));
                    }
                    else
                    {
                        return;
                    }
                    searchIndex++;
                    while (currentDir.IsDirectory)
                    {
                        if (currentDir.IsDirectory)
                        {
                            currentDir.IsExpanded = true;
                            //currentDir.IsSelected = true;
                            if (currentDir.Childrens.Any(d => d.Name == allDirectoryInPath[searchIndex]))
                            {
                                currentDir = currentDir.Childrens.First(d => d.Name == allDirectoryInPath[searchIndex]);
                                //currentDir.IsSelected = true;
                                searchIndex++;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("MainViewModel::NavigateToPath |", ex);
            }
            
        }
        [RelayCommand]
        private void CalculateSize()
        {
            ByteSize byteSize = ByteSize.FromBits(0);
            var size = GetByteSize(SelectedDirectoryTreeItem);
        }

        private ByteSize GetByteSize(DirectoryTreeItem directoryTreeItem)
        {
            ByteSize byteSize = ByteSize.FromBits(0);

            foreach (var item in directoryTreeItem.Childrens)
            {
                if (item.Size != null)
                {
                    byteSize += item.Size.Value;
                }
                else
                {
                    if (!item.IsDirectory)
                    {
                        
                        FileInfo fileInfo = new FileInfo(item.FullPath);
                        item.Size = ByteSize.FromBytes(fileInfo.Length);
                        byteSize += item.Size.Value;
                    }
                    else
                    {
                        if (!item.IsExpanded)
                            item.IsExpanded = true;
                        item.Size = GetByteSize(item);
                        byteSize += item.Size.Value;
                    }
                }
            }
            return byteSize;
        }



        partial void OnSelectedDirectoryTreeItemChanged(DirectoryTreeItem value)
        {
            AddressColumnPath = value.FullPath;
        }
    }
}
