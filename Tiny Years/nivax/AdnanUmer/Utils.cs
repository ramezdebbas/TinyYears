using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace BabyJournal
{
    class Utils
    {
        public static string SuggestFileName(IReadOnlyList<StorageFile> list)
        {
            int t = list.Count;
            int i;
            List<string> Files = new List<string>();

            for (i = 0; i < t; i++)
                Files.Add(list[i].DisplayName);

            Files.Sort();

            for (i = 0; i < t; i++)
            {
                if (!Files.Contains(i.ToString()))
                    break;
            }

            return i.ToString();
        }
    }
}