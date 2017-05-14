﻿using Affdex;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEmotions : ImageResultsListener
{
    private Dictionary<string, float> emotions;

    private VideoFeedbackController videoFeedbackController;
    private VideoInput videoInput;

    public void setup(VideoFeedbackController controller, VideoInput videoInput)
    {
        videoFeedbackController = controller;
        this.videoInput = videoInput;

        emotions = new Dictionary<string, float>();
        emotions.Add("Smile", 0);
        emotions.Add("BrowRaise", 0);
        emotions.Add("Sadness", 0);
    }

    public override void onFaceFound(float timestamp, int faceId)
    {
        Debug.Log("Found the face");
    }

    public override void onFaceLost(float timestamp, int faceId)
    {
        Debug.Log("Lost the face");
    }

    public override void onImageResults(Dictionary<int, Face> faces)
    {
        //Debug.Log("Got FACE results:");
        //Debug.Log(faces.Count);

        foreach (KeyValuePair<int, Face> pair in faces)
        {
            int FaceId = pair.Key;  // The Face Unique Id.
            Face face = pair.Value;    // Instance of the face class containing emotions, and facial expression values.

            //Retrieve the Emotions Scores
            float currSadness;
            face.Emotions.TryGetValue(Emotions.Sadness, out currSadness);
            emotions["Sadness"] += currSadness;

            //Retrieve the Smile Score
            float currSmile;
            face.Expressions.TryGetValue(Expressions.Smile, out currSmile);
            emotions["Smile"] += currSmile;
            float currBrowRaise;
            face.Expressions.TryGetValue(Expressions.BrowRaise, out currBrowRaise);
            emotions["BrowRaise"] += currBrowRaise;

            Debug.Log("Sadness: " + emotions["Sadness"] + ", smile: " + emotions["Smile"] + ", brow raise: " + emotions["BrowRaise"]);
        }

        if(!videoInput.getIsRecording())
        {
            evaluate();
        }
    }

    private void evaluate ()
    {
        float biggestValue = 0;
        string nameOfBiggestValue = "";
        foreach (KeyValuePair<string, float> entry in emotions)
        {
            if(entry.Value > biggestValue)
            {
                biggestValue = entry.Value;
                nameOfBiggestValue = entry.Key;
            }
        }

        resetData();

        switch(nameOfBiggestValue)
        {
            case "Smile":
                videoFeedbackController.SendFeedback(1);
                break;
            case "BrowRaise":
                videoFeedbackController.SendFeedback(-1);
                break;
            case "Sadness":
                videoFeedbackController.SendFeedback(-1);
                break;
        }    
    }

    private void resetData ()
    {
        emotions["Sadness"] = 0;
        emotions["Smile"] = 0;
        emotions["BrowRaise"] = 0;
    }
}