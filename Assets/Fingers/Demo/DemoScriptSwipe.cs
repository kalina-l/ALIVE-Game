//
// Fingers Gestures
// (c) 2015 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DigitalRubyShared
{
    public class DemoScriptSwipe : MonoBehaviour
    {
        public ParticleSystem SwipeParticleSystem;

        private void Start()
        {
            SwipeGestureRecognizer swipe = new SwipeGestureRecognizer();
            swipe.Updated += Swipe_Updated;
            swipe.DirectionThreshold = 0;
            FingersScript.Instance.AddGesture(swipe);
        }

        private void Swipe_Updated(GestureRecognizer gesture, ICollection<GestureTouch> touches)
        {
            SwipeGestureRecognizer swipe = gesture as SwipeGestureRecognizer;
            if (swipe.State == GestureRecognizerState.Began)
            {
                float angle = Mathf.Atan2(-swipe.DistanceY, swipe.DistanceX) * Mathf.Rad2Deg;
                SwipeParticleSystem.transform.rotation = Quaternion.Euler(angle, 90.0f, 0.0f);
                Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(gesture.StartFocusX, gesture.StartFocusY, 0.0f));
                pos.z = 0.0f;
                SwipeParticleSystem.transform.position = pos;
                SwipeParticleSystem.Play();
                Debug.Log("Swipe began!");
            }
            else if (swipe.State == GestureRecognizerState.Executing)
            {
                Debug.Log("Swipe executing!");
            }
            else if (swipe.State == GestureRecognizerState.Ended)
            {
                Debug.Log("Swipe ended!");
            }
        }

        private void Update()
        {

        }
    }
}