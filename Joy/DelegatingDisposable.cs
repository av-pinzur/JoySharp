namespace AvP.Joy;

public sealed class DelegatingDisposable : IDisposable
{
    private readonly Action disposer;
    private readonly OnExtraCall onExtraCall;
    private bool isDisposed;

    public DelegatingDisposable(Action disposer)
        : this(disposer, OnExtraCall.Throw) { }

    public DelegatingDisposable(Action disposer, OnExtraCall onExtraCall)
    {
        if (disposer == null) throw new ArgumentNullException(nameof(disposer));
        if (!onExtraCall.IsDefined()) throw new ArgumentOutOfRangeException(nameof(onExtraCall), "Parameter must be a defined member of the enumerated type.");

        this.disposer = disposer;
        this.onExtraCall = onExtraCall;
    }

    public void Dispose()
    {
        if (isDisposed) switch (onExtraCall)
            {
                case OnExtraCall.Throw: throw new ObjectDisposedException("The specified object has already been disposed.");
                case OnExtraCall.Ignore: return;
                    // Otherwise, pass through.
            }
        isDisposed = true;
        disposer();
    }

    public enum OnExtraCall { Throw, PassThrough, Ignore }
}
