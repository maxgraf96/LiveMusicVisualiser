using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeBehavior : MonoBehaviour
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

    public void ChangeColor(GameObject cube, Color col)
    {
        Color lerp = Color.Lerp(cube.GetComponent<Renderer>().material.color,
            col, 0.5f);
        cube.GetComponent<Renderer>().material.SetColor("_Color", lerp);
    }

    public void ChangeHeight(GameObject cube, float depthTarget) {
        float newScale = Mathf.SmoothDamp(cube.transform.localScale.x, depthTarget, ref yVelocity, smoothTime);
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, newScale);
    }
}
