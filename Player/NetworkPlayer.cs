using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;


public class NetworkPlayer : NetworkBehaviour
{
    [Header("Movement")]
    [SerializeField] private InputActionProperty movementInput;
    [SerializeField] float playerSpeed = 2.0f;
    [SerializeField] float playerWidth = 0.2f;
    [SerializeField] Camera mainCam;

    [Header("Appearence")]

    private bool isLocal = true;

    //Network
    private NetworkVariable<NetworkString> playerName = new NetworkVariable<NetworkString>();
    private bool setName = false;
    

    private void Start()
    {

    }

    public override void OnNetworkSpawn()
    {
        isLocal = false;
        if (IsServer)
        {
            playerName.Value = CrossSceneNetworkData.PlayerName;
        }
        
    }

    void Update()
    {
        if (!isLocal)
        {
            if (!IsOwner) return;
        }
        HandleMovement();

        if (!setName && !string.IsNullOrEmpty(playerName.Value))
        {
            SetPlayerName();
            setName = true;
        }

    }

    public void SetPlayerName()
    {
        var playerNametag = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        playerNametag.text = playerName.Value;
    }

    void HandleMovement()
    {
        float playerVerticalInput = Input.GetAxis("Vertical");
        float playerHorizontalInput = Input.GetAxis("Horizontal");
        Vector3 forward = Camera.main.transform.forward;   
        Vector3 right = Camera.main.transform.right;
        forward.y = 0;
        right.y = 0;
        forward = forward.normalized;
        right = right.normalized;

        Vector3 verticalRelative = playerVerticalInput * forward;
        Vector3 horizontalRelative = playerHorizontalInput * right;

        Vector3 cameraMoveDir = verticalRelative + horizontalRelative;
        Vector3 targetMovePosition = this.transform.position + cameraMoveDir * playerSpeed * Time.deltaTime;

         if(TestMovementDirection(cameraMoveDir))
         {
             //Can move here
            this.transform.Translate(cameraMoveDir * playerSpeed * Time.deltaTime, Space.World);

         }
         else
         {
            //Cannot move here
            cameraMoveDir = horizontalRelative;
            if (TestMovementDirection(cameraMoveDir))
            {
                //Can move horizontal
                this.transform.Translate(cameraMoveDir * playerSpeed * Time.deltaTime, Space.World);

            }
            else
            {
                //Cannot move horizontal
                cameraMoveDir = verticalRelative;
                if (TestMovementDirection(cameraMoveDir))
                {
                    //Can move vertical
                    this.transform.Translate(cameraMoveDir * playerSpeed * Time.deltaTime, Space.World);
                }
            }
        } 
    }

    bool TestMovementDirection(Vector3 movementDir)
    {
        for(int x = 0; x < mainCam.transform.position.y / 10; x++)
        {
            if(Physics.Raycast(new Vector3(transform.position.x,transform.position.y + x*10,transform.position.z), movementDir, playerSpeed * Time.deltaTime + playerWidth / 2))
            {
                return false;
            }
        }
        return true;
    }
}
