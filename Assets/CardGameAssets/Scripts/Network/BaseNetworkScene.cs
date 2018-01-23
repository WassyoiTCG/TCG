using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseNetworkScene : MonoBehaviour
{
    public abstract void HandleMessage(MessageInfo message);
}
