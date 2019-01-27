using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HermitMaster : MonoBehaviour 
{
	[SerializeField] private List<Hermit> m_Hermits = new List<Hermit> ();
	[SerializeField] private BeachOfDespair m_BeachOfDespair = null;
	[SerializeField] private AudioClip m_SelectClip = null;
	[SerializeField] private AudioClip m_UnselectClip = null;
	[SerializeField] private ShellSpawner m_ShellSpawner = null;

	public List<Hermit> Hermits
	{
		get
		{
			return m_Hermits;
		}
	}

	private void Awake()
	{
		// Hide all hermits on launch
		for (int i = 0; i < m_Hermits.Count; ++i)
		{
			m_Hermits [i].m_OnHermitDied += OnHermitDied;
			m_Hermits[i].gameObject.SetActive(false);
		}
	}

	private void Update()
	{
		if (m_BeachOfDespair.IsGameStarted)
		{
			return;
		}

		bool controllerStateChanged = false;
		for (int i = 0; i < m_Hermits.Count; ++i)
		{
			PlayerInputManager.ControllerInput controller = (PlayerInputManager.ControllerInput)(i + 1);
			if (PlayerInputManager.Instance.GetButtonDown(controller))
			{
				controllerStateChanged = true;
				GameObject hermit = m_Hermits[i].gameObject;
				hermit.SetActive(!hermit.activeSelf);
				m_Hermits[i].OnControllerReady(controller);
				// TODO: Remap the keyboard as well

				if (hermit.activeSelf)
				{
					m_BeachOfDespair.SFXSource.PlayOneShot(m_SelectClip);
				}
				else
				{
					m_BeachOfDespair.SFXSource.PlayOneShot(m_UnselectClip);
				}
			}
		}

		if (controllerStateChanged)
		{
			m_BeachOfDespair.OnControllerReady();
		}
	}

	public int GetHermitReadyCount()
	{
		int hermitReadyCount = 0;
		for (int i = 0; i < m_Hermits.Count; ++i)
		{
			if (m_Hermits[i].IsReady)
			{
				++hermitReadyCount;
			}
		}

		return hermitReadyCount;
	}

	private void OnHermitDied(Hermit hermit)
	{
		Hermit newHermit = Instantiate (hermit, transform);
		newHermit.m_OnHermitDied += OnHermitDied;
		newHermit.OnControllerReady (hermit.Controller);
		m_Hermits.Add (newHermit);
		newHermit.Reset ();
		m_ShellSpawner.OnNewHermit (newHermit);
	}
}
