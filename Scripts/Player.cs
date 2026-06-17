using Godot;
using System;

public partial class Player : RigidBody3D
{
	[Export] public float Speed = 30.0f;
	[Export] public float JumpForce = 25.0f;

	private bool _isGrounded = false;

	public override void _Ready()
	{
		// Tell the physics engine to keep track of what this body is touching.
		// This is required so our ground check functions properly.
		ContactMonitor = true;
		MaxContactsReported = 4;
		GravityScale = 7.0f;
	}

	public override void _PhysicsProcess(double delta)
	{
		// 1. Handle Horizontal WASD Movement
		Vector3 movement = Vector3.Zero;

		if (Input.IsKeyPressed(Key.W)) movement.Z -= 1;
		if (Input.IsKeyPressed(Key.S)) movement.Z += 1;
		if (Input.IsKeyPressed(Key.A)) movement.X -= 1;
		if (Input.IsKeyPressed(Key.D)) movement.X += 1;

		movement = movement.Normalized() * Speed;

		// Crucial: We keep LinearVelocity.Y as-is. 
		// This ensures Godot's engine gravity continues to pull the player down naturally.
		LinearVelocity = new Vector3(movement.X, LinearVelocity.Y, movement.Z);

		// 2. Check if the player is resting on a surface
		_isGrounded = CheckIfGrounded();

		// 3. Handle Jumping
		// "ui_accept" is built into Godot by default and is automatically mapped to the Spacebar.
		if (Input.IsActionJustPressed("ui_accept") && _isGrounded)
		{
			// ApplyCentralImpulse adds an instantaneous vertical velocity kick
			ApplyCentralImpulse(Vector3.Up * JumpForce);
		}
	}

	private bool CheckIfGrounded()
	{
		// Grab a list of every physical body Charles is currently colliding with
		var collidingBodies = GetCollidingBodies();
		
		foreach (var body in collidingBodies)
		{
			// If we are touching the Ground (StaticBody3D), we are safe to jump
			if (body is StaticBody3D)
			{
				return true;
			}
		}
		return false;
	}
}
