using BabyJournal.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Search Contract item template is documented at http://go.microsoft.com/fwlink/?LinkId=234240

namespace BabyJournal
{
    // TODO: Respond to activation for search results
    //
    // The following code could not be automatically added to your application subclass,
    // either because the appropriate class could not be located or because a method with
    // the same name already exists.  Ensure that appropriate code deals with activation
    // by displaying search results for the specified search term.
    //
    //         /// <summary>
    //         /// Invoked when the application is activated to display search results.
    //         /// </summary>
    //         /// <param name="args">Details about the activation request.</param>
    //         protected async override void OnSearchActivated(Windows.ApplicationModel.Activation.SearchActivatedEventArgs args)
    //         {
    //             // TODO: Register the Windows.ApplicationModel.Search.SearchPane.GetForCurrentView().QuerySubmitted
    //             // event in OnWindowCreated to speed up searches once the application is already running
    // 
    //             // If the Window isn't already using Frame navigation, insert our own Frame
    //             var previousContent = Window.Current.Content;
    //             var frame = previousContent as Frame;
    // 
    //             // If the app does not contain a top-level frame, it is possible that this 
    //             // is the initial launch of the app. Typically this method and OnLaunched 
    //             // in App.xaml.cs can call a common method.
    //             if (frame == null)
    //             {
    //                 // Create a Frame to act as the navigation context and associate it with
    //                 // a SuspensionManager key
    //                 frame = new Frame();
    //                 BabyJournal.Common.SuspensionManager.RegisterFrame(frame, "AppFrame");
    // 
    //                 if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
    //                 {
    //                     // Restore the saved session state only when appropriate
    //                      try
    //                     {
    //                         await BabyJournal.Common.SuspensionManager.RestoreAsync();
    //                     }
    //                     catch (BabyJournal.Common.SuspensionManagerException)
    //                     {
    //                         //Something went wrong restoring state.
    //                         //Assume there is no state and continue
    //                     }
    //                 }
    //             }
    // 
    //             frame.Navigate(typeof(SearchResultsPage1), args.QueryText);
    //             Window.Current.Content = frame;
    // 
    //             // Ensure the current window is active
    //             Window.Current.Activate();
    //         }
    /// <summary>
    /// This page displays search results when a global search is directed to this application.
    /// </summary>
    public sealed partial class SearchResultsPage1 : BabyJournal.Common.LayoutAwarePage
    {

        public SearchResultsPage1()
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
            var queryText = navigationParameter as String;

            // TODO: Application-specific searching logic.  The search process is responsible for
            //       creating a list of user-selectable result categories:
            //
            //       filterList.Add(new Filter("<filter name>", <result count>));
            //
            //       Only the first filter, typically "All", should pass true as a third argument in
            //       order to start in an active state.  Results for the active filter are provided
            //       in Filter_SelectionChanged below.

            //var filterList = new List<Filter>();
            //filterList.Add(new Filter("All", 0, true));

            // Communicate results through the view model
            //this.DefaultViewModel["QueryText"] = '\u201c' + queryText + '\u201d';
            //this.DefaultViewModel["Filters"] = filterList;
            //this.DefaultViewModel["ShowFilters"] = filterList.Count > 1;

            //var sampleDataGroups = SampleDataSource.GetGroups("AllGroups");
            //this.DefaultViewModel["Results"] = sampleDataGroups.FirstOrDefault().Items;
            this.queryText.Text = queryText;
            Search(queryText);
        }

        void Search(string query)
        {
            noResultsTextBlock.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            int count = 0;
            List<JournalItem> AllItems = App.AppDataFile.AllItems;

            foreach (var item in AllItems)
            {
                if (item.Title.ToLower().Contains(query))
                {
                    var i = new SearchResultItem(item);
                    i.Tapped += Item_Tapped;
                    i.Tag = item;
                    resultsGridView.Items.Add(i);
                    count++;
                }
            }

            if (count > 0)
                noResultsTextBlock.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            else
                noResultsTextBlock.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        void Item_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ItemDetailPage), (sender as SearchResultItem).Tag);
        }

    }
}
