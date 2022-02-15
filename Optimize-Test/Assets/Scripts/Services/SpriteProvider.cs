using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(fileName = "SpriteProvider", menuName = "ScriptableObjects/SpriteProvider")]
public class SpriteProvider : ScriptableObject
{
    [Tooltip("Sprites referenced by Index.")]
    [SerializeField] private Sprite[] _sprites;

    /// <summary>
    /// Returns sprite of given index or sprite[0] if there isn't one.
    /// </summary>
    /// <param name="index">Index of sprite.</param>
    /// <returns>Sprite</returns>
    public Sprite GetIcon(int index)
    {
        if (index < 0 || index > _sprites.Length)
        {
            Debug.LogWarning($"Sprite for Index {index} not found!");
            return _sprites[0];
        }

        return _sprites[index];
    }
}
