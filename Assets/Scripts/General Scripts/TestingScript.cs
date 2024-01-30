using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.XR.OpenXR;

public class TestingScript : MonoBehaviour, IEntity
{
    private Collider col;
    private void Awake()
    {
        col = GetComponent<Collider>();
    }
    private void Start()
    {
        //Debug.Log(Mathf.Max(col.bounds.size.x, col.bounds.size.z));
    }
    public void ApplyMotion(Vector3 force, ForceMode forceMode)
    {
        Debug.Log("Bruh. How did you get this error?");
    }
}