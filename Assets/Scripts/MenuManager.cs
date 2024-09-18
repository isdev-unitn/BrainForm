using Gtec.UnityInterface;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEditor;

static class MenuConstants
{
    public const int Level_menu = 0;
    public const string BciConfigurationTag = "BciConfiguration";
    public const string BciSystemTag = "BciSystem";
    public const string FlashControllerTag = "FlashController";
    public const string TrainingTargetTag = "TrainingTarget";
    public const string AppTargetTempTag = "AppTargetTemp";
}

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    private GameObject trainingTarget;
    private GameObject bciSystem;
    private GameObject bciControllerMenu;
    private bool mainMenuState;
    private bool bciControllerMenuState;
    private bool trainingTargetState;


    private void Awake()
    {
        // START avoid having duplicate objects due to dontdestroyonload
        if (GameObject.FindGameObjectsWithTag(MenuConstants.BciSystemTag).Length > 1)
        {
            // the object in dontdestroyonload is always the firs in the list, so we delete the second which is the new and duplicate one
            Destroy(GameObject.FindGameObjectsWithTag(MenuConstants.BciSystemTag)[1]);
        }
        // END avoid having duplicate objects due to dontdestroyonload

        DontDestroyOnLoad(GameObject.FindGameObjectWithTag(MenuConstants.BciSystemTag));
        DontDestroyOnLoad(GameObject.FindGameObjectWithTag(MenuConstants.AppTargetTempTag));

        bciSystem = GameObject.FindGameObjectWithTag(MenuConstants.BciSystemTag);
        bciControllerMenu = bciSystem.transform.GetChild(2).GetChild(5).gameObject;
        trainingTarget = GameObject.FindGameObjectWithTag(MenuConstants.BciSystemTag).transform.GetChild(2).GetChild(4).gameObject;

        // add listener for function between bci system and menu scene
        bciControllerMenu.transform.GetChild(1).gameObject.GetComponent<Button>().onClick.AddListener(OpenCloseBCIMenu);
    }

    void Start()
    {
        bciSystem.transform.position = new Vector3(0, 0, 0); // reset bci system position to the center
        setFlashControllerTargets(); // reset targets for flashcontroller

        mainMenuState = mainMenu.activeInHierarchy;
        bciControllerMenuState = bciControllerMenu.activeInHierarchy;
        trainingTargetState = trainingTarget.activeInHierarchy;
    }

    public void OpenCloseBCIMenu()
    {
        mainMenuState = !mainMenuState;
        bciControllerMenuState = !bciControllerMenuState;
        trainingTargetState = !trainingTargetState;

        mainMenu.SetActive(mainMenuState);
        bciControllerMenu.SetActive(bciControllerMenuState);
        trainingTarget.SetActive(trainingTargetState);
    }

    public void QuitGame()
    {
        Application.Quit();
    }


    private void setFlashControllerTargets()
    {
        GameObject taskTargets = GameObject.FindGameObjectWithTag(MenuConstants.AppTargetTempTag);
        // set number of classes, initially set at 10 to handle the biggest task in the game level 
        bciSystem.transform.GetChild(3).gameObject.GetComponent<BCIConfiguration>().NumberOfClasses = 10;

        // START preparing task targets list 
        List<NeurofeedbackTarget> taskTargetElements = new List<NeurofeedbackTarget>();

        for (int i = 0; i < taskTargets.transform.childCount; i++)
        {
            taskTargetElements.Add(taskTargets.transform.GetChild(i).gameObject.GetComponent<NeurofeedbackTarget>());
        }
        // END preparing task target list

        // add new targets to the flash controller
        bciSystem.transform.GetChild(1).gameObject.GetComponent<FlashController>().SetApplicationObjects(taskTargetElements);
    }

}
