using UnityEngine;
using UnityEngine.UI;
using UniRx;
using HC.AI;

public class RunState : State
{
    public RunState(StateMachine stateMachine, Animator animator, Text tutorialText) : base(stateMachine)
    {
        // 走行アニメーションを再生する
        BeginStream.Subscribe(_ => animator.Play("Run"));

        // チュートリアルの文言を変更する
        BeginStream.Subscribe(_ => tutorialText.text = "左クリックで待機ステートに遷移します");

        // 左クリックされたら待機ステートに遷移する
        UpdateStream.Where(_ => Input.GetMouseButtonDown(0))
            .Subscribe(_ => Transition<IdleState>());
    }
}