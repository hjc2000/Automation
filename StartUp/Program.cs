using AutomationLib;

await using CMD cmd = new();
string fullPath = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName ?? string.Empty;
string dir = Path.GetDirectoryName(fullPath) ?? string.Empty;
using FileStream cmdFile = File.Open($"{dir}\\StartUp.cmd", FileMode.Open, FileAccess.Read, FileShare.Read);
using StreamReader sr = new(cmdFile);

try
{
	while (true)
	{
		string? cmdStr = await sr.ReadLineAsync();
		if (cmdStr == null)
		{
			break;
		}
		else
		{
			cmdStr = cmdStr.Trim();
			// 空命令不执行
			if (cmdStr != string.Empty)
			{
				Console.WriteLine($"执行：{cmdStr}");
				Console.WriteLine(await cmd.RunCommandAsync(cmdStr));
			}
		}
	}
}
catch (Exception e)
{
	Console.WriteLine(e.ToString());
}
