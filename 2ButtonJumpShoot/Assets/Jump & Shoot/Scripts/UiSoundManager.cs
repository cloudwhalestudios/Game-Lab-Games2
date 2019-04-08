using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiSoundManager : MonoBehaviour
{
    public void HoverSound()
    {
        AudioManager.Instance.PlaySoundNormally(AudioManager.Instance.UI);
    }
    public void ConfirmSound()
    {
        AudioManager.Instance.PlaySoundNormally(AudioManager.Instance.UI_confirm);
    }
    public void CancelSound()
    {
        AudioManager.Instance.PlaySoundNormally(AudioManager.Instance.UI_cancel);
    }
}
