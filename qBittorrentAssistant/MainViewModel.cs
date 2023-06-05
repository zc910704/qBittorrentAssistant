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

        [ObservableProperty]
        private string _UserName;

        [ObservableProperty]
        private string _Password;

        [ObservableProperty]
        private BindingList<TorrentInfo> _Torrents = new BindingList<TorrentInfo>();

        [ObservableProperty]
        private TorrentInfo _SelectedTorrent;

        [ObservableProperty]
        private DirectoryTreeItem _SelectedItemInCurrentDirectory;

        private QBittorrentClient? _Client = null;

        public MainViewModel()
        {
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
        public void SwitchLanguageToEnglish()
        {            
            var eng = "/Resource/LanguageResource/LanguageResource_en_US.xaml";
            SwitchLanguage(eng);
        }

        [RelayCommand]
        public void SwitchLanguageToChinese()
        {
            var chn = "/Resource/LanguageResource/LanguageResource_zh_CN.xaml";
            SwitchLanguage(chn);
        }

        private void SwitchLanguage(string xamlSourceUri)
        {
            var resDict = Application.Current.Resources.MergedDictionaries;
            foreach (var res in resDict)
            {
                if (res.Source != null && res.Source.OriginalString.Contains("LanguageResource"))
                {
                    resDict.Remove(res);
                    res.Source = new Uri(xamlSourceUri, UriKind.RelativeOrAbsolute);
                    resDict.Add(res);
                    break;
                }
            }
        }

        [RelayCommand]
        public void DeleteItem()
        {
            try
            {
                File.Delete(SelectedItemInCurrentDirectory.FullPath);
            }
            catch (UnauthorizedAccessException accessExp) 
            {
                MessageBox.Show(accessExp.Message);
            }

            catch(Exception e)             
            {
                LogHelper.Error("MainViewModel::DeleteItem |Exception ", e);
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
                LogHelper.Error("MainViewModel::NavigateToPath |", ex);
            }
            
        }

        [RelayCommand]
        private async void LoginAndConnect()
        {
            try
            {
                _Client = new QBittorrentClient(new Uri("http://localhost:8080/"));
                if(!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password)) 
                {
                    await _Client.LoginAsync(UserName, Password);
                }                
                var result = _Client.GetTorrentListAsync();
                foreach (var torrent in result.Result)
                {
                    var contents = await _Client.GetTorrentContentsAsync(torrent.Hash);
                    foreach (var file in contents)
                    {
                        TorrentHelper.Add(torrent, file);
                    }
                    Torrents.Add(torrent);
                }
            }
            catch (Exception e)
            { 
                LogHelper.Error("MainViewModel::LoginAndConnect |Exception:", e);
            }

        }

        [RelayCommand]
        private void CalculateSize()
        {
            ByteSize byteSize = ByteSize.FromBits(0);
            var size = GetByteSize(SelectedDirectoryTreeItem);
            foreach(var directoryTreeItem in SelectedDirectoryTreeItem.Childrens) 
            {
                directoryTreeItem.IsContainByTorrent = TorrentHelper.IsContainAnyTorrent(directoryTreeItem.FullPath);
            }
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
                        {
                            item.IsExpanded = true;                            
                        }                        
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

        partial void OnSelectedTorrentChanged(TorrentInfo value)
        {
            if(value !=  null) 
            {
                AddressColumnPath = value?.ContentPath;
                NavigateToPath();
            }            
        }

        partial void OnSelectedItemInCurrentDirectoryChanged(DirectoryTreeItem value)
        {
            SelectedTorrent = TorrentHelper.GetTorrentInfo(value.FullPath);
        }
    }
}
