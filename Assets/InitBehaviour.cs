using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitBehaviour : MonoBehaviour
{
    public GameObject freqSpherePrefab;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = -6; i < 8; i += 2)
        {
            Instantiate(freqSpherePrefab, new Vector3(i, 0, 0), Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
