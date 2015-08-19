using System;
using System.IO;
//using System.Drawing;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

namespace WebPaper.Utilities
{
    public sealed class WinConfigWrapper
    {
        //Flags
        private const int SPI_SETDESKWALLPAPER = 20;
        private const int SPIF_UPDATEINIFILE = 0x01;
        private const int SPIF_SENDWINIINICHANGE = 0x02;

        //Wrap function
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lvpParam, int fuWinIni);

        public enum Style: int
        {
            Tiled, Centered, Stretched, Fill, Span, Fit
        }

        /// <summary>
        /// Sets the Wallpaper only
        /// ONLY CALL AFTER WALLPAPER HAS BEEN SET!
        /// Use SetWallPaper function first
        /// </summary>
        /// <param name="style"></param>
        public static void SetWallPaperStyle(Style style)
        {
            string tempPath = Path.Combine(Path.GetTempPath(), "wallpaper");

            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            if (style == Style.Stretched)
            {
                key.SetValue(@"WallpaperStyle", 2.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }

            if (style == Style.Centered)
            {
                key.SetValue(@"WallpaperStyle", 1.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }

            if (style == Style.Tiled)
            {
                key.SetValue(@"WallpaperStyle", 1.ToString());
                key.SetValue(@"TileWallpaper", 1.ToString());
            }
            if (style == Style.Fill)
            {
                key.SetValue(@"WallpaperStyle", 10.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }
            if (style == Style.Span)
            {
                key.SetValue(@"WallpaperStyle", 22.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }
            if (style == Style.Fit)
            {
                key.SetValue(@"WallpaperStyle", 6.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }

            //Win API Call
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, tempPath, SPIF_UPDATEINIFILE | SPIF_SENDWINIINICHANGE);
        }

        /// <summary>
        /// Sets wallpaper and style
        /// WinowsAPI Call
        /// </summary>
        /// <param name="image"></param>
        /// <param name="style"></param>
        public static void SetWallPaper(BitmapImage image, Style style)
        {
            if (image != null)
            {
                string tempPath = Path.Combine(Path.GetTempPath(), "wallpaper");

                //image.Save(tempPath, System.Drawing.Imaging.ImageFormat.Jpeg)
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();

                try
                {
                    encoder.Frames.Add(BitmapFrame.Create(image));
                }
                catch { }

                using(Stream fs = new FileStream(tempPath, FileMode.Create))
                {
                    encoder.Save(fs);
                }

                SetWallPaperStyle(style);

                //Win API Call
                SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, tempPath, SPIF_UPDATEINIFILE | SPIF_SENDWINIINICHANGE);
            }
        }
    }
}
