using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCloudBehaviour : MonoBehaviour
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

    public void ChangeBass(GameObject sphere, float amount)
    {
        float currentNoise = sphere.GetComponent<Renderer>().material.GetFloat("_BassIntensity");
        float noiseAmount = Mathf.SmoothDamp(currentNoise, amount, ref yVelocity, smoothTime);
        if (noiseAmount < 0.01) noiseAmount = 0.0f;
        sphere.GetComponent<Renderer>().material.SetFloat("_BassIntensity", noiseAmount);
    }

    public void ChangeColour(GameObject sphere, Color col)
    {
        float currentA = sphere.GetComponent<Renderer>().material.GetColor("_Colour").a;
        float a = Mathf.SmoothDamp(currentA, col.a, ref yVelocity, smoothTime);
        col.a = a;
        sphere.GetComponent<Renderer>().material.SetColor("_Colour", col);
    }
}
