using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuGameManager : MonoBehaviour
{
    [Header("Managers")]
    public GameObject SettingsManager;
    [Header("Settings Menu")]
    public GameObject optionsMenuHolder;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowSettingsMenu()
    {
        optionsMenuHolder.SetActive(true);
    }

    public void HideSettingsMenu()
    {
        optionsMenuHolder.SetActive(false);
    }

    public void StartGame()
    {
        switch (SettingsManager.GetComponent<SettingsManager>().vrSetting)
        {
            case global::SettingsManager.VRSelectables.NoVR:
                //Load non vr scene
                SceneManager.LoadScene(1);
                break;
            case global::SettingsManager.VRSelectables.OculusRift:
                //Load Oculus VR scene
                Debug.Log("I got into here");
                SceneManager.LoadScene(2);
                break;
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
