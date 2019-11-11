using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
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

    public GameObject sphere;
    private SphereBehavior sphereBehavior;
    private List<GameObject> cubes = new List<GameObject>();
    private List<CubeBehavior> cubeBehaviors = new List<CubeBehavior>();
    private int counter, red, green, blue;

    // Start is called before the first frame update
    void Start()
    {
        sphere = GameObject.Find("Sphere");
        sphereBehavior = sphere.GetComponent<SphereBehavior>();

        // Cubes
        cubes.AddRange(GameObject.FindGameObjectsWithTag("cubeVisualiser"));
        cubes.ForEach(cube => {
            cubeBehaviors.Add(cube.GetComponent<CubeBehavior>());
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
                // Structure is 7 * rgb => 21 bytes
                // Coming in as an unsigned byte []
                counter = 0;
                for(int i = 0; i < 7; i++)
                {
                    red = returnData[counter++];
                    green = returnData[counter++];
                    blue = returnData[counter++];

                    color = new Color(red, green, blue);
                    cubeBehaviors[i].ChangeColor(cubes[i], color);
                    cubeBehaviors[i].ChangeHeight(cubes[i], red);
                }

                //Debug.Log(cubeIdx.ToString());
                //Debug.Log(color.r.ToString() + " " + color.g + " " + color.b);

                //sphereBehavior.ChangeColor(sphere, color);
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
