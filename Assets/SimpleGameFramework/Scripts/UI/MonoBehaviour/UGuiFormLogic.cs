using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleGameFramework.UI
{
    /// <summary>
    /// UGUI界面逻辑基类
    /// </summary>
    public abstract class UGuiFormLogic : UIFormLogic{

        public const int DepthFactor = 100;

        /// <summary>
        /// 界面隐藏时间
        /// </summary>
        private const float FadeTime = 0.3f;

        /// <summary>
        /// 主字体
        /// </summary>
        private static Font s_MainFont = null;

        /// <summary>
        /// 缓存的 Canvas
        /// </summary>
        private Canvas m_CachedCanvas = null;

        /// <summary>
        /// Canvas组
        /// </summary>
        private CanvasGroup m_CanvasGroup = null;

        public int OriginalDepth { get; private set; }

        /// <summary>
        /// 界面深度
        /// </summary>
        public int Depth
        {
            get
            {
                return m_CachedCanvas.sortingOrder;
            }
        }

        /// <summary>
        /// 界面变色
        /// </summary>
        private IEnumerator FadeToAlpha(float alpha, float duration)
        {
            float time = 0f;
            float originalAlpha = m_CanvasGroup.alpha;
            while (time < duration)
            {
                time += Time.deltaTime;
                m_CanvasGroup.alpha = Mathf.Lerp(originalAlpha, alpha, time / duration);
                yield return new WaitForEndOfFrame();
            }

            m_CanvasGroup.alpha = alpha;
        }

        /// <summary>
        /// 关闭界面
        /// </summary>
        public void Close(bool ignoreFade = false)
        {
            StopAllCoroutines();

            if (ignoreFade)
            {
                FrameworkEntry.Instance.GetManager<UIManager>().CloseUIForm(UIForm.SerialId);
            }
            else
            {
                StartCoroutine(FadeToAlpha(0,FadeTime));
            }
        }

        

        /// <summary>
        /// 设置主字体
        /// </summary>
        public static void SetMainFont(Font mainFont)
        {
            if (mainFont == null)
            {
                Debug.LogError("要设置的主字体为空");
                return;
            }

            s_MainFont = mainFont;

            GameObject go = new GameObject();
            go.AddComponent<Text>().font = mainFont;
            Destroy(go);
        }

        /// <summary>
        /// 界面初始化
        /// </summary>
        /// <param name="userData"></param>
        public override void OnInit(object userData)
        {
            base.OnInit(userData);
            
            m_CachedCanvas = gameObject.GetOrAddComponent<Canvas>();
            m_CachedCanvas.overrideSorting = true;
            OriginalDepth = m_CachedCanvas.sortingOrder;

            m_CanvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();

            RectTransform transform = GetComponent<RectTransform>();
            transform.anchorMin = Vector2.zero;
            transform.anchorMax = Vector2.one;
            transform.anchoredPosition = Vector2.zero;
            transform.sizeDelta = Vector2.zero;

            gameObject.GetOrAddComponent<GraphicRaycaster>();

            //修改文字
            Text[] texts = GetComponentsInChildren<Text>(true);
            for (int i = 0; i < texts.Length; i++)
            {
                texts[i].font = s_MainFont;
                if (!string.IsNullOrEmpty(texts[i].text))
                {
                    texts[i].text = FrameworkEntry.Instance.GetManager<Localization.LocalizationManager>().GetString(texts[i].text);
                }
            }
        }

        public override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            m_CanvasGroup.alpha = 0f;
            StopAllCoroutines();
            StartCoroutine(FadeToAlpha(1f, FadeTime));
        }

        public override void OnResume()
        {
            m_CanvasGroup.alpha = 0f;
            StopAllCoroutines();
            StartCoroutine(FadeToAlpha(1f, FadeTime));
        }

        public override void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
        {
            int oldDepth = Depth;
            base.OnDepthChanged(uiGroupDepth, depthInUIGroup);
            int deltaDepth = UGuiGroupHelper.DepthFactor * uiGroupDepth + DepthFactor * depthInUIGroup - oldDepth + OriginalDepth;
            Canvas[] canvases = GetComponentsInChildren<Canvas>(true);
            for (int i = 0; i < canvases.Length; i++)
            {
                canvases[i].sortingOrder += deltaDepth;
            }

        }
    }
}

