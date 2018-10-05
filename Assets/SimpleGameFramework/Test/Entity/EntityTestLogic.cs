using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Entity;

public class EntityTestLogic : EntityLogic{

    public override void OnShow(object userData)
    {
        base.OnShow(userData);

        Debug.Log("OnShow");
    }

    public override void OnUpdate(float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(elapseSeconds, realElapseSeconds);

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        transform.Translate(new Vector3(h, 0, v) * elapseSeconds);
    }

    public override void OnHide(object userData)
    {
        base.OnHide(userData);

        Debug.Log("OnHide");
    }
}
