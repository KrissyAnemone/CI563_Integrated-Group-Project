using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineSFX : MonoBehaviour
{

    public AudioClip whirlClip;
    AudioSource whirSource;
    // Start is called before the first frame update
    void Start()
    {
        whirSource = gameObject.AddComponent<AudioSource>();
        whirSource.clip = whirlClip;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
