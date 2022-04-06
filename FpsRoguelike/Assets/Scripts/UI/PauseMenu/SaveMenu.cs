using UnityEngine;
using UnityEngine.UI;

public class SaveMenu : MonoBehaviour
{
    [SerializeField] private Text _title;

    void OnEnable()
    {
        SetTexts();
        // TODO
    }

    void SetTexts()
    {
        // TODO
        _title.text = "Save files";
    }
}
