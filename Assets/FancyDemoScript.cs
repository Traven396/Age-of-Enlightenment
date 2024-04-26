using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FancyDemoScript : MonoBehaviour
{
    public GestureManager InputDetection;

    private void Update()
    {
        var movementAmount = new Vector3((InputDetection.angularVel / 20).y, -(InputDetection.angularVel / 20).x, InputDetection.angularVel.z);

        var distanceToHand = Vector3.Distance(transform.position, InputDetection.transform.position);

        if(movementAmount.x <= -1.5f && distanceToHand < 3)
        {
            Debug.Log("Yay, easter egg");
            iTween.RotateAdd(gameObject, new Vector3(0, -125, 0), 1.5f);
        }
    }
}
