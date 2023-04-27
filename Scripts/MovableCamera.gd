extends Camera2D


# Declare member variables here. Examples:
# var a = 2
# var b = "text"

var _target_zoom: float = 1.0
const MIN_ZOOM: float = 0.1
const MAX_ZOOM: float = 1.0
const ZOOM_INCREMENT: float = 0.1
const ZOOM_RATE: float = 8.0
# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.
	
func _physics_process(delta):
	zoom = lerp(
		zoom,
		_target_zoom * Vector2.ONE,
		ZOOM_RATE * delta
	)	
	
	set_physics_process(
		not is_equal_approx(zoom.x, _target_zoom)
	)

func _unhandled_input(event: InputEvent) -> void:
	if event is InputEventMouseMotion:
		if event.button_mask == BUTTON_MASK_LEFT || event.button_mask == BUTTON_MASK_MIDDLE:
			position -= event.relative * zoom
			
	if event is InputEventMouseButton:
		if event.is_pressed():
			if event.button_index == BUTTON_WHEEL_UP:
				zoom_in()
			if event.button_index == BUTTON_WHEEL_DOWN:
				zoom_out()			


func zoom_in() -> void:
	_target_zoom = max(_target_zoom - ZOOM_INCREMENT, MIN_ZOOM);
	set_physics_process(true)
	
func zoom_out() -> void:
	_target_zoom = max(_target_zoom + ZOOM_INCREMENT, MAX_ZOOM);
	set_physics_process(true)



func _on_ZoomIn_pressed():
	zoom_in()


func _on_ZoomOut_pressed():
	zoom_out() # Replace with function body.
