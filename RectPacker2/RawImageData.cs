using RectPacker2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RectPacker2;

/// <summary>
/// Raw pixel data of an image with dimensions
/// </summary>
/// <typeparam name="TPixel">Type of the pixel</typeparam>
public readonly ref struct RawImageData<TPixel> where TPixel : unmanaged {

    /// <summary>
    /// Bounds of the image
    /// </summary>
    public Area Bounds => new Area(0, 0, _width, _height);

    /// <summary>
    /// Width of the image
    /// </summary>
    public int Width => _width;
    
    /// <summary>
    /// Height of the image
    /// </summary>
    public int Height => _height;

    private readonly Span<TPixel> _data;
    private readonly int _width;
    private readonly int _height;

    /// <summary>
    /// Constructs a new <see cref="RawImageData{TPixel}"/> from a span, width and height
    /// </summary>
    /// <param name="data">Span of the images raw data</param>
    /// <param name="width">Width of the image</param>
    /// <param name="height">Height image</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public RawImageData(Span<TPixel> data, int width, int height) {
        if (width * height != data.Length) {
            throw new ArgumentOutOfRangeException(nameof(data));
        }
        _data = data;
        _width = width;
        _height = height;
    }

    /// <summary>
    /// Provides access to a single row of the image
    /// </summary>
    /// <param name="row"></param>
    /// <returns></returns>
    public Span<TPixel> GetRow(int row) => _data.Slice(row * _width);

    /// <summary>
    /// Creates a slice of the image
    /// </summary>
    /// <param name="x">X starting position of the slice</param>
    /// <param name="y">Y starting position of the slice</param>
    /// <param name="width">Width of the slice</param>
    /// <param name="height">Height of the slice</param>
    /// <returns></returns>
    public RawImageSlice<TPixel> Slice(int x, int y, int width, int height) {
        return Slice(new Area(x, y, width, height));
    }

    /// <summary>
    /// Creates a slice of the image
    /// </summary>
    /// <param name="source">Area to slice from the image</param>
    /// <returns></returns>
    public RawImageSlice<TPixel> Slice(Area source) => new RawImageSlice<TPixel>(this, source);

    /// <summary>
    /// Copies the image into another image
    /// </summary>
    /// <param name="image">The image to copy to</param>
    public void CopyTo(in RawImageData<TPixel> image) => CopyTo(image.Slice(0,0, _width, _height));
    
    /// <summary>
    /// Copies the image into another image at a specified location
    /// </summary>
    /// <param name="image">The image to copy to</param>
    /// <param name="location">The location where to copy to</param>
    public void CopyTo(in RawImageData<TPixel> image, Area location) => CopyTo(image.Slice(location));
    
    /// <summary>
    /// Copies the image into a slice 
    /// </summary>
    /// <param name="src"></param>
    public void CopyTo(in RawImageSlice<TPixel> src) => Slice(Bounds).CopyTo(src);
}