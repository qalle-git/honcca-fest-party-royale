// EnumExtensions.cs
// Author Omid Jawadi
// LBS Kreativa Gymnasiet

using System;
using System.Linq;

namespace HonccaFest.Files
{
    public static class EnumExtensions
    {
        // Creates a random sequence of an enums elements and then returns the first value in the sequence
        public static Enum GetRandomEnumValue(this Type t)
        {
            return Enum.GetValues(t)
                .OfType<Enum>()
                .OrderBy(e => Guid.NewGuid())
                .FirstOrDefault();
        }

        // Returns an enums next element in an ascending order
        public static T NextEnum<T>(this T source)
        {
            T[] Arr = (T[])Enum.GetValues(source.GetType()); // Creates an array containing all of the source enums elements
            int n = Array.IndexOf<T>(Arr, source) + 1; // Sets n to be the next element in the enum
            return (Arr.Length == n) ? Arr[0] : Arr[n]; // Returns the first element in the enum if the n'th element is outside of the arrays bounds, otherwise returns the next element
        }
    }
}