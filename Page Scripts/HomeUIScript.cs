using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;

public class HomeUIScript : MonoBehaviour
{
    // Start is called before the first frame 
    private VisualElement root;

    private List<Button> btns;
    private string usernamePref;
    private string choosenCharacter;
    VisualElement transitionIN;

    UserData userData;
    void OnEnable()
    {
        root = gameObject.GetComponent<UIDocument>().rootVisualElement;
        btns = root.Query<Button>("Button").ToList();
        userData = GameObject.FindGameObjectWithTag("UserDataTag").GetComponent<UserData>();

        transitionIN = root.Q<VisualElement>("TransitionerIN");

        root.Q<Label>("HomeUsername").text = usernamePref + choosenCharacter;
        SetMainButtonProperties(btns);
    }

    void SetMainButtonProperties(List<Button> btns)
    {
        Button LBButton = btns[0];
        Button PlayButton = btns[1];
        Button ContactButton = btns[2];
        Button SettingsButton = btns[3];

        VisualElement leaderboardsModalContainer = root.Q<VisualElement>("LeaderboardsModal");
        Button closeLeaderboardsButton = root.Q<Button>("CloseLeaderboardsButton");
        LBButton.clickable = new Clickable(() =>
        {
            leaderboardsModalContainer.style.display = DisplayStyle.Flex;
            Debug.Log("Leaderboards clicked");

            // Display player scores
            string scoreRecord = PlayerPrefs.GetString("LeaderboardScoresAndName");

            if (scoreRecord == "" || scoreRecord.Length <= 1)
            {
                Debug.Log("There is no record here");
                return;
            }

            scoreRecord = scoreRecord[..(scoreRecord.Length - 1)];
            Debug.Log("Comma removed:" + scoreRecord);
            // ex output: michael-12345
            string[] decodedData = scoreRecord.Split(",");

            Debug.Log(decodedData);
            string[] usernames = new string[decodedData.Length];
            int[] scores = new int[decodedData.Length];

            for (int i = 0; i < decodedData.Length; i++)
            {
                string[] data = decodedData[i].Split("-");

                usernames[i] = data[0];
                scores[i] = Int32.Parse(data[1]);
            }

            CreateListOfScores(usernames, scores);

            void CreateListOfScores(string[] _usernames, int[] _scores)
            {
                VisualElement scoresContainer = leaderboardsModalContainer.Q<VisualElement>("ScoresContainer");

                for (int i = 0; i < usernames.Length; i++)
                {
                    string username = _usernames[i];
                    string score = _scores[i].ToString();

                    VisualElement playerScore = new();
                    playerScore.AddToClassList("PlayerScoreClass");

                    Label playerUsernameLabel = new(username);
                    Label playerScoreLabel = new(score);

                    Debug.Log(playerScoreLabel.text);

                    playerScore.Add(playerUsernameLabel);
                    playerScore.Add(playerScoreLabel);

                    scoresContainer.Add(playerScore);
                }
            }
        });
        closeLeaderboardsButton.clickable = new Clickable(() =>
        {
            leaderboardsModalContainer.style.display = DisplayStyle.None;
        });

        VisualElement usernameModalContainer = root.Q<VisualElement>("UsernameModal");
        Button closeUsernameModalButton = usernameModalContainer.Q<Button>(className: "CloseUsernameModal");
        Button submitUsernameBtn = usernameModalContainer.Q<Button>("SubmitUsernameButton");
        TextField usernameTF = usernameModalContainer.Q<TextField>("UsernameTF");
        Label usernameErrorMessage = usernameModalContainer.Q<Label>("UsernameErrorMessage");

        PlayButton.clickable = new Clickable(() =>
        {
            Debug.Log("Play button clicked");
            usernamePref = PlayerPrefs.GetString("username").Trim();
            choosenCharacter = PlayerPrefs.GetString("choosenCharacter").Trim();

            if (usernamePref == "" || usernamePref == null) { ShowUsernameInputModal(); return; }
            if (choosenCharacter == "" || choosenCharacter == null) { ChooseCharacter(); return; }

            if (choosenCharacter == "nightshade" || choosenCharacter == "celestia") ShowWelcomeUserModal();
            else Debug.Log("Invalid character.");
        });

        ContactButton.clickable = new Clickable(() =>
        {
            Debug.Log("Contact button clicked");
        });

        VisualElement settingsModal = root.Q<VisualElement>("SettingsModal");
        Button closeSettingsButton = settingsModal.Q<Button>("CloseSettings");
        Button clearDataButton = settingsModal.Q<Button>("ClearDataButton");
        Button exitGameButton = settingsModal.Q<Button>("ExitGameButton");

        VisualElement mainModal = settingsModal.Q<VisualElement>("MainModal");

        VisualElement deleteDataModal = settingsModal.Q<VisualElement>("DeleteDataModal");
        Button deleteDataButton = deleteDataModal.Q<Button>("DeleteDataButton");
        Button closeDeleteDataButton = deleteDataModal.Q<Button>("CloseDeleteDataButton");

        VisualElement deletedSuccessfullyModal = deleteDataModal.Q<VisualElement>("DeletedSuccessfullyModal");
        SettingsButton.clickable = new Clickable(() =>
        {
            ShowSettings();
        });

        void ShowSettings()
        {
            settingsModal.style.display = DisplayStyle.Flex;
            closeSettingsButton.clickable = new Clickable(() => settingsModal.style.display = DisplayStyle.None);

            clearDataButton.clickable = new Clickable(() => deleteDataModal.style.display = DisplayStyle.Flex);
            exitGameButton.clickable = new Clickable(() => Application.Quit());

            deleteDataButton.clickable = new Clickable(() =>
            {
                PlayerPrefs.SetString("username", "");
                PlayerPrefs.SetString("choosenCharacter", "");
                StartCoroutine(showDeleteDateConfirmed());
            });

            closeDeleteDataButton.clickable = new Clickable(() => deleteDataModal.style.display = DisplayStyle.None);

            IEnumerator showDeleteDateConfirmed()
            {
                mainModal.style.display = DisplayStyle.None;
                deletedSuccessfullyModal.style.display = DisplayStyle.Flex;
                yield return new WaitForSeconds(3);
                deletedSuccessfullyModal.style.display = DisplayStyle.None;
                mainModal.style.display = DisplayStyle.Flex;

                // Close main
                deleteDataModal.style.display = DisplayStyle.None;
            }

            // Volume Slider
            SliderInt volumeSlider = root.Q<SliderInt>("VolumeSlider");

            if (PlayerPrefs.HasKey("VolumeValue"))
                volumeSlider.value = PlayerPrefs.GetInt("VolumeValue");

            volumeSlider.RegisterCallback<ChangeEvent<int>>((e) =>
            {
                volumeSlider.value = e.newValue;
                PlayerPrefs.SetInt("VolumeValue", e.newValue);
                Debug.Log(e.newValue);
            });
        }

        void ShowUsernameInputModal()
        {
            Debug.Log("Play button adding clickable code blocks");

            usernameModalContainer.style.display = DisplayStyle.Flex;
            closeUsernameModalButton.clickable = new Clickable(() => closeModalOnAction());

            void closeModalOnAction()
            {
                usernameModalContainer.style.display = DisplayStyle.None;
            }

            usernameTF.Focus();

            submitUsernameBtn.clickable = new Clickable(() => SubmitUsername(usernameTF.value));

            void SubmitUsername(string _username)
            {
                if (_username == "" || _username.Trim() == "")
                {
                    Debug.Log("Error, username of empty is invalid");
                    DisplayAndHideUsernameErrorMessage();

                    void DisplayAndHideUsernameErrorMessage()
                    {

                        StartCoroutine(HideUsernameErrorMessage());

                        IEnumerator HideUsernameErrorMessage()
                        {
                            usernameErrorMessage.style.display = DisplayStyle.Flex;
                            yield return new WaitForSeconds(2);
                            usernameErrorMessage.style.display = DisplayStyle.None;
                        }
                    }
                }
                else
                {
                    userData.SetUsername(_username);
                    closeModalOnAction();
                }

            }
        }
        void ChooseCharacter()
        {
            SceneManager.LoadSceneAsync("ChooseCharacterScene", LoadSceneMode.Additive);
        }
        void ShowWelcomeUserModal()
        {
            VisualElement welcomeContainer = root.Q<VisualElement>("WelcomeUserModal");
            welcomeContainer.style.display = DisplayStyle.Flex;

            // transitionIN.style.width = Length.Percent(100);

            Label welcomeUserText = welcomeContainer.Q<Label>("WelcomeUserText");
            welcomeUserText.text = "Welcome " + usernamePref + "!";

            StartCoroutine(RouteToSCene());

            IEnumerator RouteToSCene()
            {
                yield return new WaitForSeconds(0);
                // Route to ARScene

                // if (PlayerPrefs.GetInt("UserReadTheStory") == 0)
                // {

                // Route to story
                SceneManager.LoadSceneAsync("StoryScene", LoadSceneMode.Single);
                // }
                // else
                // {
                //     PlayerPrefs.SetInt("UserReadTheStory", 1);

                //     LoaderUtility.Initialize();
                //     SceneManager.LoadSceneAsync("ARScene", LoadSceneMode.Single);
                // }


            }
        }
    }
}
