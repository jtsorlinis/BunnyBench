import { VertexBuffer } from "@babylonjs/core/Buffers/buffer";
import "./style.css";
import { FreeCamera } from "@babylonjs/core/Cameras/freeCamera";
import { ComputeShader, UniformBuffer, WebGPUEngine } from "@babylonjs/core";
import { Texture } from "@babylonjs/core/Materials/Textures/texture";
import { ShaderMaterial } from "@babylonjs/core/Materials/shaderMaterial";
import { Vector3 } from "@babylonjs/core/Maths/math.vector";
import { CreatePlane } from "@babylonjs/core/Meshes/Builders/planeBuilder";
import { Scene } from "@babylonjs/core/scene";
import { StorageBuffer } from "@babylonjs/core/Buffers/storageBuffer";

let numBunnies = 10;
const bufferSize = 2000000;
const maxSpeed = 0.13;
const gravity = 0.007;

const canvas = document.getElementById("renderCanvas") as HTMLCanvasElement;
const bunnyText = document.getElementById("bunnyText") as HTMLElement;
const fpsText = document.getElementById("fpsText") as HTMLElement;
const engine = new WebGPUEngine(canvas);
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

const xBound = orthoSize * aspectRatio - 0.05;
const yBound = orthoSize - 0.07;

const bunnyVertexShader = `
  attribute vec2 position;
  attribute vec2 uv;
  attribute vec3 bunnyPos;

  varying vec2 vUV;
  uniform mat4 worldViewProjection;
  
  void main() {
      float angle = bunnyPos.z;
      vec2 pos = vec2(
          position.x * cos(angle) - position.y * sin(angle),
          position.x * sin(angle) + position.y * cos(angle)
      );
      gl_Position = worldViewProjection * vec4(pos + bunnyPos.xy, 0.0,1.0);;
      vUV = uv;
  }
`;

const bunnyFragmentShader = `
    varying vec2 vUV;
    uniform sampler2D bunnyTexture;

    void main() {
        gl_FragColor = texture2D(bunnyTexture, vUV);
    }
`;

const bunnyMat = new ShaderMaterial(
  "bunnyMat",
  scene,
  {
    vertexSource: bunnyVertexShader,
    fragmentSource: bunnyFragmentShader,
  },
  {
    attributes: ["position", "uv", "bunnyPos"],
    uniforms: ["worldViewProjection"],
  }
);

const bufferStride = 8;
const initialParticleData = new Float32Array(bufferSize * bufferStride);
for (let i = 0; i < bufferSize; ++i) {
  initialParticleData[bufferStride * i + 0] = -xBound;
  initialParticleData[bufferStride * i + 1] = yBound;
  initialParticleData[bufferStride * i + 4] = Math.random() * maxSpeed; // velocity x
  initialParticleData[bufferStride * i + 5] = (Math.random() - 0.5) * maxSpeed; // velocity y
}

const bunnyComputeBuffer = new StorageBuffer(
  engine,
  initialParticleData.byteLength,
  8 | 2
);

bunnyComputeBuffer.update(initialParticleData);

const bunnyPosBuffer = new VertexBuffer(
  engine,
  bunnyComputeBuffer.getBuffer(),
  "bunnyPos",
  false,
  false,
  bufferStride,
  true,
  0,
  3
);

const bunnyTexture = new Texture("bunny.png", scene);
bunnyTexture.hasAlpha = true;

bunnyMat.setTexture("bunnyTexture", bunnyTexture);
bunnyMat.alpha = 0;

const bunnyMesh = CreatePlane("bunnyMesh", { width: 0.3, height: 0.5 }, scene);
bunnyMesh.material = bunnyMat;
bunnyMesh.forcedInstanceCount = numBunnies;
bunnyMesh.setVerticesBuffer(bunnyPosBuffer, false);

const bunnyComputeShaderSource = `
struct Bunny {
    pos : vec3<f32>,
    vel : vec2<f32>,
};

struct Params {
    gravity: f32,
    xBound : f32,
    yBound : f32,
    rngSeed : u32,
};

fn wang_hash(seed : ptr<function, u32>) -> f32 {
  *seed = (*seed ^ 61) ^ (*seed >> 16);
  *seed *= 9;
  *seed = *seed ^ (*seed >> 4);
  *seed *= 0x27d4eb2d;
  *seed = *seed ^ (*seed >> 15);
  return f32(*seed) / 4294967296.0;
}

@binding(0) @group(0) var<uniform> params : Params;
@binding(1) @group(0) var<storage, read_write> bunnies : array<Bunny>;

@compute @workgroup_size(256)
fn main(@builtin(global_invocation_id) GlobalInvocationID : vec3<u32>) {
  var index : u32 = GlobalInvocationID.x;
  var seed : u32 = params.rngSeed + index;

  var bunny = bunnies[index];

  bunny.pos.x += bunny.vel.x;
  bunny.pos.y += bunny.vel.y;
  bunny.vel.y -= params.gravity;

  if (bunny.pos.x > params.xBound) {
    bunny.pos.x = params.xBound;
    bunny.vel.x *= -1.0;
  } else if (bunny.pos.x < -params.xBound) {
    bunny.pos.x = -params.xBound;
    bunny.vel.x *= -1.0;
  }

  if (bunny.pos.y < -params.yBound) {
    bunny.pos.y = -params.yBound;
    bunny.vel.y *= -0.85;
    bunny.pos.z = wang_hash(&seed) * 0.2 - 0.1;
    if(wang_hash(&seed) > 0.5) {
      bunny.vel.y += wang_hash(&seed) * 0.1;
    }
  } else if (bunny.pos.y > params.yBound) {
    bunny.pos.y = params.yBound;
    bunny.vel.y = 0;
  }

  bunnies[index] = bunny;
}
`;

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
  { computeSource: bunnyComputeShaderSource },
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
