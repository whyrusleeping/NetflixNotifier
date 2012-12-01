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

namespace HackFall12.Data
{
    /// <summary>
    /// Base class for <see cref="MovieDataItem"/> and <see cref="MovieDataGroup"/> that
    /// defines properties common to both.
    /// </summary>
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class MovieDataCommon : HackFall12.Common.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");

        public MovieDataCommon(String uniqueId, String url, String title, String rating, String imagePath, String description, List<String> actors)
        {
            this._url = url;
            this._uniqueId = uniqueId;
            this._title = title;
            this._rating= rating;
            this._description = description;
            this._imagePath = imagePath;
            this._actors = actors;
        }
        private string _url = string.Empty;
        public string URL
        {
            get { return this._url; }
            set { this.SetProperty(ref this._url, value); }
        }
        private List<string> _actors = new List<string>();
        public List<string> Actors
        {
            get { return this._actors; }
            set { this.SetProperty(ref this._actors, value); }
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

        private string _rating = string.Empty;
        public string Rating
        {
            get { return this._rating; }
            set { this.SetProperty(ref this._rating, value); }
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
                    this._image = new BitmapImage(new Uri(MovieDataCommon._baseUri, this._imagePath));
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
    public class MovieDataItem : MovieDataCommon
    {
        public MovieDataItem(String uniqueId, String url, String title, String rating, String imagePath, String description, List<String> actors)
            : base(uniqueId, url, title, rating, imagePath, description, actors)
        {

        }

    }

    /// <summary>
    /// Creates a collection of groups and items with hard-coded content.
    /// 
    /// SampleDataSource initializes with placeholder data rather than live production
    /// data so that sample data is provided at both design-time and run-time.
    /// </summary>
    public class MovieDataSource
    {
        private static MovieDataSource _movieDataSource = new MovieDataSource();
        private ObservableCollection<MovieDataItem> _allItems = new ObservableCollection<MovieDataItem>();
        public ObservableCollection<MovieDataItem> AllItems
        {
            get { return this._allItems; }
        }
        public static IEnumerable<MovieDataItem> GetItems(string uniqueId)
        {
            try
            {
                if (!uniqueId.Equals("AllItems"))
                    throw new ArgumentException("Only 'AllItems' is supported as a collection of groups");

                return _movieDataSource.AllItems;
            }
            catch (ArgumentException)
            {
                return null;
            }
            
        }

        public static IEnumerable<MovieDataItem> GetAllItems()
        {
            return _movieDataSource.AllItems;
        }

        public static MovieDataItem GetItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _movieDataSource.AllItems.Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }
        public MovieDataSource()
        {

        }

    }

    public sealed class MovieDataSourceTest
    {

        private static MovieDataSourceTest _movieDataSource = new MovieDataSourceTest();

        private ObservableCollection<MovieDataItem> _allItems = new ObservableCollection<MovieDataItem>();
        public ObservableCollection<MovieDataItem> AllItems
        {
            get { return this._allItems; }
        }


        public static IEnumerable<MovieDataItem> GetItems(string uniqueId)
        {
            if (!uniqueId.Equals("AllItems")) 
                throw new ArgumentException("Only 'AllItems' is supported as a collection of groups");

            return _movieDataSource.AllItems;
        }
        public static IEnumerable<MovieDataItem> GetAllItems()
        {
            return _movieDataSource.AllItems;
        }

        public static MovieDataItem GetItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _movieDataSource.AllItems.Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }


        public MovieDataSourceTest()
        {
            String ITEM_CONTENT = String.Format("Item Content: {0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}",
                        "Curabitur class aliquam vestibulum nam curae maecenas sed integer cras phasellus suspendisse quisque donec dis praesent accumsan bibendum pellentesque condimentum adipiscing etiam consequat vivamus dictumst aliquam duis convallis scelerisque est parturient ullamcorper aliquet fusce suspendisse nunc hac eleifend amet blandit facilisi condimentum commodo scelerisque faucibus aenean ullamcorper ante mauris dignissim consectetuer nullam lorem vestibulum habitant conubia elementum pellentesque morbi facilisis arcu sollicitudin diam cubilia aptent vestibulum auctor eget dapibus pellentesque inceptos leo egestas interdum nulla consectetuer suspendisse adipiscing pellentesque proin lobortis sollicitudin augue elit mus congue fermentum parturient fringilla euismod feugiat");

            this.AllItems.Add(new MovieDataItem("LOTR", "http://en.wikipedia.org/wiki/Main_Page", "LOTR", "*****", "Assets/60004480.jpg", "A Very long movie", null));

            this.AllItems.Add(new MovieDataItem("BSG", "http://en.wikipedia.org/wiki/Main_Page", "Battlestar", "*****", "Assets/595265.jpg", "Good show", null));
        }
    }
}
