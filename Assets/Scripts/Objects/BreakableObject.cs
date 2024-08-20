using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    public AudioClip glassShatterSound;
    public GameObject glassShatterEffect;

    private AudioSource audioSource; // Add an AudioSource component

    private void Start()
    {
        Collider collider = GetComponent<Collider>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<BoxCollider>();
        }
        collider.isTrigger = true;

        // Get the AudioSource component (or add one if it doesn't exist)
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

 public void Break()
{
    Instantiate(glassShatterEffect, transform.position, Quaternion.identity);

    // Play the glass shattering sound normally
    audioSource.clip = glassShatterSound;
    audioSource.Play(); 

    // Destroy the object after the sound has finished playing
    Destroy(gameObject, audioSource.clip.length); 
}

    private void OnTriggerEnter(Collider other)
    {
        Break();
        BreakAllBreakableObjects(this);
    }

    public static void BreakAllBreakableObjects(BreakableObject objectToExclude = null)
    {
        GameObject[] breakableObjects = GameObject.FindGameObjectsWithTag("BreakableObject");

        foreach (GameObject obj in breakableObjects)
        {
            BreakableObject breakable = obj.GetComponent<BreakableObject>();
            if (breakable != null && breakable != objectToExclude)
            {
                breakable.Break();
            }
        }
    }
}