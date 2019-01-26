using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlanctonSpawner : MonoBehaviour {

	[Header("Spawn Values")]
	public int m_MinPlanctonOnScreen = 5;
	public int m_MaxPlanctonOnScreen = 10;
	public float m_MinRefreshTime = 2;
	public float m_MaxRefreshTime = 5;
	public float m_PlanctonSize;

	public GameObject m_PlanctonPrefab;

	private int m_PlanctonOnScreen = 0;
	private List<Plancton> m_UnusedPlancton = new List<Plancton>();
	private List<Plancton> m_UsedPlancton = new List<Plancton>();

	private float m_RefreshTime = 0;
	private bool m_ShouldSpawn = false;
	
	// Update is called once per frame
	private void Update ()
	{
		if (m_ShouldSpawn)
		{
			m_RefreshTime -= Time.deltaTime;

			if (m_RefreshTime <= 0 || m_PlanctonOnScreen < m_MinPlanctonOnScreen) {
				int numberOfPlanctonToSpawn = UnityEngine.Random.Range (Mathf.Max (0, m_MinPlanctonOnScreen - m_PlanctonOnScreen), m_MaxPlanctonOnScreen - m_PlanctonOnScreen + 1);
				for (int i = 0; i < numberOfPlanctonToSpawn; i++) {
					Spawn ();
				}

				if (m_RefreshTime <= 0) {
					m_RefreshTime = UnityEngine.Random.Range (m_MinRefreshTime, m_MaxRefreshTime);				
				}
			}
		}
	}

	public void Initialize()
	{
		Reset ();
		//Spawn initial plancton on screen.
		int startNumber = UnityEngine.Random.Range(m_MinPlanctonOnScreen, m_MaxPlanctonOnScreen + 1);

		for (int i = 0; i < startNumber; i++)
		{
			Spawn ();
		}

		m_RefreshTime = UnityEngine.Random.Range (m_MinRefreshTime, m_MaxRefreshTime);

		m_ShouldSpawn = true;
	}

	public void StopSpawning()
	{
		m_ShouldSpawn = false;
	}

	public void Reset()
	{
		StopSpawning();
		if (m_UsedPlancton.Count > 0)
		{
			for (int i = m_UsedPlancton.Count - 1; i >= 0; i--)
			{
				m_UsedPlancton [i].Die ();
			}
		}
	}

	//Private
	private void Spawn()
	{
		float halfCameraWidth = Camera.main.orthographicSize * Camera.main.aspect;
		float halfCameraHeight = Camera.main.orthographicSize;

		Collider2D[] hits;
		Vector3 planctonPosition;
		do
		{
			planctonPosition = new Vector3 (UnityEngine.Random.Range (-halfCameraWidth + m_PlanctonSize, halfCameraWidth - m_PlanctonSize),
				UnityEngine.Random.Range (-halfCameraHeight + m_PlanctonSize, halfCameraHeight - m_PlanctonSize),
				transform.position.z);

			hits = Physics2D.OverlapCircleAll (planctonPosition, m_PlanctonSize, 1 << LayerMask.NameToLayer("Plancton"));
		} while(hits.Length > 0);

		Plancton plancton;
		if (m_UnusedPlancton.Count > 0)
		{
			plancton = m_UnusedPlancton [0];
			m_UnusedPlancton.RemoveAt (0);
		}
		else
		{
			GameObject planctonObj = Instantiate (m_PlanctonPrefab, planctonPosition, Quaternion.identity, transform);
			plancton = planctonObj.GetComponent<Plancton> ();
		}

		plancton.transform.position = planctonPosition;
		plancton.m_OnPlanctonDied += OnPlanctonDied;
		plancton.Spawn();

		m_UsedPlancton.Add (plancton);
		m_PlanctonOnScreen++;
	}

	private void OnPlanctonDied(Plancton plancton)
	{
		plancton.m_OnPlanctonDied -= OnPlanctonDied;
		m_PlanctonOnScreen--;

		m_UsedPlancton.Remove (plancton);
		m_UnusedPlancton.Add (plancton);
	}
}
