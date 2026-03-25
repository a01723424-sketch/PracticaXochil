using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class ForestGameController_JuanRdz : MonoBehaviour
{
    public static ForestGameController_JuanRdz Instance;

    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI timerText;
    public GameObject timerPanel;

    [Header("Player")]
    public PlayerMove playerMove;

    public float gameTime = 60f;

    private float currentTime;
    private bool gameStarted = false;
    private bool gameEnded = false;
    private bool endSoundPlayed = false;

    public PhotoSpot[] photoSpots;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ResetRunState();
        StartCoroutine(BeginCountdownSequence());
    }

    IEnumerator BeginCountdownSequence()
    {
        yield return null;

        if (playerMove != null)
            playerMove.SetMovementEnabled(false);

        SFXManager_JuanRdz.StopAmbience();

        if (timerText != null)
        {
            timerText.gameObject.SetActive(false);
            timerText.text = "Tiempo: " + Mathf.Ceil(gameTime).ToString();
        }

        if (timerPanel != null)
            timerPanel.SetActive(false);

        if (countdownText != null)
            countdownText.gameObject.SetActive(true);

        StartCoroutine(StartCountdown());
    }

    void ResetRunState()
    {
        gameStarted = false;
        gameEnded = false;
        currentTime = gameTime;
        endSoundPlayed = false;

        if (QuestController_JuanRdz.Instance != null)
        {
            QuestController_JuanRdz.Instance.currentPhotos = 0;
            QuestController_JuanRdz.Instance.targetPhotos = 3;
            QuestController_JuanRdz.Instance.ResetRunScore();
            QuestController_JuanRdz.Instance.SetObjective("Toma 3 fotos (0/3)");
        }
    }

    IEnumerator StartCountdown()
    {
        SFXManager_JuanRdz.Play("Countdown");

        if (playerMove != null)
            playerMove.SetMovementEnabled(false);

        countdownText.text = "5";
        yield return new WaitForSeconds(1f);

        countdownText.text = "4";
        yield return new WaitForSeconds(1f);

        countdownText.text = "3";
        yield return new WaitForSeconds(1f);

        countdownText.text = "2";
        yield return new WaitForSeconds(1f);

        countdownText.text = "1";
        yield return new WaitForSeconds(1f);

        countdownText.text = "¡Vamos!";
        yield return new WaitForSeconds(1f);

        if (countdownText != null)
            countdownText.gameObject.SetActive(false);

        if (timerText != null)
        {
            timerText.gameObject.SetActive(true);
            timerText.text = "Tiempo: " + Mathf.Ceil(currentTime).ToString();
        }

        if (timerPanel != null)
            timerPanel.SetActive(true);

        if (playerMove != null)
            playerMove.SetMovementEnabled(true);

        StartGame();
        ActivateRandomSpots();
    }

    void StartGame()
    {
        gameStarted = true;
        gameEnded = false;
        currentTime = gameTime;

        SFXManager_JuanRdz.PlayAmbience("GameMusic");

        if (timerText != null)
            timerText.text = "Tiempo: " + Mathf.Ceil(currentTime).ToString();
    }

    void Update()
    {
        if (!gameStarted || gameEnded)
            return;

        if (QuestController_JuanRdz.Instance != null)
        {
            if (QuestController_JuanRdz.Instance.currentPhotos >= QuestController_JuanRdz.Instance.targetPhotos)
            {
                EndGame();
                return;
            }
        }

        currentTime -= Time.deltaTime;

        if (currentTime <= 1f && !endSoundPlayed)
        {
            SFXManager_JuanRdz.Play("End");
            endSoundPlayed = true;
        }

        if (currentTime < 0f)
            currentTime = 0f;

        if (timerText != null)
            timerText.text = "Tiempo: " + Mathf.Ceil(currentTime).ToString();

        if (currentTime <= 0f)
            EndGame();
    }

    void ActivateRandomSpots()
    {
        foreach (PhotoSpot spot in photoSpots)
            spot.SetActiveSpot(false);

        System.Collections.Generic.List<PhotoSpot> shuffledSpots =
            new System.Collections.Generic.List<PhotoSpot>(photoSpots);

        for (int i = 0; i < shuffledSpots.Count; i++)
        {
            PhotoSpot temp = shuffledSpots[i];
            int randomIndex = Random.Range(i, shuffledSpots.Count);
            shuffledSpots[i] = shuffledSpots[randomIndex];
            shuffledSpots[randomIndex] = temp;
        }

        for (int i = 0; i < 3 && i < shuffledSpots.Count; i++)
            shuffledSpots[i].SetActiveSpot(true);
    }

    void EndGame()
    {
        if (gameEnded)
            return;

        gameStarted = false;
        gameEnded = true;

        if (playerMove != null)
            playerMove.SetMovementEnabled(false);

        SFXManager_JuanRdz.StopAmbience();

        if (QuestController_JuanRdz.Instance != null)
        {
            QuestController_JuanRdz quest = QuestController_JuanRdz.Instance;
            bool won = quest.currentPhotos >= quest.targetPhotos;

            quest.RegisterTime(currentTime);

            if (won)
            {
                if (!quest.IsPostGameActive())
                    quest.ActivatePostGame();

                quest.RegisterBiomeCompletion("terrestre");

                if (currentTime > 30f)
                    quest.AddSpeedBonus();
            }
        }

        StartCoroutine(GoToResultsAfterDelay());
    }

    IEnumerator GoToResultsAfterDelay()
    {
        PlayerPrefs.SetString("LastLevel", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();

        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Mini2_Resultados");
    }

    public void FinishGameAndGoToResults()
    {
        if (gameEnded)
            return;

        gameStarted = false;
        gameEnded = true;

        if (playerMove != null)
            playerMove.SetMovementEnabled(false);

        if (QuestController_JuanRdz.Instance != null)
        {
            QuestController_JuanRdz.Instance.RegisterTime(currentTime);

            if (QuestController_JuanRdz.Instance.currentPhotos >= QuestController_JuanRdz.Instance.targetPhotos)
            {
                if (!QuestController_JuanRdz.Instance.IsPostGameActive())
                    QuestController_JuanRdz.Instance.ActivatePostGame();

                QuestController_JuanRdz.Instance.RegisterBiomeCompletion("terrestre");

                if (currentTime > 30f)
                    QuestController_JuanRdz.Instance.AddSpeedBonus();
            }
        }

        StartCoroutine(PlayEndAndLoadResults());
    }

    IEnumerator PlayEndAndLoadResults()
    {
        SFXManager_JuanRdz.StopAmbience();

        AudioClip clip = SFXManager_JuanRdz.Play("End");

        if (clip != null)
            yield return new WaitForSeconds(clip.length);
        else
            yield return new WaitForSeconds(1f);

        SceneManager.LoadScene("Mini2_Resultados");
    }
}