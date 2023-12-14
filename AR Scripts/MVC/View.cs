using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;

public class View : MonoBehaviour
{
    // Start is called before the first frame update
    private VisualElement root;
    private VisualElement hpHandle;
    private Label hpLabel;
    private VisualElement manaHandle;
    private Label manaLabel;
    Models models;

    // Game stats
    Label gameLevelLabel;
    Label TotalKillLabel;
    Label KillReqLabel;

    // Enemy profile
    VisualElement enemyInfoContainer;
    VisualElement enemyInfoImage;
    VisualElement enemyTotalHPHandle;
    Label enemyDamageLabel;
    Label enemyTotalHPLabel;

    // -- // Stats (with icons)
    Label statDamageLabel;

    // Game finished
    VisualElement gameFinishedModalContainer;
    Label finishedTotalScoreLabel;
    Label finishedTotalZombiesKilledLabel;
    Label finishedTotalDamageDealthLabel;
    Button finshedGoHomeButton;
    Button aboutThisGameButton;
    VisualElement aboutThisGameModalContainer;
    void Start()
    {
        models = gameObject.GetComponent<Models>();

        root = gameObject.GetComponent<UIDocument>().rootVisualElement;

        hpHandle = root.Q<VisualElement>("HPHandle");
        hpLabel = root.Q<Label>("HPLabel");

        manaHandle = root.Q<VisualElement>("ManaHandle");
        manaLabel = root.Q<Label>("ManaLabel");

        gameLevelLabel = root.Q<Label>("GameLevelLabel");
        TotalKillLabel = root.Q<Label>("TotalKillLabel");
        KillReqLabel = root.Q<Label>("KillReqLabel");

        enemyInfoContainer = root.Q<VisualElement>("EnemyInfo");
        enemyInfoImage = root.Q<VisualElement>("EnemyInfoImage");
        enemyTotalHPHandle = root.Q<VisualElement>("EnemyTotalHPHandle");
        enemyDamageLabel = root.Q<Label>("EnemyDamageLabel");
        enemyTotalHPLabel = root.Q<Label>("EnemyTotalHPLabel");

        statDamageLabel = root.Q<Label>("StatDamageLabel");

        gameFinishedModalContainer = root.Q<VisualElement>("GameFinishedModalContainer");
        finishedTotalScoreLabel = root.Q<Label>("FinishedTotalScoreLabel");
        finishedTotalZombiesKilledLabel = root.Q<Label>("FinishedTotalZombiesKilledLabel");
        finishedTotalDamageDealthLabel = root.Q<Label>("FinishedTotalDamageDealthLabel");
        finshedGoHomeButton = root.Q<Button>("FinshedGoHomeButton");
        aboutThisGameButton = root.Q<Button>("AboutThisGameButton");
        aboutThisGameModalContainer = root.Q<VisualElement>("AboutThisGameModalContainer");

        finshedGoHomeButton.clickable = new Clickable(() =>
        {
            SceneManager.LoadSceneAsync("MainScene", LoadSceneMode.Single);
            LoaderUtility.Deinitialize();
        });
        aboutThisGameButton.clickable = new Clickable(() =>
        {
            gameFinishedModalContainer.style.display = DisplayStyle.None;
            aboutThisGameModalContainer.style.display = DisplayStyle.Flex;
            StartCoroutine(HideAboutThisGame());

            IEnumerator HideAboutThisGame()
            {
                yield return new WaitForSeconds(3);

                gameFinishedModalContainer.style.display = DisplayStyle.Flex;
                aboutThisGameModalContainer.style.display = DisplayStyle.None;
            }
        });
    }

    public void UpdatePlayerHP()
    {
        int playerHP = models.playerData.Hp;
        int playerTotalHP = models.playerData.TotalHP;

        hpLabel.text = playerHP + " / " + playerTotalHP;
        float hpPercentage = (float)(playerHP * 100) / playerTotalHP;

        hpHandle.style.width = Length.Percent(hpPercentage);
    }

    public void UpdatePlayerMana()
    {
        int playerMana = models.playerData.Mana;
        int playerTotalMana = models.playerData.TotalMana;

        manaLabel.text = playerMana + " / " + playerTotalMana;
        float manaPercentage = (float)(playerMana * 100) / playerTotalMana;

        manaHandle.style.width = Length.Percent(manaPercentage);
    }

    public void UpdateGameStats()
    {
        int gameLevel = models.playerData.GameLevel;
        int enemyKilled = models.gameState.EnemyKilled;

        int enemyCountLvl1 = models.gameState.EnemyCountLvl1;
        int enemyCountLvl2 = models.gameState.EnemyCountLvl2;
        int enemyCountLvl3 = models.gameState.EnemyCountLvl3;

        gameLevelLabel.text = "Level: " + gameLevel.ToString();
        TotalKillLabel.text = "Total zombie killed: " + enemyKilled.ToString();

        string killReqLabelVal = gameLevel == 1 ?
        enemyKilled.ToString() + " / " + enemyCountLvl1.ToString()
        : gameLevel == 2 ?
        (enemyKilled - enemyCountLvl1).ToString() + " / " + enemyCountLvl2.ToString()
        : gameLevel == 3 ?
        (enemyKilled - enemyCountLvl1 - enemyCountLvl2).ToString() + " / " + enemyCountLvl3.ToString()
        : "Hotdog";

        KillReqLabel.text = "Level kill required: " + killReqLabelVal;

        statDamageLabel.text = models.playerData.PlayerDamage.ToString();
    }

    public void UpdateEnemyProfile(string objectName, CubeScript cubeScript)
    {
        if (objectName.Contains("Zombie"))
        {
            enemyInfoContainer.style.display = DisplayStyle.Flex;
            // string enemyProfilePath = 
            //     objectName.Contains("Men")
            //         ? "EnemyProfiles/ZombieMen/ZombieMenPNG"
            //         : "EnemyProfiles/ZombieMen/ZombieWomenPNG";
            string enemyProfilePath =
                objectName.Contains("gatmaitan") ? "EnemyProfiles/ZombieMen/gatmaitan_zombie_profile"
                : objectName.Contains("llorca") ? "EnemyProfiles/ZombieMen/llorca_zombie_profile"
                : objectName.Contains("luna") ? "EnemyProfiles/ZombieMen/luna_zombie_profile"
                : objectName.Contains("garcia") ? "EnemyProfiles/ZombieMen/garcia_zombie_profile"
                : objectName.Contains("honey") ? "EnemyProfiles/ZombieMen/honey_zombie_profile"
                : objectName.Contains("delmonte") ? "EnemyProfiles/ZombieMen/delmonte_zombie_profile"
                : objectName.Contains("dasalla") ? "EnemyProfiles/ZombieMen/dasalla_zombie_profile"
                : "";

            float enemyHPPercentage = (float)(cubeScript.thisEnemyHP * 100) / models.enemyData.EnemyHP;
            enemyTotalHPHandle.style.width = Length.Percent(enemyHPPercentage);

            enemyTotalHPLabel.text = cubeScript.thisEnemyHP.ToString() + " / " + models.enemyData.EnemyHP;
            Sprite enemyProfilePNG = Resources.Load<Sprite>(enemyProfilePath);
            enemyInfoImage.style.backgroundImage = new StyleBackground(enemyProfilePNG);

            enemyDamageLabel.text = "Damage: " + models.enemyData.EnemyDamage;

            if (cubeScript.thisEnemyHP <= 0)
            {
                enemyInfoContainer.style.display = DisplayStyle.None;
                Debug.Log(enemyInfoContainer);
                // Sprite ivan = Resources.Load<Sprite>("EnemyProfiles/Ivan/ivan");
                // enemyInfoImage.style.backgroundImage = new StyleBackground(ivan);

                // enemyTotalHPLabel.text = "0 / 0";
                // // enemyInfoImage.style.backgroundImage = null;

                // enemyDamageLabel.text = "Rawr";
            }
        }
        else
        {
            enemyInfoContainer.style.display = DisplayStyle.None;
            // Sprite ivan = Resources.Load<Sprite>("EnemyProfiles/Ivan/ivan");
            // enemyInfoImage.style.backgroundImage = new StyleBackground(ivan);

            // enemyTotalHPHandle.style.width = 0;

            // enemyTotalHPLabel.text = "0 / 0";
            // // enemyInfoImage.style.backgroundImage = null;

            // enemyDamageLabel.text = "Rawr";
        }
    }

    public void PlayerFinishedGame()
    {
        string playerUsername = PlayerPrefs.GetString("username");
        string decodedString = playerUsername + "-" + models.playerData.PlayerScore;

        string scorePlainString = PlayerPrefs.GetString("LeaderboardScoresAndName");
        Debug.Log(scorePlainString);
        // ex output: michael-12345
        string finalRecords = scorePlainString + "" + decodedString + ",";

        Debug.Log(finalRecords);
        Debug.Log("FInal records: " + scorePlainString + "" + decodedString + ",");
        PlayerPrefs.SetString("LeaderboardScoresAndName", finalRecords);

        // GetSetOfUsernames(PlayerPrefs.GetString("LeaderboardScoresAndName"));

        gameFinishedModalContainer.style.display = DisplayStyle.Flex;

        finishedTotalScoreLabel.text = "Total Score of: " + models.playerData.PlayerScore;
        finishedTotalZombiesKilledLabel.text = "Total zombies killed: " + models.gameState.EnemyKilled;
        finishedTotalDamageDealthLabel.text = "Total Damage Dealth: " + models.playerData.TotalDamageDealth;
    }
}
