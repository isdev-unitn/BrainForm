using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private GameObject patrolStart;
    [SerializeField] private GameObject patrolEnd;
    [SerializeField] private float speed;
    private bool enemyGoesRight;
    // Start is called before the first frame update
    void Start()
    {
        enemyGoesRight = Random.Range(0, 2) == 1 ? true : false; // set the initial enemy direction
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyGoesRight)
        {
            transform.position += speed * Time.deltaTime * Vector3.right;
        }
        else
        {
            transform.position += speed * Time.deltaTime * Vector3.left;
        }

        checkPatrolArea();
    }

    private void checkPatrolArea()
    {
        if (transform.position.x <= patrolStart.transform.position.x)
        {
            enemyGoesRight = true;
        }
        else if (transform.position.x >= patrolEnd.transform.position.x)
        {
            enemyGoesRight = false;
        }
    }

}
