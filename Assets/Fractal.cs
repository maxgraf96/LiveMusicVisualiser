using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fractal : MonoBehaviour
{
    public Mesh mesh;
    public Material material;
    public int depth;
    public int maxDepth;
    public float childScale;
    private float depthRatio;
    private float iVisibilty; // Incoming visibilty [0...1]
    private Fractal[] children;
    private Color blank = new Color(0, 0, 0, 0f);
    private Color filled = new Color(0, 0, 0, 1f);

    // Timer for trigger events
    // Scaling
    private bool scaleTimerStarted = false;
    private float scaleTimer = 0f;
    public float scaleTimeInSeconds;
    public Vector3 originalScale;
    public float scaleProportion; // To what size of the original size the fractal should be increased

    // Rotation
    private bool rotTimerStarted = false;
    private float rotTimer = 0f;
    public float rotTimeInSeconds;
    public float rotDegrees; // How far the fractal should be rotated on impact
    private Quaternion currentRotation;

    // Fading
    private bool fadeTimerStarted = false;
    private float fadeTimer = 0f;
    public float fadeTimeInSeconds;

    // Flag for checking if is root
    public bool isRoot = false;

    private static Vector3[] childDirections = {
        Vector3.up,
        Vector3.right,
        Vector3.left,
        Vector3.forward,
        Vector3.back
    };

    private static Quaternion[] childOrientations = {
        Quaternion.identity,
        Quaternion.Euler(0f, 0f, -90f),
        Quaternion.Euler(0f, 0f, 90f),
        Quaternion.Euler(90f, 0f, 0f),
        Quaternion.Euler(-90f, 0f, 0f)
    };

    private void Start()
    {
        gameObject.AddComponent<MeshFilter>().mesh = mesh;
        gameObject.AddComponent<MeshRenderer>().material = material;

        if (depth < maxDepth)
        {
            StartCoroutine(CreateChildren());
        }

        depthRatio = depth / maxDepth;
        currentRotation = transform.localRotation;
    }

    private void Initialize(Fractal parent, int childIndex)
    {
        mesh = parent.mesh;
        material = parent.material;
        material.SetColor("_Colour", blank);
        maxDepth = parent.maxDepth;
        depth = parent.depth + 1;
        childScale = parent.childScale;
        transform.parent = parent.transform;
        transform.localScale = Vector3.one * childScale; // This makes children childScale (half) the size of their parents
        transform.localPosition =
            childDirections[childIndex] * (0.5f + 0.5f * childScale);
        transform.localRotation = childOrientations[childIndex];
    }

    private IEnumerator CreateChildren()
    {
        for (int i = 0; i < childDirections.Length; i++)
        {
            yield return new WaitForSeconds(0.00001f);
            new GameObject("Fractal Child").AddComponent<Fractal>().
                Initialize(this, i);
        }
        children = GetComponentsInChildren<Fractal>();
    }

    void Update()
    {
        // Transforms need to be done only on the root object as they will cascade to child objects
        if (isRoot)
        {
            scale(scaleProportion);
            rotate();
            fadeOut();
        }
    }

    public void ChangeColor(Color color)
    {
        // This also sets the colour of all child elements
        material.SetColor("_Colour", color);
        material.SetFloat("_AlphaValue", color.a);
    }

    private void scale(float scale)
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

    private void rotate()
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

    private void fadeOut()
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
            else
            {
                // Fade out
                float alpha = map(fadeTimer, 0f, fadeTimeInSeconds, 1f, 0f);
                Color current = material.GetColor("_Colour");
                current.a = alpha;
                ChangeColor(current);
            }
        }
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

    // Helper methods

    private static float map(float value, float fromLow, float fromHigh, float toLow, float toHigh)
    {
        return (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow) + toLow;
    }
}
