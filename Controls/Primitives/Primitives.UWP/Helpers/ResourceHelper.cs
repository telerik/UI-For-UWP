using System;
using System.IO;
using System.Reflection;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    internal static class ResourceHelper
    {
        public static ResourceDictionary GetResourceDictionaryByPath(Type type, string resourcePath)
        {
            Assembly assembly = type.GetTypeInfo().Assembly;
            using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
            {
                StreamReader reader = new StreamReader(stream);
                ResourceDictionary dictionary = XamlReader.Load(reader.ReadToEnd()) as ResourceDictionary;
                return dictionary;
            }
        }

        public static object LoadEmbeddedResource(Type type, string resourcePath, object key)
        {
            return GetResourceDictionaryByPath(type, resourcePath)[key];
        }

        public static byte[] LoadManifestStreamBytes(Type type, string resourcePath)
        {
            Assembly assembly = type.GetTypeInfo().Assembly;
            using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
            {
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);

                return bytes;
            }
        }

        public static BitmapImage LoadEmbeddedImageResource(Type type, string resourcePath)
        {
            Assembly assembly = type.GetTypeInfo().Assembly;
            using (var stream = assembly.GetManifestResourceStream(resourcePath))
            {
                MemoryStream buffer = new MemoryStream();
                stream.CopyTo(buffer);
                buffer.Seek(0, SeekOrigin.Begin);

                BitmapImage image = new BitmapImage();
                image.SetSource(buffer.AsRandomAccessStream());

                return image;
            }
        }
    }
}
