using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{

    private GameObject player;
    [SerializeField]
    private Animator anim;
    private NavMeshAgent navAgent;
    public GameManager gameManager;
    public float health = 50f;
    [SerializeField]
    private Slider slider;

    public float damageForPlayer = 20f;

    private bool playerInReach;
    private float attackDelayTimer;

    public int points = 20;
    GameObject[] playersInScene;
    public PhotonView photonView;
    // Start is called before the first frame update
    void Start()
    {
        playersInScene = GameObject.FindGameObjectsWithTag("Player");
        
        gameManager = FindObjectOfType<GameManager>();
        slider.maxValue = health;
        slider.value = health;
    }

    // Update is called once per frame
    void Update()
    {
        if(PhotonNetwork.InRoom && !PhotonNetwork.IsMasterClient)
        {
            return;
        }
        GetClosestPlayer();
        if(player == null)
        {
            return;
        }
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.destination = player.transform.position;
        slider.transform.LookAt(player.transform);
        if(navAgent.velocity.magnitude > 1)
        {
            anim.SetBool("isRunning", true);
        }

        else
        {
            anim.SetBool("isRunning", false);
        }
    }

    private void GetClosestPlayer()
    {
        float minDist = Mathf.Infinity;
        Vector3 curPosition = transform.position;

        foreach(GameObject thisPlayer in playersInScene)
        {
            if(thisPlayer != null)
            {
                float distance = Vector3.Distance(thisPlayer.transform.position, curPosition);
                if(distance < minDist)
                {
                    player = thisPlayer;
                    minDist = distance;
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == "Player")
        {
            Debug.Log("Player Hit");
            playerInReach = true;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if(playerInReach)
        {
            attackDelayTimer += Time.deltaTime;
        }

        if (attackDelayTimer >= 0 && attackDelayTimer <= 0.5f && playerInReach)
        {
            anim.SetTrigger("isAttacking");
        }

        if(attackDelayTimer >= 0.5f && playerInReach)
        {
            player.GetComponent<PlayerManager>().Hit(damageForPlayer);
            attackDelayTimer = 0;
        }
    }



    private void OnCollisionExit(Collision collision)
    {
        if(collision.collider.tag == "Player")
        {
            playerInReach = false;
            attackDelayTimer = 0;
        }
    }

    public void Hit(float damage)
    {
        if(PhotonNetwork.InRoom)
        {
            photonView.RPC("TakeDamage", RpcTarget.All,damage, photonView.ViewID);
        }
        else
        {
            TakeDamage(damage, photonView.ViewID);
        }
        
    }

    [PunRPC]
    public void TakeDamage(float damage, int viewId)
    {
        if(photonView.ViewID == viewId)
        {
            health -= damage;
            slider.value = health;
            Debug.Log("Health Reduce");
            if (health <= 0)
            {
                anim.SetTrigger("isDead");

                if(!PhotonNetwork.InRoom || PhotonNetwork.IsMasterClient && photonView.IsMine)
                {
                    gameManager.enemiesAlive--;
                }

                Destroy(GetComponent<NavMeshAgent>());
                Destroy(GetComponent<EnemyManager>());
                Destroy(gameObject, 5f);
            }
        }
        
    }
}
