using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Game")]
    public string nextLevel;
    public int targetFrameRate;

    public GameObject player;
    public PlayerController playerController;
    public Vector3 playerStartPosition;

    public bool countTime;
    public float levelTimer;
    public GameObject levelEndUI;
    public TextMeshProUGUI levelTimerText;
    public TextMeshProUGUI levelEndTime;

    public bool timerStarted;

    [Header("Pause Menu")]
    public static bool gamePaused;
    public static bool settingsShown;
    public KeyCode pauseKey = KeyCode.Escape;
    public float regularTimeScale = 1f;
    public float pausedTimeScale = 0.001f;
    public GameObject pauseMenuUI;
    public GameObject settingsUI;
    

    [Header("Settings")]
    public PlayerCamera playerCamera;
    public Volume volume;
    public static float xSens = 400;
    public float ySens = 400;
    public float bloomIntensity = 0.3f;
    public float motionBlurIntensity;
    [Range(0,1)] public float chromaticAbberation = 0.3f;

    public TextMeshProUGUI xSensText;
    public TextMeshProUGUI ySensText;
    public TextMeshProUGUI bloomText;
    public TextMeshProUGUI motionBlurText;
    public TextMeshProUGUI chromaticAbberationText;

    public Slider xSensSlider;
    public Slider ySensSlider;
    public Slider bloomSlider;
    public Slider motionBlurSlider;
    public Slider chromaticAbberationSlider;

    [Header("Portal Texture")]
    public Camera targetPortalCamera;
    public Material targetPortalCameraMat;

    private void Awake()
    {
        LoadSettings();
        Resume();

        xSensSlider.onValueChanged.AddListener(delegate { OnSliderValueChanged(xSensSlider); });
        ySensSlider.onValueChanged.AddListener(delegate { OnSliderValueChanged(ySensSlider); });
        bloomSlider.onValueChanged.AddListener(delegate { OnSliderValueChanged(bloomSlider); });
        motionBlurSlider.onValueChanged.AddListener(delegate { OnSliderValueChanged(motionBlurSlider); });
        chromaticAbberationSlider.onValueChanged.AddListener(delegate { OnSliderValueChanged(chromaticAbberationSlider); });

        levelTimerText.text = levelTimer.ToString("0#.00");
        playerStartPosition = player.transform.position;

        if (targetPortalCamera)
        {
            if (targetPortalCamera.targetTexture != null)
                targetPortalCamera.targetTexture.Release();
            targetPortalCamera.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
            targetPortalCameraMat.mainTexture = targetPortalCamera.targetTexture;
        }
    }

    private void Update()
    {
        if (countTime)
        {
            levelTimer += Time.deltaTime;
            levelTimerText.text = levelTimer.ToString("0#.000");
        }

        // Application.targetFrameRate = targetFrameRate;
        //vDebug.Log(1/Time.deltaTime);

        playerCamera.sensX = xSens;
        playerCamera.sensY = ySens;

        if (volume.profile.TryGet(out Bloom bloom))
            bloom.intensity.value = bloomIntensity;

        if (volume.profile.TryGet(out MotionBlur motionBlur))
            motionBlur.intensity.value = motionBlurIntensity;

        if (Input.GetKeyDown(pauseKey))
        {
            if (!gamePaused && !settingsShown)
            {
                Pause();
            }
            else if (settingsShown)
            {
                HideSettings();
            } else
            {
                Resume();
            }
        }
    }

    public void Resume()
    {
        HideSettings();
        pauseMenuUI.SetActive(false);
        Time.timeScale = regularTimeScale;
        gamePaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (playerController != null)
        {
            playerController.canCast = true;
            playerController.canGrapple = true;
        }
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = pausedTimeScale;
        gamePaused = true;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        if (playerController != null)
        {
            playerController.canCast = false;
            playerController.canGrapple = false;
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void OnApplicationQuit()
    {
        SaveSettings();
    }

    public void ShowSettings()
    {
        pauseMenuUI.SetActive(false);
        settingsUI.SetActive(true);
        settingsShown = true;
        gamePaused = false;
    }

    public void HideSettings()
    {
        pauseMenuUI.SetActive(true);
        settingsUI.SetActive(false);
        settingsShown = false;
        gamePaused = true;
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void FinishLevel()
    {
        countTime = false;
        levelEndUI.SetActive(true);
        levelEndTime.text = levelTimerText.text;
        levelTimerText.gameObject.SetActive(false);
        foreach (Transform child in levelEndTime.transform)
        {
            child.GetComponent<TextMeshProUGUI>().text = levelTimerText.text;
        }

        player.GetComponent<PlayerController>().godMode = true;

        pauseKey = KeyCode.None;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        Time.timeScale = pausedTimeScale;
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(nextLevel);
    }

    public void Menu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("xSens", xSens);
        PlayerPrefs.SetFloat("ySens", ySens);
        PlayerPrefs.SetFloat("bloomIntensity", bloomIntensity);
        PlayerPrefs.SetFloat("motionBlurIntensity", motionBlurIntensity);
        PlayerPrefs.SetFloat("chromaticAbberation", chromaticAbberation);
        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        xSens = PlayerPrefs.GetFloat("xSens", 400);
        ySens = PlayerPrefs.GetFloat("ySens", 400);
        bloomIntensity = PlayerPrefs.GetFloat("bloomIntensity", 0.3f);
        motionBlurIntensity = PlayerPrefs.GetFloat("motionBlurIntensity", 0.3f);
        chromaticAbberation = PlayerPrefs.GetFloat("chromaticAbberation", 1.0f);

        xSensSlider.value = xSens / 10;
        ySensSlider.value = ySens / 10;
        bloomSlider.value = bloomIntensity;
        motionBlurSlider.value = motionBlurIntensity;
        chromaticAbberationSlider.value = chromaticAbberation;

        xSensText.text = (10 * Mathf.Round(xSensSlider.value)).ToString();
        ySensText.text = (10 * Mathf.Round(ySensSlider.value)).ToString();
        bloomText.text = bloomIntensity.ToString("0#.00");
        motionBlurText.text = motionBlurIntensity.ToString("0#.00");
        chromaticAbberationText.text = chromaticAbberation.ToString("0#.00");
    }

    void OnSliderValueChanged(Slider changedSlider)
    {
        if (changedSlider == xSensSlider)
        {
            xSens = changedSlider.value * 10;
            xSensText.text = (10 * Mathf.Round(changedSlider.value)).ToString();
        }
        else if (changedSlider == ySensSlider)
        {
            ySens = changedSlider.value * 10;
            ySensText.text = (10 * Mathf.Round(changedSlider.value)).ToString();
        }
        else if (changedSlider == bloomSlider)
        {
            bloomIntensity = changedSlider.value;
            bloomText.text = changedSlider.value.ToString("#.00");
        }
        else if (changedSlider == motionBlurSlider)
        {
            motionBlurIntensity = changedSlider.value;
            motionBlurText.text = changedSlider.value.ToString("#.00");
        }
        else if (changedSlider == chromaticAbberationSlider)
        {
            chromaticAbberation = changedSlider.value;
            chromaticAbberationText.text = changedSlider.value.ToString("#.00");
        }

        SaveSettings();
    }
}
