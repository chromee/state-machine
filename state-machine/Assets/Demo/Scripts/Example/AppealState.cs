using UnityEngine;
using UnityEngine.UI;
using UniRx;
using HC.AI;
using HC.Extensions;

public class AppealState : StateBase
{
    private Animator _animator;
    private Text _tutorialText;

    public AppealState(Animator animator, Text tutorialText)
    {
        _animator = animator;
        _tutorialText = tutorialText;
    }

    public override void Initialize()
    {
        // アピールアニメーションを再生する
        BeginStream.Subscribe(_ => _animator.Play("Appeal"));

        // チュートリアルの文言を変更する
        BeginStream.Subscribe(_ => _tutorialText.text =
            (int) IdleState.TRANSITION_TO_APPEAL_DURATION + "秒経過したのでアピールステートに遷移しました");

        // アピールアニメーションの再生が完了したら待機ステートに遷移する
        UpdateStream.Where(_ => _animator.IsCompleted(Animator.StringToHash("Appeal")))
            .Subscribe(_ => StateMachine.Transition<IdleState, int>(100));

        // 左クリックされたら走行ステートに遷移する
        UpdateStream.Where(_ => Input.GetMouseButtonDown(0))
            .Subscribe(_ => StateMachine.Transition<RunState>());
    }
}