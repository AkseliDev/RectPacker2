using System;
using System.Collections.Generic;
using System.Text;

namespace RectPacker2 {
    
    /// <summary>
    /// Area that represents location and size
    /// </summary>
    public struct Area {

        /// <summary>
        /// Determines whether the area is empty or not
        /// </summary>
        public readonly bool IsEmpty => Width <= 0 || Height <= 0;

        /// <summary>
        /// The x coordinate of the area
        /// </summary>
        public int X;

        /// <summary>
        /// The y coordinate of the area
        /// </summary>
        public int Y;

        /// <summary>
        /// The width of the area
        /// </summary>
        public int Width;

        /// <summary>
        /// The height of the area
        /// </summary>
        public int Height;

        /// <summary>
        /// Constructs a new <see cref="Area"/> with the specified position, width and height
        /// </summary>
        /// <param name="x">The x coordinate of the area</param>
        /// <param name="y">The y coordinate of the area</param>
        /// <param name="width">The width of the area</param>
        /// <param name="height">The height of the area</param>
        public Area(int x, int y, int width, int height) {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Cuts an area of specified size and returns the splitted parts
        /// </summary>
        /// <param name="width">Width of the cut</param>
        /// <param name="height">Height of the cut</param>
        /// <returns>The splitted parts of the cut area</returns>
        public readonly (Area a, Area b) Cut(int width, int height) {

            int leftoverWidth = Width - width;
            int leftoverHeight = Height - height;

            if (leftoverWidth * height > leftoverHeight * width) {
                return (
                    new Area(X + width, Y, leftoverWidth, Height),
                    new Area(X, Y + height, width, leftoverHeight)
                );
            }

            return (
                new Area(X + width, Y, leftoverWidth, height),
                new Area(X, Y + height, Width, leftoverHeight)
            );
        }

        public override readonly string ToString() {
            return $"( X: {X}, Y: {Y}, W: {Width}, H: {Height} )";
        }

        public readonly bool Contains(Area value) {
            return ((((X <= value.X) && ((value.X + value.Width) <= (X + Width))) && (Y <= value.Y)) && ((value.Y + value.Height) <= (Y + Height)));
        }
    }
}
