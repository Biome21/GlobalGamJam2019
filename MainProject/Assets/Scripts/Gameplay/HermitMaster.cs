using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HermitMaster : MonoBehaviour 
{
	[SerializeField] private Hermit[] m_Hermits = null;
	[SerializeField] private BeachOfDespair m_BeachOfDespair = null;
	[SerializeField] private AudioClip m_SelectClip = null;
	[SerializeField] private AudioClip m_UnselectClip = null;

	public Hermit[] Hermits
	{
		get
		{
			return m_Hermits;
		}
	}

	private void Awake()
	{
		// Hide all hermits on launch
		for (int i = 0; i < m_Hermits.Length; ++i)
		{
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
		for (int i = 0; i < m_Hermits.Length; ++i)
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
		for (int i = 0; i < m_Hermits.Length; ++i)
		{
			if (m_Hermits[i].IsReady)
			{
				++hermitReadyCount;
			}
		}

		return hermitReadyCount;
	}
}
