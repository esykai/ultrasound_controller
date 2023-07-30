using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;
using NAudio.Wave;

namespace UltraSoundController
{
    internal class FFT
    {
        private WaveInEvent waveIn;
        private Complex32[] fftBuffer;
        private static List<ICommandHandler> allCommands;

        private int startHZ;
        private int endHZ;


        public FFT(int startHZ = 0, int endHZ = 0, Action<int> onSuccess = null)
        {
            fftBuffer = new Complex32[CONSTANTS.BUFFER_SIZE];
            waveIn = new WaveInEvent();
            allCommands = CommandHandler.getAllSubClassOfICommandHandler();

            this.startHZ = startHZ;
            this.endHZ = endHZ;

            if (onSuccess != null)
            {
                this.onSuccess = onSuccess;
            }
        }

        private Action<int> onSuccess = (int hz) =>
        {
            foreach (var instance in allCommands)
            {
                if (hz >= instance.startHZ && hz <= instance.endHZ)
                {
                    CommandHandler.DestroyFakeSignal(instance);

                    if (CommandHandler.IsItTrue(instance))
                    {
                        if (CommandHandler.GetCoolDownByCMD(instance))
                        {
                            continue;
                        }

                        instance.Start(hz);
                        Console.WriteLine("Команда " + instance.commandName + " запущена!");
                        CommandHandler.ActivateCoolDownSec(instance, instance.coolDown);
                    }
                }
            }
        };


        public void Start()
        {
            waveIn = new WaveInEvent();
            waveIn.BufferMilliseconds = CONSTANTS.BUFFER_MS;
            waveIn.WaveFormat = new WaveFormat(CONSTANTS.SAMPLING_RATE, waveIn.WaveFormat.Channels);
            waveIn.DataAvailable += WaveIn_DataAvailable;
            waveIn.StartRecording();
        }

        public void Stop()
        {
            if (waveIn != null)
            {
                waveIn.StopRecording();
                waveIn.Dispose();
                waveIn = null;
            }
        }

        private void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            var buffer = new float[e.BytesRecorded / 2];
            for (int i = 0; i < e.BytesRecorded; i += 2)
            {
                buffer[i / 2] = (short)((e.Buffer[i + 1] << 8) | e.Buffer[i]) / 32768f; //2 ^ 15
            }

            for (int i = 0; i < CONSTANTS.BUFFER_SIZE; ++i)
            {
                fftBuffer[i] = new Complex32(i < buffer.Length ? buffer[i] : 0, 0);
            }

            Fourier.Forward(fftBuffer, FourierOptions.NoScaling);

            int startFreqIndex = (int)Math.Ceiling(startHZ * CONSTANTS.BUFFER_SIZE / (double)CONSTANTS.SAMPLING_RATE);
            int endFreqIndex = (int)Math.Ceiling(endHZ * CONSTANTS.BUFFER_SIZE / (double)CONSTANTS.SAMPLING_RATE);

            for (int i = startFreqIndex; i < endFreqIndex; ++i)
            {
                if (fftBuffer[i].Magnitude >= CONSTANTS.MAGNITUDE)
                {
                    onSuccess(i * 8);
                }
            }
        }
    }
}
