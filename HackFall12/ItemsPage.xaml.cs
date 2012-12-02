using HackFall12.Data;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.System;
// The Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234233

namespace HackFall12
{
    /// <summary>
    /// A page that displays a collection of item previews.  In the Split Application this page
    /// is used to display and select one of the available groups.
    /// </summary>
    public sealed partial class ItemsPage : HackFall12.Common.LayoutAwarePage
    {
        private WebRequester webRequester;
        private MovieDataItem foundItem;
        MovieDataSourceTest mainSource;
        public ItemsPage()
        {
            webRequester = new WebRequester("");
            mainSource = new MovieDataSourceTest();
            foundItem = null;
            webRequester.UpdateStatusAction += UpdateStatus;
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
            this.DefaultViewModel["Items"] = MovieDataSourceTest.GetAllItems();
        }

        /// <summary>
        /// Invoked when an item is clicked.
        /// </summary>
        /// <param name="sender">The GridView (or ListView when the application is snapped)
        /// displaying the item clicked.</param>
        /// <param name="e">Event data that describes the item clicked.</param>
        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var groupId = ((MovieDataItem)e.ClickedItem).UniqueId;
            this.Frame.Navigate(typeof(ItemDetailPage), groupId);
        }

        private void itemListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Launcher.LaunchUriAsync(new Uri(((MovieDataItem)e.ClickedItem).URL));
        }

        private void Search_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                if (foundItem == null)
                {
                    string text = SearchBox.Text;
                    if (text != "")
                    {
                        string s = new string(text.ToCharArray().Where(char.IsLetterOrDigit).ToArray());
                        webRequester.GetMovieByName(s);
                        Search_Done(webRequester.Movie);
                    }
                }
                else
                {
                    Add_Movie();
                }
            }
        }
        private void Search_Done(MovieDataItem newItem)
        {
            if (newItem != null)
            {
                searchTitleScroll.Visibility = Windows.UI.Xaml.Visibility.Visible;
                searchDetailScroll.Visibility = Windows.UI.Xaml.Visibility.Visible;
                searchResultImg.Visibility = Windows.UI.Xaml.Visibility.Visible;
                addButton.Visibility = Windows.UI.Xaml.Visibility.Visible;


                searchResultDetail.Text = newItem.Description;
                searchResultTitle.Text = newItem.Title;
                searchResultImg.Source = newItem.Image;
                SearchBox.Text = "";
                foundItem = newItem;
            }

        }

        private void Search_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (SearchBox.Text == "Search Here" || SearchBox.Text.ToLower().Contains("finished") || SearchBox.Text.ToLower().Contains("failed"))
                SearchBox.Text = "";
        }

        private void Search_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (SearchBox.Text == "")
                SearchBox.Text = "Search Here";
        }

        private void Add_Button_Clicked(object sender, RoutedEventArgs e)
        {
            Add_Movie();
        }

        private void Add_Movie()
        {
            if (foundItem != null)
            {
                //TODO hide everything again...
                searchTitleScroll.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                searchDetailScroll.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                searchResultImg.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                addButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                // add this to our watch list
                mainSource.AllItems.Add(foundItem);
            }
            foundItem = null;
        }
        public void UpdateStatus(String update)
        {
            this.SearchBox.Text = update;
        }
    }
}
