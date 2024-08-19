using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOpen : MonoBehaviour
{
    private Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<FirstPersonController>().cam;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
