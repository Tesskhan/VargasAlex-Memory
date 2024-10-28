using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI timerText;      // UI element for displaying time
    public TextMeshProUGUI bestTimeText;   // UI element for displaying the best time
    public TextMeshProUGUI congratsText;   // UI element for the congratulations message
    public int pairs;                      // Number of pairs found
    public float time;                     // Current time spent in game
    public GameObject[] cards;             // Array of all cards
    public Texture[] images;               // Array of images for each card
    private float bestTime;                // Best time to complete the game

    private Card firstCard, secondCard;    // To track the two flipped cards
    private bool isCheckingMatch = false;  // Flag to prevent flipping more than two cards at once

    void Start()
    {
        cards = GameObject.FindGameObjectsWithTag("Card");
        Debug.Log("Found " + cards.Length + " cards.");

        // Verify the images array has enough pairs
        if (images.Length < cards.Length / 2)
        {
            Debug.LogError("Not enough images for pairs. Ensure images array has enough entries.");
            return;
        }

        AssignCardIDsAndTextures();  // Assign IDs and textures to the cards

        pairs = 0; // Reset pairs count
        Debug.Log("Pairs reset to 0.");

        time = 0f; // Reset time
        Debug.Log("Time reset to 0.");

        // Load the best time from PlayerPrefs, default to a large value if not set
        bestTime = PlayerPrefs.GetFloat("BestTime", float.MaxValue);
        Debug.Log("Best time loaded: " + bestTime);

        // Update the best time UI
        if (bestTime != float.MaxValue)
        {
            bestTimeText.text = "Best Time: " + bestTime.ToString("F2") + "s";
        }
        else
        {
            bestTimeText.text = "Best Time: 0s";
        }

        // Hide the congratulations message at the start
        congratsText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (pairs < images.Length) // Game is ongoing if pairs are not completed
        {
            time += Time.deltaTime;
            timerText.text = "Time: " + time.ToString("F2") + "s";
        }
        else
        {
            Debug.Log("All pairs found! Calling GameFinish.");
            GameFinish();
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
        Debug.Log("All IDs and textures assigned.");
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
        if (isCheckingMatch) return;

        if (firstCard == null)
        {
            firstCard = clickedCard;
        }
        else
        {
            secondCard = clickedCard;
            isCheckingMatch = true;
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
            firstCard.SolveCard();
            secondCard.SolveCard();
        }
        else
        {
            firstCard.HideCard();
            secondCard.HideCard();
        }

        firstCard = null;
        secondCard = null;
        isCheckingMatch = false;
    }
}
