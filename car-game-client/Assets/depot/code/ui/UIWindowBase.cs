using UnityEngine;
using System.Collections;

/// <summary>
/// messages that WindowManager will issue for widget processing
/// </summary>
public interface UIWindowBase
{
    void Hide();
    void Show();
    void SetupData(object data);
}
