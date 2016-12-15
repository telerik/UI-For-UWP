using System.Text;

namespace Telerik.Geospatial
{
    internal class DbfHeader
    {
        private DbfFieldCollection fields;

        public Encoding Encoding
        {
            get;
            set;
        }

        public int RecordsCount
        {
            get;
            set;
        }

        public int RecordsOffset
        {
            get;
            set;
        }

        public int RecordLength
        {
            get;
            set;
        }

        public DbfFieldCollection Fields
        {
            get
            {
                if (this.fields == null)
                {
                    this.fields = new DbfFieldCollection();
                }

                return this.fields;
            }
        }
    }
}
