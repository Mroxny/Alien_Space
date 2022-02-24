using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public Color color;
    public Vector2 direction = new Vector2(0,1);
    public float speed = 1;
    public bool isFriendly;
    public ParticleSystem particle;

    void Start()
    {
        // Set color of projectile

        ParticleSystem.MainModule settings;

        if (GetComponent<ParticleSystem>() != null) {
            settings = GetComponent<ParticleSystem>().main;
            settings.startColor = color;
        }

        GetComponent<SpriteRenderer>().color = color;
        settings = particle.main;
        settings.startColor = color;

    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Projectile")) StartCoroutine(DestroyProjectile(1));
        if (collision.CompareTag("Map end")) Destroy(gameObject);
    }

    public IEnumerator DestroyProjectile(float time) {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
        if (GetComponent<ParticleSystem>() != null) Destroy(GetComponent<ParticleSystem>());


        speed = 0;

        particle.gameObject.SetActive(true);
        if(particle.isPlaying) particle.Play();

        yield return new WaitForSeconds(time);

        if (gameObject != null) Destroy(gameObject); 
    }

}
