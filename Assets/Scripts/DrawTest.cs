
using UnityEngine;

public class DrawTest : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            int dir = Random.Range(1, 5);
            PlayerManager.Instance.GetPlayerByIndex(1).DrawPathDebug((GridSystem.Directions)dir);
        }
    }
}
