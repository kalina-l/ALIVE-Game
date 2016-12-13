using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArtificialIntelligence
{
    private Personality _personality;

    public ArtificialIntelligence(Personality personality)
    {
        _personality = personality;
    }

    public int GetNextActivity()
    {
        return getBestAction();
    }

	public void decideOnAction()
	{
        //TODO: Start seperate thread for this
		getBestAction ();
    }

	private int getBestAction(){
		PersonalityNode root = new PersonalityNode(_personality);

        float timePassed = Time.realtimeSinceStartup;
        dfs (root, ApplicationManager.DFS_DEPTH_LEVEL);
        Debug.Log("Deciding on action with depth of " + ApplicationManager.DFS_DEPTH_LEVEL + " took " + (Time.realtimeSinceStartup-timePassed) + " second to calculate.");

        if (root.Children.Count != 0)
        {
            return root.Children[0].ParentActionID;
        }
        else
        {
            Debug.LogError("No Child generated. why?");
            return -1;
        }
	}

	private void dfs(PersonalityNode pn, int maxDepth){

		if (pn.Depth < maxDepth && !pn.visited) {
			for (int i = 0; i < pn.ActivityIDs.Count; i++) {
				PersonalityNode newPerson = new PersonalityNode (pn, _personality.GetActivity (pn.ActivityIDs [i]).GetExperience (pn), pn.ActivityIDs [i], _personality.GetActivity(pn.ActivityIDs[i]).Feedback.GetFeedback(pn.Needs));
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
