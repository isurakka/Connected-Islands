using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace LD30
{
    static class ResourceManager
    {
        static Dictionary<string, object> resources = new Dictionary<string, object>();

        public static void LoadResource<T>(string path, string name)
        {
            var type = typeof(T);
            if (type == typeof(Image))
            {
                resources.Add(name, new Image(path));
            }
            else if (type == typeof(Font))
            {
                resources.Add(name, new Font(path));
            }
            else
            {
                throw new ArgumentException("Tried to load resource which doesn't exist.");
            }
        }

        public static void DeriveResource<TSource, TResult>(string sourceName, string resultName, Func<TSource, TResult> derivation)
        {
            var source = (TSource)resources[sourceName];
            var result = derivation(source);
            resources.Add(resultName, result);
        }

        public static T GetResource<T>(string name)
        {
            return (T)resources[name];
        }
    }
}
