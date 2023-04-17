using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    #region Singleton Behavior
    public static EnemyManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType(typeof(EnemyManager)) as EnemyManager;

            return instance;
        }
        set
        {
            instance = value;
        }
    }
    private static EnemyManager instance;
    #endregion

    public List<AudioClip> hurtSounds = new List<AudioClip>();
    public List<AudioClip> ambientSounds = new List<AudioClip>();

    public AudioClip GetRandomHurtSound()
    {
        var num = Random.Range(0, hurtSounds.Count);
        return hurtSounds[num];
    }
    public AudioClip GetRandomSoundEffect()
    {
        var num = Random.Range(0, ambientSounds.Count);
        return ambientSounds[num];
    }
}
