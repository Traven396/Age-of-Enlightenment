using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
/*
 * Aidyn Reis
 * 9/25/22
 * Advanced Technologies Project 2
 */

public class MenuController : MonoBehaviour
{
   
    public Slider difficultySlider;
    public TMP_Text difficultyLabel;
    [Space(10f)]
    public Transform spawnPoint;
    public GameObject starPrefab;
    public ConstellationSocketHead socketVisuals;
    [Space(5f)]
    [Header("Puzzle Configs")]
    public float distanceRange = .5f;
    public int maxConnections = 3;
    private int starNumber = 0;

    [Space(5f)]
    public Puzzle[] puzzleArrayBetter;

    private GameObject puzzleParent;
    private ConstellationOverhead puzzleHeadBrain;
    private void Start()
    {
        //Initailly set the text and value of the variable
        starNumber = (int)difficultySlider.value;
        difficultyLabel.text = "Number of stars: " + starNumber;
        
    }
    public void GeneratePuzzle()
    {
        //Call the clear puzzle method, freeing up the space
        ClearPuzzle();

        //Make an artificial Puzzle Head object
        GameObject artPuzzleHead = new GameObject("Puzzle Head");
        artPuzzleHead.transform.parent = spawnPoint.transform;
        puzzleParent = artPuzzleHead;
        puzzleHeadBrain = puzzleParent.AddComponent<ConstellationOverhead>();
        puzzleHeadBrain.socketDisplay = socketVisuals;

        //Generate a new holding array for all the stars
        GameObject[] starsInPuzzle = new GameObject[starNumber];
        //Loop until you reach the requested amount of stars, making a random position for each of them
        
        for (int i = 0; i < starNumber; i++)
        {
            //Create a random position based on the center of the puzzle to place the new stars in
            float randomX = spawnPoint.position.x - Random.Range(-distanceRange, distanceRange);
            float randomY = spawnPoint.position.y - Random.Range(-distanceRange, distanceRange);
            float randomZ = spawnPoint.position.z - Random.Range(-distanceRange, distanceRange);
            Vector3 randomPos = new Vector3(randomX, randomY, randomZ);

            starsInPuzzle[i] = Instantiate(starPrefab, randomPos, Quaternion.identity, puzzleParent.transform);
        }
        
        foreach (GameObject star in starsInPuzzle)
        {
            //Get a reference to the StarBehavior we are currently using
            StarBehavior currentStar = star.GetComponentInChildren<StarBehavior>();

            //Set the current star's parent to what we need
            currentStar.PuzzleHead = puzzleHeadBrain;
            
            //Loop a random number of times creating random connections
            //based on the max connections you set
            for (int i = 0; i < Random.Range(1, maxConnections); i++)
            {
                //Pick out a random star from all the stars in the puzzle
                int loops = 0;
                GameObject randomConnectionStar = starsInPuzzle[Random.Range(0, (starsInPuzzle.Length - 1))];
                bool validStar = true;
                do
                {
                    randomConnectionStar = starsInPuzzle[Random.Range(0, (starsInPuzzle.Length - 1))];
                    validStar = true;
                    if (currentStar.destinationPoints.Contains(randomConnectionStar))
                    { validStar = false; }
                    if (currentStar.gameObject == randomConnectionStar)
                    { validStar = false; }
                    if (randomConnectionStar.GetComponentInChildren<StarBehavior>().destinationPoints.Contains(currentStar.gameObject))
                    { validStar = false; }
                    loops++;
                    if (loops > 100)
                    {
                        Debug.Log("Loop broken");
                        break;
                    }

                } while (validStar == false);
                if (validStar)
                    currentStar.destinationPoints.Add(randomConnectionStar);
            }
        }
    }
    public void LoadPuzzle(int puzzleNumber)
    {
        //Once again clear the puzzle
        ClearPuzzle();
        //Spawn in the puzzle we are loading at the spawn positions
        var spawnedPuzzle = Instantiate(puzzleArrayBetter[puzzleNumber].loadablePuzzle, spawnPoint.transform.position, Quaternion.identity);
        spawnedPuzzle.transform.localScale = Vector3.one;
        //Begin setting up all the variables for the puzzle, like the brain
        puzzleHeadBrain = spawnedPuzzle.GetComponent<ConstellationOverhead>();
        //The puzzleParent object
        puzzleParent = puzzleHeadBrain.gameObject;
        //Telling the puzzle which button it is
        puzzleHeadBrain.referencedButton = puzzleArrayBetter[puzzleNumber].associatedButton.GetComponent<Image>();
        //And providing the puzzle with the socket visuals
        puzzleHeadBrain.socketDisplay = socketVisuals;

    }
    private void Update()
    {
        //Get the value from the provided slider
        starNumber = (int)difficultySlider.value;
        //Update the text
        difficultyLabel.text = "Number of stars: " + starNumber;
    }
    public void ClearPuzzle()
    {
        if (puzzleParent != null)
        {
            
            for (int i = 0; i < puzzleHeadBrain.childStarList.Count; i++)
            {
                Destroy(puzzleHeadBrain.childStarList[i].gameObject);
            }
            Destroy(puzzleParent);
            puzzleHeadBrain = null;
        }
    }




    public void LoadSpell(BaseSpell spellToLoad)
    {

    }
}

[System.Serializable]
public class Puzzle
{
    public GameObject loadablePuzzle;
    public GameObject associatedButton;

}