using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI timer;
    public TextMeshProUGUI congrats; 
    public int parelles; // Les parelles de cartes que hi ha
    public float time; // Time displayed on the screen
    public float rest; // Randomized rest time
    public GameObject[] cards;

    // Start is called before the first frame update
    void Start()
    {
        cards = GameObject.FindGameObjectsWithTag("Card");
        rest = 3f;
        time = 0f; // Initialize time at the start
    }

    void Update()
    {
        // Activate cards if rest is up and no cards are currently active
            ActivateCard();

        // Decrease time
        time = Time.deltaTime;
        timer.text = "Time: " + time.ToString("F2") + "s";
            
        if (parelles == 8)
        {
            GameFinish();
        }

    }

    private void ActivateCard()
    {
        foreach (GameObject card in cards)
        {
            Card c = card.GetComponent<Card>();
            // Randomly decide if this Card should be green
            bool isGreen = UnityEngine.Random.Range(0, 2) == 0;
            bool activated = UnityEngine.Random.Range(0, 2) == 0;

            c.ActivateCard(isGreen, activated); // Activate the Card
        }
    }

    private void GameFinish()
    {
        congrats.text = "Congratulations";
        Time.timeScale = 0;

        // Deactivate all cards
        foreach (GameObject card in cards)
        {
            card.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            RestartGame();
        }
    }

    // Method to handle what happens when a card is hit
    public void onCardPick(GameObject face, int green)
    {
        // Check if the clicked card is green and update the score
        if (face.GetComponent<MeshRenderer>().material.color.Equals(Color.green))
        {
            // puntuatge += 1; // Increment score for hitting green
        }
        else
        {
            // puntuatge -= 1; // Decrement score for hitting red
        }

    }

    // Restart the game
    public void RestartGame()
    {
        // Reset the game variables
        time = 0f;  // Set initial game time

        // Re-enable the cards
        foreach (GameObject card in cards)
        {
            card.SetActive(true);
        }

        // Resume the game
        Time.timeScale = 1;
    }
}

public class TextMeshProGUI
{
}