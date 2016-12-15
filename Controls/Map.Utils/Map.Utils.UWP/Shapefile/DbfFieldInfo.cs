using System;

namespace Telerik.Geospatial
{
    internal class DbfFieldInfo
    {
        public string Name
        {
            get;
            set;
        }

        public byte Length
        {
            get;
            set;
        }

        public byte DecimalCount
        {
            get;
            set;
        }

        public char NativeDbfType
        {
            get;
            set;
        }

        public Type MappedType
        {
            get
            {
                switch (this.NativeDbfType)
                {
                    // Currency
                    case 'Y':
                    // Float
                    case 'F':
                    // Double
                    case 'B':
                    // Numeric
                    // TODO: Consider handling numeric as int/long if appropriate? (MS does not do this http://msdn.microsoft.com/en-us/library/windows/desktop/ms713987(v=vs.85).aspx)
                    case 'N':
                        return typeof(double);

                    // Integer
                    case 'I':
                        return typeof(int);

                    // Logical
                    case 'L':
                        return typeof(bool);

                    // Date
                    case 'D':
                    // DateTime
                    case 'T':
                        return typeof(DateTime);

                    default:
                        return typeof(string);
                }
            }
        }
    }
}
