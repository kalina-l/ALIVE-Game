using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController {

    private Animator _animation;

    public bool IsAnimating { get; private set; }

    private float _animationTime;
    private float _currentEmotion;

	public AnimationController(GameObject lemo)
    {
        _animation = lemo.GetComponent<Animator>();
    }

    public void SetEmotion(EmotionType type)
    {
        float emotion = 0f;

        switch (type)
        {
            case EmotionType.BAD:
                emotion = 0f;
                break;
            case EmotionType.NORMAL:
                emotion = 0.5f;
                break;
            case EmotionType.GOOD:
                emotion = 1f;
                break;
        }


        if (emotion != _currentEmotion)
        {
            ApplicationManager.Instance.StartCoroutine(EmotionRoutine(emotion));
        }
    }

    public void PlayActivityAnimation(string activityName, Dictionary<NeedType, Evaluation> needs) {

        DebugController.Instance.Log("Play animation for " + activityName, DebugController.DebugType.Animation);

        switch (activityName) {
            case "Sleep":
                ApplicationManager.Instance.StartCoroutine(SleepRoutine());
                break;
            case "Cake.Eat":
                EatAnimation(true);
                break;
            case "Cake.Play with":
                PlayAnimation(false);
                break;
            case "Ball.Eat":
                EatAnimation(false);
                break;
            case "Ball.Play with":
                PlayAnimation(true);
                break;
            default:
                PlayIdleAnimation(needs);
                break;
        }
    }

    private void EatAnimation(bool isGood)
    {
        

        if (isGood)
        {
            _animationTime = 1.5f;
            _animation.Play("Eat");
        } else
        {
            _animationTime = 0.5f;
            _animation.Play("EatBall");
        }

        ApplicationManager.Instance.StartCoroutine(AnimationRoutine());
    }

    private void PlayAnimation(bool isBall)
    {
        if (isBall)
        {
            _animationTime = 3.0f;
            _animation.Play("PlayBall");
        }
        else
        {
            _animationTime = 2.5f;
            _animation.Play("PlayCake");
        }

        ApplicationManager.Instance.StartCoroutine(AnimationRoutine());
    }

    private void PlayIdleAnimation(Dictionary<NeedType, Evaluation> needs) {

        _animationTime = 1.5f;

        List<string> idleAnims = new List<string>();

        if(needs[NeedType.ENERGY] < Evaluation.BAD) {
            //Play sleepy Idle
            idleAnims.Add("Idle_Yawn");
        }

        if (needs[NeedType.HEALTH] < Evaluation.BAD) {
            //Play sneeze Idle
            idleAnims.Add("Idle_Sneeze");
        }

        if (needs[NeedType.HUNGER] < Evaluation.BAD) {
            //Play tongue Idle
            idleAnims.Add("Idle_Tongue");
        }

        if (needs[NeedType.SATISFACTION] < Evaluation.BAD){
            //Play bored Idle
            idleAnims.Add("Idle_Bored");
        }

        if (needs[NeedType.SOCIAL] < Evaluation.BAD){
            //Play lonely Idle
            idleAnims.Add("Idle_Lonely");
        }

        if(idleAnims.Count == 0) {
            //Play dance Idle because everything is fine
            _animation.CrossFade("Idle_Dance", 0.25f);
        } else {
            _animation.CrossFade(idleAnims[(int)(idleAnims.Count * Random.value)], 0.25f);
        }

        ApplicationManager.Instance.StartCoroutine(AnimationRoutine());
    }

    private IEnumerator EmotionRoutine(float targetEmotion)
    {
        float timer = 0;
        float startEmotion = _currentEmotion;

        while(timer < 1)
        {
            timer += Time.deltaTime * 2;

            _currentEmotion = Mathf.Lerp(_currentEmotion, targetEmotion, timer);
            _animation.SetFloat("Emotion", _currentEmotion);

            yield return 0;
        }
    }

    private IEnumerator AnimationRoutine()
    {
        IsAnimating = true;

        yield return new WaitForSeconds(_animationTime);

        IsAnimating = false;
    }

    private IEnumerator SleepRoutine()
    {
        IsAnimating = true;

        _animation.SetTrigger("Sleep");

        yield return new WaitForSeconds(7);

        _animation.SetTrigger("WakeUp");

        yield return new WaitForSeconds(3);

        IsAnimating = false;
    }

    public void StartSequence()
    {
        _animation.Play("WakeUp");
        //_animation.SetTrigger("WakeUp");
    }

    public void GameOver()
    {
        _animation.CrossFade("Death", 0.25f);
    }
}
