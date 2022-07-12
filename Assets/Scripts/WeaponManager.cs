using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class WeaponManager : MonoBehaviour
{

    [SerializeField]
    private Transform shootPoint;
    [SerializeField]
    private float damageForEnemy = 20f;

    [SerializeField]
    private float maxDistance = 100f;
    [SerializeField]
    private ParticleSystem fireParticle;

    [SerializeField]
    private Animator weaponAnimator;

    [SerializeField]
    private GameObject hitParticleEffect;
    [SerializeField]
    private GameObject nonTargetHitParticles;


    public AudioSource audioSource;
    public AudioClip gunShot;


    public WeaponSway weaponSway;
    float swaySensitiviy;
    public GameObject crossHair;


    public float currentAmmo;
    public float maxAmmo;

    public float reloadTime = 2f;

    bool isReloading;
    public float reservedAmmo;
    public float ammoCap;

    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI reserveAmmoText;

    public float firerateTimer = 0;
    public float firerate = 10;

    public bool isAutomatic;
    public string WeaponType;

    public PlayerManager playerManager;

    public PhotonView photonView;

    private void OnEnable()
    {
        weaponAnimator.SetTrigger(WeaponType);
        ammoText.text = currentAmmo.ToString();
        reserveAmmoText.text = reservedAmmo.ToString();
    }
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        swaySensitiviy = weaponSway.swaySensitivity;
        ammoText.text = currentAmmo.ToString();
        reserveAmmoText.text = reservedAmmo.ToString();

    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.InRoom && !photonView.IsMine)
        {
            return;
        }

        if (weaponAnimator.GetBool("isShooting"))
        {
            weaponAnimator.SetBool("isShooting", false);
        }
        if (isReloading)
        {
            return;

        }
        if (currentAmmo <= 0 && !isReloading)
        {
            StartCoroutine(Reload(reloadTime));
            return;
        }

        if(currentAmmo <=0 && reservedAmmo <= 0)
        {
            Debug.Log("No Ammo");
            return;
        }

        if(Input.GetKeyDown(KeyCode.R) && reservedAmmo > 0)
        {
            StartCoroutine(Reload(reloadTime));
        }


        if(firerateTimer > 0)
        {
            firerateTimer -= Time.deltaTime;
        }
        
        if (Input.GetButton("Fire1") && firerateTimer <= 0 && isAutomatic)
        {
            Shoot();
            firerateTimer = 1 / firerate ;
        }


        if (Input.GetButtonDown("Fire1") && firerateTimer <= 0)
        {
            Shoot();
            firerateTimer = 1 / firerate;
        }

        if (Input.GetButtonDown("Fire2"))
        {
            Aim();
        }

        if (Input.GetButtonUp("Fire2"))
        {
            if (weaponAnimator.GetBool("isAiming"))
            {
                weaponAnimator.SetBool("isAiming", false);
            }

            swaySensitiviy = weaponSway.swaySensitivity;
            crossHair.SetActive(true);
        }

        
    }

    public IEnumerator Reload(float st)
    {
        isReloading = true;
        weaponAnimator.SetBool("isReloading", true);
        yield return new WaitForSeconds(st);

        float missingAmmo = maxAmmo - currentAmmo;
        weaponAnimator.SetBool("isReloading", false);

        if (reservedAmmo >= missingAmmo)
        {
            currentAmmo += missingAmmo;
            reservedAmmo -= missingAmmo;
            ammoText.text = currentAmmo.ToString();
            reserveAmmoText.text = reservedAmmo.ToString();
        }

        else
        {
            currentAmmo += reservedAmmo;
            reservedAmmo = 0;
            ammoText.text = currentAmmo.ToString();
            reserveAmmoText.text = reservedAmmo.ToString();
        }
        isReloading = false;
    }
    private void Aim()
    {
        weaponAnimator.SetBool("isAiming", true);
        swaySensitiviy = weaponSway.swaySensitivity / 3;
        crossHair.SetActive(false);
    }

    private void Shoot()
    {
        currentAmmo--;
        ammoText.text = currentAmmo.ToString();
        weaponAnimator.SetBool("isShooting", true);
        RaycastHit hit;
        if(Physics.Raycast(shootPoint.position, shootPoint.transform.forward, out hit, maxDistance))
        {
            Debug.Log("Hit");
            EnemyManager enemyManager = hit.transform.GetComponent<EnemyManager>();
            if (enemyManager != null)
            {
                enemyManager.Hit(damageForEnemy);
                if(enemyManager.health <= 0)
                {
                    playerManager.curPoints += enemyManager.points;
                }
                GameObject tempParticle = Instantiate(hitParticleEffect, hit.point, Quaternion.LookRotation(hit.normal));
                tempParticle.transform.parent = hit.transform;
                Destroy(tempParticle, 1.3f);
            }

            else if(hit.collider.tag != "Enemy")
            {
                GameObject InstParticles = Instantiate(nonTargetHitParticles, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(InstParticles, 5f);
            }   
        }
        
        if(PhotonNetwork.InRoom)
        {
            photonView.RPC("WeaponShootVFX", RpcTarget.All, photonView.ViewID);
        }
        else
        {
            ShootVFX(photonView.ViewID);
        }
    }

    public void ShootVFX(int viewID)
    {
        if(photonView.ViewID == viewID)
        {
            fireParticle.Play();
            audioSource.PlayOneShot(gunShot);
        }
    }
    private void OnDisable()
    {
        weaponAnimator.SetBool("isReloading", false);
        isReloading = false;

    }
}
