using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hermit : MonoBehaviour 
{
	private readonly float[] VELOCITIES = new float[]{2.0f, 1.6f, 1.3f, 1.0f, 0.8f};

	private Rigidbody2D m_Rigidbody = null;
	private int m_Fatness = 0;

	public int Fatness
	{
		get
		{
			return m_Fatness;
		}
	}

	private float Velocity
	{
		get
		{
			return VELOCITIES[Mathf.Clamp(m_Fatness, 0, VELOCITIES.Length)];
		}
	}

	private void Awake()
	{
		m_Rigidbody = GetComponent<Rigidbody2D>();
	}

	private void Start() 
	{
		
	}

	private void Update() 
	{
		// TODO: Prevent hermits from going out of bounds
		PlayerInputManager.Instance.DebugButton();

		Vector2 joystick = PlayerInputManager.Instance.GetLeftJoystick(PlayerInputManager.ControllerInput.Controller1);
		Vector3 position = m_Rigidbody.position;
		position.x += Velocity * joystick.x * Time.deltaTime;
		position.y += Velocity * joystick.y * Time.deltaTime;

		m_Rigidbody.MovePosition(position);
	}
}
