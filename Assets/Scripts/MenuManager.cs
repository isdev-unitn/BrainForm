using Gtec.UnityInterface;
using UnityEngine;
using UnityEngine.UI;

static class MenuConstants
{
    public const int Level_menu = 0;
    public const string BciConfigurationTag = "BciConfiguration";
    public const string BciSystemTag = "BciSystem";
    public const string AppTargetTempTag = "AppTargetTemp";
    public const string FlashControllerTag = "FlashController";
    public const string TrainingTargetTag = "TrainingTarget";
    public const string OutlineControllerTag = "OutlineController";
}

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    private GameObject trainingTarget;
    private GameObject bciControllerMenu;
    private OutlineController outlineController;
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

        // problema attuale: la posizione del bci system va fixata ogni volta che apri il menu
        bciControllerMenu = GameObject.FindGameObjectWithTag(MenuConstants.BciSystemTag).transform.GetChild(2).GetChild(5).gameObject;
        trainingTarget = GameObject.FindGameObjectWithTag(MenuConstants.BciSystemTag).transform.GetChild(2).GetChild(4).gameObject;
        outlineController = GameObject.FindGameObjectWithTag(MenuConstants.OutlineControllerTag).GetComponent<OutlineController>();

        // add listeners for functions between bci system and menu scene
        bciControllerMenu.transform.GetChild(1).gameObject.GetComponent<Button>().onClick.AddListener(OpenCloseBCIMenu);
        trainingTarget.GetComponent<FlashObject2D>().OnActivated.AddListener(delegate { outlineController.ActivateOutline(trainingTarget.transform.GetChild(2).GetChild(0).gameObject); });
        trainingTarget.GetComponent<FlashObject2D>().OnDeactivated.AddListener(delegate { outlineController.DeactivateOutline(trainingTarget.transform.GetChild(2).GetChild(0).gameObject); });
    }

    void Start()
    {
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
}
