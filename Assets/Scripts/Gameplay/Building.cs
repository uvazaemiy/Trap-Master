using UnityEngine;
using UnityEngine.AI;

public class Building : MonoBehaviour
{
    public Renderer MainRenderer;
    public Vector2Int Size = Vector2Int.one;
    public int x, y;
    public bool isCollided;
    public CustomButton button;
    public NavMeshObstacle Obstacle;

    public Color savedColor;

    public void SetTransparent(bool available)
    {
        if (available)
        {
            MainRenderer.material.color = Color.green;
        }
        else
        {
            MainRenderer.material.color = Color.red;
        }
    }

    public void SetNormal()
    {
        MainRenderer.material.color = savedColor;
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.tag != "Plane")
            isCollided = true;
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag != "Plane")
            isCollided = false;
    }

    private void OnDrawGizmos()
    {
        for (int x = 0; x < Size.x; x++)
        {
            for (int y = 0; y < Size.y; y++)
            {
                if ((x + y) % 2 == 0) Gizmos.color = new Color(0.88f, 0f, 1f, 0.3f);
                else Gizmos.color = new Color(1f, 0.68f, 0f, 0.3f);

                Gizmos.DrawCube(transform.position + new Vector3(x, 0, y), new Vector3(1, .1f, 1));
            }
        }
    }

    private void OnMouseDown()
    {
        BuildingsGrid.instance.MovePlacingBuilding(this);
    }
}