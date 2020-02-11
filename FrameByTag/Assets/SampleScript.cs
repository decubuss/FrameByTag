using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var ting = "";
        foreach (var str in gameObject.GetComponent<Animator>().parameters)
        {
            ting += str + " "; //maybe also + '\n' to put them on their own line.
        }
        //Debug.Log(ting);

        Debug.Log(gameObject.GetComponent<Animator>());

        gameObject.GetComponent<Animator>().speed = 0f;
        gameObject.GetComponent<Animator>().PlayInFixedTime("Idle01", 0, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
