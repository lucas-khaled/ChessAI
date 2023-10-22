using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnvironmentable
{
    public Environment Environment { get; }
    public IEnvironmentable Copy(Environment env);
}
