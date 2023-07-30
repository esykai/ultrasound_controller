using NAudio.Wave;
using UltraSoundController;

namespace UltraSoundController.Commands
{
    class Music : ICommandHandler
    {
        public string commandName => "Music";
        public string commandDesc => "Играет музыку";
        public int startHZ => 18000;
        public int endHZ => 18100;
        public int coolDown => 1;

        private MusicPlayer player = new MusicPlayer();

        public Music()
        {
            Console.WriteLine("Музыка инициализирована успешно.");
        }
        public void Start(int hz)
        {
            if (hz >= 18080)
            {
                player.Stop();
                return;
            }

            if (hz >= 18040)
            {
                player.Up();
            }
            else
            {
                player.Down();
            }
        }
    }
}

class MusicPlayer
{
    private string musicFolderPath = CONSTANTS.FOLDER_MUSIC;
    private string[] musicFiles;
    private int currentTrackIndex = 0;
    private WaveOutEvent outputDevice;
    private AudioFileReader audioFile;


    public MusicPlayer()
    {
        // Получаем список всех файлов с расширениями .mp3, .wav и другими аудиоформатами из папки
        musicFiles = Directory.GetFiles(musicFolderPath)
                              .Where(file => IsAudioFile(file))
                              .ToArray();

        if (musicFiles.Length == 0)
        {
            Console.WriteLine("В папке нет музыкальных файлов.");
            return;
        }
    }

    private bool IsAudioFile(string filePath)
    {
        string ext = Path.GetExtension(filePath).ToLower();
        return ext == ".mp3" || ext == ".wav" || ext == ".ogg"; // Добавьте другие поддерживаемые форматы, если нужно
    }

    public void Up()
    {
        Stop();

        currentTrackIndex++;
        if (currentTrackIndex >= musicFiles.Length)
        {
            currentTrackIndex = 0;
        }
        Play();
    }

    public void Down()
    {
        Stop();

        currentTrackIndex--;
        if (currentTrackIndex < 0)
        {
            currentTrackIndex = musicFiles.Length - 1;
        }

        Play();
    }

    public void Play()
    {
        if (musicFiles.Length == 0)
        {
            Console.WriteLine("В папке нет музыкальных файлов.");
            return;
        }


        outputDevice = new WaveOutEvent();


        audioFile = new AudioFileReader(musicFiles[currentTrackIndex]);
        outputDevice.Init(audioFile);


        outputDevice.Play();
        // Console.WriteLine("Now playing: " + musicFiles[currentTrackIndex]);
    }

    public void Stop()
    {
        if (outputDevice != null)
        {
            outputDevice.Stop();
        }
    }
}