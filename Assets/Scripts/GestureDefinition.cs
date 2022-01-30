using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GestureDefinition : MonoBehaviour
{
    public enum FingerStates {OPEN, CLOSE};

    public string gestureName;
    public Image gestureIcon;

    public FingerStates firstFinger;
    public FingerStates secondFinger;
    public FingerStates thirdFinger;
    public FingerStates fourthFinger;
    public FingerStates fifthFinger;
}
