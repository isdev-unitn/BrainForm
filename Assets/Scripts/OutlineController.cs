using UnityEngine;

public class OutlineController : MonoBehaviour
{
    public void ActivateOutline([SerializeField] GameObject outline)
    {
        outline.SetActive(true);
    }

    public void DeactivateOutline([SerializeField] GameObject outline)
    {
        outline.SetActive(false);
    }
}
