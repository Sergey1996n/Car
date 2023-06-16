using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Effects))]
[RequireComponent(typeof(Rigidbody))]
public class Wheels : MonoBehaviour
{
    [Space(10)]
    [Range(70, 250)]
    [SerializeField] private int maxSpeed = 140;
    [Range(10, 60)]
    [SerializeField] private int maxReverseSpeed = 30;
    [Range(1, 10)]
    [SerializeField] private int accelerationMultiplier = 2;
    [Range(100, 600)]
    [SerializeField] private int brakeForce = 350; 
    [Range(1, 10)]
    [SerializeField] private int decelerationMultiplier = 2;
    [Range(1, 10)]
    [SerializeField] private int handbrakeDriftMultiplier = 5;
    [Space(10)]
    [Range(10, 45)]
    [SerializeField] private int maxSteeringAngle = 27;
    [Range(1f, 10f)]
    [SerializeField] private float steeringSpeed = 5f;

    [Header("WHEELS")]
    [SerializeField] private GameObject frontLeftMesh;
    [SerializeField] private WheelCollider frontLeftCollider;
    [Space(10)]
    [SerializeField] private GameObject frontRightMesh;
    [SerializeField] private WheelCollider frontRightCollider;
    [Space(10)]
    [SerializeField] private GameObject rearLeftMesh;
    [SerializeField] private WheelCollider rearLeftCollider;
    [Space(10)]
    [SerializeField] private GameObject rearRightMesh;
    [SerializeField] private WheelCollider rearRightCollider;

    private float localVelocityZ;
    private Rigidbody carRigidbody; 
    private Effects carEffects;
    private float throttleAxis; 
    private float driftingAxis;

    private WheelFrictionCurve FLwheelFriction;
    private float FLWextremumSlip;
    private WheelFrictionCurve FRwheelFriction;
    private float FRWextremumSlip;
    private WheelFrictionCurve RLwheelFriction;
    private float RLWextremumSlip;
    private WheelFrictionCurve RRwheelFriction;
    private float RRWextremumSlip;

    public float LocalVelocityX { get; private set; }
    public float CarSpeed { get; private set; }
    public bool IsTractionLocked { get; private set; }
    public float SteeringAxis { get; private set; }

    private void Awake()
    {
        carRigidbody = GetComponent<Rigidbody>();
        carEffects = GetComponent<Effects>();
    }

    private void Start()
    {
        FLwheelFriction = frontLeftCollider.sidewaysFriction;
        FLWextremumSlip = frontLeftCollider.sidewaysFriction.extremumSlip;

        FRwheelFriction = frontRightCollider.sidewaysFriction;
        FRWextremumSlip = frontRightCollider.sidewaysFriction.extremumSlip;

        RLwheelFriction = rearLeftCollider.sidewaysFriction;
        RLWextremumSlip = rearLeftCollider.sidewaysFriction.extremumSlip;

        RRwheelFriction = rearRightCollider.sidewaysFriction;
        RRWextremumSlip = rearRightCollider.sidewaysFriction.extremumSlip;
    }
    private void FixedUpdate()
    {
        CarSpeed = (2 * Mathf.PI * frontLeftCollider.radius * frontLeftCollider.rpm * 60) / 1000;
        LocalVelocityX = transform.InverseTransformDirection(carRigidbody.velocity).x;
        localVelocityZ = transform.InverseTransformDirection(carRigidbody.velocity).z;

    }

    private void ThrottleIncrease()
    {
        throttleAxis += Time.deltaTime * 3f;
        throttleAxis = Mathf.Min(throttleAxis, 1f);
    }

    private void ThrottleDecrease()
    {
        throttleAxis -= Time.deltaTime * 3f;
        throttleAxis = Mathf.Max(throttleAxis, -1f);
    }

    private void SetBrakes(float number = 0)
    {
        frontLeftCollider.brakeTorque = number;
        frontRightCollider.brakeTorque = number;
        rearLeftCollider.brakeTorque = number;
        rearRightCollider.brakeTorque = number;
    }

    private void SetMotors(float number = 0)
    {
        frontLeftCollider.motorTorque = number;
        frontRightCollider.motorTorque = number;
        rearLeftCollider.motorTorque = number;
        rearRightCollider.motorTorque = number;
    }

    private void SetSidewaysFriction(WheelFrictionCurve curve, float extremumSlip, float driftMultiplier, WheelCollider collider)
    {
        curve.extremumSlip = extremumSlip * driftMultiplier;
        collider.sidewaysFriction = curve;
    }

    private void SetSteeringAngle()
    {
        var steeringAngle = SteeringAxis * maxSteeringAngle;
        frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
        frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
    }

    public void GoForward()
    {
        carEffects.DriftCar();
        ThrottleIncrease();

        if (localVelocityZ < -1f)
        {
            SetBrakes(brakeForce);
        }
        else
        {
            if (Mathf.RoundToInt(CarSpeed) < maxSpeed)
            {
                SetBrakes();
                SetMotors(accelerationMultiplier * 50f * throttleAxis);
            }
            else
            {
                SetMotors();
            }
        }
    }

    public void GoReverse()
    {
        carEffects.DriftCar();
        ThrottleDecrease();

        if (localVelocityZ > 1f)
        {
            SetBrakes(brakeForce);
        }
        else
        {
            if (Mathf.RoundToInt(CarSpeed) < maxReverseSpeed)
            {
                SetBrakes();
                SetMotors(accelerationMultiplier * 50f * throttleAxis);
            }
            else
            {
                SetMotors();
            }
        }
    }

    public void DecelerateCar()
    {
        carEffects.DriftCar();
        if (throttleAxis != 0f)
        {
            if (throttleAxis > 0f)
            {
                throttleAxis -= Time.deltaTime * 10f;
            }
            else if (throttleAxis < 0f)
            {
                throttleAxis += Time.deltaTime * 10f;
            }
            if (Mathf.Abs(throttleAxis) < 0.15f)
            {
                throttleAxis = 0f;
            }
        }
        carRigidbody.velocity *= 1f / (1f + (0.025f * decelerationMultiplier));
        SetMotors();
        if (carRigidbody.velocity.magnitude < 0.25f)
        {
            carRigidbody.velocity = Vector3.zero;
            CancelInvoke("DecelerateCar");
        }
    }

    public void Handbrake()
    {
        CancelInvoke("RecoverTraction");
        driftingAxis += Time.deltaTime;
        float secureStartingPoint = FLWextremumSlip * handbrakeDriftMultiplier * driftingAxis;

        if (secureStartingPoint < FLWextremumSlip)
        {
            driftingAxis = 1f / handbrakeDriftMultiplier;
        }
        driftingAxis = Mathf.Min(driftingAxis, 1f);

        if (driftingAxis < 1f)
        {
            float driftMultiplier = handbrakeDriftMultiplier * driftingAxis;

            SetSidewaysFriction(FLwheelFriction, FLWextremumSlip, driftMultiplier, frontLeftCollider);
            SetSidewaysFriction(FRwheelFriction, FRWextremumSlip, driftMultiplier, frontRightCollider);
            SetSidewaysFriction(RLwheelFriction, RLWextremumSlip, driftMultiplier, rearLeftCollider);
            SetSidewaysFriction(RRwheelFriction, RRWextremumSlip, driftMultiplier, rearRightCollider);
        }
        IsTractionLocked = true;
        carEffects.DriftCar();
    }

    public void RecoverTraction()
    {
        IsTractionLocked = false;
        driftingAxis -= Time.deltaTime / 1.5f;
        driftingAxis = Mathf.Max(driftingAxis, 0f);

        if (FLwheelFriction.extremumSlip > FLWextremumSlip)
        {
            float driftMultiplier = handbrakeDriftMultiplier * driftingAxis;

            SetSidewaysFriction(FLwheelFriction, FLWextremumSlip, driftMultiplier, frontLeftCollider);
            SetSidewaysFriction(FRwheelFriction, FRWextremumSlip, driftMultiplier, frontRightCollider);
            SetSidewaysFriction(RLwheelFriction, RLWextremumSlip, driftMultiplier, rearLeftCollider);
            SetSidewaysFriction(RRwheelFriction, RRWextremumSlip, driftMultiplier, rearRightCollider);

            Invoke("RecoverTraction", Time.deltaTime);

        }
        else if (FLwheelFriction.extremumSlip < FLWextremumSlip)
        {
            SetSidewaysFriction(FLwheelFriction, FLWextremumSlip, 1, frontLeftCollider);
            SetSidewaysFriction(FRwheelFriction, FRWextremumSlip, 1, frontRightCollider);
            SetSidewaysFriction(RLwheelFriction, RLWextremumSlip, 1, rearLeftCollider);
            SetSidewaysFriction(RRwheelFriction, RRWextremumSlip, 1, rearRightCollider);

            driftingAxis = 0f;
        }
    }

    public void TurnLeft()
    {
        SteeringAxis -= Time.deltaTime * steeringSpeed;
        SteeringAxis = Mathf.Max(SteeringAxis, -1f);
        SetSteeringAngle();
    }

    public void TurnRight()
    {
        SteeringAxis += Time.deltaTime * steeringSpeed;
        SteeringAxis = Mathf.Min(SteeringAxis, 1f);
        SetSteeringAngle();
    }

    public void ResetSteeringAngle()
    {
        if (SteeringAxis < 0f)
        {
            SteeringAxis += Time.deltaTime * steeringSpeed;
        }
        else if (SteeringAxis > 0f)
        {
            SteeringAxis -= Time.deltaTime * steeringSpeed;
        }
        if (Mathf.Abs(frontLeftCollider.steerAngle) < 1f)
        {
            SteeringAxis = 0f;
        }
        SetSteeringAngle();
    }
    public void ThrottleOff()
    {
        frontLeftCollider.motorTorque = 0;
        frontRightCollider.motorTorque = 0;
        rearLeftCollider.motorTorque = 0;
        rearRightCollider.motorTorque = 0;
    }
}
