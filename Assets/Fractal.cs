using System.Collections;
using UnityEngine;

/**
 * Child of ResponsiveGameObject.
 * This class represents a semi-abstract way of an interactive 3D fractal. It can take any mesh and material and is hence
 * not limited to particular look. For cruft fest 2 a cube with displaced normals was chosen as the shape (defined in Unity).
 * The class is inherently recursive, meaning both the root and the fractal and its children are instances of the same class.
 * This makes transforming the whole fractal very cheap in terms of computational complexity, as only the root has to be transformed.
 * 
 * Some of the techniques (especially the generation of children) are adapted from: 
 * https://catlikecoding.com/unity/tutorials/constructing-a-fractal/
 */
public class Fractal : ResponsiveGameObject
{
    // The depth in the tree of children
    public int depth;
    // The maximum number of children allowed
    public int maxDepth;
    // What scale the children should be compared to their parents
    public float childScale;
    // The fractal's children
    private Fractal[] children;
    // Initial color
    private Color initial = new Color(0, 0, 0, 0.5f);
    // Determines how quickly the fractal rotates without interaction
    public float standardRotationSpeed;

    // Kick event related fields
    private bool kickTimerStarted = false;
    private float kickTimer;
    public float kickTimeDuration;
    // How far the kick impact should move the fractal
    public float kickScale;

    // HiHat event related fields
    private bool hihatTimerStarted = false;
    private float hihatTimer;
    public float hihatTimeDuration;
    // How far the hihat impact should rotate the fractal
    public float hihatRotationDegrees;

    // Flag for checking if is root
    public bool isRoot = false;

    // Where the child objects should be spawned
    // In this case they are generated in all directions except downwards
    private static Vector3[] childDirections = {
        Vector3.up,
        Vector3.right,
        Vector3.left,
        Vector3.forward,
        Vector3.back,
    };

    private static Quaternion[] childOrientations = {
        Quaternion.identity,
        Quaternion.Euler(0f, 0f, -90f),
        Quaternion.Euler(0f, 0f, 90f),
        Quaternion.Euler(90f, 0f, 0f),
        Quaternion.Euler(-90f, 0f, 0f),
    };

    // Intensity of the fourth frequency band
    private float f4;

    new void Start()
    {
        // Initialise
        base.Start();

        // Add material
        gameObject.AddComponent<MeshFilter>().mesh = mesh;
        gameObject.AddComponent<MeshRenderer>().material = material;

        // Create next layer of child objects if still allowed
        if (depth < maxDepth)
        {
            // By using StartCoroutine the children are generated in game
            // Additionally it allows for easy spawning of children in a loop
            StartCoroutine(CreateChildren());
        }

    }

    new void Update()
    {
        base.Update();
        // Transforms need to be done only on the root object as they will cascade to child objects
        if (isRoot)
        {
            scale();
            //rotate();
            standardRotate();
            kickMove();
            hihatRotate();
            fadeOut();
            // Make color responsive to intensity in the fourth frequency band
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

    // Initialiser for each fractal
    private void Initialize(Fractal parent, int childIndex)
    {
        // Get base values from parent
        mesh = parent.mesh;
        material = parent.material;
        material.SetColor("_Colour", initial);
        maxDepth = parent.maxDepth;
        depth = parent.depth + 1;
        childScale = parent.childScale;
        transform.parent = parent.transform;
        transform.localScale = Vector3.one * childScale; // This makes children childScale (half) the size of their parents
        // Move to position relative to parent
        transform.localPosition =
            childDirections[childIndex] * (0.5f + 0.5f * childScale);
        // Rotate relative to parent
        transform.localRotation = childOrientations[childIndex];
    }

    // Create children
    private IEnumerator CreateChildren()
    {
        for (int i = 0; i < childDirections.Length; i++)
        {
            yield return new WaitForSeconds(0.00001f);
            new GameObject("Fractal Child").AddComponent<Fractal>().
                Initialize(this, i);
        }
        // After the loop is done the children can be easily accessed
        children = GetComponentsInChildren<Fractal>();
    }

    // Standard rotation around the y-axis (without interaction)
    private void standardRotate()
    {
        // Rotate around y-axis
        transform.localRotation = Quaternion.Euler(
            transform.localRotation.eulerAngles.x,
            transform.localRotation.eulerAngles.y + standardRotationSpeed * Time.deltaTime,
            transform.localRotation.eulerAngles.z);
    }

    // Influence of kick drum on the fractal. This method currently scales the fractal down to make space for the
    // extended kick sphere.
    private void kickMove()
    {
        // Only execute if not currently executing
        if (kickTimerStarted)
        {
            // Update timer
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

    // Influence of hihat on the fractal. This method currently rotates the fractal around the xz-axes upon impact.
    private void hihatRotate()
    {
        // Only execute if not currently executing
        if (hihatTimerStarted)
        {
            // Update timer
            hihatTimer += Time.deltaTime;

            if (hihatTimer >= hihatTimeDuration)
            {
                // Reset timer
                hihatTimer = 0f;
                hihatTimerStarted = false;
            }
            else
            {
                // Rotate
                float process = map(hihatTimer, 0f, hihatTimeDuration, 0f, 1f);
                transform.localRotation = Quaternion.Slerp(currentRotation, Quaternion.Euler(
                    currentRotation.eulerAngles.x + hihatRotationDegrees,
                    0,
                    currentRotation.eulerAngles.z + hihatRotationDegrees), process);
            }
        }
    }

    // Trigger method for the kick drum
    public void triggerKick()
    {
        if (!kickTimerStarted) kickTimerStarted = true;
    }

    // Trigger method for the hihat
    public void triggerHihat()
    {
        if (!hihatTimerStarted) hihatTimerStarted = true;
        currentRotation = transform.localRotation;
    }

    // Method to set the intensity of the frequency in the fourth band.
    public void setF4(float f4)
    {
        this.f4 = f4;
    }
}
