using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLoggerScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(this.GetComponent<Transform>().position.y < 0)
        {
            Debug.Log("Below Zero");
        }
        else
        {
            Debug.Log("Above Zero");
        }
    }
}
