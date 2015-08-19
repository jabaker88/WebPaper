using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace WebPaper.Plugins
{
    /// <summary>
    /// IPluginable Interface, only needed to  provide list of images
    /// Use only when you intend to return a list of URLs/Strings 
    /// and provide own implenetation; otherwise use ImageProvier
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPluginable
    {
        IList GetMediaList();
        void SetQuery(string query, int count);
        void AdvanceQuery(int count);
    }

    /// <summary>
    /// Abstract base class used for image providers
    /// </summary>
    public abstract class ImageProvider : IPluginable
    {
        protected String query;
        protected IEnumerator imageIterator;
        protected IList innerCollection;

        /// <summary>
        /// Sets the query paramater
        /// </summary>
        /// <param name="query"></param>
        public virtual void SetQuery(string query, int count)
        {
            this.query = query;
        }

        public abstract void AdvanceQuery(int count);

        /// <summary>
        /// List Indexer
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public object this[int index]
        {
            get
            {
                return innerCollection[index];
            }
        }

        /// <summary>
        /// Returns an iterator and jumps to the next query by skip amount
        /// </summary>
        /// <param name="skipAmount"></param>
        /// <returns></returns>
        public abstract IEnumerator NextQuery(int skipAmount);

        /// <summary>
        /// Returns an Iterator
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerator GetEnumerator();

        public IList GetMediaList()
        {
            return innerCollection;
        }
    }
}
