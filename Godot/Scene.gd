extends Node2D

const Bunny = preload("res://Bunny.gd")
const bunnySprite = preload("res://bunny2.png")

var gravity = 0.5
var minX = 0
var maxX = OS.get_window_size().x
var minY = 0
var maxY = OS.get_window_size().y
onready var fpsLabel = get_node("FPS")
onready var bunnyLabel = get_node("count")

var bunnies = []


# Called when the node enters the scene tree for the first time.
func _ready():
	for n in 10:
		var sprite = Bunny.new()
		sprite.texture = bunnySprite
		sprite.set_offset(Vector2(.5,1))
		sprite.set_position(Vector2(10,10))
		sprite.speedX = rand_range(0,10)
		sprite.speedY = rand_range(-5, 5)
		sprite.z_index = -1
		add_child(sprite)
		bunnies.append(sprite)


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	fpsLabel.text = 'FPS: %.2f' % Engine.get_frames_per_second()
	# Add bunnies while over 60fps
	if 1/delta > 60:
		for i in 100:
			var sprite = Bunny.new()
			sprite.texture = bunnySprite
			sprite.set_offset(Vector2(.5,1))
			sprite.set_position(Vector2(10,10))
			sprite.speedX = rand_range(0,10)
			sprite.speedY = rand_range(-5, 5)
			sprite.z_index = -1
			add_child(sprite)
			bunnies.append(sprite)

		bunnyLabel.text = 'Bunnies: %d' % bunnies.size()

	
	# Move bunnies
	for bunny in bunnies:
		bunny.position.x += bunny.speedX
		bunny.position.y += bunny.speedY
		bunny.speedY += gravity
		
		if bunny.position.x > maxX:
			bunny.speedX *= -1
			bunny.position.x = maxX
		elif bunny.position.x < minX:
			bunny.speedX *= -1
			bunny.position.x = minX
			
		if bunny.position.y > maxY:
			bunny.speedY *= -0.85;
			bunny.position.y = maxY;
			bunny.rotation = rand_range(-0.1, 0.1)	
			if(rand_range(0,1) > 0.5):
				bunny.speedY -= rand_range(0, 6)
		elif bunny.position.y < minY:
			bunny.speedY = 0
			bunny.position.y = minY
			
			
			
