using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using System;
using System.Collections.Generic;

namespace MonoGame;

public class Game1 : Game
{
  private GraphicsDeviceManager _graphics;
  private Texture2D bunnySprite;
  private SpriteBatch _spriteBatch;
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
  }

  protected override void Update(GameTime gameTime)
  {
    var fps = 1 / gameTime.ElapsedGameTime.TotalSeconds;
    Console.WriteLine("FPS: " + Math.Round(fps) + "\t Bunnies: " + bunnies.Count);

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
        bunny.rot = r.NextSingle() * 12 - 6;
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
      _spriteBatch.Draw(bunnySprite, bunnies[i].pos, Color.White);

    }
    _spriteBatch.End();


    base.Draw(gameTime);
  }
}
