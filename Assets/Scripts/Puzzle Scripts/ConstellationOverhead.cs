using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

/*
 * Aidyn Reis
 * 9/25/22
 * Advanced Technologies Project 2
 */
public class ConstellationOverhead : MonoBehaviour
{
    [HideInInspector]
    public List<LineBehavior> childLinesList = new List<LineBehavior>();
    //[HideInInspector]
    public List<StarBehavior> childStarList = new List<StarBehavior>();
    public bool PuzzleSolved = false;

    public Image referencedButton;

    [HideInInspector]
    public ConstellationSocketHead socketDisplay;
    [HideInInspector]
    public bool rightGrab = false;
    [HideInInspector]
    public bool leftGrab = false;
    public void VisualDisplay(bool YN)
    {
        if (YN)
        {
            if (!rightGrab || !leftGrab)
            {
                foreach (MeshRenderer socket in socketDisplay.childSockets)
                {
                    socket.enabled = YN;
                }
            }
        }
        else
        {
            if (!rightGrab && !leftGrab)
            {
                foreach (MeshRenderer socket in socketDisplay.childSockets)
                {
                    socket.enabled = YN;
                }
            }
        }
        //if (!rightGrab && !leftGrab)
        //{
        //    foreach (MeshRenderer socket in socketDisplay.childSockets)
        //    {
        //        socket.enabled = YN;
        //    }
        //}
    }

    private void Start()
    {
        foreach (StarBehavior star in childStarList)
        {
            for (int i = 0; i < star.destinationPoints.Count; i++)
            {
                if (star.destinationPoints[i].GetComponent<StarBehavior>().destinationPoints.Contains(star.gameObject))
                    star.destinationPoints.Remove(star.destinationPoints[i]);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        //Check if every child line is valid, if they are the puzzle has been solved
        if (childLinesList.All(line => line.valid))
        {
            PuzzleSolved = true;
            if (referencedButton != null)
            {
                referencedButton.color = Color.green;
            }
        }
        //Otherwise it has not been solved
        else
        {
            PuzzleSolved = false;
            if (referencedButton != null && referencedButton.color != Color.red)
            {
                referencedButton.color = Color.red;
            }
        }
    }
}
