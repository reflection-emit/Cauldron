using System.Runtime.InteropServices;

namespace Cauldron.Core.Extensions
{
    /// <summary>
    /// Provides usefull extension methods
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// Replaces the values of data in memory with random values. The GC handle will be freed.
        /// </summary>
        /// <remarks>Will only work on <see cref="GCHandleType.Pinned"/></remarks>
        /// <param name="target"></param>
        /// <param name="targetLength">The length of the data</param>
        public static void FillWithRandomValues(this GCHandle target, int targetLength) => target.FillWithRandomValues(targetLength, true);

        /// <summary>
        /// Replaces the values of data in memory with random values.
        /// </summary>
        /// <remarks>Will only work on <see cref="GCHandleType.Pinned"/></remarks>
        /// <param name="target"></param>
        /// <param name="targetLength">The length of the data</param>
        /// <param name="freeHandle">If true, the GC handle will be freed; otherwise not.</param>
        public static void FillWithRandomValues(this GCHandle target, int targetLength, bool freeHandle)
        {
            unsafe
            {
                byte* insecureData = (byte*)target.AddrOfPinnedObject();

                for (int i = 0; i < targetLength; i++)
                    insecureData[i] = Randomizer.NextByte();

                if (freeHandle)
                    target.Free();
            }
        }

        /// <summary>
        /// Picks a random element from the given array
        /// </summary>
        /// <typeparam name="T">The type of element in the array</typeparam>
        /// <param name="array">The array to pick a random element from</param>
        /// <returns>The randomly picked element</returns>
        public static T RandomPick<T>(this T[] array) => array[Randomizer.Next(0, array.Length - 1)];



    }
}