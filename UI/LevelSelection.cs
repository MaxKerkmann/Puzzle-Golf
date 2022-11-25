using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelection : MonoBehaviour
{
    private int currentSelectedLevel;

    [SerializeField] private string levelBaseName = "Level ";
    void Start()
    {
        if (CrossSceneNetworkData.lastLevel < 0)
            currentSelectedLevel = CrossSceneNetworkData.lastLevel;
        else
            currentSelectedLevel = 1;

        SetSelectedLevelButton();
    }
    

    private void SetSelectedLevelButton()
    {

    }
}
