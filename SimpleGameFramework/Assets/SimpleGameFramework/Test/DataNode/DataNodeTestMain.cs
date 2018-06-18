using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Base;
using SimpleGameFramework.DataNode;
public class DataNodeTestMain : MonoBehaviour
{

     void Start()
    {
        //根据绝对路径设置与获取数据
        DataNodeManager dataNodeManager = FrameworkEntry.Instance.GetManager<DataNodeManager>();
        dataNodeManager.SetData("Player.Name", "Ellan");
        string playerName = dataNodeManager.GetData<string>("Player.Name");
        Debug.Log(playerName);

        //根据相对路径设置与获取数据
        DataNode playerNode = dataNodeManager.GetNode("Player");
        dataNodeManager.SetData("Level", 99, playerNode);
        int playerLevel = dataNodeManager.GetData<int>("Level", playerNode);
        Debug.Log(playerLevel);

        //直接通过数据结点来操作
        DataNode playerExpNode = playerNode.GetOrAddChild("Exp");
        playerExpNode.SetData(1000);
        int playerExp = playerExpNode.GetData<int>();
        Debug.Log(playerExp);
    }

}



