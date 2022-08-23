extends Sprite

var speedX = rand_range(0,10)
var speedY = rand_range(-5, 5)
var gravity = 0.5
var minX = 0
var maxX = OS.get_window_size().x
var minY = 0
var maxY = OS.get_window_size().y

func _ready():
	set_offset(Vector2(.5,1))
	set_position(Vector2(10,10))
	z_index = -1;

func _process(_delta):
	# Move
	position.x += speedX
	position.y += speedY
	speedY += gravity
		
	if position.x > maxX:
		speedX *= -1
		position.x = maxX
	elif position.x < minX:
		speedX *= -1
		position.x = minX
		
	if position.y > maxY:
		speedY *= -0.85;
		position.y = maxY;
		rotation = rand_range(-0.1, 0.1)	
		if(rand_range(0,1) > 0.5):
			speedY -= rand_range(0, 6)
	elif position.y < minY:
		speedY = 0
		position.y = minY
			
