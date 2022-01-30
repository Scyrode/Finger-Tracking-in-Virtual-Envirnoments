using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class HandBehaviour : MonoBehaviour
{
    // finger states: '0' = OPEN, '1' = CLOSE

    public float animationLength = 0.1f;
    public float animationStepDuration = 0.01f;

    public float firstJointRotation = -90.0f;
    public float secondJointRotation = -30.0f;
    public float thirdJointRotation = -45.0f;

    public GameObject[] fingers;

    private float firstJointRotationScaleFactor;
    private float secondJointRotationScaleFactor;
    private float thirdJointRotationScaleFactor;

    private float accelX;
    private float accelY;
    private float accelZ;
            
    private float gyroX;
    private float gyroY;
    private float gyroZ;
            
    private float gyroOffsetX;
    private float gyroOffsetY;
    private float gyroOffsetZ;

    private float accelOffsetX;
    private float accelOffsetY;
    private float accelOffsetZ;

    private bool completedSetup = false;

    // set to true to enable motion tracking
    private bool motionTracking = false;

    // set to true to enable arduino tracking
    private bool withArduino = true;

    private int[] currentFingerStates = new int[5];

    //private Rigidbody rb;

    private SerialPort sp;

    private CurrentGestureController currentGestureController;
    private GestureDefinition currentGesture;

    private GameController gameController;

    private AudioSource GestureMadeSound;

    private GameObject[][] joints;

    void Start()
    {
        GestureMadeSound = GetComponent<AudioSource>();
        gameController = FindObjectOfType<GameController>();
        currentGestureController = FindObjectOfType<CurrentGestureController>();
        currentGesture = currentGestureController.getRandomGestureDefinition();

        //rb = GetComponent<Rigidbody>();

        for (int i = 0; i < currentFingerStates.Length; i++)
        {
            currentFingerStates[i] = 0;
        }

        joints = new GameObject[fingers.Length][];

        for (int i = 0; i < fingers.Length; i++)
        {
            joints[i] = new GameObject[3];

            var temp = fingers[i];

            for (int j = 0; j < 3; j++)
            {
                temp = temp.transform.GetChild(0).gameObject;
                joints[i][j] = temp;
            }
        }

        firstJointRotationScaleFactor = Mathf.Lerp(0, firstJointRotation, animationStepDuration / animationLength);
        secondJointRotationScaleFactor = Mathf.Lerp(0, secondJointRotation, animationStepDuration / animationLength);
        thirdJointRotationScaleFactor = Mathf.Lerp(0, thirdJointRotation, animationStepDuration / animationLength);

        if (withArduino)
        {
            //yield return new WaitForSeconds(1);
            sp = new SerialPort("COM7", 38400);
            sp.Open();
            sp.ReadTimeout = 2000;

            if (sp.IsOpen && motionTracking)
            {
                string[] tokens = sp.ReadLine().Split(',');

                gyroOffsetX = float.Parse(tokens[5]);
                gyroOffsetY = float.Parse(tokens[6]);
                gyroOffsetZ = float.Parse(tokens[7]);

                // accelOffset calculations assume the hand is resting almost entirely flat on any of the axis,
                // meaning the magnitude of the gravity component is greater than 1g on at least one of the axis.
                accelOffsetX = float.Parse(tokens[8]);
                if (accelOffsetX > 1.0f)
                {
                    accelOffsetX -= 1.0f;
                }
                else if (accelOffsetX < -1.0f)
                {
                    accelOffsetX += 1.0f;
                }

                accelOffsetY = float.Parse(tokens[9]);
                if (accelOffsetY > 1.0f)
                {
                    accelOffsetY -= 1.0f;
                }
                else if (accelOffsetY < -1.0f)
                {
                    accelOffsetY += 1.0f;
                }

                accelOffsetZ = float.Parse(tokens[10]);
                if (accelOffsetZ > 1.0f)
                {
                    accelOffsetZ -= 1.0f;
                }
                else if (accelOffsetZ < -1.0f)
                {
                    accelOffsetZ += 1.0f;
                }

                completedSetup = true;
            }
        }
    }

    private void Update()
    {
        if (withArduino && sp.IsOpen)
        {
            string readLine = sp.ReadLine();
            string[] tokens = readLine.Split(',');

            // Finger Tracking

            // check if any finger was just closed (read state '1') & is currently open (read state '0')
            if (tokens[0] == "1" && currentFingerStates[0] == 0)
            {
                currentFingerStates[0] = 1;
                StartCoroutine(AnimateFinger(joints[0], 1));
                if (IsGestureMade())
                {
                    UpdateCurrentGesture();
                }
            }

            if (tokens[1] == "1" && currentFingerStates[1] == 0)
            {
                currentFingerStates[1] = 1;
                StartCoroutine(AnimateFinger(joints[1], 1));
                if (IsGestureMade())
                {
                    UpdateCurrentGesture();
                }
            }

            if (tokens[2] == "1" && currentFingerStates[2] == 0)
            {
                currentFingerStates[2] = 1;
                StartCoroutine(AnimateFinger(joints[2], 1));
                if (IsGestureMade())
                {
                    UpdateCurrentGesture();
                }
            }

            if (tokens[3] == "1" && currentFingerStates[3] == 0)
            {
                currentFingerStates[3] = 1;
                StartCoroutine(AnimateFinger(joints[3], 1));
                if (IsGestureMade())
                {
                    UpdateCurrentGesture();
                }
            }

            if (tokens[4] == "1" && currentFingerStates[4] == 0)
            {
                currentFingerStates[4] = 1;
                StartCoroutine(AnimateFinger(joints[4], 1));
                if (IsGestureMade())
                {
                    UpdateCurrentGesture();
                }
            }

            // check if any finger was just opened (read state '0') & is currently closed (read state '1')
            if (tokens[0] == "0" && currentFingerStates[0] == 1)
            {
                currentFingerStates[0] = 0;
                StartCoroutine(AnimateFinger(joints[0], 0));
                if (IsGestureMade())
                {
                    UpdateCurrentGesture();
                }
            }

            if (tokens[1] == "0" && currentFingerStates[1] == 1)
            {
                currentFingerStates[1] = 0;
                StartCoroutine(AnimateFinger(joints[1], 0));
                if (IsGestureMade())
                {
                    UpdateCurrentGesture();
                }
            }

            if (tokens[2] == "0" && currentFingerStates[2] == 1)
            {
                currentFingerStates[2] = 0;
                StartCoroutine(AnimateFinger(joints[2], 0));
                if (IsGestureMade())
                {
                    UpdateCurrentGesture();
                }
            }

            if (tokens[3] == "0" && currentFingerStates[3] == 1)
            {
                currentFingerStates[3] = 0;
                StartCoroutine(AnimateFinger(joints[3], 0));
                if (IsGestureMade())
                {
                    UpdateCurrentGesture();
                }
            }

            if (tokens[4] == "0" && currentFingerStates[4] == 1)
            {
                currentFingerStates[4] = 0;
                StartCoroutine(AnimateFinger(joints[4], 0));
                if (IsGestureMade())
                {
                    UpdateCurrentGesture();
                }
            }

            if (completedSetup && motionTracking)
            {
                gyroX = float.Parse(tokens[5]);
                gyroX -= gyroOffsetX;
                //gyroX /= 20;

                gyroY = float.Parse(tokens[6]);
                gyroY -= gyroOffsetY;
                //gyroY /= 20;

                gyroZ = float.Parse(tokens[7]);
                gyroZ -= gyroOffsetZ;
                //gyroZ /= 20;

                // 6-Axis Tracking
                accelX = float.Parse(tokens[8]);
                accelX -= accelOffsetX;
                //accelX /= 20;

                accelY = float.Parse(tokens[9]);
                accelY -= accelOffsetY;
                //accelY /= 20;

                accelZ = float.Parse(tokens[10]);
                accelZ -= accelOffsetZ;
                //accelZ /= 20;

                var gyroVector = new Vector3(gyroY, gyroZ, gyroX) * Time.deltaTime;

                gyroVector.x = Mathf.Abs(gyroVector.x) > 0.03f ? gyroVector.x : 0;
                gyroVector.y = Mathf.Abs(gyroVector.y) > 0.03f ? gyroVector.y : 0;
                gyroVector.z = Mathf.Abs(gyroVector.z) > 0.03f ? gyroVector.z : 0;

                gameObject.transform.Rotate(gyroVector);


                var accelVector = new Vector3(accelY, accelZ, accelX);

                accelVector.x = Mathf.Abs(accelVector.x) > 0.1f ? accelVector.x : 0;
                accelVector.y = Mathf.Abs(accelVector.y) > 0.1f ? accelVector.y : 0;
                accelVector.z = Mathf.Abs(accelVector.z) > 0.1f ? accelVector.z : 0;

                accelVector *= 10.0f;

                /*if (accelVector.magnitude - Physics.gravity.magnitude < 0.5f)
                {
                    accelVector = accelVector.normalized * 10.0f;
                }*/

                //Debug.Log(accelVector);

                //transform.Translate(accelVector * Time.deltaTime, Space.Self);
                //transform.Translate(Physics.gravity * Time.deltaTime);

                //var magnitude = accelVector.magnitude;
                //magnitude -= 1.0f;

                //accelVector = accelVector.normalized * magnitude;

                //var subtractionVector = transform.InverseTransformDirection(-Vector3.up);
                //accelVector += subtractionVector;

                //transform.Translate(-Vector3.up);

                //transform.Translate(accelVector, Space.Self);

                /*if (Mathf.Abs(gyroVector.x) > 0.03f || Mathf.Abs(gyroVector.y) > 0.03f || Mathf.Abs(gyroVector.z) > 0.03f)
                {
                    gameObject.transform.Rotate(gyroVector);
                }*/
            }
        } else if (!withArduino)
        {
            // check if any finger was just closed (read state '1') & is currently open (read state '0')
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                currentFingerStates[0] = 1;
                StartCoroutine(AnimateFinger(joints[0], 1));
                if (IsGestureMade())
                {
                    UpdateCurrentGesture();
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                currentFingerStates[1] = 1;
                StartCoroutine(AnimateFinger(joints[1], 1));
                if (IsGestureMade())
                {
                    UpdateCurrentGesture();
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                currentFingerStates[2] = 1;
                StartCoroutine(AnimateFinger(joints[2], 1));
                if (IsGestureMade())
                {
                    UpdateCurrentGesture();
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                currentFingerStates[3] = 1;
                StartCoroutine(AnimateFinger(joints[3], 1));
                if (IsGestureMade())
                {
                    UpdateCurrentGesture();
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                currentFingerStates[4] = 1;
                StartCoroutine(AnimateFinger(joints[4], 1));
                if (IsGestureMade())
                {
                    UpdateCurrentGesture();
                }
            }

            // check if any finger was just opened (read state '0') & is currently closed (read state '1')
            if (Input.GetKeyUp(KeyCode.Alpha1))
            {
                currentFingerStates[0] = 0;
                StartCoroutine(AnimateFinger(joints[0], 0));
                if (IsGestureMade())
                {
                    UpdateCurrentGesture();
                }
            }

            if (Input.GetKeyUp(KeyCode.Alpha2))
            {
                currentFingerStates[1] = 0;
                StartCoroutine(AnimateFinger(joints[1], 0));
                if (IsGestureMade())
                {
                    UpdateCurrentGesture();
                }
            }

            if (Input.GetKeyUp(KeyCode.Alpha3))
            {
                currentFingerStates[2] = 0;
                StartCoroutine(AnimateFinger(joints[2], 0));
                if (IsGestureMade())
                {
                    UpdateCurrentGesture();
                }
            }

            if (Input.GetKeyUp(KeyCode.Alpha4))
            {
                currentFingerStates[3] = 0;
                StartCoroutine(AnimateFinger(joints[3], 0));
                if (IsGestureMade())
                {
                    UpdateCurrentGesture();
                }
            }

            if (Input.GetKeyUp(KeyCode.Alpha5))
            {
                currentFingerStates[4] = 0;
                StartCoroutine(AnimateFinger(joints[4], 0));
                if (IsGestureMade())
                {
                    UpdateCurrentGesture();
                }
            }
        }
    }

    private bool IsGestureMade()
    {
        bool isGestureMade = true;

        if ((int) currentGesture.firstFinger != currentFingerStates[0])
        {
            isGestureMade = false;
        }
        if ((int)currentGesture.secondFinger != currentFingerStates[1])
        {
            isGestureMade = false;
        }
        if ((int)currentGesture.thirdFinger != currentFingerStates[2])
        {
            isGestureMade = false;
        }
        if ((int)currentGesture.fourthFinger != currentFingerStates[3])
        {
            isGestureMade = false;
        }
        if ((int)currentGesture.fifthFinger != currentFingerStates[4])
        {
            isGestureMade = false;
        }

        return isGestureMade;
    }

    private void UpdateCurrentGesture()
    {
        if (gameController.gameIsOn)
        {
            currentGesture = currentGestureController.getRandomGestureDefinition();
            GestureMadeSound.Play();
            gameController.IncrementScore();
        }
    }

    /*private void FixedUpdate()
    {
        if (completedSetup)
        {

            //Debug.Log(Vector3.Scale(accelVector, -Physics.gravity));

            rb.AddForce(accelVector * 10.0f);
            rb.AddForce(Physics.gravity);

            //prevAccelX = accelX;
            //prevAccelY = accelY;
            //prevAccelZ = accelZ;

            *//*if (Mathf.Abs(accelVector.x) > 0.1f || Mathf.Abs(accelVector.y) > 0.1f || Mathf.Abs(accelVector.z) > 0.1f)
            {
                gameObject.transform.Translate(accelVector);
            }*//*
        }
    }*/

    IEnumerator AnimateFinger(GameObject[] joints, int state)
    {
        if (state == 1)
        {
            /*joints[0].transform.localRotation = Quaternion.identity;
            joints[1].transform.localRotation = Quaternion.identity;
            joints[2].transform.localRotation = Quaternion.identity;*/

            for (float i = 0; i < animationLength; i += animationStepDuration)
            {
                joints[0].transform.Rotate(Vector3.right * firstJointRotationScaleFactor);
                joints[1].transform.Rotate(Vector3.right * secondJointRotationScaleFactor);
                joints[2].transform.Rotate(Vector3.right * thirdJointRotationScaleFactor);
                yield return new WaitForSeconds(animationStepDuration);
            }

            /*joints[0].transform.localRotation = Quaternion.Euler(firstJointRotation, 0, 0);
            joints[1].transform.localRotation = Quaternion.Euler(secondJointRotation, 0, 0);
            joints[2].transform.localRotation = Quaternion.Euler(thirdJointRotation, 0, 0);*/
        } else
        {
            /*joints[0].transform.localRotation = Quaternion.Euler(firstJointRotation, 0, 0);
            joints[1].transform.localRotation = Quaternion.Euler(secondJointRotation, 0, 0);
            joints[2].transform.localRotation = Quaternion.Euler(thirdJointRotation, 0, 0);*/

            for (float i = 0; i < animationLength; i += animationStepDuration)
            {
                joints[0].transform.Rotate(-Vector3.right * firstJointRotationScaleFactor);
                joints[1].transform.Rotate(-Vector3.right * secondJointRotationScaleFactor);
                joints[2].transform.Rotate(-Vector3.right * thirdJointRotationScaleFactor);
                yield return new WaitForSeconds(animationStepDuration);
            }

            /*joints[0].transform.localRotation = Quaternion.identity;
            joints[1].transform.localRotation = Quaternion.identity;
            joints[2].transform.localRotation = Quaternion.identity;*/
        }
    }
}
