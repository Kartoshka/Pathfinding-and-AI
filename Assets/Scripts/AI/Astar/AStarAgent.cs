using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ACell
{
    public Cell main;
    public ACell parent;

    public float f
    {
        get { return g + h; }
    }
    //Distance from start node
    public float g;
    //Heuristic distance from end node
    public float h;

    public ACell(Cell a)
    {
        main = a;
        parent = null;
    }

    public ACell(Cell a, ACell p)
    {
        main = a;
        parent = p;
    }

    public bool Equals(Cell c)
    {
        return main.Equals(c);
    }

    public override int GetHashCode()
    {
        return main.worldPosition.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        if (obj.GetType().Equals(typeof(Cell)))
        {
            Cell a = (Cell)obj;
            return Equals(a);
        }
        else if (obj.GetType().Equals(typeof(ACell)))
        {
            ACell o = (ACell)obj;
            return this.main.worldPosition == o.main.worldPosition;
        }
        else
        {
            return false;
        }
    }

}

public class AStarAgent : MonoBehaviour {

    public DataContext agentData = new DataContext();

    //X:localX Y:LocalY, Z:time
    HashSet<Vector3> closedSet = new HashSet<Vector3>();
    List<ACell> open = new List<ACell> ();

	public WorldGrid grid;
    public float speed;

	private List<Cell> trajectory = new List<Cell>();
    private List<Cell> moveList = new List<Cell>();

    private Cell currentDestination;
    private Cell currentOrigin;

    public Color debugColor;

    private bool moving = false;

    public void Update()
    {
        if(moving && moveList.Count>0)
        {
            if(grid.GetCell(this.transform.position).Equals(moveList[0]))
            {
                moveList.RemoveAt(0);
            }
            else
            {
                this.GetComponent<Rigidbody>().velocity = (moveList[0].worldPosition - this.transform.position).normalized * speed;
            }
        }

    }

    public void GetPath(Cell start, Cell end)
    {
        //Closed and open sets
        closedSet.Clear();
        open.Clear();

        //Holds result
        trajectory.Clear();
        currentDestination = end;
        ACell first = new ACell(start);

        open.Add(first);
        List<Cell> result = new List<Cell>();

        while (open.Count > 0)
        {
            //Get first node by sorting by smallest f cost and picking that
            ACell current = open[0];
            for (int i = 1; i < open.Count; i++)
            {
                if (open[i].f < current.f || (open[i].f == current.f && open[i].h < current.h))
                {
                    current = open[i];
                }
            }

            open.Remove(current);
            if (current.main == null || current == null)
                continue;


            closedSet.Add(current.main.worldPosition);

            if (current.main.worldPosition == end.worldPosition)
            {
                while (current != null)
                {
                    result.Insert(0, current.main);
                    current = current.parent;
                }
                trajectory = result;
				moveList = trajectory;
                return;
            }

            List<Cell> neighbours = grid.GetNeighbours(current.main);
            foreach (Cell n in neighbours)
            {
                ACell neighbour = new ACell(n);
                if (closedSet.Contains(n.worldPosition))
                {
                    continue;
                }

                float newDistFromStart = current.g + GetDistance(current.main, n);

                if (newDistFromStart < neighbour.g || !open.Contains(neighbour))
                {
                    open.Remove(neighbour);
                    neighbour.g = newDistFromStart;

                    neighbour.h = GetDistance(neighbour.main, end);
                    neighbour.parent = current;

                    //Remove if it exists so we can store the new updated value
                    open.Add(neighbour);
                }
            }

        }
    }

    public float GetDistance(Cell s, Cell e){
		Vector3 sPos = s.worldPosition;
		sPos.y = 0;
		Vector3 ePos = e.worldPosition;
		ePos.y = 0;

        return Vector3.Distance (ePos,sPos);
	}

    public bool HasTrajectory()
    {
        return this.trajectory.Count > 0;
    }

	public float DistancePathLeft()
	{
		return moveList.Count * grid.cellSize;
	}

	public float TimePathLeft()
	{
		if (this.speed == 0)
		{
			return -1.0f;
		}
		return DistancePathLeft () / this.speed;
	}

    public Cell GetCurrentCell()
    {
        return grid.GetCell(this.transform.position);
    }

    public Cell GetDestination()
    {
        return trajectory.Count > 0 ? trajectory[trajectory.Count-1] : null;
    }

    public void MoveTo(Cell d)
    {
        this.GetComponent<Rigidbody>().velocity = (d.worldPosition - this.transform.position).normalized * speed;
    }

    public void Stop()
    {
        moving = false;
        this.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    public void FollowTrajectory()
    {
        moving = true;
    }

    void OnDrawGizmos() {
        if(grid == null)
        {
            return;
        }

        Gizmos.color = debugColor;

        foreach (Cell n in trajectory) {
			Gizmos.DrawCube(n.worldPosition + Vector3.up*(0.05f+grid.cellSize), Vector3.one * (grid.cellSize));
        }
        Gizmos.color = Color.black;
        //Debug to see current destination
        if (currentDestination!=null)
            Gizmos.DrawCube(currentDestination.worldPosition + Vector3.up * (0.05f), Vector3.one * (grid.cellSize));
    }

}
