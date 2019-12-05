using UnityEngine;

/**
 * Custom behaviour for the main camera. There are two modes:
 *      1) Free flight (set by the bool "freeFlight"): Control the camera using the WASD keys on the keyboard.
 *      Look around by using the mouse/trackpad. This behaviour was adapted from https://gist.github.com/0606/20566973ed23083e0772491777de162b
 *      2) Glove control: If "freeFlight" is set to false, the camera is controlled by the left glove.
 *      Thumb movements control the height (y-axis), index finger movements rotate the camera around the centre of the scene
 *      (where all game objects are located) and ring fing finger movements control the zoom of the camera, i.e. distance to
 *      the centre of the scene.
 * */
public class CameraBehavior : MonoBehaviour
{
    // Base camera speed
    float mainSpeed = 100.0f;
    // Camera sensitivity to mouse movement
    float camSens = 0.1f;
    // Stores the last mouse position
    private Vector3 lastMouse = new Vector3(255, 255, 255);
    // False for center camera on the scene centre, true for freely moving around via mouse and WASD
    public bool freeFlight = false;
    // The invisible sphere in the middle of the world, used for initialising and setting various behaviours
    public GameObject trackingSphere;
    // Incoming data to control the camera (from UDP)
    // Rotation values for the y and xz-axes
    private float iRotation, iRotation2;
    // Rotation sensitivity
    private float rotationVelocity;
    // How smooth the camera movements are
    private float smoothTime = 0.03f;

    // Rotational angle for the y-axis
    float angle = 0;
    // How quickly to rotate
    float speed = 2 * Mathf.PI;
    // Distance from camera to trackingSphere
    float radius = 15;
    // Storage for camera position values
    float x, y, z;

    void Start()
    {
        // Hide mouse cursor
        Cursor.visible = false;
    }

    void Update()
    {
        // Differentiate between free flight and glove control
        if (freeFlight)
        {
            lastMouse = Input.mousePosition - lastMouse;
            lastMouse = new Vector3(-lastMouse.y * camSens, lastMouse.x * camSens, 0);
            lastMouse = new Vector3(transform.eulerAngles.x + lastMouse.x, transform.eulerAngles.y + lastMouse.y, 0);
            transform.eulerAngles = lastMouse;
            lastMouse = Input.mousePosition;
            // Mouse  camera angle done.  

            // Get keyboard commands
            Vector3 p = GetBaseInput();
            p = p * mainSpeed;

            p = p * Time.deltaTime;
            transform.Translate(p);
        } else
        {
            // Look at centre
            transform.LookAt(trackingSphere.transform);

            // Basically rotation around unit circle scaled up to radius 15
            // - Mathf.PI / 2 because initial camera position is not at re = 15, im = 0
            // But at re = 0, im = -15 speaking in terms of imaginary numbers on the not-quite-unit circle
            angle = speed * iRotation - Mathf.PI / 2;
            x = Mathf.SmoothDamp(transform.position.x, Mathf.Cos(angle) * radius, ref rotationVelocity, smoothTime);
            z = Mathf.SmoothDamp(transform.position.z, Mathf.Sin(angle) * radius, ref rotationVelocity, smoothTime);

            // Height of camera (in terms of y-axis)
            y = Mathf.SmoothDamp(transform.position.y, map(iRotation2, 0f, 1f, -1f, 5f), ref rotationVelocity, smoothTime);

            // Update camera position
            transform.position = new Vector3(x, y, z);
            // Necessary to keep camera in place after transformations
            transform.LookAt(trackingSphere.transform);
        }
    }

    // Set rotation values for the xz-axes
    public void SetRotationValue(float rotation)
    {
        iRotation = rotation;
    }

    // Set rotation value for the y-axis
    public void SetRotation2Value(float rotation2)
    {
        iRotation2 = rotation2;
    }

    // Set and map value for the zoom level
    public void SetZoom(float zoom)
    {
        radius = map(zoom, 0f, 1f, 6f, 3f);
    }

    // Maps a value in a given range to a given range
    private static float map(float value, float fromLow, float fromHigh, float toLow, float toHigh)
    {
        return (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow) + toLow;
    }

    // Read keyboard input (only for free flight)
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