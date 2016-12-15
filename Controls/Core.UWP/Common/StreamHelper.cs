using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace Telerik.Core
{
    internal static class StreamHelper
    {
        public static async Task<IRandomAccessStream> AsRandomAccessStreamAsync(this Stream stream)
        {
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);

            InMemoryRandomAccessStream randomAccessStream = new InMemoryRandomAccessStream();
            IOutputStream outputStream = randomAccessStream.GetOutputStreamAt(0);

            DataWriter writer = new DataWriter(outputStream);
            writer.WriteBytes(buffer);

            await writer.StoreAsync();
            await writer.FlushAsync();

            return randomAccessStream;
        }
    }
}
