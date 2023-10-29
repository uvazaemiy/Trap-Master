using UnityEngine;
using UnityEngine.Serialization;

public class BuildingsGrid : MonoBehaviour
{
    public static BuildingsGrid instance;

    public Vector2Int GridSize = new Vector2Int(10, 10);

    private Building[,] grid;
    private Building flyingBuilding;
    private Camera mainCamera;
    private CustomButton currentButton;
    
    private void Awake()
    {
        instance = this;
        
        grid = new Building[GridSize.x, GridSize.y];
        
        mainCamera = Camera.main;
    }

    public void StartPlacingBuilding(CustomButton button)
    {
        currentButton = button;
        if (currentButton.count != 0)
        {
            if (GameManager.instance.allowMoving && flyingBuilding == null)
            {
                GameManager.instance.GridCanvas.SetActive(true);
                SoundController.instance.PlayClick1();
                
                flyingBuilding = Instantiate(currentButton.buildingPrefab);
                flyingBuilding.button = button;
                
                currentButton.count--;
                currentButton.countText.text = currentButton.count.ToString();
            }
        }
    }

    public void MovePlacingBuilding(Building currentBuilding)
    {
        if (GameManager.instance.allowMoving && flyingBuilding == null)
        {
            GameManager.instance.GridCanvas.SetActive(true);
            SoundController.instance.PlayClick1();

            flyingBuilding = currentBuilding;
            currentButton = currentBuilding.button;
        }
    }

    private void Update()
    {
        if (flyingBuilding != null)
        {
            if (flyingBuilding.Obstacle != null)
                flyingBuilding.Obstacle.enabled = false;
            
            var groundPlane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (groundPlane.Raycast(ray, out float position))
            {
                Vector3 worldPosition = ray.GetPoint(position);

                int x = Mathf.RoundToInt(worldPosition.x);
                int y = Mathf.RoundToInt(worldPosition.z);

                bool available = true;

                if (x < 0 || x > GridSize.x - flyingBuilding.Size.x) available = false;
                if (y < 0 || y > GridSize.y - flyingBuilding.Size.y) available = false;

                if (available && (IsPlaceTaken(x, y) || flyingBuilding.isCollided)) available = false;

                Vector3 offset = new Vector3(0, 0, -flyingBuilding.transform.localScale.z + 1);
                flyingBuilding.transform.position = new Vector3(x, 0, y) + offset;
                flyingBuilding.SetTransparent(available);

                if (Input.GetMouseButtonUp(0))
                {
                    if (available)
                        PlaceFlyingBuilding(x, y);
                    else
                    {
                        if (flyingBuilding.tag == "LandMine")
                            foreach (Mine mine in flyingBuilding.GetComponent<Mine>().Mines)
                                mine.Mines.Remove(flyingBuilding.GetComponent<Mine>());
                        
                        Destroy(flyingBuilding.gameObject);
                        flyingBuilding = null;
                        currentButton.count++;
                        currentButton.countText.text = currentButton.count.ToString();
                    }
                    SoundController.instance.PlayClick2();
                    
                    GameManager.instance.GridCanvas.SetActive(false);
                }
            }
        }
    }

    private bool IsPlaceTaken(int placeX, int placeY)
    {
        for (int x = 0; x < flyingBuilding.Size.x; x++)
        {
            for (int y = 0; y < flyingBuilding.Size.y; y++)
            {
                if (grid[placeX + x, placeY + y] != null) return true;
            }
        }

        return false;
    }

    private void PlaceFlyingBuilding(int placeX, int placeY)
    {
        for (int x = 0; x < flyingBuilding.Size.x; x++)
        {
            for (int y = 0; y < flyingBuilding.Size.y; y++)
            {
                grid[placeX + x, placeY + y] = flyingBuilding;
            }
        }

        flyingBuilding.x = placeX;
        flyingBuilding.y = placeY;
        flyingBuilding.SetNormal();
        
        if (flyingBuilding.Obstacle != null)
            flyingBuilding.Obstacle.enabled = true;
        flyingBuilding = null;
        
        if (PlayerPrefs.GetInt("Tutorial") == 0)
            PlayerPrefs.SetInt("Tutorial", 1);
    }
}
