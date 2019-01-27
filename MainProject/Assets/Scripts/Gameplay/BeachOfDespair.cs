using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeachOfDespair : MonoBehaviour 
{
	private const int COUNTDOWN_TIME = 5;
	private const int MIN_HERMIT_COUNT = 2;
	private const int GAME_TIME = 120;
	private const float WINNING_OFFSET_BETWEEN_HERMITS = 0.1f;

	[SerializeField] private TextMesh[] m_Countdown = null;
	[SerializeField] private HermitMaster m_Master = null;
	[SerializeField] private GameObject m_TitleText = null;
	[SerializeField] private GameObject m_PressToJoinText = null;
	[SerializeField] private GameObject m_NeedMoreHermitsText = null;
	[SerializeField] private TextMesh[] m_GameTimeText = null;
	[SerializeField] private AudioSource m_SFXSource = null;
	[SerializeField] private GameObject m_WinningScreen = null;
	[SerializeField] private Transform m_WinningHermitsAnchor = null;

	private Timer m_CountdownTimer = new Timer(COUNTDOWN_TIME);
	private Timer m_GameTimer = new Timer(GAME_TIME);
	private bool m_IsGameStarted = false;
	private bool m_IsGameOver = false;

	[SerializeField] private PlanctonSpawner m_PlanctonSpawner = null;
	[SerializeField] private WaveCleaner m_WaveCleaner = null;
	[SerializeField] private Foot m_Foot = null;
	[SerializeField] private ShellSpawner m_ShellSpawner = null;

	public bool IsGameStarted
	{
		get
		{
			return m_IsGameStarted;
		}
	}

	public bool IsGameOver
	{
		get
		{
			return m_IsGameOver;
		}
	}

	public AudioSource SFXSource
	{
		get
		{
			return m_SFXSource;
		}
	}

	private void Awake()
	{
		m_TitleText.SetActive(true);
		m_PressToJoinText.SetActive(true);
		m_NeedMoreHermitsText.SetActive(false);
		SetTextsActive(m_Countdown, false);
		SetTextsActive(m_GameTimeText, false);
		m_WinningScreen.SetActive(false);

		m_CountdownTimer.m_OnUpdate += UpdateCountdown;
		m_CountdownTimer.m_OnDone += OnGameStarted;
		m_GameTimer.m_OnUpdate += UpdateGameTime;
		m_GameTimer.m_OnDone += OnGameOver;
	}

	private void Update()
	{
		m_CountdownTimer.Update();
		m_GameTimer.Update();

		if (m_IsGameOver)
		{
			for (int i = 0; i < m_Master.Hermits.Count; ++i)
			{
				PlayerInputManager.ControllerInput controller = (PlayerInputManager.ControllerInput)(i + 1);
				if (PlayerInputManager.Instance.GetButtonDown(controller))
				{
					UnityEngine.SceneManagement.SceneManager.LoadScene(0);
					return;
				}
			}
		}
	}

	private void OnDestroy()
	{
		m_CountdownTimer.m_OnUpdate -= UpdateCountdown;
		m_CountdownTimer.m_OnDone -= OnGameStarted;
		m_GameTimer.m_OnUpdate -= UpdateGameTime;
		m_GameTimer.m_OnDone -= OnGameOver;
	}

	private void UpdateCountdown()
	{
		SetTexts(m_Countdown, Mathf.CeilToInt(m_CountdownTimer.StartTime - m_CountdownTimer.ElapsedTime).ToString());
	}

	private void UpdateGameTime()
	{
		SetTexts(m_GameTimeText, Mathf.CeilToInt(m_GameTimer.StartTime - m_GameTimer.ElapsedTime).ToString());
	}

	private void OnGameStarted()
	{
		m_IsGameStarted = true;
		SetTextsActive(m_GameTimeText, true);
		m_GameTimer.Start();
		SetTextsActive(m_Countdown, false);
		m_NeedMoreHermitsText.SetActive(false);
		m_PressToJoinText.SetActive(false);

		m_PlanctonSpawner.Initialize ();
		m_WaveCleaner.Init ();
		m_Foot.Init ();
		m_ShellSpawner.Init ();
	}

	public void OnGameOver()
	{
		m_IsGameOver = true;

		m_GameTimer.Stop();
		m_PlanctonSpawner.StopSpawning ();
		m_WaveCleaner.Stop ();
		m_Foot.Stop ();

		m_WinningScreen.SetActive(true);
		SetTextsActive(m_Countdown, false);

		// Sort hermits in winning order
		List<Hermit> hermits = new List<Hermit>();
		for (int i = 0; i < m_Master.Hermits.Count; ++i)
		{
			if (m_Master.Hermits[i].IsReady)
			{
				hermits.Add(m_Master.Hermits[i]);
			}
		}
		hermits.Sort();

		// Position hermits
		float width = (hermits.Count - 1) * WINNING_OFFSET_BETWEEN_HERMITS;
		Vector3 localPosition = new Vector3(-width / 2, 0.0f, 0.0f);
		for (int i = 0; i < hermits.Count; ++i)
		{
			hermits[i].transform.SetParent(m_WinningHermitsAnchor);
			hermits[i].transform.localPosition = localPosition;
			localPosition.x += WINNING_OFFSET_BETWEEN_HERMITS;
			hermits[i].OnGameOver();
		}
	}

	public void OnControllerReady()
	{
		int hermitReadyCount = m_Master.GetHermitReadyCount();
		if (hermitReadyCount >= MIN_HERMIT_COUNT)
		{
			m_CountdownTimer.Start();
			SetTextsActive(m_Countdown, true);
			UpdateCountdown();
			m_TitleText.SetActive(false);
			m_NeedMoreHermitsText.SetActive(false);
		}
		else
		{
			m_CountdownTimer.Stop();
			SetTextsActive(m_Countdown, false);

			if (hermitReadyCount > 0)
			{
				m_TitleText.SetActive(false);
				m_NeedMoreHermitsText.SetActive(true);
			}
			else
			{
				m_TitleText.SetActive(true);
				m_PressToJoinText.SetActive(true);
				m_NeedMoreHermitsText.SetActive(false);
			}
		}
	}

	private void SetTexts(TextMesh[] texts, string text)
	{
		for (int i = 0; i < texts.Length; ++i)
		{
			texts[i].text = text;
		}
	}

	private void SetTextsActive(TextMesh[] texts, bool isActive)
	{
		for (int i = 0; i < texts.Length; ++i)
		{
			texts[i].gameObject.SetActive(isActive);
		}
	}
}
