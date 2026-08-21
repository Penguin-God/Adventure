using System;

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}

public record Inventory(int Log, int Sap, int Flower, int Wood, int Stone, int Iron, int Gold, int Alloy, int Artifact);

public record UpgradeState(
    int SellBonus,
    bool HasWoodPick = false, bool HasIronPick = false, bool HasShears = false,
    bool HasWoodRec = false, bool HasWoodPickRec = false,
    bool HasAlloyRec = false, bool HasIronPickRec = false, bool HasShearsRec = false,
    bool HasArtifactRec = false, bool HasEndingDeed = false
);

public record ClickerState(Inventory Inv, int Money, UpgradeState Upgrades);

public static class GameLogic
{
    public static ClickerState UpdateInv(ClickerState s, Func<Inventory, Inventory> updateFunc) => s with { Inv = updateFunc(s.Inv) };

    public static ClickerState GatherForest(ClickerState s, float matRoll, float sapCh, float flowerCh)
    {
        if (s.Upgrades.HasShears && matRoll <= flowerCh) return UpdateInv(s, i => i with { Flower = i.Flower + 1 });
        if (matRoll <= flowerCh + sapCh) return UpdateInv(s, i => i with { Sap = i.Sap + 1 });
        return UpdateInv(s, i => i with { Log = i.Log + 1 });
    }

    public static ClickerState RollForestRecipes(ClickerState s, float rWood, float cWood, float rPick, float cPick) =>
        s with
        {
            Upgrades = s.Upgrades with
            {
                HasWoodRec = s.Upgrades.HasWoodRec || rWood <= cWood,
                HasWoodPickRec = s.Upgrades.HasWoodPickRec || rPick <= cPick
            }
        };

    public static ClickerState GatherMine(ClickerState s, float matRoll, float ironCh, float goldCh)
    {
        if (!s.Upgrades.HasWoodPick && !s.Upgrades.HasIronPick) return s;
        if (s.Upgrades.HasIronPick && matRoll <= goldCh) return UpdateInv(s, i => i with { Gold = i.Gold + 1 });
        if (matRoll <= goldCh + ironCh) return UpdateInv(s, i => i with { Iron = i.Iron + 1 });
        return UpdateInv(s, i => i with { Stone = i.Stone + 1 });
    }

    public static ClickerState RollMineRecipes(ClickerState s, float rAlloy, float cAlloy, float rPick, float cPick, float rShears, float cShears) =>
        s with
        {
            Upgrades = s.Upgrades with
            {
                HasAlloyRec = s.Upgrades.HasAlloyRec || rAlloy <= cAlloy,
                HasIronPickRec = s.Upgrades.HasIronPickRec || rPick <= cPick,
                HasShearsRec = s.Upgrades.HasShearsRec || rShears <= cShears
            }
        };

    public static bool CanCraftWood(ClickerState s) => s.Upgrades.HasWoodRec && s.Inv.Log >= 10 && s.Inv.Sap >= 3;
    public static bool CanCraftAlloy(ClickerState s) => s.Upgrades.HasAlloyRec && s.Inv.Stone >= 10 && s.Inv.Iron >= 3;
    public static bool CanCraftArtifact(ClickerState s) => s.Upgrades.HasArtifactRec && s.Inv.Flower >= 1 && s.Inv.Gold >= 1;

    public static bool CanCraftWoodPick(ClickerState s) => s.Upgrades.HasWoodPickRec && !s.Upgrades.HasWoodPick && s.Inv.Wood >= 5;
    public static bool CanCraftIronPick(ClickerState s) => s.Upgrades.HasIronPickRec && s.Upgrades.HasWoodPick && !s.Upgrades.HasIronPick && s.Inv.Iron >= 10;

    // ✨ 원예 가위 요구량 변경: 철 5개, 통나무 10개
    public static bool CanCraftShears(ClickerState s) => s.Upgrades.HasShearsRec && !s.Upgrades.HasShears && s.Inv.Iron >= 5 && s.Inv.Log >= 10;

    public static ClickerState CraftWood(ClickerState s) => CanCraftWood(s) ? UpdateInv(s, i => i with { Log = i.Log - 10, Sap = i.Sap - 3, Wood = i.Wood + 1 }) : s;
    public static ClickerState CraftAlloy(ClickerState s) => CanCraftAlloy(s) ? UpdateInv(s, i => i with { Stone = i.Stone - 10, Iron = i.Iron - 3, Alloy = i.Alloy + 1 }) : s;
    public static ClickerState CraftArtifact(ClickerState s) => CanCraftArtifact(s) ? UpdateInv(s, i => i with { Flower = i.Flower - 1, Gold = i.Gold - 1, Artifact = i.Artifact + 1 }) : s;

    public static ClickerState CraftWoodPick(ClickerState s) => CanCraftWoodPick(s) ? UpdateInv(s, i => i with { Wood = i.Wood - 5 }) with { Upgrades = s.Upgrades with { HasWoodPick = true } } : s;
    public static ClickerState CraftIronPick(ClickerState s) => CanCraftIronPick(s) ? UpdateInv(s, i => i with { Iron = i.Iron - 10 }) with { Upgrades = s.Upgrades with { HasIronPick = true } } : s;

    // ✨ 원예 가위 차감량 변경
    public static ClickerState CraftShears(ClickerState s) => CanCraftShears(s) ? UpdateInv(s, i => i with { Iron = i.Iron - 5, Log = i.Log - 10 }) with { Upgrades = s.Upgrades with { HasShears = true } } : s;

    public static ClickerState BuyUpgrade(ClickerState s, int cost, Func<UpgradeState, UpgradeState> upg) => s.Money >= cost ? s with { Money = s.Money - cost, Upgrades = upg(s.Upgrades) } : s;
    public static ClickerState Sell(ClickerState s, Func<Inventory, (Inventory next, int count)> sellFunc, int price) => sellFunc(s.Inv) switch { var (n, c) => s with { Inv = n, Money = s.Money + (c * price) } };
}