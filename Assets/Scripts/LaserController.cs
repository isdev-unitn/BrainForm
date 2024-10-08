using System.Collections;
using System.Threading.Tasks;
using CortexBenchmark;
using Gtec.UnityInterface;
using UnityEngine;


public class LaserController : MonoBehaviour
{
    [SerializeField] GameObject targetEnemy;
    [SerializeField] ParticleSystem enemyExplosion;
    [SerializeField] float activationTime;
    [SerializeField] private AudioSource laserSound;
    [SerializeField] private AudioSource explosionSound;
    [SerializeField] private TaskController2D task;

    private bool isActivated = false;
    private bool targetDestroyed = false;
    private FlashObject2D parentFlashObject;

    public void Start()
    {
        parentFlashObject = GetComponentInParent<FlashObject2D>();
    }

    public void ActivateBeam()
    {
        if (!isActivated)
        {
            isActivated = true;
            laserSound.Play();
            gameObject.SetActive(true);
            StartCoroutine(LaserTime(activationTime));
        }
    }

    public void DectivateBeam()
    {
        gameObject.SetActive(false);
        isActivated = false;
        if (!targetDestroyed)
        {
            task.TargetMiss(parentFlashObject.ClassId);
        }
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
            task.TargetHit(parentFlashObject.ClassId);
            targetDestroyed = true;
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
