using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ManagerHelper : MonoBehaviour
{
    protected IGameManager manager;
    public virtual void SetManager(IGameManager manager) 
    {
        this.manager = manager;
    }
}
