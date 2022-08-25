#include <vector>

#include "raylib-cpp.hpp"

float RandomFloat(float a, float b) {
  float random = (static_cast<float>(rand())) / static_cast<float>(RAND_MAX);
  float diff = b - a;
  float r = random * diff;
  return a + r;
}

class Bunny {
 public:
  float posX = 0;
  float posY = 0;
  float speedX;
  float speedY;
  Bunny() {
    speedX = RandomFloat(0, 10);
    speedY = RandomFloat(-5, 5);
  }
};

int main() {
  int screenWidth = 800;
  int screenHeight = 600;
  float gravity = 0.5;
  int minX = 0;
  int minY = 0;

  raylib::Window window(screenWidth, screenHeight, "raylib Bunnymark");
  raylib::Texture bunnyTex("assets/bunny.png");

  int maxX = screenWidth - bunnyTex.width;
  int maxY = screenHeight - bunnyTex.height;

  // SetTargetFPS(GetMonitorRefreshRate(0));

  std::vector<Bunny> bunnies;

  // Spawn initial bunnies
  for (int i = 0; i < 10; i++) {
    bunnies.emplace_back();
  }

  while (!window.ShouldClose()) {
    // Add bunnies while over 59fps
    if (GetFPS() > 59) {
      for (int i = 0; i < 100; i++) {
        bunnies.emplace_back();
      }
    }

    BeginDrawing();
    window.ClearBackground(RAYWHITE);

    // Draw bunnies
    for (auto& bunny : bunnies) {
      bunny.posX += bunny.speedX;
      bunny.posY += bunny.speedY;
      bunny.speedY += gravity;
      bunnyTex.Draw(bunny.posX, bunny.posY);

      if (bunny.posX > maxX) {
        bunny.speedX *= -1;
        bunny.posX = maxX;
      } else if (bunny.posX < minX) {
        bunny.speedX *= -1;
        bunny.posX = minX;
      }

      if (bunny.posY > maxY) {
        bunny.speedY *= -0.85;
        bunny.posY = maxY;
        if (RandomFloat(0, 1) > 0) {
          bunny.speedY -= RandomFloat(0, 6);
        }
      } else if (bunny.posY < minY) {
        bunny.speedY = 0;
        bunny.posY = minY;
      }
    }
    DrawRectangle(0, 0, 180, 55, BLACK);
    raylib::DrawText(TextFormat("FPS: %i", GetFPS()), 5, 5, 20, WHITE);
    raylib::DrawText(TextFormat("Bunnies: %i", bunnies.size()), 5, 30, 20,
                     WHITE);
    EndDrawing();
  }

  // UnloadTexture() and CloseWindow() are called automatically.
  return 0;
}