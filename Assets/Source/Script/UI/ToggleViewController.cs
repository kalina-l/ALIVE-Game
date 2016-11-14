using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ToggleViewController : AbstractViewController {

    private Button _buttonOn;
    private Button _buttonOff;
    private bool _isOn;

	public ToggleViewController(RectTransform container, string identifier, UnityEngine.Events.UnityAction callOn, UnityEngine.Events.UnityAction callOff)
    {
        Rect = container;
        View = Rect.gameObject;

        AddImage(container, null, GraphicsHelper.Instance.SpriteColorWhite);

        RectTransform buttonOnRect = CreateContainer("ButtonON", container, container.anchoredPosition, container.sizeDelta, container.anchorMin, container.anchorMax, container.pivot);
        AddImage(buttonOnRect, null, GraphicsHelper.Instance.ButtonColorOn);
        _buttonOn = CreateButton(buttonOnRect, callOn);
        _buttonOn.onClick.AddListener(delegate { Toggle(); });

        RectTransform buttonOffRect = CreateContainer("ButtonOFF", container, container.anchoredPosition, container.sizeDelta, container.anchorMin, container.anchorMax, container.pivot);
        AddImage(buttonOffRect, null, GraphicsHelper.Instance.ButtonColorOff);
        _buttonOff = CreateButton(buttonOffRect, callOff);
        _buttonOff.onClick.AddListener(delegate { Toggle(); });

        RectTransform textRect = CreateContainer("Title", container, container.anchoredPosition, container.sizeDelta, container.anchorMin, container.anchorMax, container.pivot);
        Text text = AddText(textRect, GraphicsHelper.Instance.UIFont, 36, TextAnchor.MiddleCenter);
        text.text = identifier;
        text.raycastTarget = false;

        Toggle();
    }

    public void Toggle()
    {
        _isOn = !_isOn;

        _buttonOn.gameObject.SetActive(_isOn);
        _buttonOff.gameObject.SetActive(!_isOn);
    }
}
