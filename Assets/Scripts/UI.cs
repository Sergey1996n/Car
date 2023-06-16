using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] private Text carSpeedText;
    [SerializeField] private Wheels carWheels;

    [Header("Controller")]
    [SerializeField] private GameObject throttleButton;
    [SerializeField] private GameObject reverseButton;
    [SerializeField] private GameObject turnRightButton;
    [SerializeField] private GameObject turnLeftButton;
    [SerializeField] private GameObject handbrakeButton;

    private TouchInput throttle;
    private TouchInput reverse;
    private TouchInput turnRight;
    private TouchInput turnLeft;
    private TouchInput handbrake;

    private bool deceleratingCar;
    private void Awake()
    {
        throttle = throttleButton.GetComponent<TouchInput>();
        reverse = reverseButton.GetComponent<TouchInput>();
        turnRight = turnLeftButton.GetComponent<TouchInput>();
        turnLeft = turnRightButton.GetComponent<TouchInput>();
        handbrake = handbrakeButton.GetComponent<TouchInput>();
    }

    void FixedUpdate()
    {
        float absoluteCarSpeed = Mathf.Abs(carWheels.CarSpeed);
        carSpeedText.text = Mathf.RoundToInt(absoluteCarSpeed).ToString();
    }

    private void Update()
    {
        if (throttle.ButtonPressed)
        {
            carWheels.CancelInvoke("DecelerateCar");
            deceleratingCar = false;
            carWheels.GoForward();
        }
        if (reverse.ButtonPressed)
        {
            carWheels.CancelInvoke("DecelerateCar");
            deceleratingCar = false;
            carWheels.GoReverse();
        }

        if (turnLeft.ButtonPressed)
        {
            carWheels.TurnLeft();
        }
        if (turnRight.ButtonPressed)
        {
            carWheels.TurnRight();
        }
        if (handbrake.ButtonPressed)
        {
            carWheels.CancelInvoke("DecelerateCar");
            deceleratingCar = false;
            carWheels.Handbrake();
        }
        if (!handbrake.ButtonPressed)
        {
            carWheels.RecoverTraction();
        }
        if ((!throttle.ButtonPressed && !reverse.ButtonPressed))
        {
            carWheels.ThrottleOff();
        }
        if (!reverse.ButtonPressed && !throttle.ButtonPressed && !handbrake.ButtonPressed && !deceleratingCar)
        {
            carWheels.InvokeRepeating("DecelerateCar", 0f, 0.1f);
            deceleratingCar = true;
        }
        if (!turnLeft.ButtonPressed && !turnRight.ButtonPressed && carWheels.SteeringAxis != 0f)
        {
            carWheels.ResetSteeringAngle();
        }
    }
}
