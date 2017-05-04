using UnityEngine;
using System.Collections.Generic;
namespace Lean.Touch
{
    // This script will hook into event LeanTouch event
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

        private TouchController _controller;

        protected virtual void OnEnable()
        {
            // Hook into the events we need
            LeanTouch.OnFingerDown += OnFingerDown;
            LeanTouch.OnFingerSet += OnFingerSet;
            LeanTouch.OnFingerUp += OnFingerUp;
            LeanTouch.OnFingerTap += OnFingerTap;
        }

        protected virtual void OnDisable()
        {
            // Unhook the events
            LeanTouch.OnFingerDown -= OnFingerDown;
            LeanTouch.OnFingerSet -= OnFingerSet;
            LeanTouch.OnFingerUp -= OnFingerUp;
            LeanTouch.OnFingerTap -= OnFingerTap;
        }

        public void Setup(TouchController controller)
        {
            _controller = controller;
        }

        public void OnFingerDown(LeanFinger finger)
        {
            Debug.Log("Finger " + finger.Index + " began touching the screen: " + finger.ScreenPosition);
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
                        if (distanceVector.magnitude > 100 && finger.Age > 0.2)
                        {
                            isPetting = true;
                            _controller.ShowPetFeedback(finger.ScreenPosition);
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
            _controller.EndPetFeedback();
            _controller.SendFeedback(1);
            //Debug.Log("Du hast Lemo gestreichelt.");
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
                        _controller.ShowFistFeedback(finger.ScreenPosition); // TODO: convert screen position to canvas position
                        _controller.SendFeedback(-1);
                        //Debug.Log("You SLAPPED " + hit.transform.name); // ensure you picked right object
                    }
                }
            }
        }
    }
}