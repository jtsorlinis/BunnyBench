import * as PIXI from "pixi.js";

type Bunny = PIXI.Sprite & { speedX: number; speedY: number };

let app = new PIXI.Application({
  width: 800,
  height: 600,
  backgroundColor: 0x00b5e2,
});
document.body.appendChild(app.view);

const uiContainer = new PIXI.Container();
const bunnyContainer = new PIXI.ParticleContainer(200000, {});

app.stage.addChild(bunnyContainer);
app.stage.addChild(uiContainer);

const bunnyTex = PIXI.Texture.from("bunny.png");

// UI
const rectangle = PIXI.Sprite.from(PIXI.Texture.WHITE);
rectangle.width = 140;
rectangle.height = 48;
rectangle.tint = 0x000000;
rectangle.zIndex = 1000;
uiContainer.addChild(rectangle);

let fpsText = new PIXI.Text("FPS: 0", {
  fontSize: 16,
  fill: 0xffffff,
});
fpsText.x = 5;
fpsText.y = 5;
uiContainer.addChild(fpsText);

let countText = new PIXI.Text("Bunnies: 0", {
  fontSize: 16,
  fill: 0xffffff,
});
countText.x = 5;
countText.y = 25;
uiContainer.addChild(countText);

uiContainer.zIndex = 1000;

const gravity = 0.5;
const minX = 0;
const maxX = app.screen.width - 26;
const minY = 0;
const maxY = app.screen.height - 37;

const bunnies: any[] = [];

for (let i = 0; i < 10; i++) {
  let bunny = new PIXI.Sprite(bunnyTex) as Bunny;
  bunny.x = minX;
  bunny.y = minY;
  bunny.speedX = Math.random() * 10;
  bunny.speedY = Math.random() * 10 - 5;
  bunnyContainer.addChild(bunny);
  bunnies.push(bunny);
}

app.ticker.add(() => {
  fpsText.text = "FPS: " + PIXI.Ticker.system.FPS.toFixed(1);
  // Add bunnies
  if (PIXI.Ticker.system.FPS > 59) {
    for (let i = 0; i < 100; i++) {
      let bunny = new PIXI.Sprite(bunnyTex) as Bunny;
      bunny.x = minX;
      bunny.y = minY;
      bunny.speedX = Math.random() * 10;
      bunny.speedY = Math.random() * 10 - 5;
      bunnyContainer.addChild(bunny);
      bunnies.push(bunny);
    }
    countText.text = "Bunnies: " + bunnies.length;
  }

  // Move bunnies
  for (let i = 0; i < bunnies.length; i++) {
    let bunny = bunnies[i];
    bunny.x += bunny.speedX;
    bunny.y += bunny.speedY;
    bunny.speedY += gravity;

    if (bunny.x > maxX) {
      bunny.speedX *= -1;
      bunny.x = maxX;
    } else if (bunny.x < minX) {
      bunny.speedX *= -1;
      bunny.x = minX;
    }

    if (bunny.y > maxY) {
      bunny.speedY *= -0.85;
      bunny.y = maxY;
      bunny.rotation = (Math.random() - 0.5) * 0.2;
      if (Math.random() > 0.5) {
        bunny.speedY -= Math.random() * 6;
      }
    } else if (bunny.y < minY) {
      bunny.speedY = 0;
      bunny.y = minY;
    }
  }
});
