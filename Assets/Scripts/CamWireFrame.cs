using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamWireFrame : MonoBehaviour
{
    public bool isWireFrame = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnPreRender()
    {
        if(isWireFrame)
        {
            GL.wireframe = true;
        }
    }

    private void OnPostRender()
    {
        if (isWireFrame)
        {
            GL.wireframe = false;
        }
    }
}
