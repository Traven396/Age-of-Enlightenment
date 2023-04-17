using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.XR.OpenXR;

public class TestingScript : MonoBehaviour
{
    private Image gridImage;
    public Color invalidColor, validColor;
    private void Start()
    {
        gridImage = GetComponentInChildren<Image>();
    }
    public void SetInvalid()
    {
        gridImage.color = invalidColor;
    }
    public void SetValid()
    {
        gridImage.color = validColor;
    }
}