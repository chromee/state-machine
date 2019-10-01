using System;
using HC.AI;
using UnityEngine;
using UnityEngine.UI;

public class SampleStateMachine : StateMachine
{
    [SerializeField] private Animator _animator = null;

    [SerializeField] private Text _tutorialText = null;

    public override void RegisterStates()
    {
        Register(new IdleState(_animator, _tutorialText));
        Register(new AppealState(_animator, _tutorialText));
        Register(new RunState(_animator, _tutorialText));

    }
}