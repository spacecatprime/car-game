using UnityEngine;
using System.Collections;

/// <summary>
/// messages that WindowManager will issue for widget processing
/// </summary>
public interface UIWindowBase
{
    void Conceal();
    void Reveal();
    void SetupData(object data);
}
