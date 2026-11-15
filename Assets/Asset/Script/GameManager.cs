using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Card cardPrefab;
    public Transform cardGrid;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timeText;
    public GameObject startScreen;
    public List<Sprite> twelveFaces;

    public bool isChecking;
    public bool isPlaying;

    private int score;
    public float maxTime = 60f;
    private float timeLeft;
    private Card firstCard, secondCard;

    private void Awake() => Instance = this;

    private void Start()
    {
        startScreen.SetActive(true);
        isPlaying = false;
    }

    private void Update()
    {
        if (isPlaying && timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            int m = Mathf.FloorToInt(timeLeft / 60);
            int s = Mathf.FloorToInt(timeLeft % 60);
            timeText.text = string.Format("{0:00}:{1:00}", m, s);

            if (timeLeft <= 0) EndGame();
        }
    }

    public void StartGame()
    {
        startScreen.SetActive(false);

        foreach (Transform child in cardGrid)
        {
            Destroy(child.gameObject);
        }

        score = 0;
        scoreText.text = "Điểm: 0";
        timeLeft = maxTime;
        isChecking = false;
        firstCard = null;
        secondCard = null;

        List<Sprite> deck = new List<Sprite>(twelveFaces);
        deck.AddRange(twelveFaces);

        for (int i = 0; i < deck.Count; i++)
        {
            Sprite temp = deck[i];
            int randomIndex = Random.Range(i, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }

        foreach (Sprite sprite in deck)
        {
            Card newCard = Instantiate(cardPrefab, cardGrid);
            newCard.Setup(twelveFaces.IndexOf(sprite), sprite);
        }

        isPlaying = true;
    }

    public void RegisterCard(Card clickedCard)
    {
        if (firstCard == null) firstCard = clickedCard;
        else
        {
            secondCard = clickedCard;
            isChecking = true;
            StartCoroutine(Compare());
        }
    }

    private IEnumerator Compare()
    {
        yield return new WaitForSeconds(0.8f);

        if (firstCard.cardID == secondCard.cardID)
        {
            score += 10;
            scoreText.text = "Điểm: " + score;
            firstCard.Hide();
            secondCard.Hide();

            if (score == 120) EndGame();
        }
        else
        {
            StartCoroutine(firstCard.FlipAnim(false));
            StartCoroutine(secondCard.FlipAnim(false));
            yield return new WaitForSeconds(0.35f);
        }

        firstCard = null;
        secondCard = null;
        isChecking = false;
    }

    public void EndGame()
    {
        isPlaying = false;
        startScreen.SetActive(true);
    }
}