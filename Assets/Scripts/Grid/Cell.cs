using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;

[System.Serializable]
public class Cell{

	public Vector3 worldPosition;
	public bool walkable;
	public IntVector2 localPos;
	public string tag;

	public Cell(bool valid, Vector3 worldPos)
	{
		walkable = valid;
		worldPosition = worldPos;
	}

	public override bool Equals (object other)
	{
		if (other == null || GetType() != other.GetType()) 
			return false;

		Cell o = (Cell)other;
		return o.worldPosition == this.worldPosition && o.walkable == this.walkable;
	}

    public override int GetHashCode()
    {
        return worldPosition.GetHashCode();
    }
}
