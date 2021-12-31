using System.Collections;
using System.Collections.Generic;
using RTS.Combat;
using UnityEngine;
using UnityEngine.InputSystem;

using RTS.Game;

namespace RTS.Player
{
    public class UnitCommander : MonoBehaviour
    {
        [SerializeField]
        private UnitSelection unitSelectionHandler = null;
        [SerializeField]
        private LayerMask layerMask;
        private Camera mainCamera;

        private void Start()
        {
            mainCamera = Camera.main;
            GameOverHandler.ClientOnGameOver += ClientGameOver;
        }
        private void OnDestroy()
        {
            GameOverHandler.ClientOnGameOver -= ClientGameOver;
        }
        private void Update()
        {
            if (!Mouse.current.rightButton.wasPressedThisFrame) { return; }

            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) { return; }

            if (hit.collider.TryGetComponent<Targetable>(out Targetable target))
            {
                if (!target.hasAuthority)
                {
                    ShootTarget(target);
                    return;
                }
            }
            MoveUnits(hit.point);
        }

        private void MoveUnits(Vector3 position)
        {
            foreach (var unit in unitSelectionHandler.SelectedUnits)
            {
                unit.UnitMovement.CmdMove(position);
            }
        }

        private void ShootTarget(Targetable target)
        {
            foreach (var unit in unitSelectionHandler.SelectedUnits)
            {
                unit.Targeter.CmdSetTarget(target.gameObject);
            }
        }
        private void ClientGameOver(string winner)
        {
            enabled = false;
        }
    }
}
