using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RectPacker2.Tests;

public class SpriteAtlasUnitTests {

    [Theory]
    [InlineData(8, 8)]
    [InlineData(16, 8)]
    [InlineData(16, 16)]
    [InlineData(32, 32)]
    [InlineData(128, 256)]
    public void SpriteAtlas_CanPackSingleImage(int width, int height) {
        var atlas = new SpriteAtlas<int>(512, 512);
        var img = new RawImageData<int>(new int[width * height], width, height);
        Assert.True(atlas.TryPack(img, out _));
    }

    [Theory]
    [InlineData(8, 8, 32)]
    [InlineData(16, 8, 16)]
    [InlineData(16, 16, 16)]
    [InlineData(32, 32, 8)]
    [InlineData(128, 256, 2)]
    public void SpriteAtlas_CanPackMultipleImages(int width, int height, int count) {
        var atlas = new SpriteAtlas<int>(512, 512);
        var img = new RawImageData<int>(new int[width * height], width, height);
        for(int i = 0; i < count; i++) {
            Assert.True(atlas.TryPack(img, out _));
        }
    }

    [Fact]
    public void SpriteAtlas_ShouldThrow_WhenFull() {
        Assert.Throws<Exception>(() => {
            var atlas = new SpriteAtlas<int>(8, 8);
            atlas.Pack(new RawImageData<int>(new int[256], 16, 16));
        });
    }
}