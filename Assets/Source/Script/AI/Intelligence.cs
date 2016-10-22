using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class Intelligence : MonoBehaviour {
	public virtual void ReceiveFeedback(int feedback)
	{
		Debug.Log ("This is just the Base class, it does nothing");
	}
}
