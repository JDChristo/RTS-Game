using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

using RTS.Inputs;
using UnityEngine.InputSystem;

namespace RTS.Cameras
{
    public class CameraController : NetworkBehaviour
    {
        [SerializeField] private Transform playerCameraTransform = null;
        [SerializeField] private float speed = 20f;
        [SerializeField] private float screenBroderThick = 10f;
        [SerializeField] private Vector2 screenXLimits = Vector2.zero;
        [SerializeField] private Vector2 screenYLimits = Vector2.zero;

        private Controls controls;
        private Vector2 previousInput;

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();
            playerCameraTransform.gameObject.SetActive(true);

            controls = new Controls();

            controls.Player.MoveCamera.performed += SetPreviousInput;
            controls.Player.MoveCamera.canceled += SetPreviousInput;

            controls.Enable();
        }

        [ClientCallback]
        private void Update()
        {
            if (!hasAuthority || !Application.isFocused) { return; }

            UpdateCameraPosition();
        }

        private void UpdateCameraPosition()
        {
            Vector3 pos = playerCameraTransform.position;

            if (previousInput == Vector2.zero)
            {
                Vector3 cursorMovement = Vector3.zero;

                Vector2 cursorPosition = Mouse.current.position.ReadValue();

                if (cursorPosition.y >= Screen.height - screenBroderThick)
                {
                    cursorMovement.z += 1;
                }
                else if (cursorPosition.y <= screenBroderThick)
                {
                    cursorMovement.z -= 1;
                }

                if (cursorPosition.x >= Screen.width - screenBroderThick)
                {
                    cursorMovement.x += 1;
                }
                else if (cursorPosition.x <= screenBroderThick)
                {
                    cursorMovement.x -= 1;
                }

                pos += cursorMovement.normalized * speed * Time.deltaTime;
            }
            else
            {
                pos += new Vector3(previousInput.x, 0f, previousInput.y) * speed * Time.deltaTime;
            }

            pos.x = Mathf.Clamp(pos.x, screenXLimits.x, screenXLimits.y);
            pos.z = Mathf.Clamp(pos.z, screenYLimits.x, screenYLimits.y);

            playerCameraTransform.position = pos;
        }
        private void SetPreviousInput(InputAction.CallbackContext ctx)
        {
            previousInput = ctx.ReadValue<Vector2>();
        }
    }
}
