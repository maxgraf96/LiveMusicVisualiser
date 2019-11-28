using UnityEngine;

public class InitBehaviour : MonoBehaviour
{
    // Prefabricated objects for easy instantiation
    public GameObject freqSpherePrefab;
    public GameObject freqPlanePrefab;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = -6; i < 8; i += 2)
        {
            Instantiate(freqSpherePrefab, new Vector3(i, 0, 0), Quaternion.identity);
            Instantiate(freqPlanePrefab, new Vector3(i, -1, 0), Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
