using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] private GameObject _parent;
    [SerializeField] private Text _title;
    [SerializeField] private Button _backButton;
    [SerializeField] private ListScroller _inventoryScroller;

    void OnEnable()
    {
        SetScroller();
        SetButtons();
        SetTexts();
    }

    void SetTexts()
    {
        _title.text = "Inventory";

        // ...
    }

    void SetButtons()
    {
        _backButton.onClick.RemoveAllListeners();
        _backButton.onClick.AddListener(GoBack);
    }

    void SetScroller()
    {
        // TODO: Get current inventory items
        // _inventoryScroller.SetData( ... inventory items ... );
    }

    void GoBack() => _parent.SetActive(false);

}
