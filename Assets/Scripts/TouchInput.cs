using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchInput : MonoBehaviour
{
    public bool ButtonPressed { get; private set; }
    private RectTransform rectTransform;
    private Vector3 initialScale;
    private float scaleDownMultiplier = 0.85f;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        initialScale = rectTransform.localScale;
    }

    public void ButtonDown()
    {
        ButtonPressed = true;
        rectTransform.localScale = initialScale * scaleDownMultiplier;
    }

    public void ButtonUp()
    {
        ButtonPressed = false;
        rectTransform.localScale = initialScale;
    }
}
