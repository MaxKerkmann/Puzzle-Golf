using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Keyboard : MonoBehaviour
{
    private TMP_InputField inputField;
    [SerializeField] private GameObject smallButtons;
    [SerializeField] private GameObject bigButtons;
    private bool caps;


    void Start()
    {
        caps = false;
    }

    public void typeChar(string c)
    {
        inputField.text += c;
    }

    public void useBackspace()
    {
        if(inputField.text.Length > 0)
        {
            inputField.text = inputField.text.Substring(0, inputField.text.Length - 1);     
        }
    }

    public void useSpace()
    {
        inputField.text += " ";
    }

    public void useCaps()
    {
        if (!caps)
        {
            smallButtons.SetActive(false);
            bigButtons.SetActive(true);
            caps = true;
        }
        else
        {
            smallButtons.SetActive(true);
            bigButtons.SetActive(false);
            caps = false;
        }

    }

    public void SetInputField(TMP_InputField inputField)
    {
        this.inputField = inputField;
    }
}
