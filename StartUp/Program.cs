using AutomationLib;

using CMD cmd = new();
string[] cmdStrs = new[]
{
	"\"C:\\Program Files\\LGHUB\\lghub.exe\"",
};

foreach (string cmdStr in cmdStrs)
{
	Console.WriteLine($"执行：{cmdStr}");
	Console.WriteLine(await cmd.RunCommandAsync(cmdStr));
}

//using FileStream cmdFile = File.Open("./cmd.bat", FileMode.Open, FileAccess.Read, FileShare.Read);
//using StreamReader sr = new(cmdFile);
//while (true)
//{
//	string? cmdStr = await sr.ReadLineAsync();
//	if (cmdStr == null)
//	{
//		break;
//	}
//	else
//	{
//		cmdStr = cmdStr.Trim();
//		// 空命令不执行
//		if (cmdStr != string.Empty)
//		{
//			Console.WriteLine(cmdStr);
//			Console.WriteLine(await cmd.RunCommandAsync(cmdStr));
//		}
//	}
//}
