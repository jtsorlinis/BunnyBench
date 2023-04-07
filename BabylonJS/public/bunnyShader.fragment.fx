varying vec2 vUV;
uniform sampler2D bunnyTexture;

void main() {
    gl_FragColor = texture2D(bunnyTexture, vUV);
}