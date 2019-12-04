using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fractal : ResponsiveGameObject
{
    public int depth;
    public int maxDepth;
    public float childScale;
    private float depthRatio;
    private float iVisibilty; // Incoming visibilty [0...1]
    private Fractal[] children;
    private Color initial = new Color(0, 0, 0, 0.5f);
    // Determines how quickly the fractal rotates without interaction
    public float standardRotationSpeed;

    // Kick related
    private bool kickTimerStarted = false;
    private float kickTimer;
    public float kickTimeDuration;
    // How far the kick impact should move the fractal
    public float kickScale;
    private Vector3 originalPosition = Vector3.zero;

    // HiHat related
    private bool hihatTimerStarted = false;
    private float hihatTimer;
    public float hihatTimeDuration;
    // How far the hihat impact should rotate the fractal
    public float hihatRotationDegrees;

    // Flag for checking if is root
    public bool isRoot = false;

    // Where the child objects should be spawned
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
    private float f4;

    new void Start()
    {
        // Initialise
        base.Start();

        // Add material
        gameObject.AddComponent<MeshFilter>().mesh = mesh;
        gameObject.AddComponent<MeshRenderer>().material = material;

        // Create next layer of child objects
        if (depth < maxDepth)
        {
            StartCoroutine(CreateChildren());
        }

        depthRatio = depth / maxDepth;
    }

    new void Update()
    {
        base.Update();
        // Transforms need to be done only on the root object as they will cascade to child objects
        if (isRoot)
        {
            scale(scaleProportion);
            //rotate();
            standardRotate();
            kickMove();
            hihatRotate();
            fadeOut();
            Color col = Color.HSVToRGB(f4, 1.0f, f4);
            if(f4 == 0f)
            {
                ChangeColor(col);
            } else
            {
                ChangeColor(Color.Lerp(material.GetColor("_Colour"), col, Time.deltaTime * 2f));
            }
        }
    }

    // Fractal specific methods
    private void Initialize(Fractal parent, int childIndex)
    {
        mesh = parent.mesh;
        material = parent.material;
        material.SetColor("_Colour", initial);
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

    private void standardRotate()
    {
        // Rotate around y-axis
        transform.localRotation = Quaternion.Euler(
            transform.localRotation.eulerAngles.x,
            transform.localRotation.eulerAngles.y + standardRotationSpeed * Time.deltaTime,
            transform.localRotation.eulerAngles.z);
    }

    private void kickMove()
    {
        if (kickTimerStarted)
        {
            kickTimer += Time.deltaTime;

            if (kickTimer >= kickTimeDuration)
            {
                // Reset timer
                kickTimer = 0f;
                kickTimerStarted = false;
            } else
            {
                if (kickTimer <= kickTimeDuration / 2f)
                {
                    // Scale down
                    transform.localScale = Vector3.Lerp(originalScale, originalScale * kickScale,
                        map(kickTimer, 0f, kickTimeDuration, 0f, 1f));
                }
                else
                {
                    // Scale back up
                    transform.localScale = Vector3.Lerp(originalScale, originalScale * kickScale,
                        map(kickTimer, 0f, kickTimeDuration, 1f, 0f));
                }
            }
        }
    }

    private void hihatRotate()
    {
        if (hihatTimerStarted)
        {
            hihatTimer += Time.deltaTime;

            if (hihatTimer >= hihatTimeDuration)
            {
                // Reset timer
                hihatTimer = 0f;
                hihatTimerStarted = false;
            }
            else
            {
                // Rotaaate
                float process = map(hihatTimer, 0f, hihatTimeDuration, 0f, 1f);
                transform.localRotation = Quaternion.Slerp(currentRotation, Quaternion.Euler(
                    currentRotation.eulerAngles.x + hihatRotationDegrees,
                    0,
                    currentRotation.eulerAngles.z + hihatRotationDegrees), process);
            }
        }
    }

    public void triggerKick()
    {
        if (!kickTimerStarted) kickTimerStarted = true;
    }

    public void triggerHihat()
    {
        if (!hihatTimerStarted) hihatTimerStarted = true;
        currentRotation = transform.localRotation;
    }

    public void setF4(float f4)
    {
        this.f4 = f4;
    }
}
