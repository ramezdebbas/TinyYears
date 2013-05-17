using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BabyJournal.Data;
using Windows.UI.Xaml.Controls;

namespace BabyJournal.VariableTemplate
{
   public class VariableTileControl : GridView
    {
       protected override void PrepareContainerForItemOverride(Windows.UI.Xaml.DependencyObject element, object item)
       {
           var viewModel = item as GalleryPageDataItem;
           element.SetValue(VariableSizedWrapGrid.ColumnSpanProperty,viewModel.ColSpan);
           element.SetValue(VariableSizedWrapGrid.RowSpanProperty, viewModel.RowSpan);
           base.PrepareContainerForItemOverride(element, item);
       }
    }
}
