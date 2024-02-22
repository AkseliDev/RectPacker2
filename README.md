# RectPacker2
The updated version of the rect packing library used for sprite packing. It is used to pack rectangles which represent a sprite's source in a sprite atlas into a bigger area as tight as possible. The updated algorithm packs more sprites tightly and supports more features for future updates. The library now contains a generic sprite atlas implementation that can be used to generate atlas images. The library does not ship any texture generation and is cross platform + graphics api independent.

## Example usage

We use a MonoGame example to pack sprites of different sizes up to 32x32 and generate a preview of the sprite atlas.

![image](https://github.com/AkseliDev/RectPacker2/assets/96961979/9f9436ab-a121-4440-a745-97fdf4892f06)

The program tries to pack as many random sprites until its full. With this exact seed and settings, the packer was able to pack 1369 sprites in total into an area of 512x512.

See [Tests/MonoGame](https://github.com/AkseliDev/RectPacker2/tree/master/RectPacker2.Tests.MonoGame) for more information
 
