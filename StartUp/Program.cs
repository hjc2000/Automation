using AutomationLib;

using CMD cmd = new();
using FileStream cmdFile = File.Open(@"C:\我的可执行文件\cmd.txt", FileMode.Open, FileAccess.Read, FileShare.Read);
using StreamReader sr = new(cmdFile);
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
