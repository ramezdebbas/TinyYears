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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;

// The Item Detail Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234232

namespace BabyJournal
{
    /// <summary>
    /// A page that displays details for a single item within a group while allowing gestures to
    /// flip through other items belonging to the same group.
    /// </summary>
    public sealed partial class ItemDetailPage : BabyJournal.Common.LayoutAwarePage
    {

        public ItemDetailPage()
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
            JournalItem item = (JournalItem)navigationParameter;
            try
            {
                Init(item);
            }
            catch (Exception) { }
            EnableLiveTile.CreateLiveTile.ShowliveTile(true, "Tiny Years");
            pageTitle.Text = item.Groups;
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }

        void Init(JournalItem sender)
        {
            flipView.Items.Clear();
            int selectedIndex = 0;
            int count = 0;

            foreach (var i in App.AppDataFile.Items[sender.Groups])
            {
                var item = new FlipViewItemDetailPage(i);
                item.Tag = i;
                if (i.ImageUri == sender.ImageUri)
                    selectedIndex = count;
                flipView.Items.Add(item);
                count++;
            }

            try
            {
                flipView.SelectedIndex = selectedIndex;
            }
            catch (Exception) { }
        }

        void OnNew(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AddNewBaby));
        }

        void OnEdit(object sender, RoutedEventArgs e)
        {
            var SelectedItem = flipView.SelectedItem as FlipViewItemDetailPage;
            this.Frame.Navigate(typeof(AddNewBaby), new JournalItem
                {
                    Groups = pageTitle.Text,
                    Title = SelectedItem.Title,
                    Description = SelectedItem.Desc,
                    ImageUri = SelectedItem.ImageUri,
                }
            );            
        }

        async void OnDelete(object sender, RoutedEventArgs e)
        {
            MessageDialog dlg = new Windows.UI.Popups.MessageDialog("Are you sure to remove this child??", "Confirmation");
            UICommand cmdYes = new UICommand("Yes");
            UICommand cmdNo = new UICommand("No");
            dlg.Commands.Add(cmdYes);
            dlg.Commands.Add(cmdNo);
            IUICommand x = await dlg.ShowAsync();
            if (x.Label == "Yes")
            {
                var item = (JournalItem)(flipView.SelectedItem as FlipViewItemDetailPage).Tag;
                int count = App.AppDataFile.Items[item.Groups].Count;
                await App.AppDataFile.RemoveItem(item);
                await App.AppDataFile.WriteData();

                if (count == 1)
                    this.Frame.Navigate(typeof(GroupedItemsPage));
                else
                {
                    Init(new JournalItem { ImageUri = new Uri("ms-appx:///Assets/Logo.png"), });
                }
            }
        }

        async void OnFavourite(object sender, RoutedEventArgs e)
        {
            var item = (JournalItem)(flipView.SelectedItem as FlipViewItemDetailPage).Tag;
            await App.AppDataFile.MarkFavourite(item);

            iFavButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            iUnFavButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        async void OnUnFavourite(object sender, RoutedEventArgs e)
        {
            var item = (JournalItem)(flipView.SelectedItem as FlipViewItemDetailPage).Tag;
            await App.AppDataFile.MarkUnFavourite(item);

            iFavButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
            iUnFavButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void OnFlipViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((JournalItem)(flipView.SelectedItem as FlipViewItemDetailPage).Tag).IsFavourite)
            {
                iFavButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                iUnFavButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                iFavButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
                iUnFavButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }
    }
}
