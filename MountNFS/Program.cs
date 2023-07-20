using AutomationLib;

#region 挂载
MountNFS mount = new();
string[] hosts = new string[]
{
	"192.168.8.8",
};

await mount.Mount(hosts);
#endregion

// 让主线程不要退出
Console.ReadLine();