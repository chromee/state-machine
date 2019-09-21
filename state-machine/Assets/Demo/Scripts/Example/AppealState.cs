using UnityEngine;
using UnityEngine.UI;
using UniRx;
using HC.AI;
using HC.Extensions;

public class AppealState : State
{
    public AppealState(StateMachine stateMachine, Animator animator, Text tutorialText) : base(stateMachine)
    {
        // アピールアニメーションを再生する
        BeginStream.Subscribe(_ => animator.Play("Appeal"));

        // チュートリアルの文言を変更する
        BeginStream.Subscribe(_ => tutorialText.text =
            (int) IdleState.TRANSITION_TO_APPEAL_DURATION + "秒経過したのでアピールステートに遷移しました");

        // アピールアニメーションの再生が完了したら待機ステートに遷移する
        UpdateStream.Where(_ => animator.IsCompleted(Animator.StringToHash("Appeal")))
            .Subscribe(_ => Transition<IdleState>());

        // 左クリックされたら走行ステートに遷移する
        UpdateStream.Where(_ => Input.GetMouseButtonDown(0))
            .Subscribe(_ => Transition<RunState>());
    }
}