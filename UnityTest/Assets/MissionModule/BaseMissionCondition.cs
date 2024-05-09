namespace MissionModule
{
    public abstract class BaseMissionCondition
    {
        public GameMission mission;
        
        public abstract bool IsFinish();
        public abstract T CurrentValue<T>();
        public abstract T TargetValue<T>();
    }
}