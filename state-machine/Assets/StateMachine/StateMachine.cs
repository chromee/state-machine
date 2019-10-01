using UnityEngine;
using System;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;


namespace HC.AI
{
    /// <summary>
    /// ステートマシン
    /// </summary>
    public abstract class StateMachine : MonoBehaviour
    {
        #region variable

        [SerializeField, Tooltip("Auto start FSM via Unity Start() function")]
        public bool AutoStart = true;

        /// <summary>
        /// ステートのマップ
        /// </summary>
        private readonly Dictionary<Type, State> _stateMap = new Dictionary<Type, State>();

        /// <summary>
        /// 現在ステート
        /// </summary>
        private State _currentState = null;

        /// <summary>
        /// 遷移先ステート
        /// </summary>
        private Type _nextState = null;

        #endregion

        #region property

        public State CurrentState
        {
            get { return _currentState; }
        }

        #endregion

        #region event

        private void Awake()
        {
            RegisterStates();
        }

        private void Start()
        {
            foreach (var state in _stateMap.Values)
            {
                state.Initialize();
            }

            if (AutoStart)
            {
                StartFSM();
            }
        }

        #endregion

        #region method

        /// <summary>
        /// ステートマシンを開始する
        /// </summary>
        public void StartFSM()
        {
            if (_currentState == null)
            {
                Debug.LogError("Plese assign Current State via Hierachy or call SetFirstState");
            }

            _currentState.StateBegin();

            // 遷移先ステートが存在するならば遷移処理を行う
            this.LateUpdateAsObservable()
                .Where(_ => _nextState != null)
                .Subscribe(_ =>
                {
                    // 現在ステートを終了
                    _currentState.StateEnd();

                    // 次ステートを開始
                    _currentState = _stateMap[_nextState];
                    _currentState.StateBegin();

                    // 遷移先をnullで初期化
                    _nextState = null;
                });
        }

        /// <summary>
        /// 初期ステートの登録
        /// </summary>
        /// <param name="firstState">初期ステート</param>
        public void SetFirstState(State firstState)
        {
            _currentState = firstState;
        }

        /// <summary>
        /// ステートの登録
        /// </summary>
        public void Register(State state)
        {
            if (_stateMap.Count == 0) SetFirstState(state);
            _stateMap.Add(state.GetType(), state);
            state.StateMachine = this;
        }

        /// <summary>
        /// ステートの登録
        /// </summary>
        public abstract void RegisterStates();

        /// <summary>
        /// ステートの遷移予約
        /// </summary>
        public void Transition<T>() where T : State
        {
            _nextState = typeof(T);
        }

        public void Transition<TS, T1>(T1 val) where TS : State<T1>
        {
            _nextState = typeof(TS);
            // FIXME: アップキャストな～～～
            var state = _stateMap[_nextState] as State<T1>;
            state?.SetVal(val);
        }

        #endregion
    }
}