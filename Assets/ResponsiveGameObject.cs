using UnityEngine;

public class ResponsiveGameObject : MonoBehaviour
{
    // Mesha and Material
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
        currentRotation = transform.localRotation;
    }

    // Update is called once per frame
    internal void Update() {

    }

    internal void scale(float scale)
    {
        if (scaleTimerStarted)
        {
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

    internal void rotate()
    {
        if (rotTimerStarted)
        {
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
                // Rotaaate
                float process = map(rotTimer, 0f, rotTimeInSeconds, 0f, 1f);
                transform.localRotation = Quaternion.Lerp(currentRotation, Quaternion.Euler(0,
                    currentRotation.eulerAngles.y + rotDegrees, 0), process);
            }
        }
    }

    internal void fadeOut()
    {
        if (fadeTimerStarted)
        {
            fadeTimer += Time.deltaTime;

            if (fadeTimer >= fadeTimeInSeconds)
            {
                // Reset timer
                fadeTimer = 0f;
                fadeTimerStarted = false;
            }
        }
        // Fadeeee
        Color current = material.GetColor("_Colour");
        current.a = currentFadeValue;
        ChangeColor(current);
    }

    public void ChangeColor(Color color)
    {
        // This also sets the colour of all child elements
        material.SetColor("_Colour", color);
        material.SetFloat("_AlphaValue", color.a);
    }

    public void triggerImpact()
    {
        triggerScale();
        triggerRotation();
    }
    // Triggers
    public void triggerFadeOut()
    {
        if (!fadeTimerStarted) fadeTimerStarted = true;
    }

    public void triggerScale()
    {
        if (!scaleTimerStarted) scaleTimerStarted = true;
    }

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
