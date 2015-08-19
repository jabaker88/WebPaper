using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Resources;

[assembly:NeutralResourcesLanguage("en")]

namespace WebPaper
{
    class NotifyIconUtility
    {
        //Class Varibles
        private NotifyIcon notifyIcon;
        private System.Drawing.Icon icon;

        //Class Members
        /// <summary>
        /// Double Clicked Event
        /// </summary>
        public event MouseEventHandler DoubleClicked
        {
            add { notifyIcon.MouseDoubleClick += value; }
            remove { notifyIcon.MouseDoubleClick -= value; }
        }

        /// <summary>
        /// On Clicked Event
        /// </summary>
        public event EventHandler Clicked
        {
            add { notifyIcon.Click += value; }
            remove { notifyIcon.Click -= value; }
        }

        /// <summary>
        /// On Mouse Up Event
        /// </summary>
        public event MouseEventHandler OnClickedUp
        {
            add { notifyIcon.MouseUp += value; }
            remove { notifyIcon.MouseUp -= value; }
        }

        /// <summary>
        /// Gets/Sets Context Menu Item
        /// </summary>
        public ContextMenu ContextualMenu 
        {
            get { return notifyIcon.ContextMenu; }
            set { notifyIcon.ContextMenu = value; }
        }

        /// <summary>
        /// Set visiblity of Icon
        /// </summary>
        public bool Visible
        {
            get
            {
                return notifyIcon.Visible;
            }
            set
            {
                notifyIcon.Visible = value;
            }
        }

        /// <summary>
        /// Constructs base icon object
        /// </summary>
        public NotifyIconUtility()
        {
            notifyIcon = new NotifyIcon();
            //icon = System.Drawing.Icon.ExtractAssociatedIcon(@".\res\WebPaperIcon.ico");
            icon = Properties.Resources.WebPaperIcon;
            notifyIcon.Icon = icon;
            notifyIcon.Visible = true;
        }

    }
}
