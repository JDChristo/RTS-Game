using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

using Mirror;
namespace RTS.Player
{
    public class UnitSelection : MonoBehaviour
    {
        [SerializeField]
        private LayerMask layerMask;
        [SerializeField]
        private RectTransform selectionBox;

        private Vector2 startPosition;
        private Camera mainCamera;
        private RTS_Player player;

        public List<Unit> SelectedUnits { get; } = new List<Unit>();

        private void Start()
        {
            mainCamera = Camera.main;
            player = NetworkClient.connection.identity.GetComponent<RTS_Player>();
            selectionBox.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (player == null)
            {
                player = NetworkClient.connection.identity.GetComponent<RTS_Player>();
            }

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                StartSelectionArea();
            }
            else if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                ClearSelectionArea();
            }
            else if (Mouse.current.leftButton.isPressed)
            {
                UpdateSelectionArea();
            }

        }
        private void StartSelectionArea()
        {
            if (!Keyboard.current.leftShiftKey.isPressed)
            {
                DeselectUnits();
            }
            selectionBox.gameObject.SetActive(true);
            startPosition = Mouse.current.position.ReadValue();
            UpdateSelectionArea();
        }
        private void UpdateSelectionArea()
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            float areaWidth = mousePosition.x - startPosition.x;
            float areaHeight = mousePosition.y - startPosition.y;

            selectionBox.sizeDelta = new Vector2(Mathf.Abs(areaWidth), Mathf.Abs(areaHeight));
            selectionBox.anchoredPosition = startPosition + new Vector2(areaWidth / 2, areaHeight / 2);
        }
        private void DeselectUnits()
        {
            foreach (Unit selectedUnit in SelectedUnits)
            {
                selectedUnit.Deselect();
            }
            SelectedUnits.Clear();
        }
        private void ClearSelectionArea()
        {
            selectionBox.gameObject.SetActive(false);
            if (selectionBox.sizeDelta.magnitude == 0)
            {
                SelectSingleUnit();
                return;
            }
            SelectUnitInBox();
        }
        private void SelectUnitInBox()
        {
            Vector2 min = selectionBox.anchoredPosition - (selectionBox.sizeDelta / 2);
            Vector2 max = selectionBox.anchoredPosition + (selectionBox.sizeDelta / 2);

            foreach (Unit unit in player.MyUnits)
            {
                if (SelectedUnits.Contains(unit)) { continue; }

                Vector3 screenPosition = mainCamera.WorldToScreenPoint(unit.transform.position);
                if (screenPosition.x > min.x && screenPosition.x < max.x &&
                    screenPosition.y > min.y && screenPosition.y < max.y)
                {
                    SelectedUnits.Add(unit);
                    unit.Select();
                }
            }
        }
        private void SelectSingleUnit()
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) { return; }
            if (!hit.collider.TryGetComponent<Unit>(out Unit unit)) { return; }
            if (!unit.hasAuthority) { return; }

            SelectedUnits.Add(unit);

            foreach (Unit selectedUnit in SelectedUnits)
            {
                selectedUnit.Select();
            }
        }
    }
}
