
using UnityEngine;

public class DrawTest : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            int dir = Random.Range(1, 5);
            PlayerManager.Instance.GetPlayerByIndex(1).DEBUG_DrawPath((GridSystem.Directions)dir);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            NinjaManager.Instance.StartMovePhase();
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            PlayerManager.Instance.GetPlayerByIndex(1).DEBUG_UndoPath(false);
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            PlayerManager.Instance.GetPlayerByIndex(1).DEBUG_UndoPath(true);
        }

        if (Input.GetKeyDown(KeyCode.Delete))
        {
            PlayerManager.Instance.GetPlayerByIndex(1).DEBUG_SelectNinja(0);
        }

        if (Input.GetKeyDown(KeyCode.End))
        {
            PlayerManager.Instance.GetPlayerByIndex(1).DEBUG_SelectNinja(1);
        }

        if (Input.GetKeyDown(KeyCode.PageDown))
        {
            PlayerManager.Instance.GetPlayerByIndex(1).DEBUG_SelectNinja(2);
        }
    }
}
