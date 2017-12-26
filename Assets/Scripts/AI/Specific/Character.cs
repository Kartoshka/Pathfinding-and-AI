using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : AStarAgent {

    //Root of our behaviour tree
    BTNode root;

    //Description of our behaviour tree
    SBTNode topNode = new SBTNode(BTNodeType.Sequence, new string[] { }, new SBTNode[]{
            new SBTNode(BTNodeType.UntilSuccess, new string[] { }, new SBTNode[]{
                new SBTNode(BTNodeType.Inverter, new string[] { }, new SBTNode[] {
                    new SBTNode(BTNodeType.Sequence, new string[] { }, new SBTNode[]{
                        new SBTNode(BTNodeType.IntGTZ,new string[] {"KeyCount"}, new SBTNode[] {}),
                        new SBTNode(BTNodeType.Sequence,new string[] {}, new SBTNode[] {
						new SBTNode(BTNodeType.FindObjectClose,new string[] {"Key","KeyLocation"}, new SBTNode[] {}),
                            new SBTNode(BTNodeType.GetPath,new string[] {"KeyLocation"}, new SBTNode[] {}),
                            new SBTNode(BTNodeType.ExecutePath,new string[]  {}, new SBTNode[] {}),
                        })
                    })  
                })
            }),
             new SBTNode(BTNodeType.Sequence,new string[] { }, new SBTNode[] {
                    new SBTNode(BTNodeType.FindObjectRNG,new string[] {"Exit","ExitLocation"}, new SBTNode[] {}),
                    new SBTNode(BTNodeType.GetPath, new string[] {"ExitLocation"}, new SBTNode[] {}),
                    new SBTNode(BTNodeType.ExecutePath, new string[]   {}, new SBTNode[] {})
            })
        });

    public void Init()
    {
        root = BTNodeFactory.ConstructTree(topNode, this);
        GameObject[] keys = GameObject.FindGameObjectsWithTag("Key");
        foreach(GameObject g in keys)
        {
            g.GetComponent<Collider>().enabled = true;
        }
        this.agentData.dictionary.Add("KeyCount", keys.Length);
        this.GetComponent<Collider>().enabled = true;
    }

    public new void Update()
    {
        if(root !=null)
        {
            if(root.Tick() == BTN_State.Success)
            {
                root = null;
            }
            
        }
        base.Update();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Key")
        {
            Destroy(other.gameObject);
        }
    }


}
