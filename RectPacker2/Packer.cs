
using System;
using System.Collections.Generic;

namespace RectPacker2 {

    /// <summary>
    /// Packer that is used to pack smaller areas into a larger area
    /// </summary>
    public sealed class Packer {

        /// <summary>
        /// List of empty areas in the packable area
        /// </summary>
        public IReadOnlyList<Area> EmptyAreas => _emptyAreas;

        /// <summary>
        /// Total width of the packable area
        /// </summary>
        public int TotalWidth { get; }

        /// <summary>
        /// Total height of the packable area
        /// </summary>
        public int TotalHeight { get; }

        private readonly List<Area> _emptyAreas;

        /// <summary>
        /// Constructs a new <see cref="Packer"/> instance with total width and height
        /// </summary>
        /// <param name="width">Total width of the available area</param>
        /// <param name="height">Total height of the available area</param>
        public Packer(int width, int height) {
            TotalWidth = width;
            TotalHeight = height;
            _emptyAreas = new List<Area> {
                new Area(0, 0, width, height)
            };
        }

        /// <summary>
        /// Packs an area of specified size
        /// </summary>
        /// <param name="width">Width of the area to pack</param>
        /// <param name="height">Height of the area to pack</param>
        /// <returns></returns>
        /// <exception cref="Exception">Throws exception if packing of the area was unsuccessful</exception>
        public Area Pack(int width, int height) {
            if (!TryPack(width, height, out Area packedArea)) {
                throw new Exception("Could not pack the area, running out of space");
            }
            return packedArea;
        }

        /// <summary>
        /// Attempts to pack an area of specified size
        /// </summary>
        /// <param name="width">Width of the area to pack</param>
        /// <param name="height">Height of the area to pack</param>
        /// <param name="packedArea">The packed area</param>
        /// <returns>whether the packing of the area was successful</returns>
        public bool TryPack(int width, int height, out Area packedArea) {

            // check if the requested size is over bounds 
            if ((uint)width > (uint)TotalWidth || (uint)height > (uint)TotalHeight) {
                packedArea = default;
                return false;
            }

            // attempt to get the most suitable area for this size
            int index = GetMostSuitableArea(_emptyAreas, width, height);

            if (index < 0) {
                packedArea = default;
                return false;
            }

            // an area was found, cut it and update the empty areas list
            var area = _emptyAreas[index];
            packedArea = new Area(area.X, area.Y, width, height);

            (Area a, Area b) = area.Cut(width, height);

            CreateSplits(_emptyAreas, a, b, index);
            
            return true;
        }

        /// <summary>
        /// Splits and updates the specified area in the list
        /// </summary>
        /// <param name="list">The target list</param>
        /// <param name="a">First part of the splitted area</param>
        /// <param name="b">Second part of the splitted area</param>
        /// <param name="index">Index of the original area in the list</param>
        private static void CreateSplits(List<Area> list, Area a, Area b, int index) {
            if (a.IsEmpty && b.IsEmpty) {
                list.RemoveAt(index);
                return;
            }

            if (a.IsEmpty) {
                list[index] = b;
                return;
            }

            if (b.IsEmpty) {
                list[index] = a;
                return;
            }

            list[index] = a;
            list.Add(b);
        }

        /// <summary>
        /// Gets the most suitable area from the list for the specified dimension
        /// </summary>
        /// <param name="areas">The list of areas to search</param>
        /// <param name="width">Width of the searched size</param>
        /// <param name="height">Height of the searched size</param>
        /// <returns>Index to the most suitable area found, returns -1 if no area was found.</returns>
        private static int GetMostSuitableArea(List<Area> areas, int width, int height) {
            
            Area currentArea = default;
            int currentIndex = -1;

            for (int i = 0; i < areas.Count; i++) {

                var area = areas[i];

                // the area cannot be smaller than the requested size
                if (area.Width < width || area.Height < height) {
                    continue;
                }

                // if the area is exactly equal to the requested size, then use that
                if (area.Width == width && area.Height == height) {
                    return i;
                }

                bool largerSurface = currentArea.Width * currentArea.Height < area.Width * area.Height;
                bool hasEqualDimension = area.Width == width || area.Height == height;

                // if the current best area is not equal on width or height, try to find area that has equal dimensions or larger surface
                if (currentArea.Width != width && currentArea.Height != height) {
                    if (hasEqualDimension || largerSurface) {
                        currentArea = area;
                        currentIndex = i;
                    }
                    continue;
                }

                // otherwise, if the current area is equal on a dimension, only allow equal dimension + larger surface to overtake
                if (hasEqualDimension && largerSurface) {
                    currentArea = area;
                    currentIndex = i;
                }
            }

            return currentIndex;
        }

    }
}
