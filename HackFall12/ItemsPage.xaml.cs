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
        private MovieDataSourceTest mainSource;
        private bool controlDown;
        private bool searching;
        public ItemsPage()
        {
            controlDown = false;
            webRequester = new WebRequester("http://twilio.nints.com:8885");
            mainSource = new MovieDataSourceTest();
            foundItem = null;
            webRequester.UpdateStatusAction += UpdateStatus;
            webRequester.RequestFinishedCallback += ReqComplete;
            this.KeyUp +=ItemsPage_KeyUp;
            this.KeyDown +=ItemsPage_KeyDown;
            this.InitializeComponent();
        }

        private void ItemsPage_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Shift)
            {
                controlDown = true;
            }
        }

        private void ItemsPage_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Shift)
            {
                controlDown = false;
            }
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
            var itemId = ((MovieDataItem)e.ClickedItem).UniqueId;
            var item = ((MovieDataItem)e.ClickedItem);
            if (controlDown)
            {
                MovieDataSourceTest.RemoveItem(item);
                //1this.mainSource.AllItems.Remove((MovieDataItem)e.ClickedItem);
            }
            else
            {
                // Navigate to the appropriate destination page, configuring the new page
                // by passing required information as a navigation parameter
                
                this.Frame.Navigate(typeof(ItemDetailPage), itemId);
            }
        }

        private void itemListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Launcher.LaunchUriAsync(new Uri(((MovieDataItem)e.ClickedItem).URL));
        }

        private void Search_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter && !searching)
            {
                if (foundItem == null)
                {
                    searching = true;
                    string text = SearchBox.Text;
                    if (text != "")
                    {
                        string s = new string(text.ToCharArray().Where(char.IsLetterOrDigit).ToArray());
                        webRequester.GetMovieByName(s);
                    }
                }
                else
                {
                    Add_Movie();
                }
            }
        }

        public void ReqComplete()
        {
            Search_Done(webRequester.Movie);
            searching = false;
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
                MovieDataSourceTest.AddItem(foundItem);
            }
            foundItem = null;
            searching = false;
        }
        public void UpdateStatus(String update)
        {
            this.SearchBox.Text = update;
        }

    }
}
