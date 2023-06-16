using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effects : MonoBehaviour
{
    [SerializeField] private ParticleSystem RLWParticleSystem;
    [SerializeField] private ParticleSystem RRWParticleSystem;

    [Space(10)]
    [SerializeField] private TrailRenderer RLWTireSkid;
    [SerializeField] private TrailRenderer RRWTireSkid;

    private Wheels carWheels;

    public bool IsDrifting => Mathf.Abs(carWheels.LocalVelocityX) > 2.5f;

    private void Awake()
    {
        carWheels = GetComponent<Wheels>();
    }
    public void DriftCar()
    {
        if (IsDrifting)
        {
            RLWParticleSystem.Play();
            RRWParticleSystem.Play();
        }
        else
        {
            RLWParticleSystem.Stop();
            RRWParticleSystem.Stop();
        }

        if ((carWheels.IsTractionLocked || Mathf.Abs(carWheels.LocalVelocityX) > 5f) && Mathf.Abs(carWheels.CarSpeed) > 12f)
        {
            RLWTireSkid.emitting = true;
            RRWTireSkid.emitting = true;
        }
        else
        {
            RLWTireSkid.emitting = false;
            RRWTireSkid.emitting = false;
        }
    }
}
