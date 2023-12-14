using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightShadeSkills : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    public void Skill1()
    {
        Debug.Log("skiill 1 niightshade");

        GameObject player = GameObject.FindGameObjectWithTag("PlayerTag");
        for (int i = 0; i < 3; i++)
        {
            Debug.Log(player);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
