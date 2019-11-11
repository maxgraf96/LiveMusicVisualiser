using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereBehavior : MonoBehaviour
{
    public GameObject cubePrefab;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 6; i > -8; i -= 2)
        {
            Instantiate(cubePrefab, new Vector3(i, 0, 0), Quaternion.identity);
        }
        Instantiate(cubePrefab, new Vector3(-2, 0, 0), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Move()
    {
        Vector3 moveVector = new Vector3(0.01f, 0, 0);
        transform.Translate(moveVector);
    }

    public void ChangeColor(GameObject sphere, Color col)
    {
        sphere.GetComponent<Renderer>().material.SetColor("_Color", col);
    }
}
