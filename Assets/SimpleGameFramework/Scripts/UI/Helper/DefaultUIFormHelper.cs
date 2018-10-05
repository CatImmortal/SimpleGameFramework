using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGameFramework.Resource;
namespace SimpleGameFramework.UI
{

    public class DefaultUIFormHelper : UIFormHelperBase
    {
        private ResourceManager m_ResourceManager;

        public DefaultUIFormHelper()
        {
            m_ResourceManager = FrameworkEntry.Instance.GetManager<ResourceManager>();
        }

        /// <summary>
        /// 创建界面
        /// </summary>
        /// <param name="uiFormInstance">界面实例</param>
        /// <param name="uiGroup">界面所属的界面组</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>界面</returns>
        public override UIForm CreateUIForm(object uiFormInstance, UIGroup uiGroup, object userData)
        {
            GameObject gameObject = uiFormInstance as GameObject;
            if (gameObject == null)
            {
                Debug.LogError("要创建的界面的实例为空");
                return null;
            }

            Transform transform = gameObject.transform;
            transform.SetParent(uiGroup.UIGroupHelper.transform);
            transform.localScale = Vector3.one;

            //挂载UIForm脚本
            UIForm uiForm = gameObject.GetOrAddComponent<UIForm>();
            return uiForm;
        }

        /// <summary>
        /// 实例化界面
        /// </summary>
        /// <param name="uiFormAsset">要实例化的界面资源</param>
        /// <returns>实例化后的界面</returns>
        public override object InstantiateUIForm(object uiFormAsset)
        {
            return Object.Instantiate((Object)uiFormAsset);
        }

        /// <summary>
        /// 释放界面
        /// </summary>
        /// <param name="uiFormAsset">要释放的界面资源</param>
        /// <param name="uiFormInstance">要释放的界面实例</param>
        public override void ReleaseUIForm(object uiFormAsset, object uiFormInstance)
        {
            m_ResourceManager.UnloadAsset(uiFormAsset);
            Object.Destroy((Object)uiFormAsset);
        }
    }

}
