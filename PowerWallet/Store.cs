using RapidBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PowerWallet
{
    public interface IStorage
    {
        Task<T> Get<T>(string name);
        Task Put<T>(string name, T obj);
    }
    public class LocalStorage : IStorage
    {
        string _Path;
        public LocalStorage(string path = null)
        {
            if (path == null)
                path = "LocalStorage";
            _Path = path;
        }

        public LocalStorage CreateSubStorage(string path)
        {
            return new LocalStorage(_Path + "-" + path);
        }
        private async Task<Stream> GetAccountsFile(string name, FileMode fileMode, FileAccess access, FileShare fileShare)
        {
            var localStorage = GetStorage();
            return await DoAsync(() => localStorage.OpenFile(GetFullName(name), fileMode, access, fileShare)).ConfigureAwait(false);
        }

        async Task<T> DoAsync<T>(Func<T> act)
        {
            try
            {
                return act();
            }
            catch (Exception)
            {

            }
            await Task.Delay(1000).ConfigureAwait(false);
            return act();
        }

        private string GetFullName(string name)
        {
            return _Path + "-" + name;
        }



        public string[] GetFiles()
        {
            var localStorage = GetStorage();
            return localStorage.GetFileNames(GetFullName("") + "*");
        }

        private IsolatedStorageFile GetStorage()
        {
            return IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain, null, null);
        }

        public Task Clear()
        {
            var localStorage = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain, null, null);
            foreach (var file in localStorage.GetFileNames(GetFullName("") + "*"))
            {
                localStorage.DeleteFile(file);
            }
            return Task.FromResult(true);
        }

        #region IDataRepository Members

        public async Task Put<TSource>(string key, TSource data)
        {
            if (key == null)
                key = typeof(TSource).Name;
            if (object.Equals(data, default(TSource)))
            {
                await Delete(key).ConfigureAwait(false);
                return;
            }
            using (var fs = await GetAccountsFile(key, FileMode.Create, FileAccess.Write, FileShare.None).ConfigureAwait(false))
            {
                var serialized = Serializer.ToString(data);
                var writer = new StreamWriter(fs);
                await writer.WriteAsync(serialized).ConfigureAwait(false);
                await writer.FlushAsync().ConfigureAwait(false);
            }
        }

        private async Task Delete(string key)
        {
            var storage = GetStorage();
            await DoAsync(() =>
            {
                storage.DeleteFile(GetFullName(key));
                return true;
            }).ConfigureAwait(false);
        }

        public async Task<TSource> Get<TSource>(string key)
        {
            if (key == null)
                key = typeof(TSource).Name;
            using (var fs = await GetAccountsFile(key, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read).ConfigureAwait(false))
            {
                var reader = new StreamReader(fs);
                var txt = await reader.ReadToEndAsync().ConfigureAwait(false);
                if (String.IsNullOrEmpty(txt))
                    return default(TSource);
                try
                {
                    return Serializer.ToObject<TSource>(txt);
                }
                catch (Exception)
                {
                    return default(TSource);
                }
            }
        }

        #endregion
    }
}
