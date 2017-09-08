using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace Telerik.Geospatial
{
    internal partial class ShapefileReader
    {
        private const string InvalidFormat = "The DBF file is invalid or has unsupported format.";

        // TODO: What about Null values?
        // TODO: Consider whether supporting dBASE version 7 makes sense at all (currently unsupported).
        private static readonly byte[] AllowedTypes = new byte[]
        {
            0x02, /* FoxBASE */
            0x03, /* FoxBASE+/Dbase III plus, no memo */
            0x30, /* Visual FoxPro */
            0x43, /* dBASE IV SQL table files, no memo */
            0x63, /* dBASE IV SQL system files, no memo */
            0x83, /* FoxBASE+/dBASE III PLUS, with memo */
            0x8b, /* dBASE IV with memo */
            0xcb, /* dBASE IV SQL table files, with memo */
            0xf5, /* FoxPro 2.x (or earlier) with memo */
            0xfb  /* FoxBASE */
        };

        public List<string> AttributesToLoad
        {
            get;
            set;
        }

        public IAttributeValueConverter AttributeValueConverter
        {
            get;
            set;
        }

        private static Task<DbfHeader> BuildDbfHeaderData(IRandomAccessStream stream, Encoding encoding, CancellationTokenSource cancellationTokenSource)
        {
            var token = cancellationTokenSource.Token;

            var task = Task.Factory.StartNew(
                async () =>
                {
                    if (cancellationTokenSource.IsCancellationRequested)
                    {
                        return null;
                    }

                    if (stream.Size < 32)
                    {
                        throw new NotSupportedException(InvalidFormat);
                    }

                    using (var dataStream = stream.CloneStream())
                    {
                        byte[] header = new byte[32];
                        await dataStream.ReadAsync(header.AsBuffer(), 32u, InputStreamOptions.Partial);

                        byte fileType = header[0];
                        if (!AllowedTypes.Contains(fileType))
                        {
                            throw new NotSupportedException(InvalidFormat);
                        }

                        DbfHeader dbfHeader = new DbfHeader();
                        dbfHeader.RecordsCount = BitConverter.ToInt32(header, 4);
                        dbfHeader.RecordsOffset = BitConverter.ToInt16(header, 8);
                        dbfHeader.RecordLength = BitConverter.ToInt16(header, 10);

                        if (encoding == null)
                        {
                            byte languageDriver = header[29];
                            encoding = DbfEncoding.GetEncoding(languageDriver);
                        }

                        dbfHeader.Encoding = encoding;

                        // header is 32 bytes + n field descriptors * 32 bytes + carriage return byte (0x0D)
                        int fieldDescriptorCount = (dbfHeader.RecordsOffset - 32 - 1) / 32;
                        byte[] fieldDescriptor;
                        DbfFieldInfo dbfField;
                        for (int i = 0; i < fieldDescriptorCount; i++)
                        {
                            if (cancellationTokenSource.IsCancellationRequested)
                            {
                                return null;
                            }

                            fieldDescriptor = new byte[32];
                            await dataStream.ReadAsync(fieldDescriptor.AsBuffer(), 32u, InputStreamOptions.Partial);

                            dbfField = new DbfFieldInfo();
                            dbfField.Name = encoding.GetString(fieldDescriptor, 0, 11).Replace("\0", string.Empty);
                            dbfField.NativeDbfType = (char)fieldDescriptor[11];

                            dbfField.Length = fieldDescriptor[16];
                            dbfField.DecimalCount = fieldDescriptor[17];

                            dbfHeader.Fields.Add(dbfField);
                        }

                        return dbfHeader;
                    }
                },
                token).Unwrap();

            return task;
        }

        private static Task ParseAttributes(object state)
        {
            var taskState = (ParseDataTaskState)state;
            var cancellationTokenSource = taskState.CancellationTokenSource;
            var token = cancellationTokenSource.Token;

            var task = Task.Factory.StartNew(
                async () =>
                {
                    using (var dataStream = taskState.Stream.CloneStream())
                    {
                        int start = taskState.Start;
                        int end = taskState.End;
                        DbfHeader dbfHeader = taskState.DbfHeader;
                        var shapeModels = taskState.ShapeModels;
                        var valueConverter = taskState.ValueConverter;
                        var attributesToLoad = taskState.AttributesToLoad;

                        dataStream.Seek((ulong)(dbfHeader.RecordsOffset + start * dbfHeader.RecordLength));

                        for (int i = start; i < end; i++)
                        {
                            if (cancellationTokenSource.IsCancellationRequested)
                            {
                                return;
                            }

                            var shapeModel = shapeModels[i] as MapShapeModel;
                            byte[] record = new byte[dbfHeader.RecordLength];
                            await dataStream.ReadAsync(record.AsBuffer(), (uint)dbfHeader.RecordLength, InputStreamOptions.Partial);

                            // Data records are preceded by one byte; that is, a space (20H) if the record is not deleted, an asterisk (2AH) if the record is deleted.
                            int offset = 1;
                            foreach (var field in dbfHeader.Fields)
                            {
                                if (attributesToLoad == null || attributesToLoad.Contains(field.Name))
                                {
                                    string value = dbfHeader.Encoding.GetString(record, offset, field.Length);
                                    object propertyValue = TransformDbfValue(field, value, valueConverter);

                                    shapeModel.Attributes[field.Name] = propertyValue;
                                }

                                offset += field.Length;
                            }
                        }
                    }
                },
                token).Unwrap();

            return task;
        }

        private static object TransformDbfValue(DbfFieldInfo field, string fieldValue, IAttributeValueConverter valueConverter)
        {
            switch (field.NativeDbfType)
            {
                // Currency
                case 'Y':

                // Numeric
                case 'N':

                // Float
                case 'F':

                // Double
                case 'B':

                    // NOTE: numeric overflow is physically stored as asterisks in the DBF file.
                    fieldValue = fieldValue.TrimStart(' ', '*');
                    break;

                // Integer
                case 'I':
                    // TODO: Handle integer overflow
                    // NOTE: Integer fields cannot handle overflow as asterisks in DBF file because they are stored in binary format. 
                    // Overflow in integer field is represented by the lowest integer value -2**32.
                    fieldValue = fieldValue.TrimStart(' ');
                    break;

                // Date
                case 'D':
                    if (string.IsNullOrEmpty(fieldValue.TrimEnd(' ')))
                    {
                        fieldValue = DateTime.MinValue.ToString(CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        fieldValue = fieldValue.Substring(0, 4) + "-" + fieldValue.Substring(4, 2) + "-" + fieldValue.Substring(6, 2);
                    }
                    break;

                // DateTime
                case 'T':
                    fieldValue = fieldValue.Substring(0, 4)
                        + "-"
                        + fieldValue.Substring(4, 2)
                        + "-"
                        + fieldValue.Substring(6, 2)
                        + " "
                        + fieldValue.Substring(8, 2)
                        + ":"
                        + fieldValue.Substring(10, 2)
                        + ":"
                        + fieldValue.Substring(12, 2);
                    break;

                case 'L':
                    if (fieldValue.ToUpper() == "T" || fieldValue.ToUpper() == "Y")
                    {
                        fieldValue = bool.TrueString;
                    }
                    else
                    {
                        fieldValue = bool.FalseString;
                    }
                    break;

                default:
                    fieldValue = fieldValue.TrimEnd(' ');
                    break;
            }

            object result = null;
            if (!string.IsNullOrEmpty(fieldValue))
            {
                result = Convert.ChangeType(fieldValue, field.MappedType, CultureInfo.InvariantCulture);
            }

            // NOTE: Optionally allow the user to customize the field value / field value type.
            if (valueConverter != null)
            {
                result = valueConverter.Convert(result, field.Name, field.MappedType);
            }

            return result;
        }

        private MapShapeModelCollection ProcessDataItems(MapShapeModelCollection shapeModels, DbfHeader dbfHeader, IRandomAccessStream dataStream, CancellationTokenSource cancellationTokenSource)
        {
            if (cancellationTokenSource.IsCancellationRequested)
            {
                return null;
            }

            var token = cancellationTokenSource.Token;

            int itemCount = dbfHeader.RecordsCount;
            int maxDegreeOfParallelism = Environment.ProcessorCount;
            if (itemCount < maxDegreeOfParallelism)
            {
                maxDegreeOfParallelism = itemCount;
            }

            int remainder = itemCount % maxDegreeOfParallelism;
            int multiplier = (itemCount / maxDegreeOfParallelism) + (remainder > 0 ? 1 : 0);

            List<Task> tasks = new List<Task>();

            for (int i = 0; i < maxDegreeOfParallelism; i++)
            {
                int start = i * multiplier;
                int end = Math.Min((i + 1) * multiplier, itemCount);

                var taskState = new ParseDataTaskState()
                {
                    Start = start,
                    End = end,
                    DbfHeader = dbfHeader,
                    Stream = dataStream,
                    ShapeModels = shapeModels,
                    CancellationTokenSource = cancellationTokenSource,
                    ValueConverter = this.AttributeValueConverter,
                    AttributesToLoad = this.AttributesToLoad
                };
                var task = Task.Factory.StartNew<Task>(ParseAttributes, taskState, token, TaskCreationOptions.AttachedToParent, TaskScheduler.Current).Unwrap();

                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());

            return shapeModels;
        }

        private Task<MapShapeModelCollection> ProcessDataFile(MapShapeModelCollection shapeModels, IRandomAccessStream dataStream, CancellationTokenSource cancellationTokenSource, Encoding encoding = null)
        {
            var token = cancellationTokenSource.Token;

            var task = Task.Factory
                .StartNew<Task<DbfHeader>>(() => BuildDbfHeaderData(dataStream, encoding, cancellationTokenSource), token).Unwrap()
                .ContinueWith((previousTask) => this.ProcessDataItems(shapeModels, previousTask.Result, dataStream, cancellationTokenSource), token);

            return task;
        }

        private class ParseDataTaskState
        {
            internal IRandomAccessStream Stream
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

            internal DbfHeader DbfHeader
            {
                get;
                set;
            }

            internal MapShapeModelCollection ShapeModels
            {
                get;
                set;
            }

            internal CancellationTokenSource CancellationTokenSource
            {
                get;
                set;
            }

            internal IAttributeValueConverter ValueConverter
            {
                get;
                set;
            }

            internal List<string> AttributesToLoad
            {
                get;
                set;
            }
        }
    }
}
