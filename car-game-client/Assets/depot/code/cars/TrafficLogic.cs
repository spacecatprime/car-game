using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TrafficLogic : MonoBehaviour 
{
    //
    // to be set by editor
    public Transform grid = null;
    public Transform car = null;
    public Transform notification = null;
    public FXManager fxManager = null;

    //
    // playback data
    Vector3 m_originalPos;
    Quaternion m_originalRot;
    GGObject m_car;
    NotificationPanel m_noteboard;

    public enum Operation
    {
        Reset,      // waiting to be told to either playback or start stepping
        
        // real-time
        Playback,
        Pause,
        
        // turn-by-turn
        TurnByTurn,
        StepUp,
        StepBack
    };
    public Operation Mode { get; private set; }

    #region LOGIC STATE

    public enum LogicState
    {
        Idle,       // nothing happening, yet
        Playing,    // real-time playback in progress
        Succeeded,  // Succeeded - done as far as the steps can be
        Failed      // Failed - car crash or some logic error happened
    };
    private LogicState m_state = LogicState.Idle;
    public LogicState State 
    {
        get
        {
            return m_state;
        }
        private set
        {
            if (OnStateChange != null && m_state != value)
            {
                OnStateChange(value);
            }
            m_state = value;
        }
    }
    public event Action<LogicState> OnStateChange;

    void FinishLineTriggered()
    {
        m_noteboard.AddNote("Finish Hit!");
        State = LogicState.Succeeded;
    }

    #endregion

    //
    // action data such as moves and cursor into moves
    private List<MoveType> m_moveList = new List<MoveType>();
    private List<MoveType>.Enumerator m_moveCursor;
    private Coroutine m_moveCoroutine;

    public bool SetOperation(Operation op)
    {
        return SetOperation(op, new List<RectTransform>());
    }

    public bool SetOperation(Operation op, List<RectTransform> opList)
    {
        if (op == Mode)
        {
            return false;
        }

        if (opList.Count > 0)
        {
            var moveList = new List<MoveType>();
            foreach (var opItem in opList)
            {
                MoveType mt = opItem.GetComponent<MoveType>();
                if (mt == null)
                {
                    return false;
                }
                if (mt.kind == MoveType.MoveKind.None)
                {
                    return false;
                }
                moveList.Add(mt);
            }
            m_moveList = moveList;
        }

        return ApplyOperation(op);
    }

    private bool ApplyOperation(Operation op)
    {
        switch (op)
        { 
            case Operation.Reset:
                ResetBoard();
                Mode = Operation.Reset;
                fxManager.audioFX.PlaySound(AudioFX.Sounds.Bonk);
                break;

            case Operation.Playback:
            case Operation.Pause:
                if (OperateRealTime(op))
                {
                    Mode = Operation.Playback;
                    return true;
                }
                break;

            case Operation.TurnByTurn:
            case Operation.StepUp:
            case Operation.StepBack:
                if (OperateTurnByTurn(op))
                {
                    Mode = Operation.TurnByTurn;
                    return true;
                }
                break;
        }
        return false;
    }

    private bool OperateTurnByTurn(Operation op)
    {
        if (op == Operation.TurnByTurn)
        {
            ResetBoard();
            m_noteboard.AddNote(op.ToString());
            return true;
        }
        else if (op == Operation.StepUp)
        {
            bool isOk = m_moveCursor.MoveNext();
            if (isOk)
            {
                OperateCar(m_moveCursor.Current);
            }
            m_noteboard.AddNote(op.ToString());
            return isOk;
        }
        else if (op == Operation.StepBack)
        {
            m_moveCursor = m_moveList.GetEnumerator();
            int pos = m_moveList.IndexOf(m_moveCursor.Current);
            while (pos > 0)
            {
                m_moveCursor.MoveNext();
                pos--;
            }
            return true;
        }
        return false;
    }

    private void MoveCar(MoveType moveType)
    {
        var angles = m_car.CachedTransform.rotation.eulerAngles;
        GGDirection dir = GGDirection.Invalid;
        if (angles.y > 269.0)
        {
            dir = GGDirection.Right;
        }
        else if (angles.y > 179.0)
        {
            dir = GGDirection.Up;
        }
        else if (angles.y > 89.0)
        {
            dir = GGDirection.Left;
        }
        else
        {
            dir = GGDirection.Down;
        }
        var nextCell = m_car.Cell.GetCellInDirection(dir);
        if (nextCell == null || nextCell.IsPathable == false)
        {
            fxManager.audioFX.PlaySound(AudioFX.Sounds.Bonk);
            fxManager.particleFX.Run(ParticleFX.Effects.Explosion, car.position);
            m_noteboard.AddNote("Oops!");
            State = LogicState.Failed;
        }
        else
        {
            iTween.MoveTo(m_car.gameObject, nextCell.CenterPoint3D, 1.0f);
            fxManager.audioFX.PlaySound(AudioFX.Sounds.Forward);
        }
    }

    private void OperateCar(MoveType moveType)
    {
        if( moveType.kind == MoveType.MoveKind.TurnLeft)
        {
            iTween.RotateAdd(m_car.gameObject, new Vector3(0, -90, 0), 1.0f);
            fxManager.audioFX.PlaySound(AudioFX.Sounds.Turn);
        }
        else if (moveType.kind == MoveType.MoveKind.TurnRight)
        {
            iTween.RotateAdd(m_car.gameObject, new Vector3(0, 90, 0), 1.0f);
            fxManager.audioFX.PlaySound(AudioFX.Sounds.Turn);
        }
        else
        {
            MoveCar(moveType);
        }
    }

    private bool OperateRealTime(Operation op)
    {
        State = LogicState.Playing;
        m_moveCoroutine = StartCoroutine(MoveAllMoves());
        return true;
    }

    private IEnumerator<WaitForSeconds> MoveAllMoves()
    {
        foreach (var move in m_moveList)
        {
            if (State != LogicState.Playing)
            {
                break;
            }
            OperateCar(move);
            yield return new WaitForSeconds(1);
        }
        if (State == LogicState.Playing)
        {
            State = LogicState.Failed;
            m_noteboard.AddNote("Out of moves...");
        }
    }

    private void ResetBoard()
    {
        // reset the car
        iTween.MoveTo(m_car.gameObject, m_originalPos, 1.0f);
        iTween.RotateTo(m_car.gameObject, m_originalRot.eulerAngles, 1.0f);

        // reset the recorded move list
        m_moveList.Clear();
        m_moveCursor = m_moveList.GetEnumerator();
        if (m_moveCoroutine != null)
        {
            StopCoroutine(m_moveCoroutine);
            m_moveCoroutine = null;
        }
    }

    void Start () 
	{
        State = LogicState.Idle;
        Mode = Operation.Reset;
        m_originalPos = car.transform.position;
        m_originalRot = car.transform.rotation;
        m_car = car.GetComponent<GGObject>();
        m_noteboard = notification.GetComponent<NotificationPanel>();
    }

    void Update()
    {
        // TODO: real-time updates, tweens, etc.
    }
}
