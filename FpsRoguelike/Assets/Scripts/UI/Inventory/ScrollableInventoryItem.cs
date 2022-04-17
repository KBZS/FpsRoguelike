using UnityEngine;
using UnityEngine.UI;

public class ScrollableInventoryItem : ScrollableElement
{
    [SerializeField] private Text _name = null;
    [SerializeField] private RawImage _image = null;
    [SerializeField] private Text _count = null;
    [SerializeField] private RawImage _rarityIndicator = null;

    // TODO: private value of item type (Weapon, healing, etc. ...)

    // private ThisContent _content = new ThisContent(); <- something like this

    public override void SetContent(object content)
    {
        // _name.text = _content.name;
        // etc.
    }

    public override void OnClick(Vector3 position)
    {
        // ...
    }
}
