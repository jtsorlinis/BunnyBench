// Spawn bunnies
if(fps_real > 59) {
	for(var i = 0; i < 100; i++) {
		var bunny = {
		x: 10,
		y: 10,
		vx: random_range(0,10),
		vy: random_range(-5,5)
	}
	array_push(bunnies, bunny)
	}
}

// Move bunnies
for (var i = 0; i < array_length(bunnies); i++) {
	var bunny = bunnies[i]
	
	bunny.x += bunny.vx
	bunny.y += bunny.vy
	bunny.vy += grav
	
	
	if(bunny.x > maxX) {
		bunny.vx *= -1
		bunny.x = maxX
	} else if (bunny.x < minX) {
		bunny.vx *= -1
		bunny.x = minX
	}
	
	if(bunny.y > maxY) {
		bunny.vy *= -0.85
		bunny.y = maxY
		bunny.image_angle = random_range(-6,6)
		if(random(1) > 0.5) {
			bunny.vy -= random_range(0,6)
		}
	} else if (bunny.y < minY) {
		bunny.vy = 0
		bunny.y = minY
	}
}




