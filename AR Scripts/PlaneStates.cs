using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;

public class PlaneStates : MonoBehaviour
{
    // Start is called before the first frame update
    private ARPlaneManager planeManager;
    // private ARRaycastManager aRRaycastManager;
    // private TrackableId planeID;

    public GameObject[] zombiesPrefabs;
    public GameObject bullet;

    private ARPlane biggestPlane;
    private float biggestPlaneArea = 0;
    private readonly float validPlaneArea = 5;

    public int enemyCount;
    private int totalEnemies;
    VisualElement gamePlayUIRoot;
    VisualElement spawnHandle;
    VisualElement gamePlayContainer;
    VisualElement planeWarnerContainer;

    Models models;
    View view;
    Controllers controllers;

    public Camera arCamera;
    void Start()
    {
        gamePlayUIRoot = GameObject.FindGameObjectWithTag("GamePlayUITag").GetComponent<UIDocument>().rootVisualElement;
        gamePlayContainer = gamePlayUIRoot.Q<VisualElement>("GameplayContainer");
        planeWarnerContainer = gamePlayUIRoot.Q<VisualElement>("PlaneWarnerContainer");
        spawnHandle = gamePlayUIRoot.Q<VisualElement>("SpawnHandle");
        planeManager = gameObject.GetComponent<ARPlaneManager>();
        // aRRaycastManager = GetComponent<ARRaycastManager>();
        models = GameObject.FindGameObjectWithTag("GamePlayUITag").GetComponent<Models>();
        view = GameObject.FindGameObjectWithTag("GamePlayUITag").GetComponent<View>();
        controllers = GameObject.FindGameObjectWithTag("GamePlayUITag").GetComponent<Controllers>();
        enemyCount = models.gameState.EnemyCountLvl1;
        totalEnemies = models.gameState.EnemyCountLvl1 + models.gameState.EnemyCountLvl2 + models.gameState.EnemyCountLvl3;

        VisualElement gameFinishedModalContainer = gamePlayUIRoot.Q<VisualElement>("GameFinishedModalContainer");
        gameFinishedModalContainer.style.display = DisplayStyle.None;

        arCamera = Camera.main;

        models.gameState.ReadyToPlay = false;

        view.UpdateGameStats();
    }

    public bool spawning = false;
    public bool enemyCountUpdatedLvl2 = false;
    public bool enemyCountUpdatedLvl3 = false;

    private bool regenCalled = false;
    void Update()
    {


        if (!regenCalled)
        {

            StartCoroutine(RegenHP(1));
            regenCalled = true;
        }

        IEnumerator RegenHP(int regenValue)
        {
            int hp = models.playerData.Hp;
            int totalHP = models.playerData.TotalHP;

            if (hp != totalHP || hp + regenValue != totalHP || hp + regenValue < totalHP)
            {
                yield return new WaitForSeconds(1);
                models.playerData.Hp += regenValue;
                StartCoroutine(RegenHP(regenValue));
            }
            else
            {
                models.playerData.Hp += 0;
                StartCoroutine(RegenHP(0));
            }
        }

        bool planeReady = models.gameState.PlaneReady;
        if (planeReady)
        {
            gamePlayContainer.style.display = DisplayStyle.Flex;
            planeWarnerContainer.style.display = DisplayStyle.None;
        }
        else
        {
            gamePlayContainer.style.display = DisplayStyle.None;
            planeWarnerContainer.style.display = DisplayStyle.Flex;
        }

        bool readyToPlay = models.gameState.ReadyToPlay;

        int currentGameLevel = models.playerData.GameLevel;
        int enemyKilled = models.gameState.EnemyKilled;

        if (!biggestPlane)
        {
            FindBiggestPlane();
        }

        if (biggestPlane && !spawning && readyToPlay && planeReady)
        {
            StartCoroutine(SpawnShits());
            view.UpdateGameStats();
            spawning = true;
        }

        if (currentGameLevel == 2 && enemyCountUpdatedLvl2 == false)
        {
            // Player prefs
            // PlayerPrefs.SetInt("CurrentLevel", 2);
            // PlayerPrefs.SetInt();
            // Clear planes (new)
            if (!models.sceneState.SceneForLvl2Reloaded)
            {
                models.sceneState.SceneForLvl2Reloaded = true;
                models.gameState.PlaneReady = false;

                StartCoroutine(ClearPlaneDelay());

                IEnumerator ClearPlaneDelay()
                {
                    yield return new WaitForSeconds(
                        // GameObject.FindGameObjectWithTag("EnemyTargetTag").GetComponent<AudioSource>().clip.length
                        3
                    );
                    controllers.ClearPlanes();
                }
            }

            enemyCount = models.gameState.EnemyCountLvl2;

            spawning = false;
            models.gameState.ReadyToPlay = false;
            enemyCountUpdatedLvl2 = true;

            // StartCoroutine(StepUpLevel());

            if (models.gameState.ReadyToPlay) StartCoroutine(SpawnShits());
        }

        if (currentGameLevel == 3 && enemyCountUpdatedLvl3 == false)
        {
            if (!models.sceneState.SceneForLvl3Reloaded)
            {
                models.sceneState.SceneForLvl3Reloaded = true;
                models.gameState.PlaneReady = false;

                StartCoroutine(ClearPlaneDelay());

                IEnumerator ClearPlaneDelay()
                {
                    yield return new WaitForSeconds(
                        // GameObject.FindGameObjectWithTag("EnemyTargetTag").GetComponent<AudioSource>().clip.length
                        3
                    );
                    controllers.ClearPlanes();
                }
            }

            enemyCount = models.gameState.EnemyCountLvl3;

            spawning = false;
            models.gameState.ReadyToPlay = false;
            enemyCountUpdatedLvl3 = true;

            // StartCoroutine(StepUpLevel());
            if (models.gameState.ReadyToPlay) StartCoroutine(SpawnShits());
        }

        if (currentGameLevel == 3 && enemyKilled == totalEnemies && enemyCountUpdatedLvl3 && !models.gameState.GameFinished)
        {
            // PlayerPrefs.SetString("LeaderboardScoresAndName", "");
            Debug.Log("You have finished the game!");
            // Player finished the game
            models.gameState.GameFinished = true;
            view.PlayerFinishedGame();
            models.SetGameStates(1);
            models.RestartStates();
        }

        // IEnumerator StepUpLevel()
        // {
        //     spawning = true;
        //     Debug.Log("Next level approching...");

        //     // Show UI
        //     int currentLevel = currentGameLevel - 1;
        //     VisualElement levelUpModalContainer = gamePlayUIRoot.Q("LevelUpModalContainer");

        //     Label finishedLevelLabel = gamePlayUIRoot.Q<Label>("FinishedLevelLabel");
        //     Label newLevelSecsLabel = gamePlayUIRoot.Q<Label>("NewLevelSecsLabel");

        //     levelUpModalContainer.style.display = DisplayStyle.Flex;
        //     finishedLevelLabel.text = "You have finished the Stage " + currentLevel + "!";

        //     int secs = 5;

        //     StartCoroutine(GoToNextLevel());

        //     IEnumerator GoToNextLevel()
        //     {
        //         newLevelSecsLabel.text = secs.ToString();
        //         yield return new WaitForSeconds(1);
        //         secs -= 1;

        //         if (secs != 0) StartCoroutine(GoToNextLevel());
        //         else
        //         {
        //             spawning = true;
        //             levelUpModalContainer.style.display = DisplayStyle.None;
        //         }
        //     }

        //     // spawning = true;
        //     // levelUpModalContainer.style.display = DisplayStyle.None;


        //     yield return new WaitForSeconds(5);
        //     Debug.Log("Next level arrived");

        //     StartCoroutine(SpawnShits());
        // }
    }

    ARPlane FindBiggestPlane()
    {

        if (planeManager.trackables.count <= 0) return null;

        foreach (ARPlane aRPlane in planeManager.trackables)
        {
            float currentPlaneSize = aRPlane.size.x * aRPlane.size.y;

            if (currentPlaneSize > biggestPlaneArea)
            {
                biggestPlaneArea = currentPlaneSize;
                // Debug.Log(biggestPlaneArea);

                if (biggestPlaneArea > validPlaneArea)
                {
                    biggestPlane = aRPlane;
                    models.gameState.PlaneReady = true;
                }
                else Debug.Log("NOT Valid area");
            }
        }

        return biggestPlane;
    }

    IEnumerator SpawnShits()
    {
        if (models.playerData.PlayerDied == false && models.gameState.ReadyToPlay)
        {
            Transform planeTransform = biggestPlane.transform != null ? biggestPlane.transform : FindBiggestPlane().transform;

            float planeSzX = biggestPlane.size.x;
            float planeSzY = biggestPlane.size.y;
            float planeXDevided = (planeSzX - (planeSzX / 2)) / 2;
            float planeYDevided = (planeSzY - (planeSzY / 2)) / 2;

            float randXFrom = planeTransform.position.x - planeXDevided;
            float randXTo = planeTransform.position.x + planeXDevided;

            float randX = Random.Range(randXFrom, randXTo);

            float randZFrom = planeTransform.position.z - planeYDevided;
            float randZTo = planeTransform.position.z + planeYDevided;

            float randZ = Random.Range(randZFrom, randZTo);

            // Random number 0 - 7
            int rand = Random.Range(0, zombiesPrefabs.Length);
            GameObject zombie = zombiesPrefabs[rand];

            GameObject spawnedPrefab =
                Instantiate(zombie, new Vector3(0, 0, 0), Quaternion.identity);
            // float prefabYPos = planeTransform.position.y + zombie.transform.localScale.y;
            spawnedPrefab.transform.position =
                new Vector3(randX, arCamera.transform.position.y, randZ);

            spawnHandle.style.transitionDuration = new List<TimeValue> { .5f };
            spawnHandle.style.width = Length.Percent(100);

            yield return new WaitForSeconds(.5f);

            spawnHandle.style.transitionDuration = new List<TimeValue> { 0 };
            spawnHandle.style.width = Length.Percent(0);

            enemyCount -= 1;
            if (enemyCount > 0) StartCoroutine(SpawnShits());
        }
    }
}
