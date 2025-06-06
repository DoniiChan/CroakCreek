using UnityEngine;

namespace CroakCreek
{
    public enum FrogType { Water, Fire, Plant, Flying, Metal }
    public enum Temperament { Shy, Calm, Hostile }
    public enum FrogSize { Teacup, Modest, Chunky }
    public enum FrogRarity { Common, Uncommon, Rare, Epic, Legendary }

    [CreateAssetMenu(fileName = "New Frog", menuName = "Frog")]
    public class Frog : ScriptableObject
    {
        public new string name;
        public FrogType type;
        public Temperament temperament;
        public FrogSize size;
        public FrogRarity rarity;

        public Sprite sprite;

        public int health;
        public int damage;
        public float speed;
    }
}
