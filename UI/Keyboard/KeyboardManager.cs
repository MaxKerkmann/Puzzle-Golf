using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KeyboardManager : MonoBehaviour
{
    [SerializeField] Keyboard keyboard;
    [SerializeField] GameObject player;

    [SerializeField] GameObject leftTypingHand;
    [SerializeField] GameObject rightTypingHand;
    void Start()
    {
      //  keyboard = GetComponent<Keyboard>();
    }

    public void createKeyboard(TMP_InputField inputField)
    {
        keyboard.gameObject.SetActive(true);
        keyboard.SetInputField(inputField);
        keyboard.transform.position = player.transform.position + new Vector3(0, -player.transform.position.y * 0.35f, player.transform.forward.z * 0.4f);
    }

    public void disableKeyboard()
    {

        keyboard.transform.position = Vector3.zero;
        leftTypingHand.SetActive(false);
        rightTypingHand.SetActive(false);
        keyboard.gameObject.SetActive(false);
    }
}
