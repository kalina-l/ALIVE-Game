Fingers, by Jeff Johnson
Fingers (c) 2015 Digital Ruby, LLC
http://www.digitalruby.com

Version 1.3.2

Changelog
--------------------
1.3.2	- Change ImageGestureRecognizer again. It sends begin, executing and end states. It got a new ThresholdUnits property, which is the distance in units the touch must move before this gesture begins.
        - Critical bug fixes for all the gestures. I've tested a lot on iPad and Android devices to try and fix little glitches and problems with the asset.
        - Refactor TouchesBegan. It receives an enumerable collection of all beginning touches. CurrentTouches property is removed, use CurrentTrackedTouches instead inside TouchesMoved and TouchesEnded.
1.3.1	- ImageGestureRecognizer no longer sends began and executing states. Only Ended state is sent. You can tell if the state began if the gesture calls back and TouchesAreMoving is false, then the touches have just begun. If it is true, then the touches are moving.
1.3.0	- Added a demo showing a zoomable scroll view
		- Added a FingersScriptPrefab so the script can live for the lifetime of your game. See DemoScript.cs Start method.
1.2.5	- Added a way to override the anchor of the one touch rotate gesture.
1.2.4	- Pass through handler tweak: renamed to CaptureGestureHandler. A null handler or null return value uses default capture behavior, otherwise return true to capture the gesture, false to pass it through.
1.2.3	- Improvements to pass through gestures - now a parent UI element will pass through gestures to all child elements.
		- Also added a custom pass through handler function for complex cases.
1.2.2	- One finger gestures will not begin if more than one finger is down.
1.2.1	- Critical bug fixes for image gesture recognizer and image demo scene.
1.2		- Big refactor of code. Gestures now track touches, which means simultaneous gestures can execute on separate touches now.
		- Some properties have been removed or renamed. In GestureRecognizer.cs, the public properties (at the bottom of the file) are where most things can be found now.
		- Added custom image recognition gestures.
1.1.1	- Code refactoring, clear gestures when scenes change and added tutorial video link.
1.1		- Fingers can use all mouse buttons and the mouse wheel.
		- Use ctrl + wheel to pinch and shift + wheel to rotate.
		- FingersScript has Touches property and the demo adds circles for touch points.
1.0.2	- Bug fixes. conv
		- Breaking change: OnUpdated is now just Updated.
1.0.0	- Initial release.

Fingers is an advanced gesture recognizer system for Unity and any other platform where C# is supported (such as Xamarin). I am using this same code in a native drawing app for Android (You Doodle) and they work great.

If you've used UIGestureRecognizer on iOS, you should feel right at home using Fingers. In fact, Apple's documentation is very relevant to fingers: https://developer.apple.com/library/ios/documentation/UIKit/Reference/UIGestureRecognizer_Class/

Tutorial
--------------------
I've made a video that gives a full run down of this asset. Please view it here: https://youtu.be/97tJz0y52Fw
This tutorial covers creating custom shape gestures: https://youtu.be/7dvP_zhlWvU

Instructions
--------------------

To get started, add FingersScript.cs to a game object in your scene. The script has just a few properties and is easy to use.
- Treat mouse as pointer (default is true, useful for testing in the player for some gestures). Disable this if you are using Unity Remote or running on a touch screen like Surface Pro.
- Pass through views. By default all Text objects are ignored by Fingers. These are game objects that are part of the event system that will be ignored. For example, you could put a panel in this list and it would be ignored.
- Default DPI. In the event that Unity can't figure out the DPI of the device, use this default value.
- Add "using DigitalRubyShared;" to the top of your script.

Event System
--------------------
The gestures work with the Unity event system. Gestures over UI elements in a Canvas will be stopped and not executed by default - the exception being top level UI elements with only Text components.

Options for allowing gestures on UI elements:
- You can set a platform specific view on your gesture that is the game object of your UI element that you want to allow gestures on
- You can populate the pass through elements of the fingers script. Any element in this list will always pass the gesture through. Make sure to clear the list when a new scene loads.
- You can use the CaptureGestureHandler callback on the fingers script to run custom logic to determine whether the element can be pased through.

See the DemoScript.cs file for more details and examples.

Standard Gestures:
--------------------

Once you've added the script, you will need to add some gestures. This will require you to create a C# script of your own and add a reference to the FingersScript object that you added. See the demo script (Demo*.cs) for what this looks like. Remember to add the namespace DigitalRubyShared.

Each gesture has public properties that can configure things such as thresholds for movement, rotation, etc. The defaults should work well for most cases. Fingers works in inches by default.

Please review the Start method of DemoScript.cs to see how gestures are created and added to the finger script. Also watch that tutorial video if you get lost, it should be really helpful.

Custom Shapes:
--------------------

Custom shapes are possible with ShapeGestureRecognizer. This uses a fuzzy image recognition algorithm so it may not work 100% but hopefully will be good enough for your needs. Shapes are defined as an 16x16 image with 3 pixel width stroke.

To learn more about creating custom shape gestures and how to test them and refine them, please watch the tutorial video at https://youtu.be/7dvP_zhlWvU

One Finger Gestures:
--------------------
Scaling and Rotation one finger gestures are available. Please see DemoSceneOneFinger for more details.

Demos:
--------------------
I've made several demo scenes. Please check them out as they are great for seeing everything Fingers - Gestures for Unity can do.

Misc:
--------------------

*Note* I don't use anonymous / inline delegates in the demo script as these seem to crash on iOS.

I'm available to answer your questions or feedback at jeff@digitalruby.com

Thank you.

- Jeff Johnson

