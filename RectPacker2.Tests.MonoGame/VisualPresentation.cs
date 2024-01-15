using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RectPacker2.Tests.MonoGame;

record struct Sprite(Texture2D Texture, Area Area);

public class VisualPresentation : Game {

    const int Speed = 5;

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Packer _packer;
    private List<Sprite> _sprites;

    (int w, int h)[] Sizes;
    Texture2D[] Textures;
    Random Random;
    SpriteFont DefaultFont;

    private int _count;

    public VisualPresentation() {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _graphics.PreferredBackBufferWidth = 600;
        _graphics.PreferredBackBufferHeight = 600;
    }

    protected override void Initialize() {
        base.Initialize();

        _sprites = new List<Sprite>();
        _packer = new Packer(512, 512);
        Random = new Random(1337);


        Sizes = new[] {
            (10, 10),
            (10, 14),
            (10, 8),
            (10, 9),
            (14, 12),
            (15, 11),
            (15, 15),
            (16, 16),
            (16, 8),
            (4, 10),
            (6, 6),
            (7, 14),
            (8, 16),
            (8, 8),
            (9, 15),
            (32, 32)
        };

        Textures = new Texture2D[Sizes.Length];
        for(int i = 0; i < Sizes.Length; i++) {
            var (w, h) = Sizes[i];
            Textures[i] = Content.Load<Texture2D>($"{w}x{h}");
        }

        DefaultFont = Content.Load<SpriteFont>("DefaultFont");

        Task.Run(AddSprites);
    }

    private async void AddSprites() {
        while (true) {
            for (int i = 0; i < Speed; i++) {
                int idx = Random.Next(Sizes.Length);
                var (w, h) = Sizes[idx];

                if (!_packer.TryPack((ushort)w, (ushort)h, out var p)) {
                    return;
                }

                _sprites.Add(new Sprite(Textures[idx], p));
                _count++;
            }
            await Task.Delay(1);
        }
    }

    protected override void LoadContent() {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime) {
        base.Update(gameTime);
    }


    protected override void Draw(GameTime gameTime) {
        GraphicsDevice.Clear(new Color(255, 0, 255, 255));
        _spriteBatch.Begin();

        for(int i = _sprites.Count - 1; i >= 0; i--) {
            var sprite = _sprites[i];
            _spriteBatch.Draw(sprite.Texture, new Rectangle(sprite.Area.X, sprite.Area.Y, sprite.Area.Width, sprite.Area.Height), Color.White);
        }

        _spriteBatch.DrawString(DefaultFont, $"Sprites packed: {_count}", new Vector2(0, 520), Color.White);
        _spriteBatch.DrawRectangle(new RectangleF(0,0,_packer.TotalWidth + 1, _packer.TotalHeight + 1), Color.Black, 1);
        _spriteBatch.End();
        base.Draw(gameTime);
    }
}
