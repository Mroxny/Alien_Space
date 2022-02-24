using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    public GameObject projectile;
    public Color projectileColor;
    public float shootDelay = .5f;
    public int lifes = 3;
    public GameObject playerMesh;
    public GameObject playerCanvas;
    public TextMeshProUGUI tmLifes;
    public TextMeshProUGUI tmScore;
    public ParticleSystem explode;
    public AudioSource audioSource;
    public AudioClip shot;
    public AudioClip explosion;
    public AudioClip takeDamage;


    private Vector2 shootPoint;
    private bool canShoot=true;
    private bool canHurt = true;
    private bool isDead = false;
    private int score = 0;


    // Start is called before the first frame update
    void Start()
    {
        DisplayLifes();
        DisplayScore();
    }

    #region UI

    public void SetActivePlayerUI(bool val) {
        playerCanvas.SetActive(val);
    }

    public void DisplayLifes() {
        tmLifes.text = lifes.ToString();
    }

    public void DisplayScore()
    {
        tmScore.text = "Score: "+ score;
    }

    public void AddScore(int score) {
        this.score += score;
        DisplayScore();
    }

    public int GetScore() {
        return this.score;
    }

    #endregion

    #region Shooting

    public void Shoot() {
        if (canShoot && canHurt && !isDead) StartCoroutine(StartShooting(shootDelay));
    }

    private IEnumerator StartShooting(float delay) {
        canShoot = false;

        shootPoint = new Vector2(transform.position.x, transform.position.y + .2f);

        projectile.GetComponent<ProjectileController>().color = projectileColor;
        projectile.GetComponent<ProjectileController>().direction = new Vector2(0, 1);
        projectile.GetComponent<ProjectileController>().isFriendly = true;

        Instantiate(projectile, shootPoint, Quaternion.identity);

        audioSource.PlayOneShot(shot, .5f);

        yield return new WaitForSeconds(delay);
        canShoot = true;
    }

    #endregion

    #region Damage player

    private void OnTriggerEnter2D(Collider2D collision)
    {

        // Collision with projectile
        if (collision.CompareTag("Projectile")){
            if (!collision.GetComponent<ProjectileController>().isFriendly && canHurt && !isDead){

                //Damage player
                StartCoroutine(collision.GetComponent<ProjectileController>().DestroyProjectile(1));
                StartCoroutine(DamagePlayer(2));

            }
        }
    }

    private IEnumerator DamagePlayer(float delay) {
        Vibrate();

        if (lifes > 1)
        {
            canHurt = false;
            playerMesh.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, .5f);
            lifes--;
            DisplayLifes();
            audioSource.PlayOneShot(takeDamage, .5f);

            yield return new WaitForSeconds(delay);

            canHurt = true;
            playerMesh.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1f);
        }
        else {
            StartCoroutine(DestroyPlayer(1));
        }
        
    }

    private IEnumerator DestroyPlayer(float time) {
        lifes--;
        DisplayLifes();


        playerMesh.GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;
        Destroy(GetComponent<ParticleSystem>());

        explode.gameObject.SetActive(true);
        if (explode.isPlaying) explode.Play();

        audioSource.PlayOneShot(explosion, .75f);   //but there is no sound in space!!! >:<

        yield return new WaitForSeconds(time);

        //Player Lost
        isDead = true;
        playerMesh.SetActive(false);
        GameObject sceneManager = GameObject.FindWithTag("SceneManager");
        sceneManager.GetComponent<GameManager>().ShowEndPanel("You Lost", GetScore());

    }

    #endregion

    private void Vibrate() {
        string keyName = GameManager.SETTINGS_VIBRATIONS;

        float duration = .5f;
        float magnitude = .05f;


        if(GameObject.FindWithTag("MainCamera") != null) 
            GameObject.FindWithTag("MainCamera").GetComponent<CameraShake>().Shake(duration, magnitude);

        if (PlayerPrefs.GetInt(keyName)>=1) {
            Handheld.Vibrate();
            Debug.Log("Vibrate");
        }
        
    }
}
