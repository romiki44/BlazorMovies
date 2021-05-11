﻿using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorMovies.Components.Helpers
{
    public static class IJSRuntimeExtensionMethods
    {
        public static async ValueTask InitializeInactivityTimer<T>(this IJSRuntime js,
                                    DotNetObjectReference<T> dotNetObjectReference) where T : class
        {
            Console.WriteLine("Calling 'initializeInactivityTimer' from js");
            await js.InvokeVoidAsync("initializeInactivityTimer", dotNetObjectReference);
        }

        public static async ValueTask<bool> Confirm(this IJSRuntime js, string message)
        {
            await js.InvokeVoidAsync("console.log", "A loging example");
            return await js.InvokeAsync<bool>("confirm", message);
        }

        public static async ValueTask My_Function(this IJSRuntime js, string message)
        {
            await js.InvokeVoidAsync("my_function", message);
        }

        public static ValueTask<object> SetInLocalStorage(this IJSRuntime js, string key, string content)
            => js.InvokeAsync<object>("localStorage.setItem", key, content);

        public static ValueTask<string> GetFromLocalStorage(this IJSRuntime js, string key)
            => js.InvokeAsync<string>("localStorage.getItem", key);

        public static ValueTask<object> RemoveItem(this IJSRuntime js, string key)
            => js.InvokeAsync<object>("localStorage.removeItem", key);
    }
}