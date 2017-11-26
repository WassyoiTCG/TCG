
using UnityEngine;

public class BaseEntityStateMachine<entity_type> where entity_type : class, new()
{
    entity_type owner;
    public BaseEntityState<entity_type> currentState { get; set; }
    public BaseEntityState<entity_type> previousState { get; set; }
    public BaseEntityState<entity_type> globalState { get; set; }

    public BaseEntityStateMachine(entity_type owner)
    {
        this.owner = owner;
        currentState = previousState = globalState = null;
    }

    public void Update()
    {
        if (globalState != null) globalState.Execute(owner);
        if (currentState != null) currentState.Execute(owner);
    }

    public void ChangeState(BaseEntityState<entity_type> newState)
    {
        // ステート保存
        previousState = currentState;
        // 終了関数
        if (currentState != null) currentState.Exit(owner);
        // ステート切り替え
        currentState = newState;
        // 開始関数
        currentState.Enter(owner);
    }
    public void ReventToPreviousState()
    {
        Debug.Assert(previousState != null, "前回ステートがnull");
        ChangeState(previousState);
    }

    public bool isInState(BaseEntityState<entity_type> st) { return (currentState.GetType() == st.GetType()); }

    public bool HandleMessage(MessageInfo message)
    {
        if (globalState != null)
        {
            if (globalState.OnMessage(owner, message)) return true;
        }

        if (currentState != null)
        {
            if (currentState.OnMessage(owner, message)) return true;
        }

        return false;
    }
}
