bunnies = array_create(0)
grav = 0.5
minX = 0
maxX = room_width - 26
minY = 0
maxY = room_height - 37


for(var i = 0; i < 10; i++) {
	
	var bunny = {
		x: 10,
		y: 10,
		vx: random_range(0,10),
		vy: random_range(-5,5)
	}
	array_push(bunnies, bunny)
}