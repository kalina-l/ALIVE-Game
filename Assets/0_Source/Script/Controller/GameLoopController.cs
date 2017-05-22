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
        Debug.Log("Start of Loop");

        bool removeExtraActivity = false;

        if (_manager.Multiplayer.IsRequestpending())
        {
            Debug.Log("Check multiplayer Request");

            _data.Person.AddBaseActivity(_manager.Multiplayer.GetPendingActivity());
            removeExtraActivity = true;
        }

        Debug.Log("Calculate Next Activity");

        _data.Intelligence.GetNextActivity(_data.Person, _manager.Multiplayer.IsConnected);

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
            Debug.Log("Ask for Item");

            _data.Intelligence.AskForItem(_data.Person, _data.Items);

            timer = 0;

            while (!_data.Intelligence.IsDone)
            {
                timer += Time.deltaTime;
                yield return 0;
            }

            askActivityID = _data.Intelligence.GetResult();

            Debug.Log("Item Found");
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

            Debug.Log("Get Activity");

            _lastActivity = _data.Person.GetActivity(activityID);

            Debug.Log("Do Multiplayer");

            if (_lastActivity.IsMultiplayer) {
                if (_lastActivity.IsRequest) {
                    _manager.Multiplayer.AcceptRequest();
                }
                else {
                    if (_manager.Multiplayer.IsRequestpending()) {
                        _manager.Multiplayer.DeclineRequest();
                    }

                    _manager.Multiplayer.SendActivityRequest(_lastActivity);

                    while (_manager.Multiplayer.IsWaitingForAnswer()) {
                        yield return 0;
                    }
                }
            }
            else if(_manager.Multiplayer.IsRequestpending()) {
                _manager.Multiplayer.DeclineRequest();
            }

            Debug.Log("Do Activity");

            _lastExperience = _lastActivity.DoActivity(_data.Person);

            //Show Activity
            _manager.ShowMessage(_lastActivity.feedBackString);

            //Ask for Feedback
            waitForFeedback = true;

            _manager.CharacterAnimation.PlayActivityAnimation(_lastActivity, _data.Person);

            Debug.Log("Animate");

            while (_manager.CharacterAnimation.IsAnimating)
            {
                yield return 0;
            }

            Debug.Log("UpdateUI");

            _manager.UpdateUI();

            //remove multiplayer activity
            if (removeExtraActivity) {
                _data.Person.RemovePendingActivity();
            }
        }
        else
        {
            _lastActivity = null;
        }

        Debug.Log("End of Loop");
    }


}
