namespace RectPacker2.Tests;

public class RectPackerUnitTests
{

    [Theory]
    [InlineData(1, 1)]
    [InlineData(8, 8)]
    [InlineData(16, 8)]
    [InlineData(32, 32)]
    public void Packer_CanPackSingle(int width, int height)
    {
        var packer = new Packer(512, 512);
        Assert.True(packer.TryPack(width, height, out _), $"Packer should fit {width}x{height}");
    }

    [Theory]
    [InlineData(1, 1, 512)]
    [InlineData(8, 8, 64)]
    [InlineData(16, 8, 32)]
    [InlineData(32, 32, 32)]
    public void Packer_CanPackMultiple(int width, int height, int count) {
        var packer = new Packer(512, 512);
        for(int i = 0; i < count; i++) {
            Assert.True(packer.TryPack(width, height, out _), $"Packer should fit {width}x{height} {count} times");
        }
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 0)]
    [InlineData(0, 1)]
    [InlineData(-1, -1)]
    public void Packer_ShouldNotAllowEmptyOrNegative(int width, int height) {
        var packer = new Packer(512, 512);
        Assert.False(packer.TryPack(width, height, out _));
    }
}