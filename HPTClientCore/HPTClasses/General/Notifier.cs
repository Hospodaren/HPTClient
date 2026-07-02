using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HPTClient;

/// <summary>
/// Base class providing INotifyPropertyChanged implementation with CallerMemberName support.
/// </summary>
[Serializable]
public abstract class Notifier : INotifyPropertyChanged
{
    [field: NonSerialized]
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Raises PropertyChanged for the specified property.
    /// </summary>
    /// <param name="propertyName">The property name (auto-populated via CallerMemberName).</param>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Sets a field to a new value if different, raising PropertyChanged.
    /// </summary>
    /// <typeparam name="T">Type of the field.</typeparam>
    /// <param name="field">Reference to the backing field.</param>
    /// <param name="value">The new value.</param>
    /// <param name="propertyName">Property name (auto-populated).</param>
    /// <returns>True if the value changed; otherwise false.</returns>
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}

/// <summary>
/// Carries metadata for a property setter operation.
/// </summary>
public class PropertySetterInfo
{
    public string? PropertyName { get; set; }
    public Type? ClassType { get; set; }
    public object? ClassInstance { get; set; }
    public object? OldValue { get; set; }
    public object? NewValue { get; set; }
}