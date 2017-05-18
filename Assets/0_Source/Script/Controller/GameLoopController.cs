using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KKSpeech;

public class GameLoopController {

    private ApplicationManager _manager;
    private GameData _data;

    private int saveCounter;

    private Experience _lastExperience;
    private Activity _lastActivity;

    public bool waitForFeedback;

    public GameLoopController(ApplicationManager manager, GameData data) {
        _manager = manager;
        _data = data;

        saveCounter = 1;

        _manager.StartCoroutine(Run());
    }

    public void GiveFeedback(int feedback)
    {
        if (waitForFeedback)
        {
            if (_lastActivity != null)
            {
                //Store Feedback in Activity
                _lastActivity.Feedback.AddFeedback(_lastExperience.BaseNeeds, feedback);

                //Show Feedback
                _manager.ShowFeedback(feedback);
            }

            waitForFeedback = false;
        }
    }

    private IEnumerator Run()
    {
        while (true)
        {
            //yield return new WaitForSeconds(WaitTime);

            yield return _manager.StartCoroutine(DoActivityRoutine());

            float timer = 0;
            while (timer < _manager.WaitTime || _manager.getFeedbackController().IsRecording())
            {
                timer += Time.deltaTime;
                yield return 0;
            }

            if (waitForFeedback)
            {
                GiveFeedback(0);
            }

            if (saveCounter >= _manager.AutomaticSaveAfterActions)
            {
                _data.SaveData();
                saveCounter = 1;
            }
            else
            {
                saveCounter++;
            }
        }
    }

    private IEnumerator DoActivityRoutine()
    {
        _data.Intelligence.GetNextActivity(_data.Person);

        float timer = 0;

        while (!_data.Intelligence.IsDone)
        {
            timer += Time.deltaTime;
            yield return 0;
        }

        int activityID = _data.Intelligence.GetResult();
        int askActivityID = -1;

        Debug.Log("Calculation took " + timer + " seconds " + _data.Intelligence.GetValue());

        bool askForItem = _data.Intelligence.GetValue() <= 0;

        if (askForItem)
        {
            //Debug.Log("Ask for Item!!!");
            _data.Intelligence.AskForItem(_data.Person, _data.Items);

            timer = 0;

            while (!_data.Intelligence.IsDone)
            {
                timer += Time.deltaTime;
                yield return 0;
            }

            askActivityID = _data.Intelligence.GetResult();
        }


        if (activityID != -1)
        {
            if (askForItem)
            {
                Item askItem = null;

                for (int i = 0; i < _data.Items.Count; i++)
                {
                    foreach (Activity activity in _data.Items[i].GetAllActivities())
                    {
                        if (activity.ID == askActivityID)
                        {
                            askItem = _data.Items[i];
                        }
                    }
                }

                if (askItem != null)
                {
                    if (_data.Person.GetItem(askItem.ID, false) == null)
                    {
                        _manager.ShowItemAlert(askItem);

                        yield return new WaitForSeconds(2);
                    }
                }
                else
                {
                    Debug.Log("NO EXTRA ITEM NEEDED - I want to " + _data.Person.GetActivity(activityID).feedBackString);
                }
                
            }

            _lastActivity = _data.Person.GetActivity(activityID);
            _lastExperience = _lastActivity.DoActivity(_data.Person);

            //Show Activity
            _manager.ShowMessage(_lastActivity.feedBackString);

            //Ask for Feedback
            waitForFeedback = true;

            _manager.CharacterAnimation.PlayActivityAnimation(_lastActivity, _data.Person);

            while (_manager.CharacterAnimation.IsAnimating)
            {
                yield return 0;
            }

            _manager.UpdateUI();

        }
        else
        {
            _lastActivity = null;
        }
    }


}
