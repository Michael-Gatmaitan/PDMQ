using System.Collections;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    // Start is called before the first frame update
    // Spawn shuriken towards current position
    public GameObject shuriken;
    public GameObject luquidSphere;
    // private Camera arCamera;
    AudioSource source;
    AudioClip clip;

    public AudioClip[] attackClips = new AudioClip[2];

    void Start()
    {
        source = GetComponent<AudioSource>();
        // arCamera = Camera.main;
    }
    public void ShurikenAttack()
    {
        Instantiate(shuriken, gameObject.transform.position, Random.rotation);
        StartCoroutine(PlayAudio());
        // Move the shuriken
    }

    public void LiquidSphereAttack()
    {
        Instantiate(luquidSphere, gameObject.transform.position, Random.rotation);
        // StartCoroutine(PlayAudio());
        // Move the shuriken
    }

    IEnumerator PlayAudio()
    {
        clip = attackClips[Random.Range(0, attackClips.Length - 1)];
        source.clip = clip;
        source.Play();
        yield return new WaitForSeconds(source.clip.length);
    }
}
