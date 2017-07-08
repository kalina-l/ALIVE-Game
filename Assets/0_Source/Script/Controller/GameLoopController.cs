using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KKSpeech;

public class GameLoopController : GameLoop {

    private ApplicationManager _manager;
    private GameData _data;

    private int saveCounter;

    private Experience _lastExperience;
    private Activity _lastActivity;

    public bool waitForFeedback;
    public static int ASK_FOR_ITEM_FACTOR = 0;
    private Item askItem;
    private bool askForItemShown = false;

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
                if (feedback != 0)
                {
                    //Store Feedback in Activity               
                    _lastActivity.Feedback.AddFeedback(_lastExperience.BaseNeeds, feedback);

                    if(_data.Person.executedEmotion == EmotionType.NORMAL)
                    {
                        if(feedback < 0)
                        {
                            _data.Person.emotionCounter -= 1;
                        }
                        else
                        {
                            _data.Person.emotionCounter += 1;
                        }
                    }
                } else
                {
                    _lastActivity.Feedback.AddNoFeedbackGiven(_lastExperience.BaseNeeds);
                }
                _lastActivity.DebugWholeFeedbackValue();
                //Show Feedback
                _manager.ShowFeedback(feedback);
            }

            waitForFeedback = false;
        }
    }

    private IEnumerator Run()
    {
        while (_data.Person.IsAlive)
        {
            // actualize the others lemos' knowledge about your needs
            if (_manager.Multiplayer.IsConnected)
            {
                _manager.Multiplayer.SendNeeds(new PersonalityNode(_data.Person).Needs);
            }

            yield return _manager.StartCoroutine(DoActivityRoutine());

            System.Random rand = new System.Random();
            if (_manager.Multiplayer.IsConnected)
            {
                //random feedback request (25%)
                bool receivingFeedback = rand.NextDouble() < 0.25 ? true : false;
                if (receivingFeedback)
                {
                    _manager.Multiplayer.SendFeedbackRequest(_lastActivity);
                }
            }

            bool sentFeedback = false;

            float timer = 0;
            while (timer < _manager.WaitTime || _manager.getFeedbackController().IsRecording())
            {
                timer += Time.deltaTime;

                if (!sentFeedback)
                {
                    if (_manager.Multiplayer.IsFeedbackRequestPending())
                    {

                        // SEND FEEDBACK TO ANOTHER LEMO
                        Activity feedbackActivity = _manager.Multiplayer.GetFeedbackActivity();
                        PersonalityNode personality = new PersonalityNode(_data.Person);
                        personality.changeNeeds(_manager.Multiplayer.RemoteNeeds);
                        Experience experience = feedbackActivity.GetExperience(personality);
                        float feedback = feedbackActivity.Feedback.GetFeedback(personality.Needs);
                        PersonalityNode newPerson = new PersonalityNode(personality,
                                                    experience,
                                                    feedback);
                        float evaluation = newPerson.SelfEvaluation;
                        //DebugController.Instance.Log("My evaluation: " + evaluation, DebugController.DebugType.Multiplayer);
                        if (evaluation > 0)
                        {
                            _manager.Multiplayer.SendFeedback(1);
                            DebugController.Instance.Log("Send feedback to the remote LEMO: 1", DebugController.DebugType.Multiplayer);
                        }
                        else
                        {
                            _manager.Multiplayer.SendFeedback(-1);
                            DebugController.Instance.Log("Send feedback to the remote LEMO: -1", DebugController.DebugType.Multiplayer);
                        }
                    }
                }

                yield return 0;
            }

            if (waitForFeedback)
            {
                GiveFeedback(0);
            }


            _data.Person.checkEmotion();

            if (saveCounter >= _manager.AutomaticSaveAfterActions)
            {
                _data.SaveData();
                saveCounter = 1;
            }
            else
            {
                saveCounter++;
            }

            yield return new WaitForSeconds(2);
        }

        //GAME OVER

        ApplicationManager.Instance.GameOver();
    }

    private IEnumerator DoActivityRoutine()
    {
        DebugController debug = DebugController.Instance;

        debug.Log("Start of Loop", DebugController.DebugType.GameFlow);
        

        if (_manager.Multiplayer.IsRequestPending())
        {
            debug.Log("Check multiplayer Request", DebugController.DebugType.GameFlow);
        }

        debug.Log("Calculate Next Activity", DebugController.DebugType.GameFlow);

        _data.Person.PrintAllRewards();


        //add point to happy emotion when Lemo gets Item it wants
        if(askItem != null)
        debug.Log("AskItem: " + askItem.Name, DebugController.DebugType.Emotion);
        if (_data.Person.executedEmotion == EmotionType.NORMAL)
        {
            if (askForItemShown && (askItem != null) && (_data.Person.GetItems().ContainsValue(askItem)))
            {
                _data.Person.emotionCounter += 1;
            }
            if(askForItemShown && (askItem != null) && (!_data.Person.GetItems().ContainsValue(askItem)))
            {
                _data.Person.emotionCounter -= 1;
            }
        }

        
        _data.Intelligence.GetNextActivity(_data.Person, _manager.Multiplayer.IsConnected, _manager.Multiplayer.GetPendingActivity());

        float timer = 0;

        while (!_data.Intelligence.IsDone)
        {
            timer += Time.deltaTime;
            yield return 0;
        }

        int activityID = _data.Intelligence.GetResult();
        int askActivityID = -1;

        debug.Log("Calculation took " + timer + " seconds " + _data.Intelligence.GetValue() + " " + activityID, DebugController.DebugType.Activity);

        askItem = null;
        askForItemShown = false;
        bool askForItem = _data.Intelligence.GetValue() <= ASK_FOR_ITEM_FACTOR;

        if (askForItem)
        {
            debug.Log("Ask for Item", DebugController.DebugType.GameFlow);

            _data.Intelligence.AskForItem(_data.Person, _data.Items);

            timer = 0;

            while (!_data.Intelligence.IsDone)
            {
                timer += Time.deltaTime;
                yield return 0;
            }

            askActivityID = _data.Intelligence.GetResult();

            debug.Log("Item Found", DebugController.DebugType.GameFlow);
        }

        if (activityID != -1)
        {
            if (askForItem)
            {
                askItem = null;

                for (int i = 0; i < _data.Items.Count; i++)
                {
                    debug.Log("Check " + _data.Items[i].Name, DebugController.DebugType.GameFlow);
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
                        askForItemShown = true;

                        yield return new WaitForSeconds(2);
                    }
                }
                else
                {
                    debug.Log("NO EXTRA ITEM NEEDED - I want to " + _data.Person.GetActivity(activityID).feedBackString, DebugController.DebugType.GameFlow);
                }
                
            }

            debug.Log("Get Activity", DebugController.DebugType.GameFlow);

            _lastActivity = _data.Person.GetActivity(activityID);
            
            if(_lastActivity == null)
            {
                for (int i = 0; i < _data.Items.Count; i++)
                {
                    foreach (Activity activity in _data.Items[i].GetAllActivities())
                    {
                        if (activity.ID == activityID)
                        {
                            _lastActivity = activity;
                        }
                    }
                }
            }

            debug.Log("Do Multiplayer", DebugController.DebugType.GameFlow);
            //DebugController.Instance.Log("Locals activity " + _lastActivity.Name + ", Object: " + _lastActivity.GetHashCode(), DebugController.DebugType.Multiplayer);

            if (_lastActivity.IsMultiplayer) {
                if (_lastActivity.IsRequest) {
                    _manager.Multiplayer.AcceptRequest();
                }
                else {
                    if (_manager.Multiplayer.IsRequestPending()) {
                        _manager.Multiplayer.DeclineRequest();
                    }

                    _manager.Multiplayer.SendActivityRequest(_lastActivity);

                    while (_manager.Multiplayer.IsWaitingForAnswer()) {
                        yield return 0;
                    }
                }
            }
            else if(_manager.Multiplayer.IsRequestPending()) {
                _manager.Multiplayer.DeclineRequest();
            }

            debug.Log("Do Activity", DebugController.DebugType.GameFlow);

            if (_lastActivity.IsDeclined)
            {
                _manager.ShowMessage("wanted to do: " + _lastActivity.feedBackString + ", but only got a rejection reward");
            }
            else
            {
                //Show Activity
                _manager.ShowMessage(_lastActivity.feedBackString);
            }

            _lastExperience = _lastActivity.DoActivity(_data.Person);
            
            _manager.Multiplayer.ClearActivity();

            //Ask for Feedback
            waitForFeedback = true;

            _manager.CharacterAnimation.PlayActivityAnimation(_lastActivity, _data.Person);

            debug.Log("Animate", DebugController.DebugType.GameFlow);

            while (_manager.CharacterAnimation.IsAnimating)
            {
                yield return 0;
            }

            debug.Log("UpdateUI", DebugController.DebugType.GameFlow);

            _manager.UpdateUI();
            
        }
        else
        {
            _lastActivity = null;
        }

        debug.Log("End of Loop", DebugController.DebugType.GameFlow);
    }


}
