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
    public abstract class State
    {
        #region variable

        /// <summary>
        /// ステートマシン
        /// </summary>
        public StateMachine StateMachine { get; set; }

        /// <summary>
        /// ステート開始ストリーム
        /// </summary>
        private readonly Subject<Unit> _begin = new Subject<Unit>();

        /// <summary>
        /// ステート終了ストリーム
        /// </summary>
        private readonly Subject<Unit> _end = new Subject<Unit>();

        #endregion

        #region event stream

        /// <summary>
        /// ステート開始ストリーム
        /// </summary>
        public IObservable<Unit> BeginStream
        {
            get { return _begin.AsObservable(); }
        }

        /// <summary>
        /// ステート終了ストリーム
        /// </summary>
        public IObservable<Unit> EndStream
        {
            get { return _end.AsObservable(); }
        }

        /// <summary>
        /// Updateストリーム
        /// </summary>
        public IObservable<Unit> UpdateStream
        {
            get { return StateStream(StateMachine.UpdateAsObservable()); }
        }

        /// <summary>
        /// FixedUpdateストリーム
        /// </summary>
        public IObservable<Unit> FixedUpdateStream
        {
            get { return StateStream(StateMachine.FixedUpdateAsObservable()); }
        }

        /// <summary>
        /// LateUpdateストリーム
        /// </summary>
        public IObservable<Unit> LateUpdateStream
        {
            get { return StateStream(StateMachine.LateUpdateAsObservable()); }
        }

        /// <summary>
        /// OnDrawGizmosストリーム
        /// </summary>
        public IObservable<Unit> OnDrawGizmosStream
        {
            get { return StateStream(StateMachine.OnDrawGizmosAsObservable()); }
        }

        /// <summary>
        /// OnGUIストリーム
        /// </summary>
        public IObservable<Unit> OnGUIStream
        {
            get { return StateStream(StateMachine.OnGUIAsObservable()); }
        }

        /// <summary>
        /// このステートが現在ステートの間だけメッセージが流れるストリーム
        /// </summary>
        protected virtual IObservable<T> StateStream<T>(IObservable<T> source)
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
            _begin.OnNext(default(Unit));
        }

        /// <summary>
        /// ステートの終了通知(StateMachine以外では基本的に呼び出さない)
        /// </summary>
        public void StateEnd()
        {
            _end.OnNext(default(Unit));
        }

        #endregion
    }

    public abstract class State<T> : State
    {
        private readonly Subject<T> _begin = new Subject<T>();

        private T _val;

        public void SetVal(T val)
        {
            _val = val;
        }

        public new IObservable<T> BeginStream => _begin.AsObservable();

        public override void StateBegin()
        {
            _begin.OnNext(_val);
        }
        
        protected override IObservable<T> StateStream<T>(IObservable<T> source)
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
    }
}