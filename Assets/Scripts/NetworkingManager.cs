using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class NetworkingManager : MonoBehaviourPunCallbacks
{
    public GameObject connecting;
    public GameObject multiplayer;
    // Start is called before the first frame update
    void Start()
    {
        connecting.SetActive(true);
        multiplayer.SetActive(false);
        Debug.Log("Connecting To Server");
        PhotonNetwork.ConnectUsingSettings();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Joining Lobby");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Ready For Multiplayer");
        connecting.SetActive(false);
        multiplayer.SetActive(true);
    }

    public void FindMatch()
    {
        Debug.Log("Finding Room");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        MakeRoom();
    }

    void MakeRoom()
    {
        int randomNumber = Random.Range(0, 5000);
        RoomOptions roomOptions = new RoomOptions()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = 6,
            PublishUserId = true,
        };
        PhotonNetwork.CreateRoom("RoomName_" + randomNumber, roomOptions);
        Debug.Log("Room Made : " + randomNumber);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Loading 1");
        PhotonNetwork.LoadLevel(2);
    }
}
