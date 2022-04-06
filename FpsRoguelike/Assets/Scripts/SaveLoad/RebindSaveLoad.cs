using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class RebindSaveLoad : MonoBehaviour
{
    [SerializeField] private InputActionAsset _actions;

    private const string REBINDS_NAME = "rebindsSave.json";

    void Awake()
    {
        string path = Path.Combine(Application.persistentDataPath, REBINDS_NAME);
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
        File.WriteAllText(Path.Combine(Application.persistentDataPath, REBINDS_NAME), rebinds);
    }
}
