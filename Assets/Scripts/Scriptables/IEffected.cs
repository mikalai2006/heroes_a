using Cysharp.Threading.Tasks;

public interface IEffected
{
    UniTask RunHero(Player player, BaseEntity entity);
    // void RunOne(ref Player player, BaseEntity entity);
    // void RunEveryDay(ref Player player, BaseEntity entity);
    // void RunEveryWeek(ref Player player, BaseEntity entity);
}