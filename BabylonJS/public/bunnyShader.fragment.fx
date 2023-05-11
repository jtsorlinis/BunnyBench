var bunnyTexture : texture_2d<f32>;
var bunnySampler : sampler;

varying vUV : vec2<f32>;

@fragment
fn main(input : FragmentInputs) -> FragmentOutputs {
    fragmentOutputs.color = textureSample(bunnyTexture, bunnySampler, fragmentInputs.vUV);
}