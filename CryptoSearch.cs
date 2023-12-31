﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System;

namespace MyDev.BinanceApi
{
    public record CryptoSearch(string Symbol, decimal Price)
    {
        public static bool TryParse(string input, out CryptoSearch cryptoSearch)
        {
            cryptoSearch = default;
            var splitArray = input.Split(',', 2);
            if (splitArray.Length != 2) return false;
            var symbol = splitArray[0].Trim();
            if (!decimal.TryParse(splitArray[1], out var price)) return false;
            cryptoSearch = new CryptoSearch(symbol, price);
            return true;
        }

        public static async ValueTask<CryptoSearch?> BindAsync(HttpContext context,
       ParameterInfo parameter)
        {
            var input = context.GetRouteValue(parameter.Name!) as string ?? string.Empty;
            TryParse(input, out var cryptoSearch);
            return cryptoSearch;
        }   
    }

   
}
