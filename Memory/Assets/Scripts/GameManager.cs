using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI timerText;       // UI element for displaying time
    public TextMeshProUGUI bestTimeText;    // UI element for displaying the best time
    public TextMeshProUGUI guessesText;     // UI element for displaying the number of guesses
    public TextMeshProUGUI congratsText;    // UI element for the congratulations message
    public TextMeshProUGUI memoryText;      // UI element for the memory title or instructions
    public Button startButton;              // UI element for the start button
    public int pairs;                       // Number of pairs found
    public float time;                      // Current time spent in game
    public GameObject[] cards;              // Array of all cards
    public Texture[] images;                // Array of images for each card
    private float bestTime;                 // Best time to complete the game
    private int guessCount;                 // Counter for the number of guesses

    private Card firstCard, secondCard;     // To track the two flipped cards
    private bool isCheckingMatch = false;   // Flag to prevent flipping more than two cards at once
    private bool gameStarted = false;       // Flag to track if the game has started

    void Start()
    {
        // Initialize cards and images
        cards = GameObject.FindGameObjectsWithTag("Card");

        // Verify the images array has enough pairs
        if (images.Length < cards.Length / 2)
        {
            return;
        }

        AssignCardIDsAndTextures();  // Assign IDs and textures to the cards

        // Reset pairs count, time, and guess count
        pairs = 0;
        time = 0f;
        guessCount = 0;  // Initialize guess counter

        // Display initial guess count
        guessesText.text = "Guesses: 0";

        // Load the best time from PlayerPrefs, default to a large value if not set
        bestTime = PlayerPrefs.GetFloat("BestTime", float.MaxValue);
        bestTimeText.text = bestTime != float.MaxValue ? 
        "Best Time: " + bestTime.ToString("F2") + "s" : "Best Time: 0s";

        // Hide the congratulations message and set the memory title text
        congratsText.gameObject.SetActive(false);

        // Set up the start button
        startButton.gameObject.SetActive(true);
        startButton.onClick.AddListener(StartGame);

        // Deactivate all cards initially and display memory title
        foreach (GameObject card in cards)
        {
            card.SetActive(false);
        }
        memoryText.gameObject.SetActive(true); // Ensure memory title is visible initially
    }

    void Update()
    {
        if (gameStarted && pairs < images.Length) // Game is ongoing if pairs are not completed
        {
            time += Time.deltaTime;
            timerText.text = "Time: " + time.ToString("F2") + "s";
        }
        else if (pairs >= images.Length) // All pairs found
        {
            Debug.Log("All pairs found! Calling GameFinish.");
            Invoke(nameof(GameFinish), 3f);
        }
    }

    private void StartGame()
    {
        gameStarted = true;
        startButton.gameObject.SetActive(false); // Hide start button once game begins
        memoryText.gameObject.SetActive(false);  // Hide memory title once game begins

        // Activate all cards at game start
        foreach (GameObject card in cards)
        {
            card.SetActive(true);
        }
    }

    private void GameFinish()
    {
        congratsText.text = "Congratulations!";
        congratsText.gameObject.SetActive(true);

        Time.timeScale = 0;  // Pause the game
        if (time < bestTime)
        {
            bestTime = time;
            PlayerPrefs.SetFloat("BestTime", bestTime);
            PlayerPrefs.Save();
            bestTimeText.text = "Best Time: " + bestTime.ToString("F2") + "s";
        }
    }

    private void AssignCardIDsAndTextures()
    {
        List<int> idList = new List<int>();
        for (int i = 0; i < images.Length; i++)
        {
            idList.Add(i); // Add ID once for each pair
            idList.Add(i); 
        }
        idList = ShuffleList(idList);

        for (int i = 0; i < cards.Length; i++)
        {
            int cardID = idList[i];
            Card cardComponent = cards[i].GetComponent<Card>();
            cardComponent.id = cardID;

            Renderer renderer = cards[i].GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.mainTexture = images[cardID]; // Assign texture based on ID
            }
        }
    }

    private List<int> ShuffleList(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
        return list;
    }

    public void CardClicked(Card clickedCard)
    {
        if (!gameStarted || isCheckingMatch) return;

        if (firstCard == null)
        {
            firstCard = clickedCard;
        }
        else
        {
            secondCard = clickedCard;
            isCheckingMatch = true;
            
            // Increment guess count as two cards are being checked
            guessCount++;
            guessesText.text = "Guesses: " + guessCount;  // Update the UI with the current guess count
            
            CheckForMatch();
        }
    }

    private void CheckForMatch()
    {
        if (firstCard.id == secondCard.id)
        {
            pairs++;
            firstCard.isSolved = true;
            secondCard.isSolved = true;

            secondCard.SolveCard();
            firstCard.SolveCard();
        }
        else
        {
            secondCard.HideCard();
            firstCard.HideCard();
        }

        firstCard = null;
        secondCard = null;
        isCheckingMatch = false;
    }
}
