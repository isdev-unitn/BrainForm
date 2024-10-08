using UnityEngine;


public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private GameObject bciSystem;
    public float distance;
    [SerializeField] private float speed;
    private float lookAhead;
    private bool canRotate = true;
    public bool CanRotate
    {
        get { return canRotate; }
        set { canRotate = value; }
    }

    private void Start()
    {
        bciSystem = GameObject.FindGameObjectWithTag(MenuConstants.BciSystemTag);
    }

    private void Update()
    {
        transform.position = new Vector3(player.position.x + lookAhead, transform.position.y, transform.position.z);
        float cameraMovement = distance * player.localScale.x;

        // if facing left, set the camera to go left, otherwise leave it to right
        if (player.rotation.y == -1f && canRotate)
        {
            cameraMovement *= -1;
        }

        lookAhead = Mathf.Lerp(lookAhead, cameraMovement, speed * Time.deltaTime);

        // move the bci system with the player to keep the signal quality bars in place
        if (bciSystem != null)
        {
            bciSystem.transform.position = new Vector3(transform.position.x, bciSystem.transform.position.y, bciSystem.transform.position.z);
        }
    }

}
