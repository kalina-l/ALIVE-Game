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
			if (value > 100)
				_value = 100;
			else if (value < -100) {
				_value = -100;
			}
			else _value = value;
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
}
