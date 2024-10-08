using System.Collections;
using UnityEngine;


public class LaserController : MonoBehaviour
{
    [SerializeField] GameObject targetEnemy;
    [SerializeField] ParticleSystem enemyExplosion;
    [SerializeField] float activationTime;
    [SerializeField] private AudioSource laserSound;
    [SerializeField] private AudioSource explosionSound;

    private bool isActive = false;

    public void ActivateBeam()
    {
        if (!isActive)
        {
            isActive = true;
            laserSound.Play();
            gameObject.SetActive(true);
            StartCoroutine(LaserTime(activationTime));
        }
    }

    public void DectivateBeam()
    {
        gameObject.SetActive(false);
        isActive = false;
    }

    private IEnumerator LaserTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        DectivateBeam();
    }


    private void OnTriggerEnter2D(Collider2D collider)
    {
        Vector3 enemyPosition;

        if (targetEnemy && collider.name == targetEnemy.name)
        {
            enemyPosition = targetEnemy.transform.position;
            Destroy(targetEnemy);
            explosion(enemyPosition);
        }
    }

    private void explosion(Vector3 enemyPosition)
    {
        enemyExplosion.transform.position = enemyPosition;
        enemyExplosion.Play();
        explosionSound.Play();
    }
}
