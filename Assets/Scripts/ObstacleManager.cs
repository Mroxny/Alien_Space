using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    public int lifes = 5;
    public GameObject healthBar;
    public ParticleSystem explode;
    public AudioSource audioSource;
    public AudioClip takeDamage;
    public AudioClip explosion;

    private Vector2 orgHealthScale;
    private int orgLifes;


    void Start()
    {
        orgHealthScale = healthBar.transform.localScale;
        orgLifes = lifes;
    }

    public float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        // Collision with projectile
        if (collision.CompareTag("Projectile"))
        {
            //Damage obstacle

            if (!collision.GetComponent<ProjectileController>().isFriendly)
            {
                StartCoroutine(collision.GetComponent<ProjectileController>().DestroyProjectile(1));
                DamageObstacle(1);
            }
            if (collision.name.Contains("Projectile2")) {
                StartCoroutine(collision.GetComponent<ProjectileController>().DestroyProjectile(1));
                DamageObstacle(3);
            }
        }
    }

    private void DamageObstacle(int damage) {
        Shake(.1f, .02f);
        if (lifes > 1)
        {
            lifes-= damage;
            UpdateHealthBar();
            audioSource.PlayOneShot(takeDamage, .25f);

        }
        else
        {
            StartCoroutine(DestroyObstacle(1));
        }
    }


    private IEnumerator DestroyObstacle(float time) {
        lifes--;

        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
        Destroy(healthBar);

        explode.gameObject.SetActive(true);
        if (explode.isPlaying) explode.Play();

        audioSource.PlayOneShot(explosion, .25f);

        yield return new WaitForSeconds(time);

        Destroy(gameObject);

    }

    private void UpdateHealthBar() {
        Vector3 newScale = new Vector3(Remap(lifes, 0, orgLifes, 0, orgHealthScale.x), orgHealthScale.y);
        healthBar.transform.localScale = newScale;
    }

    public void Shake(float duration, float magnitude)
    {
        StartCoroutine(StartShake(duration, magnitude));
    }

    private IEnumerator StartShake(float duration, float magnitude)
    {
        Vector3 orginalPos = transform.position;
        float elapsed = 0;

        while (elapsed < duration)
        {
            float x = Random.Range(orginalPos.x-magnitude, orginalPos.x + magnitude) ;
            float y = Random.Range(orginalPos.y - magnitude, orginalPos.y + magnitude) ;

            transform.position = new Vector3(x, y, orginalPos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.position = orginalPos;
    }
}
