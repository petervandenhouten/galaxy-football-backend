
namespace GalaxyFootball.Application.Scripts
{
    public abstract class BaseScript
    {
        //protected readonly Game m_game;

        public BaseScript()
        {
            
        }
        abstract public void Run();
        abstract public bool CanRun();

    }
}
