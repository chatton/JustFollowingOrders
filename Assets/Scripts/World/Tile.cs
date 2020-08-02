using System;
using UnityEngine;

namespace World
{
    public class Tile : MonoBehaviour
    {
        [SerializeField] private GameObject highlightTile;

        private TextMesh _3dText;

        public bool IsWalkable = true;
        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;
            _3dText = highlightTile.GetComponentInChildren<TextMesh>();
        }

        public void LightUp(int roundNum = 1)
        {
            highlightTile.SetActive(true);
            _3dText.text = roundNum.ToString();

            // snap to looking towards camera at 90 degrees
            // _3dText.transform.eulerAngles = new Vector3(transform.eulerAngles.x, (Mathf.Round(_camera.transform.eulerAngles.y / 90) * 90), transform.eulerAngles.z);
            _3dText.transform.eulerAngles = new Vector3(transform.eulerAngles.x, _camera.transform.eulerAngles.y,
                transform.eulerAngles.z);
        }

        public void Unlight()
        {
            highlightTile.SetActive(false);
        }
    }
}