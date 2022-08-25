// Spawn bunnies
if(fps_real > 59) {
	for(var i = 0; i < 100; i++) {
		var bunny = instance_create_layer(10,10,"Instances",bunnyObj)
		bunny.speedX = random_range(0,10)
		bunny.speedY = random_range(-5,5)
		array_push(bunnies, bunny)
	}
}


// Move bunnies
for (var i = 0; i < array_length(bunnies); i++) {
	var bunny = bunnies[i]
	
	bunny.x += bunny.speedX
	bunny.y += bunny.speedY
	bunny.speedY += grav
	
	
	if(bunny.x > maxX) {
		bunny.speedX *= -1
		bunny.x = maxX
	} else if (bunny.x < minX) {
		bunny.speedX *= -1
		bunny.x = minX
	}
	
	if(bunny.y > maxY) {
		bunny.speedY *= -0.85
		bunny.y = maxY
		if(random(1) > 0.5) {
			bunny.speedY -= random_range(0,6)
		}
	} else if (bunny.y < minY) {
		bunny.speedY = 0
		bunny.y = minY
	}
}

