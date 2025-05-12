using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestBenchTarget.UWP.Models;
using Windows.Storage;

namespace TestBenchTarget.UWP.Services
{
    public class DataService
    {
        private CustomObservableCollection<DataItem> dataList = new CustomObservableCollection<DataItem>();
        public CustomObservableCollection<DataItem> DataList => dataList;

        private const string _fileName = "TestBenchTarget.json";

        public async Task<bool> SaveDataAsync()
        {
            try
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                StorageFile file = await localFolder.CreateFileAsync(_fileName,
                                    CreationCollisionOption.ReplaceExisting);
                string jsonData = JsonConvert.SerializeObject(dataList, Formatting.Indented);
                await FileIO.WriteTextAsync(file, jsonData);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in SaveDataAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> LoadDataAsync()
        {
            try
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                try
                {
                    StorageFile file = await localFolder.GetFileAsync(_fileName);
                    string jsonData = await FileIO.ReadTextAsync(file);
                    var loadedData = JsonConvert.DeserializeObject<List<DataItem>>(jsonData);
                    dataList.Clear();
                    if (loadedData != null)
                    {
                        foreach (var item in loadedData)
                        {
                            dataList.Add(item);
                        }
                    }
                    return true;
                }
                catch (System.IO.FileNotFoundException)
                {
                    // Súbor ešte neexistuje, čo je v poriadku pri prvom spustení
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in LoadDataAsync: {ex.Message}");
                return false;
            }
        }

        public Task<StorageFolder> GetLocalFolderAsync()
        {
            return Task.FromResult(ApplicationData.Current.LocalFolder);
        }
    }
}
