using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField]
    private float mouseSensitivity = 10f;
    [SerializeField]
    private Transform playerBody;

    private float xRotation = 0;
    [SerializeField]
    private float xMax, xMin;
    [SerializeField]
    public bool isCursorLock;

    public PhotonView photonView;
    // Start is called before the first frame update
    void Start()
    {
        if(isCursorLock)
            Cursor.lockState = CursorLockMode.Locked;   
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.InRoom && !photonView.IsMine)
        {
            return;
        }
        RotateView();
    }

    private void RotateView()
    {
        Debug.Log("ROTATE");
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;


        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, xMin, xMax);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);



    }
}
