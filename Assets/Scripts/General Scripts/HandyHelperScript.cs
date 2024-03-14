using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class HandyHelperScript
{
    #region Methods

    public static bool VisibleFromCamera(Renderer renderer, Camera camera)
    {
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(frustumPlanes, renderer.bounds);
    }

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
    public static Vector3 FindNearestPointOnLine(Vector3 origin, Vector3 end, Vector3 point)
    {
        var projectedLine = (end - origin);
        float magnitudeMax = projectedLine.magnitude;
        projectedLine.Normalize();

        //Do projection from the point but clamp it
        var lhs = point - origin;
        float dotP = Vector3.Dot(lhs, projectedLine);
        dotP = Mathf.Clamp(dotP, 0f, magnitudeMax);
        return origin + projectedLine * dotP;
    }

    public static Vector3 GetFullRotationConversion(Vector3 rotation)
    {
        rotation.x = GetAngleConversion(rotation.x);
        rotation.y = GetAngleConversion(rotation.y);
        rotation.z = GetAngleConversion(rotation.z);
        return rotation;
    }
    public static Vector3 OrthogonalVector(this Vector3 v)
    {
        //This is some wizard type shit ngl. Its fitting lol
        v.Normalize();
        var x = v.x;
        var y = v.y;
        var z = v.z;
        var v1 = new Vector3(0f, z, -y);
        var v2 = new Vector3(-z, 0f, x);
        var v3 = new Vector3(-y, x, 0f);
        var largest = v1;
        if (v2.magnitude > largest.magnitude)
            largest = v2;
        if (v3.magnitude > largest.magnitude)
            largest = v3;
        return largest;
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

    public static List<Collider> GetColliders(this GameObject go, bool includedTriggers = false)
    {
        return go.GetComponentsInChildren<Collider>().Where(e => !e.isTrigger || includedTriggers).ToList();
    }

    public static IEnumerable<Collider> GetColliders(this Rigidbody rigidbody, bool includeTriggers = false)
    {
        return GetColliders(rigidbody, rigidbody.transform, includeTriggers);
    }


    private static IEnumerable<Collider> GetColliders(this Rigidbody rigidbody, Transform transform, bool includeTriggers = false)
    {
        var rb = transform.GetComponent<Rigidbody>();
        if (rb && rb != rigidbody)
            yield break;

        foreach (var c in transform.GetComponents<Collider>())
        {
            if (!c.enabled) continue;
            if (!c.isTrigger || (c.isTrigger && includeTriggers))
                yield return c;
        }

        foreach (Transform child in transform)
        {
            foreach (var c in GetColliders(rigidbody, child))
            {
                yield return c;
            }
        }
    }
    #endregion
}
