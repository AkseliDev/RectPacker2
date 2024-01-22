using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RectPacker2;

/// <summary>
/// A generic implementation of a sprite atlas that provides an api for simple packing of sprites with settings
/// </summary>
/// <typeparam name="TPixel">Pixel type that the result consists of/typeparam>
public sealed class SpriteAtlas<TPixel> where TPixel : unmanaged {

    /// <summary>
    /// Width of the sprite atlas
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// Height of the sprite atlas
    /// </summary>
    public int Height { get; }

    /// <summary>
    /// Provides access to the raw data of the sprite atlas
    /// </summary>
    public Span<TPixel> RawData => _data;

    /// <summary>
    /// Provides a <see cref="RawImageData{TPixel}"/> of the sprite atlas
    /// </summary>
    public RawImageData<TPixel> RawImage => new RawImageData<TPixel>(_data, Width, Height);

    private readonly TPixel[] _data;
    private readonly Packer _packer;

    /// <summary>
    /// Constructs a new sprite atlas using the specified width and height
    /// </summary>
    /// <param name="width">Width of the sprite atlas</param>
    /// <param name="height">Height of the sprite atlas</param>
    public SpriteAtlas(int width, int height) {
        Width = width;
        Height = height;
        _data = new TPixel[width * height];
        _packer = new Packer(width, height);
    }

    /// <summary>
    /// Packs a <see cref="RawImageData{TPixel}"/> into the sprite atlas using <see cref="PackingOptions.Default"/>
    /// </summary>
    /// <param name="img">The raw image to pack</param>
    /// <returns><see cref="Area"/> where the image was placed with any padding included</returns>
    public Area Pack(in RawImageData<TPixel> img) => Pack(in img, PackingOptions.Default);

    /// <summary>
    /// Packs a <see cref="RawImageSlice{TPixel}"/> into the sprite atlas using <see cref="PackingOptions.Default"/>
    /// </summary>
    /// <param name="src">The raw image slice to pack</param>
    /// <returns><see cref="Area"/> where the image was placed with any padding included</returns>
    public Area Pack(in RawImageSlice<TPixel> src) => Pack(in src, PackingOptions.Default);

    /// <summary>
    /// Packs a <see cref="RawImageData{TPixel}"/> into the sprite atlas using specified <see cref="PackingOptions"/>
    /// </summary>
    /// <param name="img">The raw image to pack</param>
    /// <param name="options">The packing options to use</param>
    /// <returns><see cref="Area"/> where the image was placed with any padding included</returns>
    public Area Pack(in RawImageData<TPixel> img, PackingOptions options) => Pack(img.Slice(img.Bounds), options);

    /// <summary>
    /// Packs a <see cref="RawImageSlice{TPixel}"/> into the sprite atlas using specified <see cref="PackingOptions"/>
    /// </summary>
    /// <param name="src">The raw image slice to pack</param>
    /// <param name="options">The packing options to use</param>
    /// <returns><see cref="Area"/> where the image was placed with any padding included</returns>
    /// <exception cref="Exception"></exception>
    public Area Pack(in RawImageSlice<TPixel> src, PackingOptions options) {
        if (!TryPack(src, options, out var area)) {
            throw new Exception("Sprite Atlas is full");
        }
        return area;
    }

    /// <summary>
    /// Attempts to pack a <see cref="RawImageData{TPixel}"/> into the sprite atlas using <see cref="PackingOptions.Default"/>
    /// </summary>
    /// <param name="img">The raw image to pack</param>
    /// <param name="location">Location where the image was placed with any padding included</param>
    /// <returns><c>true</c> if packing the image was successful</returns>
    public bool TryPack(in RawImageData<TPixel> img, out Area location) => TryPack(in img, PackingOptions.Default, out location);

    /// <summary>
    /// Attempts to pack a <see cref="RawImageSlice{TPixel}"/> into the sprite atlas using <see cref="PackingOptions.Default"/>
    /// </summary>
    /// <param name="img">The raw image slice to pack</param>
    /// <param name="location">Location where the image was placed with any padding included</param>
    /// <returns><c>true</c> if packing the image was successful</returns>
    public bool TryPack(in RawImageSlice<TPixel> src, out Area location) => TryPack(src, PackingOptions.Default, out location);

    /// <summary>
    /// Attempts to pack a <see cref="RawImageData{TPixel}"/> into the sprite atlas using specified <see cref="PackingOptions"/>
    /// </summary>
    /// <param name="img">The raw image to pack</param>
    /// <param name="location">Location where the image was placed with any padding included</param>
    /// <returns><c>true</c> if packing the image was successful</returns>
    public bool TryPack(in RawImageData<TPixel> img, PackingOptions options, out Area location) => TryPack(img.Slice(img.Bounds), options, out location);
    
    /// <summary>
    /// Attempts to pack a <see cref="RawImageSlice{TPixel}"/> into the sprite atlas using specified <see cref="PackingOptions"/>
    /// </summary>
    /// <param name="img">The raw image slice to pack</param>
    /// <param name="location">Location where the image was placed with any padding included</param>
    /// <returns><c>true</c> if packing the image was successful</returns>
    public bool TryPack(in RawImageSlice<TPixel> src, PackingOptions options, out Area location) {

        options.Validate();

        int padding = options.Padding;

        if (options.CopyEdges) {
            padding++;
        }

        int truePadding = padding * 2;
        int width = src.Width + truePadding;
        int height = src.Height + truePadding;

        if (!_packer.TryPack(width, height, out Area packedArea)) {
            location = default;
            return false;
        }

        // remove padding and copy to atlas
        src.CopyTo(RawImage.Slice(packedArea with {
            X = packedArea.X + padding,
            Y = packedArea.Y + padding,
            Width = packedArea.Width - truePadding,
            Height = packedArea.Height - truePadding
        }));

        // copy edges if needed
        if (options.CopyEdges) {
            CopyEdges(in src, packedArea with {
                X = packedArea.X + options.Padding,
                Y = packedArea.Y + options.Padding,
                Width = packedArea.Width - options.Padding * 2,
                Height = packedArea.Height - options.Padding * 2
            });
        }

        location = packedArea;

        return true;
    }

    /// <summary>
    /// Copies edges (one extra pixel from each border) to prevent images from bleeding
    /// </summary>
    /// <param name="src">The raw image slice to copy from</param>
    /// <param name="to">Where to copy the edges to</param>
    private void CopyEdges(in RawImageSlice<TPixel> src, Area to) {

        var inAtlas = RawImage.Slice(to);

        Span<TPixel> topRowSrc = src.GetRow(0);
        Span<TPixel> botRowSrc = src.GetRow(src.Height - 1);
        Span<TPixel> topRowAtlas = inAtlas.GetRow(0);
        Span<TPixel> botRowAtlas = inAtlas.GetRow(to.Height - 1);

        // copy top and bottom row
        topRowSrc.CopyTo(topRowAtlas[1..]);
        botRowSrc.CopyTo(botRowAtlas[1..]);

        // corners
        topRowAtlas[0] = topRowSrc[0];
        botRowAtlas[0] = botRowSrc[0];
        topRowAtlas[^1] = topRowSrc[^1];
        botRowAtlas[^1] = botRowSrc[^1];

        // copy vertical lanes
        for (int y = 0; y < src.Height; y++) {
            Span<TPixel> srcRow = src.GetRow(y);
            Span<TPixel> atlasRow = inAtlas.GetRow(y + 1);
            atlasRow[0] = srcRow[0];
            atlasRow[^1] = srcRow[^1];
        }
    }
}
