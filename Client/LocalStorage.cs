using System;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace TestScriptsWeb.Client
{
    public class LocalStorageService
    {
        private readonly IJSRuntime _jSRuntime;
        private readonly IJSInProcessRuntime _jSInProcessRuntime;

        public LocalStorageService(IJSRuntime jSRuntime)
        {
            _jSRuntime = jSRuntime;
            _jSInProcessRuntime = jSRuntime as IJSInProcessRuntime;
        }

        public async Task SetItemAsync<T>(string key, T data)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            var e = await RaiseOnChangingAsync(key, data);

            if (e.Cancel)
                return;

            await _jSRuntime.InvokeAsync<object>("localStorage.setItem", key,
                JsonConvert.SerializeObject(data));

            RaiseOnChanged(key, e.OldValue, data);
        }

        public async Task<T> GetItemAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            var serialisedData = await _jSRuntime.InvokeAsync<string>("localStorage.getItem", key);

            if (string.IsNullOrWhiteSpace(serialisedData))
                return default;

            return JsonConvert.DeserializeObject<T>(serialisedData);
        }

        public async Task RemoveItemAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            await _jSRuntime.InvokeAsync<object>("localStorage.removeItem", key);
        }

        public async Task ClearAsync() => await _jSRuntime.InvokeAsync<object>("localStorage.clear");

        public async Task<int> LengthAsync() => await _jSRuntime.InvokeAsync<int>("eval", "localStorage.length");

        public async Task<string> KeyAsync(int index) =>
            await _jSRuntime.InvokeAsync<string>("localStorage.key", index);

        public async Task<bool> ContainKeyAsync(string key) =>
            await _jSRuntime.InvokeAsync<bool>("localStorage.hasOwnProperty", key);

        public void SetItem<T>(string key, T data)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            if (_jSInProcessRuntime == null)
                throw new InvalidOperationException("IJSInProcessRuntime not available");

            var e = RaiseOnChangingSync(key, data);

            if (e.Cancel)
                return;

            _jSInProcessRuntime.Invoke<object>("localStorage.setItem", key,
                JsonConvert.SerializeObject(data));

            RaiseOnChanged(key, e.OldValue, data);
        }

        public T GetItem<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            if (_jSInProcessRuntime == null)
                throw new InvalidOperationException("IJSInProcessRuntime not available");

            var serialisedData = _jSInProcessRuntime.Invoke<string>("localStorage.getItem", key);

            if (string.IsNullOrWhiteSpace(serialisedData))
                return default;

            return JsonConvert.DeserializeObject<T>(serialisedData);
        }

        public void RemoveItem(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            if (_jSInProcessRuntime == null)
                throw new InvalidOperationException("IJSInProcessRuntime not available");

            _jSInProcessRuntime.Invoke<object>("localStorage.removeItem", key);
        }

        public void Clear()
        {
            if (_jSInProcessRuntime == null)
                throw new InvalidOperationException("IJSInProcessRuntime not available");

            _jSInProcessRuntime.Invoke<object>("localStorage.clear");
        }

        public int Length()
        {
            if (_jSInProcessRuntime == null)
                throw new InvalidOperationException("IJSInProcessRuntime not available");

            return _jSInProcessRuntime.Invoke<int>("eval", "localStorage.length");
        }

        public string Key(int index)
        {
            if (_jSInProcessRuntime == null)
                throw new InvalidOperationException("IJSInProcessRuntime not available");

            return _jSInProcessRuntime.Invoke<string>("localStorage.key", index);
        }

        public bool ContainKey(string key)
        {
            if (_jSInProcessRuntime == null)
                throw new InvalidOperationException("IJSInProcessRuntime not available");

            return _jSInProcessRuntime.Invoke<bool>("localStorage.hasOwnProperty", key);
        }

        public event EventHandler<ChangingEventArgs> Changing;

        private async Task<ChangingEventArgs> RaiseOnChangingAsync(string key, object data)
        {
            var e = new ChangingEventArgs
            {
                Key = key,
                OldValue = await GetItemAsync<object>(key),
                NewValue = data
            };

            Changing?.Invoke(this, e);

            return e;
        }

        private ChangingEventArgs RaiseOnChangingSync(string key, object data)
        {
            var e = new ChangingEventArgs
            {
                Key = key,
                OldValue = this.GetItem<object>(key),
                NewValue = data
            };

            Changing?.Invoke(this, e);

            return e;
        }

        public event EventHandler<ChangedEventArgs> Changed;

        private void RaiseOnChanged(string key, object oldValue, object data)
        {
            var e = new ChangedEventArgs
            {
                Key = key,
                OldValue = oldValue,
                NewValue = data
            };

            Changed?.Invoke(this, e);
        }
    }

    public class ChangingEventArgs : ChangedEventArgs
    {
        public bool Cancel { get; set; }
    }

    public class ChangedEventArgs
    {
        public string Key { get; set; }
        public object OldValue { get; set; }
        public object NewValue { get; set; }
    }
}

