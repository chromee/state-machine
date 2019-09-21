using HC.AI;
using UnityEngine;
using UnityEngine.UI;

public class SampleStateMachine : StateMachine
{
    [SerializeField] private Animator _animator = null;

    [SerializeField] private Text _tutorialText = null;

    public override void RegisterStates()
    {
        Register(new IdleState(this, _animator, _tutorialText));
        Register(new AppealState(this, _animator, _tutorialText));
        Register(new RunState(this, _animator, _tutorialText));
    }
}