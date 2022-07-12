using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class GameManager : MonoBehaviourPunCallbacks
{
    public  int enemiesAlive = 0;
    public int round = 0;
    public GameObject enemyPrefab;
    public GameObject[] spawnPoints;
    public TextMeshProUGUI roundText;

    public TextMeshProUGUI playerHealthText;
    public GameObject gameOverScreen;
    public GameObject pauseScreen;
    public PhotonView photonView;
    // Start is called before the first frame update
    private void Start()
    {
        gameOverScreen.SetActive(false);
        spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoints");
    }

    // Update is called once per frame
    void Update()
    {
        if(!PhotonNetwork.InRoom || PhotonNetwork.IsMasterClient && photonView.IsMine)
        {
            if(enemiesAlive == 0)
            {
                round++;
                NextWave(round);
                if(PhotonNetwork.InRoom)
                {
                    Hashtable hash = new Hashtable();
                    hash.Add("currentRound", round);
                    PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
                }
                else
                {
                    DisplayNextRound(round);
                }
                
            }
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
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform;
            GameObject enemySpawned;
            if(PhotonNetwork.InRoom)
            {
                enemySpawned = PhotonNetwork.Instantiate("Zombie", spawnPoint.transform.position, Quaternion.identity);
            }
            else
            {
                enemySpawned = Instantiate(Resources.Load("Zombie"), spawnPoint.transform.position, Quaternion.identity) as GameObject;
            }
            
            enemySpawned.GetComponent<EnemyManager>().gameManager = GetComponent<GameManager>();
            enemiesAlive++;

        }
    }

    public void StartGame()
    {
        if (!PhotonNetwork.InRoom)
        {
            Time.timeScale = 1;
        }
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene(1);
    }

    public void EndGame()
    {
        if (gameOverScreen.activeSelf)
            return;
        if(!PhotonNetwork.InRoom)
        {
            Time.timeScale = 0;
        }
        Cursor.lockState = CursorLockMode.None;
        gameOverScreen.SetActive(true);
        
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
        if (!PhotonNetwork.InRoom)
        {
            Time.timeScale = 0;
        }
        Cursor.lockState = CursorLockMode.None;
        pauseScreen.SetActive(true);

    }

    public void ContinueGame()
    {
        if (!PhotonNetwork.InRoom)
        {
            Time.timeScale = 1;
        }
        Cursor.lockState = CursorLockMode.Locked;
        pauseScreen.SetActive(false);
    }

    private void DisplayNextRound(float round)
    {
        roundText.text = "Round : " + round.ToString();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if(photonView.IsMine)
        {
            if(changedProps["currentRound"] != null)
            {
                DisplayNextRound((int)changedProps["currentRound"]);
            }
        }
    }
}
