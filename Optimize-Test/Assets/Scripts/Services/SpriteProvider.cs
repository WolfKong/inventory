using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(fileName = "SpriteProvider", menuName = "ScriptableObjects/SpriteProvider")]
public class SpriteProvider : ScriptableObject
{
    [Tooltip("Icons referenced by Index.")]
    [SerializeField] private Sprite[] _icons;

    public Sprite GetIcon(int index)
    {
        if (index < 0 || index > _icons.Length)
        {
            Debug.LogWarning($"Sprite for Index {index} not found!");
            return _icons[0];
        }

        return _icons[index];
    }
}
