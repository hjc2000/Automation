rem 这个文件是被 StartUp.exe 读取的。会读取里面的每一行命令，调用 cmd 执行。

rem MountNFS.exe 需要使用 CMD 进程，必须要用 start 命令启动
start C:\my_app\MountNFS\MountNFS.exe