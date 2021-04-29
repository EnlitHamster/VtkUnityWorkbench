using System;
using System.IO;
using System.Text;
using UnityEngine;

public class UtilFileManager : GenericSingletonClass<UtilFileManager>
{
	public string OutputRootFolder = "C:\\3DHeartScreenCaptures";
	public string OutputSessionFolder;

	public UtilFileManager()
	{
		if (null == OutputSessionFolder || OutputSessionFolder.Length == 0)
		{
			const string FMT = "yyyy-MM-dd_HH.mm.ss.fff";
			DateTime now = DateTime.Now;
			OutputSessionFolder = now.ToString(FMT);
		}
	}

	public string OutputPathDateTimeName(string extension)
	{
		var name = new DirectoryInfo(DataStore.Instance.ImageDataFolder).Name;
		return OutputPathDateTimeName(name, extension);
	}

	public string OutputPathDateTimeName(string name, string extension)
	{
		var sessionFolder = System.IO.Path.Combine(OutputRootFolder, OutputSessionFolder);
		System.IO.Directory.CreateDirectory(sessionFolder);

		const string FMT = "yyyy-MM-dd_HH.mm.ss.fff";
		DateTime now = DateTime.Now;
		var filepath = now.ToString(FMT) + "_" + name + "." + extension;
		return (System.IO.Path.Combine(sessionFolder, filepath));
	}

	public void WriteTextBlockToFile(string path, string textBlock)
	{
		//Create the file.
		using (FileStream fs = File.Create(path))
		{
				AddText(fs, textBlock);
		}
	}

	public void AddText(FileStream fs, string value)
	{
		byte[] info = new UTF8Encoding(true).GetBytes(value);
		fs.Write(info, 0, info.Length);
	}

}
