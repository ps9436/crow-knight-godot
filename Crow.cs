using Godot;
using System;

public partial class Crow : RigidBody3D
{
	// A placeholder for the sword's swing power
	[Export]
	public float SwingForce = 15.0f;

	public override void _PhysicsProcess(double delta)
	{
		// We use _PhysicsProcess for all physics-related calculations
		
		// Simulating a swing to the right
		if (Input.IsActionJustPressed("ui_right"))
		{
			SwingSword(Vector3.Right);
		}
		
		// Simulating a swing to the left
		if (Input.IsActionJustPressed("ui_left"))
		{
			SwingSword(Vector3.Left);
		}
	}

	private void SwingSword(Vector3 direction)
	{
		// ApplyCentralImpulse adds a sudden burst of momentum, 
		// perfectly simulating the yank of a heavy weapon.
		ApplyCentralImpulse(direction * SwingForce);
		
		GD.Print("Swung sword! Momentum applied.");
	}
}
