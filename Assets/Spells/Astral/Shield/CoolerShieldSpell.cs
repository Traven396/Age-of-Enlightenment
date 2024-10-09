using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FoxheadDev.GestureDetection;

public class CoolerShieldSpell : SpellBlueprint
{
    public GameObject ShieldEffectPrefab;
    public AudioClip StepCompleteSound;
    public AudioClip CastCompleteSound;
    public AudioClip ShieldDissapearSound;

    private Step _CastingSequence;
    private AudioSource _CircleAudioSource;

    private GameObject spawnedShield;
    private Renderer shieldRenderer;
    public override void OnSelect()
    {
        base.OnSelect();

        _CircleAudioSource = _SpellCircle.GetComponent<AudioSource>();

        _CastingSequence = Step.Start();

        _CastingSequence.Then(PremadeGestureLibrary.PunchGlobal(_HandPhysicsTracker))
            .Do(StepComplete)
            .After(.2f)
            .Then(PremadeGestureLibrary.ReversePunchGlobal(_HandPhysicsTracker))
            .Do(FinishCasting);
    }
    public override void OnDeselect()
    {
        base.OnDeselect();
        if (spawnedShield)
        {
            Destroy(spawnedShield);
            AudioSource.PlayClipAtPoint(ShieldDissapearSound, _HandTransform.position, .8f);
        }
    }

    public override void GripPress()
    {
        base.GripPress();

        _CastingSequence.Reset();
        
    }

    public override void GripHold()
    {
        base.GripHold();
        _CastingSequence.Update();
        
    }

    public override void GripRelease()
    {
        base.GripRelease();

        if (spawnedShield)
        {
            StopCoroutine(nameof(ShieldSpawnCoroutine));
            StartCoroutine(nameof(ShieldDespawnCoroutine));

            AudioSource.PlayClipAtPoint(ShieldDissapearSound, _HandTransform.position, .8f);
        }
        
    }
    private void FinishCasting()
    {
        if (!spawnedShield)
        {
            spawnedShield = Instantiate(ShieldEffectPrefab, _CircleHolderTransform.transform.TransformPoint(0, 0.08f, 0), _CircleHolderTransform.transform.rotation, _CircleHolderTransform.transform);
            shieldRenderer = spawnedShield.GetComponentInChildren<Renderer>();
        }
        else
            spawnedShield.SetActive(true);

        _CircleAudioSource.PlayOneShot(CastCompleteSound);

        StopCoroutine(nameof(ShieldDespawnCoroutine));
        StartCoroutine(nameof(ShieldSpawnCoroutine));
    }
    private IEnumerator ShieldDespawnCoroutine()
    {
        var counter = 0;
        while (true)
        {
            counter++;
            var newDissolveAmount = iTween.FloatUpdate(shieldRenderer.material.GetFloat("_DissolveAmount"), 1, 2.5f);

            if (newDissolveAmount > 0.98)
                newDissolveAmount = 1;

            shieldRenderer.material.SetFloat("_DissolveAmount", newDissolveAmount);

            if (shieldRenderer.material.GetFloat("_DissolveAmount") >= 1)
            {
                if (spawnedShield.activeInHierarchy)
                    spawnedShield.SetActive(false);
                break;
            }

            if (counter >= 3000)
                break;
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
    private IEnumerator ShieldSpawnCoroutine()
    {
        var counter = 0;
        while (true)
        {
            counter++;
            var newDissolveAmount = iTween.FloatUpdate(shieldRenderer.material.GetFloat("_DissolveAmount"), 0, 2.5f);

            if (newDissolveAmount < 0.01)
                newDissolveAmount = 0;

            shieldRenderer.material.SetFloat("_DissolveAmount", newDissolveAmount);
            

            if (shieldRenderer.material.GetFloat("_DissolveAmount") <= 0)
                break;

            if (counter >= 3000)
                break;
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
    private void StepComplete()
    {
        AudioSource.PlayClipAtPoint(StepCompleteSound, _HandTransform.position);
    }
}
