using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RectPacker2;

/// <summary>
/// An image's slice
/// </summary>
/// <typeparam name="TPixel">Type of the pixel</typeparam>
public readonly ref struct RawImageSlice<TPixel> where TPixel : unmanaged {

    /// <summary>
    /// Width of the slice
    /// </summary>
    public int Width => _source.Width;
    
    /// <summary>
    /// Height of the slice
    /// </summary>
    public int Height => _source.Height;

    private readonly RawImageData<TPixel> _image;
    private readonly Area _source;

    /// <summary>
    /// Constructs a new slice from an image at a specified location
    /// </summary>
    /// <param name="image">The image to slice from</param>
    /// <param name="source">Location to create the slice from</param>
    /// <exception cref="ArgumentException"></exception>
    public RawImageSlice(RawImageData<TPixel> image, Area source) {
        if (source.IsEmpty) {
            throw new ArgumentException("Source cannot be empty");
        }
        if (!image.Bounds.Contains(source)) {
            throw new ArgumentException("Source must be within the image");
        }
        _image = image;
        _source = source;
    }

    /// <summary>
    /// Provides access to a single row of the slice
    /// </summary>
    /// <param name="row"></param>
    /// <returns></returns>
    public Span<TPixel> GetRow(int row) {
        return _image.GetRow(_source.Y + row).Slice(_source.X, _source.Width);
    }

    /// <summary>
    /// Copies the slice into an image
    /// </summary>
    /// <param name="dst">The image to copy to</param>
    public void CopyTo(in RawImageData<TPixel> dst) => CopyTo(dst.Slice(0, 0, Width, Height));

    /// <summary>
    /// Copies the slice into an image at a specified location
    /// </summary>
    /// <param name="dst">The image to copy to</param>
    /// <param name="location">The location where to copy to</param>
    public void CopyTo(in RawImageData<TPixel> dst, Area location) => CopyTo(dst.Slice(location));

    /// <summary>
    /// Copies the slice into another slice
    /// </summary>
    /// <param name="dst">The slice to copy to</param>
    /// <exception cref="ArgumentException"></exception>
    public void CopyTo(in RawImageSlice<TPixel> dst) {
        if (Width > dst.Width || Height > dst.Height) {
            throw new ArgumentException("Destination cannot be smaller than the source image");
        }
        if (dst.Width != Width || dst.Height != Height) {
            throw new ArgumentException("Destination must be same dimension as the source");
        }

        for (int y = 0; y < Height; y++) {
            GetRow(y).CopyTo(dst.GetRow(y));
        }
    }
}