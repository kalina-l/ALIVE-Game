using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticFunctions {

    public static bool ToEnum<Enum>(this string strEnumValue, out Enum enumValue)
    {
        if (!System.Enum.IsDefined(typeof(Enum), strEnumValue))
        {
            enumValue = default(Enum);
            return false;
        }

        enumValue = (Enum)System.Enum.Parse(typeof(Enum), strEnumValue);
        return true;
    }
}
