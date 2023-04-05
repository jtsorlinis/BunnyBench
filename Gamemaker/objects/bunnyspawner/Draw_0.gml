// Move bunnies
for (var i = 0; i < array_length(bunnies); i++) {
	draw_sprite(bunnySprite, 0, bunnies[i].x, bunnies[i].y)
}

// UI
draw_set_color(c_black)
draw_rectangle(0,0, 140, 50, false)

draw_set_color(c_white)
draw_text(5, 5, "FPS: " + string(fps_real));
draw_text(5, 25, "Bunnies: " + string(array_length(bunnies)))