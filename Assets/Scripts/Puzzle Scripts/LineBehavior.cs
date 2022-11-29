using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Aidyn Reis
 * 9/25/22
 * Advanced Technologies Project 2
 */
public class LineBehavior : MonoBehaviour
{
    
    public Material validColor;
    public Material invalidColor;

    
    public GameObject point1;
    public GameObject point2;

    private ConstellationOverhead PuzzleHead;

    public bool valid = true;

    private LineBehavior otherLine;

    private Renderer selfRender;

    private void Start()
    {
        PuzzleHead = GetComponentInParent<ConstellationOverhead>();
        PuzzleHead.childLinesList.Add(this);
        selfRender = GetComponentInChildren<Renderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Puzzle"))
        {
            if (other.TryGetComponent<LineBehavior>(out otherLine) && point1 != otherLine.point1 && point1 != otherLine.point2 && point2 != otherLine.point1 && point2 != otherLine.point2)
            {
                //Once we get through all the checks and have made sure we collided with a valid line
                //change the color to red, and tell the puzzle its not valid
                selfRender.material = invalidColor;
                
                valid = false;
            }
            
        }
    }
    private void OnTriggerStay(Collider other)
    {
        //Check every frame if they are colliding, but if valid is already set to false then no need to continue checking
        if (other.gameObject.CompareTag("Puzzle") && valid)
        {
            if (other.TryGetComponent<LineBehavior>(out otherLine) && point1 != otherLine.point1 && point1 != otherLine.point2 && point2 != otherLine.point1 && point2 != otherLine.point2)
            {
                //Once we get through all the checks and have made sure we collided with a valid line
                //change the color to red, and tell the puzzle its not valid
                selfRender.material = invalidColor;
                
                valid = false;
            }

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Puzzle"))
        {
            if (other.TryGetComponent<LineBehavior>(out otherLine) && point1 != otherLine.point1 && point1 != otherLine.point2 && point2 != otherLine.point1 && point2 != otherLine.point2)
            {
                //Do the opposite for exiting a collider. 
                selfRender.material = validColor;
                valid = true;
            }
        }
    }
}
