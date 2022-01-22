using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "GameSettings", order = 0)]
    public class GameSettingsSO : ScriptableObject
    {
        [SerializeField] private int _timer;
        [SerializeField] private float _tileSize;
        [SerializeField] private uint _gridSize;
     
        public int Timer => _timer;
        public float TileSize => _tileSize;
        public uint GridSize => _gridSize;
    }
}