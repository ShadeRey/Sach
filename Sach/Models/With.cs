namespace Sach.Models;

public class With
{
    public short HeroId2 { get; set; }
    public decimal Synergy { get; set; }

    public With(IHeroStatistics_HeroStats_HeroVsHeroMatchup_Advantage_With with)
    {
        HeroId2 = with.HeroId2 ?? 0;
        Synergy = with.Synergy ?? 0;
    }

    public With() { }
}