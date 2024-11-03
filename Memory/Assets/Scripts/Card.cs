using System.Collections;
using UnityEngine;

public class Card : MonoBehaviour
{
    private GameManager gm;       // Reference to GameManager
    public int id;                // Unique ID for each card (shared by pairs)
    public bool isSolved = false; // Flag to track if this card has been solved

    private Animator animator;
    private Collider cardCollider;

    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        animator = gameObject.GetComponent<Animator>();
        cardCollider = gameObject.GetComponent<Collider>();

        if (cardCollider == null)
        {
            Debug.LogError("Collider missing on card with ID: " + id);
        }
        
        SetClickable(true); // Make sure the card is clickable at the start
    }

    void OnMouseDown()
    {
        if (!isSolved && cardCollider.enabled) // Ensure collider is enabled
        {
            gm.CardClicked(this); // Notify GameManager

            animator.SetTrigger("reveal"); // Trigger reveal animation
            SetClickable(false);           // Disable collider while revealing
        }
    }

    public void HideCard()
    {
        animator.SetTrigger("hide");  // Trigger hide animation
        StartCoroutine(EnableColliderAfterAnimation()); // Re-enable collider after animation
        Debug.Log("HideCard called on Card ID: " + id);
    }

    public void SolveCard()
    {
        animator.SetTrigger("solve"); // Trigger solve animation
        SetClickable(false);          // Disable collider permanently as solved
        Debug.Log("SolveCard called on Card ID: " + id);
    }

    private IEnumerator EnableColliderAfterAnimation()
    {
        yield return new WaitForSeconds(1f); // Adjust delay if needed to match animation length
        SetClickable(true);                  // Re-enable collider after hiding
    }

    public void SetClickable(bool clickable)
    {
        if (cardCollider != null)
        {
            cardCollider.enabled = clickable;
            Debug.Log("Card ID: " + id + " collider set to " + clickable);
        }
    }
}
