using UnityEngine;

public class OutlineController : MonoBehaviour
{
    public void ActivateOutline([SerializeField] GameObject outline)
    {
        outline.SetActive(true);
    }

    public void DectivateOutline([SerializeField] GameObject outline)
    {
        outline.SetActive(false);
    }
}
