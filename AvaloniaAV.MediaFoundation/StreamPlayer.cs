using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using SharpDX.MediaFoundation;
using Device = SharpDX.DXGI.Device;
using System.Reactive.Subjects;
using SharpDX;

namespace AvaloniaAV.MediaFoundation
{
    public partial class StreamPlayer : IDisposable
    {
        private readonly MediaEngineEx engine;
        private readonly TimeSpan frameTime;
        private Task frameLoopTask;
        private CancellationTokenSource tokenSource = new CancellationTokenSource();
        
        public StreamPlayer(Device device, int fps)
        {
            Device = device.QueryInterface<SharpDX.Direct3D11.Device>();
            using (var manager = new DXGIDeviceManager())
            using (var factory = new MediaEngineClassFactory())
            using (var attributes = new MediaEngineAttributes(2))
            {
                manager.ResetDevice(device);
                attributes.DxgiManager = manager;
                attributes.VideoOutputFormat = (int)Format.B8G8R8A8_UNorm;
                var baseEngine = new MediaEngine(factory, attributes, playbackCallback: OnEngineEvent);
                engine = baseEngine.QueryInterface<MediaEngineEx>();
            }

            frameTime = TimeSpan.FromSeconds(1.0 / fps);
            SetCurrentState(StreamPlayerState.NoSource);
        }

        private void OnEngineEvent(MediaEngineEvent mediaevent, long param1, int param2)
        {
            switch (mediaevent)
            {
                case MediaEngineEvent.DurationChange:
                    SetDuration(engine.Duration);
                    break;
                case MediaEngineEvent.LoadStart:
                    SetCurrentState(StreamPlayerState.LoadingSource);
                    break;
                case MediaEngineEvent.LoadedMetadata:
                    int x, y;
                    engine.GetNativeVideoSize(out x, out y);
                    using (var texture = new Texture2D(Device, new Texture2DDescription
                    {
                        Format = Format.B8G8R8A8_UNorm,
                        Width = x,
                        Height = y,
                        ArraySize = 1,
                        MipLevels = 1,
                        SampleDescription = new SampleDescription
                        {
                            Count = 1
                        },
                        BindFlags = BindFlags.RenderTarget,
                    }))
                    {
                        TargetSurface?.Dispose();
                        TargetSurface = texture.QueryInterface<Surface>();
                        OnNewSurface(x, y);
                        SetCurrentTime(TimeSpan.Zero);
                    } 
                    break;
                case MediaEngineEvent.CanPlay:
                    SetCurrentState(StreamPlayerState.CanPlay);
                    break;
                case MediaEngineEvent.CanPlayThrough:
                    SetCurrentState(StreamPlayerState.CanPlayFully);
                    break;
                case MediaEngineEvent.Error:
                    // We don't want to throw on this thread, so instead break the debugger
                    if(System.Diagnostics.Debugger.IsAttached)
                    {
                        var exception = System.Runtime.InteropServices.Marshal.GetExceptionForHR(param2);
                        System.Diagnostics.Debugger.Break();
                    }
                    break;
            }
        }

        protected virtual void OnNewSurface(int x, int y)
        {
        }

        public void Open(Uri uri)
        {
            tokenSource.Cancel();
            frameLoopTask = null;
            engine.Source = uri.ToString();
            engine.Load();
            tokenSource = new CancellationTokenSource();
        }

        private ByteStream byteStream;

        public void Open(Stream stream, Uri uri)
        {
            tokenSource.Cancel();
            frameLoopTask = null;
            var temp = byteStream;
            engine.SetSourceFromByteStream(byteStream = new ByteStream(stream), TempPathForUri(stream, uri)); 
            engine.Load();
            temp?.Dispose();
            tokenSource = new CancellationTokenSource();
        }

        private string TempPathForUri(Stream stream, Uri uri)
        {
            var path = Path.GetTempFileName();
            using (var file = File.OpenWrite(path))
            {
                stream.CopyTo(file);
            }
            return path;
        }

        public void Play()
        {
            if (frameLoopTask == null)
            {
                frameLoopTask = Task.Run(() => FrameLoopAsync(tokenSource.Token), tokenSource.Token);
            }
            engine.Play();
        }

        public void Pause()
        {
            engine.Pause(); 
        }

        public void Seek(TimeSpan time)
        {
            engine.CurrentTime = time.TotalSeconds;
        }

        private async Task FrameLoopAsync(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    if (engine.OnVideoStreamTick(out long time))
                    {
                        engine.GetNativeVideoSize(out int width, out int height);
                        engine.TransferVideoFrame(TargetSurface, null, new RawRectangle(0, 0, width, height), null);
                        UpdateOutputSurface();
                        SetCurrentTime(new TimeSpan(time));
                    }
                    await Task.Delay(frameTime, token);
                }
            }
            catch (Exception)
            {
                Pause();
                throw;
            }
        }

        protected virtual void UpdateOutputSurface()
        {
        }

        private void SetCurrentTime(TimeSpan time)
        {
            currentTime.OnNext(time);
        }

        private void SetDuration(double secsDuration)
        {
            duration.OnNext(double.IsInfinity(secsDuration) || double.IsNaN(secsDuration)
                ? (TimeSpan?) null
                : TimeSpan.FromSeconds(secsDuration));
        }

        private void SetCurrentState(StreamPlayerState state)
        {
            currentState.OnNext(state);
        }

        public IObservable<TimeSpan> CurrentTime => currentTime;
        private readonly Subject<TimeSpan> currentTime = new Subject<TimeSpan>();

        public IObservable<TimeSpan?> Duration => duration;
        private readonly Subject<TimeSpan?> duration = new Subject<TimeSpan?>();

        public IObservable<StreamPlayerState> CurrentState => currentState;
        private readonly Subject<StreamPlayerState> currentState = new Subject<StreamPlayerState>();

        public virtual Surface Surface => TargetSurface;

        protected Surface TargetSurface { get; private set; }

        protected SharpDX.Direct3D11.Device Device { get; }

        public void Dispose()
        {
            Device.Dispose();
            engine.Shutdown();
            engine.Dispose();
            Surface.Dispose();
            byteStream?.Dispose();
        }
    }
}
