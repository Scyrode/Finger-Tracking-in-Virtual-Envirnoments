using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CurrentGestureController : MonoBehaviour
{
    private Image gestureIcon;
    public TextMeshProUGUI gestureName;

    public GestureDefinition[] gestureDefinitions;

    private void Start()
    {
        gestureIcon = GetComponent<Image>();
    }

    public GestureDefinition getRandomGestureDefinition()
    {
        GestureDefinition currentGesture = gestureDefinitions[Random.Range(0, gestureDefinitions.Length)];

        gestureIcon.sprite = currentGesture.gestureIcon.sprite;
        gestureIcon.SetNativeSize();

        gestureName.text = currentGesture.gestureName;

        return currentGesture;
    }
}
