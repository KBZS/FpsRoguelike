using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class RebindSaveLoad : MonoBehaviour
{
    [SerializeField] private InputActionAsset _actions;

    void Awake()
    {
        string path = PathsHelper.REBINDS_PATH;
        if (File.Exists(path))
        {
            string rebinds = File.ReadAllText(path);
            if (!string.IsNullOrEmpty(rebinds))
                _actions.LoadBindingOverridesFromJson(rebinds);
        }
    }

    void OnDisable()
    {
        string rebinds = _actions.SaveBindingOverridesAsJson();
        File.WriteAllText(PathsHelper.REBINDS_PATH, rebinds);
    }
}
