using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class ReaperController : MonoBehaviour
{
    public int lifes = 50;
    public NavMeshAgent agent;
    public Rigidbody rb;
    public Animator animator;
    private Transform GirlPosition;
    private bool isInWall;
    [SerializeField] private Transform SliderPosition;
    public Slider slider;
    [SerializeField] private Image[] imagesOfSlider;
    [Range(0, 0.5f)]
    [SerializeField] private float timeOfLerp;

    public int changeCounts = 0; 
    
    private void Start()
    {
        GirlPosition = GameManager.instance.GirlPosition;
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        SliderPosition.position = transform.position;
    }

    public void MoveBot()
    {
        agent.SetDestination(GirlPosition.position);

        animator.SetBool("isRunning", true);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!GameManager.instance.allowMoving)
        {
            if (other.gameObject.tag == "Player")
                StartCoroutine(GameManager.instance.LoseFunc());
            else if (other.gameObject.tag == "LandMine")
            {
                StartCoroutine(ChangeSpeed());
                Mine Mine = other.gameObject.GetComponent<Mine>();
                foreach (Mine mine in Mine.Mines)
                {
                    lifes -= 25;
                    //mine.Explode();
                }
                Mine.mesh.enabled = false;
                Mine.Explode();

                lifes -= 50;
                StartCoroutine(ChangeSlider((float)lifes / 100));

                if (lifes <= 0)
                    StartCoroutine(GameManager.instance.WinFunc("Death", true));
            }
        }
    }
    
    public IEnumerator ChangeSpeed()
    {
        agent.speed /= 1.5f;
        yield return new WaitForSeconds(2);
        agent.speed *= 1.5f;
    }

    public void ActivateSlider()
    {
        slider.gameObject.SetActive(true);
        foreach (Image image in imagesOfSlider)
            image.DOFade(1, UIController.instance.time);
    }

    public IEnumerator ChangeSlider(float value)
    {
        slider.value = Mathf.Lerp(slider.value, value, timeOfLerp);
        yield return new WaitForEndOfFrame();
        if (slider.value > value + 0.001f && !GameManager.instance.allowMoving)
            StartCoroutine(ChangeSlider(value));

        if (slider.value == 0)
            StartCoroutine(DeactivateSlider());
    }

    private IEnumerator DeactivateSlider()
    {
        foreach (Image image in imagesOfSlider)
            image.DOFade(0, UIController.instance.time);
        yield return new WaitForSeconds(UIController.instance.time);
        slider.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Wall" && !GameManager.instance.allowMoving)
            StartCoroutine(CheckCollision(other.GetComponent<Wall>()));
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Wall" && !GameManager.instance.allowMoving)
            isInWall = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Wall" && !GameManager.instance.allowMoving)
            isInWall = false;
    }

    private IEnumerator CheckCollision(Wall wall)
    {
        changeCounts++;

        if (changeCounts < 3)
        {
            SoundController.instance.PlayObstacleSound();
            agent.isStopped = true;
            agent.ResetPath();
            animator.SetBool("isRunning", false);
            SoundController.instance.StopStepsSound();
            wall.child.layer = 0;
            //wallModifierVolume.enabled = true;
            GameManager.instance.DisableAllBlockers();
            GameManager.instance.NavMesh.BuildNavMesh();
            yield return new WaitForSeconds(1);
            animator.SetBool("isRunning", true);
            SoundController.instance.PlayStepsSound();
            MoveBot();
        }
        else
        {
            SoundController.instance.PlayObstacleSound();
            yield return new WaitForSeconds(1);
            StartCoroutine(GameManager.instance.WinFunc("isRunning", false));
        }
    }
}
