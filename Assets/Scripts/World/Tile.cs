using System;
using UnityEngine;

namespace World
{
    public class Tile : MonoBehaviour
    {
        [SerializeField] private GameObject highlightTile;

        private TextMesh _3dText;

        public bool IsWalkable = true;

        private void Awake()
        {
            _3dText = highlightTile.GetComponentInChildren<TextMesh>();
        }

        public void LightUp(int roundNum = 1)
        {
            highlightTile.SetActive(true);
            _3dText.text = roundNum.ToString();
        }

        public void Unlight()
        {
            highlightTile.SetActive(false);
        }
    }
}