using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpriteList
{
	public Vector3 Offset = Vector3.zero;
	public Sprite[] Sprites = null;
}

public class Shell : MonoBehaviour 
{
	private const float EXPLODE_VELOCITY = 10.0f;
	private const float EXPLODE_ROTATION = 400.0f;
	private const float DESTROY_DELAY = 3.0f;

	private int m_Fatness = 0;
	[SerializeField] private PolygonCollider2D m_Collider = null;
	[SerializeField] private SpriteRenderer m_SpriteRenderer = null;
	[SerializeField] private Animation m_Animation = null;
	[SerializeField] private int m_DebugFatness = 0;
	private int m_TargetedCount = 0;
	private bool m_IsPickedUp = false;
	private bool m_IsExploded = false;
	private Vector3 m_ExplodeDirection = Vector3.up;

	public SpriteList[] m_Shells = null;

	public int Fatness
	{
		get
		{
			return m_Fatness;
		}
	}

	public int TargetedCount
	{
		get
		{
			return m_TargetedCount;
		}
	}

	public bool IsPickedUp
	{
		get
		{
			return m_IsPickedUp;
		}
	}

	private void Awake()
	{
		Init(m_DebugFatness);
	}

	private void Update()
	{
		if (m_IsExploded)
		{
			transform.position += m_ExplodeDirection * EXPLODE_VELOCITY * Time.deltaTime;
			transform.Rotate(0f, 0f, EXPLODE_ROTATION * Time.deltaTime);
		}
	}

	public void Init(int fatness)
	{
		m_Fatness = fatness;
		// TODO: Select an appropriate sprite
		// TODO: Do we need to update the polygon collider??

		// TODO: Debugging tests for dynamic sprite and collider
		Sprite sprite = m_Shells[m_Fatness].Sprites[0];
		m_SpriteRenderer.sprite = sprite;
		List<Vector2> physicsShape = new List<Vector2>();
		sprite.GetPhysicsShape(0, physicsShape);
		m_Collider.points = physicsShape.ToArray();
	}

	public void OnTargeted()
	{
		++m_TargetedCount;

		if (m_TargetedCount > 0)
		{
			m_Animation.Play();
		}
	}

	public void OnUntargeted()
	{
		--m_TargetedCount;

		if (m_TargetedCount <= 0)
		{
			m_Animation.Stop();
		}
	}

	public void OnPickedUp()
	{
		m_IsPickedUp = true;
		m_Collider.enabled = false;
		m_Animation.Stop();
		m_TargetedCount = 0;
	}

	public void Explode()
	{
		m_IsExploded = true;
		transform.SetParent(null);

		// Eurk
		m_ExplodeDirection.x = Random.Range(-1.0f, 1.0f);
		m_ExplodeDirection.Normalize();
		Destroy(gameObject, DESTROY_DELAY);
	}
}
