extends Camera2D


# Declare member variables here. Examples:
# var a = 2
# var b = "text"

var _target_zoom: float = 1.0
var _stopMovement: bool = false
const MOVE_SPEED: float = 500.0
const MIN_ZOOM: float = 0.1
const MAX_ZOOM: float = 1.0
const ZOOM_INCREMENT: float = 0.1
const ZOOM_RATE: float = 8.0
# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.
	
func _process(delta):
	if _stopMovement:
		return
	else:
		if Input.is_key_pressed(KEY_W):
			position += Vector2.UP * MOVE_SPEED * delta
		if Input.is_key_pressed(KEY_S):
			position += Vector2.DOWN * MOVE_SPEED * delta
		if Input.is_key_pressed(KEY_A):
			position += Vector2.LEFT * MOVE_SPEED * delta
		if Input.is_key_pressed(KEY_D):
			position += Vector2.RIGHT * MOVE_SPEED * delta
	
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
		if event.button_mask == BUTTON_MASK_MIDDLE:
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


func FileDialog_BlackboardSelected(dir):
	pass # Replace with function body.
