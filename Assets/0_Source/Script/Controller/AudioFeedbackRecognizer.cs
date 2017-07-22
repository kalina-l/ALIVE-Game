using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KKSpeech;

public class AudioFeedbackRecognizer : MonoBehaviour {
    
    private AudioFeedbackController _controller;

    public void Setup(AudioFeedbackController controller)
    {
        _controller = controller;
    }

	// Use this for initialization
	void Start () {
        if (SpeechRecognizer.ExistsOnDevice())
        {
            SpeechRecognizerListener listener = GameObject.FindObjectOfType<SpeechRecognizerListener>();
            listener.onAuthorizationStatusFetched.AddListener(OnAuthorizationStatusFetched);
            listener.onAvailabilityChanged.AddListener(OnAvailabilityChange);
            listener.onErrorDuringRecording.AddListener(OnError);
            listener.onErrorOnStartRecording.AddListener(OnError);
            listener.onPartialResults.AddListener(OnPartialResult);
            listener.onFinalResults.AddListener(OnFinalResult);
            SpeechRecognizer.RequestAccess();
        }
        else
        {
            //recordDebugger.text = "Sorry, but this device doesn't support speech recognition";
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StartRecording()
    {
        SpeechRecognizer.StartRecording(true);
    }

    public void StopRecording()
    {
        SpeechRecognizer.StopIfRecording();
        _controller.RecordingStopped();
    }

    public void OnFinalResult(string result)
    {
        _controller.RecordingStopped();
    }

    public void OnPartialResult(string result)
    {
        _controller.setAudioDebugText(result);
        result = result.ToLower();
        int feedback = 0;
        if (result.Contains("gut gemacht") || result.Contains("braver junge") || result.Contains("sehr gut") || result.Contains("nicht schlecht"))
        {
            feedback = 1;
            _controller.SendFeedback(feedback);
            StopRecording();
        }
        else if (result.Contains("aufhören") || result.Contains("so nicht") || result.Contains("aus") || result.Contains("stopp"))
        {
            feedback = -1;
            _controller.SendFeedback(feedback);
            StopRecording();
        }
    }

    public void OnAvailabilityChange(bool available)
    {
        if (!available)
        {
            //recordDebugger.text = "Speech Recognition not available";
        }
    }

    public void OnAuthorizationStatusFetched(AuthorizationStatus status)
    {
        if(status != AuthorizationStatus.Authorized) {
            //recordDebugger.text = "Cannot use Speech Recognition, authorization status is " + status;
        }
    }

    public void OnError(string error)
    {
        Debug.LogError(error);
        StopRecording();
        //recordDebugger.text = "Something went wrong... Try again! \n [" + error + "]";
    }

    public void OnStartRecordingPressed()
    {
        if (SpeechRecognizer.IsRecording())
        {
            SpeechRecognizer.StopIfRecording();
        }
        else
        {
            SpeechRecognizer.StartRecording(true);
            //recordDebugger.text = "Say something :-)";
        }
    }
}
