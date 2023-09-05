using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.XR.OpenXR;

public class TestingScript : MonoBehaviour
{
    private void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            Debug.Log("In Loop");
        }
        Debug.Log("Out of loop");
    }
}