using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GraphicsHelper : MonoBehaviour {

    public static GraphicsHelper Instance;

    public Font UIFont;

    public Color[] evaluationColor;

    public Sprite feedbackNegativeSprite;
    public Sprite feedbackNeutralSprite;
    public Sprite feedbackPositiveSprite;

    public Sprite sliderBackgroundSpirte;
    public Sprite sliderFillSprite;

    public Sprite radialSliderSprite;
    public Sprite iconEnergy;
    public Sprite iconHealth;
    public Sprite iconHunger;
    public Sprite iconSatisfaction;
    public Sprite iconSocial;

    public Color ButtonColorOn;
    public Color ButtonColorOff;
    public Color ContainerColor;
    public Color TextColor;
    public Color SpriteColorWhite;

    void Awake()
    {
        Instance = this;
    }

    
}
