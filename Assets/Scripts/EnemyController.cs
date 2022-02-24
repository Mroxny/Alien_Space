using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public bool isBig = false;
    public GameObject enemyProjectile;
    public float projectileSpeed = 2;
    public Color projectileColor;
    public ParticleSystem explode;
    public AudioSource audioSource;
    public AudioClip shot;
    public AudioClip explosion;

    [HideInInspector]
    public float shootDelay;

    private bool canShoot = false;
    private bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        //Config of shooted projectile

        if (!isBig)
        {
            shootDelay = Random.Range(15f, 30f);
            StartCoroutine(StartShootingAfter(Random.Range(3f, 20f)));
        }
        else {
            shootDelay = Random.Range(3f, 10f);
            StartCoroutine(StartShootingAfter(Random.Range(3f, 4f)));
        }
        

    }

    // Update is called once per frame
    void Update()
    {
        if(canShoot && !isDead) StartCoroutine(Shoot(shootDelay));  
    }

    #region Shooting

    private IEnumerator Shoot(float delay) {
        canShoot = false;
        Vector2 shootPoint = new Vector2(transform.position.x, transform.position.y - .15f);

        enemyProjectile.GetComponent<ProjectileController>().color = projectileColor;
        enemyProjectile.GetComponent<ProjectileController>().speed = projectileSpeed;
        enemyProjectile.GetComponent<ProjectileController>().direction = new Vector2(0, -1);
        enemyProjectile.GetComponent<ProjectileController>().isFriendly = false;

        Instantiate(enemyProjectile, shootPoint, Quaternion.identity);

        audioSource.PlayOneShot(shot, .5f);

        yield return new WaitForSeconds(delay);
        canShoot = true;
    }

    private IEnumerator StartShootingAfter(float delay){
        yield return new WaitForSeconds(delay);
        canShoot = true;
    }

    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {

        // Collision with projectile
        if (collision.CompareTag("Projectile")) {
            if (collision.GetComponent<ProjectileController>().isFriendly) {

                //Destroy
                StartCoroutine(collision.GetComponent<ProjectileController>().DestroyProjectile(1));
                StartCoroutine(DestroyEnemy(1));

            }
        }

        if (collision.CompareTag("Map end")) Destroy(gameObject);

    }

    public IEnumerator DestroyEnemy(float time)
    {
        isDead = true;

        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
        Destroy(GetComponent<ParticleSystem>());

        GameObject player = GameObject.FindWithTag("Player");
        GameObject sceneManager = GameObject.FindWithTag("SceneManager");

        if (player != null){
            if (isBig) player.GetComponent<PlayerManager>().AddScore(500);
            else player.GetComponent<PlayerManager>().AddScore(100);
        }
        if (sceneManager != null && !isBig)
            sceneManager.GetComponent<GameManager>().DecrementEnemy();


        explode.gameObject.SetActive(true);
        if (explode.isPlaying) explode.Play();

        audioSource.PlayOneShot(explosion, .5f);

        yield return new WaitForSeconds(time);

        if (gameObject != null) Destroy(gameObject);

    }
}
