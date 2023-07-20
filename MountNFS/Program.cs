using AutomationLib;
using StringLib.Parse;

#region 挂载
//MountNFS mount = new();
//string[] hosts = new string[]
//{
//	"192.168.8.8",
//};

//await mount.Mount(hosts);
#endregion

CMD cmd = new();
List<Task<string>> tasks = new();
for (int i = 0; i < 100; i++)
{
	tasks.Add(cmd.RunCommandAsync($"echo {i}"));
}

int max = -1;
while (tasks.Count > 0)
{
	Task<string> result = await Task.WhenAny(tasks);
	tasks.Remove(result);
	if (result.Result.ToInt32() > max)
	{
		max = result.Result.ToInt32();
		Console.WriteLine(max);
	}
	else
	{
		Console.WriteLine("没有按顺序");
		break;
	}
}

// 让主线程不要退出
Console.ReadLine();