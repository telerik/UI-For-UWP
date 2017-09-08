namespace Telerik.Data.Core.Layouts
{
    internal interface IRenderInfo
    {
        int Count
        {
            get;
        }

        bool HasUpdatedValues
        {
            get;
        }

        double OffsetFromIndex(int index);

        int IndexFromOffset(double offset);

        void ResetToDefaultValues(IRenderInfoState loadState, double defaultValue);

        void Clear();

        void Insert(int index, double value);

        void InsertRange(int index, double? value, int count);

        void Add(double value);

        void Update(int index, double value);

        /// <summary>
        /// Returns the value for the index specified.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="approximate">Determines whether approximate returns zero if no value is set or approximate according the default length used.</param>
        double ValueForIndex(int index, bool approximate = true);

        void RemoveAt(int index);

        void RemoveRange(int index, int count);
    }
}