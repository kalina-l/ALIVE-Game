using Affdex;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEmotions : ImageResultsListener
{
    public float currentSmile;
    public float currentBrowRaise;
    public float currentInterocularDistance;
    public float currentJoy;
    public float currentSadness;
    public float currentAnger;
    public FeaturePoint[] featurePointsList;

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
            face.Emotions.TryGetValue(Emotions.Joy, out currentJoy);
            face.Emotions.TryGetValue(Emotions.Sadness, out currentSadness);
            face.Emotions.TryGetValue(Emotions.Anger, out currentAnger);

            //Retrieve the Smile Score
            face.Expressions.TryGetValue(Expressions.Smile, out currentSmile);
            face.Expressions.TryGetValue(Expressions.BrowRaise, out currentBrowRaise);


            //Retrieve the Interocular distance, the distance between two outer eye corners.
            currentInterocularDistance = face.Measurements.interOcularDistance;


            //Retrieve the coordinates of the facial landmarks (face feature points)
            featurePointsList = face.FeaturePoints;

            Debug.Log("Joy: " + currentJoy + ", sadness: " + currentSadness + ", anger: " + currentAnger + ", smile: " + currentSmile + ", brow raise: " + currentBrowRaise);
        }
    }
}