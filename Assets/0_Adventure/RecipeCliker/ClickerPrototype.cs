using System;

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}

public record Inventory(int Log, int Sap, int Flower, int Wood, int Iron, int Silver, int Gold, int Alloy, int Artifact);

// ✨ 도구 보유, 레시피 보유, 그리고 영주의 땅문서까지 완벽 통합
public record UpgradeState(
    int SellBonus,
    bool HasStonePick = false, bool HasIronPick = false, bool HasShears = false,
    bool HasWoodRec = false, bool HasStonePickRec = false,
    bool HasAlloyRec = false, bool HasIronPickRec = false, bool HasShearsRec = false,
    bool HasArtifactRec = false, bool HasEndingDeed = false
);

public record ClickerState(Inventory Inv, int Money, UpgradeState Upgrades);

public static class GameLogic
{
    public static ClickerState UpdateInv(ClickerState s, Func<Inventory, Inventory> updateFunc) => s with { Inv = updateFunc(s.Inv) };

    // --- ⛏️ 숲 채집 (원예 가위가 있으면 꽃 해금) ---
    public static ClickerState GatherForest(ClickerState s, float matRoll, float woodCh, float sapCh, float flowerCh)
    {
        if (s.Upgrades.HasShears && matRoll <= flowerCh) return UpdateInv(s, i => i with { Flower = i.Flower + 1 });
        if (matRoll <= flowerCh + woodCh) return UpdateInv(s, i => i with { Wood = i.Wood + 1 });
        if (matRoll <= flowerCh + woodCh + sapCh) return UpdateInv(s, i => i with { Sap = i.Sap + 1 });
        return UpdateInv(s, i => i with { Log = i.Log + 1 });
    }

    public static ClickerState RollForestRecipes(ClickerState s, float rWood, float cWood, float rPick, float cPick) =>
        s with
        {
            Upgrades = s.Upgrades with
            {
                HasWoodRec = s.Upgrades.HasWoodRec || rWood <= cWood,
                HasStonePickRec = s.Upgrades.HasStonePickRec || rPick <= cPick
            }
        };

    // --- ⛏️ 광산 채집 (철 곡괭이가 있으면 금광석 해금) ---
    public static ClickerState GatherMine(ClickerState s, float matRoll, float alloyCh, float silverCh, float goldCh)
    {
        if (s.Upgrades.HasIronPick && matRoll <= goldCh) return UpdateInv(s, i => i with { Gold = i.Gold + 1 });
        if (matRoll <= goldCh + alloyCh) return UpdateInv(s, i => i with { Alloy = i.Alloy + 1 });
        if (matRoll <= goldCh + alloyCh + silverCh) return UpdateInv(s, i => i with { Silver = i.Silver + 1 });
        return UpdateInv(s, i => i with { Iron = i.Iron + 1 });
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
    public static bool CanCraftAlloy(ClickerState s) => s.Upgrades.HasAlloyRec && s.Inv.Iron >= 10 && s.Inv.Silver >= 3;
    public static bool CanCraftArtifact(ClickerState s) => s.Upgrades.HasArtifactRec && s.Inv.Flower >= 1 && s.Inv.Gold >= 1;

    public static bool CanCraftStonePick(ClickerState s) => s.Upgrades.HasStonePickRec && !s.Upgrades.HasStonePick && s.Inv.Log >= 20 && s.Inv.Wood >= 2;
    public static bool CanCraftIronPick(ClickerState s) => s.Upgrades.HasIronPickRec && !s.Upgrades.HasIronPick && s.Inv.Iron >= 20 && s.Inv.Alloy >= 2;
    public static bool CanCraftShears(ClickerState s) => s.Upgrades.HasShearsRec && !s.Upgrades.HasShears && s.Inv.Silver >= 20 && s.Inv.Alloy >= 1;

    public static ClickerState CraftWood(ClickerState s) => CanCraftWood(s) ? UpdateInv(s, i => i with { Log = i.Log - 10, Sap = i.Sap - 3, Wood = i.Wood + 1 }) : s;
    public static ClickerState CraftAlloy(ClickerState s) => CanCraftAlloy(s) ? UpdateInv(s, i => i with { Iron = i.Iron - 10, Silver = i.Silver - 3, Alloy = i.Alloy + 1 }) : s;
    public static ClickerState CraftArtifact(ClickerState s) => CanCraftArtifact(s) ? UpdateInv(s, i => i with { Flower = i.Flower - 1, Gold = i.Gold - 1, Artifact = i.Artifact + 1 }) : s;

    public static ClickerState CraftStonePick(ClickerState s) => CanCraftStonePick(s) ? s with { Inv = s.Inv with { Log = s.Inv.Log - 20, Wood = s.Inv.Wood - 2 }, Upgrades = s.Upgrades with { HasStonePick = true } } : s;
    public static ClickerState CraftIronPick(ClickerState s) => CanCraftIronPick(s) ? s with { Inv = s.Inv with { Iron = s.Inv.Iron - 20, Alloy = s.Inv.Alloy - 2 }, Upgrades = s.Upgrades with { HasIronPick = true } } : s;
    public static ClickerState CraftShears(ClickerState s) => CanCraftShears(s) ? s with { Inv = s.Inv with { Silver = s.Inv.Silver - 20, Alloy = s.Inv.Alloy - 1 }, Upgrades = s.Upgrades with { HasShears = true } } : s;

    public static ClickerState BuyUpgrade(ClickerState s, int cost, Func<UpgradeState, UpgradeState> upg) => s.Money >= cost ? s with { Money = s.Money - cost, Upgrades = upg(s.Upgrades) } : s;
    public static ClickerState Sell(ClickerState s, Func<Inventory, (Inventory next, int count)> sellFunc, int price) => sellFunc(s.Inv) switch { var (n, c) => s with { Inv = n, Money = s.Money + (c * price) } };
}