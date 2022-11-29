using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Aidyn Reis
 * 10/5/22
 * Advanced Technologies Project 2
 */

public class WandBehavior : MonoBehaviour
{
    [SerializeField] GameObject projectilePrefab, exitPoint;
    [SerializeField] float projectileSpeed = 5f;
    [Space(10f)]
    [Header("Light Effect Settings")]
    [SerializeField] Light wandLight;
    [SerializeField] ParticleSystem particleEffect;
    public float maxGlow;
    public float glowChangeSpeed = .3f;
    
    bool isGlowing = false;
    public void Shoot()
    {
        var currentBullet = Instantiate(projectilePrefab, exitPoint.transform.position, Quaternion.identity);
        currentBullet.GetComponent<Rigidbody>().AddForce(exitPoint.transform.forward * projectileSpeed);
    }
    public void Glow()
    {
        if (isGlowing)
        {
            wandLight.intensity = Mathf.Lerp(wandLight.intensity, 0, glowChangeSpeed);
            particleEffect.Stop();
        }
        else
        {
            wandLight.intensity = Mathf.Lerp(wandLight.intensity, maxGlow, glowChangeSpeed);
            particleEffect.Play();
        }
        isGlowing = !isGlowing;
    }
}
