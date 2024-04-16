using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ManagerHelper : MonoBehaviour
{
    protected GameManager manager;
    public virtual void SetManager(GameManager manager) 
    {
        this.manager = manager;
    }
}
