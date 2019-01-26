using UnityEngine;
using System.Collections;

public class Timer 
{
	public delegate void Callback();
	public Callback m_OnDone;
	public Callback m_OnUpdate;

	protected float m_Time = 0.0f;
	protected float m_ElapsedTime = 0.0f;
	protected bool m_IsDone = false;
	protected bool m_IsStarted = false;

	public float Ratio
	{
		get
		{
			return Mathf.Clamp01(m_ElapsedTime / m_Time);
		}
	}

	public bool IsDone
	{
		get
		{
			return m_ElapsedTime >= m_Time;
		}
	}

	public bool IsStarted
	{
		get
		{
			return m_IsStarted;
		}
	}

	public float StartTime
	{
		get
		{
			return m_Time;
		}
	}

	public float ElapsedTime
	{
		get
		{
			return m_ElapsedTime;
		}
	}

	public Timer(float time = 0.0f)
	{
		m_Time = time;
	}

	public void Start(float time)
	{
		m_Time = time;
		Start();
	}

	public void Start()
	{
		if (m_Time > 0.0f)
		{
			m_IsStarted = true;
			m_ElapsedTime = 0.0f;
		}
	}

	public void Stop()
	{
		m_ElapsedTime = 0.0f;
		m_IsStarted = false;
	}

	public void Update()
	{
		if (m_IsStarted && m_ElapsedTime < m_Time)
		{
			m_ElapsedTime += Time.deltaTime;
			if (m_OnUpdate != null)
			{
				m_OnUpdate();
			}

			if (m_ElapsedTime >= m_Time)
			{
				m_ElapsedTime = m_Time;

				if (m_OnDone != null)
				{
					m_OnDone();
				}
			}
		}
	}
}
