namespace Sach.Models;

public static class Utils
{
    public static IHeroStatistics_HeroStats_HeroVsHeroMatchup_Advantage_With ToWith(this 
        IHeroStatistics_HeroStats_HeroVsHeroMatchup_Advantage_Vs vs)
    {
        return new HeroStatistics_HeroStats_HeroVsHeroMatchup_Advantage_With_HeroStatsHeroDryadType(vs.HeroId2,
            vs.Synergy);
    }
}