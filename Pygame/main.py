from dataclasses import dataclass
import random
import sys
import pygame


def main():
    @dataclass
    class Bunny:
        x: float
        y: float
        vx: float
        vy: float
        rot: float

    minX = 0
    maxX = 800 - 26
    minY = 0
    maxY = 600 - 37

    gravity = 0.5

    bunnies = []

    pygame.init()

    clock = pygame.time.Clock()

    # text
    pygame.font.init()
    font = pygame.font.SysFont('Arial', 16)
    fpsText = font.render('FPS: 0', True, (255, 255, 255))
    counterText = font.render('Bunnies: 0', True, (255, 255, 255))

    pygame.display.set_caption("Pygame Bunnymark")
    screen = pygame.display.set_mode((800, 600))

    bunnySprite = pygame.image.load('bunny.png')

    for i in range(10):
        bunny = Bunny(minX, minY, random.uniform(
            0, 10), random.uniform(-5, 5), 0)
        bunnies.append(bunny)

    # main loop
    while True:
        screen.fill((0, 191, 255))

        if clock.get_fps() > 59:
            for i in range(10):
                bunny = Bunny(minX, minY, random.uniform(
                    0, 10), random.uniform(-5, 5), 0)
                bunnies.append(bunny)

        for bunny in bunnies:
            bunny.x += bunny.vx
            bunny.y += bunny.vy
            bunny.vy += gravity

            if bunny.x > maxX:
                bunny.vx *= -1
                bunny.x = maxX
            elif bunny.x < minX:
                bunny.vx *= -1
                bunny.x = minX

            if bunny.y > maxY:
                bunny.vy *= -0.85
                bunny.y = maxY
                bunny.rot = random.uniform(-0.1, 0.1)
                if random.uniform(0, 1) > 0.5:
                    bunny.vy -= random.uniform(0, 6)
            elif bunny.y < minY:
                bunny.vy = 0
                bunny.y = minY

            screen.blit(pygame.transform.rotate(
                bunnySprite, bunny.rot), (bunny.x, bunny.y))

        pygame.draw.rect(screen, (0, 0, 0), pygame.Rect(0, 0, 135, 52))
        fpsText = font.render(
            f'FPS: {int(clock.get_fps())}', True, (255, 255, 255))
        screen.blit(fpsText, (5, 5))
        counterText = font.render(
            f'Bunnies: {len(bunnies)}', True, (255, 255, 255))

        screen.blit(counterText, (5, 28))

        # check for quit
        for event in pygame.event.get():
            if event.type == pygame.QUIT:
                pygame.quit()
                sys.exit()

        pygame.display.update()
        clock.tick()


# run the main function only if this module is executed as the main script
# (if you import this as a module then nothing is executed)
if __name__ == "__main__":
    # call the main function
    main()
