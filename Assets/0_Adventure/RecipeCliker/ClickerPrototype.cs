using System;

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}

public record Inventory(int Log, int Sap, int Wood, int Iron, int Silver, int Alloy, int Artifact);
// ✨ HasEndingDeed 속성 추가
public record UpgradeState(int SellBonus, bool HasWoodRec = false, bool HasAlloyRec = false, bool HasArtifactRec = false, bool IsMineUnlocked = false, bool HasEndingDeed = false);
public record ClickerState(Inventory Inv, int Money, UpgradeState Upgrades);

public static class GameLogic
{
    public static ClickerState UpdateInv(ClickerState s, Func<Inventory, Inventory> updateFunc) => s with { Inv = updateFunc(s.Inv) };

    public static ClickerState GatherForest(ClickerState s, float matRoll, float woodChance, float sapChance)
    {
        if (matRoll <= woodChance) return UpdateInv(s, i => i with { Wood = i.Wood + 1 });
        if (matRoll <= woodChance + sapChance) return UpdateInv(s, i => i with { Sap = i.Sap + 1 });
        return UpdateInv(s, i => i with { Log = i.Log + 1 });
    }

    public static ClickerState RollForestRecipe(ClickerState s, float recRoll, float recChance) =>
        (recRoll <= recChance && !s.Upgrades.HasWoodRec) ? s with { Upgrades = s.Upgrades with { HasWoodRec = true } } : s;

    public static ClickerState GatherMine(ClickerState s, float matRoll, float alloyChance, float silverChance)
    {
        if (matRoll <= alloyChance) return UpdateInv(s, i => i with { Alloy = i.Alloy + 1 });
        if (matRoll <= alloyChance + silverChance) return UpdateInv(s, i => i with { Silver = i.Silver + 1 });
        return UpdateInv(s, i => i with { Iron = i.Iron + 1 });
    }

    public static ClickerState RollMineRecipe(ClickerState s, float recRoll, float recChance) =>
        (recRoll <= recChance && !s.Upgrades.HasAlloyRec) ? s with { Upgrades = s.Upgrades with { HasAlloyRec = true } } : s;

    public static bool CanCraftWood(ClickerState s) => s.Upgrades.HasWoodRec && s.Inv.Log >= 10 && s.Inv.Sap >= 3;
    public static bool CanCraftAlloy(ClickerState s) => s.Upgrades.HasAlloyRec && s.Inv.Iron >= 10 && s.Inv.Silver >= 3;
    public static bool CanCraftArtifact(ClickerState s) => s.Upgrades.HasArtifactRec && s.Inv.Wood >= 1 && s.Inv.Alloy >= 1;

    public static ClickerState CraftWood(ClickerState s) => CanCraftWood(s) ? UpdateInv(s, i => i with { Log = i.Log - 10, Sap = i.Sap - 3, Wood = i.Wood + 1 }) : s;
    public static ClickerState CraftAlloy(ClickerState s) => CanCraftAlloy(s) ? UpdateInv(s, i => i with { Iron = i.Iron - 10, Silver = i.Silver - 3, Alloy = i.Alloy + 1 }) : s;
    public static ClickerState CraftArtifact(ClickerState s) => CanCraftArtifact(s) ? UpdateInv(s, i => i with { Wood = i.Wood - 1, Alloy = i.Alloy - 1, Artifact = i.Artifact + 1 }) : s;

    public static ClickerState BuyUpgrade(ClickerState s, int cost, Func<UpgradeState, UpgradeState> upg) => s.Money >= cost ? s with { Money = s.Money - cost, Upgrades = upg(s.Upgrades) } : s;
    public static ClickerState Sell(ClickerState s, Func<Inventory, (Inventory next, int count)> sellFunc, int price) => sellFunc(s.Inv) switch { var (n, c) => s with { Inv = n, Money = s.Money + (c * price) } };
}