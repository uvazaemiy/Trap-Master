using UnityEngine;
using UnityEngine.UI;

public class CustomButton : MonoBehaviour
{
    public Building buildingPrefab;
    public int count;
    public Text countText;

    private void Start()
    {
        countText.text = count.ToString();
    }
    
    private void OnMouseDown()
    {
        BuildingsGrid.instance.StartPlacingBuilding(this);
    }
}
