using System.Collections.Generic;
using UnityEngine;
using Gtec.UnityInterface;
using CortexBenchmark;
using System.Collections;

static class DocConstants
{
    public const string DocTag = "Player";
    public const string FallDetectorTag = "FallDetector";
    public const string MainCameraTag = "MainCamera";
    public const string CheckPointTag = "CheckPoint";
    public const string BciActivator01Tag = "BciActivator_01";
    public const string BciActivator02Tag = "BciActivator_02";
    public const string EnemyTag = "Enemy";
    public const string RestTargetTag = "RestTarget";
    public const string EnemiesTaskManagerTag = "EnemiesTaskManager";
    public const int EndTaskWaitTime = 1;
}

public class DocManager : MonoBehaviour
{
    [SerializeField] private Rigidbody2D docBody;
    [SerializeField] private GameObject fallDetector;
    [SerializeField] private GameObject enemiesTargets;
    [SerializeField] private GameObject portalTargets;
    [SerializeField] private AudioSource deathSound;
    [SerializeField] private float bciTasksCameraDistance;
    [SerializeField] private List<TaskController2D> taskControllers2D;

    private Vector3 respawnPoint;
    private CameraController mainCamera;
    private DocMovement docMovement;
    private float baseCameraDistance;
    private BCIConfiguration bciConfiguration;
    private BCIController bciController;
    private FlashController flashController;
    private SceneChanger sceneChanger;
    private Transform endGame;
    private TaskController2D currentTaskController;

    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag(DocConstants.MainCameraTag).GetComponent<CameraController>();
        docMovement = gameObject.GetComponent<DocMovement>();
        baseCameraDistance = mainCamera.distance;
        SetRespawnPoint();

        bciConfiguration = GameObject.FindGameObjectWithTag(MenuConstants.BciConfigurationTag).GetComponent<BCIConfiguration>();
        bciController = GameObject.FindGameObjectWithTag(MenuConstants.BciControllerTag).GetComponent<BCIController>();
        flashController = GameObject.FindGameObjectWithTag(MenuConstants.FlashControllerTag).GetComponent<FlashController>();
        sceneChanger = FindObjectOfType<SceneChanger>();

        // get the endgame object for activate the color sequence objects at the second bci task
        endGame = GameObject.FindGameObjectWithTag(EndGameConstants.EndGameTag).transform;
    }

    void Update()
    {
        // moves the fall detector to keep it always under the player
        fallDetector.transform.position = new Vector3(transform.position.x, fallDetector.transform.position.y, fallDetector.transform.position.z);
    }

    public void BackToMenu()
    {
        sceneChanger.FadeToLevel(MenuConstants.Level_menu);
    }

    public IEnumerator DisableBciTaskTrigger(string activator)
    {
        yield return new WaitForSeconds(DocConstants.EndTaskWaitTime);
        GameObject.FindGameObjectWithTag(activator).GetComponent<BoxCollider2D>().enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(DocConstants.FallDetectorTag))
        {
            docDeath();
        }
        else if (collision.CompareTag(DocConstants.CheckPointTag))
        {
            Debug.Log("CheckPoint reached");
            SetRespawnPoint();
        }
        else if (collision.CompareTag(DocConstants.BciActivator01Tag))
        {
            Debug.Log("Activate BCI task 01");
            ActivateBciTask(enemiesTargets, 10);
            currentTaskController = taskControllers2D[0];
            currentTaskController.StartTaskTimer();
        }
        else if (collision.CompareTag(DocConstants.BciActivator02Tag))
        {
            Debug.Log("Activate BCI task 02");
            ActivateBciTask(portalTargets, 10);
            currentTaskController = taskControllers2D[1];
            currentTaskController.StartTaskTimer();

            // also activate the color sequence
            DeActivateColorSequence(true);
        }
        else if (collision.CompareTag(EndGameConstants.EndGameTag))
        {
            Debug.Log("End Game reached");
            docMovement.CanMove = false;
            BackToMenu();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(DocConstants.BciActivator01Tag))
        {
            // turn off all laser to avoid having them always active if you leave the task with an active laser
            foreach (Transform enemyTarget in enemiesTargets.transform)
            {
                if (enemyTarget.gameObject.tag != DocConstants.RestTargetTag && enemyTarget.GetChild(3).gameObject.activeInHierarchy == true)
                {
                    enemyTarget.GetChild(3).GetComponent<LaserController>().DeactivateBeam();
                }
            }

            DeactivateBciTask(enemiesTargets);
        }
        else if (collision.CompareTag(DocConstants.BciActivator02Tag))
        {
            DeactivateBciTask(portalTargets);

            // also deactivate the color sequence
            DeActivateColorSequence(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(DocConstants.EnemyTag)) // if player touches an enemy
        {
            docDeath();
        }
    }

    private void SetRespawnPoint() // set the respawn point to the current player position
    {
        respawnPoint = transform.position;
    }

    private void ActivateBciTask(GameObject taskTargets, uint classes)
    {
        // set camera distance an block rotation for a better field view during the bci task
        mainCamera.distance = bciTasksCameraDistance;
        mainCamera.CanRotate = false;
        docMovement.CanMove = false;

        // activate task targets
        taskTargets.SetActive(true);

        // set number of classes for the task
        bciConfiguration.NumberOfClasses = classes;

        // START preparing task targets list 
        List<NeurofeedbackTarget> taskTargetElements = new List<NeurofeedbackTarget>();
        GameObject taskTarget;

        for (int i = 0; i < taskTargets.transform.childCount; i++)
        {
            taskTarget = taskTargets.transform.GetChild(i).gameObject;

            if (taskTarget.tag != DocConstants.RestTargetTag)
            {
                taskTargetElements.Add(taskTarget.GetComponent<NeurofeedbackTarget>());
            }
        }
        // END preparing task target list

        // add new targets to the flash controller for the current task
        flashController.SetApplicationObjects(taskTargetElements);

        // application on
        bciController.OnBtnAppOnClicked();
    }

    private void DeactivateBciTask(GameObject taskTargets)
    {
        // application off
        bciController.OnBtnAppOffClicked();

        // reset camera distance and rotation to normal
        mainCamera.distance = baseCameraDistance;
        mainCamera.CanRotate = true;
        docMovement.CanMove = true;

        // deactivate task targets
        taskTargets.SetActive(false);
    }

    private void DeActivateColorSequence(bool state)
    {
        for (int i = 0; i < endGame.childCount; i++)
        {
            endGame.GetChild(i).gameObject.SetActive(state);
        }
    }

    private void docDeath()
    {
        deathSound.Play();
        transform.position = respawnPoint;
    }
}
