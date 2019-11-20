using UnityEngine;

public class FreqSphereBehaviour : MonoBehaviour
{
    private float smoothTime = 0.075f;
    private float yVelocity = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
        float noiseAmount = Mathf.SmoothDamp(currentNoise, amount, ref yVelocity, smoothTime);
        freqSphere.GetComponent<Renderer>().material.SetFloat("_Amount", noiseAmount);
    }
}
