using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour {

	public float m_Amplitude = 10;
	public bool m_Pause = false;

	private Vector3 m_StartPosition;
	private float m_CurrentTime = 0;
	private Vector3 m_TempVector = new Vector3();

	// Use this for initialization
	private void Start ()
	{
		m_StartPosition = transform.localPosition;
		m_TempVector.x = m_StartPosition.x;
		m_TempVector.z = m_StartPosition.z;

		m_CurrentTime = UnityEngine.Random.Range (0, 2 * Mathf.PI);
	}
	
	// Update is called once per frame
	private void Update ()
	{
		if (!m_Pause)
		{
			m_CurrentTime += Time.deltaTime;

			if (m_CurrentTime >= 2 * Mathf.PI) {
				m_CurrentTime = 0;
			}
				
			m_TempVector.y = m_StartPosition.y + (Mathf.Sin (m_CurrentTime) / 2 - 0.5f) * m_Amplitude;
			transform.localPosition = m_TempVector;
		}
	}
}
