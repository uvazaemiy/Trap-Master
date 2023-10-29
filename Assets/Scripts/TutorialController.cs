using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private Material mineMat;
    [SerializeField] private Transform mine;
    [SerializeField] private Transform SpawnPosition;
    [SerializeField] private Transform EndPositon;
    [SerializeField] private Image buttonImage;

    private void Start()
    {
        if (PlayerPrefs.GetInt("Tutorlal") == 1)
            PlayerPrefs.SetInt("Tutorial", 0);

        if (PlayerPrefs.GetInt("Tutorial") == 0)
        {
            mine.gameObject.SetActive(true);
            buttonImage.color = new Color(1, 1, 1, 0);
            StartCoroutine(MoveMine());
        }
    }

    private IEnumerator MoveMine()
    {
        mineMat.DOFade(1, UIController.instance.time);
        yield return mine.DOMove(EndPositon.position, 2).WaitForCompletion();
        yield return mineMat.DOFade(0, UIController.instance.time).WaitForCompletion();
        mine.position = SpawnPosition.position;
        yield return new WaitForSeconds(2);
        if (PlayerPrefs.GetInt("Tutorial") == 0)
            StartCoroutine(MoveMine());
        else
            StartCoroutine(ScaleButton());
    }

    public IEnumerator ScaleButton()
    {
        mine.gameObject.SetActive(false);
        yield return buttonImage.DOColor(Color.green, 0.5f).WaitForCompletion();
        yield return buttonImage.DOColor(Color.white, 0.5f).WaitForCompletion();
        if (PlayerPrefs.GetInt("Tutorial") == 1)
            StartCoroutine(ScaleButton());
        else
            EndTutorial();
    }

    public void EndTutorial()
    {
        buttonImage.DOColor(Color.white, 0.5f);
    }
}
