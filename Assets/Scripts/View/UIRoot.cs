using UnityEngine;

/// <summary>
/// Base class for UI roots for different controllers.
/// </summary>
public class UIRoot : MonoBehaviour
{
    /// <summary>
    /// Method used to show UI.
    /// </summary>
    public virtual void ShowRoot()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Method used to hide UI.
    /// </summary>
    public virtual void HideRoot()
    {
        gameObject.SetActive(false);
    }
}