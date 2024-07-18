using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LuckySnapCapController : MonoBehaviour
{
    [SerializeField] private GameObject startScreen;
    [SerializeField] private TMP_Text startLvlLable;
    [SerializeField] private TMP_Text startBestLable;

    [SerializeField] private GameObject playScreen;
    [SerializeField] private TMP_Text movesLable;
    [SerializeField] private TMP_Text scoreLable;
    [SerializeField] private Image progressFill;

    [SerializeField] private GameObject endScreen;
    [SerializeField] private GameObject loseContent;
    [SerializeField] private GameObject winContent;
    [SerializeField] private TMP_Text endLevelLable;
    [SerializeField] private TMP_Text endScoreLable;
    [SerializeField] private TMP_Text endBestLable;


    [SerializeField] private Transform visualizatorsRoot;

    private PointVisualizator[] visualizators;

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
        PointVisualizator.OnSequenceFinish += OnSequenceFinish;

        visualizators = new PointVisualizator[49];
        visualizators[0] = visualizatorsRoot.GetComponentInChildren<PointVisualizator>();
        for(int i = 1; i < visualizators.Length; i++)
        {
            visualizators[i] = Instantiate(visualizators[0], visualizatorsRoot);
        }

        UpdateStartScreen();
    }

    public void StartGame()
    {
        startScreen.SetActive(false);
        levelsScreen.SetActive(false);
        playScreen.SetActive(true);

        foreach (var visualizator in visualizators) visualizator.Number = Random.Range(1, 5);

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

    private void OnSequenceFinish()
    {
        if(PointVisualizator.selected.Count < 2)
        {
            HideDirections();
            return;
        }

        for (int i = 0; i < PointVisualizator.selected.Count - 1; i++)
        {
            PointVisualizator.selected[i].Number = Random.Range(1, 5);
        }
        PointVisualizator.selected[PointVisualizator.selected.Count - 1].Number = Random.Range(1, 5);

        score += PointVisualizator.selected.Count;
        moves--;

        scoreLable.text = (score + ScoreLast).ToString();
        movesLable.text = moves.ToString();

        HideDirections();

        progressFill.fillAmount = 1f * score / targetForLevel;

        if(score >= targetForLevel)
        {
            ShowResult(true);
        }
        else if(moves == 0)
        {
            ShowResult(false);
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

    private void HideDirections()
    {
        PointVisualizator.selected.Clear();
        foreach (var visualizator in visualizators) visualizator.SetDirection(false);
    }

    private void UpdateStartScreen()
    {
        startBestLable.text = $"BEST SCORE: {BestScore}";
        startLvlLable.text = $"LEVEL: {MaxLevel}";
    }
}
