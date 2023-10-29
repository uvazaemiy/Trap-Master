using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public NavMeshSurface NavMesh;
    public GameObject GridCanvas;
    [SerializeField] private GameObject[] WayBlockers;
    public Transform GirlPosition;
    public ReaperController Reaper;
    [SerializeField] private Animator GirlAnimator;

    public List<Mine> allMines;
    public List<Wall> allWalls;
    public bool allowMoving = true;
    private bool endGame;
    private bool winGame;
    
    private Vector3 reaperSpawnPosition;
    private Quaternion reaperRotation;
    private int reaperHealth;

    private void Awake()
    {
        Application.targetFrameRate = 120;
    }

    private void Start()
    {
        instance = this;
        
        
        int WayBlocker = 0;
        if (PlayerPrefs.GetInt("WayBlocker") != null)
            WayBlocker = PlayerPrefs.GetInt("WayBlocker");

        for (int i = 0; i < WayBlockers.Length; i++)
            if (i == WayBlocker)
                WayBlockers[i].gameObject.SetActive(false);
        
        NavMesh.BuildNavMesh();
        
        reaperSpawnPosition = Reaper.transform.position;
        reaperRotation = Reaper.transform.rotation;
        reaperHealth = Reaper.lifes;
    }

    public void StartGame()
    {
        if (PlayerPrefs.GetInt("Tutorial") == 1)
            PlayerPrefs.SetInt("Tutorial", 2);
        allowMoving = false;
        Reaper.MoveBot();
        StartCoroutine(UIController.instance.ChangeResetActivity(1));
        StartCoroutine(SoundController.instance.PlayStepsSound());
    }

    public IEnumerator LoseFunc()
    {
        SoundController.instance.StopStepsSound();
        StartCoroutine(UIController.instance.ChangeResetActivity(0));

        GirlAnimator.SetTrigger("Screaming");
        SoundController.instance.PlayScreamSound();
        Reaper.animator.SetBool("isRunning", false);
        yield return new WaitForSeconds(0.2f);
        Reaper.animator.SetTrigger("Punch");

        yield return new WaitForSeconds(0.5f);
        SoundController.instance.PlayPunchSound();
        SoundController.instance.PlayLoseSound();
        SoundController.instance.StopScreamSound();
        SoundController.instance.PlayShoutSound();
        GirlAnimator.SetBool("Death", true);
        
        yield return StartCoroutine(UIController.instance.EndLevel("Try again"));
        endGame = true;
    }

    public IEnumerator WinFunc(string animation, bool state)
    {
        SoundController.instance.StopStepsSound();
        StartCoroutine(UIController.instance.ChangeResetActivity(0));

        Reaper.agent.isStopped = true;
        Reaper.agent.ResetPath();
        Reaper.animator.SetBool(animation, state);
        
        GirlAnimator.SetTrigger("Clapping");
        SoundController.instance.PlayLaughSound();
        
        SoundController.instance.PlayWinSound();
        yield return StartCoroutine(UIController.instance.EndLevel("YOU WIN!"));
        endGame = true;
        winGame = true;
    }
    
    public void ResetGame()
    {
        endGame = false;
        SoundController.instance.StopStepsSound();
        StartCoroutine(UIController.instance.ChangeResetActivity(0));
        
        Reaper.animator.SetBool("Death", false);
        Reaper.animator.SetBool("isRunning", false);
        GirlAnimator.SetBool("Death", false);

        Reaper.changeCounts = 0;
        Reaper.agent.isStopped = true;
        Reaper.agent.ResetPath();
        Reaper.agent.enabled = false;
        Reaper.transform.position = reaperSpawnPosition;
        Reaper.transform.rotation = reaperRotation;
        Reaper.agent.enabled = true;
        Reaper.lifes = reaperHealth;
        Reaper.ActivateSlider();
        Reaper.slider.value = 1;

        ChangeWayBlocker();
        
        for (int i = 0; i < WayBlockers.Length; i++)
        {
            WayBlockers[i].gameObject.SetActive(true);
            if (i == PlayerPrefs.GetInt("WayBlocker"))
                WayBlockers[i].gameObject.SetActive(false);
        }

        foreach (Wall wall in allWalls)
        {
            wall.child.layer = 6;
        }
        
        NavMesh.BuildNavMesh();

        foreach (Mine mine in allMines)
            mine.mesh.enabled = true;
            
        allowMoving = true;
    }

    private void ChangeWayBlocker()
    {
        foreach (GameObject wayBlocker in WayBlockers)
            wayBlocker.SetActive(true);
        
        if (PlayerPrefs.GetInt("WayBlocker") == null)
            PlayerPrefs.SetInt("WayBlocker", 0);
        
        int WayBlocker = PlayerPrefs.GetInt("WayBlocker") + 1;
        if (WayBlocker >= WayBlockers.Length)
            WayBlocker = 0;
        
        PlayerPrefs.SetInt("WayBlocker", WayBlocker);
    }

    public void DisableAllBlockers()
    {
        foreach (GameObject wayBlocker in WayBlockers)
            wayBlocker.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && endGame)
            StartCoroutine(UIController.instance.Reset(winGame));
    }

    public void LoadNextLevel()
    {
        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        if (currentLevel == 5)
            currentLevel = -1;
        ChangeWayBlocker();
        DOTween.KillAll();
        PlayerPrefs.SetInt("SavedLevel", currentLevel + 1);
        SceneManager.LoadScene(currentLevel + 1);
    }
}
