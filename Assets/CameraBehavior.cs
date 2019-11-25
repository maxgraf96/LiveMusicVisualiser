﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{

    float mainSpeed = 100.0f; //regular speed
    float camSens = 0.1f; //How sensitive it with mouse
    private Vector3 lastMouse = new Vector3(255, 255, 255); //kind of in the middle of the screen, rather than at the top (play)
    private float totalRun = 1.0f;
    // False for center camera on sphere in the middle, true for freely moving around via mouse and WASD
    public bool freeFlight = false;
    // The invisible sphere in the middle of the world
    public GameObject trackingSphere;
    private float incomingRotation, rotationDelta, prevRotation, prevRotationDelta;
    private float rotationVelocity;
    private float smoothTime = 0.03f;

    // Rotational angle for the y-axis
    float angle = 0;
    float speed = 2 * Mathf.PI;
    float radius = 15; // Distance from camera to trackingSphere

    void Start()
    {
        // Hide mouse cursor
        Cursor.visible = false;
    }

    void Update()
    {
        if (freeFlight)
        {
            lastMouse = Input.mousePosition - lastMouse;
            lastMouse = new Vector3(-lastMouse.y * camSens, lastMouse.x * camSens, 0);
            lastMouse = new Vector3(transform.eulerAngles.x + lastMouse.x, transform.eulerAngles.y + lastMouse.y, 0);
            transform.eulerAngles = lastMouse;
            lastMouse = Input.mousePosition;
            //Mouse  camera angle done.  

            //Keyboard commands
            Vector3 p = GetBaseInput();
            p = p * mainSpeed;

            p = p * Time.deltaTime;
            Vector3 newPosition = transform.position;
            if (Input.GetKey(KeyCode.Space))
            { //If player wants to move on X and Z axis only
                transform.Translate(p);
                newPosition.x = transform.position.x;
                newPosition.z = transform.position.z;
                transform.position = newPosition;
            }
            else
            {
                transform.Translate(p);
            }
        } else
        {
            transform.LookAt(trackingSphere.transform);

            // Basically rotation around unit circle scaled up to radius 15
            // - Mathf.PI / 2 because initial camera position is not at re = 15, im = 0
            // But at re = 0, im = -15 speaking in terms of imaginary numbers on the not-quite-unit circle
            angle = speed * incomingRotation - Mathf.PI / 2;
            float x = Mathf.SmoothDamp(transform.position.x, Mathf.Cos(angle) * radius, ref rotationVelocity, smoothTime);
            float z = Mathf.SmoothDamp(transform.position.z, Mathf.Sin(angle) * radius, ref rotationVelocity, smoothTime);
            transform.position = new Vector3(x, 0, z);
            transform.LookAt(trackingSphere.transform);
        }
    }

    public void SetRotationValue(float rotation)
    {
        incomingRotation = rotation;
    }

    private static float map(float value, float fromLow, float fromHigh, float toLow, float toHigh)
    {
        return (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow) + toLow;
    }

    private Vector3 GetBaseInput()
    {
        //returns the basic values, if it's 0 than it's not active.
        Vector3 p_Velocity = new Vector3();

        if (freeFlight)
        {
            if (Input.GetKey(KeyCode.W))
            {
                p_Velocity += new Vector3(0, 0, 0.1f);
            }
            if (Input.GetKey(KeyCode.A))
            {
                p_Velocity += new Vector3(-0.1f, 0, 0);
            }
            if (Input.GetKey(KeyCode.S))
            {
                p_Velocity += new Vector3(0, 0, -0.1f);
            }
            if (Input.GetKey(KeyCode.D))
            {
                p_Velocity += new Vector3(0.1f, 0, 0);
            }
            return p_Velocity;
        } else
        {
            return p_Velocity;
        }
    }
}