using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace qBittorrentAssistant.Views
{
    // WPF 中如何将多选项目的 SelectedItems 属性绑定到 ViewModel
    // https://www.bilibili.com/video/BV1HM411k76b/?spm_id_from=333.999.0.0&vd_source=feb776f27e66ce92d09ca656a18a7d3f
    // https://www.bilibili.com/read/cv23525469?spm_id_from=333.999.0.0&jump_opus=1
    internal class MonitorTreeViewSelectionAttachProperty
    {
        public static object GetMonitorTreeViewSelection(DependencyObject obj)
        {
            return (object)obj.GetValue(MonitorTreeViewSelectionProperty);
        }

        public static void SetMonitorTreeViewSelection(DependencyObject obj, object value)
        {
            obj.SetValue(MonitorTreeViewSelectionProperty, value);
        }

        // Using a DependencyProperty as the backing store for MonitorTreeViewSelection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MonitorTreeViewSelectionProperty =
            DependencyProperty.RegisterAttached("MonitorTreeViewSelection",
                typeof(object), 
                typeof(MonitorTreeViewSelectionAttachProperty),
                new PropertyMetadata(0, MonitorTreeViewSelectionCallback));


        private static void MonitorTreeViewSelectionCallback(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (d is TreeView treeView && bool.TryParse((string)e.NewValue, out bool monitor))
            {
                if (monitor)
                {
                    treeView.SelectedItemChanged += TreeView_SelectedItemChanged;
                }
                else
                { 
                    treeView.SelectedItemChanged -= TreeView_SelectedItemChanged;
                }
            }
        }

        private static void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var treeView = (TreeView)sender;
            SetTreeViewSelection(treeView, e.NewValue);
            treeView.GetBindingExpression(MonitorTreeViewSelectionProperty)?.UpdateSource();
        }

        public static object GetTreeViewSelection(DependencyObject obj)
        {
            return (object)obj.GetValue(TreeViewSelectionProperty);
        }

        public static void SetTreeViewSelection(DependencyObject obj, object value)
        {
            obj.SetValue(TreeViewSelectionProperty, value);
        }

        // Using a DependencyProperty as the backing store for TreeViewSelection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TreeViewSelectionProperty =
            DependencyProperty.RegisterAttached("TreeViewSelection",
                typeof(object), typeof(MonitorTreeViewSelectionAttachProperty),
                new PropertyMetadata(0));



    }
}
