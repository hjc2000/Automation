﻿using AutomationLib;

using CMD cmd = new();
using FileStream cmdFile = File.Open(@"C:\my_app\StartUp.cmd", FileMode.Open, FileAccess.Read, FileShare.Read);
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
