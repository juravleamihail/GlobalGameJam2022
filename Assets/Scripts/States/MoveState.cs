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
        
        private void OnTileChanged(NinjaMovementData movementData)
        {
            Debug.LogWarning($"[MoveState] P{movementData.PlayerId} - N{movementData.NinjaId}, TilePos {movementData.TilePos}");

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
            }
        }

        private bool CheckForSameTiles()
        {
            bool hasSameTiles = false;
            foreach (var kv0 in _player0Tiles)
            {
                foreach (var kv1 in _player1Tiles)
                {
                    if (kv0.Value == kv1.Value)
                    {
                        _ninjasInCombat.Add(new NinjasCombatData()
                        {
                            Player0CombatData = new NinjaMovementData()
                            {
                                NinjaId = kv0.Key,
                                TilePos = kv0.Value
                            },
                            Player1CombatData = new NinjaMovementData()
                            {
                                NinjaId = kv1.Key,
                                TilePos = kv1.Value
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

            ComplateState();
        }
    }
    
    public struct NinjasCombatData
    {
        public NinjaMovementData Player0CombatData;
        public NinjaMovementData Player1CombatData;
    }

    public struct NinjaMovementData
    {
        public int PlayerId;
        public int NinjaId;
        public Vector2Int TilePos;
    }
}