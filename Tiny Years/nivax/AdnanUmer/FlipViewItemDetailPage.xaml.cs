using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace BabyJournal
{
    public sealed partial class FlipViewItemDetailPage : FlipViewItem
    {
        public FlipViewItemDetailPage(JournalItem item)
        {
            this.InitializeComponent();
            iImageBox.Source = new BitmapImage(item.ImageUri);
            iImageBox.Tag = item.ImageUri;
            iTitle.Text = item.Title;
            iDesc.Text = item.Description;
        }

        public string Title
        {
            get
            {
                return iTitle.Text;
            }
        }

        public string Desc
        {
            get
            {
                return iDesc.Text;
            }
        }

        public Uri ImageUri
        {
            get
            {
                return iImageBox.Tag as Uri;
            }
        }
    }
}
