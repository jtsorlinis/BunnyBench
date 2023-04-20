import { VertexBuffer } from "@babylonjs/core/Buffers/buffer";
import "./style.css";
import { FreeCamera } from "@babylonjs/core/Cameras/freeCamera";
import { Engine } from "@babylonjs/core";
import { Texture } from "@babylonjs/core/Materials/Textures/texture";
import { ShaderMaterial } from "@babylonjs/core/Materials/shaderMaterial";
import { Vector3 } from "@babylonjs/core/Maths/math.vector";
import { CreatePlane } from "@babylonjs/core/Meshes/Builders/planeBuilder";
import { Scene, ScenePerformancePriority } from "@babylonjs/core/scene";

let numBunnies = 10;
const bufferSize = 1000000;
const maxSpeed = 0.13;
const gravity = 0.007;

const canvas = document.getElementById("renderCanvas") as HTMLCanvasElement;
const bunnyText = document.getElementById("bunnyText") as HTMLElement;
const fpsText = document.getElementById("fpsText") as HTMLElement;
const engine = new Engine(canvas, false);
engine.setSize(800, 600);

var scene = new Scene(engine);

var camera = new FreeCamera("camera1", new Vector3(0, 0, -5), scene);
const orthoSize = 5;
const aspectRatio = 1.3333;
camera.mode = 1;
camera.orthoBottom = -orthoSize;
camera.orthoTop = orthoSize;
camera.orthoLeft = -orthoSize * aspectRatio;
camera.orthoRight = orthoSize * aspectRatio;

const xBound = orthoSize * aspectRatio - 0.05;
const yBound = orthoSize - 0.07;

const bunnyPositions = new Float32Array(bufferSize * 3);
const bunnyVelocities = new Float32Array(bufferSize * 2);
for (let i = 0; i < bufferSize; ++i) {
  // Position
  bunnyPositions[3 * i + 0] = -xBound;
  bunnyPositions[3 * i + 1] = yBound;

  // Rotation
  bunnyPositions[3 * i + 2] = 0;

  // Velocity
  bunnyVelocities[2 * i + 0] = Math.random() * maxSpeed;
  bunnyVelocities[2 * i + 1] = (Math.random() - 0.5) * maxSpeed;
}

const bunnyPosBuffer = new VertexBuffer(
  engine,
  bunnyPositions,
  "bunnyPos",
  true,
  false,
  3,
  true
);

// Load texture and materials
const bunnyTexture = new Texture("bunny.png", scene);
const bunnyMat = new ShaderMaterial("bunnyMat", scene, "./bunnyShader", {
  attributes: ["position", "uv", "bunnyPos"],
  uniforms: ["worldViewProjection"],
});
bunnyMat.setTexture("bunnyTexture", bunnyTexture);
bunnyMat.alpha = 0;

// Create bunny mesh
const bunnyMesh = CreatePlane("bunnyMesh", { width: 0.3, height: 0.5 }, scene);
bunnyMesh.material = bunnyMat;
bunnyMesh.forcedInstanceCount = numBunnies;
bunnyMesh.setVerticesBuffer(bunnyPosBuffer, false);

engine.runRenderLoop(() => {
  const fps = engine.getFps();
  fpsText.innerHTML = `FPS: ${fps.toFixed(2)}`;

  for (let i = 0; i < numBunnies; ++i) {
    bunnyPositions[i * 3] += bunnyVelocities[i * 2];
    bunnyPositions[i * 3 + 1] += bunnyVelocities[i * 2 + 1];
    bunnyVelocities[i * 2 + 1] -= gravity;

    if (bunnyPositions[i * 3] > xBound) {
      bunnyVelocities[i * 2] *= -1;
      bunnyPositions[i * 3] = xBound;
    } else if (bunnyPositions[i * 3] < -xBound) {
      bunnyVelocities[i * 2] *= -1;
      bunnyPositions[i * 3] = -xBound;
    }

    if (bunnyPositions[i * 3 + 1] < -yBound) {
      bunnyVelocities[i * 2 + 1] *= -0.85;
      bunnyPositions[i * 3 + 1] = -yBound;
      bunnyPositions[i * 3 + 2] = (Math.random() - 0.5) * 0.2;
      if (Math.random() > 0.5) {
        bunnyVelocities[i * 2 + 1] += Math.random() * 0.1;
      }
    } else if (bunnyPositions[i * 3 + 1] > yBound) {
      bunnyVelocities[i * 2 + 1] = 0;
      bunnyPositions[i * 3 + 1] = yBound;
    }
  }
  bunnyPosBuffer.update(bunnyPositions);

  scene.render();
  if (fps > 59) {
    numBunnies += 1000;
    bunnyMesh.forcedInstanceCount = numBunnies;
    bunnyText.innerHTML = `Bunnies: ${numBunnies}`;
  }
});
