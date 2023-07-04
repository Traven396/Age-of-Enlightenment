using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantSpin : MonoBehaviour
{
    public float rotationSpeed = 1;
    public string rotationAxis = "z";
    // Update is called once per frame
    private void Start()
    {
        Rotate();
    }
    private void Rotate()
    {
        iTween.RotateBy(this.gameObject, iTween.Hash(rotationAxis, rotationSpeed, "time", 10, "looptype", iTween.LoopType.loop, 
                                                      "space", Space.Self,
                                                      "easetype", iTween.EaseType.linear)); 
    }
}
