using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ConditionViewController : AbstractViewController {

    private GridLayoutGroup _grid;
    
    private Dictionary<NeedType, RadialSliderViewController> _sliders;


    public ConditionViewController(Transform parent, Personality personality)
    {
        Rect = CreateContainer("ConditionMonitor", parent,
            new Vector2(20, -20), new Vector2(128, 720),
            new Vector2(0f, 1), new Vector2(0f, 1), new Vector2(0f, 1));
        View = Rect.gameObject;

        //AddImage(Rect, null, GraphicsHelper.Instance.ContainerColor);

        _grid = View.AddComponent<GridLayoutGroup>();
        _grid.cellSize = new Vector2(128, 128);
        _grid.spacing = new Vector2(20, 20);

        _sliders = new Dictionary<NeedType, RadialSliderViewController>();

        foreach (KeyValuePair<NeedType, Need> kvp in personality.Conditions)
        {
            Sprite s = null;
            switch(kvp.Key)
            {
                case NeedType.ENERGY:
                    s = GraphicsHelper.Instance.iconEnergy;
                    break;
                case NeedType.HEALTH:
                    s = GraphicsHelper.Instance.iconHealth;
                    break;
                case NeedType.HUNGER:
                    s = GraphicsHelper.Instance.iconHunger;
                    break;
                case NeedType.SATISFACTION:
                    s = GraphicsHelper.Instance.iconSatisfaction;
                    break;
                case NeedType.SOCIAL:
                    s = GraphicsHelper.Instance.iconSocial;
                    break;
            }

            RadialSliderViewController slider = new RadialSliderViewController(
                CreateContainer("Slider_" + kvp.Key, Rect,
                                Vector2.zero, _grid.cellSize,
                                Vector2.zero, Vector2.zero, Vector2.zero), s);

            _sliders[kvp.Key] = slider;
        }

        UpdateSlider(personality);
    }

    public void UpdateSlider(Personality personality)
    {
        foreach(KeyValuePair<NeedType, RadialSliderViewController> slider in _sliders)
        {
            Need c = personality.GetCondition(slider.Key);

            if (c != null)
            {
                slider.Value.UpdateSlider(c.GetSliderValue(), GraphicsHelper.Instance.evaluationColor[(int)c.getEvaluation()]);
            }
        }
    }
}
