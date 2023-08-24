#region 挂载
using MountNFS;

Mounter mount = new();
string[] hosts = new string[]
{
	"192.168.8.8",
};
await mount.Mount(hosts);
#endregion

Console.WriteLine("完成");
Console.ReadLine();
