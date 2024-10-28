using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerAnimation : MonoBehaviour
{
    public GameObject card;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Animator reveal = GetComponent<Animator>();
        Animator hide = GetComponent<Animator>();
        Animator solve = GetComponent<Animator>();
            reveal.SetTrigger("reveal");
            hide.SetTrigger("hide");
            solve.SetTrigger("solve");
    }
}
