﻿using UnityEngine;
using System.Collections;
using System.Threading;
using System.Collections.Generic;

public class ArtificialIntelligence
{
    public int DFS_DEPTH_LEVEL = 4;

    private Personality _personality;
    private Thread _thread = null;

    private bool _calculating;
    private List<Item> _items;

    public bool IsDone { get { return !_calculating; } }

    private int _result;
    private float _resultValue;
    private bool _askForItem;
    
    public void AskForItem(Personality personality, List<Item> items)
    {
        _calculating = true;
        _personality = personality;
        _items = items;
        _askForItem = true;

        _thread = new Thread(getBestItem);
        _thread.Start();
    }

    public void GetNextActivity(Personality personality)
    {
        _calculating = true;
        _personality = personality;
        _askForItem = false;

        _thread = new Thread(getBestAction);
        _thread.Start();
    }

    public int GetResult()
    {
        _thread.Abort();
        return _result;
    }

    public float GetValue()
    {
        return _resultValue;
    }

    private void getBestItem()
    {
        PersonalityNode root = new PersonalityNode(_personality, _items);

        dfs(root, 2);

        if (root.Children.Count != 0)
        {
            _result = root.Children[0].ParentActionID;
            _resultValue = root.Children[0].Evaluation() - root.Evaluation();
        }
        else
        {
            Debug.LogError("No Child generated. why?");
            _result = -1;
        }

        _calculating = false;
    }

	private void getBestAction(){
        
		PersonalityNode root = new PersonalityNode(_personality);
        
        dfs (root, DFS_DEPTH_LEVEL);

        if (root.Children.Count != 0)
        {
            _result = root.Children[0].ParentActionID;

            _resultValue = root.Children[0].Evaluation() - root.Evaluation();
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

                Activity activity = _personality.GetActivity(pn.ActivityIDs[i], false);

                if(activity == null && _askForItem)
                {
                    foreach(Item boxItem in _items)
                    {
                        if(activity == null)
                        {
                            activity = boxItem.GetActivity(pn.ActivityIDs[i]);
                        }
                    }
                }

				Item item = null;
				if (activity.item != null) {
					item = pn.GetItem (pn.ActivityIDs [i], false);
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