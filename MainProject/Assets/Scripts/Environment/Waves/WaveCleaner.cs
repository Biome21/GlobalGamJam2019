using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveCleaner : MonoBehaviour {

	[Header("Wave Clean Values")]
	public float m_MinWaveTime = 10f;
	public float m_MaxWaveTime = 15f;

	[Header("Managers")]
	public PlanctonSpawner m_PlanctonSpawner;

	private bool m_Paused = true;
	private float m_WaveTimer;
	private Animation m_WaveCleanAnimation;

	// Use this for initialization
	private void Start ()
	{
		m_WaveCleanAnimation = transform.GetComponent<Animation> ();
	}
	
	// Update is called once per frame
	private void Update ()
	{
		if (!m_Paused)
		{
			m_WaveTimer -= Time.deltaTime;

			if (m_WaveTimer <= 0)
			{
				StartWave ();
			}
		}
	}

	public void Init()
	{
		m_Paused = false;
		m_WaveTimer = UnityEngine.Random.Range (m_MinWaveTime, m_MaxWaveTime);
	}

	public void Stop()
	{
		m_Paused = true;
	}

	private void StartWave()
	{
		m_Paused = true;
		m_WaveCleanAnimation.Play ();
	}

	private void OnWaveFullScreen()
	{
		m_PlanctonSpawner.Initialize ();	
	}

	private void OnWaveDone()
	{
		m_Paused = false;
		m_WaveTimer = UnityEngine.Random.Range (m_MinWaveTime, m_MaxWaveTime);
	}
}
