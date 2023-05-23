import "./style.css";
import { FreeCamera } from "@babylonjs/core/Cameras/freeCamera";
import {
  ComputeShader,
  Constants,
  ShaderLanguage,
  TextureSampler,
  UniformBuffer,
  WebGPUEngine,
} from "@babylonjs/core";
import { Texture } from "@babylonjs/core/Materials/Textures/texture";
import { ShaderMaterial } from "@babylonjs/core/Materials/shaderMaterial";
import { Vector3 } from "@babylonjs/core/Maths/math.vector";
import { CreatePlane } from "@babylonjs/core/Meshes/Builders/planeBuilder";
import { Scene } from "@babylonjs/core/scene";
import { StorageBuffer } from "@babylonjs/core/Buffers/storageBuffer";
import bunnyVertexSource from "./shaders/bunnyVertex.wgsl?raw";
import bunnyFragmentSource from "./shaders/bunnyFragment.wgsl?raw";
import bunnyComputeSource from "./shaders/bunnyCompute.wgsl?raw";

let numBunnies = 10;
const bufferSize = 4000000;
const maxSpeed = 0.13;
const gravity = 0.007;

const canvas = document.getElementById("renderCanvas") as HTMLCanvasElement;
const bunnyText = document.getElementById("bunnyText") as HTMLElement;
const fpsText = document.getElementById("fpsText") as HTMLElement;
const engine = new WebGPUEngine(canvas, { antialias: false });
await engine.initAsync();
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

const xBound = orthoSize * aspectRatio - 0.12;
const yBound = orthoSize - 0.25;

const stride = 8;
const bunniesData = new Float32Array(bufferSize * stride);
for (let i = 0; i < bufferSize; ++i) {
  // Position
  bunniesData[stride * i + 0] = -xBound;
  bunniesData[stride * i + 1] = yBound;

  // Rotation
  bunniesData[stride * i + 2] = 0;

  // Velocity
  bunniesData[stride * i + 4] = Math.random() * maxSpeed;
  bunniesData[stride * i + 5] = (Math.random() - 0.5) * maxSpeed;
}

const bunnyMat = new ShaderMaterial(
  "bunnyMat",
  scene,
  {
    vertexSource: bunnyVertexSource,
    fragmentSource: bunnyFragmentSource,
  },
  {
    attributes: ["position", "uv"],
    uniformBuffers: ["Scene", "Mesh"],
    storageBuffers: ["bunnies"],
    shaderLanguage: ShaderLanguage.WGSL,
  }
);

const bunnyComputeBuffer = new StorageBuffer(engine, bunniesData.byteLength);
bunnyComputeBuffer.update(bunniesData);
bunnyMat.setStorageBuffer("bunnies", bunnyComputeBuffer);

const bunnyTexture = new Texture("bunny.png", scene);
bunnyMat.setTexture("bunnyTexture", bunnyTexture);
bunnyMat.alpha = 0;

const sampler = new TextureSampler();
sampler.setParameters();
sampler.samplingMode = Constants.TEXTURE_NEAREST_SAMPLINGMODE;
bunnyMat.setTextureSampler("bunnySampler", sampler);

// Create bunny mesh
const bunnyMesh = CreatePlane("bunnyMesh", { width: 0.3, height: 0.5 }, scene);
bunnyMesh.material = bunnyMat;
bunnyMesh.forcedInstanceCount = numBunnies;

const params = new UniformBuffer(engine, undefined, true, "params");
params.addUniform("gravity", 1);
params.addUniform("xBound", 1);
params.addUniform("yBound", 1);
params.addUniform("rngSeed", 1);

params.updateFloat("gravity", gravity);
params.updateFloat("xBound", xBound);
params.updateFloat("yBound", yBound);
params.update();

const bunnyComputeShader = new ComputeShader(
  "bunniesCompute",
  engine,
  { computeSource: bunnyComputeSource },
  {
    bindingsMapping: {
      params: { group: 0, binding: 0 },
      bunnies: { group: 0, binding: 1 },
    },
  }
);
bunnyComputeShader.setUniformBuffer("params", params);
bunnyComputeShader.setStorageBuffer("bunnies", bunnyComputeBuffer);

engine.runRenderLoop(() => {
  const fps = engine.getFps();
  fpsText.innerHTML = `FPS: ${fps.toFixed(2)}`;
  params.updateUInt("rngSeed", Math.random() * 1000000);
  params.update();
  bunnyComputeShader.dispatch(Math.ceil(numBunnies / 256), 1, 1);
  scene.render();
  if (fps > 59) {
    numBunnies += 1000;
    bunnyMesh.forcedInstanceCount = numBunnies;
    bunnyText.innerHTML = `Bunnies: ${numBunnies}`;
  }
});
