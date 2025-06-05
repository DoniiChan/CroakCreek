using UnityEngine.Events;

namespace CroakCreek
{
    public interface IBarValueSource
    {
        int currentValue { get; }
        int maxValue { get; }

        UnityEvent<int> OnValueIncreased { get; }
        UnityEvent<int> OnValueDecreased { get; }
    }
}
