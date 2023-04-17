using System.Collections;
using UnityEngine.Events;
using UnityEngine;
using System;

[Serializable]
public class RadialSection : MonoBehaviour
{
    public UnityEvent onSelect = new UnityEvent();
    public string nameOfSpell;
}
