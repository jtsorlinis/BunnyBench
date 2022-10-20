shader_type particles;
render_mode keep_data;

uniform float xBound;
uniform float yBound;
uniform float gravity;
uniform int numBunnies;

float wang_hash(inout uint seed)
{
    seed = (seed ^ 61u) ^ (seed >> 16u);
    seed *= 9u;
    seed = seed ^ (seed >> 4u);
    seed *= 668265261u;
    seed = seed ^ (seed >> 15u);
    return float(seed) / 4294967296.0;
}

void vertex() {
  uint rng;
  if (RESTART) {
	rng = RANDOM_SEED + NUMBER;
    TRANSFORM[3].xy = vec2(-5000.0,-5000.0);
	CUSTOM.x = 10.0 * wang_hash(rng);
	CUSTOM.y = 10.0 * wang_hash(rng) - 5.0;
	CUSTOM.z = 0.0;
  } else {
	if(INDEX < numBunnies) {
		if(CUSTOM.z < 0.5) {
			TRANSFORM[3].xyz = vec3(-xBound,-yBound,0);
			CUSTOM.z = 1.0;
		}
		CUSTOM.y += gravity;
	    TRANSFORM[3].xy += CUSTOM.xy;
		
		if(TRANSFORM[3].x > xBound) {
			CUSTOM.x *= -1.0;
		} else if (TRANSFORM[3].x < -xBound) {
			CUSTOM.x *= -1.0;
		}
		
		if(TRANSFORM[3].y > yBound) {
			CUSTOM.y *= -0.8;
			TRANSFORM[3].y = yBound;
			if(wang_hash(rng) > 0.5) {
				CUSTOM.y -= wang_hash(rng) * 6.0;
			}
		} else if (TRANSFORM[3].y < -yBound) {
			CUSTOM.y = 0.0;
			TRANSFORM[3].y = -yBound;
		}
	}
  }
}