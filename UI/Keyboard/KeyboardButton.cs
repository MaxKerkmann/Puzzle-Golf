using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KeyboardButton : MonoBehaviour
{
    private Keyboard keyboard;
    private TextMeshProUGUI buttonText;
    void Start()
    {
        keyboard = GetComponentInParent<Keyboard>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        if(buttonText.text.Length == 1)
        {
            SetButtonText();
            GetComponentInChildren<ThreeDButton>().onRelease.AddListener(() => { keyboard.typeChar(buttonText.text); });
        }
    }

    private void SetButtonText()
    {
        buttonText.text = gameObject.name;
    }


}
