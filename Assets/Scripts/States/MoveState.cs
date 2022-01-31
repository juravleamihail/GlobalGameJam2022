using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace States
{
    public class MoveState : StateBase
    {
        private int _remainingNinjasToMove;
        private Action<List<NinjasCombatData>> _onCombatTriggerCb;
        public MoveState(EStates state, Action onCompleted, Action<List<NinjasCombatData>> onCombatTriggerCb) : base(state, onCompleted)
        {
            _onCombatTriggerCb = onCombatTriggerCb;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            _remainingNinjasToMove = NinjaManager.Instance.StartMovePhase(OnMovementComplete, OnTileChanged);

            var pm = PlayerManager.Instance;
            pm.GetPlayerByIndex(0).onEnterMoveState?.Invoke();
            pm.GetPlayerByIndex(1).onEnterMoveState?.Invoke();

        }

        private static Dictionary<int, Vector2Int> _player0Tiles = new Dictionary<int, Vector2Int>();
        private static Dictionary<int, Vector2Int> _player1Tiles = new Dictionary<int, Vector2Int>();
        private List<NinjasCombatData> _ninjasInCombat = new List<NinjasCombatData>();
        
        private bool OnTileChanged(NinjaMovementData movementData)
        {
            if (movementData.PlayerId == 0)
            {
                AddNewTileToPlayerO(movementData);
            }
            else
            {
                AddNewTileToPlayer1(movementData);
            }

            if (CheckForSameTiles())
            {
                NinjaManager.Instance.ForceStopMovementPhase();
                _onCombatTriggerCb?.Invoke(_ninjasInCombat);
                return true;
            }

            return false;
        }

        private bool CheckForSameTiles()
        {
            bool hasSameTiles = false;
            foreach (var kv0 in _player0Tiles)
            {
                foreach (var kv1 in _player1Tiles)
                {
                    Vector3 p0posV3 = NinjaManager.Instance.GetNinja(0, kv0.Key).transform.position;
                    Vector2Int p0pos = GameManager.Instance.ConvertVector3CoordsToGrid(p0posV3.x, p0posV3.z);

                    Vector3 p1posV3 = NinjaManager.Instance.GetNinja(1, kv1.Key).transform.position;
                    Vector2Int p1pos = GameManager.Instance.ConvertVector3CoordsToGrid(p1posV3.x, p1posV3.z);

                    if (p0pos == p1pos)
                    {
                        _ninjasInCombat.Add(new NinjasCombatData()
                        {
                            Player0CombatData = new NinjaMovementData()
                            {
                                PlayerId = 0,
                                NinjaId = kv0.Key,
                                //TilePos = kv0.Value
                                TilePos = p0pos
                            },
                            Player1CombatData = new NinjaMovementData()
                            {
                                PlayerId = 1,
                                NinjaId = kv1.Key,
                                TilePos = p1pos
                                //TilePos = kv1.Value
                            }
                        });
                        hasSameTiles = true;
                    }
                }
            }

            return hasSameTiles;
        }
        
        private void AddNewTileToPlayerO(NinjaMovementData movementData)
        {
            if (!_player0Tiles.ContainsKey(movementData.NinjaId))
            {
                _player0Tiles.Add(movementData.NinjaId, movementData.TilePos);
                return;
            }

            _player0Tiles[movementData.NinjaId] = movementData.TilePos;
        }

        private void AddNewTileToPlayer1(NinjaMovementData movementData)
        {
            if (!_player1Tiles.ContainsKey(movementData.NinjaId))
            {
                _player1Tiles.Add(movementData.NinjaId, movementData.TilePos);
                return;
            }

            _player1Tiles[movementData.NinjaId] = movementData.TilePos;
        }

        private void OnMovementComplete()
        {
            --_remainingNinjasToMove;
            if (_remainingNinjasToMove > 0)
            {
                return;
            }

            CompleteWithDelay(3);
        }

        private async void CompleteWithDelay(float delay)
        {
            while (delay > 0)
            {
                delay -= Time.deltaTime;
                Task.Yield();
            }

            CompleteState();
        }
    }
   
    public struct NinjaMovementData
    {
        public int PlayerId;
        public int NinjaId;
        public Vector2Int TilePos;
    }
}