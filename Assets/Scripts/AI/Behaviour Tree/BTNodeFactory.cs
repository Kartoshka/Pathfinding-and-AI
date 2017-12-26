using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BTNodeFactory {

	public static BTNode BuildNode(BTNodeType type)
    {
        switch (type)
        {
            case BTNodeType.ExecutePath:
                return new BTN_ExecutePath();
            case BTNodeType.FindObjectRNG:
                return new BTN_FindObjectRNG();
			case BTNodeType.FindObjectClose:
				return new BTN_FindObjectClose ();
            case BTNodeType.GetPath:
                return new BTN_GeneratePath();
            case BTNodeType.IntGTZ:
                return new BTN_IntGTZero();
            case BTNodeType.Selector:
                return new BTN_Selector();
            case BTNodeType.Sequence:
                return new BTN_Sequence();
            case BTNodeType.Succeeder:
                return new BTN_Succeeder();
            case BTNodeType.Inverter:
                return new BTN_Inverter();
            case BTNodeType.UntilSuccess:
                return new BTN_UntilSucceed();
        }
        return new BTN_Succeeder();
    }

    public static BTNode ConstructTree(SBTNode t, AStarAgent AI)
    {
        List<BTNode> children = new List<BTNode>();

        BTNode res = BTNodeFactory.BuildNode(t.type);
        foreach (SBTNode child in t.children)
        {
            BTNode c = ConstructTree(child, AI);
            children.Add(c);
        }

        res.variables = t.variables;
        res.childrenNodes = children;
        res.AI = AI;

        return res;
    }
}
