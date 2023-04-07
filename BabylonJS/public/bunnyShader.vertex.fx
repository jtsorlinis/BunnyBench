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