import { VertexBuffer } from "@babylonjs/core/Buffers/buffer";
import "./style.css";
import { FreeCamera } from "@babylonjs/core/Cameras/freeCamera";
import { Engine } from "@babylonjs/core";
import { Texture } from "@babylonjs/core/Materials/Textures/texture";
import { ShaderMaterial } from "@babylonjs/core/Materials/shaderMaterial";
import { Vector3 } from "@babylonjs/core/Maths/math.vector";
import { CreatePlane } from "@babylonjs/core/Meshes/Builders/planeBuilder";
import { Scene } from "@babylonjs/core/scene";

let numBunnies = 10;
const bufferSize = 1000000;
const maxSpeed = 0.13;
const gravity = 0.007;

const canvas = document.getElementById("renderCanvas") as HTMLCanvasElement;
const bunnyText = document.getElementById("bunnyText") as HTMLElement;
const fpsText = document.getElementById("fpsText") as HTMLElement;
const engine = new Engine(canvas);
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

const stride = 5;
const bunniesData = new Float32Array(bufferSize * stride);
for (let i = 0; i < bufferSize; ++i) {
  // Position
  bunniesData[stride * i + 0] = -xBound;
  bunniesData[stride * i + 1] = yBound;

  // Rotation
  bunniesData[stride * i + 2] = 0;

  // Velocity
  bunniesData[stride * i + 3] = Math.random() * maxSpeed;
  bunniesData[stride * i + 4] = (Math.random() - 0.5) * maxSpeed;
}

const bunnyPosBuffer = new VertexBuffer(
  engine,
  bunniesData,
  "bunnyPos",
  true,
  false,
  stride,
  true,
  0,
  3
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
    bunniesData[stride * i + 0] += bunniesData[stride * i + 3];
    bunniesData[stride * i + 1] += bunniesData[stride * i + 4];
    bunniesData[stride * i + 4] -= gravity;

    if (bunniesData[stride * i + 0] > xBound) {
      bunniesData[stride * i + 0] = xBound;
      bunniesData[stride * i + 3] *= -1;
    } else if (bunniesData[stride * i + 0] < -xBound) {
      bunniesData[stride * i + 0] = -xBound;
      bunniesData[stride * i + 3] *= -1;
    }

    if (bunniesData[stride * i + 1] < -yBound) {
      bunniesData[stride * i + 1] = -yBound;
      bunniesData[stride * i + 4] *= -0.85;
      bunniesData[stride * i + 2] = Math.random() * 0.2 - 0.1;
      if (Math.random() > 0.5) {
        bunniesData[stride * i + 4] += Math.random() * 0.1;
      }
    } else if (bunniesData[stride * i + 1] > yBound) {
      bunniesData[stride * i + 1] = yBound;
      bunniesData[stride * i + 4] = 0;
    }
  }
  bunnyPosBuffer.update(bunniesData);

  scene.render();
  if (fps > 59) {
    numBunnies += 1000;
    bunnyMesh.forcedInstanceCount = numBunnies;
    bunnyText.innerHTML = `Bunnies: ${numBunnies}`;
  }
});
