extends Node2D

var gravity = 0.5
var xBound = DisplayServer.window_get_size().x/2-12
var yBound = DisplayServer.window_get_size().y/2-18
@onready var fpsLabel = get_node("FPS")
@onready var bunnyLabel = get_node("count")
@onready var bunnyParticleSystem = get_node("Bunnies")
var numBunnies = 10

# Called when the node enters the scene tree for the first time.
func _ready():
	bunnyParticleSystem.process_material.set_shader_parameter("xBound", xBound)
	bunnyParticleSystem.process_material.set_shader_parameter("yBound", yBound)
	bunnyParticleSystem.process_material.set_shader_parameter("gravity", gravity)
	bunnyParticleSystem.process_material.set_shader_parameter("rngSeed", randi())
	
# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	fpsLabel.text = 'FPS: %.2f' % Engine.get_frames_per_second()
	bunnyParticleSystem.process_material.set_shader_parameter("rngSeed", randi())
	
	# Add bunnies while over 60fps
	if 1/delta > 60 && numBunnies<bunnyParticleSystem.amount:
		numBunnies += 1000
		for i in range(0,1000):
			bunnyParticleSystem.emit_particle(Transform2D(),Vector2(0,0), Color("white"), Color("white"),0)
		bunnyLabel.text = 'Bunnies: %d' % numBunnies
