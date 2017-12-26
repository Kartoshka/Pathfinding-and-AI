using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    [Header("Obstacles")]
    public int numobstacles = 4;
    public GameObject obstaclePrefab;
    public float obstacleRadius;

    [Space(10)]
    [Header("Shoppers")]
    public int numShoppers = 10;
    public AStarAgent shopperPrefab;


    [Space(10)]
    [Header("Zones")]
    public float width;
    public float height;
    public GameObject zone1;
    public GameObject zone2;

    public void SpawnObstacles()
    {
        for(int i=0;i<numobstacles;i++)
        {
            bool spawned = false;
            Vector3 position = Vector3.zero;
            while (!spawned)
            {
                Vector3 chosenCenter = Random.Range(0,2) == 0 ? zone1.transform.position : zone2.transform.position;
                position = chosenCenter + new Vector3(Random.Range(-height / 2.0f, height / 2.0f), 0,Random.Range(-width/2.0f, width / 2.0f));

                spawned = Physics.CheckSphere(position, obstacleRadius);
            }

            Instantiate(obstaclePrefab, position, Quaternion.identity);
        }
    }

    public Vector3 GetRandomPos()
    {
        Vector3 chosenCenter = Random.Range(0, 2) == 0 ? zone1.transform.position : zone2.transform.position;
        return chosenCenter + new Vector3(Random.Range(-height / 2.0f, height / 2.0f), 0, Random.Range(-width / 2.0f, width / 2.0f)); ;
    }

    public void SpawnShoppers(WorldGrid wGrid)
    {
        for (int i = 0; i < numShoppers; i++)
        {
            bool spawned = false;
            Vector3 position = Vector3.zero;
            while (!spawned)
            {
                position = wGrid.GetRandomCell().worldPosition;
                spawned = Physics.CheckSphere(position, wGrid.cellSize);
            }

            AStarAgent a = Instantiate(shopperPrefab, position, Quaternion.identity) as AStarAgent ;
            a.grid = wGrid;
            a.debugColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f),a.debugColor.a);
        }
    }
    
    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(zone1.transform.position + Vector3.up * 5 / 2, new Vector3(height, 5, width));
        Gizmos.DrawWireCube(zone2.transform.position + Vector3.up * 5 / 2, new Vector3(height, 5, width));

    }


}
