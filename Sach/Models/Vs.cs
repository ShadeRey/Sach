namespace Sach.Models;

public class Vs
{
    public short HeroId2 { get; set; }
    public decimal Synergy { get; set; }

    public Vs(IHeroStatistics_HeroStats_HeroVsHeroMatchup_Advantage_Vs vs)
    {
        HeroId2 = vs.HeroId2 ?? 0;
        Synergy = vs.Synergy ?? 0;
    }
}