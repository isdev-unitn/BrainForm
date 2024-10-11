using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;
using CortexBenchmark;
using Gtec.UnityInterface;
using System.Collections;
using Unity.VisualScripting;

static class EndGameConstants
{
    public const string EndGameTag = "EndGame";
}


public class EndGameManager : MonoBehaviour
{
    [SerializeField] private GameObject selectColorText;
    [SerializeField] private Sprite portalOnSprite;
    [SerializeField] private AudioSource successSound;
    [SerializeField] private AudioSource correctColorSound;
    [SerializeField] private AudioSource wrongColorSound;
    [SerializeField] private TaskController2D currentTaskController;
    [SerializeField] private float cooldown = 0.5f;
    private SpriteRenderer portalSpriteRenderer;
    private List<Color> colors;
    private Transform colorSequence;
    private DocManager docManager;
    private int currentColor;
    public bool isCoolingDown = false;
    //cooldown timer
    public float stopwatchTime = 0;
    private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
    private bool portalOn = false;
    private static Random rng = new Random();

    // Start is called before the first frame update
    void Start()
    {
        docManager = GameObject.FindGameObjectWithTag(DocConstants.DocTag).GetComponent<DocManager>();
        portalSpriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        // get gameobject containing the colors gameobjects sequence
        colorSequence = gameObject.transform.GetChild(0);

        colors = new List<Color>
        {
            new Color32(255, 0, 0, 255),    // red
            new Color32(0, 255, 0, 255),    // green
            new Color32(0, 0, 255, 255),    // blue
            new Color32(255, 150, 0, 255),  // orange
            new Color32(0, 150, 0, 255),    // dark green
            new Color32(255, 0, 255, 255),  // pink
            new Color32(0, 0, 0, 255),      // black
            new Color32(0, 255, 255, 255),  // light blue
            new Color32(130, 130, 130, 255),// gray
            new Color32(150, 0, 0, 255)     // dark red
        };

        currentColor = colorSequence.childCount - 1;

        CreatePortalColorSequence();
    }

    public void Update()
    {
        stopwatchTime = stopwatch.ElapsedMilliseconds;
    }

    private void ActivatePortal()
    {
        // change the sprite of the portal with the activated one
        Debug.Log("Portal activated");
        portalSpriteRenderer.sprite = portalOnSprite;
    }

    private void CreatePortalColorSequence()
    {
        // shuffle the colors list to get a random order
        colors = colors.OrderBy(_ => rng.Next()).ToList();

        for (int i = 0; i < colorSequence.childCount; i++)
        {
            colorSequence.GetChild(i).GetComponent<SpriteRenderer>().color = colors[i];
        }
    }

    public void ColorSelected([SerializeField] SpriteRenderer targetCenter)
    {
        if (!isCoolingDown && !portalOn)
        {
            isCoolingDown = true;
            stopwatch.Start();
            GameObject currentColorObject = colorSequence.GetChild(currentColor).gameObject;
            FlashObject2D flashObject = targetCenter.GetComponentInParent<FlashObject2D>();

            if (currentColorObject.GetComponent<SpriteRenderer>().color == targetCenter.color)
            {
                currentColorObject.SetActive(false);
                currentColor -= 1;
                currentTaskController.TargetHit(flashObject.ClassId);

                // condition to avoid playing the sound at the end of the sequence
                if (currentColor >= 0)
                {
                    correctColorSound.Play();
                }
                else  // if all colors have been selected
                {
                    selectColorText.SetActive(false);
                    ActivatePortal();
                    successSound.Play();
                    portalOn = true;

                    // disable trigger for bci action (with a delay for ux purpose)
                    StartCoroutine(docManager.DisableBciTaskTrigger(DocConstants.BciActivator02Tag));
                }
            }
            else if (currentColorObject.GetComponent<SpriteRenderer>().color != targetCenter.color)
            {
                currentTaskController.TargetMiss(flashObject.ClassId);
                wrongColorSound.Play();
            }
            StartCoroutine(DeactivateInputLock(cooldown));
        }
    }

    public void ColorDeselected()
    {
        
    }

    public IEnumerator DeactivateInputLock(float seconds)
    {

        yield return new WaitForSeconds(seconds);
        stopwatch.Stop();

        Debug.Log("Couroutine execution time: " + stopwatchTime);
        stopwatch.Reset();
        isCoolingDown = false;
    }
}
