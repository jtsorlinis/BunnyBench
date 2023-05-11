#include<sceneUboDeclaration>

struct Bunny {
    pos : vec3<f32>,
    vel : vec2<f32>,
};

var<storage> bunnies : array<Bunny>;

attribute position : vec3<f32>;
attribute uv: vec2<f32>;

varying vUV : vec2<f32>;

@vertex
fn main(input : VertexInputs) -> FragmentInputs {
    var bunnyPos = bunnies[input.instanceIndex].pos + vertexInputs.position;
    vertexOutputs.position = scene.viewProjection * vec4<f32>(bunnyPos, 1.0);
    vertexOutputs.vUV = vertexInputs.uv;
}