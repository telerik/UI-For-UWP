using System;

namespace Telerik.Core
{
    internal class FastPropertyStore
    {
        private Entry[] entries;

        public FastPropertyStore()
        {
        }

        public void Clear()
        {
            this.entries = null;
        }

        public bool ContainsEntry(int key)
        {
            bool found;
            this.GetEntry(key, out found);

            return found;
        }

        public object GetEntry(int key)
        {
            bool found;
            return this.GetEntry(key, out found);
        }

        public object GetEntry(int key, out bool found)
        {
            int objectIndex;
            short element;
            short entryKey = SplitKey(key, out element);
            found = false;

            bool locate = this.LocateObject(entryKey, out objectIndex);
            if (locate == false)
            {
                return null;
            }

            if (((1 << element) & this.entries[objectIndex].Mask) == 0)
            {
                return null;
            }

            found = true;
            switch (element)
            {
                case 0:
                    return this.entries[objectIndex].Val1;

                case 1:
                    return this.entries[objectIndex].Val2;

                case 2:
                    return this.entries[objectIndex].Val3;

                case 3:
                    return this.entries[objectIndex].Val4;
            }

            return null;
        }

        public void SetEntry(int key, object value)
        {
            int objectIndex;
            short element;
            short entryKey = SplitKey(key, out element);

            if (!this.LocateObject(entryKey, out objectIndex))
            {
                if (this.entries == null)
                {
                    this.entries = new Entry[1];
                }
                else
                {
                    Entry[] destArray = new Entry[this.entries.Length + 1];
                    if (objectIndex > 0)
                    {
                        Array.Copy(this.entries, 0, destArray, 0, objectIndex);
                    }
                    if ((this.entries.Length - objectIndex) > 0)
                    {
                        Array.Copy(this.entries, objectIndex, destArray, objectIndex + 1, this.entries.Length - objectIndex);
                    }
                    this.entries = destArray;
                }

                this.entries[objectIndex].Key = entryKey;
            }

            switch (element)
            {
                case 0:
                    this.entries[objectIndex].Val1 = value;
                    break;

                case 1:
                    this.entries[objectIndex].Val2 = value;
                    break;

                case 2:
                    this.entries[objectIndex].Val3 = value;
                    break;

                case 3:
                    this.entries[objectIndex].Val4 = value;
                    break;
            }

            this.entries[objectIndex].Mask = (short)(((ushort)this.entries[objectIndex].Mask) | (1 << element));
        }

        public void RemoveEntry(int key)
        {
            int objectIndex;
            short element;

            short entryKey = SplitKey(key, out element);
            bool hasObject = this.LocateObject(entryKey, out objectIndex);

            if (hasObject)
            {
                bool hasMask = ((1 << element) & this.entries[objectIndex].Mask) != 0;
                hasObject = hasObject && hasMask;
            }

            if (!hasObject)
            {
                return;
            }

            short mask = this.entries[objectIndex].Mask;
            mask = (short)(mask & ~((short)(1 << element)));
            this.entries[objectIndex].Mask = mask;

            if (mask == 0)
            {
                int length = this.entries.Length;
                if (length == 1)
                {
                    this.entries = null;
                }
                else
                {
                    Entry[] destArray = new Entry[length - 1];
                    if (objectIndex > 0)
                    {
                        Array.Copy(this.entries, 0, destArray, 0, objectIndex);
                    }
                    if (objectIndex < length)
                    {
                        Array.Copy(this.entries, objectIndex + 1, destArray, objectIndex, (length - objectIndex) - 1);
                    }
                    this.entries = destArray;
                }
            }
            else
            {
                switch (element)
                {
                    case 0:
                        this.entries[objectIndex].Val1 = null;
                        return;

                    case 1:
                        this.entries[objectIndex].Val2 = null;
                        return;

                    case 2:
                        this.entries[objectIndex].Val3 = null;
                        return;

                    case 3:
                        this.entries[objectIndex].Val4 = null;
                        return;
                }
            }
        }

        private static short SplitKey(int key, out short element)
        {
            element = (short)(key & 3);
            return (short)(key & 0xfffffffc);
        }

        private bool LocateObject(short entryKey, out int index)
        {
            index = 0;

            if (this.entries == null)
            {
                return false;
            }

            int middle;
            int length = this.entries.Length;

            if (length <= 16)
            {
                middle = length / 2;
                if (this.entries[middle].Key <= entryKey)
                {
                    index = middle;
                    if (this.entries[index].Key == entryKey)
                    {
                        return true;
                    }
                }

                middle = (length + 1) / 4;
                if (this.entries[index + middle].Key <= entryKey)
                {
                    index += middle;
                    if (this.entries[index].Key == entryKey)
                    {
                        return true;
                    }
                }

                middle = (length + 3) / 8;
                if (this.entries[index + middle].Key <= entryKey)
                {
                    index += middle;
                    if (this.entries[index].Key == entryKey)
                    {
                        return true;
                    }
                }

                middle = (length + 7) / 16;
                if (this.entries[index + middle].Key <= entryKey)
                {
                    index += middle;
                    if (this.entries[index].Key == entryKey)
                    {
                        return true;
                    }
                }

                if (entryKey > this.entries[index].Key)
                {
                    index++;
                }

                return false;
            }

            int left = 0;
            int right = length - 1;
            middle = 0;
            short key;

            do
            {
                middle = (right + left) / 2;
                key = this.entries[middle].Key;

                if (key == entryKey)
                {
                    index = middle;
                    return true;
                }

                if (entryKey < key)
                {
                    right = middle - 1;
                }
                else
                {
                    left = middle + 1;
                }
            }
            while (right >= left);

            index = middle;
            if (entryKey > this.entries[index].Key)
            {
                index++;
            }

            return false;
        }

        internal struct Entry
        {
            internal short Key;
            internal short Mask;
            internal object Val1;
            internal object Val2;
            internal object Val3;
            internal object Val4;

            public object GetVal(int valIndex)
            {
                object val = null;
                switch (valIndex)
                {
                    case 1:
                        val = this.Val1;
                        break;
                    case 2:
                        val = this.Val2;
                        break;
                    case 3:
                        val = this.Val3;
                        break;
                    case 4:
                        val = this.Val4;
                        break;
                }

                return val;
            }
        }
    }
}
