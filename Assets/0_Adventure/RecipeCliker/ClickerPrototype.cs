using System;

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}

public record TerrainState(int Tier1, int Tier2, int Tier3);
public record UpgradeState(int Tier2Prob, int Tier3Prob, int SellBonus, bool HasRec8 = false, bool HasRec7 = false, bool HasRec9 = false, bool IsMineUnlocked = false);
public record ClickerState(Inventory Inv, int Money, UpgradeState Upgrades);
public record Inventory(int I1, int I2, int I3, int I4, int I5, int I6, int I7, int I8, int I9, int I10);

public static class GameLogic
{
    // --- ⛏️ 채집 로직 (T3=희귀, T2=중간, T1=일반) ---
    public static ClickerState GatherForest(ClickerState s, float roll, float t2R, float t3R) => roll <= t3R ? s with { Inv = s.Inv with { I3 = s.Inv.I3 + 1 } } : roll <= t3R + t2R ? s with { Inv = s.Inv with { I2 = s.Inv.I2 + 1 } } : s with { Inv = s.Inv with { I1 = s.Inv.I1 + 1 } };
    public static ClickerState GatherMine(ClickerState s, float roll, float t2R, float t3R) => roll <= t3R ? s with { Inv = s.Inv with { I6 = s.Inv.I6 + 1 } } : roll <= t3R + t2R ? s with { Inv = s.Inv with { I5 = s.Inv.I5 + 1 } } : s with { Inv = s.Inv with { I4 = s.Inv.I4 + 1 } };

    // --- 📜 조합 가능 여부 검사 로직 ---
    public static bool CanCraft8(ClickerState s) => s.Upgrades.HasRec8 && s.Inv.I1 >= 10 && s.Inv.I2 >= 3;
    public static bool CanCraft7(ClickerState s) => s.Upgrades.HasRec7 && s.Inv.I3 >= 1 && s.Inv.I4 >= 10 && s.Inv.I5 >= 3;
    public static bool CanCraft9(ClickerState s) => s.Upgrades.HasRec9 && s.Inv.I8 >= 1 && s.Inv.I6 >= 1;
    public static bool CanCraft10(ClickerState s) => s.Inv.I7 >= 1 && s.Inv.I8 >= 1 && s.Inv.I9 >= 1;

    // --- 🔨 조합 실행 로직 (한 줄 마법) ---
    public static ClickerState Craft8(ClickerState s) => CanCraft8(s) ? s with { Inv = s.Inv with { I1 = s.Inv.I1 - 10, I2 = s.Inv.I2 - 3, I8 = s.Inv.I8 + 1 } } : s;
    public static ClickerState Craft7(ClickerState s) => CanCraft7(s) ? s with { Inv = s.Inv with { I3 = s.Inv.I3 - 1, I4 = s.Inv.I4 - 10, I5 = s.Inv.I5 - 3, I7 = s.Inv.I7 + 1 } } : s;
    public static ClickerState Craft9(ClickerState s) => CanCraft9(s) ? s with { Inv = s.Inv with { I8 = s.Inv.I8 - 1, I6 = s.Inv.I6 - 1, I9 = s.Inv.I9 + 1 } } : s;
    public static ClickerState Craft10(ClickerState s) => CanCraft10(s) ? s with { Inv = s.Inv with { I7 = s.Inv.I7 - 1, I8 = s.Inv.I8 - 1, I9 = s.Inv.I9 - 1, I10 = s.Inv.I10 + 1 } } : s;

    // --- 🛒 상점 및 판매 고차 함수 ---
    public static ClickerState BuyUpgrade(ClickerState s, int cost, Func<UpgradeState, UpgradeState> upg) => s.Money >= cost ? s with { Money = s.Money - cost, Upgrades = upg(s.Upgrades) } : s;

    // ✨ 극한의 한 줄 처리: 람다식으로 인벤토리 분해 방식을 던져주면, 알아서 상태를 갈아끼우고 돈을 더합니다!
    public static ClickerState Sell(ClickerState s, Func<Inventory, (Inventory next, int count)> sellFunc, int price) =>
        sellFunc(s.Inv) switch { var (n, c) => s with { Inv = n, Money = s.Money + (c * price) } };
}