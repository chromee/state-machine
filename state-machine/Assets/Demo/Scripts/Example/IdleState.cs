using UnityEngine;
using UnityEngine.UI;
using UniRx;
using HC.AI;

public class IdleState : State
{
    /// <summary>
    /// アピールステートに遷移する時間
    /// </summary>
    public const float TRANSITION_TO_APPEAL_DURATION = 3f;

    public IdleState(StateMachine stateMachine, Animator animator, Text tutorialText) : base(stateMachine)
    {
        // 待機アニメーションを再生する
        BeginStream.Subscribe(_ => animator.Play("Idle"));

        // チュートリアルの文言を変更する
        BeginStream.Subscribe(_ => tutorialText.text = "左クリックで走行ステートに遷移します");
        BeginStream.Subscribe(_ => Debug.LogError("aaaaaaaaaaaaaaaaaaaaaaaa"));
        UpdateStream.Subscribe(_ => Debug.LogError("bbbbbbbbbbbbbbb"));
        float counter = 0f;

        // n秒経過したらアピールステートに遷移する
        UpdateStream.Do(_ => counter += Time.deltaTime)
            .Where(count => counter > TRANSITION_TO_APPEAL_DURATION)
            .Subscribe(_ => Transition<AppealState>());

        // 左クリックされたら走行ステートに遷移する
        UpdateStream.Where(_ => Input.GetMouseButtonDown(0))
            .Subscribe(_ => Transition<RunState>());

        // ステート遷移を行うときカウンタをリセットする
        EndStream.Subscribe(_ => counter = 0f);
    }
}