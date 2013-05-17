using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Storage;

namespace BabyJournal
{
    public struct JournalItem
    {
        public Uri ImageUri;
        public string Title;
        public string Description;
        public string Groups;
        public bool IsFavourite;        
    }

    public class AppData
    {
        public List<string> Groups;
        public Dictionary<string, List<JournalItem>> Items;
        public List<JournalItem> Favourites;
        public List<JournalItem> AllItems;

        StorageFile groupFile;
        StorageFolder roamingFolder = ApplicationData.Current.RoamingFolder;

        public AppData()
        {
            Groups = new List<string>();
            Items = new Dictionary<string, List<JournalItem>>();
            Favourites = new List<JournalItem>();
            AllItems = new List<JournalItem>();            
        }

        public async Task ReadData()
        {
            await ReadGroups();

            foreach (var group in Groups)
            {
                var GroupItems = ReadItems(group);

                foreach (var item in GroupItems)
                {
                    if (item.IsFavourite)
                    {
                        Favourites.Add(item);
                    }
                    AllItems.Add(item);
                }

                Items.Add(group, GroupItems);
            }
        }

        public async Task WriteData()
        {
            foreach (var group in Groups)
            {
                await WriteItem(group, Items[group]);
            }
            
            await WriteGroups(Groups);
        }

        public void AddItem(JournalItem item)
        {
            if (!Items.ContainsKey(item.Groups))
            {
                Items.Add(item.Groups, new List<JournalItem>());
                Groups.Add(item.Groups);
            }
            Items[item.Groups].Add(item);
            AllItems.Add(item);
        }

        public async Task EditItem(JournalItem Old, JournalItem New)
        {
            await _RemoveItem(Old, false);
            AddItem(New);
        }

        public async Task MarkFavourite(JournalItem item)
        {
            if (item.IsFavourite)
                return;
            await _RemoveItem(item, false);
            item.IsFavourite = true;
            AddItem(item);
            if (!Favourites.Contains(item))
            {
                Favourites.Add(item);
                WriteData();
            }
        }

        public async Task MarkUnFavourite(JournalItem item)
        {
            if (!item.IsFavourite)
                return;
            await _RemoveItem(item, false);
            item.IsFavourite = false;
            AddItem(item);

            int t = Favourites.Count;

            for (int i = 0; i < t; i++)
            {
                if (Favourites[i].ImageUri == item.ImageUri)
                {
                    Favourites.RemoveAt(i);
                    break;
                }
            }

            WriteData();
        }

        public async Task RemoveItem(JournalItem item)
        {
            if (Favourites.Contains(item))
            {
                int t = Favourites.Count;

                for (int i = 0; i < t; i++)
                {
                    if (Favourites[i].ImageUri == item.ImageUri)
                    {
                        Favourites.RemoveAt(i);
                        break;
                    }
                }
            }
            await _RemoveItem(item, true);
        }

        async Task _RemoveItem(JournalItem item, bool DeleteImage)
        {
            var GroupItem = Items[item.Groups];
            int t = GroupItem.Count;

            for (int i = 0; i < t; i++)
            {
                if (GroupItem[i].ImageUri == item.ImageUri)
                {
                    if (DeleteImage)
                    {
                        string path = item.ImageUri.AbsolutePath;
                        await (await ApplicationData.Current.LocalFolder.CreateFileAsync(path.Substring(path.LastIndexOf("/") + 1), CreationCollisionOption.OpenIfExists)).DeleteAsync();
                    }
                    GroupItem.RemoveAt(i);
                    break;
                }
            }
            if (GroupItem.Count > 0)
                Items[item.Groups] = GroupItem;
            else
            {
                StorageFile itemFile = await roamingFolder.CreateFileAsync(item.Groups + ".xml", CreationCollisionOption.ReplaceExisting);
                await itemFile.DeleteAsync();
                Items.Remove(item.Groups);
                RemoveGroup(item.Groups);
            }
        }

        public void RemoveGroup(string GroupName)
        {
            Groups.Remove(GroupName);
        }

        async Task WriteGroups(List<string> groups)
        {
            groupFile = await roamingFolder.CreateFileAsync("GroupNames.txt.tmp", CreationCollisionOption.OpenIfExists);
            await FileIO.WriteLinesAsync(groupFile, groups);
            await groupFile.RenameAsync("GroupNames.txt", NameCollisionOption.ReplaceExisting);
        }

        async Task ReadGroups()
        {
            groupFile = await roamingFolder.CreateFileAsync("GroupNames.txt", CreationCollisionOption.OpenIfExists);
            try
            {
                string text = await FileIO.ReadTextAsync(groupFile);
                Groups.AddRange(text.Replace("\r", "").Split('\n'));
                Groups.RemoveAt(Groups.Count - 1);
            }
            catch (Exception) { }
        }

        async Task WriteItem(string Group, List<JournalItem> Lines)
        {
            StorageFile itemFile = await roamingFolder.CreateFileAsync(Group + ".xml.tmp", CreationCollisionOption.ReplaceExisting);

            string content = "<?xml version=\"1.0\" encoding=\"utf-8\" ?><group>";

            foreach (var item in Lines)
            {
                content += "<child><title>" + item.Title + "</title>" +
                    "<fav>" + item.IsFavourite.ToString() + "</fav>" +
                    "<desc>" + item.Description.Replace("<", "#1").Replace(">", "#2") + "</desc>" +
                    "<image>" + item.ImageUri.ToString() + "</image></child>\r\n";
            }

            await FileIO.WriteTextAsync(itemFile, content + "</group>");
            await itemFile.RenameAsync(Group + ".xml", NameCollisionOption.ReplaceExisting);
        }

        List<JournalItem> ReadItems(string GroupName)
        {
            string XMLPath = Path.Combine(ApplicationData.Current.RoamingFolder.Path, GroupName + ".xml");
            List<JournalItem> Items = new List<JournalItem>();

            try
            {
                XDocument loadedData = XDocument.Load(XMLPath);

                var data = from query in loadedData.Descendants("child")
                           select new JournalItem
                           {
                               Title = (string)query.Element("title"),
                               IsFavourite = (bool)query.Element("fav"),
                               Description = ((string)query.Element("desc")).Replace("#1", "<").Replace("#2", ">"),
                               ImageUri = new Uri((string)query.Element("image")),
                               Groups = GroupName,
                           };
                Items.AddRange(data);
            }
            catch (Exception) { }

            return Items;
        }
    }
}