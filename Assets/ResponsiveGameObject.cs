using UnityEngine;

/*
 * Base class for custom game objects that can be affected by the incoming UDP data.
 * This includes transformations like scaling and rotation as well as color/surface effects like fading the game objects.
 * All of the effects are designed for impact-like actions, i.e. they are triggered at some point and then perform a certain
 * action over a certain amount of time. This decision was made to translate the playing of drums (triggering sounds) to visual objects.
 * The particular settings for the transformations are exposed as public fields, which enables them to be set directly from inside
 * the Unity editor. This makes for an easy way of tweaking and customising the effects.
 * 
 * IMPORTANT NOTE: Not all of the methods declared here are currently in use as some have been written for trying out different
 * styles and looks for the visual effects. However, as I plan to continue working on this project they have been kept in the 
 * code so as to keep them for further development.
 * */
public class ResponsiveGameObject : MonoBehaviour
{
    // Mesh and Material
    public Mesh mesh;
    public Material material;

    // Timer for trigger events
    // Scaling
    internal bool scaleTimerStarted = false;
    internal float scaleTimer = 0f;
    public float scaleTimeInSeconds;
    public Vector3 originalScale;
    public float scaleProportion; // To what size of the original size the fractal should be increased

    // Rotation
    internal bool rotTimerStarted = false;
    internal float rotTimer = 0f;
    public float rotTimeInSeconds;
    public float rotDegrees; // How far the fractal should be rotated on impact
    internal Quaternion currentRotation;

    // Fading
    internal bool fadeTimerStarted = false;
    internal float fadeTimer = 0f;
    public float fadeTimeInSeconds;
    internal float currentFadeValue;

    // Start is called before the first frame update
    internal void Start() {
        // Set the current rotation once
        currentRotation = transform.localRotation;
    }

    // Update is called once per frame
    internal void Update() {

    }

    // Scale the game object by a certain value.
    internal void scale()
    {
        if (scaleTimerStarted)
        {
            // Update timer
            scaleTimer += Time.deltaTime;

            if (scaleTimer >= scaleTimeInSeconds)
            {
                // Reset timer
                scaleTimer = 0f;
                scaleTimerStarted = false;
                // Fadeout
                triggerFadeOut();
            }
            else
            {
                if (scaleTimer <= scaleTimeInSeconds / 2f)
                {
                    // Scale up
                    transform.localScale = Vector3.Lerp(originalScale, originalScale * scaleProportion,
                        map(scaleTimer, 0f, scaleTimeInSeconds, 0f, 1f));
                }
                else
                {
                    // Scale back down
                    transform.localScale = Vector3.Lerp(originalScale, originalScale * scaleProportion,
                        map(scaleTimer, 0f, scaleTimeInSeconds, 1f, 0f));
                }
            }
        }
    }

    // Rotate the object around the y axis
    internal void rotate()
    {
        if (rotTimerStarted)
        {
            // Update timer
            rotTimer += Time.deltaTime;

            if (rotTimer >= rotTimeInSeconds)
            {
                // Reset timer
                rotTimer = 0f;
                rotTimerStarted = false;
                currentRotation = transform.localRotation;
            }
            else
            {
                // Rotate
                float process = map(rotTimer, 0f, rotTimeInSeconds, 0f, 1f);
                // Lerp produces a linear interpolation between two values which generates a smoother transition
                transform.localRotation = Quaternion.Lerp(currentRotation, Quaternion.Euler(
                    transform.localRotation.eulerAngles.x,
                    currentRotation.eulerAngles.y + rotDegrees,
                    transform.localRotation.eulerAngles.z), process);
            }
        }
    }

    // Fade the game object out by decreasing the alpha values
    internal void fadeOut()
    {
        if (fadeTimerStarted)
        {
            // Update timer
            fadeTimer += Time.deltaTime;

            if (fadeTimer >= fadeTimeInSeconds)
            {
                // Reset timer
                fadeTimer = 0f;
                fadeTimerStarted = false;
            }
        }
        // Fade
        Color current = material.GetColor("_Colour");
        current.a = currentFadeValue;
        ChangeColor(current);
    }

    // Change the color and alpha value of the game object
    public void ChangeColor(Color color)
    {
        // This also sets the colour of all child elements
        material.SetColor("_Colour", color);
        material.SetFloat("_AlphaValue", color.a);
    }

    // Triggers both a scaling and rotation
    public void triggerImpact()
    {
        triggerScale();
        triggerRotation();
    }
    // Triggers the fade out effect
    public void triggerFadeOut()
    {
        if (!fadeTimerStarted) fadeTimerStarted = true;
    }

    // Trigger the scaling effect
    public void triggerScale()
    {
        if (!scaleTimerStarted) scaleTimerStarted = true;
    }

    // Trigger the rotation effect
    public void triggerRotation()
    {
        if (!rotTimerStarted) rotTimerStarted = true;
    }

    // Sets fade value and automatically maps it to 0...1
    public void updateFade(float fadeValue, float max)
    {
        currentFadeValue = map(fadeValue, 0f, max, 0f, 1f);
    }

    internal static float map(float value, float fromLow, float fromHigh, float toLow, float toHigh)
    {
        return (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow) + toLow;
    }
}
