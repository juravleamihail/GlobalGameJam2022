
using UnityEngine;

public class OldStyleInputs : Singleton<OldStyleInputs>
{
    public Player player0;
    public Player player1;
        
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            player0.OldStyleUp();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            player0.OldStyleDown();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            player0.OldStyleLeft();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            player0.OldStyleRight();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            player1.OldStyleUp();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            player1.OldStyleDown();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            player1.OldStyleLeft();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            player1.OldStyleRight();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            player0.OldStyleSelect(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            player0.OldStyleSelect(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            player0.OldStyleSelect(2);
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            player1.OldStyleSelect(0);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            player1.OldStyleSelect(1);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            player1.OldStyleSelect(2);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            player0.OldStyleUndo(false);
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            player1.OldStyleUndo(false);
        }
        //if (Input.GetKeyDown(KeyCode.M))
        //{
        //    NinjaManager.Instance.StartMovePhase();
        //}
    }
}
