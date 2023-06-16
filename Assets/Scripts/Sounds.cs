using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sounds : MonoBehaviour
{
    [SerializeField] private GameObject car;

    [Header("Sound")]
    [SerializeField] private AudioSource carEngineSound;
    [SerializeField] private AudioSource tireScreechSound; 

    private float initialCarEngineSoundPitch;
    private Effects carEffects;
    private Wheels carWheels;
    private Rigidbody carRigidbody;

    private void Awake()
    {
        carEffects = car.GetComponent<Effects>();
        carWheels = car.GetComponent<Wheels>();
        carRigidbody = car.GetComponent<Rigidbody>();
    }
    void Start()
    {
        initialCarEngineSoundPitch = carEngineSound.pitch;
        InvokeRepeating("CarSounds", 0f, 0.1f);
    }

    public void CarSounds()
    {
        float engineSoundPitch = initialCarEngineSoundPitch + (Mathf.Abs(carRigidbody.velocity.magnitude) / 25f);
        carEngineSound.pitch = engineSoundPitch;

        if (carEffects.IsDrifting || (carWheels.IsTractionLocked && Mathf.Abs(carWheels.CarSpeed) > 12f))
        {
            if (!tireScreechSound.isPlaying)
            {
                tireScreechSound.Play();
            }
        }
        else 
        {
            tireScreechSound.Stop();
        }
    }
}
