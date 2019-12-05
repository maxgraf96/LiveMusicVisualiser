using UnityEngine;

/**
 * Child class of ResponsiveGameObject. A responsive sphere that changes color, transformation and shape depending on its input.
 * Upon triggering its surface normals are warped by a noisemap which produces a spike-like surface. 
 * Accordingly, in the current setup given by UDPReceiver, its color is dependent on the intensity in a certain frequency band.
 */
public class FreqSphere : ResponsiveGameObject
{
    // How smooth transformations should happen
    private float smoothTime = 0.075f;
    private float yVelocity = 0.0f;
    // Determines how quickly the sphere rotates without interaction
    public float standardRotationSpeed;

    new void Start()
    {
        base.Start();
    }

    new void Update()
    {
        base.Update();
        scale();
        rotate();
        standardRotate();
    }

    // Change the color of the sphere (this is done in a different fashion than in the base class, hence the new keyword before
    // the method name)
    new public void ChangeColor(Color col)
    {
        Color lerp = Color.Lerp(gameObject.GetComponent<Renderer>().material.GetColor("_Colour"),
            col, 0.5f);
        gameObject.GetComponent<Renderer>().material.SetColor("_Colour", lerp);
    }

    // Change the amount of the impact of the noise on the surface normals
    public void ChangeNoiseAmount(float amount)
    {
        float currentNoise = gameObject.GetComponent<Renderer>().material.GetFloat("_Amount");
        float noiseAmount;
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

    // Rotates the sphere continuously around the y-axis
    private void standardRotate()
    {
        // Rotate around y-axis
        transform.localRotation = Quaternion.Euler(
            transform.localRotation.eulerAngles.x,
            transform.localRotation.eulerAngles.y + standardRotationSpeed * Time.deltaTime,
            transform.localRotation.eulerAngles.z);
    }
}
