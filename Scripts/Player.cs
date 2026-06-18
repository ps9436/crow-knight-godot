using Godot;
using System;

public partial class Player : RigidBody3D
{
	[Export] public float PullForce = 75.0f; 
	
	// How fast the sword snaps to its new position. Higher = more violent/instant.
	[Export] public float SwingSpeed = 25.0f; 

	[Export] public Node3D SwordPivot;

	private Vector3 _previousLaser = Vector3.Forward;
	
	// These variables track our smooth, code-driven swing
	private float _targetSwordAngle = 0.0f;
	private float _currentSwordAngle = 0.0f;

	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Visible;
	}

	public override void _PhysicsProcess(double delta)
	{
		// 1. Constantly animate the sword towards its target angle
		if (SwordPivot != null)
		{
			// LerpAngle mathematically calculates the shortest rotation path and smoothly moves there.
			// Once _current equals _target, it stops completely. No extra spinning!
			_currentSwordAngle = Mathf.LerpAngle(_currentSwordAngle, _targetSwordAngle, (float)delta * SwingSpeed);
			SwordPivot.Rotation = new Vector3(0, _currentSwordAngle, 0);
		}

		if (Input.IsActionJustPressed("attack"))
		{
			ExecuteClickDash();
		}
	}

	private void ExecuteClickDash()
	{
		Vector3 newLaser = GetMouseDirection();
		if (newLaser.LengthSquared() < 0.001f) return;

		float dotProduct = _previousLaser.Dot(newLaser);
		float sqrt2Over2 = Mathf.Sqrt(2.0f) / 2.0f; 
		bool updateLaser = true;

		// --- TRIGGER THE SWING ---
		// We set the new target angle. _PhysicsProcess will violently swing the pivot to match this.
		_targetSwordAngle = Mathf.Atan2(-newLaser.X, -newLaser.Z);

		// --- EVALUATING THE ZONES ---
		if (dotProduct >= sqrt2Over2)
		{
			// ZONE 1: BRAKING (0 to 45 degrees)
			LinearVelocity = new Vector3(LinearVelocity.X * 0.3f, LinearVelocity.Y, LinearVelocity.Z * 0.3f);
			
			if (LinearVelocity.LengthSquared() < 0.1f) 
			{
				updateLaser = false;
				GD.Print($"[STATIONARY] Didn't move. Laser locked.");
			}
		}
		else if (dotProduct >= 0.0f && dotProduct < sqrt2Over2)
		{
			// ZONE 2: COASTING (45 to 90 degrees)
			// The sword will still swing to the new angle, but we apply no player forces.
			GD.Print($"[COASTING/SWINGING] Momentum Maintained.");
		}
		else if (dotProduct < 0.0f)
		{
			// ZONE 3: ACCELERATING (90 to 180 degrees)
			float speedUpFactor = Mathf.Abs(dotProduct); 

			// Instantly kill the Crow's previous momentum so it snaps exactly where we want
			LinearVelocity = Vector3.Zero;
			AngularVelocity = Vector3.Zero;

			// Apply the massive force back directly to the Player
			Vector3 impulseVector = newLaser * (PullForce * speedUpFactor);
			ApplyCentralImpulse(impulseVector);
			
			GD.Print($"[ACCELERATING] Momentum Wiped. Snapping back! Speed Factor: {speedUpFactor:F2}");
		}

		if (updateLaser)
		{
			_previousLaser = newLaser;
		}
	}

	private Vector3 GetMouseDirection()
	{
		Camera3D camera = GetViewport().GetCamera3D();
		if (camera != null)
		{
			Vector2 mousePos = GetViewport().GetMousePosition();
			Vector3 rayOrigin = camera.ProjectRayOrigin(mousePos);
			Vector3 rayNormal = camera.ProjectRayNormal(mousePos);
			
			Plane floorPlane = new Plane(Vector3.Up, GlobalPosition.Y);
			Vector3? intersection = floorPlane.IntersectsRay(rayOrigin, rayNormal);
			
			if (intersection.HasValue)
			{
				Vector3 directionToMouse = intersection.Value - GlobalPosition;
				directionToMouse.Y = 0; 

				if (directionToMouse.LengthSquared() > 0.001f)
				{
					return directionToMouse.Normalized();
				}
			}
		}
		return Vector3.Zero;
	}
}
