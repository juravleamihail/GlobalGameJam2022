using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

namespace States
{
    public class CombatState : StateBase
    {
        private NinjaManager _nm;
        private GameManager _gm;

        private List<Ninja> _player0Ninjas;
        private List<Ninja> _player1Ninjas;
        private Action<Vector3> _onCombatCameraChangePos;
        public CombatState(Action<Vector3> onCombatCameraChangePos, EStates state, Action onCompleted) : base(state, onCompleted)
        {
            _onCombatCameraChangePos = onCombatCameraChangePos;
            _nm = NinjaManager.Instance;
            _gm = GameManager.Instance;
        }

        public override void OnEnter()
        {
            Debug.Log($"Enter state: {_state}");
            TryGetPlayers();
            _gm.StartCoroutine(TryTriggerCombat());
        }

        private void TryGetPlayers()
        {
            _player0Ninjas = _nm.GetAllNinjaForPlayer(0);
            _player1Ninjas = _nm.GetAllNinjaForPlayer(1);
        }

        private IEnumerator TryTriggerCombat()
        {
            for (var i = 0; i < _player0Ninjas.Count; i++)
            {
                for (var j = 0; j < _player1Ninjas.Count; j++)
                {
                    if (_player0Ninjas[i].GetPositionOnGrid() == _player1Ninjas[j].GetPositionOnGrid())
                    {
                        yield return TriggerCombatCoroutine(_player0Ninjas[i],_player1Ninjas[j]);
                    }
                }
            }

            //ComplateState();
        }
        private IEnumerator TriggerCombatCoroutine(Ninja n1, Ninja n2)
        {
            var tilePos = n1.GetPositionOnGrid();
            var transform = _gm.GetTileObjectAt((uint)tilePos.x, (uint)tilePos.y);
            _onCombatCameraChangePos?.Invoke(transform.position);

            var res = _setCameras?.Invoke(_state);
            float transitionTimer = res == null ? 0 : (float) res;
            
            yield return new WaitForSeconds(transitionTimer);

            var ph = transform.gameObject.GetComponent<PlayerHolder>();
            
            if (ph.PlayerType.PlayerIndex == n1.GetPlayerIndex()) 
            {
                n1.KillEnemy(n2);
            }
            else
            {
                n2.KillEnemy(n1);
            }
        }

        public class Vector2IntComparer : IEqualityComparer<Vector2Int>{
 
            public bool Equals(Vector2Int vec1, Vector2Int vec2) {
                return vec1.x == vec2.x && vec1.y == vec2.y;
            }
 
            public int GetHashCode (Vector2Int vec){
                return Mathf.FloorToInt (vec.x) ^ Mathf.FloorToInt (vec.y) << 2;
            }
        }
    }
}