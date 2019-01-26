using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour 
{
	private int m_Fatness = 0;
	[SerializeField] private PolygonCollider2D m_Collider = null;
	[SerializeField] private SpriteRenderer m_SpriteRenderer = null;
	[SerializeField] private Sprite m_Sprite = null;
	[SerializeField] private Animation m_Animation = null;
	private int m_TargetedCount = 0;
	private bool m_IsPickedUp = false;

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
		Init(4);
	}

	public void Init(int fatness)
	{
		m_Fatness = fatness;
		// TODO: Select an appropriate sprite
		// TODO: Do we need to update the polygon collider??

		// TODO: Debugging tests for dynamic sprite and collider
		m_SpriteRenderer.sprite = m_Sprite;
		List<Vector2> physicsShape = new List<Vector2>();
		m_Sprite.GetPhysicsShape(0, physicsShape);
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
	}
}
