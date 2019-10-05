using System;
using UniRx;
using UniRx.Triggers;
using HC.UniRxCustom;


/*
 * このStateクラスを継承して各種ステートを定義する
 * 
 * 
 * 各種処理はBegin,End,Update,LateUpdateのストリームを利用して記述する
 * 
 * ex)
 *      BeginStream.Subscribe(_=>Debug.Log("State begin"));
 *      EndStream.Subscribe(_=>Debug.Log("State end"));
 *      UpdateStream.Subscribe(_=>Debug.Log("State update"));
 *      ...
 *      
 * 他ステートへの遷移はTransition関数を使う
 * ex)
 *      UpdateStream.Where(_=>Input.GetMouseButtonDown(0))
 *                      .Subscribe(_=>Transition(typeof(OtherState)));
 *                      
 * ジェネリック版)
 *      UpdateStream.Where(_=>Input.GetMouseButtonDown(0))
 *                      .Subscribe(_=>Transition<OtherState>());
 */


namespace HC.AI
{
    /// <summary>
    /// ステートクラス
    /// </summary>
    public abstract class State<T> : IState
    {
        #region variable

        /// <summary>
        /// ステートマシン
        /// </summary>
        protected StateMachine StateMachine { get; private set; }

        /// <summary>
        /// ステート開始ストリーム
        /// </summary>
        private readonly Subject<T> _begin = new Subject<T>();

        /// <summary>
        /// ステート終了ストリーム
        /// </summary>
        private readonly Subject<Unit> _end = new Subject<Unit>();

        private T _val;

        #endregion

        #region event stream

        /// <summary>
        /// ステート開始ストリーム
        /// </summary>
        protected IObservable<T> BeginStream => _begin.AsObservable();

        /// <summary>
        /// ステート終了ストリーム
        /// </summary>
        protected IObservable<Unit> EndStream => _end.AsObservable();

        /// <summary>
        /// Updateストリーム
        /// </summary>
        protected IObservable<Unit> UpdateStream => StateStream(StateMachine.UpdateAsObservable());

        /// <summary>
        /// FixedUpdateストリーム
        /// </summary>
        protected IObservable<Unit> FixedUpdateStream => StateStream(StateMachine.FixedUpdateAsObservable());

        /// <summary>
        /// LateUpdateストリーム
        /// </summary>
        protected IObservable<Unit> LateUpdateStream => StateStream(StateMachine.LateUpdateAsObservable());

        /// <summary>
        /// OnDrawGizmosストリーム
        /// </summary>
        protected IObservable<Unit> OnDrawGizmosStream => StateStream(StateMachine.OnDrawGizmosAsObservable());

        /// <summary>
        /// OnGUIストリーム
        /// </summary>
        protected IObservable<Unit> OnGuiStream => StateStream(StateMachine.OnGUIAsObservable());

        /// <summary>
        /// このステートが現在ステートの間だけメッセージが流れるストリーム
        /// </summary>
        protected IObservable<TT> StateStream<TT>(IObservable<TT> source)
        {
            return source
                // BeginStreamがOnNextされてから
                .SkipUntil(BeginStream)
                // EndStreamがOnNextされるまで
                .TakeUntil(EndStream)
                .RepeatUntilDestroy(StateMachine)
                .Publish()
                .RefCount();
        }

        #endregion

        #region method

        public abstract void Initialize();

        /// <summary>
        /// ステートの開始通知(StateMachine以外では基本的に呼び出さない)
        /// </summary>
        public virtual void StateBegin()
        {
            _begin.OnNext(_val);
        }

        /// <summary>
        /// ステートの終了通知(StateMachine以外では基本的に呼び出さない)
        /// </summary>
        public void StateEnd()
        {
            _end.OnNext(Unit.Default);
        }

        /// <summary>
        /// BeginStreamで流す値のSet(StateMachine以外では基本的に呼び出さない)
        /// </summary>
        public void SetVal(T val)
        {
            _val = val;
        }

        public void SetStateMachine(StateMachine stateMachine)
        {
            StateMachine = stateMachine;
        }

        #endregion
    }

    public abstract class StateBase : State<Unit>
    {
    }
}