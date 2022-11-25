using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class PlayerAnimationController: NetworkBehaviour
{

    [SerializeField] InputActionProperty pinchAnimation;
    [SerializeField] Animator handAnimator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float triggerValue = pinchAnimation.action.ReadValue<float>(); 
        handAnimator.SetFloat("Trigger",triggerValue);
    }
}
