using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UnityEngine.InputSystem.Samples.RebindUI
{
    public class RebindActionUI : MonoBehaviour
    {
        [SerializeField] private string m_BindingId;

        [Tooltip("Reference to action that is to be rebound from the UI.")]
        [SerializeField] private InputActionReference m_Action;

        [SerializeField] private InputBinding.DisplayStringOptions m_DisplayStringOptions;

        [Tooltip("Main 'start a binding process' button.")]
        [SerializeField] private Button m_BindingButton;

        [Tooltip("Button to reset a binding to default.")]
        [SerializeField] private Button m_ResetButton;

        [Tooltip("Text for the reset button.")]
        [SerializeField] private Text m_ResetText;

        [Tooltip("Text label that will receive the name of the action. Optional. Set to None to have the "
            + "rebind UI not show a label for the action.")]
        [SerializeField] private Text m_ActionLabel;

        [Tooltip("Text label that will receive the current, formatted binding string.")]
        [SerializeField] private Text m_BindingText;

        [Tooltip("Optional UI that will be shown while a rebind is in progress.")]
        [SerializeField] private GameObject m_RebindOverlay;

        [Tooltip("Optional Text that will be shown while a rebind is in progress.")]
        [SerializeField] private Text m_RebindOverlayText;

        [Tooltip("Optional text label that will be placed instead of the input action name (used with composites).")]
        [SerializeField] private string m_CustomBindingString;

        [Tooltip("Event that is triggered when the way the binding is display should be updated. This allows displaying "
            + "bindings in custom ways, e.g. using images instead of text.")]
        [SerializeField] private UpdateBindingUIEvent m_UpdateBindingUIEvent;

        [Tooltip("Event that is triggered when an interactive rebind is being initiated. This can be used, for example, "
            + "to implement custom UI behavior while a rebind is in progress. It can also be used to further "
            + "customize the rebind.")]
        [SerializeField] private InteractiveRebindEvent m_RebindStartEvent;

        [Tooltip("Event that is triggered when an interactive rebind is complete or has been aborted.")]
        [SerializeField] private InteractiveRebindEvent m_RebindStopEvent;

        [Serializable] public class UpdateBindingUIEvent : UnityEvent<RebindActionUI, string, string, string> { }
        [Serializable] public class InteractiveRebindEvent : UnityEvent<RebindActionUI, InputActionRebindingExtensions.RebindingOperation> { }

        private InputActionRebindingExtensions.RebindingOperation m_RebindOperation;
        private static List<RebindActionUI> s_RebindActionUIs;

        private const string MOUSE_PRESS_IGNORE = "<Mouse>/press";
        private const string MOUSE_POINTER_IGNORE = "<Mouse>/pointer";
        private const string MOUSE_LEFT_IGNORE = "<Mouse>/leftButton";
        private const string MOUSE_RIGHT_IGNORE = "<Mouse>/rightButton";
        private const string KEYBOARD_ESCAPE_IGNORE = "<Keyboard>/escape";

        void OnEnable()
        {
            s_RebindActionUIs ??= new List<RebindActionUI>();
            s_RebindActionUIs.Add(this);
            if (s_RebindActionUIs.Count == 1)
                InputSystem.onActionChange += OnActionChange;

            // TODO: localization
            m_RebindOverlayText.text = "Waiting for a key...";
            m_ResetText.text = "Reset";
        }

        void OnDisable()
        {
            m_RebindOperation?.Dispose();
            m_RebindOperation = null;

            s_RebindActionUIs.Remove(this);
            if (s_RebindActionUIs.Count == 0)
            {
                s_RebindActionUIs = null;
                InputSystem.onActionChange -= OnActionChange;
            }
        }

        bool ResolveActionAndBinding(out InputAction action, out int bindingIndex)
        {
            bindingIndex = -1;

            action = m_Action?.action;
            if (action == null)
                return false;

            if (string.IsNullOrEmpty(m_BindingId))
                return false;

            // Look up binding index.
            Guid bindingId = new Guid(m_BindingId);
            bindingIndex = action.bindings.IndexOf(x => x.id == bindingId);
            if (bindingIndex == -1)
            {
                Debug.LogError($"Cannot find binding with ID '{bindingId}' on '{action}'", this);
                return false;
            }

            return true;
        }

        void UpdateBindingDisplay()
        {
            string displayString = string.Empty;
            string deviceLayoutName = default(string);
            string controlPath = default(string);

            // Get display string from action.
            InputAction action = m_Action?.action;
            if (action != null)
            {
                int bindingIndex = action.bindings.IndexOf(x => x.id.ToString() == m_BindingId);
                if (bindingIndex != -1)
                    displayString = action.GetBindingDisplayString(bindingIndex, out deviceLayoutName, out controlPath, m_DisplayStringOptions);
            }

            // Set on label (if any).
            if (m_BindingText != null)
                m_BindingText.text = displayString;

            // Give listeners a chance to configure UI in response.
            m_UpdateBindingUIEvent?.Invoke(this, displayString, deviceLayoutName, controlPath);

            // Setting buttons
            m_BindingButton.onClick.RemoveAllListeners();
            m_BindingButton.onClick.AddListener(StartInteractiveRebind);
            m_ResetButton.onClick.RemoveAllListeners();
            m_ResetButton.onClick.AddListener(ResetToDefault);
        }

        void ResetToDefault()
        {
            if (!ResolveActionAndBinding(out var action, out var bindingIndex))
                return;

            if (action.bindings[bindingIndex].isComposite)
            {
                // It's a composite. Remove overrides from part bindings.
                for (int i = bindingIndex + 1; i < action.bindings.Count && action.bindings[i].isPartOfComposite; ++i)
                    action.RemoveBindingOverride(i);
            }
            else
                action.RemoveBindingOverride(bindingIndex);

            UpdateBindingDisplay();
        }

        void StartInteractiveRebind()
        {
            if (!ResolveActionAndBinding(out var action, out var bindingIndex))
                return;

            // If the binding is a composite, we need to rebind each part in turn.
            if (action.bindings[bindingIndex].isComposite)
            {
                int firstPartIndex = bindingIndex + 1;
                if (firstPartIndex < action.bindings.Count && action.bindings[firstPartIndex].isPartOfComposite)
                    PerformInteractiveRebind(action, firstPartIndex, allCompositeParts: true);
            }
            else
                PerformInteractiveRebind(action, bindingIndex);
        }

        void PerformInteractiveRebind(InputAction action, int bindingIndex, bool allCompositeParts = false)
        {
            m_RebindOperation?.Cancel(); // Will null out m_RebindOperation.

            // Before configuring, action should be disabled.
            action.Disable();

            // Configure the rebind.
            m_RebindOperation = action.PerformInteractiveRebinding(bindingIndex)
                .WithControlsExcluding(MOUSE_PRESS_IGNORE)
                .WithControlsExcluding(MOUSE_POINTER_IGNORE)
                .WithControlsExcluding(MOUSE_LEFT_IGNORE)
                .WithControlsExcluding(MOUSE_RIGHT_IGNORE)
                .WithControlsExcluding(KEYBOARD_ESCAPE_IGNORE)
                .OnCancel(
                    operation =>
                    {
                        // After rebinding is canceled -> enable action back again
                        action.Enable();
                        m_RebindStopEvent?.Invoke(this, operation);
                        m_RebindOverlay?.SetActive(false);
                        UpdateBindingDisplay();
                        CleanUp();
                    })
                .OnComplete(
                    operation =>
                    {
                        // After rebinding is completed -> enable action back again
                        action.Enable();
                        m_RebindOverlay?.SetActive(false);
                        m_RebindStopEvent?.Invoke(this, operation);

                        if (CheckDuplicates(action, bindingIndex, allCompositeParts))
                        {
                            action.RemoveBindingOverride(bindingIndex);
                            CleanUp();
                            PerformInteractiveRebind(action, bindingIndex, allCompositeParts);
                            return;
                        }

                        UpdateBindingDisplay();
                        CleanUp();

                        // If there's more composite parts we should bind, initiate a rebind
                        // for the next part.
                        if (allCompositeParts)
                        {
                            int nextBindingIndex = bindingIndex + 1;
                            if (nextBindingIndex < action.bindings.Count && action.bindings[nextBindingIndex].isPartOfComposite)
                                PerformInteractiveRebind(action, nextBindingIndex, true);
                        }
                    });

            // If it's a part binding, show the name of the part in the UI.
            string partName = default(string);
            if (action.bindings[bindingIndex].isPartOfComposite)
                partName = $"Binding '{action.bindings[bindingIndex].name}'. ";

            // Bring up rebind overlay, if we have one.
            m_RebindOverlay?.SetActive(true);

            // Give listeners a chance to act on the rebind starting.
            m_RebindStartEvent?.Invoke(this, m_RebindOperation);

            m_RebindOperation.Start();

            void CleanUp()
            {
                m_RebindOperation?.Dispose();
                m_RebindOperation = null;
            }
        }

        bool CheckDuplicates(InputAction action, int bindingIndex, bool allCompositeParts = false)
        {
            InputBinding newBinding = action.bindings[bindingIndex];
            byte counter = 0;
            foreach (InputBinding binding in action.actionMap.bindings)
            {
                if (binding.effectivePath == newBinding.effectivePath)
                {
                    ++counter;
                    if (counter == 2)
                        return true;
                }
            }

            if (allCompositeParts)
            {
                for (int i = 1; i < bindingIndex; ++i)
                {
                    if (action.bindings[i].effectivePath == newBinding.effectivePath)
                        return true;
                }
            }

            return false;
        }

        static void OnActionChange(object obj, InputActionChange change)
        {
            if (change != InputActionChange.BoundControlsChanged)
                return;

            InputAction action = obj as InputAction;
            InputActionMap actionMap = action?.actionMap ?? obj as InputActionMap;

#pragma warning disable UNT0007 // Null coalescing on Unity objects
            InputActionAsset actionAsset = actionMap?.asset ?? obj as InputActionAsset;
#pragma warning restore UNT0007 // Null coalescing on Unity objects

            for (var i = 0; i < s_RebindActionUIs.Count; ++i)
            {
                RebindActionUI component = s_RebindActionUIs[i];
                InputAction referencedAction = component.m_Action?.action;
                if (referencedAction == null)
                    continue;

                if (referencedAction == action ||
                    referencedAction.actionMap == actionMap ||
                    referencedAction.actionMap?.asset == actionAsset)
                    component.UpdateBindingDisplay();
            }
        }

        void Start() => Refresh();

#if UNITY_EDITOR
        protected void OnValidate() => Refresh();
#endif
        void Refresh()
        {
            UpdateActionLabel();
            UpdateBindingDisplay();
        }

        void UpdateActionLabel()
        {
            if (m_ActionLabel != null)
            {
                InputAction action = m_Action != null ? m_Action.action : null;
                m_ActionLabel.text = 
                    !string.IsNullOrEmpty(m_CustomBindingString) ? m_CustomBindingString : 
                    (action != null ? action.name : string.Empty); 
            }
        }

    }
}
