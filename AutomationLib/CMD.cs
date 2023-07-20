using StringLib;
using System.Diagnostics;
using System.Text;

namespace AutomationLib;

public class CMD : IDisposable
{
	#region 生命周期
	public CMD()
	{
		_process = new Process();

		#region 进程配置
		_process.StartInfo.FileName = @"CMD.exe";
		_process.StartInfo.Arguments = "/q /k @echo off";
		// 直接从可执行文件启动它，不使用shell启动它
		_process.StartInfo.UseShellExecute = false;
		// 不创建窗口
		_process.StartInfo.CreateNoWindow = true;
		// 重定向输入输出
		_process.StartInfo.RedirectStandardInput = true;
		_process.StartInfo.RedirectStandardOutput = true;
		_process.StartInfo.RedirectStandardError = true;
		// 订阅事件
		_process.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
		{
			string data = e.Data ?? string.Empty;
			ReceiveData(data);
		};
		#endregion

		_process.Start();
		_process.BeginOutputReadLine();
	}

	~CMD()
	{
		Dispose();
	}

	public void Dispose()
	{
		_process.Close();
		_process.Dispose();
	}
	#endregion

	#region 私有字段、属性
	private Process _process { get; set; }
	private Queue<Action<string>> _callbackQueue = new();
	private Semaphore _semaphore = new(1, 1);
	#endregion

	#region 私有方法
	private StringBuilder _receiveStringBuilder = new();
	private int _flag = 0;
	/// <summary>
	/// 在回调函数中接收 CMD 进程传来的数据
	/// </summary>
	/// <param name="data"></param>
	private void ReceiveData(string data)
	{
		#region 检测帧开始、结尾
		if (data.StartsWith('{'))
		{
			// 开始接收
			_receiveStringBuilder.Clear();
			_flag = 1;
		}
		else if (data.StartsWith('}'))
		{
			// 结束接收
			_flag = 2;
		}
		#endregion

		switch (_flag)
		{
		case 1:
			{
				_receiveStringBuilder.AppendLine(data);
				break;
			}
		case 2:
			{
				_receiveStringBuilder.AppendLine(data);
				string result = _receiveStringBuilder.ToString();

				// 接收完成
				result = result.SliceMaxBetween('{', '}') ?? string.Empty;
				result = result.Trim();
				DistributeData(result);
				_flag = 0;
				break;
			}
		}
	}

	private void DistributeData(string receiveStr)
	{
		lock (_callbackQueue)
		{
			Action<string> action = _callbackQueue.Dequeue();
			action?.Invoke(receiveStr);
		}
	}

	private async Task RunCommandAsync(string cmd, Action<string> callback)
	{
		// 禁止多线程同时向CMD发送命令，会串在一起
		_semaphore.WaitOne();
		_callbackQueue.Enqueue(callback);
		await _process.StandardInput.WriteLineAsync("echo {");
		await _process.StandardInput.WriteLineAsync(cmd);
		await _process.StandardInput.WriteLineAsync("echo }");
		await _process.StandardInput.FlushAsync();
		_semaphore.Release();
	}
	#endregion

	#region 公共 RunCommandAsync 重载
	public async Task<string> RunCommandAsync(string cmd)
	{
		Semaphore semaphore = new(0, 1);
		string result = string.Empty;
		await RunCommandAsync(cmd, (str) =>
		{
			result = str;
			semaphore.Release();
		});
		semaphore.WaitOne();
		return result;
	}

	public async Task<string[]> RunCommandAsync(string[] cmds)
	{
		string[] results = new string[cmds.Length];
		for (int i = 0; i < cmds.Length; i++)
		{
			results[i] = await RunCommandAsync(cmds[i]);
		}

		return results;
	}
	#endregion
}
