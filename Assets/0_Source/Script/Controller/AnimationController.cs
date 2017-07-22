﻿using System.Collections;
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

        if(needs[NeedType.ENERGY] < Evaluation.BAD) {
            //Play sleepy Idle
            _animation.Play("Idle_Yawn");
        } else if(needs[NeedType.HEALTH] < Evaluation.BAD) {
            //Play sneeze Idle
            _animation.Play("Idle_Sneeze");
        } else if (needs[NeedType.HUNGER] < Evaluation.BAD) {
            //Play tongue Idle
            _animation.Play("Idle_Tongue");
        } else if (needs[NeedType.SATISFACTION] < Evaluation.BAD){
            //TODO: Play sad Idle
            _animation.Play("Idle_Tongue");
        } else if (needs[NeedType.SOCIAL] < Evaluation.BAD){
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

        yield return new WaitForSeconds(7);

        IsAnimating = false;
    }

    public void StartSequence()
    {
        _animation.Play("WakeUp");
        //_animation.SetTrigger("WakeUp");
    }

    public void GameOver()
    {
        _animation.Play("Death");
    }
}
