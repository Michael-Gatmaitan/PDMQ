using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LoadingUIScript : MonoBehaviour
{
    // Start is called before the first frame update
    private VisualElement root;
    private VisualElement loadingHandle;
    private Label progressTxt;
    private Label gameSettingUpTxt;
    private VisualElement blocker;
    private Button switchBtn;
    public void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        loadingHandle = root.Q<VisualElement>("Handle");
        progressTxt = root.Q<Label>("LoadingText");
        gameSettingUpTxt = root.Q<Label>("GameSettingUp");
        blocker = root.Q<VisualElement>("Blocker");

        switchBtn = root.Q<Button>("SwitchBtn");

        gameSettingUpTxt.text = "";

        StartCoroutine(LoadingProgress());
    }

    void Start()
    {
        if (!PlayerPrefs.HasKey("LeaderboardScoresAndName"))
        {
            PlayerPrefs.SetString("LeaderboardScoresAndName", "");
        }

        PlayerPrefs.SetInt("VolumeValue", 0);
    }

    IEnumerator LoadingProgress()
    {
        float progressNumber;

        // yield return new  WaitForSeconds(1f);
        // blocker.style.width = 1000;
        // yield return new  WaitForSeconds(5f);

        for (float x = 0; x <= 436; x += 4)
        {
            yield return new WaitForSeconds(.0001f);
            progressNumber = Convert.ToInt32((x / 436) * 100);

            loadingHandle.style.width = x;
            progressTxt.text = progressNumber + "%";

            // Debug.Log(progressNumber);
        }

        // After the loop, display the text below of the UI.
        StartCoroutine(DisplayGameSetup());
    }

    IEnumerator DisplayGameSetup()
    {
        gameSettingUpTxt.text = "";

        string txt = "PDM Quest (2023)";
        for (int i = 0; i < txt.Length; i++)
        {
            yield return new WaitForSeconds(0.05f);
            gameSettingUpTxt.text += txt[i];
        }

        yield return new WaitForSeconds(0.5f);
        Debug.Log("Game Loading");

        // The animations and loading in this component is ended
        SceneManager.LoadSceneAsync("MainScene", LoadSceneMode.Single);
    }
}

