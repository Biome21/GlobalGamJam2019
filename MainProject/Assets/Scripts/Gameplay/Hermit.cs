using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Hermit : MonoBehaviour, IComparable
{
	private readonly float[] VELOCITIES = new float[]{8.0f, 7.5f, 7.0f, 6.5f, 6.0f};
	private readonly float[] WALK_SPEEDS = new float[]{8.0f, 7.0f, 6.0f, 5.0f, 4.0f};
	public const int MAXIMUM_FATNESS = 5;
	private const int FOOD_PER_FATNESS = 5;
	private const int MAX_FOOD = (MAXIMUM_FATNESS - 1) * FOOD_PER_FATNESS;
	private const float MIN_SCALE = 0.4f;
	private const float MAX_SCALE = 1.0f;
	private const string WALK_ANIM = "Walk";
	private const string IDLE_ANIM = "Idle";
	private const string POP_ANIM = "Pop";
	private const string EXPLODE_ANIM = "Explode";
	private const float SHELL_PICKUP_RADIUS_EXT = 1.0f;

	public Action<Shell> m_OnShellExplode;
	public Action<Hermit> m_OnHermitDied;

	[SerializeField] private Transform m_ShellAnchor = null;
	[SerializeField] private Transform m_Body = null;
	[SerializeField] private SpriteRenderer m_GrayscaleSprite = null;
	[SerializeField] private SpriteRenderer m_OverlaySprite = null;
	[SerializeField] private Sprite m_DeadSprite = null;
	[SerializeField] private Animation m_Animation = null;
	[SerializeField] private CircleCollider2D m_Collider = null;
	[SerializeField] private AudioClip[] m_EatClips = null;
	[SerializeField] private AudioClip m_BreakClip = null;
	[SerializeField] private AudioClip m_PickupClip = null;
	[SerializeField] private ThoughtBubble m_ThoughtBubble = null;
	private BeachOfDespair m_Beach = null;
	private Rigidbody2D m_Rigidbody = null;
	private int m_Fatness = 0;
	private int m_Food = 0;
	private PlayerInputManager.ControllerInput m_Controller;
	private bool m_IsReady = false;
	private Shell m_TargetedShell = null;
	private Shell m_PickedUpShell = null;
	private bool m_IsDead = false;

	public int Fatness
	{
		get
		{
			return m_Fatness;
		}
		private set
		{
			m_Fatness = Mathf.Clamp(value, 0, MAXIMUM_FATNESS - 1);
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

	public bool HasShellEquipped
	{
		get
		{
			return m_PickedUpShell != null;
		}
	}

	public PlayerInputManager.ControllerInput Controller
	{
		get
		{
			return m_Controller;
		}
	}

	private void Awake()
	{
		m_Rigidbody = GetComponent<Rigidbody2D>();
		UpdateFatness();

		// Fuck it
		m_Beach = FindObjectOfType<BeachOfDespair>();

		m_ThoughtBubble.gameObject.SetActive (false);
	}

	private void Start()
	{
		
	}

	private void Update()
	{
		if ((m_Beach != null && !m_Beach.IsGameStarted) || m_IsDead || m_Beach.IsGameOver)
		{
			return;
		}

		Vector2 joystick = PlayerInputManager.Instance.GetLeftJoystick(m_Controller);
		Vector3 position = m_Rigidbody.position;
		position.x += Velocity * joystick.x * Time.deltaTime;
		position.y += Velocity * joystick.y * Time.deltaTime;

		float magnitude = joystick.magnitude;
		if (magnitude > 0.0f)
		{
			m_Animation.CrossFade(WALK_ANIM);
			m_Animation[WALK_ANIM].speed = WalkSpeed * magnitude;
		}
		else
		{
			m_Animation.CrossFade(IDLE_ANIM);
		}

		// TODO: Prevent hermits from going out of bounds
		m_Rigidbody.MovePosition(position);

		HandleShellPickup();

		m_ThoughtBubble.gameObject.SetActive (true);
		m_ThoughtBubble.SetIcon (Fatness);
	}

	private void OnTriggerEnter2D(Collider2D collider)
	{
		Plancton plancton = collider.GetComponentInParent<Plancton>();
		if (plancton != null && m_PickedUpShell != null)
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

			ExplodeShell ();

			m_ThoughtBubble.SetIcon (Fatness);
		}
		else
		{
			m_Animation[POP_ANIM].layer = 1;
			m_Animation.Play(POP_ANIM);
		}

		m_Beach.SFXSource.PlayOneShot(m_EatClips[UnityEngine.Random.Range(0, m_EatClips.Length)]);
		UpdateFatness();
	}

	public void ExplodeShell()
	{
		if (m_OnShellExplode != null)
		{
			m_OnShellExplode (m_PickedUpShell);
		}		
		// Explode the shell!!!
		m_PickedUpShell.Explode();
		m_PickedUpShell = null;
		m_Animation[EXPLODE_ANIM].layer = 1;
		m_Animation.Play(EXPLODE_ANIM);
		m_Beach.SFXSource.PlayOneShot(m_BreakClip);
	}

	public void Die()
	{
		if (!m_IsDead)
		{
			if (m_OnHermitDied != null) {
				m_OnHermitDied (this);
			}

			Food = 0;
			Fatness = 0;
			UpdateFatness ();

			m_GrayscaleSprite.sprite = m_DeadSprite;
			m_GrayscaleSprite.material.shader = Shader.Find ("Sprites/Default");
			m_GrayscaleSprite.color = Color.white;
			m_OverlaySprite.gameObject.SetActive (false);

			m_Animation.CrossFade (IDLE_ANIM);

			m_Collider.enabled = false;
			Destroy (GetComponent<Rigidbody2D> ());

			m_ThoughtBubble.gameObject.SetActive (false);

			m_IsDead = true;
			m_IsReady = false;
		}
	}

	public void Reset()
	{
	}

	private void UpdateFatness()
	{
		m_Body.localScale = Vector3.one * Mathf.Lerp(MIN_SCALE, MAX_SCALE, (float)TotalFood / MAX_FOOD);
		Color color = m_GrayscaleSprite.color;
		color.r = ((float)m_Fatness / MAXIMUM_FATNESS) + (0.5f / MAXIMUM_FATNESS);
		m_GrayscaleSprite.color = color;
	}

	private void OnShellPickedUp()
	{
		m_Beach.SFXSource.PlayOneShot(m_PickupClip);
		m_PickedUpShell = m_TargetedShell;
		m_PickedUpShell.transform.SetParent(transform);
		m_PickedUpShell.transform.position = m_ShellAnchor.position + m_PickedUpShell.m_Shells[m_PickedUpShell.Fatness].Offset;
		m_PickedUpShell.transform.localEulerAngles = Vector3.zero;
		m_PickedUpShell.OnPickedUp();
		m_TargetedShell = null;

		if (Fatness == MAXIMUM_FATNESS - 1)
		{
			m_Beach.OnGameOver();
		}
	}

	private void HandleShellPickup()
	{
		if (m_PickedUpShell != null)
		{
			return;
		}

		float radius = m_Collider.bounds.extents.x + SHELL_PICKUP_RADIUS_EXT;
		Collider2D[] shells = Physics2D.OverlapCircleAll(m_Rigidbody.position, radius, LayerMask.GetMask("Shell"));
		Shell closestShell = null;
		float closestDistance = float.MaxValue;
		for (int i = 0; i < shells.Length; ++i)
		{
			
			// TODO: Use ClosestPoint??
			float distance = Vector3.Distance(m_Rigidbody.position, shells[i].bounds.center);
			Shell shell = shells[i].GetComponentInParent<Shell>();

			// Make sure it's the proper size for us
			if (distance < closestDistance && shell.Fatness == Fatness)
			{
				closestDistance = distance;
				closestShell = shell;
			}
		}

		// A new shell was targeted
		if (m_TargetedShell != closestShell)
		{
			// Not the one we used to have
			if (m_TargetedShell != null)
			{
				m_TargetedShell.OnUntargeted();
				m_TargetedShell = null;
			}

			// Target the new shell
			if (closestShell != null)
			{
				closestShell.OnTargeted();
				m_TargetedShell = closestShell;
			}
		}

		// Check for shell pickup
		if (m_TargetedShell != null && !m_TargetedShell.IsPickedUp && m_PickedUpShell == null && PlayerInputManager.Instance.GetButtonDown(m_Controller))
		{
			OnShellPickedUp();
		}
	}

	public void OnGameOver()
	{
		m_Animation.CrossFade (IDLE_ANIM);
		Destroy(m_Rigidbody);
	}

	public int CompareTo(object obj) 
	{
		Hermit other = obj as Hermit;
		if (HasShellEquipped)
		{
			if (other.HasShellEquipped)
			{
				return other.Fatness - Fatness;
			}
			else
			{
				return -1;
			}
		}

		if (other.HasShellEquipped)
		{
			return 1;
		}
		else
		{
			return other.Fatness - Fatness;
		}
	}
}
