using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class UDPReceiver : MonoBehaviour
{
    static readonly object lockObject = new object();
    byte[] returnData;
    bool processingData = false;
    Color color = Color.red;

    static UdpClient udp;
    Thread thread;

    // Game objects
    public Camera camera;
    private CameraBehavior cameraBehavior;
    private GameObject kickSphere;
    private FreqSphereBehaviour kickSphereBehaviour;
    private Fractal fractalRoot;

    // Incoming data
    private float level, spectralCentroid;
    private float flexThumb, flexIndex, flexRing;
    private float drumThumb, drumIndex, drumMiddle, drumRing, drumPinky;

    public PSManager psManager;

    // Start is called before the first frame update
    void Start()
    {
        // Kick related - freqSpheres
        kickSphere = GameObject.FindGameObjectWithTag("freqSphereVisualiser");
        kickSphereBehaviour = kickSphere.GetComponent<FreqSphereBehaviour>();

        // Snare related
        fractalRoot = GameObject.Find("Fractal").GetComponent<Fractal>();

        // Camera behaviour
        cameraBehavior = camera.GetComponent<CameraBehavior>();

        // Start UDP receiver in separate thread
        thread = new Thread(new ThreadStart(ThreadMethod));
        thread.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (processingData)
        {
            /*lock object to make sure there data is 
             *not being accessed from multiple threads at thesame time*/
            lock (lockObject)
            {
                processingData = false;

                // Change kickSphere
                drumThumb = returnData[11] / 127.0f;
                level = returnData[0] == 0.0f ? 0.0f : Mathf.Log10(returnData[0]);
                AutoMapper.updateF0(level);
                float colorVal = 20 * level / AutoMapper.F0;
                color = Color.HSVToRGB(colorVal, 1.0f, colorVal);
                kickSphereBehaviour.ChangeColor(kickSphere, color);
                kickSphereBehaviour.ChangeNoiseAmount(kickSphere, level / AutoMapper.F0);
                if (drumThumb > 0.1f)
                {
                    // Trigger kick behaviour
                    color.a = 1f;
                    kickSphereBehaviour.triggerImpact();

                    // Trigger fractal movement
                    fractalRoot.triggerKick();
                }

                drumMiddle = returnData[13] / 127.0f;
                if (drumMiddle > 0.1f)
                {
                    psManager.triggerSnare();
                }

                // Update the fractal
                drumMiddle = returnData[13] / 127f;
                level = returnData[1] == 0.0f ? 0.0f : Mathf.Log10(returnData[1]);
                //Update max value if higher (taken care of by AutoMapper)
                AutoMapper.updateF2(level);
                // Update fade level(alpha value)
                fractalRoot.updateFade(level, AutoMapper.F2);
                //fractalRoot.updateFade(1, 1);
                // Trigger fractal scaling if snare hits
                if (drumMiddle > 0.2f)
                {
                    color = Color.HSVToRGB(drumMiddle, 1.0f, 1f);
                    fractalRoot.ChangeColor(color);
                    fractalRoot.triggerImpact();
                }

                // Ring finger
                drumRing = returnData[14] / 127f;
                if (drumRing > 0f)
                {
                    fractalRoot.triggerHihat();
                }

                // Get values from flex-sensors and pass to camera
                // Scale values to 0...1
                flexIndex = returnData[8] / 127.0f;
                flexThumb = returnData[9] / 127.0f;
                flexRing = returnData[10] / 127.0f;

                // Set camera control parameters
                cameraBehavior.SetRotationValue(flexIndex); // Rotation around y-axis
                cameraBehavior.SetRotation2Value(flexThumb); // Rotation around x/z-axis
                cameraBehavior.SetZoom(flexRing); // Zoom in reference to center of scene

                //// Spectral centroid
                //spectralCentroid = 1.5f * Mathf.Log10(returnData[7]) / Mathf.Log10(22050);
                //spectralCentroid = Mathf.Clamp01(spectralCentroid);
            }
        }
    }

    private void ThreadMethod()
    {
        udp = new UdpClient(1236);
        while (true)
        {
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

            byte[] receiveBytes = udp.Receive(ref RemoteIpEndPoint);

            /*lock object to make sure there data is 
            *not being accessed from multiple threads at thesame time*/
            lock (lockObject)
            {
                returnData = receiveBytes; 
                processingData = true;
            }
        }
    }
}
