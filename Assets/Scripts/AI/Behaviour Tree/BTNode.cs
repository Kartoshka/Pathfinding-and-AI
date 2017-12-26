using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BTNodeType
{
    Sequence, 
    Selector,
    Inverter,
    UntilSuccess,
    Succeeder,
    FindObjectRNG,
	FindObjectClose,
    IntGTZ,
    GetPath,
    ExecutePath
}

public enum BTN_State
{
    Success,
    Process,
    Failure
}
public struct SBTNode
{
    public BTNodeType type;
    public SBTNode[] children;
    public string[] variables;

    public SBTNode(BTNodeType t, string[] v, SBTNode[] c)
    {
        type = t;
        children = c;
        variables = v;
    }
}

public class DataContext
{
    public Dictionary<string, object> dictionary = new Dictionary<string, object>();
    
}

public abstract class BTNode {

    public AStarAgent AI;
    public string[] variables;
    protected int activeChild = 0;
    public List<BTNode> childrenNodes = new List<BTNode>();
    public abstract BTN_State Tick();
}

public class BTN_Succeeder : BTNode
{
    //Runs all its children, when they're all done running, regardless of result, return success, if one is in process, wait until next tick
    override public BTN_State Tick()
    {
        while (activeChild < childrenNodes.Count)
        {
            BTN_State childState = childrenNodes[activeChild].Tick();
            if(childState==BTN_State.Process)
            {
                return BTN_State.Process;
            }
        }
        return BTN_State.Success;
    }
}

public class BTN_Inverter : BTNode
{
    //Runs all its children, when they're all done running, regardless of result, return success, if one is in process, wait until next tick
    override public BTN_State Tick()
    {
        while (activeChild < childrenNodes.Count)
        {
            BTN_State childState = childrenNodes[activeChild].Tick();
            if (childState == BTN_State.Process)
            {
                return BTN_State.Process;
            }
            else
            {
                activeChild = 0;
                return childState == BTN_State.Failure ? BTN_State.Success : BTN_State.Failure;
            }
        }
        activeChild = 0;
        return BTN_State.Success;
    }
}

public class BTN_Selector : BTNode
{
    override public BTN_State Tick()
    {
        //Keep running children until one succeeds, if one is in process, we wait for the next Tick
        while (activeChild<childrenNodes.Count)
        {
            BTN_State childState = childrenNodes[activeChild].Tick();
            switch (childState)
            {
                case BTN_State.Success:
                    return BTN_State.Success;
                case BTN_State.Process:
                    return BTN_State.Process;
                case BTN_State.Failure:
                    activeChild++;
                    break;
            }
        }
        activeChild = 0;
        return BTN_State.Failure;
    }
}

public class BTN_Sequence : BTNode
{
    override public BTN_State Tick()
    {
        //Keep running children until all succeed, if one is in process, we wait for the next Tick
        while (activeChild < childrenNodes.Count)
        {
            BTN_State childState = childrenNodes[activeChild].Tick();
            switch (childState)
            {
                case BTN_State.Failure:
                    return BTN_State.Failure;
                case BTN_State.Process:
                    return BTN_State.Process;
                case BTN_State.Success:
                    activeChild++;
                    break;
            }
        }

        activeChild = 0;
        return BTN_State.Success;
    }
}

public class BTN_UntilSucceed : BTNode
{
    override public BTN_State Tick()
    {
        //Keep running children until all succeed, if one is in process, we wait for the next Tick
        while (activeChild < childrenNodes.Count)
        {
            BTN_State childState = childrenNodes[activeChild].Tick();
            switch (childState)
            {
                case BTN_State.Process:
                    return BTN_State.Process;
                case BTN_State.Failure:
                    activeChild = 0;
                    return BTN_State.Process;
                case BTN_State.Success:
                    activeChild++;
                    break;
            }
        }

        activeChild = 0;
        return BTN_State.Success;
    }
}

public class BTN_FindObjectRNG : BTNode
{
    override public BTN_State Tick()
    {
        if(this.AI == null || this.variables.Length < 2)
        {
            return BTN_State.Failure;
        }
        else
        {
			GameObject[] objects = GameObject.FindGameObjectsWithTag(this.variables[0]);
            if(objects.Length == 0)
            {
                return BTN_State.Failure;
            }

            Cell destination = this.AI.grid.GetCell(objects[Random.Range(0,objects.Length)].transform.position);
            AI.agentData.dictionary.Remove(this.variables[1]);
            AI.agentData.dictionary.Add(this.variables[1], destination);
            return BTN_State.Success;
        }
    }
}

public class BTN_FindObjectClose : BTNode
{
	override public BTN_State Tick()
	{
		if(this.AI == null || this.variables.Length < 2)
		{
			return BTN_State.Failure;
		}
		else
		{
			GameObject[] objects = GameObject.FindGameObjectsWithTag(this.variables[0]);
			if(objects.Length == 0)
			{
				return BTN_State.Failure;
			}
			int picked = 0;
			float closestDistance = Vector3.Distance (objects [0].transform.position, this.AI.transform.position);
			for (int i = 0; i < objects.Length; i++)
			{
				float newDist = Vector3.Distance (objects [i].transform.position, this.AI.transform.position);
				if (newDist < closestDistance)
				{
					picked = i;
					closestDistance = newDist;
				}
			}
			Cell destination = this.AI.grid.GetCell(objects[picked].transform.position);

			AI.agentData.dictionary.Remove(this.variables[1]);
			AI.agentData.dictionary.Add(this.variables[1], destination);
			return BTN_State.Success;
		}
	}
}

public class BTN_IntGTZero : BTNode
{
    override public BTN_State Tick()
    {
        if (this.AI == null || this.variables.Length == 0)
        {
            return BTN_State.Success;
        }
        else
        {
            object keyCount;
            if (!AI.agentData.dictionary.TryGetValue(this.variables[0], out keyCount))
            {
                return BTN_State.Failure;
            }
            if (!(keyCount is int))
            {
                return BTN_State.Failure;
            }

            return ((int)keyCount > 0) ? BTN_State.Success : BTN_State.Failure ;
        }
    }
}

public class BTN_GeneratePath : BTNode
{
    override public BTN_State Tick()
    {
        if (this.AI == null || this.variables.Length == 0)
        {
            return BTN_State.Failure;
        }
        else
        {
            object destination;
            if(!AI.agentData.dictionary.TryGetValue(this.variables[0], out destination))
            {
                return BTN_State.Failure;
            }
            if(!(destination is Cell))
            {
                return BTN_State.Failure;
            }
            //Tell the Astar to get a new path
            this.AI.GetPath(this.AI.grid.GetCell(this.AI.transform.position), (Cell)destination);
            return BTN_State.Success;
        }
    }
}

public class BTN_ExecutePath : BTNode
{
    override public BTN_State Tick()
    {
        if (this.AI == null)
        {
            return BTN_State.Failure;
        }
        else
        {
           Cell current = this.AI.grid.GetCell(this.AI.transform.position);
           Cell destination = this.AI.GetDestination();
            if(current.localPos.Equals(destination.localPos))
            {
                this.AI.Stop();
                return BTN_State.Success;
            }
            else
            {
                this.AI.FollowTrajectory();
                return BTN_State.Process;
            }
        }
    }
}