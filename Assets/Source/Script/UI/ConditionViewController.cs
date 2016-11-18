using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ConditionViewController : AbstractViewController {

    private GridLayoutGroup _grid;
    
    private Dictionary<NeedType, SliderViewController> _sliders;

	public ConditionViewController(Transform parent, Personality personality)
    {
        Rect = CreateContainer("ConditionMonitor", parent,
            new Vector2(0, 420), new Vector2(1000, 420),
            new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0));
        View = Rect.gameObject;

        AddImage(Rect, null, GraphicsHelper.Instance.ContainerColor);

        _grid = View.AddComponent<GridLayoutGroup>();
        _grid.cellSize = new Vector2(490, 128);
        _grid.spacing = new Vector2(20, 20);

        _sliders = new Dictionary<NeedType, SliderViewController>();

        foreach (KeyValuePair<NeedType, Need> kvp in personality.Conditions)
        {

            SliderViewController slider = new SliderViewController(
                CreateContainer("Slider_" + kvp.Key, Rect,
                                Vector2.zero, _grid.cellSize,
                                Vector2.zero, Vector2.zero, Vector2.zero), kvp.Key.ToString());

            _sliders[kvp.Key] = slider;
        }
        
    }

    public void UpdateSlider(Personality personality)
    {
        foreach(KeyValuePair<NeedType, SliderViewController> slider in _sliders)
        {
            Need c = personality.GetCondition(slider.Key);

            if(c != null)
                slider.Value.UpdateSlider(c.GetSliderValue());
        }
    }
}
