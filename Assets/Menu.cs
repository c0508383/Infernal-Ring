using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
//using UnityEngine.InputSystem;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Menu : MonoBehaviour

      
{
  //  public GameObject pauseMenu, optionsMenu;
    public AudioMixer audioMixer;
    public TMPro.TMP_Dropdown resolutionDropdown;

    Resolution[] resolutions;
    //public GameObject pauseFirstButton, optionsFirstButton, optionsClosedButton;

    void Start()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetFullscreen (bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
    /*void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Fire3"))
        {
            PauseUnpause();
        }
    }

    public void PauseUnpause()
    {
        if (!pauseMenu.activeInHierarchy)
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;

            //clears selected object
            EventSystem.current.SetSelectedGameObject(null);
            //set new selected object
            EventSystem.current.SetSelectedGameObject(pauseFirstButton);
        }
        else
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
            optionsMenu.SetActive(false);
        }
    }
    */
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);

    }
    public void SingleplayerGame()
    {
        SceneManager.LoadScene("StoryMode");
    }

    public void MultiplayerGame()
    {
        SceneManager.LoadScene("VersusMode");
    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    /*public void OpenOptions()
    {
        optionsMenu.SetActive(true);

        //clears selected object
        EventSystem.current.SetSelectedGameObject(null);
        //set new selected object
        EventSystem.current.SetSelectedGameObject(optionsFirstButton);
    }

    public void CloseOptions()
    {
        optionsMenu.SetActive(false);

        //clears selected object
        EventSystem.current.SetSelectedGameObject(null);
        //set new selected object
        EventSystem.current.SetSelectedGameObject(optionsClosedButton);
    }
    */
}