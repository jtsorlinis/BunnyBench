import Phaser from "phaser";

const gravity = 30;
const minX = 0;
const maxX = 800;
const minY = 0;
const maxY = 600;

export default class Demo extends Phaser.Scene {
  counter!: Phaser.GameObjects.Text;
  fps!: Phaser.GameObjects.Text;
  emitter!: Phaser.GameObjects.Particles.ParticleEmitter;
  constructor() {
    super("GameScene");
  }

  preload() {
    this.load.image("bunny", "./assets/bunny.png");
  }

  create() {
    // UI
    this.fps = this.add.text(5, 5, `FPS: 0`).setDepth(2);
    this.counter = this.add.text(5, 25, `Bunnies: 0`).setDepth(2);
    this.add.rectangle(0, 0, 150, 45, 0x000000).setOrigin(0, 0).setDepth(1);

    const particles = this.add.particles("bunny");
    this.emitter = particles.createEmitter({
      active: true,
      frequency: -1,
      x: 10,
      y: 10,
      lifespan: Number.MAX_VALUE,
    });
  }

  update(time: number, delta: number): void {
    this.fps.text = `FPS: ${this.game.loop.actualFps.toFixed(1)}`;
    this.emitter.forEachAlive((bunny) => {
      bunny.velocityY += gravity;

      if (bunny.x > maxX) {
        bunny.velocityX *= -1;
        bunny.x = maxX;
      } else if (bunny.x < minX) {
        bunny.velocityX *= -1;
        bunny.x = minX;
      }

      if (bunny.y > maxY) {
        bunny.velocityY *= -0.85;
        bunny.y = maxY;
        bunny.rotation = (Math.random() - 0.5) * 0.2;
        if (Math.random() > 0.5) {
          bunny.velocityY -= Math.random() * 360;
        }
      } else if (bunny.y < minY) {
        bunny.velocityY = 0;
        bunny.y = minY;
      }
    }, this);

    // // Add bunnies while over 59fps
    if ((1 / delta) * 1000 > 59) {
      // if (this.emitter.getParticleCount() < 50) {
      for (var i = 0; i < 100; i++) {
        const particle = this.emitter.emitParticle(1, 10, 10);
        particle.velocityX = Math.random() * 600;
        particle.velocityY = Math.random() * 600 - 300;
      }
      this.counter.text = `Bunnies: ${this.emitter.getParticleCount()}`;
    }
  }
}
