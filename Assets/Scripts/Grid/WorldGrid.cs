using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;

public class WorldGrid : MonoBehaviour {

	public LayerMask obstacleMask;
	public Vector2 worldGridSize;
	public float cellSize;

	public float gridHeight;

	private float half_cellSize;
	public int gridSizeX;
	public int gridSizeY;

	public Cell[,] grid;

	public bool regenerate =false;

	public bool done = false;
    public int totalWalkable = 0;

    public Color debugColor;
    public Color reservationColor;
    public float verticalScale = 1.0f;

    //Reservation table stores the 2d coordinates in the x and y of the Vector3 and the time in the z coordinate
    private HashSet<Vector3> reservationTable = new HashSet<Vector3>();

	// Use this for initialization
	public void Init () {

        ClearReservations();

        gridSizeX = Mathf.RoundToInt (worldGridSize.x / cellSize);
		gridSizeY = Mathf.RoundToInt (worldGridSize.y / cellSize);

		half_cellSize = cellSize / 2;
		GenerateWalkabilityGraph ();
	}

	void GenerateWalkabilityGraph()
	{
		grid = new Cell[gridSizeX,gridSizeY];
		Vector3 worldBottomLeft = transform.position - Vector3.right * worldGridSize.x/2 - Vector3.forward * worldGridSize.y/2;
		
		for (int x = 0; x < gridSizeX; x ++) {
			for (int y = 0; y < gridSizeY; y ++) {

				Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * cellSize + half_cellSize) + Vector3.forward * (y * cellSize + half_cellSize);
				RaycastHit resultHit;
				bool walkable = (Physics.BoxCast (worldPoint + Vector3.up * gridHeight*1.5f, new Vector3 (half_cellSize, gridHeight/2 , half_cellSize), Vector3.down, out resultHit,Quaternion.identity, gridHeight)); //(Physics.Raycast (worldPoint + Vector3.up * gridHeight, Vector3.down, out resultHit, gridHeight));
				string tag = "";
                if (walkable && resultHit.collider != null)
				{
					walkable = obstacleMask != (obstacleMask | (1 << resultHit.collider.gameObject.layer));
					worldPoint.y = resultHit.point.y;
					tag = resultHit.collider.tag;
                    //worldPoint +=resultHit.normal.normalized *cellSize*0.5f;
				}

				Cell t = new Cell(walkable,worldPoint);
				t.tag = tag;
				t.localPos = new IntVector2 (x, y);
				grid [x, y] = t;

                if(walkable)
                {
                    totalWalkable++;
                }

			}
		}
		done = true;
	}

	void OnDrawGizmos() {

		Gizmos.DrawWireCube(transform.position+Vector3.up*gridHeight/2,new Vector3(worldGridSize.x,gridHeight,worldGridSize.y));
		if (!done)
        {
            return;
        }

        Gizmos.color = debugColor;
        if (grid != null)
        {
            foreach (Cell n in grid)
            {
                if (n.walkable && n != null)
                {
                    Gizmos.DrawCube(n.worldPosition - new Vector3(0, 0.1f, 0), Vector3.one * (cellSize));

                }
            }
        }

    }

	IntVector2 FromWorldPos(Vector3 worldPos)
	{
		worldPos = worldPos - transform.position;
		int x = Mathf.FloorToInt((worldPos.x + worldGridSize.x*0.5f)/ cellSize);
		int y = Mathf.FloorToInt((worldPos.z + worldGridSize.y*0.5f) / cellSize);

		return new IntVector2(x,y);
	}

	public Cell GetCell(Vector3 worldPos)
	{
		IntVector2 pos =FromWorldPos(worldPos);
		if (pos.x >= 0 && pos.x < gridSizeX && pos.y >= 0 && pos.y < gridSizeY)
		{
			return grid [pos.x, pos.y];
		} else
		{
			return null;
		}
	}

	void Update()
	{
		if (regenerate)
		{
            Init();            
			regenerate = false;
		}
	}

    public List<Cell> GetNeighbours(Cell a)
    {
        List<Cell> result = new List<Cell>();
        IntVector2 cellPos = a.localPos;

        //DEFINES 4 WAY MOVEMENT ONLY
        for (int x = -1 + cellPos.x; x <= 1 + cellPos.x; x++)
        {
            for (int y = -1 + cellPos.y; y <= 1 + cellPos.y; y++)
            {
                if (x == cellPos.x && y == cellPos.y)
                    continue;

                //Limit to 4 way movement
                if (x == cellPos.x && y != cellPos.y || y == cellPos.y && x != cellPos.x)
                {
                    if (x >= 0 && x < gridSizeX && y >= 0 && y < gridSizeY)
                    {
                        if (grid[x, y].walkable)
                        {
                            result.Add(grid[x, y]);
                        }
                    }

                }

            }
        }

        return result;
    }

    public void MakeReservation(Vector3 pos)
    {
        if(pos.z>=0 && !reservationTable.Contains(pos))
        {
            reservationTable.Add(pos);
        }
    }

    public void ClearReservations()
    {
        reservationTable.Clear();
    }

    public Cell GetRandomCell()
    {
        Cell result = null;

        while(result==null || !result.walkable)
        {
            result = grid[Random.Range(0, gridSizeX), Random.Range(0, gridSizeY)];
        }

        return result;
    }
		
}
