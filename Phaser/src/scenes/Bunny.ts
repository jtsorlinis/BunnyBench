import Phaser from "phaser";

const gravity = 0.5;
const minX = 0;
const maxX = 800;
const minY = 0;
const maxY = 600;

export default class Bunny extends Phaser.GameObjects.Sprite {
  speedX = Math.random() * 10;
  speedY = Math.random() * 10 - 5;

  constructor(scene: Phaser.Scene) {
    super(scene, 10, 10, "bunny");
    // speedX = Math.random() * 10;
    // speedY = Math.random() * 10 - 5;
    this.setOrigin(0.5, 1);
    scene.add.existing(this);
  }

  preUpdate() {
    this.x += this.speedX;
    this.y += this.speedY;
    this.speedY += gravity;

    if (this.x > maxX) {
      this.speedX *= -1;
      this.x = maxX;
    } else if (this.x < minX) {
      this.speedX *= -1;
      this.x = minX;
    }

    if (this.y > maxY) {
      this.speedY *= -0.85;
      this.y = maxY;
      this.rotation = (Math.random() - 0.5) * 0.2;
      if (Math.random() > 0.5) {
        this.speedY -= Math.random() * 6;
      }
    } else if (this.y < minY) {
      this.speedY = 0;
      this.y = minY;
    }
  }
}
