using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using System;
using PathCreation;

public class TestingScript : MonoBehaviour
{
    [SerializeField]private GameObject savedNumber;
    private int CreditCardNumber;
    public Vector3[] points;

    private void Start()
    {
        BezierPath bezPath = new BezierPath(points, false, PathSpace.xyz);
        VertexPath bruh = new VertexPath(bezPath, transform, .1f);
        GetComponent<PathCreator>().bezierPath = bezPath;
    }
    IEnumerator OHFUCK()
    {
        yield return null;
    }
}