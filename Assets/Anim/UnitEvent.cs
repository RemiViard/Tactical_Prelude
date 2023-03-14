using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnitEvent : MonoBehaviour
{
    [HideInInspector] public UnityEvent strikeEvent;
    [HideInInspector] public UnityEvent rotEvent;
    Unit unit_link;
    public void LoadEvent(Unit _unit)
    {
        unit_link = _unit;
        strikeEvent.AddListener(_unit.Strike);
        rotEvent.AddListener(_unit.Rot);
    }
    public void strikeInvoke()
    {
        strikeEvent.Invoke();
    }
    public void rotInvoke()
    {
        rotEvent.Invoke();
    }
}
