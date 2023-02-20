using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyPlayer : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _nameText;
    [SerializeField]
    private Renderer _readyRenderer;

    private MaterialPropertyBlock _materialPropertyBlock;
    private LobbyPlayerData _data;

    public void UpdateData(LobbyPlayerData data)
    {
        if (this == null)
        {
            return;
        }

        _data = data;
        _nameText.text = _data.GamerTag;

        if (_readyRenderer)
        {
            if (_materialPropertyBlock == null)
            {
                _materialPropertyBlock = new MaterialPropertyBlock();
            }

            _readyRenderer.GetPropertyBlock(_materialPropertyBlock);
            _materialPropertyBlock.SetColor("_BaseColor", _data.IsReady ? Color.green : Color.red);
            _readyRenderer.SetPropertyBlock(_materialPropertyBlock);
        }

        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
        }
    }
}
