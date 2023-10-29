using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    public List<Mine> Mines;
    public GameObject child;
    public MeshRenderer mesh;
    [SerializeField] private ParticleSystem[] allParticles;

    private void Start()
    {
        GameManager.instance.allMines.Add(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "LandMine")
            Mines.Add(other.GetComponent<Mine>());
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "LandMine")
            Mines.Remove(other.GetComponent<Mine>());
    }

    private void OnDestroy()
    {
        GameManager.instance.allMines.Remove(this);
    }

    public void Explode()
    {
        mesh.enabled = false;
        foreach (ParticleSystem particle in allParticles)
            particle.Play();
        
        SoundController.instance.PlayBombSound();
    }
}
