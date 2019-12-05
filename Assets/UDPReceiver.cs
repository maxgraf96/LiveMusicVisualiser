using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

/**
 * This listens to a UDP port (currently 1236), parses the incoming data and triggers game actions accordingly.
 * */
public class UDPReceiver : MonoBehaviour
{
    // Lock 
    static readonly object lockObject = new object();
    // Stores the incoming UDP data
    byte[] returnData;
    // Guard for the processing of data
    // This makes sure each block of incoming UDP data is processed without interruption by another block
    bool processingData = false;
    // Variable for passing colors to game objects
    Color color = Color.red;

    // The UDP client
    static UdpClient udp;
    // The separate thread in which the UDP client is executed
    Thread thread;

    // Game objects
    private CameraBehavior cameraBehavior;
    private FreqSphere kickSphere;
    private Fractal fractalRoot;

    // Incoming data
    private float level;
    // Data from the left glove (flex sensors)
    private float flexThumb, flexIndex, flexRing;
    // Data from the right glove (piezos)
    private float drumThumb, drumMiddle, drumRing, drumPinky;

    // The manager class for the particle system and its force field
    public PSManager psManager;

    // Time scaling related fields (for slow down effect upon crash hit)
    private bool slowdownStarted = false;
    private float slowdownTimer;

    // Start is called before the first frame update
    void Start()
    {
        // Assign kick game object
        kickSphere = GameObject.FindGameObjectWithTag("freqSphereVisualiser").GetComponent<FreqSphere>();

        // Assign snare/hihat game object
        fractalRoot = GameObject.Find("Fractal").GetComponent<Fractal>();

        // Assign camera behaviour
        cameraBehavior = Camera.main.GetComponent<CameraBehavior>();

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
                // Ready for next block of incoming data
                processingData = false;

                // Trigger the sphere in the centre upon a kick hit
                drumThumb = returnData[11] / 127.0f;
                level = returnData[0] == 0.0f ? 0.0f : Mathf.Log10(returnData[0]);
                AutoMapper.updateF0(level);
                float colorVal = level / AutoMapper.F0;
                color = Color.HSVToRGB(colorVal, 1.0f, colorVal);
                kickSphere.ChangeColor(color);
                kickSphere.ChangeNoiseAmount(2 * colorVal);
                if (drumThumb > 0.07f && drumThumb > drumMiddle)
                {
                    // Trigger kick behaviour
                    color.a = 1f;
                    kickSphere.triggerImpact();

                    // Trigger fractal movement
                    fractalRoot.triggerKick();
                }

                // Trigger the particle system gravity loss upon snare hit
                drumMiddle = returnData[13] / 127.0f;
                if (drumMiddle > 0.07f)
                {
                    psManager.triggerSnare();
                }

                // Update the fractal
                drumMiddle = returnData[13] / 127f;
                level = returnData[1] == 0.0f ? 0.0f : Mathf.Log10(returnData[1]);
                
                //Update max value if higher (taken care of by AutoMapper)
                AutoMapper.updateF2(level);

                // Trigger fractal scaling if snare hits
                if (drumMiddle > 0.4f)
                {
                    color = Color.HSVToRGB(drumMiddle, 1.0f, 1f);
                    fractalRoot.ChangeColor(color);
                    fractalRoot.triggerImpact();
                }
                fractalRoot.setF4(level);

                // Ring finger (Hihat)
                drumRing = returnData[14] / 127f;
                if (drumRing > 0.1f)
                {
                    fractalRoot.triggerHihat();
                }

                // Pinky finger (crash)
                drumPinky = returnData[15] / 127f;
                if(drumPinky > 0.1f)
                {
                    // Slow game time down
                    triggerSlowdown();
                }

                // Slows down game upon crash impact
                slowDown();

                // Get values from flex-sensors and pass to camera
                // Scale values to 0...1
                flexIndex = returnData[8] / 127.0f;
                flexThumb = returnData[9] / 127.0f;
                flexRing = returnData[10] / 127.0f;

                // Set camera control parameters
                cameraBehavior.SetRotationValue(flexIndex); // Rotation around y-axis
                cameraBehavior.SetRotation2Value(flexThumb); // Rotation around x/z-axis
                cameraBehavior.SetZoom(flexRing); // Zoom in reference to center of scene
            }
        }
    }

    // This method is created in a new thread and is responsible for listening for incoming data, then passing it to the
    // main thread for further processing.
    private void ThreadMethod()
    {
        // Create a new UDP client and start listening on port 1236
        udp = new UdpClient(1236);
        // Listen indefinitely
        while (true)
        {
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

            // Read data from UDP
            byte[] receiveBytes = udp.Receive(ref RemoteIpEndPoint);

            /*
            * Lock object to make sure there data is 
            * not being accessed from multiple threads at the same time
            */
            lock (lockObject)
            {
                // Assign received data
                returnData = receiveBytes; 
                processingData = true;
            }
        }
    }

    // Slow down the game scene upon crash hit
    private void slowDown()
    {
        // Only trigger if the process is currently not ongoing
        if (slowdownStarted)
        {
            // Update timer
            slowdownTimer += Time.deltaTime;

            // If time is up (1s in this case) reset everything and set time to normal speed
            if (slowdownTimer >= 1f)
            {
                // Reset timer
                slowdownTimer = 0f;
                slowdownStarted = false;
                Time.timeScale = 1f; // Normal game speed
            } else
            {
                // Gradually increase the game speed
                Time.timeScale = 0.6f + slowdownTimer * 1.5f;
            }
        }
        

    }

    // Trigger slow down upon crash hit
    public void triggerSlowdown()
    {
        if (!slowdownStarted) slowdownStarted = true;
    }
}
