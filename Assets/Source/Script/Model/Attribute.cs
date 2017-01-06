using UnityEngine;
using System.Collections;

public enum AttributeType { STRENGTH, WISDOM, INTELLIGENCE, CONSTITUTION, CHARISMA};

public class Attribute {

    public AttributeType Identifier { get; set; }

    private int _value;
    [SerializeField]
    private int _min;
    [SerializeField]
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
