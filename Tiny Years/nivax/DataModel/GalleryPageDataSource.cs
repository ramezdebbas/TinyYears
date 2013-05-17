﻿using BabyJournal.Data;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Specialized;

// The data model defined by this file serves as a representative example of a strongly-typed
// model that supports notification when members are added, removed, or modified.  The property
// names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs.

namespace BabyJournal.Data
{
    /// <summary>
    /// Base class for <see cref="GalleryPageDataItem"/> and <see cref="GalleryPageDataGroup"/> that
    /// defines properties common to both.
    /// </summary>
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class GalleryPageDataCommon : BabyJournal.Common.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");

        public GalleryPageDataCommon(String uniqueId, String title, String subtitle, String imagePath, String description)
        {
            this._uniqueId = uniqueId;
            this._title = title;
            this._subtitle = subtitle;
            this._description = description;
            this._imagePath = imagePath;
        }

        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private string _subtitle = string.Empty;
        public string Subtitle
        {
            get { return this._subtitle; }
            set { this.SetProperty(ref this._subtitle, value); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return this._description; }
            set { this.SetProperty(ref this._description, value); }
        }

        private ImageSource _image = null;
        private String _imagePath = null;
        public ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(new Uri(GalleryPageDataCommon._baseUri, this._imagePath));
                }
                return this._image;
            }

            set
            {
                this._imagePath = null;
                this.SetProperty(ref this._image, value);
            }
        }

        public void SetImage(String path)
        {
            this._image = null;
            this._imagePath = path;
            this.OnPropertyChanged("Image");
        }

        public override string ToString()
        {
            return this.Title;
        }
    }

    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class GalleryPageDataItem : GalleryPageDataCommon
    {
        public GalleryPageDataItem(String uniqueId, String title, String subtitle, String imagePath, String description, String content, int colSpan, int rowSpan, GalleryPageDataGroup group)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            this._colSpan = colSpan;
            this._rowSpan = rowSpan;
            this._content = content;
            this._group = group;
        }

        private string _content = string.Empty;
        public string Content
        {
            get { return this._content; }
            set { this.SetProperty(ref this._content, value); }
        }

        private int _rowSpan = 1;
        public int RowSpan
        {
            get { return this._rowSpan; }
            set { this.SetProperty(ref this._rowSpan, value); }
        }

        private int _colSpan = 1;
        public int ColSpan
        {
            get { return this._colSpan; }
            set { this.SetProperty(ref this._colSpan, value); }
        }


        private GalleryPageDataGroup _group;
        public GalleryPageDataGroup Group
        {
            get { return this._group; }
            set { this.SetProperty(ref this._group, value); }
        }
    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class GalleryPageDataGroup : GalleryPageDataCommon
    {
        public GalleryPageDataGroup(String uniqueId, String title, String subtitle, String imagePath, String description)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            Items.CollectionChanged += ItemsCollectionChanged;
        }

        private void ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Provides a subset of the full items collection to bind to from a GroupedItemsPage
            // for two reasons: GridView will not virtualize large items collections, and it
            // improves the user experience when browsing through groups with large numbers of
            // items.
            //
            // A maximum of 12 items are displayed because it results in filled grid columns
            // whether there are 1, 2, 3, 4, or 6 rows displayed

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        if (TopItems.Count > 12)
                        {
                            TopItems.RemoveAt(12);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    if (e.OldStartingIndex < 12 && e.NewStartingIndex < 12)
                    {
                        TopItems.Move(e.OldStartingIndex, e.NewStartingIndex);
                    }
                    else if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        TopItems.Add(Items[11]);
                    }
                    else if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        TopItems.RemoveAt(12);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        if (Items.Count >= 12)
                        {
                            TopItems.Add(Items[11]);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems[e.OldStartingIndex] = Items[e.OldStartingIndex];
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    TopItems.Clear();
                    while (TopItems.Count < Items.Count && TopItems.Count < 12)
                    {
                        TopItems.Add(Items[TopItems.Count]);
                    }
                    break;
            }
        }

        private ObservableCollection<GalleryPageDataItem> _items = new ObservableCollection<GalleryPageDataItem>();
        public ObservableCollection<GalleryPageDataItem> Items
        {
            get { return this._items; }
        }

        private ObservableCollection<GalleryPageDataItem> _topItem = new ObservableCollection<GalleryPageDataItem>();
        public ObservableCollection<GalleryPageDataItem> TopItems
        {
            get { return this._topItem; }
        }
    }

    /// <summary>
    /// Creates a collection of groups and items with hard-coded content.
    /// 
    /// GalleryPageDataSource initializes with placeholder data rather than live production
    /// data so that sample data is provided at both design-time and run-time.
    /// </summary>
    public sealed class GalleryPageDataSource
    {
        private static GalleryPageDataSource _GalleryPageDataSource = new GalleryPageDataSource();

        private ObservableCollection<GalleryPageDataGroup> _allGroups = new ObservableCollection<GalleryPageDataGroup>();
        public ObservableCollection<GalleryPageDataGroup> AllGroups
        {
            get { return this._allGroups; }
        }

        public static IEnumerable<GalleryPageDataGroup> GetGroups(string uniqueId)
        {
            if (!uniqueId.Equals("AllGroups")) throw new ArgumentException("Only 'AllGroups' is supported as a collection of groups");

            return _GalleryPageDataSource.AllGroups;
        }

        public static GalleryPageDataGroup GetGroup(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _GalleryPageDataSource.AllGroups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static GalleryPageDataItem GetItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _GalleryPageDataSource.AllGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }



        public GalleryPageDataSource()
        {
            String ITEM_CONTENT = String.Format("Item Content: {0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}",
                        "Curabitur class aliquam vestibulum nam curae maecenas sed integer cras phasellus suspendisse quisque donec dis praesent accumsan bibendum pellentesque condimentum adipiscing etiam consequat vivamus dictumst aliquam duis convallis scelerisque est parturient ullamcorper aliquet fusce suspendisse nunc hac eleifend amet blandit facilisi condimentum commodo scelerisque faucibus aenean ullamcorper ante mauris dignissim consectetuer nullam lorem vestibulum habitant conubia elementum pellentesque morbi facilisis arcu sollicitudin diam cubilia aptent vestibulum auctor eget dapibus pellentesque inceptos leo egestas interdum nulla consectetuer suspendisse adipiscing pellentesque proin lobortis sollicitudin augue elit mus congue fermentum parturient fringilla euismod feugiat");

            var group1 = new GalleryPageDataGroup("Group-1",
                 "Group 1",
                 "Group Subtitle: 1",
                 "Assets/DarkGray.png",
                 "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");

            group1.Items.Add(new GalleryPageDataItem("Big-Group-1-Item1",
                 "",
                 "Item Subtitle: 1",
                 "Assets/GalleryPage/Gallerpic.png",
                 "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                 ITEM_CONTENT,
                 105,
                 104,
                 group1));

            group1.Items.Add(new GalleryPageDataItem("Small-Group-1-Item2",
                 "SAMPLE NAME",
                 "Item Subtitle: 2",
                 "Assets/GalleryPage/Gallerpic.png",
                 "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                 ITEM_CONTENT,
                 50,
                 50,
                 group1));

            group1.Items.Add(new GalleryPageDataItem("Small-Group-1-Item3",
                 "SAMPLE NAME",
                 "Item Subtitle: 3",
                 "Assets/GalleryPage/Gallerpic.png",
                 "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                 ITEM_CONTENT,
                 50,
                 50,
                 group1));

            group1.Items.Add(new GalleryPageDataItem("Small-Group-1-Item4",
                 "SAMPLE NAME",
                 "Item Subtitle: 4",
                 "Assets/GalleryPage/Gallerpic.png",
                 "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                 ITEM_CONTENT,
                 50,
                 50,
                 group1));

            group1.Items.Add(new GalleryPageDataItem("Small-Group-1-Item5",
                 "SAMPLE NAME",
                 "Item Subtitle: 5",
                 "Assets/GalleryPage/Gallerpic.png",
                 "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                 ITEM_CONTENT,
                 50,
                 50,
                 group1));
            this.AllGroups.Add(group1);

            //var group2 = new GalleryPageDataGroup("Group-2",
            //    "Group 2",
            //    "Group Subtitle: 2",
            //    "Assets/DarkGray.png",
            //    "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");

            //group2.Items.Add(new GalleryPageDataItem("Big-Group-2-Item1",
            //    "Item Title: 1",
            //    "Item Subtitle: 1",
            //    "Assets/BigTile.jpg",
            //    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
            //    ITEM_CONTENT,
            //    79,
            //    49,
            //    group2));

            //group2.Items.Add(new GalleryPageDataItem("Small-Group-2-Item2",
            //    "Item Title: 2",
            //    "Item Subtitle: 2",
            //    "Assets/SmallTile.jpg",
            //    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
            //    ITEM_CONTENT,
            //    53,
            //    49,
            //    group2));

            //group2.Items.Add(new GalleryPageDataItem("Big-Group-2-Item3",
            //    "Item Title: 3",
            //    "Item Subtitle: 3",
            //    "Assets/BigTile.jpg",
            //    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
            //    ITEM_CONTENT,
            //    79,
            //    49,
            //    group2));

            //group2.Items.Add(new GalleryPageDataItem("Big-Group-2-Item4",
            //    "Item Title: 4",
            //    "Item Subtitle: 4",
            //    "Assets/BigTile.jpg",
            //    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
            //    ITEM_CONTENT,
            //    79,
            //    49,
            //    group2));

            //group2.Items.Add(new GalleryPageDataItem("Big-Group-2-Item5",
            //    "Item Title: 5",
            //    "Item Subtitle: 5",
            //    "Assets/BigTile.jpg",
            //    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
            //    ITEM_CONTENT,
            //    79,
            //    49,
            //    group2));

            //group2.Items.Add(new GalleryPageDataItem("Small-Group-2-Item6",
            //    "Item Title: 6",
            //    "Item Subtitle: 6",
            //    "Assets/SmallTile.jpg",
            //    "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
            //    ITEM_CONTENT,
            //    53,
            //    49,
            //    group2));

            //this.AllGroups.Add(group2);


        }
    }
}
