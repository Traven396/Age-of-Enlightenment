using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DynamicLineBehavior : MonoBehaviour
{
    public Transform startPoint, endPoint;

    private void Update()
    {
        float lineLength = Vector3.Distance(startPoint.position, endPoint.position);
        transform.localScale = new Vector3(0.02f, lineLength / 2f, 0.02f);

        Vector3 midPoint = (startPoint.position + endPoint.position) / 2;
        transform.position = midPoint;

        //Then just make the line look at the end point, and make it rotate correctly
        transform.LookAt(endPoint.transform);
        transform.Rotate(new Vector3(1f, 0, 0), 90);
    }
}
