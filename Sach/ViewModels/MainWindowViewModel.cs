using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Collections;
using Avalonia.Media;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using ReactiveUI;
using Sach.Models;
using Sach.Views;
using SukiUI.Controls;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Sach.ViewModels;

public class MainWindowViewModel : ViewModelBase {
    public MainWindowViewModel() {
        OnHeroButtonClickCommand = ReactiveCommand.Create<Hero>(SetSelectedHeroId);
        OpenUrlCommand = ReactiveCommand.Create<string>(OpenUrl);
    }

    public static TimeSpan opacityTime { get; set; } = new TimeSpan(0, 0, 0, 2);

    private AvaloniaList<HeroButtonView> _heroesPreSearch;

    public AvaloniaList<HeroButtonView> HeroesPreSearch {
        get => _heroesPreSearch;
        set => this.RaiseAndSetIfChanged(ref _heroesPreSearch, value);
    }

    private AvaloniaList<HeroButtonView> _allHeroes = SetAllHeroes();

    public AvaloniaList<HeroButtonView> AllHeroes {
        get => _allHeroes;
        set => this.RaiseAndSetIfChanged(ref _allHeroes, value);
    }
    private static AvaloniaList<HeroButtonView> SetAllHeroes() {
        AvaloniaList<HeroButtonView> allHeroes = new AvaloniaList<HeroButtonView>();
        allHeroes.Add(new HeroButtonView {
            HeroId = 73,
            HeroName = "ALCHEMIST",
            HeroIcon = "avares://Sach/Assets/HeroIcons/alchemist_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 2,
            HeroName = "AXE",
            HeroIcon = "avares://Sach/Assets/HeroIcons/axe_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 99,
            HeroName = "BRISTLEBACK",
            HeroIcon = "avares://Sach/Assets/HeroIcons/bristleback_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 96,
            HeroName = "CENTAUR WARRUNNER",
            HeroIcon = "avares://Sach/Assets/HeroIcons/centaur_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 73,
            HeroName = "ALCHEMIST",
            HeroIcon = "avares://Sach/Assets/HeroIcons/alchemist_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 2,
            HeroName = "AXE",
            HeroIcon = "avares://Sach/Assets/HeroIcons/axe_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 99,
            HeroName = "BRISTLEBACK",
            HeroIcon = "avares://Sach/Assets/HeroIcons/bristleback_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 96,
            HeroName = "CENTAUR WARRUNNER",
            HeroIcon = "avares://Sach/Assets/HeroIcons/centaur_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 81,
            HeroName = "CHAOS KNIGHT",
            HeroIcon = "avares://Sach/Assets/HeroIcons/chaos_knight_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 135,
            HeroName = "DAWNBREAKER",
            HeroIcon = "avares://Sach/Assets/HeroIcons/dawnbreaker_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 69,
            HeroName = "DOOM",
            HeroIcon = "avares://Sach/Assets/HeroIcons/doom_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 49,
            HeroName = "DRAGON KNIGHT",
            HeroIcon = "avares://Sach/Assets/HeroIcons/dragon_knight_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 107,
            HeroName = "EARTH SPIRIT",
            HeroIcon = "avares://Sach/Assets/HeroIcons/earth_spirit_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 7,
            HeroName = "EARTHSHAKER",
            HeroIcon = "avares://Sach/Assets/HeroIcons/earthshaker_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 103,
            HeroName = "ELDER TITAN",
            HeroIcon = "avares://Sach/Assets/HeroIcons/elder_titan_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 59,
            HeroName = "HUSKAR",
            HeroIcon = "avares://Sach/Assets/HeroIcons/huskar_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 23,
            HeroName = "KUNNKA",
            HeroIcon = "avares://Sach/Assets/HeroIcons/kunkka_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 104,
            HeroName = "LEGION COMMANDER",
            HeroIcon = "avares://Sach/Assets/HeroIcons/legion_commander_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 54,
            HeroName = "LIFESTEALER",
            HeroIcon = "avares://Sach/Assets/HeroIcons/life_stealer_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 129,
            HeroName = "MARS",
            HeroIcon = "avares://Sach/Assets/HeroIcons/mars_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 60,
            HeroName = "NIGHT STALKER",
            HeroIcon = "avares://Sach/Assets/HeroIcons/night_stalker_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 84,
            HeroName = "OGRE MAGI",
            HeroIcon = "avares://Sach/Assets/HeroIcons/ogre_magi_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 57,
            HeroName = "OMNIKNIGHT",
            HeroIcon = "avares://Sach/Assets/HeroIcons/omniknight_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 137,
            HeroName = "PRIMAL BEAST",
            HeroIcon = "avares://Sach/Assets/HeroIcons/primal_beast_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 14,
            HeroName = "PUDGE",
            HeroIcon = "avares://Sach/Assets/HeroIcons/pudge_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 28,
            HeroName = "SLARDAR",
            HeroIcon = "avares://Sach/Assets/HeroIcons/slardar_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 71,
            HeroName = "SPIRIT BREAKER",
            HeroIcon = "avares://Sach/Assets/HeroIcons/spirit_breaker_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 18,
            HeroName = "SVEN",
            HeroIcon = "avares://Sach/Assets/HeroIcons/sven_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 29,
            HeroName = "TIDEHUNTER",
            HeroIcon = "avares://Sach/Assets/HeroIcons/tidehunter_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 19,
            HeroName = "TINY",
            HeroIcon = "avares://Sach/Assets/HeroIcons/tiny_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 83,
            HeroName = "TREANT PROTECTOR",
            HeroIcon = "avares://Sach/Assets/HeroIcons/treant_protector_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 100,
            HeroName = "TUSK",
            HeroIcon = "avares://Sach/Assets/HeroIcons/tusk_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 108,
            HeroName = "UNDERLORD",
            HeroIcon = "avares://Sach/Assets/HeroIcons/underlord_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 85,
            HeroName = "UNDYING",
            HeroIcon = "avares://Sach/Assets/HeroIcons/undying_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 42,
            HeroName = "WRAITH KING",
            HeroIcon = "avares://Sach/Assets/HeroIcons/wraith_king_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 1,
            HeroName = "ANTI-MAGE",
            HeroIcon = "avares://Sach/Assets/HeroIcons/antimage_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 113,
            HeroName = "ARC WARDEN",
            HeroIcon = "avares://Sach/Assets/HeroIcons/arc_warden_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 4,
            HeroName = "BLOODSEEKER",
            HeroIcon = "avares://Sach/Assets/HeroIcons/bloodseeker_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 62,
            HeroName = "BOUNTY HUNTER",
            HeroIcon = "avares://Sach/Assets/HeroIcons/bounty_hunter_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 56,
            HeroName = "CLINKZ",
            HeroIcon = "avares://Sach/Assets/HeroIcons/clinkz_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 6,
            HeroName = "DROW RANGER",
            HeroIcon = "avares://Sach/Assets/HeroIcons/drow_ranger_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 106,
            HeroName = "EMBER SPIRIT",
            HeroIcon = "avares://Sach/Assets/HeroIcons/ember_spirit_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 41,
            HeroName = "FACELESS VOID",
            HeroIcon = "avares://Sach/Assets/HeroIcons/faceless_void_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 72,
            HeroName = "GYROCOPTER",
            HeroIcon = "avares://Sach/Assets/HeroIcons/gyrocopter_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 123,
            HeroName = "HOODWINK",
            HeroIcon = "avares://Sach/Assets/HeroIcons/hoodwink_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 8,
            HeroName = "JUGGERNAUT",
            HeroIcon = "avares://Sach/Assets/HeroIcons/juggernaut_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 48,
            HeroName = "LUNA",
            HeroIcon = "avares://Sach/Assets/HeroIcons/luna_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 94,
            HeroName = "MEDUSA",
            HeroIcon = "avares://Sach/Assets/HeroIcons/medusa_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 82,
            HeroName = "MEEPO",
            HeroIcon = "avares://Sach/Assets/HeroIcons/meepo_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 114,
            HeroName = "MONKEY KING",
            HeroIcon = "avares://Sach/Assets/HeroIcons/monkey_king_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 10,
            HeroName = "MORPHLING",
            HeroIcon = "avares://Sach/Assets/HeroIcons/morphling_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 89,
            HeroName = "NAGA SIREN",
            HeroIcon = "avares://Sach/Assets/HeroIcons/naga_siren_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 44,
            HeroName = "PHANTOM ASSASSIN",
            HeroIcon = "avares://Sach/Assets/HeroIcons/phantom_assassin_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 12,
            HeroName = "PHANTOM LANCER",
            HeroIcon = "avares://Sach/Assets/HeroIcons/phantom_lancer_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 15,
            HeroName = "RAZOR",
            HeroIcon = "avares://Sach/Assets/HeroIcons/razor_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 32,
            HeroName = "RIKI",
            HeroIcon = "avares://Sach/Assets/HeroIcons/riki_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 11,
            HeroName = "SHADOW FIEND",
            HeroIcon = "avares://Sach/Assets/HeroIcons/nevermore_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 93,
            HeroName = "SLARK",
            HeroIcon = "avares://Sach/Assets/HeroIcons/slark_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 35,
            HeroName = "SNIPER",
            HeroIcon = "avares://Sach/Assets/HeroIcons/sniper_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 67,
            HeroName = "SPECTRE",
            HeroIcon = "avares://Sach/Assets/HeroIcons/spectre_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 46,
            HeroName = "TEMPLAR ASSASSIN",
            HeroIcon = "avares://Sach/Assets/HeroIcons/templar_assassin_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 109,
            HeroName = "TERRORBLADE",
            HeroIcon = "avares://Sach/Assets/HeroIcons/terrorblade_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 95,
            HeroName = "TROLL WARLORD",
            HeroIcon = "avares://Sach/Assets/HeroIcons/troll_warlord_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 70,
            HeroName = "URSA",
            HeroIcon = "avares://Sach/Assets/HeroIcons/ursa_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 47,
            HeroName = "VIPER",
            HeroIcon = "avares://Sach/Assets/HeroIcons/viper_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 63,
            HeroName = "WEAVER",
            HeroIcon = "avares://Sach/Assets/HeroIcons/weaver_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 68,
            HeroName = "ANCIENT APPARITION",
            HeroIcon = "avares://Sach/Assets/HeroIcons/ancient_apparition_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 5,
            HeroName = "CRYSTAL MAIDEN",
            HeroIcon = "avares://Sach/Assets/HeroIcons/crystal_maiden_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 43,
            HeroName = "DEATH PROPHET",
            HeroIcon = "avares://Sach/Assets/HeroIcons/death_prophet_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 87,
            HeroName = "DISRUPTOR",
            HeroIcon = "avares://Sach/Assets/HeroIcons/disruptor_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 58,
            HeroName = "ENCHANTRESS",
            HeroIcon = "avares://Sach/Assets/HeroIcons/enchantress_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 121,
            HeroName = "GRIMSTROKE",
            HeroIcon = "avares://Sach/Assets/HeroIcons/grimstroke_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 64,
            HeroName = "JAKIRO",
            HeroIcon = "avares://Sach/Assets/HeroIcons/jakiro_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 90,
            HeroName = "KEEPER OF THE LIGHT",
            HeroIcon = "avares://Sach/Assets/HeroIcons/keeper_of_the_light_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 52,
            HeroName = "LESHRAC",
            HeroIcon = "avares://Sach/Assets/HeroIcons/leshrac_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 31,
            HeroName = "LICH",
            HeroIcon = "avares://Sach/Assets/HeroIcons/lich_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 25,
            HeroName = "LINA",
            HeroIcon = "avares://Sach/Assets/HeroIcons/lina_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 26,
            HeroName = "LION",
            HeroIcon = "avares://Sach/Assets/HeroIcons/lion_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 138,
            HeroName = "MUERTA",
            HeroIcon = "avares://Sach/Assets/HeroIcons/muerta_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 53,
            HeroName = "NATURE'S PROPHET",
            HeroIcon = "avares://Sach/Assets/HeroIcons/furion_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 36,
            HeroName = "NECROPHOS",
            HeroIcon = "avares://Sach/Assets/HeroIcons/necrophos_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 111,
            HeroName = "ORACLE",
            HeroIcon = "avares://Sach/Assets/HeroIcons/oracle_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 76,
            HeroName = "OUTWORLD DESTROYER",
            HeroIcon = "avares://Sach/Assets/HeroIcons/obsidian_destroyer_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 13,
            HeroName = "PUCK",
            HeroIcon = "avares://Sach/Assets/HeroIcons/puck_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 45,
            HeroName = "PUGNA",
            HeroIcon = "avares://Sach/Assets/HeroIcons/pugna_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 39,
            HeroName = "QUEEN OF PAIN",
            HeroIcon = "avares://Sach/Assets/HeroIcons/queenofpain_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 86,
            HeroName = "RUBICK",
            HeroIcon = "avares://Sach/Assets/HeroIcons/rubick_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 79,
            HeroName = "SHADOW DEMON",
            HeroIcon = "avares://Sach/Assets/HeroIcons/shadow_demon_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 27,
            HeroName = "SHADOW SHAMAN",
            HeroIcon = "avares://Sach/Assets/HeroIcons/shadow_shaman_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 75,
            HeroName = "SILENCER",
            HeroIcon = "avares://Sach/Assets/HeroIcons/silencer_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 101,
            HeroName = "SKYWRATH MAGE",
            HeroIcon = "avares://Sach/Assets/HeroIcons/skywrath_mage_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 17,
            HeroName = "STORM SPIRIT",
            HeroIcon = "avares://Sach/Assets/HeroIcons/storm_spirit_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 34,
            HeroName = "TINKER",
            HeroIcon = "avares://Sach/Assets/HeroIcons/tinker_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 37,
            HeroName = "WARLOCK",
            HeroIcon = "avares://Sach/Assets/HeroIcons/warlock_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 30,
            HeroName = "WITCH DOCTOR",
            HeroIcon = "avares://Sach/Assets/HeroIcons/witch_doctor_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 22,
            HeroName = "ZEUS",
            HeroIcon = "avares://Sach/Assets/HeroIcons/zeus_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 102,
            HeroName = "ABBADON",
            HeroIcon = "avares://Sach/Assets/HeroIcons/abaddon_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 3,
            HeroName = "BANE",
            HeroIcon = "avares://Sach/Assets/HeroIcons/bane_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 65,
            HeroName = "BATRIDER",
            HeroIcon = "avares://Sach/Assets/HeroIcons/batrider_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 38,
            HeroName = "BEASTMASTER",
            HeroIcon = "avares://Sach/Assets/HeroIcons/beastmaster_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 78,
            HeroName = "BREWMASTER",
            HeroIcon = "avares://Sach/Assets/HeroIcons/brewmaster_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 61,
            HeroName = "BROODMOTHER",
            HeroIcon = "avares://Sach/Assets/HeroIcons/broodmother_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 66,
            HeroName = "CHEN",
            HeroIcon = "avares://Sach/Assets/HeroIcons/chen_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 51,
            HeroName = "CLOCKWERK",
            HeroIcon = "avares://Sach/Assets/HeroIcons/clockwerk_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 55,
            HeroName = "DARK SEER",
            HeroIcon = "avares://Sach/Assets/HeroIcons/dark_seer_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 119,
            HeroName = "DARK WILLOW",
            HeroIcon = "avares://Sach/Assets/HeroIcons/dark_willow_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 50,
            HeroName = "DAZZLE",
            HeroIcon = "avares://Sach/Assets/HeroIcons/dazzle_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 33,
            HeroName = "ENIGMA",
            HeroIcon = "avares://Sach/Assets/HeroIcons/enigma_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 74,
            HeroName = "INVOKER",
            HeroIcon = "avares://Sach/Assets/HeroIcons/invoker_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 91,
            HeroName = "IO",
            HeroIcon = "avares://Sach/Assets/HeroIcons/wisp_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 80,
            HeroName = "LONE DRUID",
            HeroIcon = "avares://Sach/Assets/HeroIcons/lone_druid_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 77,
            HeroName = "LYCAN",
            HeroIcon = "avares://Sach/Assets/HeroIcons/lycan_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 97,
            HeroName = "MAGNUS",
            HeroIcon = "avares://Sach/Assets/HeroIcons/magnus_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 136,
            HeroName = "MARCI",
            HeroIcon = "avares://Sach/Assets/HeroIcons/marci_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 9,
            HeroName = "MIRANA",
            HeroIcon = "avares://Sach/Assets/HeroIcons/mirana_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 88,
            HeroName = "NYX ASSASSIN",
            HeroIcon = "avares://Sach/Assets/HeroIcons/nyx_assassin_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 120,
            HeroName = "PANGOLIER",
            HeroIcon = "avares://Sach/Assets/HeroIcons/pangolier_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 110,
            HeroName = "PHOENIX",
            HeroIcon = "avares://Sach/Assets/HeroIcons/phoenix_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 16,
            HeroName = "SAND KING",
            HeroIcon = "avares://Sach/Assets/HeroIcons/sand_king_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 128,
            HeroName = "SNAPFIRE",
            HeroIcon = "avares://Sach/Assets/HeroIcons/snapfire_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 105,
            HeroName = "TECHIES",
            HeroIcon = "avares://Sach/Assets/HeroIcons/techies_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 98,
            HeroName = "TIMBERSAW",
            HeroIcon = "avares://Sach/Assets/HeroIcons/timbersaw_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 20,
            HeroName = "VENGEFUL SPIRIT",
            HeroIcon = "avares://Sach/Assets/HeroIcons/vengeful_spirit_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 40,
            HeroName = "VENOMANCER",
            HeroIcon = "avares://Sach/Assets/HeroIcons/venomancer_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 92,
            HeroName = "VISAGE",
            HeroIcon = "avares://Sach/Assets/HeroIcons/visage_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 126,
            HeroName = "VOID SPIRIT",
            HeroIcon = "avares://Sach/Assets/HeroIcons/void_spirit_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 21,
            HeroName = "WIND RANGER",
            HeroIcon = "avares://Sach/Assets/HeroIcons/wind_ranger_icon.png"
        });

        allHeroes.Add(new HeroButtonView {
            HeroId = 112,
            HeroName = "WINTER WYVERN",
            HeroIcon = "avares://Sach/Assets/HeroIcons/winter_wyvern_icon.png"
        });

        return allHeroes;
    }


    private AvaloniaList<Hero> _playerHeroes = ListInit();

    public AvaloniaList<Hero> PlayerHeroes {
        get => _playerHeroes;
        set => this.RaiseAndSetIfChanged(ref _playerHeroes, value);
    }

    private IStratzAPI? _stratzApi => App.Services?.GetRequiredService<IStratzAPI>();

    Dictionary<short, List<Vs>> _dict = new Dictionary<short, List<Vs>>();

    private bool _isVisibleLoading;

    public bool IsVisibleLoading
    {
        get => _isVisibleLoading;
        set => this.RaiseAndSetIfChanged(ref _isVisibleLoading, value);
    }
    
    private bool _isEnabledHeroView = true;

    public bool IsEnabledHeroView
    {
        get => _isEnabledHeroView;
        set => this.RaiseAndSetIfChanged(ref _isEnabledHeroView, value);
    }

    public async Task GetHeroStats()
    {
        IsVisibleLoading = true;
        IsEnabledHeroView = false;
        var operationResult = await _stratzApi.HeroStatistics.ExecuteAsync(SelectedHero.HeroId);
        if (operationResult.Data is null) {
            return;
        }

        // Определение статистики с учетом выбранной команды
        var data = new {
            HeroId = SelectedHero.HeroId,
            Stats = SelectedHero.IsAlly
                ? operationResult.Data.HeroStats?.HeroVsHeroMatchup?.Advantage?.FirstOrDefault()?.With.ToList()
                : SelectedHero.IsEnemy
                    ? operationResult.Data.HeroStats?.HeroVsHeroMatchup?.Advantage?.FirstOrDefault()?.Vs
                        .Select(x => x.ToWith()).ToList()
                    : null
        };

        // Обработка статистики и сортировка
        if (data.Stats != null && data.Stats.Any()) {
            var sortedStats = data.Stats
                .OrderByDescending(x => x.Synergy) // Сортировка по синергии
                .ToList();

            // Выполните дополнительные действия с отсортированными данными, если это необходимо

            await WriteObjectToFileJson(sortedStats, $"sorted_stats_{SelectedHero.HeroId}.json");


            Top10Heroes = await UpdateTop10Heroes(); // GetTop10HeroesFromJson("top_10_heroes.json");

            // Очистите данные после обработки
            _dict.Clear();
        }

        IsVisibleLoading = false;
        IsEnabledHeroView = true;
    }

    public List<With> Top10Heroes {
        get => _top10Heroes;
        set => this.RaiseAndSetIfChanged(ref _top10Heroes, value);
    }

    private async Task<List<With>> UpdateTop10Heroes() {
        var top10Heroes = await GetTopHeroes();
        await WriteObjectToFileJson(top10Heroes, "top_10_heroes.json");
        return top10Heroes;
    }

    private async Task WriteObjectToFileJson(object? o, String filePath) {
        var serializer = new JsonSerializer {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented,
        };

        await using var file = File.Create(filePath);
        await using var sw = new StreamWriter(file, Encoding.UTF8);
        await using JsonWriter writer = new JsonTextWriter(sw);

        serializer.Serialize(writer, o);
    }
    
    public static List<short> bannedHeroes = new List<short>();

    public async Task<List<With>> GetTopHeroes() {
        var selectedAllyHeroes = PlayerHeroes.Where(x => x.IsAlly).ToList();
        var selectedEnemyHeroes = PlayerHeroes.Where(x => x.IsEnemy).ToList();

        var topHeroes = new List<With>();

        // Получение статистики для героев союзной команды
        var allyHeroesStats = await GetHeroesStats(selectedAllyHeroes);

        // Получение статистики для героев вражеской команды
        var enemyHeroesStats = await GetHeroesStats(selectedEnemyHeroes);

        // Объединение статистики для героев обеих команд
        var allHeroesStats = allyHeroesStats.Concat(enemyHeroesStats).ToList();

        // Сортировка статистики по синергии или противодействию
        var sortedStats = allHeroesStats
            .OrderByDescending(x => x.Synergy) // Сортировка по синергии
            .ToList();

        // Выбор 10 наилучших героев
        var selectedAllyHeroIds = selectedAllyHeroes.Select(x => x.HeroId);
        var selectedEnemyHeroIds = selectedEnemyHeroes.Select(x => x.HeroId);
        var selectedHeroIds = selectedAllyHeroIds.Concat(selectedEnemyHeroIds);
        var bannedHeroIds = bannedHeroes;
        var bannedAndSelectedHeroIds = selectedHeroIds.Concat(bannedHeroIds);
        topHeroes = sortedStats.Where(x => !bannedAndSelectedHeroIds.Contains(x.HeroId2)).Take(5).ToList();

        return topHeroes;
    }

    private async Task<List<With>> GetHeroesStats(List<Hero> heroes) {
        var heroesStats = new List<With>();

        foreach (var hero in heroes) {
            var operationResult = await _stratzApi.HeroStatistics.ExecuteAsync(hero.HeroId);
            if (operationResult.Data is not null) {
                var stats = hero.IsAlly
                    ? operationResult.Data.HeroStats?.HeroVsHeroMatchup?.Advantage?.FirstOrDefault()?.With.ToList()
                    : hero.IsEnemy
                        ? operationResult.Data.HeroStats?.HeroVsHeroMatchup?.Advantage?.FirstOrDefault()?.Vs
                            .Select(x => x.ToWith()).ToList()
                        : null;

                if (stats != null && stats.Any()) {
                    heroesStats.AddRange(stats.Select(x => new With(x)));
                }
            }
        }

        return heroesStats;
    }

    private Hero? _selectedHero;

    public Hero? SelectedHero {
        get => _selectedHero;
        set => this.RaiseAndSetIfChanged(ref _selectedHero, value);
    }

    public ReactiveCommand<Hero, Unit> OnHeroButtonClickCommand { get; set; }
    public ReactiveCommand<string, Unit> OpenUrlCommand { get; set; }

    private async void SetSelectedHeroId(Hero hero) {
        if (SelectedHero is null) {
            return;
        }

        SelectedHero.HeroId = hero.HeroId;
        SelectedHero.HeroName = hero.HeroName;
        SelectedHero.HeroIconPath = hero.HeroIconPath;
        await GetHeroStats();
    }

    private IBrush _playerHero;

    private List<With> _top10Heroes;

    private static AvaloniaList<Hero> ListInit() {
        var list = new AvaloniaList<Hero>();
        for (int i = 0; i < 5; i++) {
            list.Add(new Hero() {
                CurrentTeam = Hero.Team.Ally
            });
        }

        for (int i = 0; i < 5; i++) {
            list.Add(new Hero() {
                CurrentTeam = Hero.Team.Enemy
            });
        }

        return list;
    }

    private void OpenUrl(object urlObj) {
        var url = urlObj as string;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
            using var proc = new Process { StartInfo = { UseShellExecute = true, FileName = url } };
            proc.Start();

            return;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
            Process.Start("xdg-open", url);
            return;
        }

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) throw new ArgumentException("invalid url: " + url);
        Process.Start("open", url);
        return;
    }
}