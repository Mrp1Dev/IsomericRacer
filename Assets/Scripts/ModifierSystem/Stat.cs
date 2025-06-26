using System;
using System.Collections.Generic;

[Serializable]
public class Stat
{
    public float realValue;
    private List<StatModifierSO> modifiers = new List<StatModifierSO>();

    public float GetValue()
    {
        var val = realValue;
        foreach (var modifier in modifiers)
        {
            val *= modifier.modifier;
        }
        return val;
    }

    public void AddModifier(StatModifierSO modifier) { modifiers.Add(modifier); }
    public void RemoveModifier(StatModifierSO modifier) { modifiers.Remove(modifier); }

}
