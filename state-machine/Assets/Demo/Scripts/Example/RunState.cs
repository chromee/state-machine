using UnityEngine;
using UnityEngine.UI;
using UniRx;
using HC.AI;

public class RunState : State
{
    private Animator _animator;
    private Text _tutorialText;
    
    public RunState(Animator animator, Text tutorialText)
    {
        _animator = animator;
        _tutorialText = tutorialText;
    }

    public override void Initialize()
    {
        // 走行アニメーションを再生する
        BeginStream.Subscribe(_ => _animator.Play("Run"));

        // チュートリアルの文言を変更する
        BeginStream.Subscribe(_ => _tutorialText.text = "左クリックで待機ステートに遷移します");

        // 左クリックされたら待機ステートに遷移する
        UpdateStream.Where(_ => Input.GetMouseButtonDown(0))
            .Subscribe(_ =>
            {
                Debug.LogError("transition ");
                StateMachine.Transition<IdleState, int>(10000);
            });
    }
}