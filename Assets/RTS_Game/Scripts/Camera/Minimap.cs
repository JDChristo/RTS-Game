using System.Collections;
using System.Collections.Generic;
using Mirror;
using RTS.Player;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace RTS.Cameras
{
    public class Minimap : MonoBehaviour, IPointerDownHandler, IDragHandler
    {
        [SerializeField] private RectTransform minimapRect;
        [SerializeField] private Vector2 mapScale;
        [SerializeField] private float offset = -6;

        private Transform playerCameraTransform;

        private void Update()
        {
            if (playerCameraTransform != null) { return; }
            if (NetworkClient.connection.identity == null) { return; }

            playerCameraTransform = NetworkClient.connection.identity.GetComponent<RTS_Player>().CameraTransform;

        }
        public void OnDrag(PointerEventData eventData)
        {
            if (!Mouse.current.middleButton.isPressed) { return; }
            MoveCamera();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!Mouse.current.middleButton.isPressed) { return; }
            MoveCamera();
        }
        private void MoveCamera()
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(minimapRect, mousePos, null, out Vector2 localPoint)) { return; }

            Vector2 lerp = new Vector2(
                (localPoint.x - minimapRect.rect.x) / minimapRect.rect.width,
                (localPoint.y - minimapRect.rect.y) / minimapRect.rect.height);

            Vector3 newCameraPos = new Vector3(
                    Mathf.Lerp(-mapScale.x, mapScale.x, lerp.x),
                    playerCameraTransform.position.y,
                    Mathf.Lerp(-mapScale.y, mapScale.y, lerp.y));

            playerCameraTransform.position = newCameraPos + (Vector3.forward * offset);
        }

    }
}
