using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelpfulScript
{
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
}
