using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    Vector3 startPosition;
    public float swaySensitivity = 2f;
    public float swayClamp = 20f;

    public float swaySmoothness = 20f;


    Vector3 nextPosition;
    Vector3 currentVelocity = Vector3.zero;

    public PhotonView photonView;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        if (PhotonNetwork.InRoom && !photonView.IsMine)
        {
            return;
        }

        float mouseX = Input.GetAxis("Mouse X") * swaySensitivity/100 * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * swaySensitivity / 100 * Time.deltaTime;


        mouseX = Mathf.Clamp(mouseX, -swaySensitivity, swayClamp);
        mouseY = Mathf.Clamp(mouseY, -swayClamp, swayClamp);

        nextPosition = new Vector3(mouseX, mouseY,0);

        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, nextPosition + startPosition, ref currentVelocity, swaySmoothness * Time.deltaTime);

    }
}
