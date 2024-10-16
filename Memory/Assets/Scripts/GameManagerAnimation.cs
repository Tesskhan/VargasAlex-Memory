using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerAnimation : MonoBehaviour
{
    public GameObject mole;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Animator anim = GetComponent<Animator>();
            anim.SetTrigger("poow");
    }
}
