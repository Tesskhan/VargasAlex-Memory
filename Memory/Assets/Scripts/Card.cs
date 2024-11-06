using System.Collections;
using UnityEngine;

public class Card : MonoBehaviour
{
    private GameManager gm;       // Reference to GameManager
    public int id;                // Unique ID for each card (shared by pairs)
    public bool isSolved = false; // Flag to track if this card has been solved
    public AudioClip flip;
    private Animator animator;
    private Collider cardCollider;

    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        animator = gameObject.GetComponent<Animator>();
        cardCollider = gameObject.GetComponent<Collider>();
        SetClickable(true); // Make sure the card is clickable at the start
    }

    void OnMouseDown()
    {
        if (!isSolved && cardCollider.enabled) // Ensure collider is enabled
        {
            gm.CardClicked(this); // Notify GameManager

            gm.src.clip = flip;
            gm.src.Play();

            animator.SetTrigger("reveal"); // Trigger reveal animation
            SetClickable(false);           // Disable collider while revealing
            isSolved = true;
        }
    }

    public void HideCard()
    {
        StartCoroutine(EnableColliderAfterAnimation(true)); // Re-enable collider after animation
    }

    public void SolveCard()
    {
        StartCoroutine(EnableColliderAfterAnimation(false)); // Re-enable collider after animation
    }

    private IEnumerator EnableColliderAfterAnimation(bool correct)
    {
        yield return new WaitForSeconds(1.5f); // Adjust delay if needed to match animation length

        if (correct)
        {
            animator.SetTrigger("hide");
            isSolved = false;  
        }
        else if (!correct)
        {
            animator.SetTrigger("solve");
            isSolved = true;
        }

        yield return new WaitForSeconds(0.5f); // Adjust delay if needed to match animation length
        
        if (!isSolved)
        {
            SetClickable(true);
        }
    }


    public void SetClickable(bool clickable)
    {
        if (cardCollider != null)
        {
            cardCollider.enabled = clickable;
        }
    }
}
