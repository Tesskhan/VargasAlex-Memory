using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    private int green;
    private GameObject gm;
    public GameObject face;

    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController");
    }

    void OnMouseDown()
    {
        if (face.activeSelf)
        {
            TriggerSmash();
        }
    }

    public void ActivateCard(bool isGreen, bool activated)
    {
        if (activated)
        {
            face.SetActive(true); // Show the face

            // Set the color based on the isGreen value passed in
            if (isGreen)
            {
                face.GetComponent<Renderer>().material.color = Color.green;
                green = 1;
            }
            else
            {
                face.GetComponent<Renderer>().material.color = Color.red;
                green = 0;
            }
        }
    }

    private void TriggerSmash()
    {
        if (gm != null)
        {
            GameManager gmScript = gm.GetComponent<GameManager>();
            Animator anim = face.GetComponent<Animator>();

            anim.SetTrigger("smash"); // Trigger the smash animation

            // Start coroutine to deactivate the face after the animation is complete
            StartCoroutine(DeactivatefaceAfterAnimation(anim));

            if (gmScript != null)
            {
                gmScript.onCardPick(face, green); // Call the GameManager's method
                green = 0;
            }
        }
    }

    private IEnumerator DeactivatefaceAfterAnimation(Animator anim)
    {
        // Wait for the duration of the animation
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);

        face.SetActive(false); // Deactivate face after animation
    }
}
