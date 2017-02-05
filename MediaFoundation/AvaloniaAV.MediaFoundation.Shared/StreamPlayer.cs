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

namespace AvaloniaAV.MediaFoundation
{
    public class StreamPlayer : IDisposable
    {
        private readonly Device device;
        private readonly MediaEngineEx engine;
        private readonly TimeSpan frameTime;
        private Task frameLoopTask;
        private CancellationTokenSource tokenSource = new CancellationTokenSource();


        public StreamPlayer(Device device, int fps)
        {
            this.device = device;
            using (var manager = new DXGIDeviceManager())
            using (var factory = new MediaEngineClassFactory())
            using (var attributes = new MediaEngineAttributes(2))
            {
                manager.ResetDevice(device);
                attributes.DxgiManager = manager;
                attributes.VideoOutputFormat = (int)Format.R32G32B32A32_Float;
                using (var baseEngine = new MediaEngine(factory, attributes, playbackCallback: OnEngineEvent))
                {
                    engine = baseEngine.QueryInterface<MediaEngineEx>();
                }
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
                    SetDuration(engine.Duration);
                    engine.GetNativeVideoSize(out x, out y);
                    using (var d3DDevice = device.QueryInterface<SharpDX.Direct3D11.Device>())
                    using (var texture = new Texture2D(d3DDevice, new Texture2DDescription
                    {
                        Format = Format.B8G8R8A8_UNorm,
                        Width = x,
                        Height = y
                    }))
                    {
                        Surface?.Dispose();
                        Surface = texture.QueryInterface<Surface>();
                    } 
                    break;
                case MediaEngineEvent.CanPlay:
                    SetCurrentState(StreamPlayerState.CanPlay);
                    break;
                case MediaEngineEvent.CanPlayThrough:
                    SetCurrentState(StreamPlayerState.CanPlayFully);
                    break;
            }
        }

        public void Open(Uri uri)
        {
            tokenSource.Cancel();
            frameLoopTask = null;
            engine.Source = uri.ToString();
            engine.Load();
            tokenSource = new CancellationTokenSource();
        }
        
        public void Open(Stream stream, Uri uri)
        {
            tokenSource.Cancel();
            frameLoopTask = null;
            engine.SetSourceFromByteStream(new ByteStream(stream), uri.ToString());
            engine.Load();
            tokenSource = new CancellationTokenSource();
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
            while (!token.IsCancellationRequested)
            {
                if (engine.OnVideoStreamTick(out long time))
                {
                    engine.GetNativeVideoSize(out int width, out int height);
                    engine.TransferVideoFrame(Surface, null, new RawRectangle(0, 0, width, height), null);
                    SetCurrentTime(new TimeSpan(time));
                }
                await Task.Delay(frameTime, token);
            }
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

        public Surface Surface { get; private set; }

        public void Dispose()
        {
            engine.Shutdown();
            engine.Dispose();
            Surface.Dispose();
        }
    }
}
