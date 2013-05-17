using BabyJournal.Data;

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
using Windows.UI.Xaml.Navigation;

// The Grouped Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234231

namespace BabyJournal
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class GroupedItemsPage : BabyJournal.Common.LayoutAwarePage
    {
        public GroupedItemsPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Create an appropriate data model for your problem domain to replace the sample data
            //var sampleDataGroups = SampleDataSource.GetGroups((String)navigationParameter);
            //this.DefaultViewModel["Groups"] = sampleDataGroups;

            if (App.AppDataFile.Favourites.Count > 0)
            {
                FavGrid.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
                FavGrid.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            if (App.AppDataFile.Groups.Count == 0)
            {
                iNoItem.Visibility = Windows.UI.Xaml.Visibility.Visible ;
                itemGridView.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            else
            {
                foreach (var item in App.AppDataFile.Groups)
                {
                    GroupThumnail thumb = new GroupThumnail();
                    thumb.Text = item;
                    thumb.Tapped += Thumbnail_Tapped;
                    itemGridView.Items.Add(thumb);
                }
            }
            EnableLiveTile.CreateLiveTile.ShowliveTile(true, "Tiny Years");
        }

        void Thumbnail_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(GalleryPage), sender);
        }

        void OnAddNew(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AddNewBaby));
        }

        void OnFavClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(FavGalleryPage));
        }
    }
}
