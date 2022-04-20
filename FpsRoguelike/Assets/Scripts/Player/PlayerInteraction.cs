using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] Text _info;
    [SerializeField] PlayerInput _playerInput;
    private Transform _playerCamera;
    private Transform _hoveredInteractable;
    private IInteractable _interactable;
    private const float RAY_LENGTH = 2f;

    void Awake()
    {
        _playerCamera = Camera.main.transform;
    }

    void Update()
    {
        Physics.Raycast(_playerCamera.position, _playerCamera.forward, out RaycastHit hit, RAY_LENGTH);
        if (_hoveredInteractable != hit.transform)
        {
            _hoveredInteractable = hit.transform;
            _interactable = _hoveredInteractable?.GetComponent<IInteractable>();
            _info.text = _interactable?.Info ?? string.Empty;
        }

        if (PlayerInputController.GetFirePressedDown(_playerInput) && _interactable != null)
        {
            _interactable.DoAction();
        }
    }
}
