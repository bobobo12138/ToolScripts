using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class RefreshContentSizeFitters : MonoBehaviour
{
    private void Start()
    {
        RefreshAll();
    }

    public void RefreshAll()
    {
        UniTask.Void(async () =>
        {
            await UniTask.DelayFrame(1); // 等待一帧，确保所有布局更新完成

            // 获取所有 ContentSizeFitter 组件
            ContentSizeFitter[] fitters = GetComponentsInChildren<ContentSizeFitter>(true);

            foreach (var fitter in fitters)
            {
                fitter.enabled = false;
            }

            Canvas.ForceUpdateCanvases();

            foreach (var fitter in fitters)
            {
                // 强制刷新布局
                fitter.enabled = true;
                LayoutRebuilder.ForceRebuildLayoutImmediate(fitter.GetComponent<RectTransform>());
            }
        });

    }
}