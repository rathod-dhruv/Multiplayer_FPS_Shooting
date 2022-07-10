using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public  int enemiesAlive = 0;
    public int round = 0;
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public TextMeshProUGUI roundText;

    public TextMeshProUGUI playerHealthText;
    public GameObject gameOverScreen;
    public GameObject pauseScreen;
    // Start is called before the first frame update
    private void Start()
    {
        gameOverScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(enemiesAlive == 0)
        {
            round++;
            NextWave(round);
            roundText.text = "Round : " + round.ToString();
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
    }

    public void NextWave(int round)
    {
        for(int i = 0; i < round; i++)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Instantiate(enemyPrefab, spawnPoint.transform.position, Quaternion.identity);
            enemiesAlive++;
        }
    }

    public void StartGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene(1);
        Time.timeScale = 1;
    }

    public void EndGame()
    {
        if (gameOverScreen.activeSelf)
            return;
        Cursor.lockState = CursorLockMode.None;
        gameOverScreen.SetActive(true);
        Time.timeScale = 0;
    }

    public void SetPlayerHealth(float health)
    {
        playerHealthText.text = "Health : " + health.ToString();
    }

    public void LoadMenuScene()
    {
        SceneManager.LoadScene(0);
    }

    public void PauseGame()
    {
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
        pauseScreen.SetActive(true);

    }

    public void ContinueGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
        pauseScreen.SetActive(false);
    }
}
