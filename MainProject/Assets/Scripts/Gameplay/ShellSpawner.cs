using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellSpawner : MonoBehaviour {

	public Shell m_ShellPrefab;
	public HermitMaster m_HermitMaster;
	public WaveCleaner m_WaveCleaner;

	private List<Shell> m_ShellsOnScreen = new List<Shell>();

	// Use this for initialization
	private void Start ()
	{
		m_WaveCleaner.m_OnWaveFullScreen +=	OnWaveFullScreen;

		for (int i = 0; i < m_HermitMaster.Hermits.Count; i++)
		{
			m_HermitMaster.Hermits [i].m_OnShellExplode += OnShellExplode;
		}
	}

	public void Init()
	{
		CheckToSpawnShells ();
	}

	private void OnWaveFullScreen()
	{
		CheckToSpawnShells ();
	}

	private void CheckToSpawnShells()
	{
		List<Hermit> hermitsOnScreen = new List<Hermit> (m_HermitMaster.Hermits);
		for (int i = 0; i < Hermit.MAXIMUM_FATNESS; i++)
		{
			int numberOfHermitOnScreenForFatness = hermitsOnScreen.FindAll (x => x.Fatness == i && x.IsReady && !x.HasShellEquipped).Count;
			int numberOfShellsOnScreenForFatness = m_ShellsOnScreen.FindAll (x => x.Fatness == i && !x.IsPickedUp).Count;

			int numberOfShellsToSpawn = Mathf.Max (0, numberOfHermitOnScreenForFatness - numberOfShellsOnScreenForFatness);

			for (int j = 0; j < numberOfShellsToSpawn; j++)
			{
				SpawnShell (i);
			}
		}

		for (int i = 0; i < m_ShellsOnScreen.Count; i++)
		{
			if (!m_ShellsOnScreen [i].IsPickedUp)
			{
				RandomizeShellPosition (m_ShellsOnScreen [i]);
			}
		}
	}

	private void SpawnShell(int fatness)
	{
		Shell shell = Instantiate (m_ShellPrefab, new Vector3(1000f, 1000f, transform.position.z), Quaternion.identity, transform);
		shell.Init (fatness);

		m_ShellsOnScreen.Add (shell);
	}

	private void OnShellExplode(Shell explodedShell)
	{
		m_ShellsOnScreen.Remove (explodedShell);		
	}

	private void RandomizeShellPosition(Shell shell)
	{
		Vector2 shellSize = shell.GetComponent<PolygonCollider2D> ().bounds.extents;
		float halfCameraWidth = Camera.main.orthographicSize * Camera.main.aspect;
		float halfCameraHeight = Camera.main.orthographicSize;

		Collider2D[] hits;
		Vector3 shellPosition;

		do
		{
			shellPosition = new Vector3 (UnityEngine.Random.Range (-halfCameraWidth + shellSize.x, halfCameraWidth - shellSize.x),
				UnityEngine.Random.Range (-halfCameraHeight + shellSize.y, halfCameraHeight - shellSize.y),
				transform.position.z);

			hits = Physics2D.OverlapCircleAll (shellPosition, Mathf.Max(shellSize.x, shellSize.y), LayerMask.GetMask("Plancton", "Hermit", "Shell", "Obstacle"));
		} while(hits.Length > 0);

		shell.transform.position = new Vector3 (shellPosition.x, shellPosition.y, transform.position.z);
	}
}
