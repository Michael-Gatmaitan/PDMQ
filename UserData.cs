
using UnityEngine;

public class UserData : MonoBehaviour
{
    // Start is called before the first frame update
    public string username;
    public string choosenCharacter = "";
    public int gameLevel = 1;

    // Particle

    public GameObject[] buttons;

    void Start()
    {

        // if (PlayerPrefs.GetInt("GameLevel")) {

        // }

        for (int x = 0; x < buttons.Length; x++)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("clicked");
            }
        }
    }

    public void SetUsername(string _username)
    {
        username = _username;
        Debug.Log("Username set as " + username);
        PlayerPrefs.SetString("username", _username);
    }

    public void SetGameLevel(int level)
    {
        gameLevel = level;

    }

    public void SetChoosenCharacter(string _choosenCharacter)
    {
        choosenCharacter = _choosenCharacter;
        Debug.Log("Choosen character set as " + _choosenCharacter);
        PlayerPrefs.SetString("choosenCharacter", _choosenCharacter);
    }

    public string GetUsername() { return username; }
}
