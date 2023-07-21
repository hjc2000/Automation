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

#region 挂载
MountNFS mount = new();
string[] hosts = new string[]
{
	"192.168.8.8",
};
await mount.Mount(hosts);
#endregion

#region 测试并发执行任务时是否会导致输入输出不对应
//CMD cmd = new();
//List<Task<string>> tasks = new();
//for (int i = 0; i < 10000; i++)
//{
//	tasks.Add(cmd.RunCommandAsync($"echo {i}"));
//}

//string[] results = await Task.WhenAll(tasks);
//int count = 0;
//foreach (string result in results)
//{
//	if (count++ != result.ToInt32())
//	{
//		Console.WriteLine("没按顺序");
//		Console.WriteLine(result);
//		break;
//	}
//}
#endregion

Console.WriteLine("完成");
