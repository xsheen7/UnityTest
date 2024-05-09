using GameFramework;

namespace MissionModule
{
    public abstract class BaseMissionReward
    {
        public GameMission gameMission;
        
        public abstract T GetRewardItems<T>();
    }
}