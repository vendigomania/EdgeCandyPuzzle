using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainGameController : MonoBehaviour
{
    [SerializeField] private GameObject startScreen;
    [SerializeField] private TMP_Text startLvlLable;
    [SerializeField] private TMP_Text startBestLable;

    [SerializeField] private GameObject playScreen;
    [SerializeField] private TMP_Text movesLable;
    [SerializeField] private TMP_Text scoreLable;
    [SerializeField] private Image progressFill;
    [SerializeField] private GameObject screenBlock;

    [SerializeField] private GameObject endScreen;
    [SerializeField] private GameObject loseContent;
    [SerializeField] private GameObject winContent;
    [SerializeField] private TMP_Text endLevelLable;
    [SerializeField] private TMP_Text endScoreLable;
    [SerializeField] private TMP_Text endBestLable;

    [SerializeField] private BallGenerator board;
    [SerializeField] private PointsConnector pointsConnector;

    private int MaxLevel
    {
        get => PlayerPrefs.GetInt("Level", 1);
        set => PlayerPrefs.SetInt("Level", value);
    }

    private int BestScore
    {
        get => PlayerPrefs.GetInt("Best", 0);
        set => PlayerPrefs.SetInt("Best", value);
    }

    private int ScoreLast
    {
        get => PlayerPrefs.GetInt("Score", 0);
        set => PlayerPrefs.SetInt("Score", value);
    }

    int currentLevel;
    int moves;
    int score;

    int targetForLevel => 20 + currentLevel * 4;

    void Start()
    {
        float coeff = ((float)Screen.height / Screen.width / (1920f / 1080f));
        Camera.main.orthographicSize = 9 * coeff;

        pointsConnector.OnLineEnd += OnLineFinish;
        board.OnBlockScreen += (isBlockActive) => screenBlock.SetActive(isBlockActive);

        UpdateStartScreen();
    }

    public void StartGame()
    {
        startScreen.SetActive(false);
        levelsScreen.SetActive(false);
        playScreen.SetActive(true);

        board.gameObject.SetActive(true);
        pointsConnector.gameObject.SetActive(true);
        board.StartGame();

        endScreen.SetActive(false);

        score = 0;
        moves = 20;

        scoreLable.text = (score + ScoreLast).ToString();
        movesLable.text = moves.ToString();

        progressFill.fillAmount = 0f;

        SoundController.Instance.Click();

        ParticlesController.INstance.SetLevel(currentLevel);
    }

    [Header("Levels screen")]
    [SerializeField] private GameObject levelsScreen;
    [SerializeField] private Button[] levelsButtons;
    [SerializeField] private Button prevPage;
    [SerializeField] private Button nextPage;

    int page = 0;

    public void ShowLevels()
    {
        levelsScreen.SetActive(true);

        for (int i = 0; i < levelsButtons.Length; i++)
        {
            int number = page * 5 + i + 1;
            levelsButtons[i].GetComponentInChildren<TMP_Text>().text = number.ToString();
            levelsButtons[i].interactable = MaxLevel >= number;
        }

        prevPage.interactable = page > 0;

        SoundController.Instance.Click();
    }

    public void SwitchPage(int addPage)
    {
        page += addPage;

        ShowLevels();
    }

    public void LaunchLevelFromLevels(int btnIndex)
    {
        currentLevel = page * 5 + btnIndex + 1;
        StartGame();
    }

    public void BackToMenu()
    {
        board.gameObject.SetActive(false);
        pointsConnector.gameObject.SetActive(false);

        startScreen.SetActive(true);
        playScreen.SetActive(false);

        UpdateStartScreen();

        SoundController.Instance.Click();
    }

    public void Next()
    {
        currentLevel++;
        StartGame();
    }

    private void OnLineFinish(int length)
    {
        score += length;
        moves--;

        scoreLable.text = (score + ScoreLast).ToString();
        movesLable.text = moves.ToString();

        progressFill.fillAmount = 1f * score / targetForLevel;

        if (score >= targetForLevel)
        {
            ShowResult(true);
        }
        else if (moves == 0)
        {
            ShowResult(false);
        }
        else
        {
            SoundController.Instance.Right();
        }
    }

    private void ShowResult(bool isWin)
    {
        if (isWin)
        {
            SoundController.Instance.Win();
            if (currentLevel == MaxLevel)
            {
                MaxLevel++;
            }
        }
        else
        {
            SoundController.Instance.Lose();
            ScoreLast = 0;
        }

        endScreen.SetActive(true);

        winContent.SetActive(isWin);
        loseContent.SetActive(!isWin);
        
        ScoreLast = isWin? ScoreLast + score : 0;
        score = 0;
        

        if (isWin && ScoreLast > BestScore)
        {
            BestScore = ScoreLast;
            endBestLable.text = $"RECORD!";
        }
        else
        {
            endBestLable.text = BestScore.ToString();
        }

        endLevelLable.text = $"LEVEL: {currentLevel}";
        endScoreLable.text = ScoreLast.ToString();
    }

    private void UpdateStartScreen()
    {
        startBestLable.text = $"BEST SCORE: {BestScore}";
        startLvlLable.text = $"LEVEL: {MaxLevel}";
    }
}
