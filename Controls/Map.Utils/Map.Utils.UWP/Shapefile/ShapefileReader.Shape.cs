using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace Telerik.Geospatial
{
    internal partial class ShapefileReader
    {
        public ICoordinateValueConverter CoordinateValueConverter
        {
            get;
            set;
        }

        private static Task<MapShapeModelCollection> ParseShapes(object state)
        {
            ParseShapeTaskState taskState = (ParseShapeTaskState)state;
            var cancellationTokenSource = taskState.CancellationTokenSource;
            var token = cancellationTokenSource.Token;

            var task = Task.Factory.StartNew(
                async () =>
            {
                using (var shapeStream = taskState.Stream.CloneStream())
                {
                    int start = taskState.Start;
                    int end = taskState.End;
                    var coordinateConverter = taskState.CoordinateConverter;
                    var records = taskState.Records;

                    MapShapeModelCollection shapeModels = new MapShapeModelCollection();

                    for (int i = start; i < end; i++)
                    {
                        if (cancellationTokenSource.IsCancellationRequested)
                        {
                            return null;
                        }

                        int contentOffset = records[i].ContentOffset;
                        shapeStream.Seek((ulong)contentOffset);

                        int contentLength = records[i].ContentLength;
                        byte[] content = new byte[contentLength];

                        await shapeStream.ReadAsync(content.AsBuffer(), (uint)contentLength, InputStreamOptions.Partial);

                        EsriShapeType shapeType = (EsriShapeType)BitConverter.ToInt32(content, 0);
                        if (shapeModels.Type == EsriShapeType.NullShape)
                        {
                            shapeModels.Type = shapeType;
                        }

                        // TODO: Consider specific support for "M" types (PointM, MultipointM, PolygonM) that support measure coordinate -- is this the Bubble visualization?.
                        switch (shapeType)
                        {
                            case EsriShapeType.NullShape:
                                shapeModels.Add(new NullShapeModel());
                                break;
                            case EsriShapeType.Point:
                            case EsriShapeType.PointM:
                            case EsriShapeType.PointZ:
                                CreatePoint(content, shapeModels, 4, coordinateConverter);
                                break;

                            case EsriShapeType.Polyline:
                            case EsriShapeType.PolylineM:
                            case EsriShapeType.PolylineZ:
                                CreatePolyline(content, shapeModels, coordinateConverter);
                                break;

                            case EsriShapeType.Polygon:
                            case EsriShapeType.PolygonM:
                            case EsriShapeType.PolygonZ:
                                CreatePolygon(content, shapeModels, coordinateConverter);
                                break;

                            default:
                                throw new NotSupportedException("The supported ESRI shape types are Point(MZ), Polyline(MZ), and Polygon(MZ).");
                        }
                    }

                    return shapeModels;
                }
            },
            token).Unwrap();

            return task;
        }

        private static void CreatePoint(byte[] content, MapShapeModelCollection shapeModels, int pointOffset, ICoordinateValueConverter coordinateConverter)
        {
            double longitude = BitConverter.ToDouble(content, pointOffset);
            double latitude = BitConverter.ToDouble(content, pointOffset + 8);

            var location = new Location(latitude, longitude);

            // NOTE: Optionally allow the user to re-project coordinates to different system.
            if (coordinateConverter != null)
            {
                location = coordinateConverter.Convert(location);
            }

            shapeModels.Add(new MapPointModel() { Location = location });
        }

        private static void CreatePoints(byte[] content, MapShapeModelCollection shapeModels, ICoordinateValueConverter coordinateConverter)
        {
            int numberOfPoints = BitConverter.ToInt32(content, 36);
            int pointOffset;
            for (int i = 0; i < numberOfPoints; i++)
            {
                // NOTE: Points field starts at Byte 40 and consists of [x,y] double pair (16 bytes)
                pointOffset = 40 + 16 * i;

                CreatePoint(content, shapeModels, pointOffset, coordinateConverter);
            }
        }

        private static void CreatePolyline(byte[] content, MapShapeModelCollection shapeModels, ICoordinateValueConverter coordinateConverter)
        {
            shapeModels.Add(new MapPolylineModel() { Locations = GetPoints(content, coordinateConverter) });
        }

        private static void CreatePolygon(byte[] content, MapShapeModelCollection shapeModels, ICoordinateValueConverter coordinateConverter)
        {
            shapeModels.Add(new MapPolygonModel() { Locations = GetPoints(content, coordinateConverter) });
        }

        private static Collection<LocationCollection> GetPoints(byte[] content, ICoordinateValueConverter coordinateConverter)
        {
            int numberOfParts = BitConverter.ToInt32(content, 36);
            int numberOfPoints = BitConverter.ToInt32(content, 40);

            int startPoints = 44 + 4 * numberOfParts;
            Collection<LocationCollection> collection = new Collection<LocationCollection>();

            int pointsCount, partFirstPointIndex, startPoint;
            for (int partIndex = 0; partIndex < numberOfParts; partIndex++)
            {
                partFirstPointIndex = BitConverter.ToInt32(content, 44 + 4 * partIndex);

                // Each point is represented by x,y double coordinates -- 16 bytes 
                startPoint = startPoints + partFirstPointIndex * 16;

                if (partIndex < numberOfParts - 1)
                {
                    pointsCount = BitConverter.ToInt32(content, 44 + 4 * (partIndex + 1)) - partFirstPointIndex;
                }
                else
                {
                    pointsCount = numberOfPoints - partFirstPointIndex;
                }

                LocationCollection locations = GetPoints(content, startPoint, pointsCount, coordinateConverter);

                collection.Add(locations);
            }

            return collection;
        }

        private static LocationCollection GetPoints(byte[] content, int startPoint, int count, ICoordinateValueConverter coordinateConverter)
        {
            int point;
            double longitude, latitude;
            Location location;

            LocationCollection locations = new LocationCollection();
            for (int pointIndex = 0; pointIndex < count; pointIndex++)
            {
                point = startPoint + pointIndex * 16;
                longitude = BitConverter.ToDouble(content, point);
                latitude = BitConverter.ToDouble(content, point + 8);
                location = new Location() { Latitude = latitude, Longitude = longitude };

                // NOTE: Optionally allow the user to re-project coordinates to different system.
                if (coordinateConverter != null)
                {
                    location = coordinateConverter.Convert(location);
                }

                locations.Add(location);
            }

            return locations;
        }

        private static LocationRect GetBoundingRect(byte[] boundingBox, ICoordinateValueConverter coordinateConverter)
        {
            double west = BitConverter.ToDouble(boundingBox, 0);
            double south = BitConverter.ToDouble(boundingBox, 8);
            double east = BitConverter.ToDouble(boundingBox, 16);
            double north = BitConverter.ToDouble(boundingBox, 24);

            Location northwest = new Location(north, west);
            if (coordinateConverter != null)
            {
                northwest = coordinateConverter.Convert(northwest);
            }

            Location southeast = new Location(south, east);
            if (coordinateConverter != null)
            {
                southeast = coordinateConverter.Convert(southeast);
            }

            return new LocationRect(northwest, southeast);
        }

        private Task<MapShapeModelCollection> ProcessShapefile(IRandomAccessStream shapeStream, CancellationTokenSource cancellationTokenSource)
        {
            var token = cancellationTokenSource.Token;

            var task = Task.Factory
                .StartNew<Task<ShapefileRecordInfoCollection>>(() => this.BuildShapeIndexData(shapeStream, cancellationTokenSource), token).Unwrap()
                .ContinueWith<MapShapeModelCollection>((previousTask) => this.ProcessItems(shapeStream, previousTask.Result, cancellationTokenSource), token);

            return task;
        }

        private Task<ShapefileRecordInfoCollection> BuildShapeIndexData(IRandomAccessStream stream, CancellationTokenSource cancellationTokenSource)
        {
            var token = cancellationTokenSource.Token;

            var task = Task.Factory.StartNew(
                async () =>
                {
                    if (cancellationTokenSource.IsCancellationRequested)
                    {
                        return null;
                    }

                    using (var shapeStream = stream.CloneStream())
                    {
                        int offset = 36;
                        shapeStream.Seek((ulong)offset);

                        byte[] boundingBox = new byte[32];
                        await shapeStream.ReadAsync(boundingBox.AsBuffer(), 32u, InputStreamOptions.Partial);

                        var recordInfos = new ShapefileRecordInfoCollection();
                        recordInfos.BoundingRect = GetBoundingRect(boundingBox, this.CoordinateValueConverter);

                        offset = 100;
                        shapeStream.Seek((ulong)offset);

                        while (shapeStream.Position < shapeStream.Size)
                        {
                            if (cancellationTokenSource.IsCancellationRequested)
                            {
                                return null;
                            }

                            byte[] recordHeader = new byte[8];
                            await shapeStream.ReadAsync(recordHeader.AsBuffer(), 8u, InputStreamOptions.Partial);

                            // The content length for a record is the length of the record contents section measured in 16-bit words. 
                            int contentLength = ToInt32BigEndian(recordHeader, 4) * 2;

                            offset += 8;
                            recordInfos.Add(new ShapefileRecordInfo() { ContentOffset = offset, ContentLength = contentLength });

                            offset += contentLength;
                            shapeStream.Seek((ulong)offset);
                        }

                        return recordInfos;
                    }
                },
                token).Unwrap();

            return task;
        }

        private MapShapeModelCollection ProcessItems(IRandomAccessStream shapeStream, ShapefileRecordInfoCollection recordInfos, CancellationTokenSource cancellationTokenSource)
        {
            if (cancellationTokenSource.IsCancellationRequested)
            {
                return null;
            }

            int itemCount = recordInfos.Count;
            int maxDegreeOfParallelism = Environment.ProcessorCount;
            if (itemCount < maxDegreeOfParallelism)
            {
                maxDegreeOfParallelism = itemCount;
            }

            int remainder = itemCount % maxDegreeOfParallelism;
            int multiplier = (itemCount / maxDegreeOfParallelism) + (remainder > 0 ? 1 : 0);

            var token = cancellationTokenSource.Token;
            List<Task<MapShapeModelCollection>> tasks = new List<Task<MapShapeModelCollection>>();

            for (int i = 0; i < maxDegreeOfParallelism; i++)
            {
                int start = i * multiplier;
                int end = Math.Min((i + 1) * multiplier, itemCount);

                ParseShapeTaskState taskState = new ParseShapeTaskState()
                {
                    Start = start,
                    End = end,
                    Records = recordInfos,
                    Stream = shapeStream,
                    CancellationTokenSource = cancellationTokenSource,
                    CoordinateConverter = this.CoordinateValueConverter
                };

                var task = Task.Factory.StartNew<Task<MapShapeModelCollection>>(ParseShapes, taskState, token, TaskCreationOptions.AttachedToParent, TaskScheduler.Current).Unwrap();

                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());

            MapShapeModelCollection models = tasks[0].Result;
            for (int i = 1; i < maxDegreeOfParallelism; i++)
            {
                var modelsToMerge = tasks[i].Result;
                if (modelsToMerge == null)
                {
                    continue;
                }

                foreach (var model in modelsToMerge)
                {
                    models.Add(model);
                }
            }

            models.BoundingRect = recordInfos.BoundingRect;

            return models;
        }

        private struct ShapefileRecordInfo
        {
            public int ContentOffset
            {
                get;
                set;
            }

            public int ContentLength
            {
                get;
                set;
            }
        }

        private class ShapefileRecordInfoCollection : Collection<ShapefileRecordInfo>
        {
            public LocationRect BoundingRect
            {
                get;
                set;
            }
        }

        private class ParseShapeTaskState
        {
            public IRandomAccessStream Stream
            {
                get;
                set;
            }

            internal int Start
            {
                get;
                set;
            }

            internal int End
            {
                get;
                set;
            }

            internal ShapefileRecordInfoCollection Records
            {
                get;
                set;
            }

            internal CancellationTokenSource CancellationTokenSource
            {
                get;
                set;
            }

            internal ICoordinateValueConverter CoordinateConverter
            {
                get;
                set;
            }
        }
    }
}
