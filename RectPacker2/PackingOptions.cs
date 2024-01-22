using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RectPacker2;

/// <summary>
/// Provides options for packing images
/// </summary>
/// <param name="Padding">Padding around the image</param>
/// <param name="CopyEdges">Whether the edges of the image should be copied to prevent bleeding</param>
public record struct PackingOptions(int Padding, bool CopyEdges) {

    /// <summary>
    /// Default <see cref="PackingOptions"/> with no padding and no edges copied
    /// </summary>
    public static PackingOptions Default => new PackingOptions(0, false);

    /// <summary>
    /// Validates that the packing options are within valid values and throws <see cref="ArgumentException"/> if they are invalid
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    public readonly void Validate() {
        if (Padding < 0) {
            throw new ArgumentException("Padding cannot be negative");
        }
    }
}