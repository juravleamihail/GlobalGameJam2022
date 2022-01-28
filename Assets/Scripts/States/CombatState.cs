using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

namespace States
{
    public class CombatState : StateBase
    {
        private NinjaManager _nm;

        private List<Ninja> _player0Ninjas;
        private List<Ninja> _player1Ninjas;
        private Action<Vector3> _onCombatCameraChangePos;
        private List<NinjasCombatData> _ninjasInCombat;
        
        public CombatState(Action<Vector3> onCombatCameraChangePos, EStates state, Action onCompleted, List<NinjasCombatData> ninjasInCombat) : base(state, onCompleted)
        {
            _onCombatCameraChangePos = onCombatCameraChangePos;
            _ninjasInCombat = ninjasInCombat;
            _nm = NinjaManager.Instance;
        }

        public override void OnEnter()
        {
            Debug.Log($"Enter state: {_state}");
            
            if (_ninjasInCombat != null)
            {
                TriggerOnMoveOverlapCombat();
            }
            else
            {
                Init();
            }
        }

        private void TriggerOnMoveOverlapCombat()
        {
            _player0Ninjas = new List<Ninja>();
            _player1Ninjas = new List<Ninja>();
            
            _ninjasInCombat.ForEach((data) =>
            {
                _player0Ninjas.Add(_nm.GetNinja(data.Player0CombatData.PlayerId, data.Player0CombatData.NinjaId));
                _player1Ninjas.Add(_nm.GetNinja(data.Player1CombatData.PlayerId, data.Player1CombatData.NinjaId));
            });
            
            _nm.StartCoroutine(TryTriggerCombat());
        }

        private void Init()
        {
            TryGetPlayers();
            _nm.StartCoroutine(TryTriggerCombat());
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
                    if (_player0Ninjas[i].IsNinjaAlive&& _player1Ninjas[j].IsNinjaAlive &&
                        _player0Ninjas[i].GetGridPositionViaTransform() == _player1Ninjas[j].GetGridPositionViaTransform())
                    {
                        yield return TriggerCombatCoroutine(_player0Ninjas[i],_player1Ninjas[j]);
                    }
                }
            }

            if (_ninjasInCombat != null)
            {
                var res = _setCameras?.Invoke(EStates.Gameplay);
                float transitionTimer = res == null ? 0 : (float) res + 1;
                yield return new WaitForSeconds(transitionTimer);
            }
            
            ComplateState();
        }
        
        private IEnumerator TriggerCombatCoroutine(Ninja n1, Ninja n2)
        {
            var tilePos = n1.GetGridPositionViaTransform();
            var transform = GameManager.Instance.GetTileObjectAt((uint)tilePos.x, (uint)tilePos.y);
            _onCombatCameraChangePos?.Invoke(transform.position);

            var res = _setCameras?.Invoke(_state);
            float transitionTimer = res == null ? 0 : (float) res;

            n1.Reveal(true);
            n2.Reveal(true);

            while (transitionTimer > 0)
            {
                transitionTimer -= Time.deltaTime;
                yield return null;
            }

            var ph = transform.gameObject.GetComponent<TileToPlayerConnection>();
            
            if (ph.PlayerType.PlayerIndex == n1.GetPlayerIndex()) 
            {
                n1.KillEnemy(n2);
            }
            else
            {
                n2.KillEnemy(n1);
            }

            //should wait for animations and everything else
            float delayTimer = 3;
            while (delayTimer > 0)
            {
                delayTimer -= Time.deltaTime;
                yield return null;
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