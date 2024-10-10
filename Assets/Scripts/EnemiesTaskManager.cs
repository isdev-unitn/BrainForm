using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesTaskManager : MonoBehaviour
{
    private DocManager docManager;
    private int enemiesNumber;

    void Start()
    {
        docManager = GameObject.FindGameObjectWithTag(DocConstants.DocTag).GetComponent<DocManager>();
        enemiesNumber = GameObject.FindGameObjectsWithTag(DocConstants.EnemyTag).Length;
    }

    public void CheckIfTaskComplete()
    {
        enemiesNumber--;

        if (enemiesNumber == 0)
        {
            StartCoroutine(docManager.DisableBciTaskTrigger(DocConstants.BciActivator01Tag));
        }
    }
}
