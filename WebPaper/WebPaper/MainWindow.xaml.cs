using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Threading;

using WebPaper.Utilities;
using WebPaper.Plugins;
using System.Net;

namespace WebPaper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Class Constants
        const string PLUGINS_PATH = "plugins";
        int DEFAULT_TIME = 60 * 1; //seconds, min

        //Flags
        public bool RESET_IMAGE_FLAG { get; set; }

        //Class Objects
        NotifyIconUtility myIcon; //Notification icon
        System.Windows.Forms.ContextMenu contextMenu; //Context menu for myIcon
        PluginManager pluginManager;
        ICollection<IPluginable> pluginsCollection;

        //Images to be downloaded
        IList<string> mediaList = new List<string>();
        static IEnumerator<string> mediaItr;
        private string query; //Query/search term
        private string previousQuery;
        private BitmapImage image1, image2;

        //Image counter
        static int persistentImageCount = 0;
        const int IMAGE_MAX = 2;  //Leave at 2
        const int MEDIA_MAX = 99;
        
        //Timer Event
        DispatcherTimer timer = new DispatcherTimer();

        //Paper Styles
        WinConfigWrapper.Style wallpaperStyle;

        //Control Panel
        ControlPanel controlPanel;

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initialize Items on startup
        /// </summary>
        private void Init()
        {
            //Get the media iterator
            mediaItr = mediaList.GetEnumerator();

            //Setup Notify Icon
            this.Visibility = Visibility.Hidden;
            myIcon = new NotifyIconUtility();
            myIcon.OnClickedUp += myIcon_Clicked;

            //Context Menu
            contextMenu = new System.Windows.Forms.ContextMenu();
            contextMenu.MenuItems.Add("Next Image").Click += NextContextMenu;
            contextMenu.MenuItems.Add("Exit").Click += ExitContextMenu;
            myIcon.ContextualMenu = contextMenu;

            //Setup control panel
            controlPanel = new ControlPanel();
            controlPanel.Deactivated += controlPanel_Deactivated;

            //Set inital Query to Wallpaper
            query = "wallpaper";
            previousQuery = query;

            //Load Plugins
            LoadPlugins(0);

            //Load images on intial run
            LoadImages();

            //Setup Timer
            timer.Interval = new TimeSpan(0, 0, 0, DEFAULT_TIME);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        //On Timer event
        void timer_Tick(object sender, EventArgs e)
        {
            //Simulate like clicking next image
            nextImage_MouseLeftButtonUp(sender, null);
        }

        /// <summary>
        /// First Load
        /// </summary>
        private void LoadImages()
        {
            mediaItr = mediaList.GetEnumerator();

            mediaItr.MoveNext();
            string imageA = mediaItr.Current;
            mediaItr.MoveNext();
            string imageB = mediaItr.Current;

            for (int i = 0; i < IMAGE_MAX; i++ )
                persistentImageCount++;

            AdvanceDownloadAsync(imageA, imageB);
        }


        //ONLY USE ON INTIAL LOAD
        //Advance Images Download method, params act as tuple
        private void AdvanceDownloadAsync(string url1, string url2)
        {
            WebClient clientA = new WebClient();
            WebClient clientB = new WebClient();
            clientA.DownloadDataCompleted += AdvanceDownloadCompleteA;
            clientB.DownloadDataCompleted += AdvanceDownloadCompleteB;
            try
            {
                clientA.DownloadDataAsync(new Uri(url1));
                clientB.DownloadDataAsync(new Uri(url2));
            }
            catch (Exception ex)
            {
                //General catch all
                //Silently fail
            }

        }

        //Advance Images Download Completed Event
        private void AdvanceDownloadCompleteA(object sender, DownloadDataCompletedEventArgs e)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = new MemoryStream(e.Result);
            image.EndInit();

            image1 = image;
            currentImage.Source = image1;
            WinConfigWrapper.SetWallPaper(image, wallpaperStyle);
        }

        private void AdvanceDownloadCompleteB(object sender, DownloadDataCompletedEventArgs e)
        {
            try {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = new MemoryStream(e.Result);
                image.EndInit();

                image2 = image;

            }  
            catch(Exception ex)
            {

            }      
         }

        //Advance next image only
        private void AdvanceDownloadAsync(string url)
        {
            WebClient client = new WebClient();
            client.DownloadDataCompleted += AdvanceDownloadComplete;

            WinConfigWrapper.SetWallPaper(image2, wallpaperStyle);

            try
            {
                client.DownloadDataAsync(new Uri(url));
            }
            catch (Exception ex)
            {
                //General catch all
                //Silently fail
            }
        }

        private void AdvanceDownloadComplete(object sender, DownloadDataCompletedEventArgs e)
        {
            try
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = new MemoryStream(e.Result);
                image.EndInit();

                image2 = image;
                nextImage.Source = image2;
            }
            catch(Exception ex)
            {

            }
        }

        /// <summary>
        /// Advance Image
        /// </summary>
        /// <returns></returns>
        private void AdvanceImages()
        {
            //Clear and reload master media list and reload plugins 
            if (persistentImageCount >= MEDIA_MAX || RESET_IMAGE_FLAG)
            {
                mediaList.Clear();
                LoadPlugins(persistentImageCount);
                mediaItr = mediaList.GetEnumerator();
                RESET_IMAGE_FLAG = false;
            }

            persistentImageCount++;
            currentImage.Source = nextImage.Source;

            mediaItr.MoveNext();
            AdvanceDownloadAsync(mediaItr.Current);

            currentImage.Source = nextImage.Source;
        }

        //Load plugins found in the plugin directory
        public void LoadPlugins(int queryCount)
        {

            //Check to see if plugins directory exists, if not; create it
            if (!Directory.Exists(PLUGINS_PATH))
            {
                Directory.CreateDirectory(PLUGINS_PATH);
            }

            //Initilze plugin manager
            try
            {
                pluginManager = new Plugins.PluginManager(PLUGINS_PATH, query, persistentImageCount);
                pluginsCollection = pluginManager.GetPlugins();

                //For each of the plugins, retrive the media list
                foreach (IPluginable plugin in pluginsCollection)
                {
                    plugin.SetQuery(query, persistentImageCount);
                    
                    foreach (string media in plugin.GetMediaList())
                    {
                        mediaList.Add(media);
                    }
                }
            }
            catch (NoAssemblyException ex)
            {
                //Log error message
            }
            catch (QueryNotSetException ex)
            {
                //Log
            }
            catch
            {
                //General Catch
            }
        }

        //Notfiy Icon is Clicked; 
        void myIcon_Clicked(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                //Brings the form into imediate view and focus
                this.Activate();
                this.Visibility = Visibility.Visible;
                this.Left = System.Windows.Forms.Cursor.Position.X - (this.Width / 2);
                this.Top = System.Windows.Forms.Cursor.Position.Y - (this.Height + 20);
            }
        }

        //Exit Context Menu
        private void ExitContextMenu(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        //Next Image Context Menu
        private void NextContextMenu(object sender, EventArgs e)
        {
            nextImage_MouseLeftButtonUp(sender, null);
        }

        //On Closing
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Hide on closing
            myIcon.Visible = false;
        }

        //Window Deactivated Event
        private void Window_Deactivated(object sender, EventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        //ExitButton Click Event Handler
        private void ExitButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
           this.Visibility = Visibility.Hidden;
        }

        //Move Next Image
        private void nextImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AdvanceImages();
        }

        //Configure Event
        private void ConfigureButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            controlPanel.Left = System.Windows.Forms.Cursor.Position.X - (controlPanel.Width);
            controlPanel.Top = System.Windows.Forms.Cursor.Position.Y - (controlPanel.Height); 
            controlPanel.Show();
        }

        //Control panel is deactivated
        void controlPanel_Deactivated(object sender, EventArgs e)
        {
            //Get config info from control panel on deactivation
            query = controlPanel.SearchTerm;
            timer.Interval = new TimeSpan(0, 0, controlPanel.PictureTimeout);
            wallpaperStyle = controlPanel.PictureStyle;
            WinConfigWrapper.SetWallPaperStyle(wallpaperStyle);

            //If the query has changed then reload
            if(previousQuery != query)
            {
                RESET_IMAGE_FLAG = true;
                persistentImageCount = 0;
                AdvanceImages();
                previousQuery = query;
            }
        }

        //On window loading
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
        }
    }
}
