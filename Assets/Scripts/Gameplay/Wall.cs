using UnityEngine;

public class Wall : MonoBehaviour
{
    public GameObject child;
    
    private void Start()
    {
        GameManager.instance.allWalls.Add(this);
    }

    private void OnDestroy()
    {
        GameManager.instance.allWalls.Remove(this);
    }
}
