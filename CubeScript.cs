using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class CubeScript : MonoBehaviour
{
    // Start is called before the first frame update
    // public Camera arCamera;
    private bool attacking = false;

    Controllers controllers;
    Models models;

    GameObject mainPlayer;
    Animator animator;
    bool playerMoving = true;

    bool goChase = true;
    bool died = false;

    public AudioSource source;

    private ZombieAudioHandler zombieAudioHandler;
    private AudioClip[] angryClips;
    private AudioClip[] attackClips;
    private AudioClip[] diedClips;

    private VisualElement gameUIRoot;
    private Label hpLabel;
    private VisualElement hpHandle;

    public int thisEnemyHP;
    void Start()
    {

        if (PlayerPrefs.HasKey("VolumeValue"))
        {
            source.volume = PlayerPrefs.GetInt("VolumeValue");
        }


        zombieAudioHandler = GameObject.FindGameObjectWithTag("ZAHTag").GetComponent<ZombieAudioHandler>();
        angryClips = zombieAudioHandler.angryClips;
        attackClips = zombieAudioHandler.attackClips;
        diedClips = zombieAudioHandler.diedClips;

        gameUIRoot = GameObject.FindGameObjectWithTag("GamePlayUITag").GetComponent<UIDocument>().rootVisualElement;
        hpHandle = gameUIRoot.Q<VisualElement>("HPHandle");
        hpLabel = gameUIRoot.Q<Label>("HPLabel");


        source.PlayOneShot(angryClips[Random.Range(0, angryClips.Length - 1)]);

        controllers = GameObject.FindGameObjectWithTag("GamePlayUITag").GetComponent<Controllers>();
        models = GameObject.FindGameObjectWithTag("GamePlayUITag").GetComponent<Models>();
        animator = GetComponent<Animator>();

        // arCamera = Camera.main;
        thisEnemyHP = models.enemyData.EnemyHP;
    }

    // Update is called once per frame
    private float distance;
    private float cameraYPos;
    private Vector3 targetPath;
    void Update()
    {
        transform.LookAt(Camera.main.transform);

        if (goChase && !died)
        {
            distance = Vector3.Distance(Camera.main.transform.position, transform.position);
            cameraYPos = Camera.main.transform.position.y;

            if (distance >= 0.6)
            {
                targetPath = new(Camera.main.transform.position.x, cameraYPos, Camera.main.transform.position.z);
                // Vector3 targetPath = new(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z);

                transform.position = Vector3.MoveTowards(transform.position, targetPath, 0.3f * Time.deltaTime);
            }

            // if (transform.position.z == Camera.main.transform.position.z && )
            // float dist = Vector3.Distance(Camera.main.transform.position, transform.position);
            // Debug.Log(dist);
            // float enemyPosition = Camera.main.transform.position.x - transform.position.x - (Camera.main.transform.position.z - transform.position.z);
            // playerMoving = mainPlayer.GetComponent<Rigidbody>().velocity.magnitude != 0;
        }

        if (thisEnemyHP <= 0 && !died)
        {
            died = true;
            Debug.Log("Enemy died");
            models.playerData.PlayerScore += models.enemyData.ScoreOnKill;

            // animator.SetBool("Walk", false);
            animator.SetBool("Attack", false);
            animator.SetBool("Died", true);
            gameObject.GetComponent<BoxCollider>().enabled = false;

            StartCoroutine(Die());

            IEnumerator Die()
            {
                AudioClip dieClip = diedClips[Random.Range(0, diedClips.Length - 1)];
                source.PlayOneShot(dieClip);

                yield return new WaitForSeconds(dieClip.length);
                // yield return new WaitForSeconds(1);
                animator.SetBool("Died", false);
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        Debug.Log("Collider in");
        animator.SetBool("Attack", true);

        Debug.Log(playerMoving);
        // if (playerMoving)
        // {
        //     attacking = false;
        // }
        // else
        // {
        //     attacking = true;
        // }
        attacking = true;
        goChase = false;

        if (models.playerData.Hp >= 1f && attacking)
        {
            StartCoroutine(Attack(models.enemyData.EnemyDamage));
            StartCoroutine(PlayAttackAudio());
        }
    }

    IEnumerator PlayAttackAudio()
    {
        source.clip = attackClips[Random.Range(0, angryClips.Length - 1)];
        source.Play();

        yield return new WaitForSeconds(source.clip.length);
        // yield return new WaitForSeconds(1);

        if (models.playerData.Hp >= 1f && attacking)
        {
            StartCoroutine(PlayAttackAudio());
        }
    }

    void OnCollisionEnter(Collision col)
    {
        Debug.Log(col);
    }

    void OnTriggerExit(Collider collider)
    {
        animator.SetBool("Attack", false);
        animator.SetBool("Idle", true);

        Debug.Log("Collider off");
        attacking = false;

        goChase = false;
        StartCoroutine(ChaseDelay());


        IEnumerator ChaseDelay()
        {
            yield return new WaitForSeconds(2);
            goChase = true;

            animator.SetBool("Idle", false);
        }
    }

    IEnumerator Attack(int damage)
    {
        if (models.playerData.Hp >= 1f && attacking)
        {
            yield return new WaitForSeconds(0.4f);

            // controllers.EnemytAttack(models.enemyData.EnemyDamage);
            models.playerData.Hp -= models.enemyData.EnemyDamage;

            int playerHP = models.playerData.Hp;
            int playerTotalHP = models.playerData.TotalHP;

            hpLabel.text = playerHP + " / " + playerTotalHP;
            float hpPercentage = (float)(playerHP * 100) / playerTotalHP;

            hpHandle.style.width = Length.Percent(hpPercentage);

            StartCoroutine(Attack(models.enemyData.EnemyDamage));
        }
    }
}
