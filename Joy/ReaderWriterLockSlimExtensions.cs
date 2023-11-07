namespace AvP.Joy;

public static class ReaderWriterLockSlimExtensions
{
    public static IDisposable EnterReadLockDisposable(this ReaderWriterLockSlim rwLock)
    {
        rwLock.EnterReadLock();
        return new DelegatingDisposable(rwLock.ExitReadLock,
            DelegatingDisposable.OnExtraCall.PassThrough);
    }

    public static IDisposable EnterUpgradeableReadLockDisposable(this ReaderWriterLockSlim rwLock)
    {
        rwLock.EnterUpgradeableReadLock();
        return new DelegatingDisposable(rwLock.ExitUpgradeableReadLock,
            DelegatingDisposable.OnExtraCall.PassThrough);
    }

    public static IDisposable EnterWriteLockDisposable(this ReaderWriterLockSlim rwLock)
    {
        rwLock.EnterWriteLock();
        return new DelegatingDisposable(rwLock.ExitWriteLock,
            DelegatingDisposable.OnExtraCall.PassThrough);
    }
}
