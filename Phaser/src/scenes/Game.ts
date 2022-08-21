import Phaser from "phaser";

export default class Demo extends Phaser.Scene {
  startBunnyCount = 10;
  count = 0;
  gravity = 0.5;
  minX = 0;
  maxX = 800;
  minY = 0;
  maxY = 600;
  bunnys: any[] = [];
  counter!: Phaser.GameObjects.Text;
  fps!: Phaser.GameObjects.Text;
  constructor() {
    super("GameScene");
  }

  preload() {
    this.load.image("bunny", "./assets/bunny.png");
  }

  create() {
    // UI
    this.count = this.startBunnyCount;
    this.fps = this.add
      .text(5, 5, `FPS: ${this.game.loop.actualFps}`)
      .setDepth(1);
    this.counter = this.add.text(5, 20, `Bunnies: ${this.count}`).setDepth(1);

    // Bunnies
    for (var i = 0; i < this.startBunnyCount; i++) {
      const bunny: any = this.add.sprite(10, 10, "bunny");
      bunny.speedX = Math.random() * 10;
      bunny.speedY = Math.random() * 10 - 5;
      bunny.setOrigin(0.5, 1);

      this.bunnys.push(bunny);
    }
  }

  update(time: number, delta: number): void {
    this.fps.text = `FPS: ${this.game.loop.actualFps}`;

    // Add bunnies while over 60fps
    if (1 / delta > 0.06) {
      if (this.count < 200000) {
        for (var i = 0; i < 100; i++) {
          const bunny: any = this.add.sprite(10, 10, "bunny");
          bunny.speedX = Math.random() * 10;
          bunny.speedY = Math.random() * 10 - 5;
          bunny.setOrigin(0.5, 1);
          this.bunnys.push(bunny);

          this.count++;
        }
      }
      this.counter.text = `Bunnies: ${this.count}`;
    }

    // move bunnys
    for (let i = 0; i < this.bunnys.length; i++) {
      const bunny = this.bunnys[i];

      bunny.x += bunny.speedX;
      bunny.y += bunny.speedY;
      bunny.speedY += this.gravity;

      if (bunny.x > this.maxX) {
        bunny.speedX *= -1;
        bunny.x = this.maxX;
      } else if (bunny.x < this.minX) {
        bunny.speedX *= -1;
        bunny.x = this.minX;
      }

      if (bunny.y > this.maxY) {
        bunny.speedY *= -0.85;
        bunny.y = this.maxY;
        bunny.rotation = (Math.random() - 0.5) * 0.2;
        if (Math.random() > 0.5) {
          bunny.speedY -= Math.random() * 6;
        }
      } else if (bunny.y < this.minY) {
        bunny.speedY = 0;
        bunny.y = this.minY;
      }
    }
  }
}
