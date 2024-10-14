using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CortexBenchmark;
using UnityEngine;

public class EnemiesTaskManager : MonoBehaviour
{
    [SerializeField]
    private TaskController2D taskController;
    private DocManager docManager;
    private int enemiesNumber;

    void Start()
    {
        docManager = GameObject.FindGameObjectWithTag(DocConstants.DocTag).GetComponent<DocManager>();
        enemiesNumber = GameObject.FindGameObjectsWithTag(DocConstants.EnemyTag).Length;
    }

    void Update()
    {
        // if the player cannot move after the end of the task due to a bug, the task can be disabled pressing enter
        if (Input.GetKey(KeyCode.Return))
        {
            StartCoroutine(docManager.DisableBciTaskTrigger(DocConstants.BciActivator01Tag));
        }
    }

    public void CheckIfTaskComplete()
    {
        enemiesNumber--;

        if (enemiesNumber == 0)
        {
            taskController.StopTaskTimer();
            StartCoroutine(docManager.DisableBciTaskTrigger(DocConstants.BciActivator01Tag));
        }
    }
}
