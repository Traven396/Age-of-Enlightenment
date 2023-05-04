using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleCard : MonoBehaviour
{
    [SerializeField]
    private GameObject StoredPuzzle;
    [SerializeField]
    private Sprite SolutionPrefab;
    [SerializeField]
    private FullSpellObject GrantedSpell;
    
    public GameObject GetPuzzle()
    {
        return StoredPuzzle;
    }
    public FullSpellObject GetSpell()
    {
        return GrantedSpell;
    }
    public Sprite GetSolution() { return SolutionPrefab; }
}
