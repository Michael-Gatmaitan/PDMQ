using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;

public class GameplayUIScript : MonoBehaviour
{
    private VisualElement root;

    private VisualElement hpHandle;
    private Label hpLabel;
    private Label manaLabel;

    private Models models;
    private View view;
    private Controllers controllers;

    // private VisualElement transitionOUT;

    private VisualElement characterProfile;
    private Button regenButton;
    private bool regenButtonCoolDowned = true;
    public bool leveledUpToLevel2 = false;
    public bool leveledUpToLevel3 = false;

    private PlayerScript playerScript;
    private string choosenCharacter;

    private Button attackButton;
    private Button readyToPlayButton;

    public AudioClip[] hitClips;

    NightShadeSkills nightShadeSkills;

    private bool publicDied = false;

    void Start()
    {
        // SetPlayerPrefs();
        PlayerPrefs.SetInt("UserReadTheStory", 0);
        models = GetComponent<Models>();
        view = GetComponent<View>();
        controllers = GetComponent<Controllers>();

        Debug.Log(models.gameState.EnemyKilled + " idddddddd");
        choosenCharacter = PlayerPrefs.GetString("choosenCharacter");

        root = GetComponent<UIDocument>().rootVisualElement;

        // transitionOUT = root.Q<VisualElement>("TransitioinerOUT");
        // transitionOUT.style.transitionDuration = new List<TimeValue> { 0 };
        // transitionOUT.style.width = Length.Percent(100);

        // StartCoroutine(TransitionOUT());

        hpHandle = root.Q<VisualElement>("HPHandle");
        hpLabel = root.Q<Label>("HPLabel");
        manaLabel = root.Q<Label>("ManaLabel");

        characterProfile = root.Q<VisualElement>("CharacterProfile");
        attackButton = root.Q<Button>("AttackButton");

        readyToPlayButton = root.Q<Button>("ReadyToPlayButton");
        regenButton = root.Q<Button>("RegenButton");

        playerScript = GameObject.FindGameObjectWithTag("MainPlayerTag").GetComponent<PlayerScript>();
        nightShadeSkills = GameObject.FindGameObjectWithTag("CHSkillsTag").GetComponent<NightShadeSkills>();

        // Set up UI
        NewLevelSetup(1);
        SetupRayCast();
        SetupUI();
    }
    void NewLevelSetup(int _gameLevel)
    {
        models.SetGameStates(_gameLevel);
        UpdatePlayerHPLocal();
        UpdatePlayerHPLocal();

        // view.UpdatePlayerMana();
    }

    void UpdatePlayerHPLocal()
    {
        int playerHP = models.playerData.Hp;
        int playerTotalHP = models.playerData.TotalHP;

        hpLabel.text = playerHP + " / " + playerTotalHP;
        float hpPercentage = (float)(playerHP * 100) / playerTotalHP;

        hpHandle.style.width = Length.Percent(hpPercentage);
    }

    void SetupUI()
    {

        Button readyToPlayButton = root.Q<Button>("ReadyToPlayButton");
        readyToPlayButton.clickable = new Clickable(() =>
        {
            models.gameState.ReadyToPlay = true;
        });


        Button[] skillButtons = {
            root.Q<Button>("Skill1"),
            root.Q<Button>("Skill2"),
            root.Q<Button>("Skill3")
        };

        int[] skillManaCost = { 20, 40, 50 };

        VisualElement[] skillButtonIcons = {
            skillButtons[0].Q<VisualElement>("GameplayButtonIcon1"),
            skillButtons[1].Q<VisualElement>("GameplayButtonIcon2"),
            skillButtons[2].Q<VisualElement>("GameplayButtonIcon3"),
        };

        Sprite playerProfile = Resources.Load<Sprite>(
            choosenCharacter == "nightshade" ?
            "EnemyProfiles/nightshade_profile" :
            "EnemyProfiles/celestia_profile"
        );

        characterProfile.style.backgroundImage = new StyleBackground(playerProfile);

        SetPlaneWarner();

        SetSkillButtons();
        SetSkillActions();
        SetGameSettings();
        SetPlayerStats();
        SetPowerUps();

        SetLifeUI();

        void SetPlaneWarner()
        {
            VisualElement planeWarnerContainer = root.Q<VisualElement>("PlaneWarnerContainer");
            Button restartSceneButton = planeWarnerContainer.Q<Button>("RestartSceneButton");

            restartSceneButton.clickable = new Clickable(() =>
            {
                // controller
                Debug.Log("Restarting scene");
                controllers.RestartScene();
            });
        }

        void SetSkillButtons()
        {
            Nightshade nightshadeClass = new();
            Celestia celestiaClass = new();

            (string skillTitle, string desc, string path)[] characterSkills =
                (choosenCharacter == "nightshade" ? nightshadeClass.nightshadeSkills
                : choosenCharacter == "celestia" ? celestiaClass.celestiaSkills : null)
                ?? throw new NullReferenceException("Character skills cannot be null");

            for (int i = 0; i < skillButtonIcons.Length; i++)
            {
                VisualElement skillVE = skillButtonIcons[i];
                (string skillTitle, string desc, string path) characterSkill = characterSkills[i];
                Sprite skillSprite = Resources.Load<Sprite>(characterSkill.path);
                skillVE.style.backgroundImage = new StyleBackground(skillSprite);
            }
        }

        void SetSkillActions()
        {
            for (int x = 0; x < skillButtons.Length; x++)
            {
                int index = x;
                skillButtons[x].clickable = new Clickable(() =>
                {
                    models.playerData.Mana -= skillManaCost[index];
                    // view.UpdatePlayerMana();

                    if (index == 0 && choosenCharacter == "nightshade")
                    {
                        nightShadeSkills.Skill1();
                    }
                });
            }
        }

        void SetGameSettings()
        {
            Button gameSettingsButton = root.Q<Button>("GameSettingsButton");

            VisualElement gSettingsMCont = root.Q<VisualElement>("GameSettingsModalContainer");
            Button closeGameSettingsButton = root.Q<Button>("CloseGameSettings");

            Button restartPlaneRenderingButton = root.Q<Button>("RestartPlaneRenderingButton");

            VisualElement rSWMC = root.Q<VisualElement>("RestartSceneWarningModalContainer");

            gameSettingsButton.clickable = new Clickable(() =>
            {
                Debug.Log("Game settings clicked");
                gSettingsMCont.style.display = DisplayStyle.Flex;
            });

            closeGameSettingsButton.clickable = new Clickable(() =>
            {
                gSettingsMCont.style.display = DisplayStyle.None;
            });

            restartPlaneRenderingButton.clickable = new Clickable(() =>
            {
                rSWMC.style.display = DisplayStyle.Flex;
                // gSettingsMCont.style.display = DisplayStyle.None;
            });

            Button restartOnGameButton = root.Q<Button>("RestartOnGameButton");
            restartOnGameButton.clickable = new Clickable(() =>
            {
                controllers.RestartScene();
                models.SetGameStates(1);
            });
        }

        void SetPlayerStats()
        {
            VisualElement hpHandle = root.Q<VisualElement>("HPHandle");
            VisualElement manaHandle = root.Q<VisualElement>("ManaHandle");

            Length fullPercent = Length.Percent(100f);
            hpHandle.style.width = fullPercent;
            manaHandle.style.width = fullPercent;

            // Set HP start up
            int playerHP = models.playerData.Hp;
            int playerTotalHP = models.playerData.TotalHP;

            hpLabel.text = playerHP + " / " + playerTotalHP;

            int playerMana = models.playerData.Mana;
            int playerTotalMana = models.playerData.TotalMana;

            manaLabel.text = playerMana + " / " + playerTotalMana;
            // float hpPercentage = (float)(playerHP * 100) / playerTotalHP;
        }

        void SetPowerUps()
        {

            int HPtoRegen = 100;
            IEnumerator RegenPlayer()
            {
                if (models.playerData.Hp + 10 > models.playerData.TotalHP)
                {
                    models.playerData.Hp = models.playerData.TotalHP;
                    HPtoRegen = 0;
                }
                else
                {
                    models.playerData.Hp += 10;
                    HPtoRegen -= 10;
                    UpdatePlayerHPLocal();
                    Debug.Log("Regenerating HP.");
                    yield return new WaitForSeconds(1);
                    if (HPtoRegen > 0) StartCoroutine(RegenPlayer());
                }
            }

            regenButton.clickable = new Clickable(() =>
            {
                HPtoRegen = 100;
                StartCoroutine(RegenPlayer());
                Debug.Log("Regen started.");

                regenButton.SetEnabled(false);
                StartCoroutine(EnableRegen());

                IEnumerator EnableRegen()
                {
                    regenButtonCoolDowned = false;
                    yield return new WaitForSeconds(20);
                    regenButtonCoolDowned = true;
                    regenButton.SetEnabled(true);
                }
            });

        }


    }
    void SetLifeUI()
    {
        VisualElement lifeContainer = root.Q<VisualElement>("LifeContainer");

        List<VisualElement> lifes = lifeContainer.Query<VisualElement>("Life").ToList();

        for (int i = 0; i < 3; i++)
        {
            lifes[i].style.display = DisplayStyle.Flex;
        }

        // for (int i = 0; i < models.playerData.PlayerLife; i++)
        // {
        //     lifes[i].style.display = DisplayStyle.None;
        // }

        Debug.Log(models.playerData.PlayerLife);

        for (int i = models.playerData.PlayerLife; i < 3; i++)
        {
            lifes[i].style.display = DisplayStyle.None;
        }
    }

    void SetupRayCast()
    {
        attackButton.clickable = new Clickable(() =>
        {
            playerScript = GameObject.FindGameObjectWithTag("MainPlayerTag").GetComponent<PlayerScript>();

            if (choosenCharacter == "nightshade")
            {
                playerScript.ShurikenAttack();
            }
            else if (choosenCharacter == "celestia")
            {
                playerScript.LiquidSphereAttack();
            }

            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit))
            {
                string targetName = hit.transform.name;
                Debug.Log(targetName);

                if (targetName.Contains("Zombie"))
                {
                    GameObject[] enemies = GameObject.FindGameObjectsWithTag("EnemyTargetTag");
                    GameObject enemyHitted = enemies[enemies.ToList<GameObject>().IndexOf(hit.transform.gameObject)];

                    CubeScript cubeScript = enemyHitted.GetComponent<CubeScript>();
                    // Destroy(hit.transform.gameObject);
                    Debug.Log("Enemy killed " + models.gameState.EnemyKilled);

                    cubeScript.thisEnemyHP -= models.playerData.PlayerDamage;
                    models.playerData.TotalDamageDealth += models.playerData.PlayerDamage;

                    if (cubeScript.thisEnemyHP <= 0)
                    {
                        models.gameState.EnemyKilled += 1;
                        view.UpdateGameStats();
                    }

                    if (models.gameState.EnemyKilled >= models.gameState.EnemyCountLvl1 && leveledUpToLevel2 == false)
                    {
                        leveledUpToLevel2 = true;
                        models.playerData.GameLevel = 2;
                        models.SetGameStates(models.playerData.GameLevel);

                        Debug.Log("woadh");

                        UpdatePlayerHPLocal();
                        // view.UpdatePlayerMana();
                        models.gameState.ReadyToPlay = false;
                        view.UpdateGameStats();

                        // Proceed to lvl 2 and re-set states for lvl 2 of the game
                        Debug.Log("You have reached level " + models.playerData.GameLevel);
                    }

                    if (models.gameState.EnemyKilled >= (models.gameState.EnemyCountLvl1 + models.gameState.EnemyCountLvl2) && leveledUpToLevel2 == true && leveledUpToLevel3 == false)
                    {
                        leveledUpToLevel3 = true;
                        models.playerData.GameLevel = 3;
                        models.SetGameStates(models.playerData.GameLevel);

                        UpdatePlayerHPLocal();
                        // view.UpdatePlayerMana();
                        models.gameState.ReadyToPlay = false;
                        view.UpdateGameStats();
                        Debug.Log("You have reached level " + models.playerData.GameLevel);
                    }

                    string updateEnemyProfileParam =
                        cubeScript.thisEnemyHP >= 0 ? targetName : "";

                    view.UpdateEnemyProfile(updateEnemyProfileParam, cubeScript);
                }
            }

            // Set cooldown
            IEnumerator CoolDownAttack()
            {
                attackButton.SetEnabled(false);
                yield return new WaitForSeconds(0.1f);
                attackButton.SetEnabled(true);
            }

            StartCoroutine(CoolDownAttack());
        });
    }

    void Update()
    {
        if (publicDied)
        {
            NewLevelSetup(models.playerData.GameLevel);
            SetupRayCast();
            SetupUI();
            publicDied = false;
        }

        bool readyToPlay = models.gameState.ReadyToPlay;

        if (!readyToPlay && models.gameState.PlaneReady)
        {
            readyToPlayButton.text = "Proceed to level " + models.playerData.GameLevel;
            readyToPlayButton.style.display = DisplayStyle.Flex;
        }
        else
        {
            readyToPlayButton.style.display = DisplayStyle.None;
        }

        // Player died
        PlayerDied();

        void PlayerDied()
        {
            int playerHP = models.playerData.Hp;
            bool playerDied = models.playerData.PlayerDied;

            if (playerHP <= 0 && playerDied == false)
            {
                Debug.Log("You died with life of BEFORE: " + models.playerData.PlayerLife);
                models.playerData.PlayerLife -= 1;
                int playerLife = models.playerData.PlayerLife;

                Debug.Log("You died with life of AFTER: " + models.playerData.PlayerLife);

                if (playerLife == 0)
                {
                    Debug.Log("Died with life of " + playerLife);
                    Debug.Log("Out of life.");
                    models.gameState.GameFinished = true;
                    // view.PlayerFinishedGame();
                    models.SetGameStates(1);
                    models.RestartStates();
                    models.playerData.PlayerLife = 3;

                    publicDied = true;
                }
                else
                {
                    int gameLevel = models.playerData.GameLevel;
                    // Restart the enemy count in plane states.
                    PlaneStates planeStates = GameObject.FindGameObjectWithTag("XROriginObjectTag").GetComponent<PlaneStates>();
                    planeStates.spawning = false;
                    planeStates.enemyCount = gameLevel == 1 ? models.gameState.EnemyCountLvl1
                    : gameLevel == 2 ? models.gameState.EnemyCountLvl2
                    : gameLevel == 3 ? models.gameState.EnemyCountLvl3
                    : throw new Exception("Numbers of enemy count is invalid");

                    DestroyAllZombies();

                    models.gameState.ReadyToPlay = false;
                    models.SetGameStates(gameLevel);
                    SetLifeUI();

                    view.UpdatePlayerHP();
                    // Restart the current level
                    Debug.Log("ASDASD");
                    return;
                }

                models.gameState.ReadyToPlay = false;
                models.playerData.PlayerDied = true;

                VisualElement youDiedModalContainer = root.Q<VisualElement>("YouDiedModalContainer");
                Label yourScoreLabelLabel = root.Q<Label>("YourScoreLabel");
                Label yourTotalEnemyKilledLabel = root.Q<Label>("YourTotalEnemyKilledLabel");
                Label yourLevelReachedLabel = root.Q<Label>("YourLevelReachedLabel");
                Label yourTotalDamageDealthLabel = root.Q<Label>("YourTotalDamageDealthLabel");

                Button restartGameButton = root.Q<Button>("DiedRestartGameButton");
                Button homeButton = root.Q<Button>("HomeButton");

                youDiedModalContainer.style.display = DisplayStyle.Flex;

                restartGameButton.clickable = new Clickable(() =>
                {
                    youDiedModalContainer.style.display = DisplayStyle.None;
                    models.RestartStates();

                    view.UpdateGameStats();
                    UpdatePlayerHPLocal();
                    // view.UpdatePlayerMana();
                });

                homeButton.clickable = new Clickable(() =>
                {
                    SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
                    LoaderUtility.Deinitialize();
                });

                models.playerData.PlayerDied = true;

                void DestroyAllZombies()
                {
                    GameObject[] zombies = GameObject.FindGameObjectsWithTag("EnemyTargetTag");
                    foreach (GameObject zombie in zombies) Destroy(zombie);
                }

                DestroyAllZombies();

                Debug.Log("You died.");

                yourScoreLabelLabel.text = "Your score: " + models.playerData.PlayerScore;
                yourTotalEnemyKilledLabel.text = "Total enemy killed: " + models.gameState.EnemyKilled;
                yourLevelReachedLabel.text = "Level reached: " + models.playerData.GameLevel;
                yourTotalDamageDealthLabel.text = "Total damage dealth: " + models.playerData.TotalDamageDealth;
            }
        }

        // Regen button state
        bool fullHP = models.playerData.Hp >= models.playerData.TotalHP;

        if (fullHP)
        {
            regenButton.SetEnabled(false);
        }
        else
        {
            if (regenButtonCoolDowned)
            {
                regenButton.SetEnabled(true);
            }
        }
    }

    // IEnumerator TransitionOUT()
    // {
    //     transitionOUT.style.transitionDuration = new List<TimeValue> { 1 };
    //     yield return new WaitForSeconds(1);
    //     root.Q<VisualElement>("TransitioinerOUT").style.width = Length.Percent(0);
    // }
}