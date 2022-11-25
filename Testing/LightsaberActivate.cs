using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LightsaberActivate : MonoBehaviour
{
    [SerializeField] GameObject ligthswordBlade;
    bool swordOn = false;
    bool changeSword = false;

    float swordCounter = 0;
    // Start is called before the first frame update
    void Start()
    {
        XRGrabInteractable grabbable = GetComponent<XRGrabInteractable>();
        grabbable.activated.AddListener(ActivateSword);
    }

    // Update is called once per frame
    void Update()
    {
        if (changeSword)
        {
            if (swordOn)
            {
                swordCounter++;
                if(swordCounter <= 15)
                {
                    ligthswordBlade.transform.localScale = new Vector3(ligthswordBlade.transform.localScale.x, ligthswordBlade.transform.localScale.y, ligthswordBlade.transform.localScale.z + (18 / 15));
                }else
                    changeSword = false;
            }
            else
            {
                swordCounter++;
                if (swordCounter <= 15)
                {
                    ligthswordBlade.transform.localScale = new Vector3(ligthswordBlade.transform.localScale.x, ligthswordBlade.transform.localScale.y, ligthswordBlade.transform.localScale.z - (18 / 15));
                }else
                    changeSword= false;
            }
        }
    }

    public void ActivateSword(ActivateEventArgs args)
    {
        swordOn = !swordOn;
        changeSword = true;
        swordCounter = 0;
    }
}
