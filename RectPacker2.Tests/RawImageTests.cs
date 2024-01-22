using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RectPacker2.Tests;

public class RawImageTests {

    private struct Color { }

    [Theory]
    [InlineData(4,4, 3,3)]
    [InlineData(8,8, 4,4)]
    [InlineData(16,16, 8,32)]
    public void RawImageData_ShouldThrow_CopyOutOfBounds(int width1, int height1, int width2, int height2) {
        Assert.Throws<ArgumentException>(() => {
            var data0 = new Color[width1 * height1];
            var data1 = new Color[width2 * height2];

            new RawImageData<Color>(data0, width1, height1).CopyTo(new RawImageData<Color>(data1, width2, height2));
        });
    }

    [Fact]
    public void RawImageSlice_ShouldThrow_CopyOutOfBounds() {
        Assert.Throws<ArgumentException>(() => {

            var img0 = new RawImageData<Color>(new Color[64], 8, 8);
            var img1 = new RawImageData<Color>(new Color[64], 8, 8);

            img0.Slice(0, 0, 4, 4).CopyTo(img1.Slice(5, 5, 3, 3));
        });
    }

    [Theory]
    [InlineData(0,0,0,0)]
    [InlineData(0,0,9,9)]
    [InlineData(1,1,8,8)]
    [InlineData(-1,-1,8,8)]
    [InlineData(0,0,-1,-1)]
    public void RawImageSlice_ShouldThrow_SliceOutOfBounds(int x, int y, int w, int h) {
        Assert.Throws<ArgumentException>(() => {

            var img0 = new RawImageData<Color>(new Color[64], 8, 8);
            img0.Slice(x,y,w,h
                );
        });
    }

    [Theory]
    [InlineData(8, 8)]
    [InlineData(16, 16)]
    [InlineData(64, 64)]
    public void RawImageData_Copying(int width, int height) {
        var data = new int[width * height];
        var data2 = new int[width * height];
        Array.Fill(data, 1);

        var img = new RawImageData<int>(data2, width, height);
        new RawImageData<int>(data, width, height).CopyTo(img);

        Assert.True(data2.SequenceEqual(data));
    }
}