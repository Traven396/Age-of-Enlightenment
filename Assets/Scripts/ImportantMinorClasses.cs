using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImportantMinorClasses 
{
}


[Serializable]
public class FullSpellObject
{
    public Sprite SpellDisplayIcon;
    public string SpellName;
    public CoreSpellComponents SpellCouple;
    [TextArea(5, 10)]
    public string SpellDescription;
    public SpellSchool School;

}
[Serializable]
public class CoreSpellComponents
{
    public GameObject SpellMechanics;
    public GameObject SpellCircle;


    public CoreSpellComponents(GameObject newMechanics, GameObject newCircle)
    {
        SpellCircle = newCircle;

        SpellMechanics = newMechanics;
    }
}

public enum SpellSchool
{
    Pyrokinesis,
    Geomancy,
    Tempestia,
    Hydromorphy,
    Frostweaving,
    Voltcraft,
    Druidic,
    Metallurgy,
    Astral
}

public enum LeftRight
{
    Left,
    Right
}
public class SpellSwapCallbackContext
{
    public GameObject spawnedSelf;
    public SpellBlueprint spawnedScript;
    public GameObject circle;

    public SpellSwapCallbackContext(GameObject newSelf, SpellBlueprint newScript, GameObject theCircle)
    {
        spawnedSelf = newSelf;
        spawnedScript = newScript;
        circle = theCircle;
    }
    public SpellSwapCallbackContext()
    {
        spawnedScript = null;
        spawnedSelf = null;
        circle = null;
    }
}
