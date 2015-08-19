using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace WebPaper.Utilities
{
    public abstract class ImageFromInternet
    {
        /// <summary>
        /// Set/Get Image
        /// </summary>
        public BitmapImage Image
        {
            get;
            set;
        }

        /// <summary>
        /// Default ctor
        /// </summary>
        public ImageFromInternet()
        {
            Image = null;
        }

        /// <summary>
        /// Initilizes image from the web
        /// </summary>
        /// <param name="url"></param>
        public ImageFromInternet(string url)
        {
            try
            {
                Image = DownloadImageFromWeb(url);
            }
            catch(WebException)
            {
                throw;
            }
            catch(Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Initilizes image from a provided URL
        /// </summary>
        /// <param name="url"></param>
        public ImageFromInternet(Uri url) : this(url.AbsolutePath) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static BitmapImage DownloadImageFromWeb(Uri url)
        {
            return DownloadImageFromWeb(url.AbsolutePath);
        }

        static void client_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            BitmapImage image = new BitmapImage();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static BitmapImage DownloadImageFromWeb(string url)
        {
            //Web Client & Image
            WebClient client = new WebClient();
            BitmapImage image = new BitmapImage();

            //Sets image from URL, uses await modifier from memory stream during download
            try
            {
                image.BeginInit();
                image.StreamSource = new MemoryStream(client.DownloadData(url));
                image.EndInit();

                return image;
            }
            catch (WebException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
