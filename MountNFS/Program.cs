using AutomationLib;

CMD cmd = new();
string result = await cmd.RunCommandAsync("\"C:\\Program Files (x86)\\HP Gaming Mouse G160\\GamingMouse.exe\"");
Console.WriteLine(result);

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
// 让主线程不要退出
Console.ReadLine();