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
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        navAgent = GetComponent<NavMeshAgent>();
        gameManager = FindObjectOfType<GameManager>();
        slider.maxValue = health;
        slider.value = health;
    }

    // Update is called once per frame
    void Update()
    {
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
        health -= damage;
        slider.value = health;
        Debug.Log("Health Reduce");
        if(health <= 0)
        {
            anim.SetTrigger("isDead");

            gameManager.enemiesAlive--;
            Destroy(GetComponent<NavMeshAgent>());
            Destroy(GetComponent<EnemyManager>());
            Destroy(gameObject,5f);
        }
    }
}
