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
  raylib::Vector2 pos;
  raylib::Vector2 vel;
  float rotation = 0;
  Bunny() {
    pos = raylib::Vector2(0, 0);
    vel.x = RandomFloat(0, 10);
    vel.y = RandomFloat(-5, 5);
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

    // Update bunnies
    for (auto&& bunny : bunnies) {
      bunny.pos += bunny.vel;
      bunny.vel.y += gravity;

      if (bunny.pos.x > maxX) {
        bunny.vel.x *= -1;
        bunny.pos.x = maxX;
      } else if (bunny.pos.x < minX) {
        bunny.vel.x *= -1;
        bunny.pos.x = minX;
      }

      if (bunny.pos.y > maxY) {
        bunny.vel.y *= -0.85;
        bunny.pos.y = maxY;
        bunny.rotation = RandomFloat(-6, 6);
        if (RandomFloat(0, 1) > 0) {
          bunny.vel.y -= RandomFloat(0, 6);
        }
      } else if (bunny.pos.y < minY) {
        bunny.vel.y = 0;
        bunny.pos.y = minY;
      }

      // Actually draw the bunny
      bunnyTex.Draw(bunny.pos, bunny.rotation);
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