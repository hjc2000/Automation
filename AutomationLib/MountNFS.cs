using System.Net.NetworkInformation;

namespace AutomationLib;

public class MountNFS
{
	private CMD cmd = new();

	/// <summary>
	/// 检查特定 IP 的主机是否存活
	/// </summary>
	/// <param name="ip"></param>
	/// <returns></returns>
	public static async Task<bool> IsAlive(string ip)
	{
		Ping ping = new();
		PingReply reply = await ping.SendPingAsync(ip);
		return reply.Status == IPStatus.Success;
	}

	/// <summary>
	/// 等待特定 IP 的主机直到对方存活
	/// </summary>
	/// <param name="ip"></param>
	/// <returns></returns>
	public static async Task WaitUntilAlive(string ip)
	{
		while (true)
		{
			try
			{
				if (await IsAlive(ip))
				{
					break;
				}
				else
				{
					Console.WriteLine($"IP 为 {ip} 的主机不在线");
				}
			}
			catch
			{
				Console.WriteLine("无法 ping 远程主机，检查网络连接");
			}

			await Task.Delay(2000);
		}
	}

	#region 挂载方法的重载
	public async Task Mount(string ip)
	{
		Console.WriteLine("正在检查网络连接");
		await WaitUntilAlive(ip);
		try
		{
			string cmdResult = await cmd.RunCommandAsync($"showmount -e {ip}");
			using StringReader reader = new(cmdResult);
			await reader.ReadLineAsync();
			int startIndex = cmdResult.IndexOf('/');
			int endIndex = cmdResult.IndexOf(' ', startIndex);
			cmdResult = cmdResult[startIndex..endIndex];
			Console.WriteLine($"即将挂载：{ip}:{cmdResult}");
			// -o mtype=hard
			cmdResult = await cmd.RunCommandAsync($"mount {ip}:{cmdResult} *");
			Console.WriteLine(cmdResult);
		}
		catch
		{
			Console.WriteLine("尝试挂载时发生异常");
		}
	}

	public async Task Mount(string[] ipArr)
	{
		List<Task> tasks = new();
		foreach (string ip in ipArr)
		{
			tasks.Add(Mount(ip));
		}

		await Task.WhenAll(tasks);
	}
	#endregion
}
