using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace WebPaper.Plugins
{
    /// <summary>
    /// Plugin Manager class
    /// </summary>
    class PluginManager
    {
        //File names
        string[] dllFileNames = null;

        //Assebly Collection
        ICollection<Assembly> assemblies;

        //Plugins
        Type pluginType = typeof(IPluginable);
        ICollection<Type> pluginTypes = new List<Type>();
        ICollection<IPluginable> plugins;

        public string Query { get; set; }

        /// <summary>
        /// Constructor with option to Load Plugins with provided path
        /// </summary>
        /// <param name="path"></param>
        public PluginManager(string path, string query, int queryCount)
        {
            Query = query;
            LoadPlugins(path, queryCount);
        }

        /// <summary>
        /// Load plugins
        /// </summary>
        /// <param name="path"></param>
        public void LoadPlugins(string path, int queryCount)
        {
            if(Directory.Exists(path))
            {
                dllFileNames = Directory.GetFiles(path, "*.dll");
                assemblies = new List<Assembly>(dllFileNames.Length);
                
                if(dllFileNames.Count() < 1)
                {
                    throw new NoAssemblyException("No Assemblies Found in director!");
                }

                //Iterate through each File name, collect and add assembly
                foreach(string dllFile in dllFileNames)
                {
                    AssemblyName an = AssemblyName.GetAssemblyName(dllFile);
                    Assembly assembly = Assembly.Load(an);
                    assemblies.Add(assembly);
                }

                //Iterate through each assembly and add to plugin types
                foreach(Assembly asm in assemblies)
                {
                    if(asm != null)
                    {
                        Type[] types = asm.GetTypes();
                        foreach(Type type in types)
                        {
                            if(type.IsInterface || type.IsAbstract)
                            {
                                continue;
                            }
                            else
                            {
                                pluginTypes.Add(type);
                            }

                        }
                    }
                }

                plugins = new List<IPluginable>(pluginTypes.Count);

                if (String.IsNullOrEmpty(Query))
                    throw new QueryNotSetException("A Query has not been set!");

                //Iterate through each type and add to plugin list
                foreach(Type type in pluginTypes)
                {
                    if (typeof(IPluginable).IsAssignableFrom(type))
                    {
                        IPluginable plugin = (IPluginable)Activator.CreateInstance(type);
                        plugin.SetQuery(Query, queryCount);
                        plugins.Add(plugin);
                    }
                }
            }
            else
            {
                throw new NoAssemblyException("No Assemblies Found; try checking path.");
            }
        }

        /// <summary>
        /// Returns a Colleciton of plugins
        /// Type parameter dependant on the IPluginable Interface
        /// </summary>
        /// <returns></returns>
        public ICollection<IPluginable> GetPlugins()
        {
            return plugins;
        }
    }

    /// <summary>
    /// No Assembly Exception
    /// </summary>
    [Serializable]
    public class NoAssemblyException : Exception
    {
        public NoAssemblyException() { }
        public NoAssemblyException(string message) : base(message) { }
        public NoAssemblyException(string message, Exception inner) : base(message, inner) { }
        
        protected NoAssemblyException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    /// <summary>
    /// No query set
    /// </summary>
    [Serializable]
    public class QueryNotSetException : Exception
    {
        public QueryNotSetException() { }
        public QueryNotSetException(string message) : base(message) { }
        public QueryNotSetException(string message, Exception inner) : base(message, inner) { }
        protected QueryNotSetException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
