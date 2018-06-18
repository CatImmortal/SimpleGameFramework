using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.DataTable;
public class DataTableTestMain : MonoBehaviour {

    private DataTableManager m_DataTableManager;

    private void Start()
    {
        m_DataTableManager = FrameworkEntry.Instance.GetManager<DataTableManager>();
        m_DataTableManager.LoadDataTable<DRAircraft>("Aircraft", "Aircraft");

        DataTable<DRAircraft> aircraftTable = m_DataTableManager.GetDataTable<DRAircraft>();
        DRAircraft aircraft = aircraftTable[10000];  //这里要给战机编号
        Debug.Log(aircraft.Id);
    }

}
