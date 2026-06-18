using Godot;
using System;

public partial class Camera3d : Camera3D 
{
	// [Export] adjusts the Godot Inspector
	[Export] public float MaxZoom = 57f;
	[Export] public float MinZoom = 10f;
	
	public override void _Ready() 
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}
	
	// Capital 'I' for C# override methods, and explicitly typed parameter
	public override void _Input(InputEvent @event) 
	{
		if (Input.IsActionJustPressed("ui_cancel")) 
		{
			GetTree().Quit();
		}
		
		_CameraZoom();
	}
	
	private void _CameraZoom() 
	{
		float zoomChange = 0f;
		
		if (Input.IsActionJustPressed("mouse_wheel_up")) 
		{
			zoomChange -= 1f;
		}
		else if (Input.IsActionJustPressed("mouse_wheel_down")) 
		{
			zoomChange += 1f;
		}
		
		// Mathf.Clamp' to keep it bounded
		Size += zoomChange;
		Size = Mathf.Clamp(Size, MinZoom, MaxZoom);
	}
}
