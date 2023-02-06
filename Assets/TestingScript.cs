using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using System;

public class TestingScript : MonoBehaviour
{

    private void Update()
    {
        float angle = 0;
        angle = transform.rotation.eulerAngles.x;
        if(angle > 180f)
        {
            angle -= 360;
        }
        Debug.Log(Mathf.RoundToInt(angle));
    }
}