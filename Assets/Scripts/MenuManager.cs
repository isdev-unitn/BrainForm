using Gtec.UnityInterface;
using UnityEngine;

static class MenuConstants
{
    public const int Level_menu = 0;
    public const string BciConfigurationTag = "BciConfiguration";
    public const string BciSystemTag = "BciSystem";
    public const string AppTargetTempTag = "AppTargetTemp";
    public const string FlashControllerTag = "FlashController";
}

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject bciControllerMenu;
    [SerializeField] private GameObject trainingTarget;
    private bool mainMenuState;
    private bool bciControllerMenuState;
    private bool trainingTargetState;

    private void Awake()
    {
        DontDestroyOnLoad(GameObject.FindGameObjectWithTag(MenuConstants.BciSystemTag));
        DontDestroyOnLoad(GameObject.FindGameObjectWithTag(MenuConstants.AppTargetTempTag));
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
