using AutomationLib;

using CMD cmd = new();
/* 这里使用绝对路径来查找 StartUp.cmd 是因为相对路径 ./ 获得的路径会根据本程序被启动的位置变化
 * 而变化。例如在一个别的目录下用 shell 启动本程序，则 ./ 指向的是 shell 的工作目录，而不是本
 * 程序的 exe 文件所在的目录
 */
using FileStream cmdFile = File.Open(@"C:\msys64\home\huang\my_app\StartUp.cmd", FileMode.Open, FileAccess.Read, FileShare.Read);
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
