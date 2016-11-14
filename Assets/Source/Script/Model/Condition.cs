using UnityEngine;
using System.Collections;

public enum Evaluation {	SUICIDAL, SUPER_BAD, VERY_BAD, BAD, NEUTRAL, GOOD, VERY_GOOD, SUPER_GOOD		}; 

public class Condition {

	private int _value;
	public int value
	{
		get
		{
			return _value;
		}
		set
		{
			_value = Mathf.Clamp(value, -100, 100);
		}
	}

	private int[] thresholds;

	public Condition (int value, int[] thresholds) {
		this.value = value;
		this.thresholds = thresholds;
	}

	public Evaluation getEvaluation () {
		for(int i = 0; i < thresholds.Length; i++){
			if (value < thresholds [i])
				return (Evaluation)i;
		}
		return Evaluation.SUPER_GOOD;
	}

    public float GetSliderValue()
    {
        return (float)(_value + 100f) / 200f;
    }
}
