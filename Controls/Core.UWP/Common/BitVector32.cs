using System;

namespace Telerik.Core.Data
{
    /// <summary>
    /// A simple structure that uses the bits of a <see cref="System.UInt32"/> structure to define boolean values.
    /// </summary>
    public struct BitVector32
    {
        private uint data;

        /// <summary>
        /// Initializes a new instance of the <see cref="BitVector32"/> struct using the raw data specified by the provided source vector.
        /// </summary>
        /// <param name="source">The source.</param>
        public BitVector32(BitVector32 source)
        {
            this.data = source.data;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BitVector32"/> struct.
        /// </summary>
        /// <param name="data">The data.</param>
        public BitVector32(uint data)
        {
            this.data = data;
        }

        /// <summary>
        /// Gets the <see cref="T:UInt32"/> structure holding the separate bits of the vector.
        /// </summary>
        public uint Data
        {
            get
            {
                return this.data;
            }
        }

        /// <summary>
        /// Determines whether the bit, corresponding to the specified key is set.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1043:UseIntegralOrStringArgumentForIndexers")]
        public bool this[uint key]
        {
            get
            {
                return (this.data & key) == key;
            }
            set
            {
                if (value)
                {
                    this.data |= key;
                }
                else
                {
                    this.data &= ~key;
                }
            }
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="vector1">The vector1.</param>
        /// <param name="vector2">The vector2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(BitVector32 vector1, BitVector32 vector2)
        {
            return vector1.data == vector2.data;
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="vector1">The vector1.</param>
        /// <param name="vector2">The vector2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(BitVector32 vector1, BitVector32 vector2)
        {
            return vector1.data != vector2.data;
        }

        /// <summary>
        /// Clears all currently set bits in this vector.
        /// </summary>
        public void Reset()
        {
            this.data = 0;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (!(obj is BitVector32))
            {
                return false;
            }

            return (BitVector32)obj == this;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return this.data.GetHashCode();
        }
    }
}
