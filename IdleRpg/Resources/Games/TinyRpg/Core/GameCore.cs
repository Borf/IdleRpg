namespace TinyRpg.Core;

using IdleRpg.Game;
using IdleRpg.Game.Attributes;
using IdleRpg.Game.Core;
using IdleRpg.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TinyRpg.Items;
using TinyRpg.Maps;
using TinyRpg.Monsters;

public class GameCore : IGameCore2D, IDiscordGame
{
    public Type GetStats() => typeof(Stats);
    public Type GetItemIdEnum() => typeof(ItemIds);
    public Type GetEquipEnum() => typeof(EquipSlots);
    public Enum[] GetMoneyTypes() => [ Stats.Money ];

    public StatModifier CalculateStat(Enum s)
    {
        Stats stat = (Stats)s;
        return stat switch
        {
            Stats.LookWeapon => new FixedStatModifier(stat, 0),
            Stats.LookPants => new FixedStatModifier(stat, 0),
            Stats.LookShirt => new FixedStatModifier(stat, 0),

            Stats.LookBody => new FixedStatModifier(stat, 0),
            Stats.LookFace => new FixedStatModifier(stat, 0),
            Stats.LookHair => new FixedStatModifier(stat, 0),

            Stats.Money => new FixedStatModifier(stat, 0),
            Stats.TotalExp => new FixedStatModifier(stat, 0),
            Stats.Hp => new FixedStatModifier(stat, 1),
            Stats.Sp => new FixedStatModifier(stat, 1),
            Stats.Level => new FixedStatModifier(stat, 1),
            Stats.Exp =>
                new StatModifier
                {
                    Stat = stat,
                    StatsUsed = [Stats.Level, Stats.TotalExp],
                    Calculation = (currentStats) =>
                    {

                        return currentStats[Stats.TotalExp] - ExpNeededToLevel((int)currentStats[Stats.Level]);
                    },
                },
            Stats.LevelExp =>
                new StatModifier
                {
                    Stat = stat,
                    StatsUsed = [Stats.Level],
                    Calculation = (currentStats) =>
                    {
                        return ExpNeededPerLevel[(int)currentStats[Stats.Level]-1];
                    },
                },
            Stats.Attack =>
                new StatModifier
                {
                    Stat = stat,
                    StatsUsed = [Stats.Level],
                    Calculation = (currentStats) => currentStats[Stats.Level],
                },
            Stats.Defense =>
                new StatModifier
                {
                    Stat = stat,
                    StatsUsed = [Stats.Level],
                    Calculation = (currentStats) => currentStats[Stats.Level],
                },
            Stats.MaxHp =>
                new StatModifier
                {
                    Stat = stat,
                    StatsUsed = [Stats.Level],
                    Calculation = (currentStats) => 5 * currentStats[Stats.Level],
                },
            Stats.MaxSp =>
                new StatModifier
                {
                    Stat = stat,
                    StatsUsed = [Stats.Level],
                    Calculation = (currentStats) => 3 * currentStats[Stats.Level],
                },
            Stats.Dodge =>
                new StatModifier
                {
                    Stat = stat,
                    StatsUsed = [Stats.Level],
                    Calculation = (currentStats) => 5 * currentStats[Stats.Level],
                },
            Stats.Accuracy =>
                new StatModifier
                {
                    Stat = stat,
                    StatsUsed = [Stats.Level],
                    Calculation = (currentStats) => 5 * currentStats[Stats.Level],
                },
            _ => throw new Exception("Requested stat that is not implemented: " + stat.ToString()),
        };
    }
    public List<Map> LoadMaps()
    { 
        return new List<Map>()
        {
            new WorldMap()
        };
    }


    public (Point position, string mapName) SpawnLocation => (new Point(747, 707), nameof(WorldMap));

    public void Damage(Character source, Character target, IDamageProperties damageProperties)
    {
        var properties = damageProperties as DamageProperties ?? throw new Exception();
        target.Stats[Stats.Hp] = Math.Max(0, target.Stats[Stats.Hp] - properties.Damage);
    }
    public bool IsAlive(Character character) => character.Stats[Stats.Hp] > 0;

    public void GainExp(Character character, IMonsterTemplate npcTemplate)
    {
        if (npcTemplate is Mob mob)
        {
            var exp = mob.Exp;
            var player = character as CharacterPlayer;
            player?.Log(LogCategory.Character, $"You gained {exp} experience");
            character.Stats[Stats.TotalExp] += exp; //npcTemplate's Exp
            while(character.Stats[Stats.TotalExp] >= ExpNeededToLevel((int)character.Stats[Stats.Level]+1))
            {
                character.Stats[Stats.Level]++; //levelup!
                character.CalculateStats();
                player?.Log(LogCategory.Character, $"You leveled up! You are now level {character.Stats[Stats.Level]}");
            }

        }
    }



    public IImageGenerator<CharacterPlayer, SpriteDirection> MapCharacterGenerator => new CharacterGenerator(); //would love to be able to do dependency injection here
    public IImageGenerator<ICharacterCreateCharOptions> CharCreateCharacterGenerator => new CharacterGenerator(); //would love to be able to do dependency injection here

    List<long> ExpNeededPerLevel = [
    10, 20, 40, 70, 110, 160, 230, 320, 430, 560, 710, 880, 1070, 1280, 1510, 1760,
    2030, 2320, 2630, 2960, 3310, 3680, 4070, 4480, 4910, 5360, 5830, 6320, 6830, 7360,
    7910, 8480, 9070, 9680, 10310, 10960, 11630, 12320, 13030, 13760, 14510, 15280, 16070, 16880, 17710, 18560, 19430, 20320, 21230, 22160,
    23110, 24080, 25070, 26080, 27110, 28160, 29230, 30320, 31430, 32560, 33710, 34880, 36070, 37280, 38510, 39760,
    41030, 42320, 43630, 44960, 46310, 47680, 49070, 50480, 51910, 53360, 54830, 56320, 57830, 59360, 60910, 62480,
    64070, 65680, 67310, 68960, 70630, 72320, 74030, 75760, 77510, 79280, 81070, 82880, 84710, 86560, 88430, 90320,
    92230, 94160, 96110, 98100, 100120, 102160, 104230, 106320, 108430, 110560, 112710, 114880, 117070, 119280, 121510, 123760,
    126030, 128320, 130630, 132960, 135310, 137680, 140070, 142480, 144910, 147360, 149830, 152320, 154830, 157360, 159910, 162480,
    165070, 167680, 170310, 172960, 175630, 178320, 181030, 183760, 186510, 189280, 192070, 194880, 197710, 200560, 203430, 206320,
    209230, 212160, 215110, 218080, 221070, 224080, 227110, 230160, 233230, 236320, 239430, 242560, 245710, 248880, 252070, 255280,
    258510, 261760, 265030, 268320, 271630, 274960, 278310, 281680, 285070, 288480, 291910, 295360, 298830, 302320, 305830, 309360,
    312910, 316480, 320070, 323680, 327310, 330960, 334630, 338320, 342030, 345760, 349510, 353280, 357070, 360880, 364710, 368560,
    372430, 376320, 380230, 384160, 388110, 392080, 396070, 400080, 404110, 408160, 412230, 416320, 420430, 424560, 428710, 432880,
    437070, 441280, 445510, 449760, 454030, 458320, 462630, 466960, 471310, 475680, 480070, 484480, 488910, 493360, 497830, 502320,
    506830, 511360, 515910, 520480, 525070, 529680, 534310, 538960, 543630, 548320, 553030, 557760, 562510, 567280, 572070, 576880,
    581710, 586560, 591430, 596320, 601230, 606160, 611110, 616080, 621070, 626080, 631110, 636160, 641230, 646320, 651430, 656560
    ];

    long ExpNeededToLevel(int level) => ExpNeededPerLevel.Take(level - 1).Sum(u => u);


    public IImageGenerator<DiscordMenu, Character> HeaderGenerator => new DiscordHeaderGenerator();
    public ICharacterCreateCharOptions GetNewCharacterOptions() => new CharacterCreateCharOptions();

    public void InitializeCharacter(CharacterPlayer newCharacter, ICharacterCreateCharOptions options)
    {
        var charOptions = (CharacterCreateCharOptions)options;
        newCharacter.Stats[Stats.LookBody] = charOptions.BodyId;
        newCharacter.Stats[Stats.LookFace] = charOptions.HeadId;
        newCharacter.Stats[Stats.LookHair] = charOptions.HairId;
    }
}




// How do we determine death condition?
// How do we handle multiple exp systems?
public enum Stats
{
    [NotCalculated, Group("Looks"), Hidden]
    LookBody,
    [NotCalculated, Group("Looks"), Hidden]
    LookFace,
    [NotCalculated, Group("Looks"), Hidden]
    LookHair,

    [Group("Looks"), Hidden]
    LookPants,
    [ Group("Looks"), Hidden]
    LookShirt,
    [Group("Looks"), Hidden]
    LookWeapon,

    [NotCalculated, Group("Core")]
    Level, //could be calculated from TotalExp
    [Group("Core")]
    Exp,
    [Group("Core", nameof(Exp))]
    LevelExp,
    [NotCalculated, Group("Core"), Hidden]
    TotalExp,

    [Group("Base")]
    Attack,
    [Group("Base")]
    Defense,
    [Group("Base")]
    Dodge,
    [Group("Base")]
    Accuracy,
    
    [NotCalculated, Group("Core")]
    Hp,
    [Group("Core", nameof(Hp))]
    MaxHp,
    [NotCalculated, Group("Core")]
    Sp,
    [Group("Core", nameof(Sp))]
    MaxSp,

    [NotCalculated, Group("Core"), Hidden]
    Money
}


public enum Jobs
{
    Warrior,
    Mage,
    Archer,
    Healer,
}

[Flags]
public enum EquipSlots
{
    Weapon = 1<<0,
    Armor = 1<<1,
    Helm = 1<<2,
}

public enum ItemTypes
{
    Equip,
    Usable,
    Misc,
    //TODO: .... more types? how will we filter in UI? maybe combobox with multiselect?
}

public class DamageProperties : IDamageProperties
{
    public int Damage { get; set; }

    public override string? ToString()
    {
        return Damage + "dmg";
    }
}


//TODO: change this to use attributes instead of a dictionary as we need reflection anyway
public class CharacterCreateCharOptions : ICharacterCreateCharOptions
{
    public int BodyId { get; set; } = 1;
    public int HeadId { get; set; } = 1;
    public int HairId { get; set; } = 1;
    public Dictionary<string, CharacterCreateCharOption> Options => new()
    {
        { 
            nameof(BodyId), new CharacterCreateCharOption()
            {
                Minvalue = 1,
                MaxValue = 1,
                Name = "Body Style"
            }
        },
        {
            nameof(HeadId), new CharacterCreateCharOption()
            {
                Minvalue = 1,
                MaxValue = 4,
                Name = "Face"
            }
        },        
        {
            nameof(HairId), new CharacterCreateCharOption()
            {
                Minvalue = 1,
                MaxValue = 2,
                Name = "Hair Style"
            }
        }


    };
}