using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class TrafficLogic : MonoBehaviour 
{
    //
    // to be set by editor
    public Transform grid = null;
    public Transform car = null;
    public Transform notification = null;

    //
    // playback data
    Vector3 m_originalPos;
    Quaternion m_originalRot;
    GGObject m_car;
    GGGrid m_grid;
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

    public enum OperationState
    {
        Idle,
        Playing,    // real-time playback in progress
        Paused,     // paused during real-time playback
        Stepped,    // waiting for the next Step op command
        Finished,   // done as far as the steps can be
        InError     // car crash or some logic error happened
    };
    public OperationState State { get; private set; }

    public class Result
    {
        public bool inError { get; set; }
        public TrafficLogic logic { get; set; }
    }

    public event Action<Result> OnOperationChange;
    public event Action<Result> OnStateChange;

    //
    // action data such as moves and cursor into moves
    private List<MoveType> m_moveList = new List<MoveType>();
    private List<MoveType>.Enumerator m_moveCursor;

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
        m_car.CachedTransform.position = nextCell.CenterPoint3D;
        m_car.Update();
    }

    private void OperateCar(MoveType moveType)
    {
        if( moveType.kind == MoveType.MoveKind.TurnLeft)
        {
            m_car.CachedTransform.Rotate(m_car.CachedTransform.up, -90);
            m_car.Update();
        }
        else if (moveType.kind == MoveType.MoveKind.TurnRight)
        {
            m_car.CachedTransform.Rotate(m_car.CachedTransform.up, 90);
            m_car.Update();
        }
        else
        {
            MoveCar(moveType);
        }
    }

    private bool OperateRealTime(Operation op)
    {
        throw new NotImplementedException();
    }

    private void ResetBoard()
    {
        m_moveCursor = m_moveList.GetEnumerator();
        m_car.CachedTransform.position = new Vector3(m_originalPos.x, m_originalPos.y, m_originalPos.z);
        m_car.CachedTransform.rotation = new Quaternion(m_originalRot.x, m_originalRot.y, m_originalRot.z, m_originalRot.w);
    }

    void Start () 
	{
        State = OperationState.Idle;
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
