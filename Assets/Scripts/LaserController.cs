using System;
using System.Collections;
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
    private bool isFiring = false;
    private bool targetHit = false;
    private FlashObject2D parentFlashObject;
    private EnemiesTaskManager enemiesTaskManager;

    void Start()
    {
        parentFlashObject = GetComponentInParent<FlashObject2D>();
        enemiesTaskManager = GameObject.FindGameObjectWithTag(DocConstants.EnemiesTaskManagerTag).GetComponent<EnemiesTaskManager>();
    }

    void ActivateBeam()
    {
        if (!isFiring)
        {
            isFiring = true;
            targetHit = false;
            laserSound.Play();
            gameObject.SetActive(true);
            //StartCoroutine(LaserTime(activationTime));
        }
    }

    public void DeactivateBeam()
    {
        gameObject.SetActive(false);
        isFiring = false;
        if (!targetHit)
        {
            task.TargetMiss(parentFlashObject.ClassId);
        }
    }

    private IEnumerator LaserTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        DeactivateBeam();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Vector3 enemyPosition;

        if (targetEnemy && collider.name == targetEnemy.name)
        {
            task.TargetHit(parentFlashObject.ClassId);
            targetHit = true;
            enemyPosition = targetEnemy.transform.position;
            Destroy(targetEnemy);
            explosion(enemyPosition);
            enemiesTaskManager.CheckIfTaskComplete();
        }
    }

    private void explosion(Vector3 enemyPosition)
    {
        enemyExplosion.transform.position = enemyPosition;
        enemyExplosion.Play();
        explosionSound.Play();
    }
}
