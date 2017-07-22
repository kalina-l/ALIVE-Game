using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverViewController : AbstractViewController {

    private Image _background;
    private Text _gameOverText;

    private Image _restartImage;
    private Text _restartText;

    private Image _logoImage;
    private Image _titleImage;

    private bool _restart = false;

    private AnimationController _animation;

    private bool _gameOver;

    public GameOverViewController(Transform parent, AnimationController animation)
    {
        _animation = animation;

        Rect = CreateContainer("GameOver", parent,
            Vector2.zero, new Vector2(0, 0),
            new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0.5f, 0.5f));
        View = Rect.gameObject;

        _background = AddSprite(Rect, null, GraphicsHelper.Instance.SpriteColorBlack);
        _background.raycastTarget = true;

        //GameOvertext
        _gameOverText = AddText(
                            CreateContainer("GameOverText", Rect, 
                            Vector2.zero, new Vector2(1080, 500),
                            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f)),
                            GraphicsHelper.Instance.UIFont,
                            80,
                            TextAnchor.UpperCenter);
        _gameOverText.color = GraphicsHelper.Instance.SpriteColorWhiteHidden;
        _gameOverText.raycastTarget = false;
        _gameOverText.text = "Game Over";

        //RestartButton
        _restartImage = AddSprite(
                            CreateContainer("RestartButton", Rect,
                            new Vector2(0, 350), new Vector2(320, 100),
                            new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0.5f)),
                            GraphicsHelper.Instance.outputFrameSprite,
                            GraphicsHelper.Instance.SpriteColorWhiteHidden);
        _restartImage.raycastTarget = false;

        _restartText = AddText(
                            CreateContainer("RestartText", _restartImage.rectTransform,
                            Vector2.zero, Vector2.zero,
                            new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0.5f, 0.5f)),
                            GraphicsHelper.Instance.UIFont,
                            50,
                            TextAnchor.MiddleCenter);
        _restartText.text = "Restart";
        _restartText.color = GraphicsHelper.Instance.SpriteColorWhiteHidden;
        _restartText.raycastTarget = false;

        CreateButton(_restartImage.rectTransform, delegate { Restart(); });


        //TitleScreen

        _titleImage = AddSprite(
                            CreateContainer("Title", Rect,
                            new Vector2(-126f, 102f), new Vector2(466, 258),
                            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f)),
                            GraphicsHelper.Instance.title,
                            GraphicsHelper.Instance.SpriteColorWhiteHidden);
        _titleImage.raycastTarget = false;

        _logoImage = AddSprite(
                            CreateContainer("Logo", Rect,
                            new Vector2(0f, 100f), new Vector2(325, 330),
                            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f)),
                            GraphicsHelper.Instance.logo,
                            GraphicsHelper.Instance.SpriteColorWhiteHidden);
        _logoImage.raycastTarget = false;
    }

    public void GameOver()
    {
        if (!_gameOver)
        {
            _gameOver = true;
            ApplicationManager.Instance.StartCoroutine(GameOverRoutine());
        }
    }

    private void Restart()
    {
        _restart = true;
    }

    private IEnumerator GameOverRoutine()
    {
        _animation.GameOver();

        _background.raycastTarget = true;

        Color blackground = new Color(0, 0, 0, 0.8f);

        float timer = 0;

        while(timer < 1)
        {
            timer += Time.deltaTime;

            _background.color = GraphicsHelper.Instance.LerpColor(GraphicsHelper.Instance.SpriteColorBlackHidden, blackground, timer);
            
            yield return 0;
        }

        timer = 0;

        while (timer < 1)
        {
            timer += Time.deltaTime;

            _gameOverText.color = GraphicsHelper.Instance.LerpColor(GraphicsHelper.Instance.SpriteColorWhiteHidden, GraphicsHelper.Instance.SpriteColorWhite, timer);

            yield return 0;
        }

        timer = 0;

        while (timer < 1)
        {
            timer += Time.deltaTime;

            _restartImage.color = GraphicsHelper.Instance.LerpColor(GraphicsHelper.Instance.SpriteColorWhiteHidden, GraphicsHelper.Instance.SpriteColorWhite, timer);
            _restartText.color = GraphicsHelper.Instance.LerpColor(GraphicsHelper.Instance.SpriteColorWhiteHidden, GraphicsHelper.Instance.SpriteColorWhite, timer);


            yield return 0;
        }

        _restartImage.raycastTarget = true;

        while (!_restart)
        {
            yield return 0;
        }

        _restartImage.raycastTarget = false;

        timer = 0;

        while (timer < 1)
        {
            timer += Time.deltaTime * 0.5f;

            _restartImage.color = GraphicsHelper.Instance.LerpColor(GraphicsHelper.Instance.SpriteColorWhite, GraphicsHelper.Instance.SpriteColorWhiteHidden, Mathf.Clamp(timer * 4, 0, 1));
            _restartText.color = GraphicsHelper.Instance.LerpColor(GraphicsHelper.Instance.SpriteColorWhite, GraphicsHelper.Instance.SpriteColorWhiteHidden, Mathf.Clamp(timer * 4, 0, 1));
            _gameOverText.color = GraphicsHelper.Instance.LerpColor(GraphicsHelper.Instance.SpriteColorWhite, GraphicsHelper.Instance.SpriteColorWhiteHidden, timer);
            _background.color = GraphicsHelper.Instance.LerpColor(blackground, GraphicsHelper.Instance.SpriteColorBlack, timer);

            yield return 0;
        }

        yield return new WaitForSeconds(1);

        ApplicationManager.Instance.reset();
    }

    public IEnumerator StartSequence()
    {
        _background.raycastTarget = true;
        Vector2 logoStartPos = new Vector2(0, 100);
        Vector2 logoEndPos = new Vector2(198, 100);

        GraphicsHelper gh = GraphicsHelper.Instance;

        float timer = 0;

        

        //1. logo appears
        while(timer < 1)
        {
            timer += Time.deltaTime * 0.5f;
            _logoImage.color = gh.LerpColor(gh.SpriteColorWhiteHidden, gh.SpriteColorWhite, timer);

            yield return 0;
        }

        timer = 0;

        //2. black to white fade + logo transforms into title

        while (timer < 1)
        {
            timer += Time.deltaTime * 4;
            _background.color = gh.LerpColor(gh.SpriteColorBlack, gh.SpriteColorWhite, timer);

            _logoImage.rectTransform.anchoredPosition = Vector2.Lerp(logoStartPos, logoEndPos, timer*0.6f);
            _titleImage.color = gh.LerpColor(gh.SpriteColorWhiteHidden, gh.SpriteColorWhite, timer*0.5f);

            yield return 0;
        }

        _animation.StartSequence();

        timer = 0;

        //3. white to transparent fade

        while (timer < 1)
        {
            timer += Time.deltaTime * 4;
            _background.color = gh.LerpColor(gh.SpriteColorWhite, gh.SpriteColorWhiteHidden, timer);

            _logoImage.rectTransform.anchoredPosition = Vector2.Lerp(logoStartPos, logoEndPos, 0.6f + (timer * 0.4f));
            _titleImage.color = gh.LerpColor(gh.SpriteColorWhiteHidden, gh.SpriteColorWhite, 0.5f + (timer * 0.5f));

            yield return 0;
        }

        yield return new WaitForSeconds(4);

        timer = 0;

        //4. title fades

        while (timer < 1)
        {
            timer += Time.deltaTime * 2;

            _logoImage.color = gh.LerpColor(gh.SpriteColorWhite, gh.SpriteColorWhiteHidden, timer);
            _titleImage.color = gh.LerpColor(gh.SpriteColorWhite, gh.SpriteColorWhiteHidden, timer);

            yield return 0;
        }

        _background.raycastTarget = false;
    }
}
