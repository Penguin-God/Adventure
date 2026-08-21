using System;

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}

public record Inventory(int Log, int Sap, int Wood, int Iron, int Silver, int Alloy, int Artifact);
public record UpgradeState(int SellBonus, bool HasWoodRec = false, bool HasAlloyRec = false, bool HasArtifactRec = false, bool IsMineUnlocked = false);
public record ClickerState(Inventory Inv, int Money, UpgradeState Upgrades);

public static class GameLogic
{
    // --- ⛏️ 채집 로직 (결과물까지 지형에서 드롭) ---
    public static ClickerState GatherForest(ClickerState s, float matRoll, float woodChance, float sapChance) =>
        matRoll <= woodChance ? s with { Inv = s.Inv with { Wood = s.Inv.Wood + 1 } } :
        matRoll <= woodChance + sapChance ? s with { Inv = s.Inv with { Sap = s.Inv.Sap + 1 } } :
        s with { Inv = s.Inv with { Log = s.Inv.Log + 1 } };

    public static ClickerState RollForestRecipe(ClickerState s, float recRoll, float recChance) =>
        (recRoll <= recChance && !s.Upgrades.HasWoodRec) ? s with { Upgrades = s.Upgrades with { HasWoodRec = true } } : s;

    public static ClickerState GatherMine(ClickerState s, float matRoll, float alloyChance, float silverChance) =>
        matRoll <= alloyChance ? s with { Inv = s.Inv with { Alloy = s.Inv.Alloy + 1 } } :
        matRoll <= alloyChance + silverChance ? s with { Inv = s.Inv with { Silver = s.Inv.Silver + 1 } } :
        s with { Inv = s.Inv with { Iron = s.Inv.Iron + 1 } };

    public static ClickerState RollMineRecipe(ClickerState s, float recRoll, float recChance) =>
        (recRoll <= recChance && !s.Upgrades.HasAlloyRec) ? s with { Upgrades = s.Upgrades with { HasAlloyRec = true } } : s;

    // --- 📜 레시피 조합 가능 여부 검사 (✨ 10개, 3개 조건 반영) ---
    public static bool CanCraftWood(ClickerState s) => s.Upgrades.HasWoodRec && s.Inv.Log >= 10 && s.Inv.Sap >= 3;
    public static bool CanCraftAlloy(ClickerState s) => s.Upgrades.HasAlloyRec && s.Inv.Iron >= 10 && s.Inv.Silver >= 3;
    public static bool CanCraftArtifact(ClickerState s) => s.Upgrades.HasArtifactRec && s.Inv.Wood >= 1 && s.Inv.Alloy >= 1;

    // --- 🔨 조합 실행 로직 (✨ 소모 개수 반영) ---
    public static ClickerState CraftWood(ClickerState s) => CanCraftWood(s) ? s with { Inv = s.Inv with { Log = s.Inv.Log - 10, Sap = s.Inv.Sap - 3, Wood = s.Inv.Wood + 1 } } : s;
    public static ClickerState CraftAlloy(ClickerState s) => CanCraftAlloy(s) ? s with { Inv = s.Inv with { Iron = s.Inv.Iron - 10, Silver = s.Inv.Silver - 3, Alloy = s.Inv.Alloy + 1 } } : s;
    public static ClickerState CraftArtifact(ClickerState s) => CanCraftArtifact(s) ? s with { Inv = s.Inv with { Wood = s.Inv.Wood - 1, Alloy = s.Inv.Alloy - 1, Artifact = s.Inv.Artifact + 1 } } : s;

    // --- 🛒 상점 구매 및 판매 ---
    public static ClickerState BuyUpgrade(ClickerState s, int cost, Func<UpgradeState, UpgradeState> upg) => s.Money >= cost ? s with { Money = s.Money - cost, Upgrades = upg(s.Upgrades) } : s;
    public static ClickerState Sell(ClickerState s, Func<Inventory, (Inventory next, int count)> sellFunc, int price) => sellFunc(s.Inv) switch { var (n, c) => s with { Inv = n, Money = s.Money + (c * price) } };
}