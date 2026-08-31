using UnityEngine;
using System;
using System.Collections.Generic;
using GatchTycoon.Domain;

namespace GatchTycoon.Managers
{
    public class CurrencyManager : MonoBehaviour
    {
        public static CurrencyManager Instance { get; private set; }
        
        private Dictionary<CurrencyType, int> _currencies = new Dictionary<CurrencyType, int>();
        
        public Action<CurrencyType, int> OnCurrencyChanged;
        
        void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
            
            _currencies[CurrencyType.Gold] = 1000;
            _currencies[CurrencyType.Town2Currency] = 0;
        }
        
        public int GetCurrency(CurrencyType type) => _currencies.ContainsKey(type) ? _currencies[type] : 0;
        
        public bool HasEnough(CurrencyType type, int amount) => GetCurrency(type) >= amount;
        
        public void AddCurrency(CurrencyType type, int amount)
        {
            if (!_currencies.ContainsKey(type)) _currencies[type] = 0;
            _currencies[type] += amount;
            OnCurrencyChanged?.Invoke(type, _currencies[type]);
        }
        
        public bool SpendCurrency(CurrencyType type, int amount)
        {
            if (HasEnough(type, amount))
            {
                _currencies[type] -= amount;
                OnCurrencyChanged?.Invoke(type, _currencies[type]);
                return true;
            }
            return false;
        }
    }
}
