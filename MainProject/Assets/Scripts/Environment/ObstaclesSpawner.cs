using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclesSpawner : MonoBehaviour {

	public int m_MinObstaclesOnScreen = 5;
	public int m_MaxObstaclesOnScreen = 10;

	public WaveCleaner m_WaveCleaner;
	public List<GameObject> m_Obstacles = new List<GameObject>();

	private List<GameObject> m_ObstaclesOnScreen = new List<GameObject>();

	// Use this for initialization
	private void Start ()
	{
		m_WaveCleaner.m_OnWaveFullScreen += OnWaveFullScreen;
	}

	public void Init()
	{
		SpawnObstacles ();		
	}
		
	private void OnWaveFullScreen()
	{
		SpawnObstacles ();
	}

	private void SpawnObstacles()
	{
		//Destroy the ones on screen to spawn new one
		for (int i = m_ObstaclesOnScreen.Count - 1; i >= 0; i--)
		{
			Destroy (m_ObstaclesOnScreen [i]);
		}
		m_ObstaclesOnScreen.Clear ();

		int numberOfObstaclesToSpawn = UnityEngine.Random.Range (m_MinObstaclesOnScreen, m_MaxObstaclesOnScreen + 1);

		for (int i = 0; i < numberOfObstaclesToSpawn; i++)
		{
			GameObject obstacle = Instantiate (m_Obstacles [UnityEngine.Random.Range (0, m_Obstacles.Count)], new Vector3(1000, 1000, transform.position.z), Quaternion.identity,  transform);
			m_ObstaclesOnScreen.Add (obstacle);
			RandomizeObstaclelPosition (obstacle);
		}
	}

	private void RandomizeObstaclelPosition(GameObject obstacle)
	{
		Vector2 obstacleSize = obstacle.GetComponent<PolygonCollider2D> ().bounds.extents;
		float halfCameraWidth = Camera.main.orthographicSize * Camera.main.aspect;
		float halfCameraHeight = Camera.main.orthographicSize;

		Collider2D[] hits;
		Vector3 obstaclePosition;

		do
		{
			obstaclePosition = new Vector3 (UnityEngine.Random.Range (-halfCameraWidth + obstacleSize.x, halfCameraWidth - obstacleSize.x),
				UnityEngine.Random.Range (-halfCameraHeight + obstacleSize.y, halfCameraHeight - obstacleSize.y),
				transform.position.z);

			hits = Physics2D.OverlapCircleAll (obstaclePosition, Mathf.Max(obstacleSize.x, obstacleSize.y), LayerMask.GetMask("Plancton", "Hermit", "Shell", "Obstacle"));
		} while(hits.Length > 0);

		obstacle.transform.position = new Vector3 (obstaclePosition.x, obstaclePosition.y, transform.position.z);
	}
}
