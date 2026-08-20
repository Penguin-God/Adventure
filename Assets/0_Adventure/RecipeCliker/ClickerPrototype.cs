using System;

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}

public record Inventory(int I1, int I2, int I3, int I4, int I5, int I6, int I7, int I8, int I9, int I10);
// ✨ HasRec10 (궁극 레시피 획득 여부) 추가
public record UpgradeState(int Tier2Prob, int Tier3Prob, int SellBonus, bool HasRec8 = false, bool HasRec7 = false, bool HasRec9 = false, bool HasRec10 = false, bool IsMineUnlocked = false);
public record ClickerState(Inventory Inv, int Money, UpgradeState Upgrades);

public static class GameLogic
{
    public static ClickerState GatherForest(ClickerState s, float roll, float t2R, float t3R) => roll <= t3R ? s with { Inv = s.Inv with { I3 = s.Inv.I3 + 1 } } : roll <= t3R + t2R ? s with { Inv = s.Inv with { I2 = s.Inv.I2 + 1 } } : s with { Inv = s.Inv with { I1 = s.Inv.I1 + 1 } };
    public static ClickerState GatherMine(ClickerState s, float roll, float t2R, float t3R) => roll <= t3R ? s with { Inv = s.Inv with { I6 = s.Inv.I6 + 1 } } : roll <= t3R + t2R ? s with { Inv = s.Inv with { I5 = s.Inv.I5 + 1 } } : s with { Inv = s.Inv with { I4 = s.Inv.I4 + 1 } };

    public static ClickerState RollForestRecipes(ClickerState s, float r8, float p8, float r7, float p7) => s with { Upgrades = s.Upgrades with { HasRec8 = s.Upgrades.HasRec8 || r8 <= p8, HasRec7 = s.Upgrades.HasRec7 || r7 <= p7 } };
    // ✨ 광산 채굴 시 10번(궁극) 레시피도 주사위를 굴립니다!
    public static ClickerState RollMineRecipes(ClickerState s, float r9, float p9, float r10, float p10) => s with { Upgrades = s.Upgrades with { HasRec9 = s.Upgrades.HasRec9 || r9 <= p9, HasRec10 = s.Upgrades.HasRec10 || r10 <= p10 } };

    public static bool CanCraft8(ClickerState s) => s.Upgrades.HasRec8 && s.Inv.I1 >= 10 && s.Inv.I2 >= 3;
    public static bool CanCraft7(ClickerState s) => s.Upgrades.HasRec7 && s.Inv.I3 >= 1 && s.Inv.I4 >= 10 && s.Inv.I5 >= 3;
    public static bool CanCraft9(ClickerState s) => s.Upgrades.HasRec9 && s.Inv.I8 >= 1 && s.Inv.I6 >= 1;
    // ✨ 궁극 조합 10번은 이제 레시피(HasRec10)를 얻어야만 가능합니다.
    public static bool CanCraft10(ClickerState s) => s.Upgrades.HasRec10 && s.Inv.I7 >= 1 && s.Inv.I8 >= 1 && s.Inv.I9 >= 1;

    public static ClickerState Craft8(ClickerState s) => CanCraft8(s) ? s with { Inv = s.Inv with { I1 = s.Inv.I1 - 10, I2 = s.Inv.I2 - 3, I8 = s.Inv.I8 + 1 } } : s;
    public static ClickerState Craft7(ClickerState s) => CanCraft7(s) ? s with { Inv = s.Inv with { I3 = s.Inv.I3 - 1, I4 = s.Inv.I4 - 10, I5 = s.Inv.I5 - 3, I7 = s.Inv.I7 + 1 } } : s;
    public static ClickerState Craft9(ClickerState s) => CanCraft9(s) ? s with { Inv = s.Inv with { I8 = s.Inv.I8 - 1, I6 = s.Inv.I6 - 1, I9 = s.Inv.I9 + 1 } } : s;
    public static ClickerState Craft10(ClickerState s) => CanCraft10(s) ? s with { Inv = s.Inv with { I7 = s.Inv.I7 - 1, I8 = s.Inv.I8 - 1, I9 = s.Inv.I9 - 1, I10 = s.Inv.I10 + 1 } } : s;

    public static ClickerState BuyUpgrade(ClickerState s, int cost, Func<UpgradeState, UpgradeState> upg) => s.Money >= cost ? s with { Money = s.Money - cost, Upgrades = upg(s.Upgrades) } : s;
    public static ClickerState Sell(ClickerState s, Func<Inventory, (Inventory next, int count)> sellFunc, int price) => sellFunc(s.Inv) switch { var (n, c) => s with { Inv = n, Money = s.Money + (c * price) } };
}