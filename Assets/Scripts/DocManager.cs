using System.Collections.Generic;
using UnityEngine;
using Gtec.UnityInterface;

static class DocConstants
{
    public const string FallDetectorTag = "FallDetector";
    public const string MainCameraTag = "MainCamera";
    public const string CheckPointTag = "CheckPoint";
    public const string BciActivator01Tag = "BciActivator_01";
    public const string BciActivator02Tag = "BciActivator_02";
    public const string BciActivator03Tag = "BciActivator_03";
    public const string EnemyTag = "Enemy";
}

public class DocManager : MonoBehaviour
{
    [SerializeField] private Rigidbody2D docBody;
    [SerializeField] private GameObject fallDetector;
    [SerializeField] private GameObject enemiesTargets;
    [SerializeField] private GameObject portalTargets;
    [SerializeField] private AudioSource deathSound;
    [SerializeField] private float bciTasksCameraDistance;

    private Vector3 respawnPoint;
    private CameraController mainCamera;
    private float baseCameraDistance;
    private BCIConfiguration bciConfiguration;
    private FlashController flashController;
    private SceneChanger sceneChanger;
    private Transform endGame;


    private void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag(DocConstants.MainCameraTag).GetComponent<CameraController>();
        baseCameraDistance = mainCamera.distance;
        SetRespawnPoint();

        bciConfiguration = GameObject.FindGameObjectWithTag(MenuConstants.BciConfigurationTag).GetComponent<BCIConfiguration>();
        flashController = GameObject.FindGameObjectWithTag(MenuConstants.FlashControllerTag).GetComponent<FlashController>();
        sceneChanger = FindObjectOfType<SceneChanger>();

        // get the endgame object for activate the color sequence objects at the second bci task
        endGame = GameObject.FindGameObjectWithTag(EndGameConstants.EndGameTag).transform;
    }

    private void Update()
    {
        // moves the fall detector to keep it always under the player
        fallDetector.transform.position = new Vector3(transform.position.x, fallDetector.transform.position.y, fallDetector.transform.position.z);
    }

    public void BackToMenu()
    {
        sceneChanger.FadeToLevel(MenuConstants.Level_menu);
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
            ActivateBciTask(enemiesTargets, 5);
        }
        else if (collision.CompareTag(DocConstants.BciActivator02Tag))
        {
            Debug.Log("Activate BCI task 02");
            ActivateBciTask(portalTargets, 10);

            // also activate the color sequence
            DeActivateColorSequence(true);
        }
        else if (collision.CompareTag(EndGameConstants.EndGameTag))
        {
            Debug.Log("End Game reached");
            BackToMenu();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(DocConstants.BciActivator01Tag))
        {
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

        // activate task targets
        taskTargets.SetActive(true);

        // set number of classes for the task
        bciConfiguration.NumberOfClasses = classes;

        // START preparing task targets list 
        List<NeurofeedbackTarget> taskTargetElements = new List<NeurofeedbackTarget>();

        for (int i = 0; i < taskTargets.transform.childCount; i++)
        {
            taskTargetElements.Add(taskTargets.transform.GetChild(i).gameObject.GetComponent<NeurofeedbackTarget>());
        }
        // END preparing task target list

        // add new targets to the flash controller for the current task
        flashController.SetApplicationObjects(taskTargetElements);
    }

    private void DeactivateBciTask(GameObject taskTargets)
    {
        // reset camera distance and rotation to normal
        mainCamera.distance = baseCameraDistance;
        mainCamera.CanRotate = true;

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
