using UnityEngine;
using System.Collections;

public enum NeedType { HEALTH, HUNGER, SOCIAL, ENERGY, SATISFACTION};
public enum Evaluation {	SUICIDAL, SUPER_BAD, VERY_BAD, BAD, NEUTRAL, GOOD, VERY_GOOD, SUPER_GOOD}; 

public class Need {

    public NeedType Type { get; set; }

	private int _value;
	public int Value
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

	public Need (int value, int[] thresholds) {
		this.Value = value;
		this.thresholds = thresholds;
	}

	public Evaluation getEvaluation () {
		for(int i = 0; i < thresholds.Length; i++){
			if (Value < thresholds [i])
				return (Evaluation)i;
		}
		return Evaluation.SUPER_GOOD;
	}

    public float GetSliderValue()
    {
        return (float)(_value + 100f) / 200f;
    }
}
