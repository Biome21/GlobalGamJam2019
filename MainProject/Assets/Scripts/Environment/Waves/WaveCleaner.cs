using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WaveCleaner : MonoBehaviour {

	private const float MIN_WAVE_VOLUME = 0.1f;
	private const float MAX_WAVE_VOLUME = 0.4f;
	private const float BIG_WAVE_VOLUME = 1.0f;
	private const float WAVE_SPEED = 0.55f;

	public Action m_OnWaveFullScreen;

	[Header("Wave Clean Values")]
	public float m_MinWaveTime = 10f;
	public float m_MaxWaveTime = 15f;

	[Header("Managers")]
	public Foot m_Foot;

	[SerializeField] private float m_BigWaveVolumeRatio = 0.0f;
	[SerializeField] private AudioSource m_WaveLoop = null;

	private bool m_Paused = true;
	private float m_WaveTimer;
	private Animation m_WaveCleanAnimation;
	private bool m_HasWavePending = false;

	// Use this for initialization
	private void Start ()
	{
		m_WaveCleanAnimation = transform.GetComponent<Animation> ();
	}
	
	// Update is called once per frame
	private void Update ()
	{
		if (!m_Paused && !m_HasWavePending)
		{
			m_WaveTimer -= Time.deltaTime;

			if (m_WaveTimer <= 0)
			{
				m_HasWavePending = true;
			}
		}

		float waveVolume = Mathf.Lerp(MIN_WAVE_VOLUME, MAX_WAVE_VOLUME, Mathf.Abs(Mathf.Sin(Time.realtimeSinceStartup * WAVE_SPEED)));
		m_WaveLoop.volume = Mathf.Lerp(waveVolume, BIG_WAVE_VOLUME, m_BigWaveVolumeRatio);
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

	public void Resume()
	{
		m_Paused = false;
	}

	public void StartWave()
	{
		if (m_HasWavePending)
		{
			m_HasWavePending = false;
			m_Paused = true;
			m_Foot.Stop ();
			m_WaveCleanAnimation.Play ();
		}
	}

	private void OnWaveFullScreen()
	{
		if (m_OnWaveFullScreen != null)
		{
			m_OnWaveFullScreen ();
		}	
	}

	private void OnWaveDone()
	{
		m_Paused = false;
		m_WaveTimer = UnityEngine.Random.Range (m_MinWaveTime, m_MaxWaveTime);

		m_Foot.Resume ();
	}
}
