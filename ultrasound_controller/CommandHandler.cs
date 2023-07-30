using System.Reflection;

namespace UltraSoundController
{
    internal interface ICommandHandler
    {
        string commandName { get; }
        string commandDesc { get; }
        int startHZ { get; }
        int endHZ { get; }
        int coolDown { get; }
        void Start(int hz);
    }

    internal class CommandHandler
    {
        static public List<ICommandHandler> getAllSubClassOfICommandHandler()
        {
            var instances = from t in Assembly.GetExecutingAssembly().GetTypes()
                            where t.GetInterfaces().Contains(typeof(ICommandHandler))
                            select Activator.CreateInstance(t) as ICommandHandler;

            return instances.ToList();
        }

        static private Dictionary<string, bool> valuesCoolDown = new Dictionary<string, bool>();
        static public bool GetCoolDownByCMD(ICommandHandler cmd)
        {
            Thread.Sleep(1); //TODO

            if (!valuesCoolDown.ContainsKey(cmd.commandName))
            {
                return false;
            }

            return valuesCoolDown[cmd.commandName];
        }
        static public void ActivateCoolDownSec(ICommandHandler cmd, int sec)
        {
            new Task(() =>
            {
                valuesCoolDown[cmd.commandName] = true;
                Thread.Sleep(sec * 1000);
                valuesCoolDown[cmd.commandName] = false;
            }).Start();
        }

        static private Dictionary<string, byte> valuesFakeSignal = new Dictionary<string, byte>();
        static public void DestroyFakeSignal(ICommandHandler cmd)
        {
            if (valuesFakeSignal.ContainsKey(cmd.commandName))
            {
                valuesFakeSignal[cmd.commandName] += 1;

                if (valuesFakeSignal[cmd.commandName] == 1)
                {
                    new Task(() =>
                    {
                        Thread.Sleep(CONSTANTS.SAFE_MS);
                        valuesFakeSignal[cmd.commandName] = 0;
                    }).Start();
                }

                if (valuesFakeSignal[cmd.commandName] == CONSTANTS.SAFE_COUNT + 1)
                {
                    valuesFakeSignal[cmd.commandName] = 0;
                }
            }
            else
            {
                valuesFakeSignal[cmd.commandName] = 0;
            }
        }
        static public bool IsItTrue(ICommandHandler cmd)
        {
            if (valuesFakeSignal.ContainsKey(cmd.commandName) && valuesFakeSignal[cmd.commandName] == CONSTANTS.SAFE_COUNT)
            {
                return true;
            }

            return false;
        }
    }
}
