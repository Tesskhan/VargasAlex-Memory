using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{   
    private GameManager gm;  // Reference to GameManager
    public int id;           // Unique ID for each card (shared by pairs)

    public bool isSolved = false; // Flag to track if this card has been solved

    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    void OnMouseDown()
    {
        // Only allow clicking if the card is not already solved and is currently active (visible)
        if (!isSolved && gameObject.activeSelf) 
        {
            // Notify GameManager that this card has been clicked
            gm.CardClicked(this);

            // Trigger the reveal animation
            Animator reveal = gameObject.GetComponent<Animator>();
            reveal.SetTrigger("reveal");
        }
    }

    public void HideCard()
    {
         // Trigger the "hide" animation on both matched cards
            Animator hide = gameObject.GetComponent<Animator>();
            hide.SetTrigger("hide");
            
            Debug.Log("HideCard called on Card ID: " + id);
    }

    public void SolveCard()
    {
         // Trigger the "hide" animation on both matched cards
            Animator solve = gameObject.GetComponent<Animator>();
            solve.SetTrigger("solve");
            
            Debug.Log("Both cards have coincident ID: " + id);
    }

    // Method to prevent further clicks
    public void SetClickable(bool clickable)
    {
        // Disable/Enable the card's collider to prevent further clicks
        GetComponent<Collider>().enabled = clickable;
    }
}
