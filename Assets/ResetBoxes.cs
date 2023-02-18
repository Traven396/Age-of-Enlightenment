using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetBoxes : MonoBehaviour
{
    public List<GameObject> resetableObjects = new List<GameObject>();
    private List<Vector3> objectPos = new List<Vector3>();
    private List<Quaternion> objectRot = new List<Quaternion>();
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < resetableObjects.Count; i++)
        {
            objectPos.Add(resetableObjects[i].transform.position);

            objectRot.Add(resetableObjects[i].transform.rotation);
        }
    }

    public void Reset()
    {
        for (int i = 0; i < resetableObjects.Count; i++)
        {
            resetableObjects[i].transform.position = objectPos[i];

            resetableObjects[i].transform.rotation = objectRot[i];
        }
    }
}
