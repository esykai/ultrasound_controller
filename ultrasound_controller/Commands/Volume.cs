using AudioSwitcher.AudioApi.CoreAudio;

namespace UltraSoundController.Commands
{
    class Volume : ICommandHandler
    {
        public string commandName => "Volume";
        public string commandDesc => "Регулирует громкость";
        public int startHZ => 18100;
        public int endHZ => 18200;
        public int coolDown => 1;

        private CoreAudioDevice defaultPlaybackDevice = new CoreAudioController().DefaultPlaybackDevice;

        public Volume()
        {
            Console.WriteLine("Громкость инициализирована успешно.");
        }
        public void Start(int hz)
        {
            hz = hz - 18100;

            defaultPlaybackDevice.Volume = hz;
        }
    }
}
