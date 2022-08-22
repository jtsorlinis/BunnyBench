import Phaser from "phaser";
import Bunny from "./Bunny";

export default class Demo extends Phaser.Scene {
  startBunnyCount = 10;
  count = 0;
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
      .setDepth(2);
    this.counter = this.add.text(5, 25, `Bunnies: ${this.count}`).setDepth(2);
    this.add.rectangle(0, 0, 150, 45, 0x000000).setOrigin(0, 0).setDepth(1);

    // Bunnies
    for (var i = 0; i < this.startBunnyCount; i++) {
      new Bunny(this);
    }
  }

  update(time: number, delta: number): void {
    this.fps.text = `FPS: ${this.game.loop.actualFps.toFixed(1)}`;

    // Add bunnies while over 59fps
    if ((1 / delta) * 1000 > 59) {
      if (this.count < 200000) {
        for (var i = 0; i < 100; i++) {
          new Bunny(this);
          this.count++;
        }
      }
      this.counter.text = `Bunnies: ${this.count}`;
    }
  }
}
