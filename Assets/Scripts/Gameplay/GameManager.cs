using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public WorldGrid grid;
    public Character characterPrefab;

	public int NumberEnemies = 3;
	public float checkDelay = 1.0f;

	public string PedestrianTagName = "Pedestrian";
	public Pedestrians pedestrian_prefab;

	private Coroutine enemySpawner;
	// Use this for initialization
	void Start () {
        grid.Init();

        //Get all rooms, pick random one
        GameObject[] rooms = GameObject.FindGameObjectsWithTag("Room");
		int chosen = Random.Range (0, rooms.Length);

		//Spawn character in random location
		Character char_spawn = Instantiate (characterPrefab, grid.GetCell (rooms [chosen].transform.position).worldPosition, Quaternion.identity);
		char_spawn.grid = this.grid;
		//Start character's AI
		char_spawn.Init();
		//Start enemy spawner
		enemySpawner = StartCoroutine(SpawnEnemies());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnDestroy()
	{
		if (enemySpawner != null)
		{
			StopCoroutine (enemySpawner);
		}
	}

	IEnumerator SpawnEnemies()
	{

		while (true)
		{
			GameObject[] enemies = GameObject.FindGameObjectsWithTag (PedestrianTagName);
			//Spawn an enemy ever X seconds as long as there are not as many as we need
			while (enemies.Length < NumberEnemies)
			{
				GameObject[] spawn = GameObject.FindGameObjectsWithTag ("StartEnemy");
				//Spawn them at a random start point
				Pedestrians spawned = Instantiate (pedestrian_prefab, spawn [Random.Range (0, spawn.Length)].transform.position, Quaternion.identity);
				spawned.grid = this.grid;
				spawned.Activate ();
				yield return new WaitForSeconds (checkDelay);
				enemies = GameObject.FindGameObjectsWithTag (PedestrianTagName);
			}
			yield return new WaitForEndOfFrame ();
		}
	}
}
