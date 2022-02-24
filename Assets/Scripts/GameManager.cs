using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


public class GameManager : MonoBehaviour
{

    public static string SETTINGS_VIBRATIONS = "settings_vibrations";
    public static string PLAYER_SCORE = "player_score";
    public static string FIRST_GAME = "first_game";


    [Header("Settings")]
    public GameObject[] enemySets = new GameObject[4];
    public GameObject bigEnemy;
    public GameObject enemyRow;
    public int enemiesInRow = 4;

    [Space(10)]

    [Header("Panels")]
    public GameObject startPanel;
    public GameObject endPanel;
    public GameObject settingsPanel;

    [HideInInspector]
    public bool isEnd = false;

    private int enemies;
    private GameObject player;
    private bool canDeployBigEnemy = true;
    private GameObject bigEnemyClone;
    private Vector2 bigEnemyDirection;






    private void Awake()
    {
        Application.targetFrameRate = 120;
        if (!PlayerPrefs.HasKey(SETTINGS_VIBRATIONS)) PlayerPrefs.SetInt(SETTINGS_VIBRATIONS, 1);
        if (!PlayerPrefs.HasKey(PLAYER_SCORE)) PlayerPrefs.SetInt(PLAYER_SCORE, 0);
        if (!PlayerPrefs.HasKey(FIRST_GAME)) PlayerPrefs.SetInt(FIRST_GAME, 0);


    }


    void Start()
    {
        player = GameObject.FindWithTag("Player");
        enemies = enemySets.Length * enemiesInRow;
        if (PlayerPrefs.GetInt(FIRST_GAME) == 0) ShowStartPanel();
        SpawnEnemies();
    }

    private void FixedUpdate()
    {

        //Controll big enemies
        if (canDeployBigEnemy && !isEnd) StartCoroutine(DeployBigEnemy(Random.Range(20, 30)));
        if (bigEnemyClone != null) {

            bigEnemyClone.transform.Translate(bigEnemyDirection * .5f* Time.deltaTime);
        }

    }

    private IEnumerator DeployBigEnemy(float delay) {
        canDeployBigEnemy = false;
        int i = Random.value >= .5f ? 1 : -1;
        bigEnemyClone = Instantiate(bigEnemy,new Vector2(3*i,2.2f), Quaternion.identity);
        bigEnemyDirection = new Vector2(-1 * i, 0);

        yield return new WaitForSeconds(delay);

        canDeployBigEnemy = true;
    }

    #region Enemy Handle

    public void DecrementEnemy()
    {
        enemies--;
        CheckEnemies();
    }

    private void CheckEnemies()
    {
        int startEnemies = enemySets.Length * enemiesInRow;
        if (enemies == (int)startEnemies * .75f || enemies == (int)startEnemies * .5f || enemies == (int)startEnemies * .25f)
        {
            for (int i = 0; i < enemySets.Length; i++)
            {

                enemySets[i].GetComponent<EnemyRow>().speed += .2f;
                enemySets[i].GetComponent<EnemyRow>().MakeHarder();

            }
        }
        else if (enemies <= 0)
        {

            //Player won
            if (bigEnemyClone != null) Destroy(bigEnemyClone);

            int score = player.GetComponent<PlayerManager>().GetScore();
            if (!isEnd) ShowEndPanel("You Win!", score);
        }
    }

    private void SpawnEnemies()
    {
        for (int i = 0; i < enemySets.Length; i++)
        {
            enemyRow.GetComponent<EnemyRow>().enemy = enemySets[i];
            enemyRow.GetComponent<EnemyRow>().enemiesInRow = enemiesInRow;

            enemySets[i] = Instantiate(enemyRow, new Vector2(0, -.1f + (.5f * i)), Quaternion.identity);
        }
    }

    #endregion

    #region Panels

    #region Start Panel

    private void ShowStartPanel() {
        player.GetComponent<PlayerManager>().SetActivePlayerUI(false);
        Time.timeScale = 0;

        startPanel.SetActive(true);
    }

    public void HideStartPanel()
    {
        PlayerPrefs.SetInt(FIRST_GAME, 1);

        player.GetComponent<PlayerManager>().SetActivePlayerUI(true);
        Time.timeScale = 1;

        startPanel.SetActive(false);
    }


    #endregion

    #region End Panel
    public void ShowEndPanel(string message, int score)
    {
        player.GetComponent<PlayerManager>().SetActivePlayerUI(false);
        if(!isEnd)StartCoroutine(ShowEndPanelWithDelay(message,score,1));
        isEnd = true;

    }

    private IEnumerator ShowEndPanelWithDelay(string message, int score, float delay) {
        yield return new WaitForSeconds(delay);

        string bestScore = "Best Score: " + PlayerPrefs.GetInt(PLAYER_SCORE).ToString();
        string yourScore = "";
        if (score == 0) bestScore = "";
        
        else if (PlayerPrefs.GetInt(PLAYER_SCORE) < score)
        {
            PlayerPrefs.SetInt(PLAYER_SCORE, score);
            yourScore = "New Best Score";
            bestScore = "Best Score: " + score.ToString();
        }

        else yourScore = "Your Score: "+score;

        endPanel.transform.GetChild(0).Find("End text (TMP)").GetComponent<TextMeshProUGUI>().text = message;
        endPanel.transform.GetChild(0).Find("Your Score(TMP)").GetComponent<TextMeshProUGUI>().text = yourScore;
        endPanel.transform.GetChild(0).Find("Best Score(TMP)").GetComponent<TextMeshProUGUI>().text = bestScore;

        endPanel.SetActive(true);

    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(0);
    }

    #endregion

    #region Settings

    public void ShowSettings()
    {
        player.GetComponent<PlayerManager>().SetActivePlayerUI(false);
        Time.timeScale = 0;
        bool val = PlayerPrefs.GetInt(SETTINGS_VIBRATIONS) >= 1 ? true : false;
        settingsPanel.transform.GetChild(0).Find("Settings").GetChild(0).GetChild(0).GetComponent<Toggle>().isOn = val;
        settingsPanel.SetActive(true);
    }

    public void HideSettings()
    {
        player.GetComponent<PlayerManager>().SetActivePlayerUI(true);
        Time.timeScale = 1;
        settingsPanel.SetActive(false);
    }

    public void SetVibrations(bool val)
    {
        PlayerPrefs.SetInt(SETTINGS_VIBRATIONS, val ? 1 : 0);
    }

    #endregion

    #endregion

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt(FIRST_GAME,0);
    }
}
