using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/*
 * Aidyn Reis
 * 9/25/22
 * Advanced Technologies Project 2
 */
//[ExecuteAlways]
public class StarBehavior : MonoBehaviour
{
    public bool spawnLines = false;
    public bool deleteLines = false;
    [Space(10f)]
    public ConstellationOverhead PuzzleHead;
    [Space(5f)]
    

    public GameObject prefabConnectorLine;
    public List<GameObject> destinationPoints = new List<GameObject>();

    private List<GameObject> spawnedLines = new List<GameObject>();



    //Audio Stuff
    private AudioSource selfAudio;
    private void Start()
    {
        selfAudio = GetComponent<AudioSource>();
        if (PuzzleHead == null)
            PuzzleHead = GetComponentInParent<ConstellationOverhead>();
        //Make sure not to run this code if this is a complete endpoint star
        CreateLines();

        //Add the constellation overhead father object thing
        //But only if I forgot to already add it
        if (PuzzleHead != null && !PuzzleHead.childStarList.Contains(this))
        {
            PuzzleHead.childStarList.Add(this);
        }

    }
    
    // Update is called once per frame
    void Update()
    {
        //Call the update lines method
        UpdateLines();
        //This allows me to cause the code to execute once in the editor, allowing me to easier make puzzles
        //By making the code execute in edit mode whenever a value changes it causes the update function once
        if (spawnLines)
        {
            CreateLines();
        }
        if (deleteLines)
        {
            DestroyLines();
        }
        //Immediately set the bool back to false to make this act as a button
        spawnLines = false;
        deleteLines = false;
        
        

    }
    private void OnDestroy()
    {
        DestroyLines();
        if (PuzzleHead != null)
            PuzzleHead.childStarList.Remove(this);
    }
    public void CreateLines()
    {
        //Make sure there is somewhere to create lines to, if I ran it without this check it could cause errors
        if (destinationPoints.Count != 0)
        {
            //Another check to make sure it knows what line to spawn
            if (prefabConnectorLine != null)
            {
                for (int i = 0; i < destinationPoints.Count; i++)
                {
                    //Create a copy of the line provided, then tell it which stars it is connected to for collision handling
                        spawnedLines.Add(Instantiate(prefabConnectorLine, PuzzleHead.transform, true));
                        spawnedLines[i].GetComponent<LineBehavior>().point1 = gameObject;
                        spawnedLines[i].GetComponent<LineBehavior>().point2 = destinationPoints[i];
                    
                }
            }
            //Add error tracking, the simplest kind though
            else
                Debug.LogError("You must provide a line prefab");
        }
    }
    public void UpdateLines()
    {
        //Another check to make sure there are destinations
        if (destinationPoints.Count != 0)
        {
            //This is placed outside the loop so that it is not repeated, saving on a little memory
            var currentPos = transform.position;
            for (int i = 0; i < spawnedLines.Count; i++)
            {
                if (spawnedLines[i] != null)
                {
                    var currentLine = spawnedLines[i];
                    //Create the variable references so that things dont get messy
                    var endStar = currentLine.GetComponent<LineBehavior>().point2;

                    //Perform some simple math to figure out the middlepoint between the two stars
                    //Then change the size of the line to match the required amount
                    float lineLength = Vector3.Distance(currentPos, endStar.transform.position);
                    currentLine.transform.localScale = new Vector3(0.02f, lineLength / 2f, 0.02f); 

                    //Calculate the middle between the two stars, and then place the line there
                    Vector3 midPoint = (currentPos + endStar.transform.position) / 2;
                    currentLine.transform.position = midPoint;

                    //Then just make the line look at the end point, and make it rotate correctly
                    currentLine.transform.LookAt(endStar.transform);
                    currentLine.transform.Rotate(new Vector3(1f, 0, 0), 90);
                }
            }

        }

    }
    public void DestroyLines()
    {
        //A simple way to remove any lines, allows me to clean up the puzzle area easily
        foreach (GameObject line in spawnedLines)
        {
            Destroy(line);
        }
    }
    public void HintEnter(SelectEnterEventArgs args)
    {
        if (args.interactorObject.GetType() == typeof(XRDirectInteractor))
        {
            PlayRandomPitchSound();
            if (PuzzleHead != null)
            {
                if (args.interactorObject.transform.tag == "Right" && PuzzleHead != null)
                {
                    PuzzleHead.rightGrab = true;
                }
                else if (args.interactorObject.transform.tag == "Left" && PuzzleHead != null)
                {
                    PuzzleHead.leftGrab = true;
                }
                PuzzleHead.VisualDisplay(true);
            }
        }
    }
    public void HintExit(SelectExitEventArgs args)
    { 
        if (args.interactorObject.GetType() == typeof(XRDirectInteractor))
        {
            PlayRandomPitchSound();
            if (PuzzleHead != null)
            {
                if (args.interactorObject.transform.tag == "Right" && PuzzleHead != null)
                {
                    PuzzleHead.rightGrab = false;
                }
                else if (args.interactorObject.transform.tag == "Left" && PuzzleHead != null)
                {
                    PuzzleHead.leftGrab = false;
                }
                PuzzleHead.VisualDisplay(false);
            }
        }
    }

    void PlayRandomPitchSound()
    {
        selfAudio.pitch = Random.Range(.9f, 1.3f);
        selfAudio.Play();
    }
   
}
