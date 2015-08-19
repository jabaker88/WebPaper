using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebPaper.Plugins;

namespace WebPaper.Utilities
{
    /// <summary>
    /// Wrapping class used for iteration of image url locations
    /// </summary>
    public class BingSearchProvider : ImageProvider, IPluginable
    {
        //Bing specific elements
        private const string AccountKey = "ENTER YOUR BING SEARCH API ACCOUNT KEY";
        private ImageSearch.BingSearchContainer bingContainer;
        private IEnumerator<ImageSearch.ImageResult> bingImageResults;

        //Events
        
        public BingSearchProvider()
        {
            bingContainer = new ImageSearch.BingSearchContainer(new Uri("https://api.datamarket.azure.com/Bing/Search/"));
            bingContainer.Credentials = new System.Net.NetworkCredential(AccountKey, AccountKey);

            innerCollection = new List<string>();
        }

        /// <summary>
        /// Constructor, used to specifiy search
        /// </summary>
        /// <param name="search"></param>
        public BingSearchProvider(string search) : this()
        {
            //Set the query, Image search type
            this.query = search;
        }

        /// <summary>
        /// Must be set before query is performed
        /// </summary>
        /// <param name="query"></param>
        public override void SetQuery(string query, int count)
        {
            base.SetQuery(query, count);
            AdvanceQuery(count);
        }

        public override void AdvanceQuery(int count)
        {
            bingImageResults = bingContainer.Image(query, null, "en-us", null, null, null, null)
                .AddQueryOption("$skip", count).Execute().GetEnumerator(); ;

            SetIterator();

        }

        /// <summary>
        /// Used to convert bing image results to a collection iterator
        /// </summary>
        private void SetIterator()
        {
            //Grab bing results and push them into a collection of media url strings (Image locations)
            while(bingImageResults.MoveNext())
            {
                innerCollection.Add(bingImageResults.Current.MediaUrl);
            }

            //Set the image iterator from collection
            imageIterator = innerCollection.GetEnumerator();
            imageIterator.MoveNext();
        }

        /// <summary>
        /// Skips to the query amount
        /// </summary>
        /// <param name="skipAmount"></param>
        /// <returns></returns>
        public override IEnumerator NextQuery(int  skipAmount)
        {
            bingImageResults = bingContainer.Image(query, null, "en-us", null, null, null, null)
                .AddQueryOption("$skip", skipAmount).Execute().GetEnumerator(); ;

            SetIterator();

            return imageIterator;
        }

        /// <summary>
        /// Returns a collection of strings, used for further processing and downloading images
        /// </summary>
        /// <returns></returns>
        public override IEnumerator GetEnumerator()
        {
            return this.imageIterator;
        }
    }
}
