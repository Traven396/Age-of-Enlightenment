using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repeater : SpellBlueprint
{
    [SerializeField] private float LaunchForce = 14;
    [SerializeField] private GameObject BladePrefab;


    private List<RepeaterBlade> Blades = new List<RepeaterBlade>(3);
    private Transform[] _bladePositions = new Transform[3];

    int counter = 0;
    private bool allBladesSpawned = false;

    private void Start()
    {
        _bladePositions[0] = new GameObject("position1").transform;

        _bladePositions[0].parent = _palmLocation;
        _bladePositions[0].position = _palmLocation.TransformPoint( new Vector3(0, 0, -0.12f));
        _bladePositions[0].localRotation = Quaternion.Euler(0, 90, 0);

        _bladePositions[1] = new GameObject("position2").transform;

        _bladePositions[1].parent = _palmLocation;
        _bladePositions[1].position = _palmLocation.TransformPoint(new Vector3(0, .045f, -0.1f));
        _bladePositions[1].localRotation = Quaternion.Euler(0, 90, 30);
        

        _bladePositions[2] = new GameObject("position3").transform;

        _bladePositions[2].parent = _palmLocation;
        _bladePositions[2].position = _palmLocation.TransformPoint(new Vector3(0, -.045f, -0.1f));
        _bladePositions[2].localRotation = Quaternion.Euler(0, 90, -30);

        StartCoroutine(TripleBladeSpawn());
        _visualsManager.ToggleReticle(currentHand, true);
    }

    public override void OnDeselect()
    {
        base.OnDeselect();
        for (int i = 0; i < _bladePositions.Length; i++)
        {
            Destroy(_bladePositions[i].gameObject);
        }

        _visualsManager.ToggleReticle(currentHand, false);
    }
    public override void TriggerPress()
    {
        base.TriggerPress();

        if (allBladesSpawned)
        {
            if (gripPressed)
            {
                foreach (RepeaterBlade blade in Blades)
                {
                    if (blade.launched)
                    {
                        blade.StartRecallBlade();
                    }
                }
            } 
        }
    }
    public override void TriggerRelease()
    {
        base.TriggerRelease();
        if (gripPressed)
        {
            Blades.ForEach(blade => blade.StopRecallBlade());
        }
        else
        {
            if (allBladesSpawned)
            {
                Blades[counter].Launch();
                counter++;
                if (counter >= Blades.Capacity)
                    counter = 0;
            }
        }
    }

    public override void GripRelease()
    {
        base.GripRelease();
        Blades.ForEach(blade => blade.StopRecallBlade());
    }
    IEnumerator SingleBladeSpawn(int bladeCounter)
    {
        Blades.Add(Instantiate(BladePrefab, _bladePositions[bladeCounter].position, _bladePositions[bladeCounter].rotation).GetComponent<RepeaterBlade>());

        Blades[bladeCounter].recallPoint = _bladePositions[bladeCounter];

        Blades[bladeCounter].transform.parent = _bladePositions[bladeCounter];

        Blades[bladeCounter].rb.isKinematic = true;

        var renderer = Blades[bladeCounter].GetComponentInChildren<Renderer>();

        var newDissolve = 1f;

        while (renderer.material.GetFloat("_DissolveAmount") > 0)
        {
            renderer.material.SetFloat("_DissolveAmount", newDissolve);
            newDissolve -= 0.05f;
            yield return new WaitForSeconds(0.05f);
        }

        yield return null;
    }
    IEnumerator TripleBladeSpawn()
    {
        StartCoroutine(SingleBladeSpawn(0));
        yield return new WaitForSeconds(1);

        StartCoroutine(SingleBladeSpawn(1));
        yield return new WaitForSeconds(1);

        StartCoroutine(SingleBladeSpawn(2));
        yield return new WaitForSeconds(1);

        allBladesSpawned = true;

        yield return null;
    }
}
