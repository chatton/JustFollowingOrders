using System;
using UnityEngine;

namespace World
{
    public class Tile : MonoBehaviour
    {
        [SerializeField] private GameObject highlightTile;

        private TextMesh _3dText;

        public bool IsWalkable = true;

        public bool IsEmpty
        {
            get
            {
                if (Physics.Raycast(transform.position, transform.up, out RaycastHit hit, 3f,
                    LayerMask.GetMask("Attackable")))
                {
                    return false;
                }

                Debug.Log("IsEmpty=" + IsWalkable);

                return IsWalkable;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, transform.up * 3f);
        }

        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;
            _3dText = highlightTile.GetComponentInChildren<TextMesh>();
        }

        public void LightUp(int roundNum = 1)
        {
            highlightTile.SetActive(true);
            if (_3dText != null)
            {
                _3dText.text = roundNum.ToString();
                // _3dText.transform.LookAt(_camera.transform);
                // snap to looking towards camera at 90 degrees
                // _3dText.transform.eulerAngles = new Vector3(transform.eulerAngles.x, (Mathf.Round(_camera.transform.eulerAngles.y / 90) * 90), transform.eulerAngles.z);
                _3dText.transform.eulerAngles = new Vector3(transform.eulerAngles.x, _camera.transform.eulerAngles.y,
                    transform.eulerAngles.z);
            }
        }

        public void Unlight()
        {
            highlightTile.SetActive(false);
        }
    }
}