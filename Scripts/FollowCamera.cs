using Godot;
using System;

public partial class FollowCamera : Camera3D
{
	// We use NodePath so you can select your Crow node directly in the Editor Inspector
	[Export]
	public NodePath TargetPath;

	// The fixed distance (offset) the camera will keep from the Crow
	[Export]
	public Vector3 Offset = new Vector3(0, 2, 10);

	// How smoothly the camera catches up to the player (lower = smoother/delayed, higher = snappier)
	[Export]
	public float SmoothSpeed = 2.0f;

	private Node3D _target;

	public override void _Ready()
	{
		if (TargetPath != null)
		{
			_target = GetNode<Node3D>(TargetPath);
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_target == null) return;

		// Calculate exactly where the camera wants to be
		Vector3 targetPosition = _target.GlobalPosition + Offset;

		// Smoothly interpolate (Lerp) from the current position to the target position
		// This prevents harsh camera snaps when Crow gets a massive speed boost
		GlobalPosition = GlobalPosition.Lerp(targetPosition, SmoothSpeed * (float)delta);
	}
}
