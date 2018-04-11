using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using IoTHub.UWP.Models;

namespace IoTHub.UWP.Helpers
{
	public static class SettingsStorageExtensions
	{
		private const string FileExtension = ".json";

		public static bool IsRoamingStorageAvailable(this ApplicationData appData) => appData.RoamingStorageQuota == 0;

		public static async Task SaveAsync<T>(this StorageFolder folder, string name, T content)
		{
			var file = await folder.CreateFileAsync(GetFileName(name), CreationCollisionOption.OpenIfExists);
			var fileContent = await Json.StringifyAsync(content);

			await FileIO.WriteTextAsync(file, fileContent);
		}

		public static async Task AppendToFileAsync(this StorageFolder folder, string name, DeviceOutputModel content)
		{
			var file = await folder.CreateFileAsync(GetFileName(name), CreationCollisionOption.OpenIfExists);
			var source = await FileIO.ReadTextAsync(file);
			if (source == string.Empty)
			{
				var list = new List<DeviceOutputModel> {content};
				var fcontent = await Json.StringifyAsync(list);
				await FileIO.WriteTextAsync(file, fcontent);
			}
			else
			{
				var fileContent = await AddObjectToJsonAsync(source, content);
				await FileIO.WriteTextAsync(file, fileContent);
			}
		}

		private static async Task<string> AddObjectToJsonAsync(string json, DeviceOutputModel obj)
		{
			var list = await Json.ToObjectAsync<List<DeviceOutputModel>>(json);
			if (list.Count > 20)
			{
				var temp = list.OrderBy(x => x.Timestamp).TakeLast(9).ToList();
				list = temp;
			}
			list.Add(obj);
			return await Json.StringifyAsync(list);
		}


		public static async Task<T> ReadAsync<T>(this StorageFolder folder, string name)
		{
			if (!File.Exists(Path.Combine(folder.Path, GetFileName(name)))) return default(T);

			var file = await folder.GetFileAsync($"{name}.json");
			var fileContent = await FileIO.ReadTextAsync(file);

			return await Json.ToObjectAsync<T>(fileContent);
		}

		public static async Task SaveAsync<T>(this ApplicationDataContainer settings, string key, T value) =>
			settings.SaveString(key, await Json.StringifyAsync(value));

		private static void SaveString(this ApplicationDataContainer settings, string key, string value)
		{
			settings.Values[key] = value;
		}

		public static async Task<T> ReadAsync<T>(this ApplicationDataContainer settings, string key)
		{
			if (settings.Values.TryGetValue(key, out var obj)) return await Json.ToObjectAsync<T>((string) obj);

			return default(T);
		}

		public static async Task<StorageFile> SaveFileAsync(this StorageFolder folder, byte[] content, string fileName,
			CreationCollisionOption options = CreationCollisionOption.ReplaceExisting)
		{
			if (content == null) throw new ArgumentNullException(nameof(content));

			if (string.IsNullOrEmpty(fileName))
				throw new ArgumentException("ExceptionSettingsStorageExtensionsFileNameIsNullOrEmpty".GetLocalized(),
					nameof(fileName));

			var storageFile = await folder.CreateFileAsync(fileName, options);
			await FileIO.WriteBytesAsync(storageFile, content);
			return storageFile;
		}

		public static async Task<byte[]> ReadFileAsync(this StorageFolder folder, string fileName)
		{
			var item = await folder.TryGetItemAsync(fileName).AsTask().ConfigureAwait(false);

			if (item == null || !item.IsOfType(StorageItemTypes.File)) return null;
			var storageFile = await folder.GetFileAsync(fileName);
			var content = await storageFile.ReadBytesAsync();
			return content;
		}

		private static async Task<byte[]> ReadBytesAsync(this IRandomAccessStreamReference file)
		{
			if (file == null) return null;
			using (IRandomAccessStream stream = await file.OpenReadAsync())
			{
				using (var reader = new DataReader(stream.GetInputStreamAt(0)))
				{
					await reader.LoadAsync((uint) stream.Size);
					var bytes = new byte[stream.Size];
					reader.ReadBytes(bytes);
					return bytes;
				}
			}
		}

		private static string GetFileName(string name) => string.Concat(name, FileExtension);
	}
}