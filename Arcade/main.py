from random import uniform
import arcade

SCREEN_WIDTH = 800
SCREEN_HEIGHT = 600
SCREEN_TITLE = "Arcade Bunnymark"

MINX = 13
MAXX = 787
MINY = 582
MAXY = 18

GRAVITY = 0.5


class MyGame(arcade.Window):
    def __init__(self, width, height, title):
        super().__init__(width, height, title)
        arcade.set_background_color(arcade.color.PICTON_BLUE)
        self.set_update_rate(1/120)

        self.bunnies = arcade.SpriteList()

        self.FPSText = arcade.Text(
            "FPS: 0",
            5,
            583,
            arcade.color.WHITE,
            12,
            width=SCREEN_WIDTH,
        )

        self.BunniesText = arcade.Text(
            "Bunnies: 0",
            5,
            563,
            arcade.color.WHITE,
            12,
            width=SCREEN_WIDTH,
        )

    def setup(self):
        for i in range(0, 10):
            bunny = arcade.Sprite("bunny.png")
            bunny.center_x = MINX
            bunny.center_y = MINY
            bunny.change_x = uniform(0, 10)
            bunny.change_y = uniform(-5, 5)
            self.bunnies.append(bunny)

    def on_draw(self):
        self.clear()
        self.bunnies.draw()
        arcade.draw_rectangle_filled(0, 600, 230, 90, arcade.color.BLACK)
        self.FPSText.draw()
        self.BunniesText.draw()

    def on_update(self, delta_time):
        fps = 1/delta_time
        self.FPSText.text = "FPS: " + str(round(fps))

        for bunny in self.bunnies:
            bunny.center_x += bunny.change_x
            bunny.center_y += bunny.change_y
            bunny.change_y -= GRAVITY

            if bunny.center_x > MAXX:
                bunny.change_x *= -1
                bunny.center_x = MAXX
            elif bunny.center_x < MINX:
                bunny.change_x *= -1
                bunny.center_x = MINX

            if bunny.center_y < MAXY:
                bunny.change_y *= -0.85
                bunny.center_y = MAXY
                bunny.angle = uniform(-6, 6)
                if uniform(0, 1) > 0.5:
                    bunny.change_y += uniform(0, 6)
            elif bunny.center_y > MINY:
                bunny.change_y = 0
                bunny.center_y = MINY

        if fps > 59:
            for i in range(0, 100):
                bunny = arcade.Sprite("bunny.png")
                bunny.center_x = MINX
                bunny.center_y = MINY
                bunny.change_x = uniform(0, 10)
                bunny.change_y = uniform(-5, 5)
                self.bunnies.append(bunny)
            self.BunniesText.text = "Bunnies: " + str(len(self.bunnies))


def main():
    game = MyGame(SCREEN_WIDTH, SCREEN_HEIGHT, SCREEN_TITLE)
    game.setup()
    arcade.run()


if __name__ == "__main__":
    main()
