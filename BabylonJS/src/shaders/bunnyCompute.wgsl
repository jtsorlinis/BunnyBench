struct Bunny {
    pos : vec3<f32>,
    vel : vec2<f32>,
};

struct Params {
    gravity : f32,
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
