using System;
using UnityEngine;
using UnityEngine.UIElements;

public class Models : MonoBehaviour
{

    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("GamePlayUITag");

        if (objs.Length > 1)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public class SceneState
    {
        private bool sceneForLvl2Reloaded = false;
        private bool sceneForLvl3Reloaded = false;

        public bool SceneForLvl2Reloaded
        {
            get { return sceneForLvl2Reloaded; }
            set { sceneForLvl2Reloaded = value; }
        }

        public bool SceneForLvl3Reloaded
        {
            get { return sceneForLvl3Reloaded; }
            set { sceneForLvl3Reloaded = value; }
        }
    }
    public class PlayerData
    {
        private int playerLife = 3;
        private int playerScore = 0;
        private int totalHP = 350;
        private int hp = 350;
        private int mana = 200;
        private int totalMana = 200;
        private int gameLevel = 1;

        private bool playerValidToProceed1 = false;

        private int playerDamage = 5;

        private bool playerDied = false;

        private int totalDamageDealth = 0;

        public int PlayerLife
        {
            get { return playerLife; }
            set { playerLife = value; }
        }

        public bool PayerValidToProceed1
        {
            get { return playerValidToProceed1; }
            set { playerValidToProceed1 = value; }
        }

        public bool PlayerDied
        {
            get { return playerDied; }
            set { playerDied = value; }
        }

        public int TotalDamageDealth
        {
            get { return totalDamageDealth; }
            set { totalDamageDealth = value; }
        }

        public int PlayerDamage
        {
            get { return playerDamage; }
            set { playerDamage = value; }
        }

        public int PlayerScore
        {
            get { return playerScore; }
            set { playerScore = value; }
        }

        public int TotalHP
        {
            get { return totalHP; }
            set { totalHP = value; }
        }

        public int Hp
        {
            get { return hp; }
            set { hp = value; }
        }

        public int TotalMana
        {
            get { return totalMana; }
            set { totalMana = value; }
        }

        public int Mana
        {
            get { return mana; }
            set { mana = value; }
        }

        public int GameLevel
        {
            get { return gameLevel; }
            set
            {
                if (value <= 0)
                {
                    Debug.Log("Invalid game level");
                    return;
                }

                gameLevel = value;
            }
        }
    }

    public class GameState
    {
        private bool planeReady = false;
        private bool readyToPlay = false;
        private readonly int enemyCountLvl1 = 10;
        // private readonly int enemyCountLvl1 = 1;
        private readonly int enemyCountLvl2 = 15;
        // private readonly int enemyCountLvl2 = 1;
        private readonly int enemyCountLvl3 = 20;
        // private readonly int enemyCountLvl3 = 1;

        private bool gameFinished = false;
        private int enemyKilled = 0;

        public bool PlaneReady
        {
            get { return planeReady; }
            set { planeReady = value; }
        }

        public bool ReadyToPlay
        {
            get { return readyToPlay; }
            set { readyToPlay = value; }
        }

        public bool GameFinished
        {
            get { return gameFinished; }
            set { gameFinished = value; }
        }

        public int EnemyKilled
        {
            get { return enemyKilled; }
            set { enemyKilled = value; }
        }

        public int EnemyCountLvl1
        {
            get { return enemyCountLvl1; }
        }

        public int EnemyCountLvl2
        {
            get { return enemyCountLvl2; }
        }

        public int EnemyCountLvl3
        {
            get { return enemyCountLvl3; }
        }
    }

    public class EnemyData
    {
        private int enemyHP = 50;
        private int enemyDamage = 1;
        private int scoreOnKill = 10;

        public int EnemyHP
        {
            get { return enemyHP; }
            set { enemyHP = value; }
        }

        public int EnemyDamage
        {
            get { return enemyDamage; }
            set { enemyDamage = value; }
        }

        public int ScoreOnKill
        {
            get { return scoreOnKill; }
            set { scoreOnKill = value; }
        }
    }

    public GameState gameState = new();
    public PlayerData playerData = new();
    public EnemyData enemyData = new();
    public SceneState sceneState = new();

    public void SetGameStates(int _gameLevel)
    {

        if (_gameLevel <= 0)
        {
            Debug.LogError("Game level should not be equal or less than zero.");
        }

        if (_gameLevel == 1)
        {
            // Set player data
            gameState.EnemyKilled = 0;

            playerData.PlayerDamage = 5;
            // playerData.PlayerDamage = 300;

            // Disable full hp on new level
            playerData.Hp = 350;
            playerData.TotalHP = 350;
            playerData.Mana = 200;
            playerData.TotalMana = 200;
            playerData.GameLevel = 1;
            playerData.TotalDamageDealth = 0;
            playerData.PlayerScore = 0;

            // Set enemy data
            enemyData.EnemyHP = 50;
            // enemyData.EnemyDamage = 50;
            enemyData.EnemyDamage = 1;
            enemyData.ScoreOnKill = 10;
        }

        else if (_gameLevel == 2)
        {
            // Set player data
            playerData.PlayerDamage = 15;
            // playerData.PlayerDamage = 300;

            // Disable full hp on new level
            playerData.Hp = 500;
            playerData.TotalHP = 500;
            playerData.Mana = 350;
            playerData.TotalMana = 350;
            playerData.GameLevel = 2;

            // Set enemy data
            enemyData.EnemyHP = 90;
            // enemyData.EnemyDamage = 50;
            enemyData.EnemyDamage = 5;
            enemyData.ScoreOnKill = 25;
        }
        else if (_gameLevel == 3)
        {
            playerData.PlayerDamage = 20;
            // playerData.PlayerDamage = 300;

            // Disable full hp on new level
            playerData.Hp = 700;
            playerData.TotalHP = 700;
            playerData.Mana = 500;
            playerData.TotalMana = 500;
            playerData.GameLevel = 3;

            // Set enemy data
            enemyData.EnemyHP = 150;
            // enemyData.EnemyDamage = 50;
            enemyData.EnemyDamage = 7;
            enemyData.ScoreOnKill = 40;
        }
    }

    public void RestartStates()
    {
        Debug.Log("Game restarted");

        gameState.ReadyToPlay = false;

        playerData.GameLevel = 1;
        SetGameStates(1);
        playerData.PlayerDied = false;

        gameState.EnemyKilled = 0;

        // Scene
        sceneState.SceneForLvl2Reloaded = false;
        sceneState.SceneForLvl3Reloaded = false;

        PlaneStates planeStates = GameObject.FindGameObjectWithTag("XROriginObjectTag").GetComponent<PlaneStates>();
        planeStates.enemyCount = gameState.EnemyCountLvl1;
        planeStates.enemyCountUpdatedLvl2 = false;
        planeStates.enemyCountUpdatedLvl3 = false;
        planeStates.spawning = false;

        GameplayUIScript gameplayUIScript = GameObject.FindGameObjectWithTag("GamePlayUITag").GetComponent<GameplayUIScript>();

        gameplayUIScript.leveledUpToLevel2 = false;
        gameplayUIScript.leveledUpToLevel3 = false;

        gameState.GameFinished = false;

    }
}
