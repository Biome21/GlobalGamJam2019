using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlanctonSpawner : MonoBehaviour {

	[Header("Spawn Values")]
	public int m_MinPlanctonOnScreen = 5;
	public int m_MaxPlanctonOnScreen = 10;
	public float m_MinPlanctionLifetime = 5f;
	public float m_MaxPlanctonLifetime = 10f;
	public float m_CameraMarginX = 50f;
	public float m_CameraMarginY = 50f;
	public float m_PlanctonSize;

	public GameObject m_PlanctonPrefab;

	// Use this for initialization
	private void Start ()
	{
		//Spawn initial plancton on screen.
		int startNumber = UnityEngine.Random.Range(m_MinPlanctonOnScreen, m_MaxPlanctonOnScreen);

		for (int i = 0; i < startNumber; i++)
		{
			Spawn ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//Private
	private void Spawn()
	{
		float halfCameraWidth = Camera.main.orthographicSize * Camera.main.aspect;
		float halfCameraHeight = Camera.main.orthographicSize;

		Collider2D[] hits;
		Vector2 planctonPosition;
		do
		{
			planctonPosition = new Vector2 (UnityEngine.Random.Range (-halfCameraWidth + m_PlanctonSize, halfCameraWidth - m_PlanctonSize),
				UnityEngine.Random.Range (-halfCameraHeight + m_PlanctonSize, halfCameraHeight - m_PlanctonSize));

			hits = Physics2D.OverlapCircleAll (planctonPosition, m_PlanctonSize, 1 << LayerMask.NameToLayer("Plancton"));
			Debug.Log(hits.Length);
		} while(hits.Length > 0);

		Instantiate(m_PlanctonPrefab, planctonPosition, Quaternion.identity, transform);
	}
}
