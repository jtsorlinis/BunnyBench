extends Node2D

const bunnySprite = preload("res://bunny2.png")

var gravity = 0.5
var minX = 0
var maxX = OS.get_window_size().x
var minY = 0
var maxY = OS.get_window_size().y
onready var fpsLabel = get_node("FPS")
onready var bunnyLabel = get_node("count")

var bunnies = []
var bunnySpeeds = []


# Called when the node enters the scene tree for the first time.
func _ready():
	for n in 10:
		var sprite = Sprite.new()
		sprite.texture = bunnySprite
		sprite.set_offset(Vector2(.5,1))
		sprite.set_position(Vector2(10,10))
		var spriteSpeed = Vector2(rand_range(0,10), rand_range(-5,5))
		sprite.z_index = -1
		add_child(sprite)
		bunnies.append(sprite)
		bunnySpeeds.append(spriteSpeed)


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	fpsLabel.text = 'FPS: %.2f' % Engine.get_frames_per_second()
	# Add bunnies while over 60fps
	if 1/delta > 60:
		for i in 100:
			var sprite = Sprite.new()
			sprite.texture = bunnySprite
			sprite.set_offset(Vector2(.5,1))
			sprite.set_position(Vector2(10,10))
			var spriteSpeed = Vector2(rand_range(0,10), rand_range(-5,5))
			sprite.z_index = -1
			add_child(sprite)
			bunnies.append(sprite)
			bunnySpeeds.append(spriteSpeed)

		bunnyLabel.text = 'Bunnies: %d' % bunnies.size()

	
	# Move bunnies
	for i in len(bunnies):
		var bunnypos = bunnies[i].position
		var bunnyvel = bunnySpeeds[i]
		bunnypos += bunnyvel
		bunnyvel.y += gravity
		
		if bunnypos.x > maxX:
			bunnyvel.x *= -1
			bunnypos.x = maxX
		elif bunnypos.x < minX:
			bunnyvel.x *= -1
			bunnypos.x = minX
			
		if bunnypos.y > maxY:
			bunnyvel.y *= -0.85;
			bunnypos.y = maxY;
			bunnies[i].rotation = rand_range(-0.1, 0.1)	
			if(rand_range(0,1) > 0.5):
				bunnyvel.y -= rand_range(0, 6)
		elif bunnypos.y < minY:
			bunnyvel.y = 0
			bunnypos.y = minY
		
		bunnies[i].position = bunnypos
		bunnySpeeds[i] = bunnyvel
			
			
			
