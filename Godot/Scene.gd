extends Node2D

var gravity = 0.5
var maxX = OS.get_window_size().x/2-12
var maxY = OS.get_window_size().y/2-18
var numBunnies = 10;
onready var fpsLabel = get_node("FPS")
onready var bunnyLabel = get_node("count")
onready var bunnyParticleSystem = get_node("Bunnies")

# Called when the node enters the scene tree for the first time.
func _ready():
	bunnyParticleSystem.process_material.set_shader_param("xBound", maxX)
	bunnyParticleSystem.process_material.set_shader_param("yBound", maxY)
	bunnyParticleSystem.process_material.set_shader_param("gravity", gravity)
	bunnyParticleSystem.process_material.set_shader_param("seed", randi())

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	fpsLabel.text = 'FPS: %.2f' % Engine.get_frames_per_second()
	
	# Add bunnies while over 60fps
	if 1/delta > 60 && numBunnies < bunnyParticleSystem.amount:
		numBunnies += 1000
		bunnyParticleSystem.process_material.set_shader_param("numBunnies", numBunnies)

	bunnyLabel.text = 'Bunnies: %d' % numBunnies
