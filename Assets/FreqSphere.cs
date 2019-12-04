using UnityEngine;

public class FreqSphere : ResponsiveGameObject
{
    private float smoothTime = 0.075f;
    private float yVelocity = 0.0f;
    // Determines how quickly the fractal rotates without interaction
    public float standardRotationSpeed;
    // Reference to the GameObject (sphere) to which this script is attached

    new void Start()
    {
        base.Start();
    }

    new void Update()
    {
        base.Update();
        scale(scaleProportion);
        rotate();
        standardRotate();
        //fadeOut();
    }

    new public void ChangeColor(Color col)
    {
        Color lerp = Color.Lerp(gameObject.GetComponent<Renderer>().material.GetColor("_Colour"),
            col, 0.5f);
        gameObject.GetComponent<Renderer>().material.SetColor("_Colour", lerp);
    }

    public void ChangeNoiseAmount(float amount)
    {
        float currentNoise = gameObject.GetComponent<Renderer>().material.GetFloat("_Amount");
        float noiseAmount = 0f;
        // Only change if triggered
        if (scaleTimerStarted)
        {
            noiseAmount = Mathf.SmoothDamp(currentNoise, amount, ref yVelocity, smoothTime);
        } else
        {
            // Go back to 0
            noiseAmount = Mathf.SmoothDamp(currentNoise, 0.1f, ref yVelocity, smoothTime);
        }
        gameObject.GetComponent<Renderer>().material.SetFloat("_Amount", noiseAmount);
    }

    private void standardRotate()
    {
        // Rotate around y-axis
        transform.localRotation = Quaternion.Euler(
            transform.localRotation.eulerAngles.x,
            transform.localRotation.eulerAngles.y + standardRotationSpeed * Time.deltaTime,
            transform.localRotation.eulerAngles.z);
    }
}
