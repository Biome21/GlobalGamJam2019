using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Foot : MonoBehaviour {

	public enum eFootState
	{
		IDLE,
		SHADOW_APPEARING,
		FOOT_APPEARING,
		FOOT_ON_GROUND,
		FOOT_LIFT
	};

	[Header("Foot Spawn Values")]
	public Transform m_FootShadow;
	public Renderer m_FootShadowRenderer;
	public Transform m_Foot;

	public float m_MinFootTimer = 10;
	public float m_MaxFootTimer = 15;
	public float m_ShadowAppearanceTime = 1;
	public float m_ShadowStartSize = 0.2f;
	public float m_FootAppearanceSpeed = 1f;
	public float m_FootOnGroundTime = 2f;
	public float m_FootLiftSpeed = 2f;

	[Header("Other Managers")]
	public WaveCleaner m_WaveCleaner;

	private eFootState m_CurrentState = eFootState.IDLE;
	private float m_FootIdleTimer = 0;
	private float m_FootShadowTimer = 0;
	private float m_FootAppearanceRatio = 0;
	private float m_FootOnGroundTimer = 0;
	private float m_FootLiftRatio = 0;

	private bool m_Paused = true;

	private PolygonCollider2D m_FootCollider;
	private Vector3 m_FootHiddenPosition;
	private Vector3 m_FootStartPosition;

	// Use this for initialization
	private void Start ()
	{
		m_FootCollider = GetComponentInChildren<PolygonCollider2D> ();
		m_FootShadow.gameObject.SetActive (false);
		m_FootHiddenPosition = m_Foot.transform.localPosition;
	}
	
	// Update is called once per frame
	private void Update ()
	{
		if (!m_Paused) {
			if (m_CurrentState == eFootState.IDLE) {
				m_FootIdleTimer -= Time.deltaTime;

				if (m_FootIdleTimer <= 0) {
					StartFootShadow ();
				}
			}
		}
		if (m_CurrentState == eFootState.SHADOW_APPEARING) {
			m_FootShadowTimer -= Time.deltaTime;

			if (m_FootShadowTimer <= 0) {
				StartFootObstacle ();
			}
			ShadowUpdate ();
		} else if (m_CurrentState == eFootState.FOOT_APPEARING) {
			m_FootAppearanceRatio += Time.deltaTime * m_FootAppearanceSpeed;

			if (m_FootAppearanceRatio >= 1) {
				OnFootHitGround ();
			}
			FootUpdate ();
		} else if (m_CurrentState == eFootState.FOOT_ON_GROUND) {
			m_FootOnGroundTimer -= Time.deltaTime;

			if (m_FootOnGroundTimer <= 0) {
				OnFootHitGroundDone ();
			}
		} else if (m_CurrentState == eFootState.FOOT_LIFT)
		{
			m_FootLiftRatio += Time.deltaTime * m_FootLiftSpeed;

			if (m_FootLiftRatio >= 1) {
				OnFootAnimationDone ();
			}
			FootLiftUpdate();
		}
	}

	public void Init()
	{
		m_FootIdleTimer = UnityEngine.Random.Range (m_MinFootTimer, m_MaxFootTimer);
		m_CurrentState = eFootState.IDLE;
		m_Paused = false;
	}

	public void Stop()
	{
		m_Paused = true;
	}

	public void Resume()
	{
		m_Paused = false;
	}

	private void StartFootShadow()
	{
		m_Paused = true;
		m_WaveCleaner.Stop();

		m_FootShadow.gameObject.SetActive (true);
		//Find a spot on screen
		float halfCameraWidth = Camera.main.orthographicSize * Camera.main.aspect;
		float halfCameraHeight = Camera.main.orthographicSize;
		Vector2 footSize = m_FootCollider.bounds.size;
	
		Vector3 footPosition = new Vector3 (UnityEngine.Random.Range (-halfCameraWidth + footSize.x / 2, halfCameraWidth - footSize.x / 2),
			UnityEngine.Random.Range (-halfCameraHeight + footSize.y / 2, halfCameraHeight - footSize.y / 2),
			transform.position.z);

		//Place the shadow
		m_FootShadow.position = new Vector3(footPosition.x, footPosition.y, transform.position.z);
		m_FootShadowTimer = m_ShadowAppearanceTime;
		m_CurrentState = eFootState.SHADOW_APPEARING;
	}

	private void ShadowUpdate()
	{
		float shadowRatio = 1 - m_FootShadowTimer / m_ShadowAppearanceTime;
		Vector3 shadowSize = Vector3.Lerp (new Vector3 (m_ShadowStartSize, m_ShadowStartSize, 1), Vector3.one, shadowRatio);
		m_FootShadow.localScale = shadowSize;
		Color tempColor = m_FootShadowRenderer.material.color;
		tempColor.a = shadowRatio;
		m_FootShadowRenderer.material.color = tempColor;
	}

	private void StartFootObstacle()
	{
		m_CurrentState = eFootState.FOOT_APPEARING;

		//Place the foot in line with the shadow
		m_FootStartPosition = new Vector3 (m_FootShadow.transform.localPosition.x,
												m_FootHiddenPosition.y,
												m_Foot.transform.localPosition.z);
		m_Foot.transform.localPosition = m_FootStartPosition;

		m_FootAppearanceRatio = 0;
	}

	private void FootUpdate()
	{
		Vector3 footLocalPosition = Vector3.Lerp (m_FootStartPosition, m_FootShadow.transform.localPosition, m_FootAppearanceRatio);
		footLocalPosition.z = m_Foot.localPosition.z;
		m_Foot.localPosition = footLocalPosition;
	}

	private void OnFootHitGround()
	{
		m_CurrentState = eFootState.FOOT_ON_GROUND;

		m_FootOnGroundTimer = m_FootOnGroundTime;

		//ScreenShake
		//Raycast
		Collider2D[] results = new Collider2D[4];
		ContactFilter2D contactFilter = new ContactFilter2D();
		contactFilter.SetLayerMask (LayerMask.GetMask ("Hermit"));
		m_FootCollider.transform.position = m_FootShadow.transform.position;
		m_FootCollider.enabled = true;
		Physics2D.OverlapCollider(m_FootCollider, contactFilter, results);

		for (int i = 0; i < results.Length; i++)
		{
			if (results [i] != null) {
				Hermit hermitCollided = results [i].gameObject.GetComponentInParent<Hermit> ();

				if (hermitCollided != null) {
					if (hermitCollided.HasShellEquipped) {
						hermitCollided.ExplodeShell ();
					} else {
						//die
						hermitCollided.Die ();
					}
				}
			}
		}

		m_FootShadow.gameObject.SetActive (false);
	}

	private void OnFootHitGroundDone()
	{
		m_CurrentState = eFootState.FOOT_LIFT;

		m_FootLiftRatio = 0;
	}

	private void FootLiftUpdate()
	{
		Vector3 footLocalPosition = Vector3.Lerp (m_FootShadow.transform.localPosition, m_FootStartPosition, m_FootLiftRatio);
		footLocalPosition.z = m_Foot.localPosition.z;
		m_Foot.localPosition = footLocalPosition;
		m_FootCollider.enabled = false;
	}

	private void OnFootAnimationDone()
	{
		m_Paused = false;
		m_CurrentState = eFootState.IDLE;
		m_FootIdleTimer = UnityEngine.Random.Range (m_MinFootTimer, m_MaxFootTimer);
		m_WaveCleaner.Resume ();
	}
}
