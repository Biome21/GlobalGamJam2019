using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hermit : MonoBehaviour
{
	private readonly float[] VELOCITIES = new float[]{8.0f, 7.5f, 7.0f, 6.5f, 6.0f};
	private readonly float[] WALK_SPEEDS = new float[]{5.0f, 4.0f, 3.0f, 2.0f, 1.5f};
	private const int MAXIMUM_FATNESS = 5;
	private const int FOOD_PER_FATNESS = 3;
	private const int MAX_FOOD = MAXIMUM_FATNESS * FOOD_PER_FATNESS;
	private const float MIN_SCALE = 0.3f;
	private const float MAX_SCALE = 1.0f;
	private const string WALK_ANIM = "Walk";
	private const string IDLE_ANIM = "Idle";

	[SerializeField] private Transform m_ShellAnchor = null;
	[SerializeField] private Transform m_Body = null;
	[SerializeField] private SpriteRenderer m_GrayscaleSprite = null;
	[SerializeField] private Animation m_Animation = null;
	private Rigidbody2D m_Rigidbody = null;
	private int m_Fatness = 0;
	private int m_Food = 0;
	private PlayerInputManager.ControllerInput m_Controller;
	private bool m_IsReady = false;

	public int Fatness
	{
		get
		{
			return m_Fatness;
		}
		private set
		{
			m_Fatness = Mathf.Clamp(value, 0, MAXIMUM_FATNESS);
		}
	}

	public int Food
	{
		get
		{
			return m_Food;
		}
		private set
		{
			m_Food = Mathf.Clamp(value, 0, MAX_FOOD);
		}
	}

	public int TotalFood
	{
		get
		{
			return Fatness * FOOD_PER_FATNESS + Food;
		}
	}

	private float Velocity
	{
		get
		{
			return VELOCITIES[Mathf.Clamp(m_Fatness, 0, MAXIMUM_FATNESS - 1)];
		}
	}

	private float WalkSpeed
	{
		get
		{
			return WALK_SPEEDS[Mathf.Clamp(m_Fatness, 0, MAXIMUM_FATNESS - 1)];
		}
	}

	public bool IsReady
	{
		get
		{
			return m_IsReady;
		}
	}

	private void Awake()
	{
		m_Rigidbody = GetComponent<Rigidbody2D>();
		UpdateFatness();
	}

	private void Start()
	{
		
	}

	private void Update() 
	{
		//PlayerInputManager.Instance.DebugButton();

		Vector2 joystick = PlayerInputManager.Instance.GetLeftJoystick(m_Controller);
		Vector3 position = m_Rigidbody.position;
		position.x += Velocity * joystick.x * Time.deltaTime;
		position.y += Velocity * joystick.y * Time.deltaTime;

		if (joystick.sqrMagnitude > 0.0f)
		{
			m_Animation.CrossFade(WALK_ANIM);
			m_Animation[WALK_ANIM].speed = WalkSpeed;
		}
		else
		{
			m_Animation.CrossFade(IDLE_ANIM);
		}

		// TODO: Prevent hermits from going out of bounds
		m_Rigidbody.MovePosition(position);
	}

	private void OnTriggerEnter2D(Collider2D collider)
	{
		Plancton plancton = collider.GetComponentInParent<Plancton>();
		if (plancton != null)
		{
			OnEatFood();
			plancton.Die();
		}
	}

	public void OnControllerReady(PlayerInputManager.ControllerInput controller)
	{	
		m_Controller = controller;

		m_IsReady = !m_IsReady;
		if (m_IsReady)
		{
			OnReady();
		}
		else
		{
			OnNotReady();
		}
	}

	private void OnReady()
	{
	}

	private void OnNotReady()
	{
	}

	private void OnEatFood()
	{
		if (++Food >= FOOD_PER_FATNESS)
		{
			Food = 0;
			++Fatness;
		}

		UpdateFatness();
	}

	private void UpdateFatness()
	{
		m_Body.localScale = Vector3.one * Mathf.Lerp(MIN_SCALE, MAX_SCALE, (float)TotalFood / MAX_FOOD);
		Color color = m_GrayscaleSprite.color;
		color.r = ((float)m_Fatness / MAXIMUM_FATNESS) + (0.5f / MAXIMUM_FATNESS);
		m_GrayscaleSprite.color = color;
	}
}
