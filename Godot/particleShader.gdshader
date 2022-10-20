shader_type particles;

uniform float xBound;
uniform float yBound;
uniform float gravity;
uniform uint rngSeed;

float wang_hash(inout uint seed)
{
    seed = (seed ^ 61u) ^ (seed >> 16u);
    seed *= 9u;
    seed = seed ^ (seed >> 4u);
    seed *= 668265261u;
    seed = seed ^ (seed >> 15u);
    return float(seed) / 4294967296.0;
}
void start() {
	uint rng = RANDOM_SEED + NUMBER;	
    TRANSFORM[3].xyz = vec3(-xBound,-yBound,0);
	CUSTOM.x = 10.0 * wang_hash(rng);
	CUSTOM.y = 10.0 * wang_hash(rng) - 5.0;
}

void process() {
	uint rng = rngSeed + INDEX;
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
		float rot = wang_hash(rng) * 0.2 - 0.1;
		TRANSFORM[0].xy = vec2(cos(rot),-sin(rot));
		TRANSFORM[1].xy = vec2(sin(rot), cos(rot));
		if(wang_hash(rng) > 0.5) {
			CUSTOM.y -= wang_hash(rng) * 12.0;
		}
	} else if (TRANSFORM[3].y < -yBound) {
		CUSTOM.y = 0.0;
		TRANSFORM[3].y = -yBound;
	}
}