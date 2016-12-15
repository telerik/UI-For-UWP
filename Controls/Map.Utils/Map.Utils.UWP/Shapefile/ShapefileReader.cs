using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace Telerik.Geospatial
{
    // TODO: Expose this as public utility class.
    internal partial class ShapefileReader
    {
        internal CancellationTokenSource CancellationTokenSource
        {
            get;
            set;
        }

        public Task<MapShapeModelCollection> Read(IRandomAccessStream shapeStream, IRandomAccessStream dataStream = null, Encoding encoding = null)
        {
            if (shapeStream == null)
            {
                return null;
            }

            this.CancellationTokenSource = new CancellationTokenSource();
            var token = this.CancellationTokenSource.Token;

            var task = Task.Factory.StartNew<Task<MapShapeModelCollection>>(() => this.ProcessShapefile(shapeStream, this.CancellationTokenSource), token).Unwrap();
            if (dataStream != null)
            {
                task = task.ContinueWith((previousTask) => this.ProcessDataFile(previousTask.Result, dataStream, this.CancellationTokenSource, encoding), token).Unwrap();
            }

            return task;
        }

        public void CancelOperation()
        {
            if (this.CancellationTokenSource == null)
            {
                return;
            }

            this.CancellationTokenSource.Cancel();
            this.CancellationTokenSource = null;
        }

        private static int ToInt32BigEndian(byte[] buffer, int startIndex)
        {
            return (buffer[startIndex] << 24) | (buffer[startIndex + 1] << 16) | (buffer[startIndex + 2] << 8) | buffer[startIndex + 3];
        }
    }
}
