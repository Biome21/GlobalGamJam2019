using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Plancton : MonoBehaviour {

	public Action<Plancton> m_OnPlanctonDied;

	[Header("Life Values")]
	public float m_MinPlanctionLifetime = 5f;
	public float m_MaxPlanctonLifetime = 10f;

	private float m_LifetimeTimer;
	private bool m_IsAlive = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	private void Update ()
	{
		if (m_IsAlive)
		{
			m_LifetimeTimer -= Time.deltaTime;

			if (m_LifetimeTimer <= 0)
			{
				Die();
			}
		}
	}

	public void Spawn()
	{
		m_LifetimeTimer = UnityEngine.Random.Range (m_MinPlanctionLifetime, m_MaxPlanctonLifetime);
		m_IsAlive = true;
		gameObject.SetActive(true);
	}

	public void Die()
	{
		m_IsAlive = false;
		gameObject.SetActive(false);
		m_OnPlanctonDied (this);
	}
}
