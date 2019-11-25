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

    public GameObject sphere, plane;
    public Camera camera;
    private CameraBehavior cameraBehavior;
    private InitBehaviour initBehaviour;
    private List<GameObject> freqSpheres = new List<GameObject>();
    private GameObject myClouds;
    private List<FreqSphereBehaviour> freqSphereBehaviours = new List<FreqSphereBehaviour>();
    private MyCloudBehaviour myCloudBehaviour;
    private float level, spectralCentroid;
    private float flexThumb, flexIndex, flexRing;

    // Start is called before the first frame update
    void Start()
    {
        sphere = GameObject.Find("Sphere");
        myClouds = GameObject.Find("MyClouds");

        initBehaviour = sphere.GetComponent<InitBehaviour>();
        myCloudBehaviour = myClouds.GetComponent<MyCloudBehaviour>();

        cameraBehavior = camera.GetComponent<CameraBehavior>();

        // Cubes
        freqSpheres.AddRange(GameObject.FindGameObjectsWithTag("freqSphereVisualiser"));
        freqSpheres.ForEach(freqSphere => {
            freqSphereBehaviours.Add(freqSphere.GetComponent<FreqSphereBehaviour>());
        });

        // Hide sphere
        sphere.GetComponent<Renderer>().enabled = false;

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

                // Convert bytes
                // Structure is 7 x level values => 7 bytes
                // Coming in as an unsigned byte []
                float current = 0.0f;
                float max = 10 * Mathf.Log10(returnData.Max());
                if(max == -Mathf.Infinity)
                {
                    max = 1; // Else we divide by 0 :(
                }
                for(int i = 0; i < 7; i++)
                {
                    current = returnData[i];
                    level = current == 0.0f ? 0.0f : Mathf.Log10(current);
                    float colorVal = 10 * level / max;
                    color = Color.HSVToRGB(colorVal, 1.0f, colorVal);

                    freqSphereBehaviours[i].ChangeColor(freqSpheres[i], color);
                    freqSphereBehaviours[i].ChangeNoiseAmount(freqSpheres[i], level);
                }

                spectralCentroid = 1.5f * Mathf.Log10(returnData[7]) / Mathf.Log10(22050);
                spectralCentroid = Mathf.Clamp01(spectralCentroid);
                plane.GetComponent<Renderer>().material.color = Color.HSVToRGB(spectralCentroid, 1.0f, spectralCentroid);

                // Get values from flex-sensors and apply to camera

                flexIndex = returnData[8] / 127.0f;
                flexThumb = returnData[9] / 127.0f;
                flexRing = returnData[10] / 127.0f;
                cameraBehavior.SetRotationValue(flexIndex);
                cameraBehavior.SetRotation2Value(flexThumb);
                cameraBehavior.SetZoom(flexRing);

                // Change MyClouds
                float bassVal = returnData[0];
                float threshold = 170.0f;
                if (bassVal > threshold)
                {
                    level = Mathf.Log10(bassVal);
                    float colorVal = 10 * level / max;
                    color = Color.HSVToRGB(colorVal, 1.0f, colorVal);
                    color.a = 1.0f;
                    myCloudBehaviour.ChangeBass(myClouds, bassVal / threshold);
                    myCloudBehaviour.ChangeColour(myClouds, color);

                } else
                {
                    myCloudBehaviour.ChangeBass(myClouds, 0.0f);
                    color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
                    myCloudBehaviour.ChangeColour(myClouds, color);
                }
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
