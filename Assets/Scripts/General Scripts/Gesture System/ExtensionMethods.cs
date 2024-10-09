namespace FoxheadDev.GestureDetection
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public static class ExtensionMethods
    {
        //This is a class full of methods that I can use globally.


        /// <summary>
        /// Gets the absolute value of a float.
        /// </summary>
        public static float Abs(this float num) => Mathf.Abs(num);
        /// <summary>
        /// Returns true if the vector's X component is its largest component
        /// </summary>
        public static bool MostlyX(this Vector3 vec) => vec.x.Abs() > vec.y.Abs() && vec.x.Abs() > vec.z.Abs();

        /// <summary>
        /// Returns true if the vector's Y component is its largest component
        /// </summary>
        public static bool MostlyY(this Vector3 vec) => vec.y.Abs() > vec.x.Abs() && vec.y.Abs() > vec.z.Abs();

        /// <summary>
        /// Returns true if the vector's Z component is its largest component
        /// </summary>
        public static bool MostlyZ(this Vector3 vec) => vec.z.Abs() > vec.x.Abs() && vec.z.Abs() > vec.y.Abs();

        /// <summary>
        /// Just turns the X value to the negative of what it is. I use this to flip things for left and right
        /// </summary>
        public static Vector3 InvertX(this Vector3 vec) => new(-vec.x, vec.y, vec.z);
        /// <summary>
        /// Just turns the Y value to the negative of what it is. I use this to flip things for left and right
        /// </summary>
        public static Vector3 InvertY(this Vector3 vec) => new(vec.x, -vec.y, vec.z);
    }

}