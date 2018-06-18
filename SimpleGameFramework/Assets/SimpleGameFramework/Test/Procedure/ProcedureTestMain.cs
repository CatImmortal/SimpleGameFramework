using SimpleGameFramework.Base;
using SimpleGameFramework.Procedure;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcedureTestMain : MonoBehaviour
{
     void Awake()
    {
        ProcedureManager procedureManager = FrameworkEntry.Instance.GetManager<ProcedureManager>();

        //添加所有流程，并创建状态机
        Procedure_1st entranceProcedure = new Procedure_1st();
        procedureManager.AddProcedure(entranceProcedure);
        procedureManager.SetEntranceProcedure(entranceProcedure);

        procedureManager.AddProcedure(new Procedure_2nd());
        procedureManager.AddProcedure(new Procedure_3rd());

        procedureManager.CreateProceduresFsm();
    }
}
