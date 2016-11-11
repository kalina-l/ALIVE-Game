using UnityEngine;
using System.Collections;

public class Attribute {

    private int _value;
    private int _min;
    private int _max;

    public Attribute(int value, int min, int max)
    {
        _value = value;
        _min = min;
        _max = max;
    }

    public int Value
    {
        get
        {
            return _value;
        }
        set
        {
            _value = Mathf.Clamp(value, _min, _max);
        }
        
    }
}
