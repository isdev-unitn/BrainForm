using UnityEngine;
using UnityEngine.SceneManagement;


static class SceneChangerConstants
{
    public const string SceneChangeAnimation = "SceneChange";
}

public class SceneChanger : MonoBehaviour
{
    public Animator animator;
    private int levelToLoad;

    public void FadeToLevel(int level)
    {
        levelToLoad = level;
        animator.SetTrigger(SceneChangerConstants.SceneChangeAnimation);
    }

    public void OnFadeEnd()
    {
        SceneManager.LoadSceneAsync(levelToLoad);
    }

}
