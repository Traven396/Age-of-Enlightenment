using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpfulScript : MonoBehaviour
{
    public TestingScript newScript;
    public TestingScript otherScirpt;
    public int wholeNumber;

    private void Start()
    {
       
    }

    #region Shit
    public static float GetAngleConversion(float angle)
    {
        float newAngle = angle;
        if (newAngle > 180f)
        {
            newAngle -= 360;
        }
        //Debug.Log(newAngle);
        return newAngle;

    }

    public static Vector3 GetFullRotationConversion(Vector3 rotation)
    {
        rotation.x = GetAngleConversion(rotation.x);
        rotation.y = GetAngleConversion(rotation.y);
        rotation.z = GetAngleConversion(rotation.z);
        return rotation;
    } 

    public static Vector3 GetVectorAverage(Vector3[] vectors)
    {
        float x = 0, y = 0, z = 0;
        int numVectors = 0;

        for (int i = 0; i < vectors.Length; i++)
        {
            if (vectors[i] != null){
                x += vectors[i].x;
                y += vectors[i].y;
                z += vectors[i].z;
                numVectors++;
            }
        }
        if(numVectors > 0)
        {
            Vector3 average = new Vector3(x / numVectors, y / numVectors, z / numVectors);
            return average;
        }
        return Vector3.zero;
    }
    #endregion
}
