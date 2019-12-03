using UnityEngine;

public class FreqSphereBehaviour : ResponsiveGameObject
{
    private float smoothTime = 0.075f;
    private float yVelocity = 0.0f;

    public FreqSphereBehaviour(float scaleTimeInSeconds, float scaleProportion, float rotTimeInSeconds, 
        float rotDegrees, float fadeTimeInSeconds)
    {
        this.scaleTimeInSeconds = scaleTimeInSeconds;
        this.scaleProportion = scaleProportion;
        this.rotTimeInSeconds = rotTimeInSeconds;
        this.rotDegrees = rotDegrees;
        this.fadeTimeInSeconds = fadeTimeInSeconds;
    }

    void Start()
    {
        base.Start();
    }

    void Update()
    {
        base.Update();
        scale(scaleProportion);
        rotate();
        //fadeOut();
    }

    public void ChangeColor(GameObject freqSphere, Color col)
    {
        Color lerp = Color.Lerp(freqSphere.GetComponent<Renderer>().material.GetColor("_Colour"),
            col, 0.5f);
        freqSphere.GetComponent<Renderer>().material.SetColor("_Colour", lerp);
    }

    public void ChangeNoiseAmount(GameObject freqSphere, float amount)
    {
        float currentNoise = freqSphere.GetComponent<Renderer>().material.GetFloat("_Amount");
        float noiseAmount = 0f;
        // Only change if triggered
        if (scaleTimerStarted)
        {
            noiseAmount = Mathf.SmoothDamp(currentNoise, amount, ref yVelocity, smoothTime);
        } else
        {
            // Go back to 0
            noiseAmount = Mathf.SmoothDamp(currentNoise, 0f, ref yVelocity, smoothTime);
        }
        freqSphere.GetComponent<Renderer>().material.SetFloat("_Amount", noiseAmount);
    }
}
