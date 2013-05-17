using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace BabyJournal
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class AddNewBaby : BabyJournal.Common.LayoutAwarePage
    {        
        StorageFile ImageFile;
        JournalItem LastRecord;
        
        StorageFile LastImageFile;

        bool IsEdit;

        public AddNewBaby()
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
        protected override async void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            IsEdit = false;
            try
            {
                var item = (JournalItem)navigationParameter;
                iTitle.Text = item.Title;
                iDesc.Text = item.Description;
                iGroupName.Text = item.Groups;
                iBabyImage.Source = new BitmapImage(item.ImageUri);
                IsEdit = true;
                LastRecord = new JournalItem
                {
                    ImageUri = item.ImageUri,
                    Title = item.Title,
                    Description = item.Description,
                    Groups = item.Groups,
                };
                string path = item.ImageUri.AbsolutePath;                
                ImageFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(path.Substring(path.LastIndexOf("/") + 1), CreationCollisionOption.OpenIfExists);
                LastImageFile = ImageFile;
            }
            catch (Exception) { }

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

        async void OnBrowseClick(object sender, RoutedEventArgs e)
        {
            HideError();
            Windows.Storage.Pickers.FileOpenPicker picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".gif");
            try
            {
                StorageFile file1 = await picker.PickSingleFileAsync();
                StorageFile file = await file1.CopyAsync(ApplicationData.Current.LocalFolder, "Temp" + file1.FileType, NameCollisionOption.ReplaceExisting);
                ImageFile = file;
                iBabyImage.Source = new BitmapImage(new Uri(file.Path, UriKind.Absolute));
            }
            catch (Exception) { }
        }

        void OnTextChanged(object sender, RoutedEventArgs e)
        {
            HideError();
        }

        async void OnSaved(object sender, RoutedEventArgs e)
        {
            if (iBabyImage.Source == null)
            {
                ShowError("Select an Image File first.");
                return;
            }

            if (iGroupName.Text == "")
            {
                ShowError("Specify Group Name.");
                return;
            }

            if (iTitle.Text == "")
            {
                ShowError("Specify Title of Child.");
                return;
            }

            if (iDesc.Text == "")
            {
                ShowError("Specify Description of Child.");
                return;
            }

            if (iGroupName.Text.Length < 3)
            {
                ShowError("Group Name must contain Atleast 3 Characters.");
                return;
            }

            if (iTitle.Text.Length < 3)
            {
                ShowError("Title must contain Atleast 3 Characters.");
                return;
            }

           if (iDesc.Text.Length < 1)
            {
                ShowError("Description must not be empty.");
                return;
            }

            if (IsEdit)
            {
                if (ImageFile.DisplayName != LastImageFile.DisplayName)
                    await LastImageFile.DeleteAsync();
            }
            else
            {
                IReadOnlyList<StorageFile> fileList = await ApplicationData.Current.LocalFolder.GetFilesAsync();
                await ImageFile.RenameAsync(Utils.SuggestFileName(fileList) + ImageFile.FileType);
            }

            JournalItem item = new JournalItem();

            item.Title = iTitle.Text.Trim(' ');
            item.Description = iDesc.Text.Trim(' ');
            item.ImageUri = new Uri(ImageFile.Path);
            item.Groups = iGroupName.Text;

            if (IsEdit)
            {
                item.IsFavourite = LastRecord.IsFavourite;
                await App.AppDataFile.EditItem(LastRecord, item);
            }
            else
            {
                item.IsFavourite = false;
                App.AppDataFile.AddItem(item);
            }

            App.AppDataFile.WriteData();
            this.Frame.Navigate(typeof(GroupedItemsPage));
        }

        void ShowError(string Text)
        {
            iInvalid.Text = Text;
            iInvalid.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        void HideError()
        {
            iInvalid.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        async void OnCancel(object sender, RoutedEventArgs e)
        {
            try { await ImageFile.DeleteAsync(); }
            catch (Exception) { }
            this.Frame.Navigate(typeof(GroupedItemsPage));
        }
    }
}