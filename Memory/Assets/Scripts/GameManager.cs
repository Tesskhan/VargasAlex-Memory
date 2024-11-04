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
    public TextMeshProUGUI newBestTimeText; // UI element for the new best time message
    public Button startButton;               // UI element for the start button
    public int pairs;                        // Number of pairs found
    public float time;                       // Current time spent in game
    public GameObject[] cards;               // Array of all cards
    public Texture[] images;                 // Array of images for each card
    private float bestTime;                  // Best time to complete the game
    private int guessCount;                  // Counter for the number of guesses

    private Card firstCard, secondCard;      // To track the two flipped cards
    private bool isCheckingMatch = false;    // Flag to prevent flipping more than two cards at once
    private bool gameStarted = false;        // Flag to track if the game has started
    public AudioSource src;
    public AudioClip intro, correct, wrong, ending, bestEnding;

    void Start()
    {
        // Find and initialize all cards by tag
        cards = GameObject.FindGameObjectsWithTag("Card");

        // Verify that we have enough images for the number of card pairs
        if (images.Length < cards.Length / 2)
        {
            Debug.LogWarning("Insufficient images provided for pairs of cards.");
            return;
        }

        AssignCardIDsAndTextures();  // Assign IDs and textures to the cards

        // Reset pairs count, timer, and guess count for a new game
        pairs = 0;
        time = 0f;
        guessCount = 0;

        // Display initial UI text
        guessesText.text = "Guesses: 0";

        // Load and display best time
        bestTime = PlayerPrefs.GetFloat("BestTime", float.MaxValue);
        bestTimeText.text = bestTime != float.MaxValue ? 
            "Best Time: " + bestTime.ToString("F2") + "s" : "Best Time: 0s";

        // Hide the congratulations message and set the memory title text
        congratsText.gameObject.SetActive(false);
        
        // Hide the new best time message at the beginning
        newBestTimeText.gameObject.SetActive(false);

        // Set up the start button
        startButton.gameObject.SetActive(true);
        startButton.onClick.AddListener(StartGame);

        // Deactivate all cards initially and display memory title
        foreach (GameObject card in cards)
        {
            card.SetActive(false);
        }
        memoryText.gameObject.SetActive(true);
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
        src.clip = intro;
        src.Play();
        gameStarted = true;
        startButton.gameObject.SetActive(false); // Hide start button once game begins
        memoryText.gameObject.SetActive(false);  // Hide memory title once game begins

        // Activate all cards at game start
        foreach (GameObject card in cards)
        {
            card.SetActive(true);
        }

        Debug.Log("Game started.");
    }

    private void GameFinish()
    {
        src.clip = ending;
        src.Play();
        congratsText.text = "Congratulations!";
        congratsText.gameObject.SetActive(true);

        Time.timeScale = 0;  // Pause the game

        // Check if the current time is a new best time
        if (time < bestTime)
        {
            src.clip = bestEnding;
            src.Play();
            bestTime = time;
            PlayerPrefs.SetFloat("BestTime", bestTime);
            PlayerPrefs.Save();
            bestTimeText.text = "Best Time: " + bestTime.ToString("F2") + "s";
            newBestTimeText.text = "New Best Time!";
            newBestTimeText.gameObject.SetActive(true); // Show the new best time message
            Debug.Log("New best time achieved!");
        }
    }

    private void AssignCardIDsAndTextures()
    {
        List<int> idList = new List<int>();

        // Prepare list of IDs for pairs
        for (int i = 0; i < images.Length; i++)
        {
            idList.Add(i);
            idList.Add(i); 
        }
        
        // Shuffle IDs and assign them to cards
        idList = ShuffleList(idList);

        for (int i = 0; i < cards.Length; i++)
        {
            int cardID = idList[i];
            Card cardComponent = cards[i].GetComponent<Card>();
            cardComponent.id = cardID;

            Renderer renderer = cards[i].GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.mainTexture = images[cardID];
            }
        }
        Debug.Log("Card IDs and textures assigned.");
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
            guessesText.text = "Guesses: " + guessCount;
            CheckForMatch();
        }
    }

    private void CheckForMatch()
{
    SetAllCardsClickable(false); // Disable all card clicks during animation

    if (firstCard.id == secondCard.id)
    {
        pairs++;
        firstCard.isSolved = true;
        secondCard.isSolved = true;

        // Start coroutine to play the correct sound with a delay
        StartCoroutine(PlaySoundWithDelay(correct, 0.75f));
       
        firstCard.SolveCard();
        secondCard.SolveCard();
    }
    else
    {
        // Start coroutine to play the wrong sound with a delay
        StartCoroutine(PlaySoundWithDelay(wrong, 0.75f));

        firstCard.HideCard();
        secondCard.HideCard();
    }

    // Wait for animations to finish, then re-enable cards
    StartCoroutine(ReEnableCardClicksAfterAnimation());

    firstCard = null;
    secondCard = null;
    isCheckingMatch = false;
}

    // Coroutine to play a sound with a specified delay
    private IEnumerator PlaySoundWithDelay(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);
        src.clip = clip;
        src.Play();
    }

    private IEnumerator ReEnableCardClicksAfterAnimation()
    {
        yield return new WaitForSeconds(1.5f); // Adjust if animations have different lengths
        SetAllCardsClickable(true);
    }

    public void SetAllCardsClickable(bool clickable)
    {
        foreach (GameObject card in cards)
        {
            Card cardComponent = card.GetComponent<Card>();
            cardComponent.SetClickable(clickable);
        }
    }
}
