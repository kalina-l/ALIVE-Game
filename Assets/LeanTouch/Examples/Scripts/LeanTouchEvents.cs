using UnityEngine;
using System.Collections.Generic;

namespace Lean.Touch
{
    // This script will hook into event LeanTouch event, and spam the console with the information
    public class LeanTouchEvents : MonoBehaviour
    {
        private Collider Lemo;
        public void setLemo(Collider collider)
        {
            Lemo = collider;
        }

        private bool isPetting;
        private bool blockPetting;
        private float maximumPetDuration = 3;

        protected virtual void OnEnable()
        {
            // Hook into the events we need
            LeanTouch.OnFingerDown += OnFingerDown;
            LeanTouch.OnFingerSet += OnFingerSet;
            LeanTouch.OnFingerUp += OnFingerUp;
            LeanTouch.OnFingerTap += OnFingerTap;
            LeanTouch.OnFingerSwipe += OnFingerSwipe;
            LeanTouch.OnGesture += OnGesture;
        }

        protected virtual void OnDisable()
        {
            // Unhook the events
            LeanTouch.OnFingerDown -= OnFingerDown;
            LeanTouch.OnFingerSet -= OnFingerSet;
            LeanTouch.OnFingerUp -= OnFingerUp;
            LeanTouch.OnFingerTap -= OnFingerTap;
            LeanTouch.OnFingerSwipe -= OnFingerSwipe;
            LeanTouch.OnGesture -= OnGesture;
        }

        public void OnFingerDown(LeanFinger finger)
        {
            //Debug.Log("Finger " + finger.Index + " began touching the screen");
        }

        public void OnFingerSet(LeanFinger finger)
        {
            if (!finger.IsOverGui && !blockPetting)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 100.0f))
                {
                    if (hit.collider == Lemo)
                    {
                        Vector2 distanceVector = finger.ScreenPosition - finger.StartScreenPosition;
                        if (distanceVector.magnitude > 30 && finger.Age > 0.2)
                        {
                            isPetting = true;
                        }
                        if (finger.Age > maximumPetDuration)
                        {
                            petSucceded();
                            blockPetting = true;
                        }
                    }
                }
            }

            //Debug.Log("Finger " + finger.Index + " is still touching the screen");
        }

        private void petSucceded()
        {
            isPetting = false;
            Debug.Log("Du hast Lemo gestreichelt.");
        }

        public void OnFingerUp(LeanFinger finger)
        {
            if (isPetting)
            {
                petSucceded();
            }
            blockPetting = false;
            //Debug.Log("Finger " + finger.Index + " finished touching the screen");

        }

        public void OnFingerTap(LeanFinger finger)
        {
            if (!finger.IsOverGui)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 100.0f))
                {
                    if (hit.collider == Lemo)
                    {
                        Debug.Log("You SLAPPED " + hit.transform.name); // ensure you picked right object
                    }
                }
            }

            //Debug.Log("Finger " + finger.Index + " tapped the screen");
        }

        public void OnFingerSwipe(LeanFinger finger)
        {
            //Debug.Log("Finger " + finger.Index + " swiped the screen");
        }

        public void OnGesture(List<LeanFinger> fingers)
        {
            //Debug.Log("Gesture with " + fingers.Count + " finger(s)");
            //Debug.Log("    pinch scale: " + LeanGesture.GetPinchScale(fingers));
            //Debug.Log("    twist degrees: " + LeanGesture.GetTwistDegrees(fingers));
            //Debug.Log("    twist radians: " + LeanGesture.GetTwistRadians(fingers));
            //Debug.Log("    screen delta: " + LeanGesture.GetScreenDelta(fingers));
        }
    }
}