using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StartCoroutineTele : MonoBehaviour, ICastable
{
    public string _coroutineName;
    public Type thingToBe;
    public void Cast(Transform target)
    {
        target.GetComponent<TelekinesisTargetable>().StartCoroutine(_coroutineName);
        
    }
}
