using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
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
    public void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        cameraOriginalRotation = playerCamera.transform.localRotation;
        WeaponSwitch(0);
    }

    private void Update()
    {

        if(hurtPanel.alpha > 0)
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
        health -= damage;
        gameManager.SetPlayerHealth(health);

        if(health <= 0)
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

    public void CameraShake()
    {
        playerCamera.transform.localRotation = Quaternion.Euler(Random.Range(-2, 2), 0f, 0f);
    }


   public void WeaponSwitch(int weaponIdx)
    {

        int index = 0;
        int amountOfWeapons = weaponHolder.transform.GetChildCount();

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
                child.gameObject.SetActive(true);
            }

            index++;
        }

        activeWeaponIdx = weaponIdx;
    }
    
}
