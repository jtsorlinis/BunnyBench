using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System;
using System.Collections.Generic;
using SpriteFontPlus;

namespace MonoGame;

public class Game1 : Game
{
  private GraphicsDeviceManager _graphics;
  private Texture2D bunnySprite;
  private SpriteFont font;
  private SpriteBatch _spriteBatch;
  double fps;
  Texture2D blackRectangle;
  Random r = new Random();

  private float minX = 0;
  private float maxX = 775;
  private float minY = 0;
  private float maxY = 565;
  private float gravity = 0.5f;

  class Bunny
  {
    public Vector2 vel;
    public Vector2 pos;
    public float rot;
  }

  List<Bunny> bunnies = new List<Bunny>();

  public Game1()
  {
    _graphics = new GraphicsDeviceManager(this);
    _graphics.PreferredBackBufferHeight = 600;
    _graphics.PreferredBackBufferWidth = 800;
    _graphics.SynchronizeWithVerticalRetrace = false;
    _graphics.ApplyChanges();
    this.IsFixedTimeStep = false;
    Content.RootDirectory = "Content";

  }

  protected override void Initialize()
  {
    for (int i = 0; i < 10; i++)
    {
      var bunny = new Bunny();
      bunny.vel = new Vector2(r.NextSingle() * 10, r.NextSingle() * 10 - 5);
      bunny.pos = new Vector2();
      bunnies.Add(bunny);
    }
    base.Initialize();
  }

  protected override void LoadContent()
  {
    _spriteBatch = new SpriteBatch(GraphicsDevice);

    FileStream fileStream = new FileStream("Content/bunny.png", FileMode.Open);
    bunnySprite = Texture2D.FromStream(GraphicsDevice, fileStream);
    fileStream.Dispose();

    blackRectangle = new Texture2D(GraphicsDevice, 1, 1);
    blackRectangle.SetData(new[] { Color.Black });

    var fontBakeResult = TtfFontBaker.Bake(File.ReadAllBytes("Content/PTRoot.ttf"),
      20,
      1024,
      1024,
      new[] {
        CharacterRange.BasicLatin,
        CharacterRange.Latin1Supplement,
        CharacterRange.LatinExtendedA,
        CharacterRange.Cyrillic
      });
    font = fontBakeResult.CreateSpriteFont(GraphicsDevice);
  }

  protected override void Update(GameTime gameTime)
  {
    fps = 1 / gameTime.ElapsedGameTime.TotalSeconds;

    // Add bunnies while over 59 fps
    if (fps > 59)
    {
      for (int i = 0; i < 100; i++)
      {
        var bunny = new Bunny();
        bunny.vel = new Vector2(r.NextSingle() * 10, r.NextSingle() * 10 - 5);
        bunny.pos = new Vector2();
        bunnies.Add(bunny);
      }
    }

    // move bunnies
    for (int i = 0; i < bunnies.Count; i++)
    {
      var bunny = bunnies[i];
      bunny.pos += bunny.vel;
      bunny.vel.Y += gravity;

      if (bunny.pos.X > maxX)
      {
        bunny.vel.X *= -1;
        bunny.pos.X = maxX;
      }
      else if (bunny.pos.X < minX)
      {
        bunny.vel.X *= -1;
        bunny.pos.X = minX;
      }

      if (bunny.pos.Y > maxY)
      {
        bunny.vel.Y *= -0.85f;
        bunny.pos.Y = maxY;
        bunny.rot = r.NextSingle() * 0.2f - 0.1f;
        if (r.NextSingle() > 0.5)
        {
          bunny.vel.Y -= r.NextSingle() * 6;
        }
      }
      else if (bunny.pos.Y < minY)
      {
        bunny.vel.Y = 0;
        bunny.pos.Y = minY;
      }

    }

    base.Update(gameTime);
  }

  protected override void Draw(GameTime gameTime)
  {
    GraphicsDevice.Clear(Color.CornflowerBlue);

    _spriteBatch.Begin();
    for (int i = 0; i < bunnies.Count; i++)
    {
      _spriteBatch.Draw(bunnySprite, bunnies[i].pos, null, Color.White, bunnies[i].rot, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
      // _spriteBatch.Draw(bunnySprite, bunnies[i].pos, Color.White);
    }
    _spriteBatch.Draw(blackRectangle, new Rectangle(0, 0, 135, 50), Color.White);
    _spriteBatch.DrawString(font, "FPS: " + Math.Round(fps), new Vector2(5, 2), Color.White);
    _spriteBatch.DrawString(font, "Bunnies: " + bunnies.Count, new Vector2(5, 22), Color.White);
    _spriteBatch.End();


    base.Draw(gameTime);
  }
}
