using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [SerializeField] RectTransform rectTransform;
    [SerializeField] TMP_Dropdown resolutionDropdown;
    [SerializeField] TMP_Dropdown windowModeDropdown;
    [SerializeField] Button applySettingsButton;
    [SerializeField] float animDuration;
    [SerializeField] private LeanTweenType entryAnimType;
    [SerializeField] private LeanTweenType exitAnimType;
    [SerializeField] bool menuEnabled;

    private static List<Resolution> supportedResolutions = new();
    private static FullScreenMode fullScreenMode;
    private static int selectedScreenMode = 0;
    private static Resolution currentResolution;
    private static int selectedResolution = 0;

    int currentTweenID = -1;
    private void Awake()
    {
        List<string> resOptionsStrings = new List<string>();
        var resolutions = Screen.resolutions;
        foreach (var resolution in resolutions)
        {
            float monitorAspectRatio = Screen.width / Screen.height;
            float aspectRatio = resolution.width / resolution.height;

            if (aspectRatio == monitorAspectRatio && resolution.refreshRateRatio.value == Screen.mainWindowDisplayInfo.refreshRate.value)
            {
                supportedResolutions.Add(resolution);
                string resOptionStr = resolution.ToString();
                int index = resOptionStr.IndexOf("@");
                //Debug.Log(resOptionStr);
                resOptionStr = resOptionStr.Substring(0, index);

                resOptionsStrings.Add(resOptionStr);
            }
        }
        resolutionDropdown.AddOptions(resOptionsStrings);
    }

    void Start()
    {
        //Debug.Log(currentResolution);
        LeanTween.reset();
        resolutionDropdown.value = selectedResolution;
        windowModeDropdown.value = selectedScreenMode;
    }

    //used in editor
    public void ChangeScreenResolution()
    {
        selectedResolution = resolutionDropdown.value;
        currentResolution = supportedResolutions[selectedResolution];
        Screen.SetResolution(currentResolution.width, currentResolution.height, fullScreenMode, currentResolution.refreshRateRatio);
        SaveSettings();
    }

    //used in editor
    public void ChangeWindowMode()
    {
        selectedScreenMode = windowModeDropdown.value;
        switch (selectedScreenMode)
        {
            case 0:
                fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
            case 1:
                fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case 2:
                fullScreenMode = FullScreenMode.Windowed;
                break;
        }
        Screen.fullScreenMode = fullScreenMode;
        SaveSettings();
    }

    //used in editor
    public void HandleDropdownChange()
    {
        if(selectedResolution != resolutionDropdown.value || selectedScreenMode != windowModeDropdown.value)
            applySettingsButton.interactable = true;
        else
            applySettingsButton.interactable = false;
    }

    //used in editor
    public void ToggleSettingsMenu()
    {

        if (currentTweenID != -1)
        {
            LeanTween.cancel(currentTweenID);
        }

        if (!menuEnabled)
            currentTweenID = LeanTween.moveX(rectTransform, 0f, animDuration).setEase(entryAnimType).setOnComplete(() => currentTweenID = -1).uniqueId;
        else
            currentTweenID = LeanTween.moveX(rectTransform, 620f, animDuration).setEase(exitAnimType).setOnComplete(() => currentTweenID = -1).uniqueId;

        menuEnabled = !menuEnabled;
    }

    private void SaveSettings()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
            return;

        PlayerPrefs.SetInt("Resolution", selectedResolution);
        PlayerPrefs.SetInt("ScreenMode", selectedScreenMode);
    }

    private void OnApplicationQuit()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
            return;

        SaveSettings();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void LoadSettings()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
            return;

        if(PlayerPrefs.HasKey("Resolution") && PlayerPrefs.HasKey("ScreenMode"))
        {
            selectedResolution = PlayerPrefs.GetInt("Resolution");
            selectedScreenMode = PlayerPrefs.GetInt("ScreenMode");

            fullScreenMode = (FullScreenMode)selectedScreenMode;
            currentResolution = supportedResolutions[selectedResolution];
        }
        else //default to display's max resolution
        {
            currentResolution.width = Screen.mainWindowDisplayInfo.width;
            currentResolution.height = Screen.mainWindowDisplayInfo.height;

            fullScreenMode = FullScreenMode.FullScreenWindow;
            selectedScreenMode = 0;
            selectedResolution = supportedResolutions.Count - 1;
            Debug.Log(currentResolution);
        }
        Screen.SetResolution(currentResolution.width, currentResolution.height, fullScreenMode, Screen.mainWindowDisplayInfo.refreshRate);
    }

    [ContextMenu("Reset PlayerPrefs Keys")]
    public void ResetPlayerPrefsData()
    {
        PlayerPrefs.DeleteKey("Resolution");
        PlayerPrefs.DeleteKey("ScreenMode");
    }
}
