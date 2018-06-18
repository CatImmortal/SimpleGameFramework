using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.ObjectPool;
public class TestObject : ObjectBase
{
    public TestObject(object target, string name = "") : base(target, name)
    {
    }


    protected override void OnSpawn()
    {
        base.OnSpawn();

    }

    public override void Release()
    {
       
    }
}
