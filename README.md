### Music.cs
Я хочу сделать так чтобы мой контроллер переключал и останавливал музыку. Как это сделать?

**Очень просто!**

```C#
class Music : ICommandHandler
{
    public string commandName => "Music";
    public string commandDesc => "Переключает музыку на определенных частотах.";
    public int startHZ => 18000;
    public int endHZ => 18100;
    public int coolDown => 1;


    private MusicPlayer player = new MusicPlayer(); //[!code ++]
    //[!code ++] // *ЗДЕСЬ ДОЛЖНА БЫТЬ ВАША РЕАЛИЗАЦИЯ*

    public void Start(int hz)
    {
        if (hz >= 18090)
        {
            player.Stop(); //[!code ++] // Остановить.
            return;
        }

        if (hz >= 18050)
        {
            player.Up(); //[!code ++] // Следующий трек.
        }
        else
        {
            player.Down(); //[!code ++] // Предыдуший трек.
        }
    }
}
```
**И это все!** Вы создали контроллер который работает на частоте 18000 до 18100 герц.

1. Если герц ***18000-18050*** то будет играть предыдущий трек.
2. Если герц ***18050-18090*** то будет играть следующий трек.
3. Если герц ***18090-18100*** то трек остановиться.
::: tip
**commandName** у каждых команд должен быть разный.
:::

### Program.cs

```C#
var FFT = new FFT(18000, 20000);
FFT.Start();

Console.ReadLine();
```
И еще раз отметим радиус **принимаемых герц** (*от них зависит и сколько памяти будет занято*)
```C#
var FFT = new FFT(18000, 20000);
```

[Результат](https://youtu.be/e6tZEpmqnqw)
