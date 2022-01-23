using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHolderTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
       var holder =  GetComponent<TileToPlayerConnection>();

        var player = PlayerManager.Instance.GetPlayerByIndex(holder.PlayerType.PlayerIndex);

        Debug.Log(player.PlayerType.NameText);
    }
}
