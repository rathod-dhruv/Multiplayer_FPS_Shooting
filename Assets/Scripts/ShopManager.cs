using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public int price = 50;

    public Text priceText;
    public bool HealthStation;
    public bool AmmoStation;

    PlayerManager playerManager;
    bool playerIsInReach = false;
    // Start is called before the first frame update
    void Start()
    {
        priceText.text = "Price : "+price.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if(playerIsInReach)
            if(Input.GetKeyDown(KeyCode.E))
            {
                BuyShop();
            }
        
    }
    public void BuyShop()
    {

        if (playerManager.curPoints >= price)
        {
            playerManager.curPoints -= price;

            if(HealthStation)
            {
                playerManager.health = playerManager.healthCap;
            }

            if(AmmoStation)
            {
                WeaponManager[] objs = FindObjectsOfType<WeaponManager>();
                foreach(WeaponManager w in objs)
                {
                    w.reservedAmmo = w.ammoCap;
                }

            }
        }
        else
        {

            priceText.text = "Poor";

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            priceText.gameObject.SetActive(true);
            playerIsInReach = true;
            playerManager = other.GetComponent<PlayerManager>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            priceText.gameObject.SetActive(false);
            playerIsInReach = false;
        }
    }
}
