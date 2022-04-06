#if UNITY_EDITOR
using System.Linq;
using UnityEditor;

namespace UnityEngine.InputSystem.Samples.RebindUI
{
    [CustomEditor(typeof(RebindActionUI))]
    public class RebindActionUIEditor : UnityEditor.Editor
    {
        private SerializedProperty m_ActionProperty;
        private SerializedProperty m_BindingIdProperty;
        private SerializedProperty m_BindingButtonProperty;
        private SerializedProperty m_ResetButtonProperty;
        private SerializedProperty m_ResetTextProperty;
        private SerializedProperty m_ActionLabelProperty;
        private SerializedProperty m_BindingTextProperty;
        private SerializedProperty m_RebindOverlayProperty;
        private SerializedProperty m_RebindOverlayTextProperty;
        private SerializedProperty m_CustomBindingStringProperty;
        private SerializedProperty m_RebindStartEventProperty;
        private SerializedProperty m_RebindStopEventProperty;
        private SerializedProperty m_UpdateBindingUIEventProperty;
        private SerializedProperty m_DisplayStringOptionsProperty;

        private GUIContent m_BindingLabel = new GUIContent("Binding");
        private GUIContent m_DisplayOptionsLabel = new GUIContent("Display Options");
        private GUIContent m_UILabel = new GUIContent("UI");
        private GUIContent m_EventsLabel = new GUIContent("Events");
        private GUIContent[] m_BindingOptions;
        private string[] m_BindingOptionValues;
        private int m_SelectedBindingOption;

        private static class Styles { public static GUIStyle boldLabel = new GUIStyle("MiniBoldLabel"); }

        protected void OnEnable()
        {
            m_ActionProperty = serializedObject.FindProperty("m_Action");
            m_BindingIdProperty = serializedObject.FindProperty("m_BindingId");
            m_BindingButtonProperty = serializedObject.FindProperty("m_BindingButton");
            m_ResetButtonProperty = serializedObject.FindProperty("m_ResetButton");
            m_ResetTextProperty = serializedObject.FindProperty("m_ResetText");
            m_ActionLabelProperty = serializedObject.FindProperty("m_ActionLabel");
            m_BindingTextProperty = serializedObject.FindProperty("m_BindingText");
            m_RebindOverlayProperty = serializedObject.FindProperty("m_RebindOverlay");
            m_RebindOverlayTextProperty = serializedObject.FindProperty("m_RebindOverlayText");
            m_CustomBindingStringProperty = serializedObject.FindProperty("m_CustomBindingString");
            m_UpdateBindingUIEventProperty = serializedObject.FindProperty("m_UpdateBindingUIEvent");
            m_RebindStartEventProperty = serializedObject.FindProperty("m_RebindStartEvent");
            m_RebindStopEventProperty = serializedObject.FindProperty("m_RebindStopEvent");
            m_DisplayStringOptionsProperty = serializedObject.FindProperty("m_DisplayStringOptions");

            RefreshBindingOptions();
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            // Binding section.
            EditorGUILayout.LabelField(m_BindingLabel, Styles.boldLabel);
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(m_ActionProperty);

                int newSelectedBinding = EditorGUILayout.Popup(m_BindingLabel, m_SelectedBindingOption, m_BindingOptions);
                if (newSelectedBinding != m_SelectedBindingOption)
                {
                    string bindingId = m_BindingOptionValues[newSelectedBinding];
                    m_BindingIdProperty.stringValue = bindingId;
                    m_SelectedBindingOption = newSelectedBinding;
                }

                InputBinding.DisplayStringOptions optionsOld = (InputBinding.DisplayStringOptions)m_DisplayStringOptionsProperty.intValue;
                InputBinding.DisplayStringOptions optionsNew = (InputBinding.DisplayStringOptions)EditorGUILayout.EnumFlagsField(m_DisplayOptionsLabel, optionsOld);
                if (optionsOld != optionsNew)
                    m_DisplayStringOptionsProperty.intValue = (int)optionsNew;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(m_UILabel, Styles.boldLabel);
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(m_BindingButtonProperty);
                EditorGUILayout.PropertyField(m_ResetButtonProperty);
                EditorGUILayout.PropertyField(m_ResetTextProperty);
                EditorGUILayout.PropertyField(m_ActionLabelProperty);
                EditorGUILayout.PropertyField(m_BindingTextProperty);
                EditorGUILayout.PropertyField(m_RebindOverlayProperty);
                EditorGUILayout.PropertyField(m_RebindOverlayTextProperty);
                EditorGUILayout.PropertyField(m_CustomBindingStringProperty);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(m_EventsLabel, Styles.boldLabel);
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(m_RebindStartEventProperty);
                EditorGUILayout.PropertyField(m_RebindStopEventProperty);
                EditorGUILayout.PropertyField(m_UpdateBindingUIEventProperty);
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                RefreshBindingOptions();
            }
        }

        protected void RefreshBindingOptions()
        {
            InputActionReference actionReference = (InputActionReference)m_ActionProperty.objectReferenceValue;
            InputAction action = actionReference?.action;

            if (action == null)
            {
                m_BindingOptions = new GUIContent[0];
                m_BindingOptionValues = new string[0];
                m_SelectedBindingOption = -1;
                return;
            }

            Utilities.ReadOnlyArray<InputBinding> bindings = action.bindings;
            int bindingCount = bindings.Count;

            m_BindingOptions = new GUIContent[bindingCount];
            m_BindingOptionValues = new string[bindingCount];
            m_SelectedBindingOption = -1;

            string currentBindingId = m_BindingIdProperty.stringValue;
            for (int i = 0; i < bindingCount; ++i)
            {
                InputBinding binding = bindings[i];
                string bindingId = binding.id.ToString();
                bool haveBindingGroups = !string.IsNullOrEmpty(binding.groups);

                // If we don't have a binding groups (control schemes), show the device that if there are, for example,
                // there are two bindings with the display string "A", the user can see that one is for the keyboard
                // and the other for the gamepad.
                InputBinding.DisplayStringOptions displayOptions =
                    InputBinding.DisplayStringOptions.DontUseShortDisplayNames | InputBinding.DisplayStringOptions.IgnoreBindingOverrides;
                if (!haveBindingGroups)
                    displayOptions |= InputBinding.DisplayStringOptions.DontOmitDevice;

                // Create display string.
                string displayString = action.GetBindingDisplayString(i, displayOptions);

                // If binding is part of a composite, include the part name.
                if (binding.isPartOfComposite)
                    displayString = $"{ObjectNames.NicifyVariableName(binding.name)}: {displayString}";

                // Some composites use '/' as a separator. When used in popup, this will lead to to submenus. Prevent
                // by instead using a backlash.
                displayString = displayString.Replace('/', '\\');

                // If the binding is part of control schemes, mention them.
                if (haveBindingGroups)
                {
                    InputActionAsset asset = action.actionMap?.asset;
                    if (asset != null)
                    {
                        string controlSchemes = string.Join(", ",
                            binding.groups.Split(InputBinding.Separator)
                                .Select(x => asset.controlSchemes.FirstOrDefault(c => c.bindingGroup == x).name));

                        displayString = $"{displayString} ({controlSchemes})";
                    }
                }

                m_BindingOptions[i] = new GUIContent(displayString);
                m_BindingOptionValues[i] = bindingId;

                if (currentBindingId == bindingId)
                    m_SelectedBindingOption = i;
            }
        }
    }
}
#endif
