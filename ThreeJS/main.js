import * as THREE from "three";
import { Clock, Vector2 } from "three";

let clock = new Clock();

let width = 8;
let height = 6;
const xBound = width - 0.2;
const yBound = height - 0.3;

const gravity = 0.005;

let numBunnies;

const vertShader = `
uniform vec2 scale;
varying vec2 vUv; 

attribute vec3 bunnyPositions;

void main() {
  vUv = position.xy + 0.5 ;
  gl_Position = projectionMatrix * modelViewMatrix * vec4(position.xy * scale + bunnyPositions.xy, 0, 1.0 );
}
`;

const fragShader = `
uniform sampler2D bunnyTexture;
varying vec2 vUv;

void main() {
  vec4 texColor = texture2D(bunnyTexture, vUv);
  gl_FragColor = texColor;
}
`;

const camera = new THREE.OrthographicCamera(
  -width,
  width,
  height,
  -height,
  -10,
  100
);

const scene = new THREE.Scene();

const uniforms = {
  bunnyTexture: { value: new THREE.TextureLoader().load("bunny.png") },
  scale: { value: new Vector2(0.43, 0.625) },
};

const shaderMaterial = new THREE.ShaderMaterial({
  uniforms: uniforms,
  vertexShader: vertShader,
  fragmentShader: fragShader,

  blending: THREE.NormalBlending,
  depthTest: false,
  transparent: true,
  vertexColors: true,
});

// Setup quad
const geometry = new THREE.BufferGeometry();
const vertices = new Float32Array([
  -0.5, -0.5, 0, 0.5, -0.5, 0, 0.5, 0.5, 0, 0.5, 0.5, 0, -0.5, 0.5, 0, -0.5,
  -0.5, 0,
]);
geometry.setAttribute(
  "position",
  new THREE.Float32BufferAttribute(vertices, 3)
);

const bunnyPositions = [];
const bunnyVelocities = [];

for (let i = 0; i < 10; i++) {
  bunnyPositions.push(...[-xBound, yBound, 0]);
  bunnyVelocities.push(...[Math.random() * 0.1, Math.random() * 0.1 - 0.05]);
}
numBunnies = 10;

let buffer = new THREE.InstancedBufferAttribute(new Float32Array(16 * 3), 3);

geometry.setAttribute("bunnyPositions", buffer);

const bunnyMesh = new THREE.InstancedMesh(geometry, shaderMaterial, numBunnies);
scene.add(bunnyMesh);

const renderer = new THREE.WebGLRenderer({ antialias: true });
renderer.setSize(800, 600);
renderer.setAnimationLoop(animation);
document.body.appendChild(renderer.domElement);

function animation() {
  const fps = 1 / clock.getDelta();
  for (let i = 0; i < numBunnies; i++) {
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
        bunnyVelocities[i * 2 + 1] += Math.random() * 0.06;
      }
    } else if (bunnyPositions[i * 3 + 1] > yBound) {
      bunnyVelocities[i * 2 + 1] = 0;
      bunnyPositions[i * 3 + 1] = yBound;
    }
  }

  buffer.set(bunnyPositions);
  buffer.needsUpdate = true;
  renderer.render(scene, camera);

  if (fps > 59 && numBunnies) {
    for (let i = 0; i < 1000; i++) {
      bunnyPositions.push(...[-xBound, yBound, 0]);
      bunnyVelocities.push(
        ...[Math.random() * 0.1, Math.random() * 0.1 - 0.05]
      );
    }
    numBunnies += 1000;
    bunnyMesh.count = numBunnies;

    // resize buffer
    if (numBunnies > buffer.count) {
      let nextPowerOf2 = Math.pow(
        2,
        Math.ceil(Math.log(numBunnies) / Math.log(2))
      );
      buffer = new THREE.InstancedBufferAttribute(
        new Float32Array(nextPowerOf2 * 3),
        3
      );
      geometry.setAttribute("bunnyPositions", buffer);
    }
    console.log(numBunnies + " " + fps);
  }
}