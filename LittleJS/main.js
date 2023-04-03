const minX = -24.4;
const maxX = -minX;
const minY = 17.8;
const maxY = -minY;

const gravity = 0.03;

const bunnies = [];

const textColour = new Color(1, 1, 1);
const fpsTextPos = vec2(-24.8, 18);
const bunnyTextPos = vec2(-24.8, 16.8);
let showFPS, timeMS;

function gameInit() {
  mainCanvas.style.background = "#555";
  mainCanvas.style.width = "800px";
  mainCanvas.style.height = "600px";

  for (let i = 0; i < 10; i++) {
    let bunny = {
      pos: vec2(minX, minY),
      vel: vec2(rand(0, 0.6), rand(-0.3, 0.3)),
      rot: 0,
    };
    bunnies.push(bunny);
  }
}

function gameUpdate() {
  console.log(1 / timeDelta);
}

function gameUpdatePost() {}

function gameRender() {
  const frameTimeMS = Date.now();
  showFPS = lerp(0.05, showFPS, 1e3 / (frameTimeMS - timeMS || 1));
  timeMS = frameTimeMS;

  // Add bunnies while over 59fps
  if (showFPS > 59) {
    for (let i = 0; i < 100; i++) {
      let bunny = {
        pos: vec2(minX, minY),
        vel: vec2(rand(0, 0.6), rand(-0.3, 0.3)),
        rot: 0,
      };
      bunnies.push(bunny);
    }
  }

  // Move bunnies
  for (const bunny of bunnies) {
    bunny.pos.x += bunny.vel.x;
    bunny.pos.y += bunny.vel.y;
    bunny.vel.y -= gravity;

    if (bunny.pos.x > maxX) {
      bunny.vel.x *= -1;
      bunny.pos.x = maxX;
    } else if (bunny.pos.x < minX) {
      bunny.vel.x *= -1;
      bunny.pos.x = minX;
    }

    if (bunny.pos.y < maxY) {
      bunny.vel.y *= -0.85;
      bunny.pos.y = maxY;
      bunny.rot = rand(-0.1, 0.1);
      if (rand() > 0.5) {
        bunny.vel.y += rand(0, 0.4);
      }
    } else if (bunny.pos.y > minY) {
      bunny.vel.y = 0;
      bunny.pos.y = minY;
    }

    glDraw(bunny.pos.x, bunny.pos.y, 1.5, 2, bunny.rot, 0, 0, 1, 1);
  }

  // Draw UI
  drawRect(vec2(-21.5, 17.4), vec2(9, 3), new Color(0, 0, 0));
  drawText(
    "FPS: " + showFPS.toFixed(2),
    fpsTextPos,
    1,
    textColour,
    0,
    textColour,
    "left"
  );
  drawText(
    "Bunnies: " + bunnies.length,
    bunnyTextPos,
    1,
    textColour,
    0,
    textColour,
    "left"
  );
}

function gameRenderPost() {}

engineInit(
  gameInit,
  gameUpdate,
  gameUpdatePost,
  gameRender,
  gameRenderPost,
  "bunny.png"
);
