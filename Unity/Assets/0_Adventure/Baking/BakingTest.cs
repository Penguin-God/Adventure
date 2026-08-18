using System.Collections.Generic;
using UnityEngine;

public class BakingTest : MonoBehaviour
{
    void Start()
    {
        // 1. 레시피 세팅 
        var breadRecipe = new Recipe("Perfect Bread", new HashSet<Ingredient>
        {
            new Ingredient("Flour", "Raw"),
            new Ingredient("Water", "Boiled"),
            new Ingredient("Yeast", "Fermented")
        });

        var recipes = new List<Recipe> { breadRecipe };

        // 2. 초기 재료 준비 (상태는 기본값인 "Raw")
        var flour = new Ingredient("Flour");
        var water = new Ingredient("Water");
        var yeast = new Ingredient("Yeast");

        // 3. 함수형 조합 (불변성 유지)
        // water와 yeast 변수 자체는 변하지 않으며, 새로운 가공 상태의 객체가 반환됩니다.
        var boiledWater = Processor.ApplyHeat(water);
        var fermentedYeast = Processor.Ferment(yeast);

        // 섞기 (순서가 상관없는 Dough 객체 생성)
        var dough = Dough.Mix(flour, boiledWater, fermentedYeast);

        // 4. 오븐 굽기 결과 도출
        string result = Oven.Bake(dough, recipes);

        Debug.Log($"Baking Result: {result}");
        // 출력: Baking Result: Perfect Bread
    }
}