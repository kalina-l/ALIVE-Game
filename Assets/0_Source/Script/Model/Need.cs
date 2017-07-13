using UnityEngine;
using System.Collections;
using System;

public enum NeedType { HEALTH, HUNGER, SOCIAL, ENERGY, SATISFACTION};
public enum Evaluation {	SUICIDAL, SUPER_BAD, VERY_BAD, BAD, NEUTRAL, GOOD, VERY_GOOD, SUPER_GOOD}; 

public class Need {

    public NeedType Type { get; set; }

	private int _value;
    [SerializeField]
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

    public int[] Thresholds { get; set; }

    public Need (int value, int[] thresholds) {
		this.Value = value;
		this.Thresholds = thresholds;
	}

    public Need Copy()
    {
      return new Need(Value, Thresholds);
    }

	public Evaluation getEvaluation () {
		for(int i = 0; i < Thresholds.Length; i++){
			if (Value < Thresholds [i])
				return (Evaluation)i;
		}
		return Evaluation.SUPER_GOOD;
	}

    public float GetSliderValue()
    {
        return (float)(_value + 100f) / 200f;
    }
}
