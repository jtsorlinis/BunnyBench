extends Node2D

const Bunny = preload("res://Bunny.tscn")

onready var fpsLabel = get_node("../FPS")
onready var bunnyLabel = get_node("../count")

var startBunnyCount = 10
var count = startBunnyCount

# Called when the node enters the scene tree for the first time.
func _ready():
	for n in startBunnyCount:
		add_child(Bunny.instance())

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	fpsLabel.text = 'FPS: %.1f' % Engine.get_frames_per_second()
	# Add bunnies while over 59fps
	if 1/delta > 59:
		if count < 200000:
			for i in 100:
				add_child(Bunny.instance())

				count += 1
		bunnyLabel.text = 'Bunnies: %d' % count
