using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticDoor : MonoBehaviour
{
    private DoorLogic Door;

    private void Awake()
    {
        Door = GetComponent<DoorLogic>();
        if (!Door)
            Door = GetComponentInChildren<DoorLogic>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            if (!Door.IsOpen)
            {
                Door.Open(other.transform.position);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            if (Door.IsOpen)
            {
                Door.Close();
            }
        }
    }
}
