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
    public sealed partial class ItemThumbnail : GridViewItem
    {
        BitmapImage img;

        public ItemThumbnail()
        {
            this.InitializeComponent();
        }

        public Uri Image
        {
            set
            {
                img = new BitmapImage(value);
                iImage.Source = img;
            }
            get
            {
                return img.UriSource;
            }
        }

        public string Text
        {
            set
            {
                iText.Text = value;
                this.Tag = value;
            }
        }
    }
}
