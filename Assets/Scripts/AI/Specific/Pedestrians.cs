using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionType
{
    Horizontal,
    Wait,
    Vertical,
    Rotate,
    InPlaceTBD
}
public struct GrammarWord
{
    public ActionType action;
    public float amount;
    public float duration; 


    public GrammarWord(ActionType a, float am, float d)
    {
        action = a;
        amount = am;
        duration = d;
    }
}

public class GrammarFactory
{
    public static List<GrammarWord> Parse(GrammarWord source, int depth)
    {
        List<GrammarWord> res = new List<GrammarWord>();

        if (depth<=0 && source.action != ActionType.InPlaceTBD)
        {
            res.Add(source);
        }
        else
        {
            List<GrammarWord> sentence = ApplyGrammar(source);
			int chosen = Random.Range (0, sentence.Count);
//			for (int i = 0; i < sentence.Count; i++)
//			{
//				if (i == chosen)
//				{
//					res.AddRange (Parse (sentence [i], depth - 1));
//				} else
//				{
//					res.Add (sentence [i]);
//				}
//			}
            foreach (GrammarWord newWord in sentence)
            {
                res.AddRange(Parse(newWord,depth-1));
            }
        }

        return res;
    }

    public static List<GrammarWord> ApplyGrammar(GrammarWord source)
    {
        List<GrammarWord> result = new List<GrammarWord>();
        switch(source.action)
        {
            case ActionType.InPlaceTBD:
                {
                    int choice = Random.Range(0, 3);
                    if (choice == 0)
                    {
                        //Pause
                        result.Add(new GrammarWord(ActionType.Wait, 0, source.duration));
                    }
                    else if (choice == 1)
                    {
                        //360 spin
                        result.Add(new GrammarWord(ActionType.Rotate, 360.0f, source.duration));

                    }
                    else if (choice == 2)
                    {
                        //Shift vertical
                        result.Add(new GrammarWord(ActionType.Vertical, Random.Range(-8,8), source.duration));
                    }
                }
                    break;
            case ActionType.Horizontal:
                {
                    int choice = Random.Range(0, 4);
                    //Forward, turn, back, turn, forward
                    if (choice == 0)
                    {
                        //Caclulate proportion of time and distance for each forward segment
						float t1 = 0.3f;//Random.Range(0.2f, 0.6f);
						float t3 = 0.2f;//Random.Range(0.15f, 1.0f - t1 - 0.2f);
                        float t5 = 1.0f - t1 - t3;

                        //Caclulate proportion of time for Each rotation
                        float t2 = Random.Range(0.2f, 0.7f);
                        float t4 = 1.0f - t2;
						
                        //Forward segment t1
                        result.Add(new GrammarWord(ActionType.Horizontal, source.amount * t1, source.duration * 0.7f * t1));
                        //Spin segment t2
                        result.Add(new GrammarWord(ActionType.Rotate, 180.0f, source.duration * 0.3f * t2));
                        //Backward segment t3
                        result.Add(new GrammarWord(ActionType.Horizontal, (-1.0f)*source.amount * t3, source.duration * 0.7f * t3));
                        //Spin segment t4
                        result.Add(new GrammarWord(ActionType.Rotate, -180.0f, source.duration * 0.3f * t4));
                        //Forward segment t5
                        result.Add(new GrammarWord(ActionType.Horizontal, source.amount * (1.0f-t1+t3), source.duration * 0.7f * t5));

                    }
					else if (choice == 1 || choice == 3)
                    {
                        //forward t1, tbd t2, forward t3
                        float t1 = Random.Range(0.2f, 0.6f);
                        float t2 = 1.0f;
                        float t3 = 1.0f - t1;

                        //Forward segment t1
                        result.Add(new GrammarWord(ActionType.Horizontal, source.amount * t1, source.duration * 0.75f * t1));
                        //TBD segment t2
						result.AddRange(GrammarFactory.ApplyGrammar(new GrammarWord(ActionType.InPlaceTBD, -1.0f, source.duration * t2 * 0.25f)));
                        //result.Add(new GrammarWord(ActionType.InPlaceTBD, -1.0f, source.duration * t2 * 0.5f));                   
                        //Forward segment t3
                        result.Add(new GrammarWord(ActionType.Horizontal, source.amount * t3, source.duration * 0.75f * t3));

                    }
                    else if(choice == 2)
                    {
                        //forward, tbd, back, tbd, forward

                        //Caclulate proportion of time and distance for each forward segment
						float t1 = 0.3f;//Random.Range(0.2f, 0.6f);
						float t3 = 0.2f;//Random.Range(0.15f, 1.0f - t1 - 0.2f);
                        float t5 = 1.0f - t1 - t3;

                        //Caclulate proportion of time for Each rotation
                        float t2 = Random.Range(0.2f, 0.7f);
                        float t4 = 1.0f - t2;

                        //Forward segment t1
                        result.Add(new GrammarWord(ActionType.Horizontal, source.amount * t1, source.duration * 0.7f * t1));
                        //TBD segment t2
						result.AddRange (GrammarFactory.ApplyGrammar (new GrammarWord (ActionType.InPlaceTBD, -1, source.duration * 0.3f * t2)));
                        //result.Add(new GrammarWord(ActionType.InPlaceTBD, -1, source.duration * 0.3f * t2));
                        //Backward segment t3
                        result.Add(new GrammarWord(ActionType.Horizontal, (-1.0f) * source.amount * t3, source.duration * 0.7f * t3));
                        //TBD segment t4
						result.AddRange (GrammarFactory.ApplyGrammar (new GrammarWord(ActionType.InPlaceTBD, -1 , source.duration * 0.3f * t4)));
                        //result.Add(new GrammarWord(ActionType.InPlaceTBD, -1 , source.duration * 0.3f * t4));
                        //Forward segment t5
                        result.Add(new GrammarWord(ActionType.Horizontal, source.amount * (1.0f - t1 + t3), source.duration * 0.7f * t5));
                    }
                }
                break;
            default:
                result.Add(source);
                break;
        }
        return result;
    }
}
public class Pedestrians : AStarAgent {

    List<GrammarWord> finalGrammar;
    float currDuration = 0.0f;
	public int recursionDepth = 2;
	// Use this for initialization
	public void Activate () {
		finalGrammar = GrammarFactory.Parse(new GrammarWord(ActionType.Horizontal, 45, 15.0f), recursionDepth);
        float finalDistance = 0;
        float finalTime = 0;

        foreach (GrammarWord g in finalGrammar)
        {
            if(g.action == ActionType.Horizontal)
            {
                finalDistance += g.amount;
            }
            finalTime += g.duration;
        }

        NextStep();
	}
	
    private void NextStep()
    {
        if(finalGrammar.Count == 0)
        {
			Destroy (this.gameObject);
            return;
        }

        GrammarWord step = finalGrammar[0];
        finalGrammar.RemoveAt(0);
        
        switch(step.action)
        {
            case ActionType.Horizontal:
                StartCoroutine(Horizontal(step.amount, step.duration));
                break;
            case ActionType.Vertical:
                StartCoroutine(Vertical(step.amount, step.duration));
                break;
            case ActionType.Wait:
                StartCoroutine(Wait(step.duration));
                break;
            case ActionType.Rotate:
                StartCoroutine(Rotate(step.amount, step.duration));
                break;
        }

    }

    IEnumerator Rotate(float amount, float duration)
    {
        this.GetComponent<Rigidbody>().angularVelocity = new Vector3(0,Mathf.Deg2Rad * amount / duration,0);
        yield return new WaitForSeconds(duration);
        this.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        NextStep();
    }

    IEnumerator Horizontal(float amount, float duration)
    {	
		float initialAmount = amount;
		Cell start = this.grid.GetCell (this.transform.position);
		Cell end = this.grid.GetCell(this.transform.position + (-1.0f * Vector3.back) * amount);
		while (end == null || !end.walkable || end.tag == "Room" )
		{
			amount -= 1.0f;
			end = this.grid.GetCell(this.transform.position + (-1.0f * Vector3.back) * amount);
			if (amount < (-1.0f * initialAmount))
			{
				end = start;
				break;
			}
		}
		this.GetPath (start, end);
		//this.speed = 1.0f;
		//this.speed = amount / duration;
		this.FollowTrajectory ();
        //this.GetComponent<Rigidbody>().velocity = (-1.0f * Vector3.back) * (amount / duration);
		yield return new WaitForSeconds(TimePathLeft());
//        this.GetComponent<Rigidbody>().velocity = Vector3.zero;
		this.Stop();
        NextStep();

    }

    IEnumerator Vertical(float amount, float duration)
    {
		float initialAmount = amount;
		Cell start = this.grid.GetCell (this.transform.position);
		Cell destination = this.grid.GetCell(this.transform.position +  (Vector3.right)*amount);

		while (!destination.walkable || destination.Equals(start) || destination.tag == "Room" )
		{
			amount -= 1.0f;
			destination = this.grid.GetCell(this.transform.position +  (Vector3.right)*amount);
			if (amount < (-1.0f * initialAmount))
			{
				destination = start;
				break;
			}
		}
		//this.speed = 1.0f;
		//this.speed = amount / duration;
		this.GetPath (start, destination);
		this.FollowTrajectory ();
//        this.GetComponent<Rigidbody>().velocity = (Vector3.right) * (amount / duration);
		yield return new WaitForSeconds(TimePathLeft());
//        this.GetComponent<Rigidbody>().velocity = Vector3.zero;
		this.Stop();

        NextStep();
    }

    IEnumerator Wait(float duration)
    {
        this.GetComponent<Rigidbody>().velocity = Vector3.zero;
        yield return new WaitForSeconds(duration);
        NextStep();

    }
}
