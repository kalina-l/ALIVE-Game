using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverViewController : AbstractViewController {

    private Image _background;
    private Text _gameOverText;

    private Image _restartImage;
    private Text _restartText;

    private bool _restart = false;

	public GameOverViewController(Transform parent)
    {
        Rect = CreateContainer("GameOver", parent,
            Vector2.zero, new Vector2(1080, 1920),
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
        View = Rect.gameObject;

        _background = AddSprite(Rect, null, GraphicsHelper.Instance.SpriteColorBlackHidden);
        _background.raycastTarget = false;

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
    }

    public void GameOver()
    {
        ApplicationManager.Instance.StartCoroutine(GameOverRoutine());
    }

    private void Restart()
    {
        _restart = true;
    }

    private IEnumerator GameOverRoutine()
    {
        //TODO: play death animation

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
}
