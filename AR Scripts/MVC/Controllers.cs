using System.Collections;
using System.Xml;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARCore;
using UnityEngine.XR.ARFoundation;

public class Controllers : MonoBehaviour
{
    // All of request should be here
    Models models;
    View view;
    void Start()
    {
        models = gameObject.GetComponent<Models>();
        view = gameObject.GetComponent<View>();
    }

    public void RestartScene()
    {
        var xrManagerSettings = UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager;
        xrManagerSettings.DeinitializeLoader();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // reload current scene
        xrManagerSettings.InitializeLoaderSync();

        models.SetGameStates(1);
    }

    public void ClearPlanes()
    {
        // ARSession arSession = GameObject.FindGameObjectWithTag("ARSessionTag").GetComponent<ARSession>();
        // arSession.Reset();

        var xrManagerSettings = UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager;
        xrManagerSettings.DeinitializeLoader();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // reload current scene
        xrManagerSettings.InitializeLoaderSync();
    }

    // public void EnemytAttack(int damage)
    // {
    //     models.playerData.Hp -= damage;
    //     // view.UpdatePlayerHP();
    // }
}