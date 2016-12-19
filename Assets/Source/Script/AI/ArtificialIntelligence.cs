using UnityEngine;
using System.Collections;
using System.Threading;
using System.Collections.Generic;

public class ArtificialIntelligence
{
    public int DFS_DEPTH_LEVEL = 6;

    private Personality _personality;
    private Thread _thread = null;

    private bool _calculating;

    public bool IsDone { get { return !_calculating; } }

    private int _result;
    

    public void GetNextActivity(Personality personality)
    {
        _calculating = true;
        _personality = personality;

        _thread = new Thread(getBestAction);
        _thread.Start();
    }

    public int GetResult()
    {
        _thread.Abort();
        return _result;
    }

	private void getBestAction(){
        
		PersonalityNode root = new PersonalityNode(_personality);
        
        dfs (root, DFS_DEPTH_LEVEL);

        if (root.Children.Count != 0)
        {
            _result = root.Children[0].ParentActionID;
        }
        else
        {
            Debug.LogError("No Child generated. why?");
            _result = -1;
        }

        _calculating = false;
	}

	private void dfs(PersonalityNode pn, int maxDepth){

		if (pn.Depth < maxDepth && !pn.visited) {
			for (int i = 0; i < pn.ActivityIDs.Count; i++) {

                Activity activity = _personality.GetActivity(pn.ActivityIDs[i]);
				Item item = null;
				if (activity.item != null) {
					item = pn.GetItem (pn.ActivityIDs [i]);
				}

                Experience experience = activity.GetExperience(pn);
                float feedback = activity.Feedback.GetFeedback(pn.Needs);

                PersonalityNode newPerson = new PersonalityNode (pn,
                                                experience, 
                                                pn.ActivityIDs [i],
												feedback,
												item,
												activity.useConsume);
                
                pn.Children.Add (newPerson);
                dfs (newPerson, maxDepth);
			}
			pn.visited = true;
		}
        

        if ((pn.Depth >= maxDepth || pn.visited) && pn.Parent!=null) {
            if (pn.Parent.BestChildsEvaluation < (pn.SelfEvaluation + pn.BestChildsEvaluation)) {
                pn.Parent.removeAllChildrenExceptOne (pn);
				pn.Parent.BestChildsEvaluation = (pn.SelfEvaluation + pn.BestChildsEvaluation);
			} else {
                pn.Parent.removeChildReference (pn);
			}
		}
    }
}
