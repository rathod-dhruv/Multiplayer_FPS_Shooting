using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    public float health = 100;
    public float healthCap = 100;

    GameManager gameManager;

    [SerializeField]
    private GameObject playerCamera;
    [SerializeField]
    private CanvasGroup hurtPanel;

    private Quaternion cameraOriginalRotation;
    private float shakeDuration;
    private float shakeTime;

    public GameObject weaponHolder;
    private int activeWeaponIdx;
    private GameObject activeWeapon;

    public float curPoints = 0;
    [SerializeField]
    private TextMeshProUGUI pointsText;

    public PhotonView photonView;

    public void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        cameraOriginalRotation = playerCamera.transform.localRotation;
        WeaponSwitch(0);
    }

    private void Update()
    {
        if (PhotonNetwork.InRoom && !photonView.IsMine)
        {
            playerCamera.SetActive(false);
            return;
        }


        if (hurtPanel.alpha > 0)
        {
            hurtPanel.alpha -= Time.deltaTime;
        }
        if(shakeTime < shakeDuration)
        {
            shakeTime += Time.deltaTime;
            CameraShake();
        }
        else if(playerCamera.transform.localRotation != cameraOriginalRotation)
        {
            playerCamera.transform.localRotation = cameraOriginalRotation;
        }

        if(Input.GetKeyDown(KeyCode.Z))
        {
            WeaponSwitch(activeWeaponIdx + 1);
        }

        pointsText.text = "Points : " + curPoints.ToString();
    }
    public void Hit(float damage)
    {
        if(PhotonNetwork.InRoom)
        {
            photonView.RPC("TakeDamage", RpcTarget.All, damage, photonView.ViewID);
        }
        else
        {
            TakeDamage(damage, photonView.ViewID);
        }
    }
    [PunRPC]
    public void TakeDamage(float damage, int viewID)
    {
        if(photonView.ViewID == viewID)
        {
            health -= damage;
            gameManager.SetPlayerHealth(health);

            if (health <= 0)
            {
                gameManager.EndGame();
            }
            else
            {
                shakeTime = 0;
                shakeDuration = 0.2f;
                hurtPanel.alpha = 1;
            }
        }
        
    }
    public void CameraShake()
    {
        playerCamera.transform.localRotation = Quaternion.Euler(Random.Range(-2, 2), 0f, 0f);
    }


   public void WeaponSwitch(int weaponIdx)
    {

        int index = 0;
        int amountOfWeapons = weaponHolder.transform.childCount;

        if(weaponIdx > amountOfWeapons - 1)
        {
            weaponIdx = 0;
        }
        foreach(Transform child in weaponHolder.transform)
        {
            if(child.gameObject.activeSelf)
            {
                child.gameObject.SetActive(false);
            }
            if(index == weaponIdx)
            {
                activeWeapon = child.gameObject;
                child.gameObject.SetActive(true);
            }

            index++;
        }

        activeWeaponIdx = weaponIdx;

        if(photonView.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("weaponIndex", weaponIdx);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }


    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if(!photonView.IsMine && targetPlayer == photonView.Owner && changedProps["weaponIndex"] != null)
        {
            WeaponSwitch((int)changedProps["weaponIndex"]);
        }
    }

    [PunRPC]
    public void WeaponShootVFX(int viewID)
    {
        activeWeapon.GetComponent<WeaponManager>().ShootVFX(viewID);
    }


}
