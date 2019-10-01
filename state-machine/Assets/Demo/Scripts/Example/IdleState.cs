using UnityEngine;
using UnityEngine.UI;
using UniRx;
using HC.AI;

public class IdleState : State<int>
{
    private Animator _animator;
    private Text _tutorialText;

    /// <summary>
    /// アピールステートに遷移する時間
    /// </summary>
    public const float TRANSITION_TO_APPEAL_DURATION = 3f;

    public IdleState(Animator animator, Text tutorialText)
    {
        _animator = animator;
        _tutorialText = tutorialText;
    }

    public override void Initialize()
    {
        // 待機アニメーションを再生する
        BeginStream.Subscribe(_ => _animator.Play("Idle"));

        BeginStream.Subscribe(v => Debug.LogError(v));

        // チュートリアルの文言を変更する
        BeginStream.Subscribe(v => _tutorialText.text = $"左クリックで走行ステートに遷移します {v}");

        float counter = 0f;
        // n秒経過したらアピールステートに遷移する
        UpdateStream.Do(_ => counter += Time.deltaTime)
            .Do(count => Debug.LogError(count))
            .Where(count => counter > TRANSITION_TO_APPEAL_DURATION)
            .Subscribe(_ => StateMachine.Transition<AppealState>());

        // 左クリックされたら走行ステートに遷移する
        UpdateStream.Where(_ => Input.GetMouseButtonDown(0))
            .Subscribe(_ => StateMachine.Transition<RunState>());

        // ステート遷移を行うときカウンタをリセットする
        EndStream.Subscribe(_ => counter = 0f);
    }
}