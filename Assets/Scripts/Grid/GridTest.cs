using UnityEngine;

public class GridTest : MonoBehaviour
{
    //this class exists for testing purposes only, it will not be in the final game

    public Transform pawn;

    void Start()
    {
        //in actual in-game used clases, avoid calls to GameManager; use callbacks instead
        //TODO make separate movement class instead of using the game manager for these
        pawn.position = GameManager.Instance.ConvertGridCoordsToVector3(1, 1);
    }

    void Update()
    {
        GridSystem.Directions direction = GridSystem.Directions.None;

        if (Input.GetKeyDown(KeyCode.W))
        {
            direction = GridSystem.Directions.Up;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            direction = GridSystem.Directions.Down;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            direction = GridSystem.Directions.Left;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            direction = GridSystem.Directions.Right;
        }

        if (direction == GridSystem.Directions.None)
        {
            return;
        }
        
        Vector3 position = GameManager.Instance.MoveOneTileInWorld(pawn.position, direction);
        Vector2Int posOnGrid = GameManager.Instance.ConvertVector3CoordsToGrid(position.x, position.z);
        if (GameManager.Instance.IsOnGrid(posOnGrid))
        {
            pawn.position = position;
        }
    }
}
