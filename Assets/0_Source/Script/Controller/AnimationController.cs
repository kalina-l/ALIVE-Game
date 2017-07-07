using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController {

    private Animator _animation; 

    public bool IsAnimating { get; private set; }

    private float _animationTime;

	public AnimationController(GameObject lemo)
    {
        _animation = lemo.GetComponent<Animator>();
    }

    public void PlayActivityAnimation(Activity activity, Personality personality) {

        DebugController.Instance.Log("Play animation for " + activity.Name, DebugController.DebugType.Animation);

        switch (activity.Name) {
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
            default:
                PlayIdleAnimation(personality);
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
        _animationTime = 2.5f;

        if (isBall)
        {
            //TODO
        }
        else
        {
            //TODO: add fx
            _animation.Play("PlayCake");
        }

        ApplicationManager.Instance.StartCoroutine(AnimationRoutine());
    }

    private void PlayIdleAnimation(Personality personality) {

        _animationTime = 1.5f;

        if(personality.GetCondition(NeedType.ENERGY).getEvaluation() < Evaluation.BAD) {
            //Play sleepy Idle
            _animation.Play("Idle_Yawn");
        } else if(personality.GetCondition(NeedType.HEALTH).getEvaluation() < Evaluation.BAD) {
            //Play sneeze Idle
            _animation.Play("Idle_Sneeze");
        } else if (personality.GetCondition(NeedType.HUNGER).getEvaluation() < Evaluation.BAD) {
            //Play tongue Idle
            _animation.Play("Idle_Tongue");
        } else if (personality.GetCondition(NeedType.SATISFACTION).getEvaluation() < Evaluation.BAD){
            //TODO: Play sad Idle
            _animation.Play("Idle_Tongue");
        } else if (personality.GetCondition(NeedType.SOCIAL).getEvaluation() < Evaluation.BAD){
            //TODO: Play lonely Idle
            _animation.Play("Idle_Tongue");
        } else {
            //Play dance Idle
            _animation.Play("Idle_Dance");

            //TODO: play cry idle
        }

        ApplicationManager.Instance.StartCoroutine(AnimationRoutine());
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

        yield return new WaitForSeconds(5);

        IsAnimating = false;
    }
}
