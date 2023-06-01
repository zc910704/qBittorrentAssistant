using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace qBittorrentAssistant.Views.Behavior
{
    internal class ScrollListViewToSelectedBehavior : Behavior<Selector>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.SelectionChanged -= AssociatedObject_SelectionChanged;
        }

        private void AssociatedObject_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if(sender is ListView listView) 
            {
                // https://www.codeproject.com/questions/193208/how-to-scroll-to-a-selected-item-in-listview
                listView.ScrollIntoView(e.AddedItems[0]);
            }
        }
    }
}
