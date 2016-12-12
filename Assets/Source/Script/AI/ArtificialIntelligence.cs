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
        return createPartialLearningTree();
    }

	public void decideOnAction()
	{
        //TODO: Start seperate thread for this
		createPartialLearningTree ();
    }

	private int createPartialLearningTree(){
		int maxDepth = 5;
		PersonalityNode root = new PersonalityNode(_personality);

		dfs (root, maxDepth);

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
