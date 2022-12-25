using System.Runtime.InteropServices;

namespace IconResMan
{
    public class CachedIntPtr : IDisposable
    {
        // TODO: Finalizer nur überschreiben, wenn "Dispose(bool disposing)" Code für die Freigabe nicht verwalteter Ressourcen enthält
        ~CachedIntPtr()
        {
            // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(bool disposing)" ein.
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(bool disposing)" ein.
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public IntPtr IntPtr
        {
            get
            {   if (!IntPtrCreated)
                {
                    IntPtrCreated = true;
                    cachedIntPtr = IntPtr.Zero;
                    cachedNeedsToBeReleased = false;
                    GetCachedIntPtr(ref cachedIntPtr, ref cachedNeedsToBeReleased);
                }
                return cachedIntPtr;
            }
        }

        protected virtual void GetCachedIntPtr(ref IntPtr handle, ref bool needsToBeReleased)
        {
        }

        public void Release()
        {
            if (cachedNeedsToBeReleased)
                Marshal.FreeHGlobal(cachedIntPtr);
            cachedIntPtr = IntPtr.Zero;
            IntPtrCreated = false;
        }

        private IntPtr cachedIntPtr = IntPtr.Zero;
        private bool IntPtrCreated = false;
        private bool cachedNeedsToBeReleased = false;

        private bool disposedValue;

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: Verwalteten Zustand (verwaltete Objekte) bereinigen
                }

                // TODO: Nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalizer überschreiben
                // TODO: Große Felder auf NULL setzen
                Release();

                disposedValue = true;
            }
        }
    }
}
