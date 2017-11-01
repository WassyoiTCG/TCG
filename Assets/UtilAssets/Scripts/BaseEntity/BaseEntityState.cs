// State基底クラスをキャラクタ毎に再利用可能に
// クラステンプレートにすると人生が楽になる。

public abstract class BaseEntityState<entity_type> where entity_type : class, new()
{
    // 入る
    public abstract void Enter(entity_type e);

	// 実行します
	public abstract void Execute(entity_type e);

	// 帰る
	public abstract void Exit(entity_type e);

    /*
		BaseGameEntityのステートがメッセージを受け取り
		処理を選択できるように修正する必要がある
	*/
    // エージェントからのメッセージを受信した場合、これが実行される
    public abstract bool OnMessage(entity_type e, MessageInfo message);
};
